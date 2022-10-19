## AllowAllowedGroupsToSpawnBannedItems  
Allows groups on the banned item allowed list to spawn banned items even if PreventBannedItemSpawn is set to true.
* **Field type**: `Boolean`
* **Default**: `False`

## AllowCorruptionCreep  
Allows the corruption to spread when a world is in hardmode.
* **Field type**: `Boolean`
* **Default**: `True`

## AllowCrimsonCreep  
Allows the crimson to spread when a world is in hardmode.
* **Field type**: `Boolean`
* **Default**: `True`

## AllowCutTilesAndBreakables  
Allows players to break temporary tiles (grass, pots, etc) where they cannot usually build.
* **Field type**: `Boolean`
* **Default**: `False`

## AllowHallowCreep  
Allows the hallow to spread when a world is in hardmode.
* **Field type**: `Boolean`
* **Default**: `True`

## AllowIce  
Allows ice placement even where a user cannot usually build.
* **Field type**: `Boolean`
* **Default**: `False`

## AllowLoginAnyUsername  
Allows users to login to any account even if the username doesn't match their character name.
* **Field type**: `Boolean`
* **Default**: `True`

## AllowRegisterAnyUsername  
Allows users to register a username that doesn't necessarily match their character name.
* **Field type**: `Boolean`
* **Default**: `False`

## AnnounceSave  
Enable or disable world save announcements.
* **Field type**: `Boolean`
* **Default**: `True`

## AnonymousBossInvasions  
Whether or not to announce boss spawning or invasion starts.
* **Field type**: `Boolean`
* **Default**: `True`

## ApplicationRestTokens  
A dictionary of REST tokens that external applications may use to make queries to your server.
* **Field type**: `Dictionary`2`
* **Default**: `System.Collections.Generic.Dictionary`2[System.String,Rests.SecureRest+TokenData]`

## AutoSave  
Enable or disable Terraria's built-in world auto save.
* **Field type**: `Boolean`
* **Default**: `True`

## BackupInterval  
The interval between backups, in minutes. Backups are stored in the tshock/backups folder.
* **Field type**: `Int32`
* **Default**: `10`

## BackupKeepFor  
For how long backups are kept in minutes.
eg. 2880 = 2 days.
* **Field type**: `Int32`
* **Default**: `240`

## BanOnHardcoreDeath  
Whether or not to ban hardcore players on death.
* **Field type**: `Boolean`
* **Default**: `False`

## BanOnMediumcoreDeath  
Whether or not to ban mediumcore players on death.
* **Field type**: `Boolean`
* **Default**: `False`

## BCryptWorkFactor  
Determines the BCrypt work factor to use. If increased, all passwords will be upgraded to new work-factor on verify. The number of computational rounds is 2^n. Increase with caution. Range: 5-31.
* **Field type**: `Int32`
* **Default**: `7`

## BombExplosionRadius  
Determines the range in tiles that a bomb can affect tiles from detonation point.
* **Field type**: `Int32`
* **Default**: `5`

## BroadcastRGB  
The RGB values used for the color of broadcast messages.
#.#.# = Red/Blue/Green
Max value: 255
* **Field type**: `Int32[]`
* **Default**: `System.Int32[]`

## ChatAboveHeadsFormat  
Changes the player name when using chat above heads. Starts with a player name wrapped in brackets, as per Terraria's formatting.
Same formatting as ChatFormat without the message.
* **Field type**: `String`
* **Default**: `{2}`

## ChatFormat  
Changes in-game chat format: {0} = Group Name, {1} = Group Prefix, {2} = Player Name, {3} = Group Suffix, {4} = Chat Message.
* **Field type**: `String`
* **Default**: `{1}{2}{3}: {4}`

## CommandSilentSpecifier  
Specifies which string starts a command silently.
Note: Will not function properly if the string length is bigger than 1.
* **Field type**: `String`
* **Default**: `.`

