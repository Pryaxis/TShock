/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Rests;

namespace TShockAPI
{
	/// <summary>The config file class, which contains the configuration for a server that is serialized into JSON and deserialized on load.</summary>
	public class ConfigFile
	{
		/// <summary>Determines the size of invasion events. The equation for calculating invasion size = 100 + (multiplier * (number of active players > 200 hp)).</summary>
		[Description("Determines the size of invasion events.\nThe equation for calculating invasion size is 100 + (multiplier * (number of active players with greater than 200 health)).")]
		public int InvasionMultiplier = 1;

		/// <summary>The default maximum number of mobs that will spawn per wave. Higher means more mobs in that wave.</summary>
		[Description("The default maximum number of mobs that will spawn per wave. Higher means more mobs in that wave.")]
		public int DefaultMaximumSpawns = 5;

		/// <summary>The delay between waves. Lower values lead to more mobs.</summary>
		[Description("The delay between waves. Lower values lead to more mobs.")]
		public int DefaultSpawnRate = 600;

		/// <summary>The port the server runs on.</summary>
		[Description("The port the server runs on.")]
		public int ServerPort = 7777;

		/// <summary>Enable or disable the whitelist based on IP addresses in the whitelist.txt file.</summary>
		[Description("Enable or disable the whitelist based on IP addresses in the whitelist.txt file.")]
		public bool EnableWhitelist;

		/// <summary>Enables never-ending invasion events. You still need to start the event.</summary>
		[Description("Enables never ending invasion events. You still need to start the event, such as with the /invade command.")]
		public bool InfiniteInvasion;

		/// <summary>Sets the PvP mode. Valid types are: "normal", "always", "disabled".</summary>
		[Description("Sets the PvP mode. Valid types are: \"normal\", \"always\" and \"disabled\".")]
		public string PvPMode = "normal";

		/// <summary>Prevents tiles from being placed within SpawnProtectionRadius of the default spawn.</summary>
		[Description("Prevents tiles from being placed within SpawnProtectionRadius of the default spawn.")]
		public bool SpawnProtection = true;

		/// <summary>The tile radius around the spawn tile that is protected by the SpawnProtection setting.</summary>
		[Description("The tile radius around the spawn tile that is protected by the SpawnProtection setting.")]
		public int SpawnProtectionRadius = 10;

		/// <summary>Maximum number of clients connected at once. If lower than Terraria's setting, the server will kick excess connections.</summary>
		[Description("Maximum number of clients connected at once.\nIf you want people to be kicked with \"Server is full\" set this to how many players you want max and then set Terraria max players to 2 higher.")]
		public int MaxSlots = 8;

		/// <summary>Enable or disable anti-cheat range checks based on distance between the player and their block placements.</summary>
		[Description("Enable or disable anti-cheat range checks based on distance between the player and their block placements.")]
		public bool RangeChecks = true;

		/// <summary>Disables any placing, or removal of blocks.</summary>
		[Description("Disables any placing, or removal of blocks.")]
		public bool DisableBuild;

		/// <summary>The chat color for the superadmin group.</summary>
		[Description("The chat color for the superadmin group.\n#.#.# = Red/Blue/Green\nMax value: 255")]
		public int[] SuperAdminChatRGB = { 255, 255, 255 };

		/// <summary>The superadmin chat prefix.</summary>
		[Description("The superadmin chat prefix.")]
		public string SuperAdminChatPrefix = "(Super Admin) ";

		/// <summary>The superadmin chat suffix.</summary>
		[Description("The superadmin chat suffix.")]
		public string SuperAdminChatSuffix = "";

		/// <summary>The interval between backups, in minutes. Backups are stored in the tshock/backups folder.</summary>
		[Description("The interval between backups, in minutes. Backups are stored in the tshock/backups folder.")]
		public int BackupInterval;

		/// <summary>For how long backups are kept in minutes.</summary>
		[Description("For how long backups are kept in minutes.\neg. 2880 = 2 days.")]
		public int BackupKeepFor = 60;

