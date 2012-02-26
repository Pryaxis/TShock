<link href="https://raw.github.com/clownfart/Markdown-CSS/master/markdown.css" rel="stylesheet"></link>

# The config file

[Back to index](index.md.html)

----

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