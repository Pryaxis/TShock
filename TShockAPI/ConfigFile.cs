/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TShockAPI
{
	public class ConfigFile
	{
		[Description(
			"The equation for calculating invasion size is 100 + (multiplier * (number of active players with greater than 200 health))"
			)] public int InvasionMultiplier = 1;

		[Description("The default maximum mobs that will spawn per wave. Higher means more mobs in that wave.")] public int
			DefaultMaximumSpawns = 5;

		[Description("The delay between waves. Shorter values lead to less mobs.")] public int DefaultSpawnRate = 600;
		[Description("The port the server runs on.")] public int ServerPort = 7777;
		[Description("Enable or disable the whitelist based on IP addresses in whitelist.txt")] public bool EnableWhitelist;

		[Description(
			"Enable the ability for invaison size to never decrease. Make sure to run /invade, and note that this adds 2 million+ goblins to the spawn que for the map."
			)] public bool InfiniteInvasion;

		[Description("Set the server pvp mode. Vaild types are, \"normal\", \"always\", \"disabled\"")] public string PvPMode
			= "normal";

		[Description("Prevents tiles from being placed within SpawnProtectionRadius of the default spawn.")] public bool
			SpawnProtection = true;

		[Description("Radius from spawn tile for SpawnProtection.")] public int SpawnProtectionRadius = 10;

		[Description(
			"Max slots for the server. If you want people to be kicked with \"Server is full\" set this to how many players you want max and then set Terraria max players to 2 higher."
			)] public int MaxSlots = 8;

		[Description("Global protection agent for any block distance based anti-grief check.")] public bool RangeChecks = true;
		[Description("Disables any building; placing of blocks")] public bool DisableBuild;

		[Description("#.#.#. = Red/Blue/Green - RGB Colors for the Admin Chat Color. Max value: 255")] public float[]
			SuperAdminChatRGB = {255, 0, 0};

		[Description("Super admin group chat prefix")] public string SuperAdminChatPrefix = "(Admin) ";
		[Description("Super admin group chat suffix")] public string SuperAdminChatSuffix = "";

		[Description(
			"Backup frequency in minutes. So, a value of 60 = 60 minutes. Backups are stored in the \\tshock\\backups folder.")] public int BackupInterval;

		[Description("How long backups are kept in minutes. 2880 = 2 days.")] public int BackupKeepFor = 60;

		[Description(
			"Remembers where a player left off. It works by remembering the IP, NOT the character.  \neg. When you try to disconnect, and reconnect to be automatically placed at spawn, you'll be at your last location. Note: Won't save after server restarts."
			)] public bool RememberLeavePos;

		[Description("Hardcore players ONLY. This means softcore players cannot join.")] public bool HardcoreOnly;
		[Description("Mediumcore players ONLY. This means softcore players cannot join.")] public bool MediumcoreOnly;
		[Description("Kicks a Hardcore player on death.")] public bool KickOnMediumcoreDeath;
		[Description("Bans a Hardcore player on death.")] public bool BanOnMediumcoreDeath;

		[Description("Enable/Disable Terrarias built in auto save")] public bool AutoSave = true;

		[Description("Number of failed login attempts before kicking the player.")] public int MaximumLoginAttempts = 3;

		[Description("Not implemented")] public string RconPassword = "";
		[Description("Not implemented")] public int RconPort = 7777;

		[Description("Used when replying to a rest /status request.")] public string ServerName = "";
		[Description("Not implemented")] public string MasterServer = "127.0.0.1";

		[Description("Valid types are \"sqlite\" and \"mysql\"")] public string StorageType = "sqlite";

		[Description("The MySQL Hostname and port to direct connections to")] public string MySqlHost = "localhost:3306";
		[Description("Database name to connect to")] public string MySqlDbName = "";
		[Description("Database username to connect with")] public string MySqlUsername = "";
		[Description("Database password to connect with")] public string MySqlPassword = "";

		[Description("Bans a Mediumcore player on death.")] public string MediumcoreBanReason = "Death results in a ban";
		[Description("Kicks a Mediumcore player on death.")] public string MediumcoreKickReason = "Death results in a kick";

		[Description("Enables DNS resolution of incoming connections with GetGroupForIPExpensive.")] public bool
			EnableDNSHostResolution;

		[Description("Enables kicking of banned users by matching their IP Address")] public bool EnableIPBans = true;

		[Description("Enables kicking of banned users by matching their Character Name")] public bool EnableBanOnUsernames;

		[Description("Selects the default group name to place new registrants under")] public string
			DefaultRegistrationGroupName = "default";

		[Description("Selects the default group name to place non registered users under")] public string
			DefaultGuestGroupName = "guest";

		[Description("Force-Disable printing logs to players with the log permission")] public bool DisableSpewLogs = true;

		[Description("Valid types are \"sha512\", \"sha256\", \"md5\", append with \"-xp\" for the xp supported algorithms")] public string HashAlgorithm = "sha512";

		[Description("Buffers up the packets and sends them out at the end of each frame")] public bool BufferPackets = true;

		[Description("String that is used when kicking people when the server is full.")] public string ServerFullReason =
			"Server is full";

	    [Description("String that is used when a user is kicked due to not being on the whitelist.")] public string WhitelistKickReason = "You are not on the whitelist.";

		[Description("String that is used when kicking people when the server is full with no reserved slots.")] public string
			ServerFullNoReservedReason = "Server is full. No reserved slots open.";

		[Description("This will save the world if Terraria crashes from an unhandled exception.")] public bool
			SaveWorldOnCrash = true;

		[Description("This will announce a player's location on join")] public bool EnableGeoIP;

		[Description("This will turn on a token requirement for the /status API endpoint.")] public bool
			EnableTokenEndpointAuthentication;

		[Description("This is used when the API endpoint /status is queried.")] public string ServerNickname = "TShock Server";

		[Description("Enable/Disable the rest api.")] public bool RestApiEnabled;

		[Description("This is the port which the rest api will listen on.")] public int RestApiPort = 7878;

		[Description("Disable tombstones for all players.")] public bool DisableTombstones = true;

		[Description("Displays a player's IP on join to everyone who has the log permission")] public bool DisplayIPToAdmins;

		[Description(
			"Some tiles are 'fixed' by not letting TShock handle them. Disabling this may break certain asthetic tiles.")] public
			bool EnableInsecureTileFixes = true;

		[Description("Kicks users using a proxy as identified with the GeoIP database")] public bool KickProxyUsers = true;

		[Description("Disables hardmode, can't never be activated. Overrides /starthardmode")] public bool DisableHardmode;

		[Description("Disables Dungeon Guardian from being spawned by player packets, this will instead force a respawn")] public bool DisableDungeonGuardian;

		[Description("Enable Server Side Inventory checks, EXPERIMENTAL")] public bool ServerSideInventory;

        [Description("How often SSI should save, in minutes")] public int ServerSideInventorySave = 15;

		[Description("Time, in milliseconds, to disallow discarding items after logging in when ServerSideInventory is ON")] public int LogonDiscardThreshold=250;
		
		[Description("Disables reporting of playercount to the stat system.")] public bool DisablePlayerCountReporting;

		[Description("Disables clown bomb projectiles from spawning")] public bool DisableClownBombs;

		[Description("Disables snow ball projectiles from spawning")] public bool DisableSnowBalls;

		[Description(
			"Change ingame chat format, {0} = Group Name, {1} = Group Prefix, {2} = Player Name, {3} = Group Suffix, {4} = Chat Message"
			)] public string ChatFormat = "{1}{2}{3}: {4}";

		[Description("Force the world time to be normal, day, or night")] public string ForceTime = "normal";

		[Description("Disable/Revert a player if they exceed this number of tile kills within 1 second.")] public int
			TileKillThreshold = 60;

		[Description("Disable/Revert a player if they exceed this number of tile places within 1 second.")] public int
			TilePlaceThreshold = 20;

		[Description("Disable a player if they exceed this number of liquid sets within 1 second.")] public int
			TileLiquidThreshold = 15;

		[Description("Disable a player if they exceed this number of projectile new within 1 second.")] public int
			ProjectileThreshold = 50;

		[Description("Ignore shrapnel from crystal bullets for Projectile Threshold.")] public bool 
			ProjIgnoreShrapnel = true;
		
		[Description("Require all players to register or login before being allowed to play.")] public bool RequireLogin;

		[Description(
			"Disables Invisibility potions from being used in PvP (Note, they can use them on the client, but the effect isn't sent to the rest of the server)"
			)] public bool DisableInvisPvP;

		[Description("The maximum distance players disabled for various reasons can move from")] public int
			MaxRangeForDisabled = 10;

		[Description("Server password required to join server")] public string ServerPassword = "";

		[Description("Protect chests with region and build permissions")] public bool RegionProtectChests;

		[Description("Disable users from being able to login with account password when joining")] public bool
			DisableLoginBeforeJoin;

		[Description("Allows users to register any username with /register")] public bool AllowRegisterAnyUsername;

		[Description("Allows users to login with any username with /login")] public bool AllowLoginAnyUsername = true;

        [Description("The maximum damage a player/npc can inflict")] public int MaxDamage = 175;
        
        [Description("The maximum damage a projectile can inflict")] public int MaxProjDamage = 175;

        [Description("Ignores checking to see if player 'can' update a projectile")] public bool IgnoreProjUpdate = false;
		
        [Description("Ignores checking to see if player 'can' kill a projectile")] public bool IgnoreProjKill = false;

	    [Description("Ignores all no clip checks for players")] public bool IgnoreNoClip = false;

	    [Description("Allow Ice placement even when user does not have canbuild")] public bool AllowIce = false;

	    [Description("Allows corrutption to spread when a world is hardmode.")] public bool AllowCorruptionCreep = true;

        [Description("Allows hallow to spread when a world is hardmode.")] public bool AllowHallowCreep = true;

        public static ConfigFile Read(string path)
		{
			if (!File.Exists(path))
				return new ConfigFile();
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				return Read(fs);
			}
		}

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

		public void Write(string path)
		{
			using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
			{
				Write(fs);
			}
		}

		public void Write(Stream stream)
		{
			var str = JsonConvert.SerializeObject(this, Formatting.Indented);
			using (var sw = new StreamWriter(stream))
			{
				sw.Write(str);
			}
		}

		public static Action<ConfigFile> ConfigRead;


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

				sb.AppendLine("## {0}  ".SFormat(name));
				sb.AppendLine("**Type:** {0}  ".SFormat(type));
				sb.AppendLine("**Description:** {0}  ".SFormat(desc));
				sb.AppendLine("**Default:** \"{0}\"  ".SFormat(def));
				sb.AppendLine();
			}

			File.WriteAllText("ConfigDescriptions.txt", sb.ToString());
		}
	}
}