		/// <summary>Remembers where a player left off, based on their IP. Does not persist through server restarts.</summary>
		[Description("Remembers where a player left off, based on their IP. Does not persist through server restarts.\neg. When you try to disconnect, and reconnect to be automatically placed at spawn, you'll be at your last location.")]
		public bool RememberLeavePos;

		/// <summary>Prevents non-hardcore players from connecting.</summary>
		[Description("Prevents non-hardcore players from connecting.")]
		public bool HardcoreOnly;

		/// <summary>Prevents softcore players from connecting.</summary>
		[Description("Prevents softcore players from connecting.")]
		public bool MediumcoreOnly;

		/// <summary>Whether or not to kick mediumcore players on death.</summary>
		[Description("Whether or not to kick mediumcore players on death.")]
		public bool KickOnMediumcoreDeath;

		/// <summary>Whether or not to ban mediumcore players on death.</summary>
		[Description("Whether or not to ban mediumcore players on death.")]
		public bool BanOnMediumcoreDeath;

		/// <summary>Enable or disable Terraria's built-in world auto save.</summary>
		[Description("Enable or disable Terraria's built-in world auto save.")]
		public bool AutoSave = true;
		/// <summary>Enable or disable world save announcements.</summary>
		[Description("Enable or disable world save announcements.")]
		public bool AnnounceSave = true;

		/// <summary>Number of failed login attempts before kicking the player.</summary>
		[Description("Number of failed login attempts before kicking the player.")]
		public int MaximumLoginAttempts = 3;

		/// <summary>Replaces the world name during a session if UseServerName is true.</summary>
		[Description("Replaces the world name during a session if UseServerName is true.")]
		public string ServerName = "";
		/// <summary>Whether or not to use ServerName in place of the world name.</summary>
		[Description("Whether or not to use ServerName in place of the world name.")]
		public bool UseServerName = false;

		/// <summary>The type of database to use when storing data (either "sqlite" or "mysql").</summary>
		[Description("The type of database to use when storing data (either \"sqlite\" or \"mysql\").")]
		public string StorageType = "sqlite";

		/// <summary>The MySQL hostname and port to direct connections to.</summary>
		[Description("The MySQL hostname and port to direct connections to.")]
		public string MySqlHost = "localhost:3306";
		/// <summary>The database name to connect to when using MySQL as the database type.</summary>
		[Description("The database name to connect to when using MySQL as the database type.")]
		public string MySqlDbName = "";
		/// <summary>The username used when connecting to a MySQL database.</summary>
		[Description("The username used when connecting to a MySQL database.")]
		public string MySqlUsername = "";
		/// <summary>The password used when connecting to a MySQL database.</summary>
		[Description("The password used when connecting to a MySQL database.")]
		public string MySqlPassword = "";

		/// <summary>The reason given if banning a mediumcore player on death.</summary>
		[Description("The reason given if banning a mediumcore player on death.")]
		public string MediumcoreBanReason = "Death results in a ban";
		/// <summary>The reason given if kicking a mediumcore players on death.</summary>
		[Description("The reason given if kicking a mediumcore players on death.")]
		public string MediumcoreKickReason = "Death results in a kick";

		/// <summary>Enables kicking banned users by matching their IP Address.</summary>
		[Description("Enables kicking banned users by matching their IP Address.")]
		public bool EnableIPBans = true;

		/// <summary>Enables kicking banned users by matching their client UUID.</summary>
		[Description("Enables kicking banned users by matching their client UUID.")]
		public bool EnableUUIDBans = true;

		/// <summary>Enables kicking banned users by matching their Character Name.</summary>
		[Description("Enables kicking banned users by matching their Character Name.")]
		public bool EnableBanOnUsernames;

		/// <summary>The default group name to place newly registered users under.</summary>
		[Description("The default group name to place newly registered users under.")]
		public string DefaultRegistrationGroupName = "default";

		/// <summary>The default group name to place unregistered players under.</summary>
		[Description("The default group name to place unregistered players under.")]
		public string DefaultGuestGroupName = "guest";

		/// <summary>Disables sending logs as messages to players with the log permission.</summary>
		[Description("Disables sending logs as messages to players with the log permission.")]
		public bool DisableSpewLogs = true;

