<p align="center">
  <img src="https://tshock.co/newlogo.png" alt="TShock for Terraria"><br />
  <a href="https://travis-ci.org/Pryaxis/TShock"><img src="https://travis-ci.org/Pryaxis/TShock.svg?branch=general-devel" alt="Build Status"></a><a href="https://ci.appveyor.com/project/hakusaro/tshock"><img src="https://ci.appveyor.com/api/projects/status/chhe61q227lqdlg1?svg=true" alt="AppVeyor Build Status"></a><br />
</p>

TShock is a toolbox for Terraria servers and communities. That toolbox is jam packed with anti-cheat tools, server-side characters, groups, permissions, item bans, tons of commands, and limitless potential. It's one of a kind.

* Download: [Stable](https://github.com/TShock/TShock/releases) or [Experimental](https://travis.tshock.co/).
* Read [the documentation](https://tshock.readme.io/) to quickly get up to speed.
* Join [Discord](https://discord.gg/XUJdH58) to get help, chat, and enjoy some swell Australian company.
* Download [other plugins](https://tshock.co/xf/index.php?resources/) to supercharge your server.

----

## Table of Contents

  * New to TShock?(#new-to-tshock)
  * Code of conduct(#code-of-conduct)

## New to TShock?

1. Download [the latest stable version](https://github.com/TShock/TShock/releases) and `unzip` the folder using your favorite unzip tool. Make sure that all of the files in the zip get into one folder. This is where your server will be stored. The file structure looks like this:

      
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
      

1. Start `TerrariaServer.exe` and TShock will boot. Answer the startup questions, and you should be ready to roll. In the background, TShock made some folders for you. We'll come back to those later.

1. Startup Terraria. Connect to a `multiplayer` server via IP and enter `localhost` if you're doing this on your local computer. If you're doing it on another computer, you need its IP address.

1. Look at the server console for the _auth code_. Type `/auth [code]` (example: `/auth 12345`), then a space, then the code you see in the console in your game chat. Instead of chatting, you'll run a command on the server. This one makes you temporary admin. All commands are prefixed with `/` or `!` (to make them silent).

1. Use the in-game command `/user add [username] [password] owner` (example: `/user add shank ashes owner`) to create an account. This gives you owner rights on your server, which you can configure more to your liking later.

1. Login to your newly created account with `/login [username] [password]` (example: `/login shank ashes`). You should see a login success message.

1. Turn off the backdoor with `/auth` and your server is setup for initial use!

## Community

Feeling like helping out? Want to find an awesome server? Some awesome plugins?

* [Website & Forums](https://tshock.co/xf/)
* [Contribute to our docs on readme.io](https://tshock.readme.io/)
* [Join our Discord chat (supports Android, iOS, Web, Mac, and Windows)](https://discord.gg/XUJdH58)

### Code of Conduct

> By participating in the TShock for Terraria community, all members will adhere to maintaining decorum with respect to all humans, in and out of the community. Members will not engage in discussion that inappropriately disparages or marginalizes any group of people or any individual. Members will not attempt to further or advance an agenda to the point of being overbearing or close minded (such as through spreading FUD). Members will not abuse services provided to them and will follow the guidance of community leaders on a situational basis about what abuse consists of. Members will adhere to United States and international law. If members notice a violation of this code of conduct, they will not engage but will instead contact the leadership team on either the forums or Discord.

> Do not attempt to circumvent or bypass the code of conduct by using clever logic or reasoning (e.g., insulting Facepunch members, because they weren't directly mentioned here).

Please see the contributing file before sending pull requests.

## Download

* [Development Builds](https://travis.tshock.co/)
* [Plugins](https://tshock.co/xf/index.php?resources/)
* [Very, very old versions of TShock](https://github.com/TShock/TShock/downloads)

## Backers

Support us with a monthly donation and help us continue our activities. [[Become a backer](https://opencollective.com/tshock#backer)]

<a href="https://opencollective.com/tshock/backer/0/website" target="_blank"><img src="https://opencollective.com/tshock/backer/0/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/1/website" target="_blank"><img src="https://opencollective.com/tshock/backer/1/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/2/website" target="_blank"><img src="https://opencollective.com/tshock/backer/2/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/3/website" target="_blank"><img src="https://opencollective.com/tshock/backer/3/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/4/website" target="_blank"><img src="https://opencollective.com/tshock/backer/4/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/5/website" target="_blank"><img src="https://opencollective.com/tshock/backer/5/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/6/website" target="_blank"><img src="https://opencollective.com/tshock/backer/6/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/7/website" target="_blank"><img src="https://opencollective.com/tshock/backer/7/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/8/website" target="_blank"><img src="https://opencollective.com/tshock/backer/8/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/9/website" target="_blank"><img src="https://opencollective.com/tshock/backer/9/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/10/website" target="_blank"><img src="https://opencollective.com/tshock/backer/10/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/11/website" target="_blank"><img src="https://opencollective.com/tshock/backer/11/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/12/website" target="_blank"><img src="https://opencollective.com/tshock/backer/12/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/13/website" target="_blank"><img src="https://opencollective.com/tshock/backer/13/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/14/website" target="_blank"><img src="https://opencollective.com/tshock/backer/14/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/15/website" target="_blank"><img src="https://opencollective.com/tshock/backer/15/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/16/website" target="_blank"><img src="https://opencollective.com/tshock/backer/16/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/17/website" target="_blank"><img src="https://opencollective.com/tshock/backer/17/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/18/website" target="_blank"><img src="https://opencollective.com/tshock/backer/18/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/19/website" target="_blank"><img src="https://opencollective.com/tshock/backer/19/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/20/website" target="_blank"><img src="https://opencollective.com/tshock/backer/20/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/21/website" target="_blank"><img src="https://opencollective.com/tshock/backer/21/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/22/website" target="_blank"><img src="https://opencollective.com/tshock/backer/22/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/23/website" target="_blank"><img src="https://opencollective.com/tshock/backer/23/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/24/website" target="_blank"><img src="https://opencollective.com/tshock/backer/24/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/25/website" target="_blank"><img src="https://opencollective.com/tshock/backer/25/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/26/website" target="_blank"><img src="https://opencollective.com/tshock/backer/26/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/27/website" target="_blank"><img src="https://opencollective.com/tshock/backer/27/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/28/website" target="_blank"><img src="https://opencollective.com/tshock/backer/28/avatar.svg"></a>
<a href="https://opencollective.com/tshock/backer/29/website" target="_blank"><img src="https://opencollective.com/tshock/backer/29/avatar.svg"></a>

## Sponsors

Become a sponsor and get your logo on our README on Github with a link to your site. [[Become a sponsor](https://opencollective.com/tshock#sponsor)]

<a href="https://opencollective.com/tshock/sponsor/0/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/0/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/1/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/1/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/2/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/2/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/3/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/3/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/4/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/4/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/5/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/5/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/6/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/6/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/7/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/7/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/8/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/8/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/9/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/9/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/10/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/10/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/11/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/11/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/12/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/12/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/13/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/13/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/14/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/14/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/15/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/15/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/16/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/16/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/17/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/17/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/18/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/18/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/19/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/19/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/20/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/20/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/21/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/21/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/22/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/22/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/23/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/23/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/24/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/24/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/25/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/25/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/26/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/26/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/27/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/27/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/28/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/28/avatar.svg"></a>
<a href="https://opencollective.com/tshock/sponsor/29/website" target="_blank"><img src="https://opencollective.com/tshock/sponsor/29/avatar.svg"></a>
