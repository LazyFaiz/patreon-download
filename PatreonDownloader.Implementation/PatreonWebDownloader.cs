using System;
using System.Threading.Tasks;
using UniversalDownloaderPlatform.Common.Interfaces;
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


            Exception last = null;
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    await base.DownloadFile(url, path, refererUrl);
                    return;
                }
                catch (Exception ex) when (attempt < 3)
                {
                    last = ex;
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 2));
                }
            }
            throw last;
        }

        public override async Task<string> DownloadString(string url, string refererUrl = null)
        {
            if (string.IsNullOrWhiteSpace(refererUrl))
                refererUrl = "https://www.patreon.com";


            return await base.DownloadString(url, refererUrl);
        }
    }
}