		/// <summary>Prevents OnSecondUpdate checks from writing to the log file.</summary>
		[Description("Prevents OnSecondUpdate checks from writing to the log file.")]
		public bool DisableSecondUpdateLogs = false;

		/// <summary>The hash algorithm used to encrypt user passwords.
		/// Valid types: "sha512", "sha256" and "md5". Append with "-xp" for the xp supported algorithms.</summary>
		[Description("The hash algorithm used to encrypt user passwords. Valid types: \"sha512\", \"sha256\" and \"md5\". Append with \"-xp\" for the xp supported algorithms.")]
		public string HashAlgorithm = "sha512";

		/// <summary>The reason given when kicking players that attempt to join while the server is full.</summary>
		[Description("The reason given when kicking players that attempt to join while the server is full.")]
		public string ServerFullReason = "Server is full";

		/// <summary>The reason given when kicking players for not being on the whitelist.</summary>
		[Description("The reason given when kicking players for not being on the whitelist.")]
		public string WhitelistKickReason = "You are not on the whitelist.";

		/// <summary>The reason given when kicking players that attempt to join while the server is full with no reserved slots available.</summary>
		[Description("The reason given when kicking players that attempt to join while the server is full with no reserved slots available.")]
		public string ServerFullNoReservedReason = "Server is full. No reserved slots open.";

		/// <summary>Whether or not to save the world if the server crashes from an unhandled exception.</summary>
		[Description("Whether or not to save the world if the server crashes from an unhandled exception.")]
		public bool SaveWorldOnCrash = true;

		/// <summary>Whether or not to announce a player's geographic location on join, based on their IP.</summary>
		[Description("Whether or not to announce a player's geographic location on join, based on their IP.")]
		public bool EnableGeoIP;

		/// <summary>Whether or not to require token authentication to use the public REST API endpoints.</summary>
		[Description("Whether or not to require token authentication to use the public REST API endpoints.")]
		public bool EnableTokenEndpointAuthentication;

		/// <summary>Enable or disable the REST API.</summary>
		[Description("Enable or disable the REST API.")]
		public bool RestApiEnabled;

		/// <summary>The port used by the REST API.</summary>
		[Description("The port used by the REST API.")]
		public int RestApiPort = 7878;

		/// <summary>Disables tombstone dropping during death for all players.</summary>
		[Description("Disables tombstone dropping during death for all players.")]
		public bool DisableTombstones = true;

		/// <summary>Displays a player's IP on join to users with the log permission.</summary>
		[Description("Displays a player's IP on join to users with the log permission.")]
		public bool DisplayIPToAdmins;

		/// <summary>If GeoIP is enabled, this will kick users identified as being under a proxy.</summary>
		[Description("If GeoIP is enabled, this will kick users identified as being under a proxy.")]
		public bool KickProxyUsers = true;

		/// <summary>If enabled, hardmode will not be activated by the Wall of Flesh or the /starthardmode command.</summary>
		[Description("If enabled, hardmode will not be activated by the Wall of Flesh or the /starthardmode command.")]
		public bool DisableHardmode;

		/// <summary>Prevents the dungeon guardian from being spawned while sending players to their spawn point instead.</summary>
		[Description("Prevents the dungeon guardian from being spawned while sending players to their spawn point instead.")]
		public bool DisableDungeonGuardian;

		/// <summary>Disables clown bomb projectiles from spawning.</summary>
		[Description("Disables clown bomb projectiles from spawning.")]
		public bool DisableClownBombs;

		/// <summary>Disables snow ball projectiles from spawning.</summary>
		[Description("Disables snow ball projectiles from spawning.")]
		public bool DisableSnowBalls;

		/// <summary>Changes in-game chat format: {0} = Group Name, {1} = Group Prefix, {2} = Player Name, {3} = Group Suffix, {4} = Chat Message.</summary>
		[Description("Changes in-game chat format: {0} = Group Name, {1} = Group Prefix, {2} = Player Name, {3} = Group Suffix, {4} = Chat Message.")]
		public string ChatFormat = "{1}{2}{3}: {4}";

