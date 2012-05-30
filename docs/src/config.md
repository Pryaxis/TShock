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

The following is the official documentation for the configuration file:

## AllowCorruptionCreep  
**Type:** Boolean  
**Description:** Allows corrutption to spread when a world is hardmode.  
**Default:** "True"  

## AllowHallowCreep  
**Type:** Boolean  
**Description:** Allows hallow to spread when a world is hardmode.  
**Default:** "True"  

## AllowIce  
**Type:** Boolean  
**Description:** Allow Ice placement even when user does not have canbuild  
**Default:** "False"  

## AllowLoginAnyUsername  
**Type:** Boolean  
**Description:** Allows users to login with any username with /login  
**Default:** "True"  

## AllowRegisterAnyUsername  
**Type:** Boolean  
**Description:** Allows users to register any username with /register  
**Default:** "False"  

## AutoSave  
**Type:** Boolean  
**Description:** Enable/Disable Terrarias built in auto save  
**Default:** "True"  

## BackupInterval  
**Type:** Int32  
**Description:** Backup frequency in minutes. So, a value of 60 = 60 minutes. Backups are stored in the \tshock\backups folder.  
**Default:** "0"  

## BackupKeepFor  
**Type:** Int32  
**Description:** How long backups are kept in minutes. 2880 = 2 days.  
**Default:** "60"  

## BanOnMediumcoreDeath  
**Type:** Boolean  
**Description:** Bans a Hardcore player on death.  
**Default:** "False"  

## BufferPackets  
**Type:** Boolean  
**Description:** Buffers up the packets and sends them out at the end of each frame  
**Default:** "True"  

## ChatFormat  
**Type:** String  
**Description:** Change ingame chat format, {0} = Group Name, {1} = Group Prefix, {2} = Player Name, {3} = Group Suffix, {4} = Chat Message  
**Default:** "{1}{2}{3}: {4}"  

## DefaultGuestGroupName  
**Type:** String  
**Description:** Selects the default group name to place non registered users under  
**Default:** "guest"  

## DefaultMaximumSpawns  
**Type:** Int32  
**Description:** The default maximum mobs that will spawn per wave. Higher means more mobs in that wave.  
**Default:** "5"  

## DefaultRegistrationGroupName  
**Type:** String  
**Description:** Selects the default group name to place new registrants under  
**Default:** "default"  

## DefaultSpawnRate  
**Type:** Int32  
**Description:** The delay between waves. Shorter values lead to less mobs.  
**Default:** "600"  

## DisableBuild  
**Type:** Boolean  
**Description:** Disables any building; placing of blocks  
**Default:** "False"  

## DisableClownBombs  
**Type:** Boolean  
**Description:** Disables clown bomb projectiles from spawning  
**Default:** "False"  

## DisableDungeonGuardian  
**Type:** Boolean  
**Description:** Disables Dungeon Guardian from being spawned by player packets, this will instead force a respawn  
**Default:** "False"  

## DisableHardmode  
**Type:** Boolean  
**Description:** Disables hardmode, can't never be activated. Overrides /starthardmode  
**Default:** "False"  

