<p align="center">
  <img src="https://tshock.co/newlogo.png" alt="TShock for Terraria"><br />
  <a href="https://ci.appveyor.com/project/hakusaro/tshock">
    <img src="https://ci.appveyor.com/api/projects/status/chhe61q227lqdlg1?svg=true" alt="AppVeyor Build Status">
  </a>
  <a href="https://github.com/Pryaxis/TShock/actions">
    <img src="https://github.com/Pryaxis/TShock/actions/workflows/build.yml/badge.svg" alt="GitHub Actions Build Status">
  </a>
  <br/><br/>
  <a href="https://github.com/Pryaxis/TShock/blob/general-devel/README_cn.md">查看中文版</a>
</p>

TShock is a toolbox for Terraria servers and communities. That toolbox is jam packed with anti-cheat tools, server-side characters, groups, permissions, item bans, tons of commands, and limitless potential. It's one of a kind.

This is the readme for TShock developers and hackers. We're building out new[TShock documentation](https://ikebukuro.tshock.co/) for server operators and plugin developers, but this is a work-in-progress right now.

## Developing TShock

If you want to contribute to TShock by sending a pull request or customize it to suit your own sparkly desires, this is the best starting point. By the end of this, you'll be able to build TShock from source, start to finish. More than that, though, you'll know how to start on the path of becoming an expert TShock developer.

This guide works assuming that you have the [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed and that you're familiar with the command line. If that doesn't describe you, you should be able to accomplish the same thing using Visual Studio 2022 or Visual Studio Code.

1. Clone the repository: `git clone https://github.com/Pryaxis/TShock.git --recurse-submodules`
1. `cd TShock` to enter the repo.
1. `dotnet build`. No really, that will build things!

If you want to run the `TShockLauncher` (which runs a server), run:

1. `dotnet run --project TShockLauncher`

To produce a packaged release (suitable for distribution), run:

1. `cd TShockLauncher`
1. `dotnet publish -r win-x64 -f net6.0 -c Release -p:PublishSingleFile=true --self-contained false`

Note that in this example, you'd be building for `win-x64`. You can build for `win-x64`, `osx-x64`, `linux-x64`, `linux-arm64`, `linux-arm`. Your release will be in the `TShockLauncher/bin/Release/net6.0/` folder under the architecture you specified.

### Working with Terraria

Working with Terraria in TShock and in other Terraria Server API plugins is different from most other APIs. Due to the nature of how OTAPI works, you have direct access to all public fields in the `Terraria` namespace. This means that you can access Terraria member methods directly. TShock and other plugins do this quite often, mostly to modify the game world, send data, and receive data. Calls to `Main` are one such example of direct access to Terraria. This is the equivalent to `net.minecraft.server` (NMS) calls in CraftBukkit.

You might find yourself wondering where these fields are. Pryaxis provides the decompiled [Sources](https://github.com/pryaxis/Sources) to Terraria's server, updated with each release. These sources are made available to developers of TShock. If you have submitted a pull request to TShock, reach out on Discord to get access. In lieu of this, you can download `ILSpy` and decompile Terraria or the server itself.

Finally, you may be interested in developing other Terraria Server API plugins. The [TShockResources](https://github.com/TShockResources) organization has several plugins you can look at and build on. TShock is itself a plugin, and most plugins are open source. This gives you ample room to figure out where to go next.

Need help? Join us on [Discord](https://discord.gg/Cav9nYX).

## Code of Conduct

> By participating in the TShock for Terraria community, all members will adhere to maintaining decorum with respect to all humans, in and out of the community. Members will not engage in discussion that inappropriately disparages or marginalizes any group of people or any individual. Members will not attempt to further or advance an agenda to the point of being overbearing or close minded (such as through spreading FUD). Members will not abuse services provided to them and will follow the guidance of community leaders on a situational basis about what abuse consists of. Members will adhere to United States and international law. If members notice a violation of this code of conduct, they will not engage but will instead contact the leadership team on either the forums or Discord.

> Do not attempt to circumvent or bypass the code of conduct by using clever logic or reasoning (e.g., insulting Facepunch members, because they weren't directly mentioned here).