		/// <summary>Changes the player name when using chat above heads. Starts with a player name wrapped in brackets, as per Terraria's formatting.\nSame formatting as ChatFormat without the message.</summary>
		[Description("Changes the player name when using chat above heads. Starts with a player name wrapped in brackets, as per Terraria's formatting.\nSame formatting as ChatFormat without the message.")]
		public string ChatAboveHeadsFormat = "{2}";

		/// <summary>Forces the world time to be normal, day, or night.</summary>
		[Description("Forces the world time to be normal, day, or night.")]
		public string ForceTime = "normal";

		/// <summary>Disables a player and reverts their actions if this number of tile kills is exceeded within 1 second.</summary>
		[Description("Disables a player and reverts their actions if this number of tile kills is exceeded within 1 second.")]
		public int TileKillThreshold = 60;

		/// <summary>Disables a player and reverts their actions if this number of tile places is exceeded within 1 second.</summary>
		[Description("Disables a player and reverts their actions if this number of tile places is exceeded within 1 second.")]
		public int TilePlaceThreshold = 20;

		/// <summary>Disables a player if this number of liquid sets is exceeded within 1 second.</summary>
		[Description("Disables a player if this number of liquid sets is exceeded within 1 second.")]
		public int TileLiquidThreshold = 15;

		/// <summary>Disable a player if this number of projectiles is created within 1 second.</summary>
		[Description("Disable a player if this number of projectiles is created within 1 second.")]
		public int ProjectileThreshold = 50;

		/// <summary>Disables a player if this number of HealOtherPlayer packets is sent within 1 second.</summary>
		[Description("Disables a player if this number of HealOtherPlayer packets is sent within 1 second.")]
		public int HealOtherThreshold = 50;

		/// <summary>Whether or not to ignore shrapnel from crystal bullets for the projectile threshold count.</summary>
		[Description("Whether or not to ignore shrapnel from crystal bullets for the projectile threshold count.")]
		public bool ProjIgnoreShrapnel = true;

		/// <summary>Require all players to register or login before being allowed to play.</summary>
		[Description("Require all players to register or login before being allowed to play.")]
		public bool RequireLogin;

		/// <summary>Disables the effect of invisibility potions while PvP is enabled by turning the player visible to the other clients.</summary>
		[Description("Disables the effect of invisibility potions while PvP is enabled by turning the player visible to the other clients.")]
		public bool DisableInvisPvP;

		/// <summary>The maximum distance, in tiles, that disabled players can move from.</summary>
		[Description("The maximum distance, in tiles, that disabled players can move from.")]
		public int MaxRangeForDisabled = 10;

		/// <summary>The server password required to join the server.</summary>
		[Description("The server password required to join the server.")]
		public string ServerPassword = "";

		/// <summary>Whether or not region protection should apply to chests.</summary>
		[Description("Whether or not region protection should apply to chests.")]
		public bool RegionProtectChests;

		/// <summary>Whether or not region protection should apply to gem locks.</summary>
		[Description("Whether or not region protection should apply to gem locks.")]
		public bool RegionProtectGemLocks = true;

		/// <summary>Prevents users from being able to login before they finish connecting.</summary>
		[Description("Prevents users from being able to login before they finish connecting.")]
		public bool DisableLoginBeforeJoin;

		/// <summary>Prevents users from being able to login with their client UUID.</summary>
		[Description("Prevents users from being able to login with their client UUID.")]
		public bool DisableUUIDLogin;

		/// <summary>Kick clients that don't send their UUID to the server.</summary>
		[Description("Kick clients that don't send their UUID to the server.")]
		public bool KickEmptyUUID;

		/// <summary>Allows users to register a username that doesn't necessarily match their character name.</summary>
		[Description("Allows users to register a username that doesn't necessarily match their character name.")]
		public bool AllowRegisterAnyUsername;

		/// <summary>Allows users to login to any account even if the username doesn't match their character name.</summary>
		[Description("Allows users to login to any account even if the username doesn't match their character name.")]
		public bool AllowLoginAnyUsername = true;

		/// <summary>The maximum damage a player/NPC can inflict.</summary>
		[Description("The maximum damage a player/NPC can inflict.")]
		public int MaxDamage = 1175;

