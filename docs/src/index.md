<link href="https://raw.github.com/clownfart/Markdown-CSS/master/markdown.css" rel="stylesheet"></link>

# TShock Downloaded Documentation

*Created for TShock version: 3.5.x.x*

*Last updated: 2/25/2012*

----

## Preface

Welcome to the official TShock for Terraria downloaded documentation. This guide will walk through the installation and basic configuration of your newly downloaded TShock server, and should provide a basic knowledge as to how to get help from outside resources if needed.

## Resources

* [The Confluence wiki](http://develop.tshock.co:8080/) contains the most up to date information compiled by the community members. If your question isn't answered here, you might find it there.
* [The forums](http://tshock.co/xf/) provide an excellent place to ask other TShock users and developers questions. Please refrain from making posts about questions that may be answered here, however.
* [Our Github page](http://github.com/TShock/TShock) is where you'll be able to find the source code and the bug tracker.
* [IRC](irc://irc.shankshock.com/terraria) is our IRC channel, if you prefer that medium for support.
* Lastly, we can be found in the "Nyx" channel on the Teamspeak 3 server: ts3.shankshock.com, port 9987.

----

## Table of contents

1. [Installation](#Installation)
2. [Basic usage](#Basics)
3. [Configuration](#Configuration)
4. [Permissions](#Permissions)
5. [The config file](#Config)
6. [Plugins](#Plugins)
7. [Closing remarks](#Closing)
8. [Credits](#Credits)

----

### Installation <a id="Installation"></a>

1. Assuming you've extracted TShock, you're done as far as files go. Run the TerrariaServer.exe file in the folder you've extracted TShock into.
2. Check to verify that the server window states that some version of TerrariaShock is now running. If this is not the case, stop. Re-download all files and extract them to a new folder, being sure to keep the file structure in tact.
3. Select a world and port to start the server. *TShock now uses its configuration file to control the number of players on the server. You can edit this value in the configuration file, discussed later. If you are generating a new world, you may experience a significantly longer time as the world creates itself. This is normal.
4. Once the server is finished starting, you will be greeted with TShock's main console window. You will see a message in yellow that states "*To become superadmin, join the game and type /auth*" preceding a series of numbers. This set of numbers we will refer to as the "authcode" in succeeding steps.
5. Connect to the server. Your IP address is 127.0.0.1, and the port will by default be on what you entered in the server console.
6. Immediately chat the following: "**/auth [authcode]**". Replace [authcode] with the code given in the server console. Example: /auth 123456.
7. Next, we will create a user account that you can login to. In the game, chat the command "**/user add [username]:[password] superadmin**". Replace [username] and [password] respectively with your appropriate details. You should be able to remeber your password, and it shouldn't be easily guessed. From now on, the phrase "run the command" is synonymous with "chat in the game chat box". In addition, where brackets ([]) are, we will assume you will replace those brackets with information that you have created.
8. Assuming the last step was a success, login. Run the command "**/login [username] [password]**".
9. To finalize installation, run the command "**/auth-verify**". This will disable the authcode, enable any previously disabled functionality, and allow the server to be used in production.

----

### Basic Usage<a id="Basics"></a>

Now that TShock is running, you may be interested in a few other features prior to playing Terraria.

* You can add admins through two methods. If the user is already registered, you can use "**/user group [username] [group-to-change-to]**". By default, TShock comes with the "vip" group, the "trustedadmin" group, and the "newadmin" group. If the user has yet to register, you can use "**/user add [username]:[password] [group]**" to generate an account with elevated permissions for them.
* When you join the server and already have an account, the server will ask for your account password, even if the server has no password setup. In the event that you set a password on the server, unregistered users will be required to enter it. Users that already have an account must enter their own password.
* If a user wishes to change accounts but retain their group, a config option exists that will allow you to allow users to login to accounts with any username.

----

### Configuration<a id="Configuration"></a>

#### Permissions<a id="Permissions"></a>

Like Bukkit and other administrative modifications, TShock supports adding groups and permissions. In the current implementation, you can only edit groups ingame, adding and removing them isn't supported *yet*.

##### Adding permissions:

To add a permission to a given group, use the command "**/modgroup [add|del] [group] [permission]**". Example: */modgroup add trustedadmin tpall*.

##### Permission nodes:

[A list of permission nodes can be found here.](permissions.html)

----

#### The config file<a id="Config"></a>

Each TShock installation automatically generates a configuration file which can edit basic settings when the server starts.

The file "config.json" is located in the *tshock* folder, in the same directory as TerrariaServer.exe.

Being a JSON file, it is extremely critical that you edit the file in a standard text editor. Use [Notepad++](http://notepad-plus-plus.org/) to edit your file, or another text editor used for programming. Before restarting TShock, check the syntax of your file using [JSONLint](http://jsonlint.com/), and verify that it contains no errors.

An example configuration file is below.

	{
	  "InvasionMultiplier": 1,
	  "DefaultMaximumSpawns": 0,
	  "DefaultSpawnRate": 600,
	  "ServerPort": 7777,
	  "EnableWhitelist": false,
	  "InfiniteInvasion": false,
	  "PvPMode": "normal",
	  "SpawnProtection": true,
	  "SpawnProtectionRadius": 5,
	  "MaxSlots": 8,
	  "RangeChecks": true,
	  "DisableBuild": false,
	  "SuperAdminChatRGB": [
	    255.0,
	    0.0,
	    0.0
	  ],
	  "SuperAdminChatPrefix": "(Admin) ",
	  "SuperAdminChatSuffix": "",
	  "BackupInterval": 0,
	  "BackupKeepFor": 60,
	  "RememberLeavePos": false,
	  "HardcoreOnly": false,
	  "MediumcoreOnly": false,
	  "KickOnMediumcoreDeath": false,
	  "BanOnMediumcoreDeath": false,
	  "AutoSave": true,
	  "MaximumLoginAttempts": 3,
	  "RconPassword": "",
	  "RconPort": 7777,
	  "ServerName": "",
	  "MasterServer": "127.0.0.1",
	  "StorageType": "sqlite",
	  "MySqlHost": "localhost:3306",
	  "MySqlDbName": "",
	  "MySqlUsername": "",
	  "MySqlPassword": "",
	  "MediumcoreBanReason": "Death results in a ban",
	  "MediumcoreKickReason": "Death results in a kick",
	  "EnableDNSHostResolution": false,
	  "EnableIPBans": true,
	  "EnableBanOnUsernames": false,
	  "DefaultRegistrationGroupName": "default",
	  "DefaultGuestGroupName": "guest",
	  "DisableSpewLogs": true,
	  "HashAlgorithm": "sha512",
	  "BufferPackets": true,
	  "ServerFullReason": "Server is full",
	  "ServerFullNoReservedReason": "Server is full. No reserved slots open.",
	  "SaveWorldOnCrash": true,
	  "EnableGeoIP": false,
	  "EnableTokenEndpointAuthentication": false,
	  "ServerNickname": "TShock Server",
	  "RestApiEnabled": false,
	  "RestApiPort": 7878,
	  "DisableTombstones": true,
	  "DisplayIPToAdmins": false,
	  "EnableInsecureTileFixes": true,
	  "KickProxyUsers": true,
	  "DisableHardmode": false,
	  "DisableDungeonGuardian": false,
	  "ServerSideInventory": false,
	  "ServerSideInventorySave": 15,
	  "LogonDiscardThreshold": 250,
	  "DisablePlayerCountReporting": false,
	  "DisableClownBombs": false,
	  "DisableSnowBalls": false,
	  "ChatFormat": "{1}{2}{3}: {4}",
	  "ForceTime": "normal",
	  "TileKillThreshold": 60,
	  "TilePlaceThreshold": 20,
	  "TileLiquidThreshold": 15,
	  "ProjectileThreshold": 50,
	  "ProjIgnoreShrapnel": true,
	  "RequireLogin": true,
	  "DisableInvisPvP": false,
	  "MaxRangeForDisabled": 10,
	  "ServerPassword": "",
	  "RegionProtectChests": false,
	  "DisableLoginBeforeJoin": false,
	  "AllowRegisterAnyUsername": false,
	  "AllowLoginAnyUsername": true,
	  "MaxDamage": 175,
	  "MaxProjDamage": 175,
	  "IgnoreProjUpdate": false,
	  "IgnoreProjKill": false,
	  "IgnoreNoClip": false,
	  "AllowIce": true
	}

In this file, if you wanted to change the maximum players to 64, you would edit that the file so that the line referring to max players looked like so:

	"MaxSlots": 64,

----

### Plugins<a id="Plugins"></a>

TShock, like most other administration modifications, allows the use of plugins. Logically, all plugins go in the *serverplugins* folder, and can be **either .dll or .dll-plugin** files. TShock is itself a plugin on the TerrariaServerAPI.

You can find all plugins on the [forums](http://tshock.co/xf/index.php?forums/plugin-releases.15/), and the documentation for creating plugins can be found on the [wiki](http://develop.tshock.co:8080/display/docs/Plugin+Development).

---

## Closing remarks<a id="Closing"></a>

Thanks for downloading TShock. Your continued support helps make TShock what it is today. We wouldn't be here without you.

From everyone at TShock, thank-you.

## Credits<a id="Credits"></a>

TShock wouldn't be possible without:

* [Xenon Servers](http://xns.cc/)
* [Kerplunc Gaming](http://kerpluncgaming.com/)
* [Multiplay UK](http://multiplay.co.uk/)
* [Atlassian](http://www.atlassian.com/)
* [Github](http://github.com/)
* You :)