## DisableInvisPvP  
**Type:** Boolean  
**Description:** Disables Invisibility potions from being used in PvP (Note, they can use them on the client, but the effect isn't sent to the rest of the server)  
**Default:** "False"  

## DisableLoginBeforeJoin  
**Type:** Boolean  
**Description:** Disable users from being able to login with account password when joining  
**Default:** "False"  

## DisablePiggybanksOnSSI  
**Type:** Boolean  
**Description:** Prevent banks on SSI  
**Default:** "False"  

## DisablePlayerCountReporting  
**Type:** Boolean  
**Description:** Disables reporting of playercount to the stat system.  
**Default:** "False"  

## DisableSnowBalls  
**Type:** Boolean  
**Description:** Disables snow ball projectiles from spawning  
**Default:** "False"  

## DisableSpewLogs  
**Type:** Boolean  
**Description:** Force-Disable printing logs to players with the log permission  
**Default:** "True"  

## DisableTombstones  
**Type:** Boolean  
**Description:** Disable tombstones for all players.  
**Default:** "True"  

## DisplayIPToAdmins  
**Type:** Boolean  
**Description:** Displays a player's IP on join to everyone who has the log permission  
**Default:** "False"  

## EnableBanOnUsernames  
**Type:** Boolean  
**Description:** Enables kicking of banned users by matching their Character Name  
**Default:** "False"  

## EnableDNSHostResolution  
**Type:** Boolean  
**Description:** Enables DNS resolution of incoming connections with GetGroupForIPExpensive.  
**Default:** "False"  

## EnableGeoIP  
**Type:** Boolean  
**Description:** This will announce a player's location on join  
**Default:** "False"  

## EnableInsecureTileFixes  
**Type:** Boolean  
**Description:** Some tiles are 'fixed' by not letting TShock handle them. Disabling this may break certain asthetic tiles.  
**Default:** "True"  

## EnableIPBans  
**Type:** Boolean  
**Description:** Enables kicking of banned users by matching their IP Address  
**Default:** "True"  

## EnableTokenEndpointAuthentication  
**Type:** Boolean  
**Description:** This will turn on a token requirement for the /status API endpoint.  
**Default:** "False"  

## EnableWhitelist  
**Type:** Boolean  
**Description:** Enable or disable the whitelist based on IP addresses in whitelist.txt  
**Default:** "False"  

## ForceTime  
**Type:** String  
**Description:** Force the world time to be normal, day, or night  
**Default:** "normal"  

## HardcoreOnly  
**Type:** Boolean  
**Description:** Hardcore players ONLY. This means softcore players cannot join.  
**Default:** "False"  

## HashAlgorithm  
**Type:** String  
**Description:** Valid types are "sha512", "sha256", "md5", append with "-xp" for the xp supported algorithms  
**Default:** "sha512"  

## IgnoreNoClip  
**Type:** Boolean  
**Description:** Ignores all no clip checks for players  
**Default:** "False"  

## IgnoreProjKill  
**Type:** Boolean  
**Description:** Ignores checking to see if player 'can' kill a projectile  
**Default:** "False"  

## IgnoreProjUpdate  
**Type:** Boolean  
**Description:** Ignores checking to see if player 'can' update a projectile  
**Default:** "False"  

## InfiniteInvasion  
**Type:** Boolean  
**Description:** Enable the ability for invaison size to never decrease. Make sure to run /invade, and note that this adds 2 million+ goblins to the spawn que for the map.  
**Default:** "False"  

## InvasionMultiplier  
**Type:** Int32  
**Description:** The equation for calculating invasion size is 100 + (multiplier * (number of active players with greater than 200 health))  
**Default:** "1"  

## KickOnMediumcoreDeath  
**Type:** Boolean  
**Description:** Kicks a Hardcore player on death.  
**Default:** "False"  

## KickProxyUsers  
**Type:** Boolean  
**Description:** Kicks users using a proxy as identified with the GeoIP database  
**Default:** "True"  

## LogonDiscardThreshold  
**Type:** Int32  
**Description:** Time, in milliseconds, to disallow discarding items after logging in when ServerSideInventory is ON  
**Default:** "250"  

## MasterServer  
**Type:** String  
**Description:** Not implemented  
**Default:** "127.0.0.1"  

## MaxDamage  
**Type:** Int32  
**Description:** The maximum damage a player/npc can inflict  
**Default:** "175"  

## MaximumLoginAttempts  
**Type:** Int32  
**Description:** Number of failed login attempts before kicking the player.  
**Default:** "3"  

## MaxProjDamage  
**Type:** Int32  
**Description:** The maximum damage a projectile can inflict  
**Default:** "175"  

## MaxRangeForDisabled  
**Type:** Int32  
**Description:** The maximum distance players disabled for various reasons can move from  
**Default:** "10"  

## MaxSlots  
**Type:** Int32  
**Description:** Max slots for the server. If you want people to be kicked with "Server is full" set this to how many players you want max and then set Terraria max players to 2 higher.  
**Default:** "8"  

## MediumcoreBanReason  
**Type:** String  
**Description:** Bans a Mediumcore player on death.  
**Default:** "Death results in a ban"  

## MediumcoreKickReason  
**Type:** String  
**Description:** Kicks a Mediumcore player on death.  
**Default:** "Death results in a kick"  

## MediumcoreOnly  
**Type:** Boolean  
**Description:** Mediumcore players ONLY. This means softcore players cannot join.  
**Default:** "False"  

## MySqlDbName  
**Type:** String  
**Description:** Database name to connect to  
**Default:** ""  

## MySqlHost  
**Type:** String  
**Description:** The MySQL Hostname and port to direct connections to  
**Default:** "localhost:3306"  

## MySqlPassword  
**Type:** String  
**Description:** Database password to connect with  
**Default:** ""  

## MySqlUsername  
**Type:** String  
**Description:** Database username to connect with  
**Default:** ""  

## PreventBannedItemSpawn  
**Type:** Boolean  
**Description:** Prevent banned items from being /i or /give  
**Default:** "False"  

## ProjectileThreshold  
**Type:** Int32  
**Description:** Disable a player if they exceed this number of projectile new within 1 second.  
**Default:** "50"  

## ProjIgnoreShrapnel  
**Type:** Boolean  
**Description:** Ignore shrapnel from crystal bullets for Projectile Threshold.  
**Default:** "True"  

## PvPMode  
**Type:** String  
**Description:** Set the server pvp mode. Vaild types are, "normal", "always", "disabled"  
**Default:** "normal"  

## RangeChecks  
**Type:** Boolean  
**Description:** Global protection agent for any block distance based anti-grief check.  
**Default:** "True"  

## RconPassword  
**Type:** String  
**Description:** Not implemented  
**Default:** ""  

## RconPort  
**Type:** Int32  
**Description:** Not implemented  
**Default:** "7777"  

## RegionProtectChests  
**Type:** Boolean  
**Description:** Protect chests with region and build permissions  
**Default:** "False"  

## RememberLeavePos  
**Type:** Boolean  
**Description:** Remembers where a player left off. It works by remembering the IP, NOT the character.  
eg. When you try to disconnect, and reconnect to be automatically placed at spawn, you'll be at your last location. Note: Won't save after server restarts.  
**Default:** "False"  

## RequireLogin  
**Type:** Boolean  
**Description:** Require all players to register or login before being allowed to play.  
**Default:** "False"  

## RestApiEnabled  
**Type:** Boolean  
**Description:** Enable/Disable the rest api.  
**Default:** "False"  

## RestApiPort  
**Type:** Int32  
**Description:** This is the port which the rest api will listen on.  
**Default:** "7878"  

## SaveWorldOnCrash  
**Type:** Boolean  
**Description:** This will save the world if Terraria crashes from an unhandled exception.  
**Default:** "True"  

## ServerFullNoReservedReason  
**Type:** String  
**Description:** String that is used when kicking people when the server is full with no reserved slots.  
**Default:** "Server is full. No reserved slots open."  

## ServerFullReason  
**Type:** String  
**Description:** String that is used when kicking people when the server is full.  
**Default:** "Server is full"  

## ServerName  
**Type:** String  
**Description:** Used when replying to a rest /status request.  
**Default:** ""  

## ServerNickname  
**Type:** String  
**Description:** This is used when the API endpoint /status is queried.  
**Default:** "TShock Server"  

## ServerPassword  
**Type:** String  
**Description:** Server password required to join server  
**Default:** ""  

## ServerPort  
**Type:** Int32  
**Description:** The port the server runs on.  
**Default:** "7777"  

## ServerSideInventory  
**Type:** Boolean  
**Description:** Enable Server Side Inventory checks, EXPERIMENTAL  
**Default:** "False"  

## ServerSideInventorySave  
**Type:** Int32  
**Description:** How often SSI should save, in minutes  
**Default:** "15"  

## SpawnProtection  
**Type:** Boolean  
**Description:** Prevents tiles from being placed within SpawnProtectionRadius of the default spawn.  
**Default:** "True"  

## SpawnProtectionRadius  
**Type:** Int32  
**Description:** Radius from spawn tile for SpawnProtection.  
**Default:** "10"  

## StatueSpawn200  
**Type:** Int32  
**Description:** How many things a statue can spawn within 200 pixels(?) before it stops spawning. Default = 3  
**Default:** "3"  

## StatueSpawn600  
**Type:** Int32  
**Description:** How many things a statue can spawn within 600 pixels(?) before it stops spawning. Default = 6  
**Default:** "6"  

## StatueSpawnWorld  
**Type:** Int32  
**Description:** How many things a statue spawns can exist in the world before it stops spawning. Default = 10  
**Default:** "10"  

## StorageType  
**Type:** String  
**Description:** Valid types are "sqlite" and "mysql"  
**Default:** "sqlite"  

## SuperAdminChatPrefix  
**Type:** String  
**Description:** Super admin group chat prefix  
**Default:** "(Admin) "  

## SuperAdminChatRGB  
**Type:** Single[]  
**Description:** #.#.#. = Red/Blue/Green - RGB Colors for the Admin Chat Color. Max value: 255  
**Default:** "System.Single[]"  

## SuperAdminChatSuffix  
**Type:** String  
**Description:** Super admin group chat suffix  
**Default:** ""  

## TileKillThreshold  
**Type:** Int32  
**Description:** Disable/Revert a player if they exceed this number of tile kills within 1 second.  
**Default:** "60"  

## TileLiquidThreshold  
**Type:** Int32  
**Description:** Disable a player if they exceed this number of liquid sets within 1 second.  
**Default:** "15"  

## TilePlaceThreshold  
**Type:** Int32  
**Description:** Disable/Revert a player if they exceed this number of tile places within 1 second.  
**Default:** "20"  

## WhitelistKickReason  
**Type:** String  
**Description:** String that is used when a user is kicked due to not being on the whitelist.  
**Default:** "You are not on the whitelist."  