## CommandSpecifier  
Specifies which string starts a command.
Note: Will not function properly if the string length is bigger than 1.
* **Field type**: `String`
* **Default**: `/`

## DebugLogs  
Whether or not the server should output debug level messages related to system operation.
* **Field type**: `Boolean`
* **Default**: `False`

## DefaultGuestGroupName  
The default group name to place unregistered players under.
* **Field type**: `String`
* **Default**: `guest`

## DefaultMaximumSpawns  
The default maximum number of mobs that will spawn per wave. Higher means more mobs in that wave.
* **Field type**: `Int32`
* **Default**: `5`

## DefaultRegistrationGroupName  
The default group name to place newly registered users under.
* **Field type**: `String`
* **Default**: `default`

## DefaultSpawnRate  
The delay between waves. Lower values lead to more mobs.
* **Field type**: `Int32`
* **Default**: `600`

## DisableBuild  
Disables any placing, or removal of blocks.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableClownBombs  
Disables clown bomb projectiles from spawning.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableCustomDeathMessages  
Allows you to disable or enable protection against creating custom messages with death. Created for developers who came up with a more original solution to this problem.
* **Field type**: `Boolean`
* **Default**: `True`

## DisableDefaultIPBan  
Disbales IP bans by default, if no arguments are passed to the ban command.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableDungeonGuardian  
Prevents the dungeon guardian from being spawned while sending players to their spawn point instead.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableHardmode  
If enabled, hardmode will not be activated by the Wall of Flesh or the /starthardmode command.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableInvisPvP  
Disables the effect of invisibility potions while PvP is enabled by turning the player visible to the other clients.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableLoginBeforeJoin  
Prevents users from being able to login before they finish connecting.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableModifiedZenith  
Prohibit the use of Zenith projectile with different objects instead of weapons.
* **Field type**: `Boolean`
* **Default**: `False`

## DisablePrimeBombs  
Disables Skeletron Prime Bombs from spawning, useful for preventing unwanted world destruction on for the worthy seed world.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableSecondUpdateLogs  
Prevents OnSecondUpdate checks from writing to the log file.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableSnowBalls  
Disables snow ball projectiles from spawning.
* **Field type**: `Boolean`
* **Default**: `False`

## DisableSpewLogs  
Disables sending logs as messages to players with the log permission.
* **Field type**: `Boolean`
* **Default**: `True`

## DisableTombstones  
Disables tombstone dropping during death for all players.
* **Field type**: `Boolean`
* **Default**: `True`

## DisableUUIDLogin  
Prevents users from being able to login with their client UUID.
* **Field type**: `Boolean`
* **Default**: `False`

## DisplayIPToAdmins  
Displays a player's IP on join to users with the log permission.
* **Field type**: `Boolean`
* **Default**: `False`

## EnableChatAboveHeads  
Whether or not to display chat messages above players' heads.
* **Field type**: `Boolean`
* **Default**: `False`

## EnableGeoIP  
Whether or not to announce a player's geographic location on join, based on their IP.
* **Field type**: `Boolean`
* **Default**: `False`

## EnableTokenEndpointAuthentication  
Whether or not to require token authentication to use the public REST API endpoints.
* **Field type**: `Boolean`
* **Default**: `False`

## EnableWhitelist  
Enable or disable the whitelist based on IP addresses in the whitelist.txt file.
* **Field type**: `Boolean`
* **Default**: `False`

## ForceHalloween  
Forces Halloween-only events to occur all year.
* **Field type**: `Boolean`
* **Default**: `False`

## ForceTime  
Forces the world time to be normal, day, or night.
* **Field type**: `String`
* **Default**: `normal`

## ForceXmas  
Forces Christmas-only events to occur all year.
* **Field type**: `Boolean`
* **Default**: `False`

## HardcoreBanReason  
The reason given when banning hardcore players on death.
* **Field type**: `String`
* **Default**: `Death results in a ban`

## HardcoreKickReason  
The reason given when kicking hardcore players on death.
* **Field type**: `String`
* **Default**: `Death results in a kick`

