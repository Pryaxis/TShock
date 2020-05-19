<p align="center">
  <img src="https://tshock.co/newlogo.png" alt="TShock for Terraria"><br />
  <a href="https://ci.appveyor.com/project/hakusaro/tshock"><img src="https://ci.appveyor.com/api/projects/status/chhe61q227lqdlg1?svg=true" alt="AppVeyor Build Status"></a><a href="https://github.com/Pryaxis/TShock/actions"><img src="https://github.com/Pryaxis/TShock/workflows/Build%20Server/badge.svg" alt="GitHub Actions Build Status"></a><a href="#contributors"><img src="https://img.shields.io/badge/all_contributors-1-orange.svg?style=flat-square" alt="All contributors"></a><br />
</p>

TShock是为泰拉瑞亚设计的多功能服务端。它拥有反作弊/强制开荒/用户组/权限管理/物品封禁/大量指令和无限的扩展性。

* 下载: [稳定版](https://github.com/TShock/TShock/releases) or [测试版](#experimental-downloads) 
* 使用方法请阅读 [文档](https://tshock.readme.io/) 
* 你可以加入 [我们的官方QQ群](https://jq.qq.com/?_wv=1027&k=5GJZCe4) 交流
* 也可以加入 [我们的Discord服务器](https://discord.gg/Cav9nYX) 提问
* 如果想要深度技术支持，可以加入 [我们的Telegram群](https://t.me/pryaxis) 
* 你可以在 [这里](https://tshock.co/xf/index.php?resources/) 下载插件增强你的服务器

----

## 内容索引

  * [第一次使用TShock?](#new-to-tshock)
  * [下载测试版](#experimental-downloads)

## 第一次使用TShock?

_这篇指南基于Windows。如果你在使用Unix或者Linux，请参考 [深度指南](https://tshock.readme.io/docs/getting-started) (不要忘记在你的Linux系统上安装 **最新版** 的 `mono-complete` )._

1. 下载 [最新稳定版](https://github.com/TShock/TShock/releases) 然后解压。解压后文件所在的文件夹就是你服务器的工作目录。文件夹结构大致如下:

      
          GeoIP.dat
          Newtonsoft.Json.dll
          OTAPI.dll
          ServerPlugins\
          |------BCrypt.Net.dll
          |------HttpServer.dll
          |------Mono.Data.Sqlite.dll
          |------MySql.Data.dll
          |------TShockAPI.dll
          TerrariaServer.exe
          sqlite3.dll
      

1. 运行 `TerrariaServer.exe` ，TShock就会启动了。 TShock会自动创建一些文件夹，具体用途稍后讨论。

1. 启动你的游戏，选择 `多人模式` 并选择 `通过IP加入`。输入 `localhost` 或者 `127.0.0.1` 如果你的服务器和游戏运行在同一台电脑上。如果你在用其他设备开服，你需要输入它的IP地址。

1. 查看服务器控制台上的 _验证码_。在游戏里打开聊天窗口输入 `/setup [验证码]` (举个例子: `/setup 12345`)然后回车。这条指令可以让你成为临时管理。 所有指令都需要以 `/` 或者 `!` 开头。

1. 在游戏里输入指令 `/user add [账号名] [密码] owner` (举个例子: `/user add 鱼鱼 真可爱 owner`) 来创建一个账号并且给这个账号服主权限。

1. 登录你刚刚创建的账号，方法是输入指令 `/login [账号名] [密码]` (举个例子: `/login 鱼鱼 真可爱`) 然后你就会看到登录成功的提示。

1. 输入指令 `/setup` 关闭初始化设置功能，因为你已经搞定了。TShock会在 `tshock` 文件夹内创建数个文件。包括 `config.json` (服务器配置文件), `sscconfig.json` (强制开荒配置文件) 和 `tshock.sqlite` (服务器数据库)。不要把 `tshock.sqlite` 搞丢了，不然就白折腾了。

1. 现在你可以 [调整配置](https://tshock.readme.io/docs/config-settings) ，创建用户组，封禁物品或者安装插件了。

## 下载测试版

想下载测试版的TShock，你有两个选择：AppVeyor或者GitHub。你也可以获取Travis CI上的旧版本。注意: 测试版的TShock理论上不受我们的支持。如果你遇到问题需要发Issue，请提前声明你的版本信息。

在 [AppVeyor](https://ci.appveyor.com/project/hakusaro/tshock/) 上，点击History，找到需要的版本并点击, 然后点击Artifacts就可以下载它的发布版或者调试版。AppVeyor只会保留半年内的版本。

在 [GitHub项目](https://github.com/Pryaxis/TShock/) 页面里，点击 `Actions`，然后点击你想要的branch的 `build server` 就可以下载它的发布版或者调试版。

关于Travis CI上的旧版本，现在还可以在 [我们的Travis CI产物镜像](https://travis.tshock.co/) 上获取。但是请注意这些旧版本已经不再受支持。

## Contributors

Thanks goes to these wonderful people ([emoji key](https://allcontributors.org/docs/en/emoji-key)):

<!-- ALL-CONTRIBUTORS-LIST:START - Do not remove or modify this section -->
<!-- prettier-ignore-start -->
<!-- markdownlint-disable -->
<table>
  <tr>
    <td align="center"><a href="https://avikav.net"><img src="https://avatars2.githubusercontent.com/u/18518861?v=4" width="100px;" alt=""/><br /><sub><b>AviKav</b></sub></a><br /><a href="https://github.com/Pryaxis/TShock/issues?q=author%3AAviKav" title="Bug reports">🐛</a> <a href="https://github.com/Pryaxis/TShock/commits?author=AviKav" title="Tests">⚠️</a></td>
    <td align="center"><a href="https://tshock.co"><img src="https://avatars0.githubusercontent.com/u/3332657?v=4" width="100px;" alt=""/><br /><sub><b>Rodrigo Rente</b></sub></a><br /><a href="https://github.com/Pryaxis/TShock/commits?author=AxisKriel" title="Code">💻</a> <a href="#projectManagement-AxisKriel" title="Project Management">📆</a> <a href="https://github.com/Pryaxis/TShock/commits?author=AxisKriel" title="Tests">⚠️</a></td>
    <td align="center"><a href="https://sgkoi.dev"><img src="https://avatars2.githubusercontent.com/u/9637711?v=4" width="100px;" alt=""/><br /><sub><b>Stargazing Koishi</b></sub></a><br /><a href="https://github.com/Pryaxis/TShock/commits?author=sgkoishi" title="Code">💻</a> <a href="#infra-sgkoishi" title="Infrastructure (Hosting, Build-Tools, etc)">🚇</a></td>
    <td align="center"><a href="https://github.com/AxeelAnder"><img src="https://avatars2.githubusercontent.com/u/25691207?v=4" width="100px;" alt=""/><br /><sub><b>Axeel</b></sub></a><br /><a href="https://github.com/Pryaxis/TShock/commits?author=AxeelAnder" title="Documentation">📖</a> <a href="#projectManagement-AxeelAnder" title="Project Management">📆</a></td>
    <td align="center"><a href="http://www.nathaneaston.com/"><img src="https://avatars2.githubusercontent.com/u/10368650?v=4" width="100px;" alt=""/><br /><sub><b>Nathan Easton</b></sub></a><br /><a href="https://github.com/Pryaxis/TShock/commits?author=ndragon798" title="Code">💻</a></td>
  </tr>
</table>

<!-- markdownlint-enable -->
<!-- prettier-ignore-end -->
<!-- ALL-CONTRIBUTORS-LIST:END -->

This project follows the [all-contributors](https://github.com/all-contributors/all-contributors) specification. Contributions of any kind welcome!
