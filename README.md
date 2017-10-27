# Media Converter
Media Converter是一个可以转换媒体文件格式的UWP应用项目，应用名称暂定为“格式转换”。

应用使用UWP SDK中的Windows.Media.Transcoding这一API实现格式转换，理论上能够支持的输入、输出格式见[支持的编解码器 - Windows开发人员中心][1]。

## 安装

通过应用商店的“试用”功能，可以免费下载本应用。若希望支持本项目，请在应用商店中购买。

应用商店下载链接（最低版本：Windows 10 1511）：

<a href="https://www.microsoft.com/store/apps/9ndjv6k3g2tj?ocid=github"><img src="https://assets.windowsphone.com/1cfd01f7-aad6-4896-8eb7-fea5f600e42d/Chinese_Simplified_Get_it_Win_10_InvariantCulture_Default.png" width="150" alt="在 Windows 10 上获取" /></a>

[1]: https://msdn.microsoft.com/zh-cn/windows/uwp/audio-video-camera/supported-codecs

## 生成
在安装有以下软件的计算机上，可直接使用Visual Studio生成：
- Visual Studio 2017以上版本
- Windows 10 SDK 10.0.14393

dev分支在AppVeyor上的生成状态：
[![Build status](https://ci.appveyor.com/api/projects/status/l0ik2t7vl2qxw3yk/branch/dev?svg=true)](https://ci.appveyor.com/project/taoyouh/converter/branch/dev)
