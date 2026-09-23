# Patreon Downloader

[中文](README.md) | English

A command-line tool for downloading posts, files, images, and attachments from Patreon creator pages.

This repository is based on [AlexCSDev/PatreonDownloader](https://github.com/AlexCSDev/PatreonDownloader). Chinese documentation is available in [README.md](README.md).

> The project is under active development. The code builds successfully, and a single image post has been downloaded with a real Patreon account. Video CDN downloads still require verification.

## Features

- Download files, images, and attachments from creator pages.
- Save post descriptions, embed metadata, and API responses.
- Download campaign avatar and cover images.
- Configure download directories, post subdirectories, file naming, proxy, and logging.
- Stream media to disk, follow redirects, resume with HTTP Range, validate response length, and retry failed downloads up to five times.
- Download a single post by its post URL.

Downloading requires a valid Patreon account and access to the requested content. Paid posts require valid access permission.

## Requirements

- Windows 10 1903 or newer, or Linux with OpenSSL 1.1.1 or newer.
- .NET 9 SDK to build from source.
- The `UniversalDownloaderPlatform` Git submodule.

Clone with submodules:

```powershell
git clone --recurse-submodules https://github.com/LazyFaiz/patreon-download.git
cd patreon-download
```

For an existing checkout:

```powershell
git submodule update --init --recursive
```

## Build and test

```powershell
dotnet restore PatreonDownloader.sln
dotnet build PatreonDownloader.sln -c Release
dotnet test PatreonDownloader.sln -c Release
```

The solution builds successfully. The test suite currently reports one failure in duplicate file naming and skips the URL-derived long filename case. The long PNG filename and extension test passes.

## Usage

Show help:

```powershell
.\PatreonDownloader.App.exe --help
```

Download a creator page:

```powershell
.\PatreonDownloader.App.exe --url "https://www.patreon.com/creator_name/posts"
```

Download to a custom directory and save metadata:

```powershell
.\PatreonDownloader.App.exe --url "https://www.patreon.com/creator_name/posts" --download-directory "D:\PatreonDownloads" --descriptions --embeds --campaign-images --json
```

Pass a post URL to `--url` and choose an output directory with `--download-directory`:

```powershell
.\PatreonDownloader.App.exe --url "https://www.patreon.com/duckie_cos/posts/yanfei-set-free-159054835" --download-directory "F:\Desktop\1"
```

The application searches the creator's paginated API results and downloads only the requested post. On first run, it opens a browser for Patreon login. If it continues waiting after login, stop and rerun the same command; the browser session is stored in `chromedata` in the application output directory.

Verified on 2026-09-23: the example post produced 26 nonempty JPEG files totaling 54,466,552 bytes, all named with the `159054835_` prefix. One post image duplicates an attachment image. This verifies an image post accessible to the test account; other post types still need testing.

## Common options

| Option | Description |
| --- | --- |
| `--url` | Creator page or single-post URL |
| `--download-directory` | Output directory |
| `--descriptions` | Save post description JSON |
| `--embeds` | Save embedded content metadata |
| `--campaign-images` | Download avatar and cover images |
| `--json` | Save API responses |
| `--use-sub-directories` | Create a directory for each post |
| `--log-level` | `Default`, `Debug`, or `Trace` |
| `--log-save` | Write logs to the `logs` directory |
| `--proxy-server-address` | HTTP or SOCKS proxy address |

## Current limitations

- Single-post image downloading has been validated with an authenticated account. Video downloads and other post types still require real-world validation.
- YouTube and imgur links are skipped; Vimeo, audio, and gallery posts require further testing.
- The application does not bypass Patreon access controls. Duplicate file naming still needs a fix: the second file with the same name does not receive a distinguishing suffix. The URL-derived long filename test is deferred.

## Documentation

- [Chinese README](README.md)
- [Build instructions](docs/BUILDING.md)
- [Remote browser configuration](docs/REMOTEBROWSER.md)

## License

Unless stated otherwise, files in this repository are distributed under the [MIT License](LICENSE.md). Dependencies retain their own licenses.
