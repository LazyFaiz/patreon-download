# Patreon Downloader

A command-line tool for downloading posts, files, images, and attachments from Patreon creator pages.

This repository is based on [AlexCSDev/PatreonDownloader](https://github.com/AlexCSDev/PatreonDownloader). Chinese documentation is available in [README.md](README.md).

> The project is under active development. Verify build and download behavior with your own account before production use.

## Features

- Download files, images, and attachments from creator pages.
- Save post descriptions, embed metadata, and API responses.
- Download campaign avatar and cover images.
- Configure download directories, post subdirectories, file naming, proxy, and logging.
- Retry failed media downloads up to three times.
- Single-post URL support is being finalized.

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

The planned single-post form is:

```text
https://www.patreon.com/posts/example-title-12345678
```

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

- Single-post crawling and video downloads still require validation against real authenticated posts.
- YouTube and imgur links are skipped; Vimeo, audio, and gallery posts require further testing.
- The application does not bypass Patreon access controls.

## Documentation

- [Chinese README](README.md)
- [Build instructions](docs/BUILDING.md)
- [Remote browser configuration](docs/REMOTEBROWSER.md)

## License

Unless stated otherwise, files in this repository are distributed under the [MIT License](LICENSE.md). Dependencies retain their own licenses.
