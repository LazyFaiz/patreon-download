# Patreon Downloader

[English](README.en.md) | 中文

用于下载 Patreon 创作者发布的帖子文件、图片和附件，支持保存正文、嵌入内容元数据以及 API 响应。

本项目基于 [AlexCSDev/PatreonDownloader](https://github.com/AlexCSDev/PatreonDownloader) 建立。

> 当前版本处于开发阶段。代码已可编译，单帖图片下载已使用真实 Patreon 账号验证；视频 CDN 下载仍需验证。程序参数和日志尚未中文化。

## 功能

- 按创作者页面抓取帖子中的文件、图片和附件。
- 可选保存帖子正文 JSON、嵌入内容元数据和 API 响应。
- 可选下载创作者头像、封面。
- 支持自定义下载目录、按帖子创建子目录和文件命名规则。
- 支持代理和远程浏览器配置。
- 媒体下载失败后最多尝试 3 次，前两次失败分别等待 2 秒和 4 秒。
- 支持按单个 post URL 下载目标帖子。

下载需要有效的 Patreon 账号；付费内容需要账号拥有相应访问权限。

## 当前限制

- 单个帖子下载已实现：程序会遍历创作者 API 分页，只处理 URL 中指定的 post ID。已对一个有权限的图片帖子完成实际下载验证。
- 视频下载已加入流式写入、重定向、临时文件保留、HTTP Range 续传、完整性检查和最多 5 次重试。
- 项目依赖 `UniversalDownloaderPlatform` 子模块。首次克隆后请执行 `git submodule update --init --recursive`。
- 正文外链提取因 Patreon 正文格式变更已在代码中停用。
- YouTube 和 imgur 链接目前会被跳过；Vimeo 视频、音频和图库仍需验证。
- 本地构建与真实登录后的单帖图片下载已验证。文件名测试中，超长 PNG 文件名及扩展名保留测试通过；同名文件测试发现第二个同名文件未追加区分后缀；超长 URL 作为文件名的测试暂缓执行。其他帖子类型仍需测试。

## 开发环境

项目目标框架为 `net9.0`，构建需要 .NET 9 SDK。程序的浏览器登录流程还依赖 Chromium 相关组件；可参考[远程浏览器说明](docs/REMOTEBROWSER.md)。

先检查 SDK：

```powershell
dotnet --list-sdks
```

如果提示 `No .NET SDKs were found`，需要先安装 SDK；仅安装运行时不能编译项目。

克隆仓库：

```powershell
git clone https://github.com/LazyFaiz/patreon-download.git
cd patreon-download
```

本项目依赖 `submodules/UniversalDownloaderPlatform`。首次克隆后请执行 `git submodule update --init --recursive`，再执行以下构建命令。关于原项目构建流程，可参考[构建说明（英文）](docs/BUILDING.md)。

```powershell
dotnet restore PatreonDownloader.sln
dotnet build PatreonDownloader.sln -c Release
dotnet test PatreonDownloader.sln -c Release
```

> 当前构建可以成功。测试套件仍包含一项同名文件失败测试和一项暂缓执行的超长 URL 文件名测试；运行 `dotnet test` 时会报告失败。

## 使用示例

以下示例适用于完成构建后的程序。在程序输出目录打开 PowerShell 执行；Linux 可使用 `dotnet PatreonDownloader.App.dll` 替换可执行文件名。

### 查看帮助

```powershell
.\PatreonDownloader.App.exe --help
```

### 下载创作者页面

```powershell
.\PatreonDownloader.App.exe --url "https://www.patreon.com/creator_name/posts"
```

原项目支持的页面格式包括：

- `https://www.patreon.com/m/123456/posts`
- `https://www.patreon.com/user?u=123456`
- `https://www.patreon.com/user/posts?u=123456`
- `https://www.patreon.com/creator_name/posts`

请将示例中的用户名和数字替换为实际页面信息。

### 指定目录并保存附加信息

```powershell
.\PatreonDownloader.App.exe --url "https://www.patreon.com/creator_name/posts" --download-directory "D:\PatreonDownloads" --descriptions --embeds --campaign-images --json
```

### 为每个帖子创建子目录

```powershell
.\PatreonDownloader.App.exe --url "https://www.patreon.com/creator_name/posts" --use-sub-directories --sub-directory-pattern "[%PostId%] %PublishedAt% %PostTitle%"
```

### 单个帖子下载

使用 `--url` 参数传入帖子链接，并用 `--download-directory` 指定保存目录，例如：

```powershell
.\PatreonDownloader.App.exe --url "https://www.patreon.com/duckie_cos/posts/yanfei-set-free-159054835" --download-directory "F:\Desktop\1"
```

程序会从创作者 API 分页中定位该 post，并只下载目标帖子的内容。首次运行会打开浏览器要求登录 Patreon；登录后如果程序仍在等待，可结束程序并重新运行相同命令，浏览器会话会保存在程序输出目录的 `chromedata` 中。

2026-09-23 实测：上述帖子下载完成 26 张 JPEG 图片，共 54,466,552 字节。文件均非空，文件名均以 `159054835_` 开头；其中 1 张帖子封面图片与附件图片重复。此结果只验证了该账号有权访问的图片帖子。

## 常用参数

| 参数 | 说明 |
| --- | --- |
| `--url` | 必填，创作者页面或单帖 URL |
| `--download-directory` | 指定下载目录 |
| `--descriptions` | 保存帖子正文 JSON |
| `--embeds` | 保存嵌入内容元数据，不代表下载嵌入视频 |
| `--campaign-images` | 下载创作者头像和封面 |
| `--json` | 保存 API 响应，方便排查问题 |
| `--use-sub-directories` | 为每个帖子创建子目录 |
| `--sub-directory-pattern` | 子目录命名模板 |
| `--max-sub-directory-name-length` | 子目录名称长度限制，默认 100 |
| `--max-filename-length` | 文件名长度限制，默认 100 |
| `--file-exists-action` | 已有文件处理策略，见下表 |
| `--disable-remote-file-size-check` | 禁用远程文件大小预检查 |
| `--log-level` | 日志级别：`Default`、`Debug` 或 `Trace` |
| `--log-save` | 将日志写入 `logs` 目录 |
| `--proxy-server-address` | 代理地址，例如 `http://127.0.0.1:7890` |
| `--remote-browser-address` | 启用远程调试的浏览器地址，详见专门文档 |

已有文件处理策略：

| 值 | 行为 |
| --- | --- |
| `BackupIfDifferent` | 默认策略；内容不同时保留旧文件备份 |
| `ReplaceIfDifferent` | 内容不同时替换旧文件 |
| `AlwaysReplace` | 始终替换 |
| `KeepExisting` | 保留已有文件 |

## 问题排查

下载失败时，可启用调试日志：

```powershell
.\PatreonDownloader.App.exe --url "https://www.patreon.com/creator_name/posts" --log-level Debug --log-save
```

- **无法访问帖子**：确认账号已登录，并且能够在浏览器中查看目标内容。
- **403 Forbidden**：检查登录状态、网络以及代理设置；账号能够查看页面不代表下载请求一定成功。
- **视频下载失败**：程序会保留 `.dwnldtmp` 临时文件，并在重试时尝试 Range 续传；如果 CDN 不支持续传，会自动重新开始。失效地址、无权限内容或未支持的媒体格式仍无法下载。
- **找不到引用项目**：检查 `submodules/UniversalDownloaderPlatform` 是否包含完整依赖源码。

提交问题时请提供运行命令、系统信息和相关错误片段，并删除 Cookie、令牌及其他凭据。

## 其他文档

- [构建说明（英文）](docs/BUILDING.md)
- [远程浏览器配置（英文）](docs/REMOTEBROWSER.md)
- [Google Drive 插件说明（英文）](docs/GOOGLEDRIVE.md)
- [Mega 插件说明（英文）](docs/MEGA.md)

## 许可证

除另有说明外，本仓库文件遵循 [MIT 许可证](LICENSE.md)。依赖项目遵循各自许可证。