		/// <summary>The maximum damage a projectile can inflict.</summary>
		[Description("The maximum damage a projectile can inflict.")]
		public int MaxProjDamage = 1175;

		/// <summary>Whether or not to kick users when they surpass the MaxDamage threshold.</summary>
		[Description("Whether or not to kick users when they surpass the MaxDamage threshold.")]
		public bool KickOnDamageThresholdBroken = false;

		/// <summary>Ignores checks to see if a player 'can' update a projectile.</summary>
		[Description("Ignores checks to see if a player 'can' update a projectile.")]
		public bool IgnoreProjUpdate = false;

		/// <summary>Ignores checks to see if a player 'can' kill a projectile.</summary>
		[Description("Ignores checks to see if a player 'can' kill a projectile.")]
		public bool IgnoreProjKill = false;

		/// <summary>Allows ice placement even where a user cannot usually build.</summary>
		[Description("Allows ice placement even where a user cannot usually build.")]
		public bool AllowIce = false;

		/// <summary>Allows the crimson to spread when a world is in hardmode.</summary>
		[Description("Allows the crimson to spread when a world is in hardmode.")]
		public bool AllowCrimsonCreep = true;

		/// <summary>Allows the corruption to spread when a world is in hardmode.</summary>
		[Description("Allows the corruption to spread when a world is in hardmode.")]
		public bool AllowCorruptionCreep = true;

		/// <summary>Allows the hallow to spread when a world is in hardmode.</summary>
		[Description("Allows the hallow to spread when a world is in hardmode.")]
		public bool AllowHallowCreep = true;

		/// <summary>How many NPCs a statue can spawn within 200 pixels(?) before it stops spawning.</summary>
		[Description("How many NPCs a statue can spawn within 200 pixels(?) before it stops spawning.\nDefault = 3.")]
		public int StatueSpawn200 = 3;

		/// <summary>How many NPCs a statue can spawn within 600 pixels(?) before it stops spawning.</summary>
		[Description("How many NPCs a statue can spawn within 600 pixels(?) before it stops spawning.\nDefault = 6.")]
		public int StatueSpawn600 = 6;

		/// <summary>How many NPCs a statue can spawn before it stops spawning.</summary>
		[Description("How many NPCs a statue can spawn before it stops spawning.\nDefault = 10.")]
		public int StatueSpawnWorld = 10;

		/// <summary>Prevent banned items from being spawned or given with commands.</summary>
		[Description("Prevent banned items from being spawned or given with commands.")]
		public bool PreventBannedItemSpawn = false;

		/// <summary>Prevent players from interacting with the world while they are dead.</summary>
		[Description("Prevent players from interacting with the world while they are dead.")]
		public bool PreventDeadModification = true;

		/// <summary>Whether or not to display chat messages above players' heads.</summary>
		[Description("Whether or not to display chat messages above players' heads.")]
		public bool EnableChatAboveHeads = false;

		/// <summary>Forces Christmas-only events to occur all year.</summary>
		[Description("Forces Christmas-only events to occur all year.")]
		public bool ForceXmas = false;

		/// <summary>Allows groups on the banned item allowed list to spawn banned items even if PreventBannedItemSpawn is set to true.</summary>
		[Description("Allows groups on the banned item allowed list to spawn banned items even if PreventBannedItemSpawn is set to true.")]
		public bool AllowAllowedGroupsToSpawnBannedItems = false;

		/// <summary>Allows stacks in chests to go beyond the stack limit during world loading.</summary>
		[Description("Allows stacks in chests to go beyond the stack limit during world loading.")]
		public bool IgnoreChestStacksOnLoad = false;

		/// <summary>The path to the directory where logs should be written to.</summary>
		[Description("The path to the directory where logs should be written to.")]
		public string LogPath = "tshock";

		/// <summary>Whether or not to save logs to the SQL database instead of a text file.</summary>
		[Description("Whether or not to save logs to the SQL database instead of a text file.\nDefault = false.")]
		public bool UseSqlLogs = false;

		/// <summary>Number of times the SQL log must fail to insert logs before falling back to the text log.</summary>
		[Description("Number of times the SQL log must fail to insert logs before falling back to the text log.")]
		public int RevertToTextLogsOnSqlFailures = 10;

