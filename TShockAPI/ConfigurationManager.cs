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
using System.IO;
using Newtonsoft.Json;
using Terraria;

namespace TShockAPI
{
    /// <summary>
    /// Provides all the stupid little variables a home away from home.
    /// </summary>
    internal class ConfigurationManager
    {
        //Add default values here and in ConfigFile.cs
        //Values written here will automatically be pulled into a config file on save.
        public static int InvasionMultiplier = 1;
        public static int DefaultMaxSpawns = 4;
        public static int DefaultSpawnRate = 700;
        public static int ServerPort = 7777;
        public static bool EnableWhitelist = false;
        public static bool InfiniteInvasion = false;
        public static bool PermaPvp = false;
        public static int KillCount;
        public static bool KickCheater = true;
        public static bool BanCheater = true;
        public static bool KickGriefer = true;
        public static bool BanGriefer = true;
        public static bool BanTnt = true;
        public static bool KickTnt = true;
        public static bool BanBoom = true;
        public static bool KickBoom = true;
        public static bool DisableBoom = true;
        public static bool SpawnProtect = true;
        public static bool RangeChecks = true;
        public static int SpawnProtectRadius = 5;
        public static string DistributationAgent = "facepunch";
        public static int AuthToken;
        public static int MaxSlots = 8;
        public static bool SpamChecks = false;
        public static bool DisableBuild = false;
        public static float[] AdminChatRGB = {255, 0, 0};
        public static string AdminChatPrefix = "(Admin) ";
        public static bool RememberLeavePos = false;
        public static int TileThreshold = 20;

        /// <summary>
        /// Don't allow pvp changing for x seconds.
        /// </summary>
        public static int PvpThrottle = 0;

        /// <summary>
        /// Backup every x minutes
        /// </summary>
        public static int BackupInterval = 0;
        /// <summary>
        /// Delete backups that are older than x mintues. 
        /// </summary>
        public static int BackupKeepFor = 60;

        /// <summary>
        /// Server will broadcast itself to the server list.
        /// </summary>
        public static bool ListServer = false;

        public static int Spawn_WorldID;
        public static int originalSpawnX;
        public static int originalSpawnY;

        public static bool hardcoreOnly = false;
        public static bool KickOnHardcoreDeath = false;
        public static bool BanOnHardcoreDeath = false;

        public static void ReadJsonConfiguration()
        {
            TextReader tr = new StreamReader(FileTools.ConfigPath);
            ConfigFile cfg = JsonConvert.DeserializeObject<ConfigFile>(tr.ReadToEnd());
            tr.Close();

            InvasionMultiplier = cfg.InvasionMultiplier;
            DefaultMaxSpawns = cfg.DefaultMaximumSpawns;
            DefaultSpawnRate = cfg.DefaultSpawnRate;
            ServerPort = cfg.ServerPort;
            EnableWhitelist = cfg.EnableWhitelist;
            InfiniteInvasion = cfg.InfiniteInvasion;
            PermaPvp = cfg.AlwaysPvP;
            KickCheater = cfg.KickCheaters;
            BanCheater = cfg.BanCheaters;
            KickGriefer = cfg.KickGriefers;
            BanGriefer = cfg.BanGriefers;
            BanTnt = cfg.BanKillTileAbusers;
            KickTnt = cfg.KickKillTileAbusers;
            BanBoom = cfg.BanExplosives;
            KickBoom = cfg.KickExplosives;
            DisableBoom = cfg.DisableExplosives;
            SpawnProtect = cfg.SpawnProtection;
            SpawnProtectRadius = cfg.SpawnProtectionRadius;
            DistributationAgent = cfg.DistributationAgent;
            MaxSlots = cfg.MaxSlots;
            RangeChecks = cfg.RangeChecks;
            SpamChecks = cfg.SpamChecks;
            DisableBuild = cfg.DisableBuild;
            TileThreshold = cfg.TileThreshold;
            NPC.maxSpawns = DefaultMaxSpawns;
            NPC.defaultSpawnRate = DefaultSpawnRate;
            AdminChatRGB = cfg.AdminChatRGB;
            AdminChatPrefix = cfg.AdminChatPrefix;
            PvpThrottle = cfg.PvpThrottle;
            BackupInterval = cfg.BackupInterval;
            BackupKeepFor = cfg.BackupKeepFor;
            ListServer = cfg.ListServer;
            originalSpawnX = Main.spawnTileX;
            originalSpawnY = Main.spawnTileY;
            Main.spawnTileX = cfg.spawnTileX;
            Main.spawnTileY = cfg.spawnTileY;
            Spawn_WorldID = cfg.Spawn_WorldID;
            RememberLeavePos = cfg.RememberLeavePos;
            hardcoreOnly = cfg.HardcoreOnly;
            KickOnHardcoreDeath = cfg.KickOnHardcoreOnlyDeath;
            BanOnHardcoreDeath = cfg.BanOnHardcoreOnlyDeath;
        }

        public static void WriteJsonConfiguration()
        {
            ConfigFile cfg = new ConfigFile();
            cfg.InvasionMultiplier = InvasionMultiplier;
            cfg.DefaultMaximumSpawns = DefaultMaxSpawns;
            cfg.DefaultSpawnRate = DefaultSpawnRate;
            cfg.ServerPort = ServerPort;
            cfg.EnableWhitelist = EnableWhitelist;
            cfg.InfiniteInvasion = InfiniteInvasion;
            cfg.AlwaysPvP = PermaPvp;
            cfg.KickCheaters = KickCheater;
            cfg.BanCheaters = BanCheater;
            cfg.KickGriefers = KickGriefer;
            cfg.BanGriefers = BanGriefer;
            cfg.BanKillTileAbusers = BanTnt;
            cfg.KickKillTileAbusers = KickTnt;
            cfg.BanExplosives = BanBoom;
            cfg.KickExplosives = KickBoom;
            cfg.DisableExplosives = DisableBoom;
            cfg.SpawnProtection = SpawnProtect;
            cfg.SpawnProtectionRadius = SpawnProtectRadius;
            cfg.MaxSlots = MaxSlots;
            cfg.RangeChecks = RangeChecks;
            cfg.SpamChecks = SpamChecks;
            cfg.DisableBuild = DisableBuild;
            cfg.TileThreshold = TileThreshold;
            cfg.AdminChatRGB = AdminChatRGB;
            cfg.AdminChatPrefix = AdminChatPrefix;
            cfg.PvpThrottle = PvpThrottle;
            cfg.BackupInterval = BackupInterval;
            cfg.BackupKeepFor = BackupKeepFor;
            cfg.ListServer = ListServer;
            cfg.spawnTileX = Main.spawnTileX;
            cfg.spawnTileY = Main.spawnTileY;
            cfg.RememberLeavePos = RememberLeavePos;
            cfg.Spawn_WorldID = Spawn_WorldID;
            cfg.HardcoreOnly = hardcoreOnly;
            cfg.BanOnHardcoreOnlyDeath = BanOnHardcoreDeath;
            cfg.KickOnHardcoreOnlyDeath = KickOnHardcoreDeath;
            string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            TextWriter tr = new StreamWriter(FileTools.ConfigPath);
            tr.Write(json);
            tr.Close();
        }
    }
}