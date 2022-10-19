<p align="center">
  <img src="https://tshock.co/newlogo.png" alt="TShock for Terraria"><br />
  <a href="https://ci.appveyor.com/project/hakusaro/tshock">
    <img src="https://ci.appveyor.com/api/projects/status/chhe61q227lqdlg1?svg=true" alt="AppVeyor Build Status">
  </a>
  <a href="https://github.com/Pryaxis/TShock/actions">
    <img src="https://github.com/Pryaxis/TShock/actions/workflows/build.yml/badge.svg" alt="GitHub Actions Build Status">
  </a>
</p>

TShock是为泰拉瑞亚服务器和社区开发的一个工具箱。这个工具箱拥有反作弊、服务端存档、用户组、权限、物品禁止、大量指令和无限的可能性。它是独一无二的。

这是面向TShock开发者和修改者的说明。我们正在为服务器运营者和插件开发者建设新的[TShock文档](https://ikebukuro.tshock.co/)，但还在进行中。

## 进行TShock开发

如果你想通过PR给TShock贡献代码或者想按照你美妙的想法定制它，这里是最好的出发点。看完之后你就能独立从源码编译出TShock。不止这样，你还能知道如何成为一名出色的TShock开发者。

本指南假设你已经安装了[.NET 6 SDK](https://dotnet.microsoft.com/zh-cn/download/dotnet/6.0)并了解命令行。如果你不满足这些条件，你应该能通过Visual Studio 2022或者Visual Studio Code做到同样的事情。

1. 克隆仓库：`git clone https://github.com/Pryaxis/TShock.git --recurse-submodules`
1. 运行`cd TShock`来进入仓库文件夹
1. 运行`dotnet build`。不开玩笑，这样就编译完了


如果你想运行`TShockLauncher`（启动服务器），运行：

1. `dotnet run --project TShockLauncher`

如果要生成打包后的发行版，运行：

1. `cd TShockLauncher`
1. `dotnet publish -r win-x64 -f net6.0 -c Release -p:PublishSingleFile=true --self-contained false`

注意在这个例子中你将会生成`win-x64`架构的版本。你也可以生成`win-x64`、`osx-x64`、`linux-x64`、`linux-arm64`、`linux-arm`的版本。你可以在`TShockLauncher/bin/Release/net6.0/`文件夹下对应架构的文件夹里找到生成后的发行版。

### 跟泰拉瑞亚本体代码交互

在TShock和其他TSAPI的插件里跟本体代码交互会跟其他API不同。因为OTAPI的原因，`Terraria`命名空间里的所有字段都变成public了。这意味着你可以直接访问所有本体代码的成员。TShock和其他插件经常会这样做，基本上是修改地图、发送和接收数据包。调用`Main`就是一个直接访问的例子。相当于CraftBukkit里对`net.minecraft.server` (NMS)的调用。

你也许会好奇这些字段在哪里能找到。Pryaxis提供了反编译原版服务端得到的[源码](https://github.com/pryaxis/Sources)，会随着游戏版本持续更新。由于版权方要求，这些源码只对TShock的开发者开放。如果你提交过TShock的PR，可以直接在Discord里索要访问权限。你也可以下载`ILSpy`自己反编译服务端。

最后，你也许会对开发TSAPI插件感兴趣。[TShock资源]这个组织有数个你可以参考学习的插件。TShock自身也是一个TSAPI的插件，并且大多数插件都是开源的。这给了你足够的空间去找到接下来的方向。

需要帮助吗？加入我们的[Discord](https://discord.gg/Cav9nYX)服务器。或者[QQ群](https://jq.qq.com/?_wv=1027&k=5GJZCe4)。

## 行为准则

> 如果参与泰拉瑞亚TShock社区，所有成员都需坚持对社区内外的所有人保持礼仪。成员不可参与不恰当地贬低或边缘化任何群体或个人的讨论。成员不会试图将议程推进或推进到专横或不开明的地步（例如通过灌输负面观念）。成员不会滥用向他们提供的服务，并将根据具体情况遵循社区领袖关于滥用行为的指导。成员将遵守美国和国际法。如果成员发现违反此行为准则的行为，他们将不会参与，而是会在论坛或Discord上联系领导团队。

> 不要试图通过巧妙的逻辑或理由来规避或绕过行为准则（例如侮辱 Facepunch上的成员，因为他们不会在这里被直接提及）。