		/// <summary>Prevents players from placing tiles with an invalid style.</summary>
		[Description("Prevents players from placing tiles with an invalid style.")]
		public bool PreventInvalidPlaceStyle = true;

		/// <summary>The RGB values used for the color of broadcast messages.</summary>
		[Description("The RGB values used for the color of broadcast messages.\n#.#.# = Red/Blue/Green\nMax value: 255")]
		public int[] BroadcastRGB = { 127, 255, 212 };

		/// <summary>A dictionary of REST tokens that external applications may use to make queries to your server.</summary>
		[Description("A dictionary of REST tokens that external applications may use to make queries to your server.")]
		public Dictionary<string, SecureRest.TokenData> ApplicationRestTokens = new Dictionary<string, SecureRest.TokenData>();

		/// <summary>The number of reserved slots past your max server slots that can be joined by reserved players.</summary>
		[Description("The number of reserved slots past your max server slots that can be joined by reserved players.")]
		public int ReservedSlots = 20;

		/// <summary>Whether or not to log REST API connections.</summary>
		[Description("Whether or not to log REST API connections.")]
		public bool LogRest = false;

		/// <summary>The number of seconds a player must wait before being respawned.</summary>
		[Description("The number of seconds a player must wait before being respawned.")]
		public int RespawnSeconds = 5;

		/// <summary>The number of seconds a player must wait before being respawned if there is a boss nearby.</summary>
		[Description("The number of seconds a player must wait before being respawned if there is a boss nearby.")]
		public int RespawnBossSeconds = 10;

		/// <summary>Disables a player if this number of tiles is painted within 1 second.</summary>
		[Description("Disables a player if this number of tiles is painted within 1 second.")]
		public int TilePaintThreshold = 15;

		/// <summary>Forces Halloween-only events to occur all year.</summary>
		[Description("Forces Halloween-only events to occur all year.")]
		public bool ForceHalloween = false;

		/// <summary>Allows players to break temporary tiles (grass, pots, etc) where they cannot usually build.</summary>
		[Description("Allows players to break temporary tiles (grass, pots, etc) where they cannot usually build.")]
		public bool AllowCutTilesAndBreakables = false;

		/// <summary>Specifies which string starts a command.
		/// Note: Will not function properly if the string length is bigger than 1.</summary>
		[Description("Specifies which string starts a command.\nNote: Will not function properly if the string length is bigger than 1.")]
		public string CommandSpecifier = "/";

		/// <summary>Specifies which string starts a command silently.
		/// Note: Will not function properly if the string length is bigger than 1.</summary>
		[Description("Specifies which string starts a command silently.\nNote: Will not function properly if the string length is bigger than 1.")]
		public string CommandSilentSpecifier = ".";

		/// <summary>Whether or not to kick hardcore players on death.</summary>
		[Description("Whether or not to kick hardcore players on death.")]
		public bool KickOnHardcoreDeath;

		/// <summary>Whether or not to ban hardcore players on death.</summary>
		[Description("Whether or not to ban hardcore players on death.")]
		public bool BanOnHardcoreDeath;

		/// <summary>The reason given when banning hardcore players on death.</summary>
		[Description("The reason given when banning hardcore players on death.")]
		public string HardcoreBanReason = "Death results in a ban";

		/// <summary>The reason given when kicking hardcore players on death.</summary>
		[Description("The reason given when kicking hardcore players on death.")]
		public string HardcoreKickReason = "Death results in a kick";

		/// <summary>Whether or not to announce boss spawning or invasion starts.</summary>
		[Description("Whether or not to announce boss spawning or invasion starts.")]
		public bool AnonymousBossInvasions = true;

		/// <summary>The maximum HP a player can have, before equipment buffs.</summary>
		[Description("The maximum HP a player can have, before equipment buffs.")]
		public int MaxHP = 500;

		/// <summary>The maximum MP a player can have, before equipment buffs.</summary>
		[Description("The maximum MP a player can have, before equipment buffs.")]
		public int MaxMP = 200;