## HardcoreOnly  
Prevents non-hardcore players from connecting.
* **Field type**: `Boolean`
* **Default**: `False`

## HealOtherThreshold  
Disables a player if this number of HealOtherPlayer packets is sent within 1 second.
* **Field type**: `Int32`
* **Default**: `50`

## IgnoreChestStacksOnLoad  
Allows stacks in chests to go beyond the stack limit during world loading.
* **Field type**: `Boolean`
* **Default**: `False`

## IgnoreProjKill  
Ignores checks to see if a player 'can' kill a projectile.
* **Field type**: `Boolean`
* **Default**: `False`

## IgnoreProjUpdate  
Ignores checks to see if a player 'can' update a projectile.
* **Field type**: `Boolean`
* **Default**: `False`

## InfiniteInvasion  
Enables never ending invasion events. You still need to start the event, such as with the /invade command.
* **Field type**: `Boolean`
* **Default**: `False`

## InvasionMultiplier  
Determines the size of invasion events.
The equation for calculating invasion size is 100 + (multiplier * (number of active players with greater than 200 health)).
* **Field type**: `Int32`
* **Default**: `1`

## KickEmptyUUID  
Kick clients that don't send their UUID to the server.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnDamageThresholdBroken  
Whether or not to kick users when they surpass the MaxDamage threshold.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnHardcoreDeath  
Whether or not to kick hardcore players on death.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnHealOtherThresholdBroken  
Whether or not to kick users when they surpass the HealOther threshold.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnMediumcoreDeath  
Whether or not to kick mediumcore players on death.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnProjectileThresholdBroken  
Whether or not to kick users when they surpass the Projectile threshold.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnTileKillThresholdBroken  
Whether or not to kick users when they surpass the TileKill threshold.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnTileLiquidThresholdBroken  
Whether or not to kick users when they surpass the TileLiquid threshold.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnTilePaintThresholdBroken  
Whether or not to kick users when they surpass the TilePaint threshold.
* **Field type**: `Boolean`
* **Default**: `False`

## KickOnTilePlaceThresholdBroken  
Whether or not to kick users when they surpass the TilePlace threshold.
* **Field type**: `Boolean`
* **Default**: `False`

## KickProxyUsers  
If GeoIP is enabled, this will kick users identified as being under a proxy.
* **Field type**: `Boolean`
* **Default**: `True`

## LogPath  
The path to the directory where logs should be written to.
* **Field type**: `String`
* **Default**: `tshock/logs`

## LogRest  
Whether or not to log REST API connections.
* **Field type**: `Boolean`
* **Default**: `False`

## MaxDamage  
The maximum damage a player/NPC can inflict.
* **Field type**: `Int32`
* **Default**: `1175`

## MaxHP  
The maximum HP a player can have, before equipment buffs.
* **Field type**: `Int32`
* **Default**: `500`

## MaximumLoginAttempts  
Number of failed login attempts before kicking the player.
* **Field type**: `Int32`
* **Default**: `3`

## MaxMP  
The maximum MP a player can have, before equipment buffs.
* **Field type**: `Int32`
* **Default**: `200`

## MaxProjDamage  
The maximum damage a projectile can inflict.
* **Field type**: `Int32`
* **Default**: `1175`

## MaxRangeForDisabled  
The maximum distance, in tiles, that disabled players can move from.
* **Field type**: `Int32`
* **Default**: `10`

## MaxSlots  
Maximum number of clients connected at once.
If you want people to be kicked with "Server is full" set this to how many players you want max and then set Terraria max players to 2 higher.
* **Field type**: `Int32`
* **Default**: `8`

## MediumcoreBanReason  
The reason given if banning a mediumcore player on death.
* **Field type**: `String`
* **Default**: `Death results in a ban`

## MediumcoreKickReason  
The reason given if kicking a mediumcore players on death.
* **Field type**: `String`
* **Default**: `Death results in a kick`

