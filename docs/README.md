<p align="center">
  <img src="https://tshock.co/newlogo.png" alt="TShock for Terraria"><br />
</p>

* Download: [official](https://github.com/TShock/TShock/releases) or [experimental](#experimental-downloads).
* Download: [plugins](https://github.com/topics/tshock-plugin) that work with TShock, [OTAPI](https://github.com/topics/otapi), TSAPI, and Terraria.
* Join [Discord](https://discord.gg/Cav9nYX).
* Talk on [GitHub discussions](https://github.com/Pryaxis/TShock/discussions) to ask for help, chat, and other things. This is the best way to get help if Discord isn't your thing.
* For news, follow [@Pryaxis](https://twitter.com/Pryaxis) on Twitter.
* Contribute changes and hack on the project [on GitHub](https://github.com/Pryaxis/TShock).
* Nuget packages for [TShock](https://www.nuget.org/packages/TShock/) and [TSAPI](https://www.nuget.org/packages/TSAPI/) are available.

----

## Installing TShock

TShock supports any system that .NET 6 supports. You should be able to run TShock on x86, x86_64, arm32, arm64, and arm64e on macOS, Windows, or Linux. TShock has also been used successfully on FreeBSD, using an unofficial version of .NET.

Generally, TShock needs at least 1GB of memory, but alternative tile providers may be able to lower the minimum memory threshold. You also need a reasonably powerful computer. Raspberry Pi 4 has been used for very small servers, but in-practice, we suggest something more powerful.

1. If you're on Windows 10 or another operating system, install the [.NET Runtime version 6.x](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or the .NET SDK 6.x if you're intending to build and develop plugins. If you're on Windows 11 22H2 or later, you probably already have .NET 6 installed.

1. Download [the latest stable version](https://github.com/TShock/TShock/releases) and `unzip` the folder using your favorite unzip tool. Make sure that all of the files in the zip get into one folder. This is where your server will be stored. The file structure looks like this:

          TShock.Server.exe
          bin\
          |------HttpServer.dll
          |------ModFramework.dll
          |------OTAPI.dll
          |------OTAPI.Runtime.dll
          |------TerrariaServer.dll
          ServerPlugins\
          |------TShockAPI.dll

1. Start `TShock.Server.exe` (or `TShock.Server` on other platforms) and TShock will boot, creating a folder called `tshock` to store its database and configuration files. Answer the startup questions, and you should be ready to roll. In the background, TShock made some folders for you. We'll come back to those later.

1. Startup Terraria. Connect to a `multiplayer` server via IP and enter `localhost` if you're doing this on your local computer. If you're doing it on another computer, you need its IP address.

1. Look at the server console for the _setup code_. Type `/setup [code]` (example: `/setup 12345`), then a space, then the code you see in the console in your game chat. Instead of chatting, you'll run a command on the server. This one makes you temporary admin. All commands are prefixed with `/` or `!` (to make them silent).

1. Use the in-game command `/user add [account name] [password] owner` (example: `/user add shank lovely-grilled-cheese owner`) to create an account. This gives you owner rights on your server, which you can configure more to your liking later.

1. Login to your newly created account with `/login [account name] [password]` (example: `/login shank grilled-cheese`). You should see a login success message.

1. Turn off the setup system with `/setup` and your server is setup for initial use. TShock also created several files inside a new `tshock` folder. These files include `config.json` (our big configuration file), `sscconfig.json` (the server side characters configuration file), and `tshock.sqlite`. Don't lose your `tshock.sqlite` or you'll have to re-setup TShock.

1. To install more plugins, add them to the `ServerPlugins` folder.

### Upgrading

To upgrade TShock 5 to future versions of TShock 5, simply download the latest release, extract the archive, and then merge all of the files in the release archive with your existing installation. The `tshock` folder contains user data, and any database changes will be automatically performed to bring your server up-to-date with the latest stuff from us.

#### Upgrading from TShock 4?

If you're upgrading from TShock 4, we suggest downloading the new release of TShock, copying the `tshock` configuration folder over (containing the `sqlitedb` file as well as the `json` configuration files, etc) to the new server, and starting the new server that way. This is because many files are removed and the existing files are no longer required from TShock 4. In addition, the new binary you need to run is called `TShock.Server.exe` or `TShock.Server`, not `TerrariaServer.exe`.

In addition, you no longer need to install `mono-complete` or `mono` on non-Windows operating systems.

### Apple Silicon

On Apple Silicon, you can run TShock using Rosetta 2. This is done by using the `x64` versions of TShock and its associated plugins. Make sure that's the version you download.

1. **Do not install `dotnet` from homebrew**. Instead, install the [.NET 6 SDK for x86](https://dotnet.microsoft.com/en-us/download).
1. Add the `x64` version of .NET to your path: `export PATH=$PATH:/usr/local/share/dotnet/x64/`.
1. If you haven't already installed, Rosetta 2, run `sudo softwareupdate --install-rosetta --agree-to-license` in your terminal.
1. Run `./TShock.Server` in the terminal.
    * If you get a prompt indicating that you can't run `TShock.Server` because Apple can't check it for malicious software, right click on `TShock.Server` in Finder, then select "Open" and then choose Terminal. You will then be prompted to bypass system security. Click "open" on the prompt asking to bypass system security. If you've done this correctly, you can run `TShock.Server` from terminal without needing to do this again.

If you get an error that looks like this:

```
rosetta error: /var/db/oah/2c885558d6a2ecad3098d24447a4071ee679371339e97846cd3d03a3b2bf5ab4/
b45bd88b435cac41689c907440d5761e7182a4da0cbacaea5b1310d4f7e965d0/TShock.Server.aot:
attachment of code signature supplement failed: 1
```

Try rebooting your Mac, redownloading the server, installing updates, or re-running `softwareupdate --install-rosetta`.

TShock needs to run under Rosetta 2 because of `W^X` memory protection and other features that aren't yet supported by [MonoMod](https://github.com/MonoMod/MonoMod). There's an [issue on the MonoMod repository about supporting Apple Silicon](https://github.com/MonoMod/MonoMod/issues/90).

## Experimental downloads

To download experimental versions of TShock, you have two real options: AppVeyor builds or GitHub builds. Fair warning though: experimental versions of TShock are point-in-time releases that are not technically supported by us. If you have to report an issue, please make it clear which commit or branch you downloaded your build from, which service, and the build number if applicable.

On [AppVeyor](https://ci.appveyor.com/project/hakusaro/tshock/), click on history, find the build you want, click on the commit message, and then click on the artifacts tab. You can download either the debug or the release build. AppVeyor only keeps builds back 6 months though, and there's a bandwidth limit.

On [GitHub](https://github.com/Pryaxis/TShock/), click on the actions tab, then click on "CI OTAPI3" on the commit or branch you want. If it was successful, you can download either the experimental release or debug artifacts. You must be signed into GitHub for the links to work.

<div style="width:100%;height:0px;position:relative;padding-bottom:83.333%;"><iframe src="https://streamable.com/e/qmi6gq?loop=0" frameborder="0" width="100%" height="100%" allowfullscreen style="width:100%;height:100%;position:absolute;left:0px;top:0px;overflow:hidden;"></iframe></div>