		/// <summary>Whether or not to save the world when the last player disconnects.</summary>
		[Description("Whether or not to save the world when the last player disconnects.")]
		public bool SaveWorldOnLastPlayerExit = true;

		/// <summary>Determines the BCrypt work factor to use. If increased, all passwords will be upgraded to new work-factor on verify.
		/// The number of computational rounds is 2^n. Increase with caution. Range: 5-31.</summary>
		[Description("Determines the BCrypt work factor to use. If increased, all passwords will be upgraded to new work-factor on verify. The number of computational rounds is 2^n. Increase with caution. Range: 5-31.")]
		public int BCryptWorkFactor = 7;

		/// <summary>The minimum password length for new user accounts. Can never be lower than 4.</summary>
		[Description("The minimum password length for new user accounts. Can never be lower than 4.")]
		public int MinimumPasswordLength = 4;

		/// <summary>The maximum REST requests in the bucket before denying requests. Minimum value is 5.</summary>
		[Description("The maximum REST requests in the bucket before denying requests. Minimum value is 5.")]
		public int RESTMaximumRequestsPerInterval = 5;

		/// <summary>How often in minutes the REST requests bucket is decreased by one. Minimum value is 1 minute.</summary>
		[Description("How often in minutes the REST requests bucket is decreased by one. Minimum value is 1 minute.")]
		public int RESTRequestBucketDecreaseIntervalMinutes = 1;

		/// <summary>Whether or not to show backup auto save messages.</summary>
		[Description("Whether or not to show backup auto save messages.")]
		public bool ShowBackupAutosaveMessages = true;

		/// <summary>
		/// Reads a configuration file from a given path
		/// </summary>
		/// <param name="path">string path</param>
		/// <returns>ConfigFile object</returns>
		public static ConfigFile Read(string path)
		{
			if (!File.Exists(path))
				return new ConfigFile();
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Read(fs);
			}
		}

		/// <summary>
		/// Reads the configuration file from a stream
		/// </summary>
		/// <param name="stream">stream</param>
		/// <returns>ConfigFile object</returns>
		public static ConfigFile Read(Stream stream)
		{
			using (var sr = new StreamReader(stream))
			{
				var cf = JsonConvert.DeserializeObject<ConfigFile>(sr.ReadToEnd());
				if (ConfigRead != null)
					ConfigRead(cf);
				return cf;
			}
		}

		/// <summary>
		/// Writes the configuration to a given path
		/// </summary>
		/// <param name="path">string path - Location to put the config file</param>
		public void Write(string path)
		{
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				Write(fs);
			}
		}

		/// <summary>
		/// Writes the configuration to a stream
		/// </summary>
		/// <param name="stream">stream</param>
		public void Write(Stream stream)
		{
			var str = JsonConvert.SerializeObject(this, Formatting.Indented);
			using (var sw = new StreamWriter(stream))
			{
				sw.Write(str);
			}
		}

		/// <summary>
		/// On config read hook
		/// </summary>
		public static Action<ConfigFile> ConfigRead;

		/// <summary>
		/// Dumps all configuration options to a text file in Markdown format
		/// </summary>
		public static void DumpDescriptions()
		{
			var sb = new StringBuilder();
			var defaults = new ConfigFile();

			foreach (var field in defaults.GetType().GetFields().OrderBy(f => f.Name))
			{
				if (field.IsStatic)
					continue;

				var name = field.Name;
				var type = field.FieldType.Name;

				var descattr =
					field.GetCustomAttributes(false).FirstOrDefault(o => o is DescriptionAttribute) as DescriptionAttribute;
				var desc = descattr != null && !string.IsNullOrWhiteSpace(descattr.Description) ? descattr.Description : "None";

				var def = field.GetValue(defaults);

				sb.AppendLine("{0}  ".SFormat(name));
				sb.AppendLine("Type: {0}  ".SFormat(type));
				sb.AppendLine("Description: {0}  ".SFormat(desc));
				sb.AppendLine("Default: \"{0}\"  ".SFormat(def));
				sb.AppendLine();
			}

			File.WriteAllText("ConfigDescriptions.txt", sb.ToString());
		}
	}
}