## MediumcoreOnly  
Prevents softcore players from connecting.
* **Field type**: `Boolean`
* **Default**: `False`

## MinimumPasswordLength  
The minimum password length for new user accounts. Can never be lower than 4.
* **Field type**: `Int32`
* **Default**: `4`

## MySqlDbName  
The database name to connect to when using MySQL as the database type.
* **Field type**: `String`
* **Default**: ``

## MySqlHost  
The MySQL hostname and port to direct connections to.
* **Field type**: `String`
* **Default**: `localhost:3306`

## MySqlPassword  
The password used when connecting to a MySQL database.
* **Field type**: `String`
* **Default**: ``

## MySqlUsername  
The username used when connecting to a MySQL database.
* **Field type**: `String`
* **Default**: ``

## PreventBannedItemSpawn  
Prevent banned items from being spawned or given with commands.
* **Field type**: `Boolean`
* **Default**: `False`

## PreventDeadModification  
Prevent players from interacting with the world while they are dead.
* **Field type**: `Boolean`
* **Default**: `True`

## PreventInvalidPlaceStyle  
Prevents players from placing tiles with an invalid style.
* **Field type**: `Boolean`
* **Default**: `True`

## ProjectileThreshold  
Disable a player if this number of projectiles is created within 1 second.
* **Field type**: `Int32`
* **Default**: `50`

## ProjIgnoreShrapnel  
Whether or not to ignore shrapnel from crystal bullets for the projectile threshold count.
* **Field type**: `Boolean`
* **Default**: `True`

## PvPMode  
Sets the PvP mode. Valid types are: "normal", "always" and "disabled".
* **Field type**: `String`
* **Default**: `normal`

## RangeChecks  
Enable or disable anti-cheat range checks based on distance between the player and their block placements.
* **Field type**: `Boolean`
* **Default**: `True`

## RegionProtectChests  
Whether or not region protection should apply to chests.
* **Field type**: `Boolean`
* **Default**: `False`

## RegionProtectGemLocks  
Whether or not region protection should apply to gem locks.
* **Field type**: `Boolean`
* **Default**: `True`

## RememberLeavePos  
Remembers where a player left off, based on their IP. Does not persist through server restarts.
eg. When you try to disconnect, and reconnect to be automatically placed at spawn, you'll be at your last location.
* **Field type**: `Boolean`
* **Default**: `False`

## RequireLogin  
Require all players to register or login before being allowed to play.
* **Field type**: `Boolean`
* **Default**: `False`

## ReservedSlots  
The number of reserved slots past your max server slots that can be joined by reserved players.
* **Field type**: `Int32`
* **Default**: `20`

## RespawnBossSeconds  
The number of seconds a player must wait before being respawned if there is a boss nearby. Valid range: 0 (default) to 30 seconds. Use at your own risk.
* **Field type**: `Int32`
* **Default**: `0`

## RespawnSeconds  
The number of seconds a player must wait before being respawned. Valid range: 0 (default) to 15 seconds. Use at your own risk.
* **Field type**: `Int32`
* **Default**: `0`

## RestApiEnabled  
Enable or disable the REST API.
* **Field type**: `Boolean`
* **Default**: `False`

## RestApiPort  
The port used by the REST API.
* **Field type**: `Int32`
* **Default**: `7878`

## RESTMaximumRequestsPerInterval  
The maximum REST requests in the bucket before denying requests. Minimum value is 5.
* **Field type**: `Int32`
* **Default**: `5`

## RESTRequestBucketDecreaseIntervalMinutes  
How often in minutes the REST requests bucket is decreased by one. Minimum value is 1 minute.
* **Field type**: `Int32`
* **Default**: `1`

## RevertToTextLogsOnSqlFailures  
Number of times the SQL log must fail to insert logs before falling back to the text log.
* **Field type**: `Int32`
* **Default**: `10`

## SaveWorldOnCrash  
Whether or not to save the world if the server crashes from an unhandled exception.
* **Field type**: `Boolean`
* **Default**: `True`

