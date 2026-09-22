using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UniversalDownloaderPlatform.Common.Interfaces;
using UniversalDownloaderPlatform.Common.Exceptions;
using UniversalDownloaderPlatform.DefaultImplementations;
using UniversalDownloaderPlatform.DefaultImplementations.Interfaces;

namespace PatreonDownloader.Implementation
{
    internal class PatreonWebDownloader : WebDownloader
    {
        public PatreonWebDownloader(IRemoteFileSizeChecker remoteFileSizeChecker, ICaptchaSolver captchaSolver) : base(remoteFileSizeChecker, captchaSolver)
        {

        }

        public override async Task DownloadFile(string url, string path, string refererUrl = null)
        {
            if (string.IsNullOrWhiteSpace(refererUrl))
                refererUrl = "https://www.patreon.com";


            string temporaryPath = $"{path}.dwnldtmp";
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? ".");

            if (File.Exists(path) && _fileExistsAction == UniversalDownloaderPlatform.Common.Enums.FileExistsAction.KeepExisting)
                return;

            Exception last = null;
            for (int attempt = 1; attempt <= 5; attempt++)
            {
                try
                {
                    long downloaded = File.Exists(temporaryPath) ? new FileInfo(temporaryPath).Length : 0;
                    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                    {
                        if (!string.IsNullOrWhiteSpace(refererUrl))
                            request.Headers.Referrer = new Uri(refererUrl);
                        if (downloaded > 0)
                            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(downloaded, null);

                        using (HttpResponseMessage response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                        {
                            if (downloaded > 0 && response.StatusCode == System.Net.HttpStatusCode.RequestedRangeNotSatisfiable)
                            {
                                File.Delete(temporaryPath);
                                continue;
                            }

                            response.EnsureSuccessStatusCode();
                            bool append = downloaded > 0 && response.StatusCode == System.Net.HttpStatusCode.PartialContent;
                            if (!append && downloaded > 0)
                            {
                                File.Delete(temporaryPath);
                                downloaded = 0;
                            }

                            using (Stream input = await response.Content.ReadAsStreamAsync())
                            using (FileStream output = new FileStream(temporaryPath, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, true))
                            {
                                await input.CopyToAsync(output);
                            }

                            long expected = response.Content.Headers.ContentLength ?? -1;
                            if (append && response.Content.Headers.ContentRange?.Length is long rangeLength)
                                expected = rangeLength;
                            long actual = new FileInfo(temporaryPath).Length;
                            if (expected >= 0 && actual < expected)
                                throw new IOException($"Video response ended early ({actual}/{expected} bytes)");
                        }
                    }

                    File.Move(temporaryPath, path, true);
                    return;
                }
                catch (Exception ex) when (attempt < 5)
                {
                    last = ex;
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
                }
            }
            throw new DownloadException($"Unable to download media after 5 attempts: {url}", last);
        }

        public override async Task<string> DownloadString(string url, string refererUrl = null)
        {
            if (string.IsNullOrWhiteSpace(refererUrl))
                refererUrl = "https://www.patreon.com";


            return await base.DownloadString(url, refererUrl);
        }
    }
}