## SaveWorldOnLastPlayerExit  
Whether or not to save the world when the last player disconnects.
* **Field type**: `Boolean`
* **Default**: `True`

## ServerFullNoReservedReason  
The reason given when kicking players that attempt to join while the server is full with no reserved slots available.
* **Field type**: `String`
* **Default**: `Server is full. No reserved slots open.`

## ServerFullReason  
The reason given when kicking players that attempt to join while the server is full.
* **Field type**: `String`
* **Default**: `Server is full`

## ServerName  
Replaces the world name during a session if UseServerName is true.
* **Field type**: `String`
* **Default**: ``

## ServerPassword  
The server password required to join the server.
* **Field type**: `String`
* **Default**: ``

## ServerPort  
The port the server runs on.
* **Field type**: `Int32`
* **Default**: `7777`

## ShowBackupAutosaveMessages  
Whether or not to show backup auto save messages.
* **Field type**: `Boolean`
* **Default**: `True`

## SoftcoreOnly  
Prevents non-softcore players from connecting.
* **Field type**: `Boolean`
* **Default**: `False`

## SpawnProtection  
Prevents tiles from being placed within SpawnProtectionRadius of the default spawn.
* **Field type**: `Boolean`
* **Default**: `True`

## SpawnProtectionRadius  
The tile radius around the spawn tile that is protected by the SpawnProtection setting.
* **Field type**: `Int32`
* **Default**: `10`

## SqliteDBPath  
The path of sqlite db.
* **Field type**: `String`
* **Default**: `tshock.sqlite`

## StatueSpawn200  
How many NPCs a statue can spawn within 200 pixels(?) before it stops spawning.
Default = 3.
* **Field type**: `Int32`
* **Default**: `3`

## StatueSpawn600  
How many NPCs a statue can spawn within 600 pixels(?) before it stops spawning.
Default = 6.
* **Field type**: `Int32`
* **Default**: `6`

## StatueSpawnWorld  
How many NPCs a statue can spawn before it stops spawning.
Default = 10.
* **Field type**: `Int32`
* **Default**: `10`

## StorageType  
The type of database to use when storing data (either "sqlite" or "mysql").
* **Field type**: `String`
* **Default**: `sqlite`

## SuperAdminChatPrefix  
The superadmin chat prefix.
* **Field type**: `String`
* **Default**: `(Super Admin) `

## SuperAdminChatRGB  
The chat color for the superadmin group.
#.#.# = Red/Blue/Green
Max value: 255
* **Field type**: `Int32[]`
* **Default**: `System.Int32[]`

## SuperAdminChatSuffix  
The superadmin chat suffix.
* **Field type**: `String`
* **Default**: ``

## SuppressPermissionFailureNotices  
Whether or not the server should suppress build permission failure warnings from regions, spawn point, or server edit failure.
* **Field type**: `Boolean`
* **Default**: `False`

## TileKillThreshold  
Disables a player and reverts their actions if this number of tile kills is exceeded within 1 second.
* **Field type**: `Int32`
* **Default**: `60`

## TileLiquidThreshold  
Disables a player if this number of liquid sets is exceeded within 1 second.
* **Field type**: `Int32`
* **Default**: `50`

## TilePaintThreshold  
Disables a player if this number of tiles is painted within 1 second.
* **Field type**: `Int32`
* **Default**: `15`

## TilePlaceThreshold  
Disables a player and reverts their actions if this number of tile places is exceeded within 1 second.
* **Field type**: `Int32`
* **Default**: `32`

## UseServerName  
Whether or not to use ServerName in place of the world name.
* **Field type**: `Boolean`
* **Default**: `False`

## UseSqlLogs  
Whether or not to save logs to the SQL database instead of a text file.
Default = false.
* **Field type**: `Boolean`
* **Default**: `False`

## WhitelistKickReason  
The reason given when kicking players for not being on the whitelist.
* **Field type**: `String`
* **Default**: `You are not on the whitelist.`

