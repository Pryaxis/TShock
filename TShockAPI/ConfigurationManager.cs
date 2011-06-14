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
        public static int invasionMultiplier = 1;
        public static int defaultMaxSpawns = 4;
        public static int defaultSpawnRate = 700;
        public static int serverPort = 7777;
        public static bool enableWhitelist = false;
        public static bool infiniteInvasion = false;
        public static bool permaPvp = false;
        public static int killCount;
        public static bool kickCheater = true;
        public static bool banCheater = true;
        public static bool kickGriefer = true;
        public static bool banGriefer = true;
        public static bool banTnt = true;
        public static bool kickTnt = true;
        public static bool banBoom = true;
        public static bool kickBoom = true;
        public static bool spawnProtect = true;
        public static bool rangeChecks = true;
        public static int spawnProtectRadius = 5;
        public static string distributationAgent = "facepunch";
        public static int authToken;
        public static int maxSlots = 8;
        public static bool spamChecks = false;
        public static bool disableBuild = false;
        public static float[] adminChatRGB = {255, 0, 0};
        public static string adminChatPrefix = "(Admin) ";

        public enum NPCList
        {
            WORLD_EATER = 0,
            EYE = 1,
            SKELETRON = 2
        }

        public static void ReadJsonConfiguration()
        {
            TextReader tr = new StreamReader(FileTools.ConfigPath);
            ConfigFile cfg = JsonConvert.DeserializeObject<ConfigFile>(tr.ReadToEnd());
            tr.Close();

            invasionMultiplier = cfg.InvasionMultiplier;
            defaultMaxSpawns = cfg.DefaultMaximumSpawns;
            defaultSpawnRate = cfg.DefaultSpawnRate;
            serverPort = cfg.ServerPort;
            enableWhitelist = cfg.EnableWhitelist;
            infiniteInvasion = cfg.InfiniteInvasion;
            permaPvp = cfg.AlwaysPvP;
            kickCheater = cfg.KickCheaters;
            banCheater = cfg.BanCheaters;
            kickGriefer = cfg.KickGriefers;
            banGriefer = cfg.BanGriefers;
            banTnt = cfg.BanKillTileAbusers;
            kickTnt = cfg.KickKillTileAbusers;
            banBoom = cfg.BanExplosives;
            kickBoom = cfg.KickExplosives;
            spawnProtect = cfg.SpawnProtection;
            spawnProtectRadius = cfg.SpawnProtectionRadius;
            distributationAgent = cfg.DistributationAgent;
            maxSlots = cfg.MaxSlots;
            rangeChecks = cfg.RangeChecks;
            spamChecks = cfg.SpamChecks;
            disableBuild = cfg.DisableBuild;
            NPC.maxSpawns = defaultMaxSpawns;
            NPC.defaultSpawnRate = defaultSpawnRate;
            adminChatRGB = cfg.AdminChatRGB;
            adminChatPrefix = cfg.AdminChatPrefix;
        }

        public static void WriteJsonConfiguration()
        {
            ConfigFile cfg = new ConfigFile();
            cfg.InvasionMultiplier = invasionMultiplier;
            cfg.DefaultMaximumSpawns = defaultMaxSpawns;
            cfg.DefaultSpawnRate = defaultSpawnRate;
            cfg.ServerPort = serverPort;
            cfg.EnableWhitelist = enableWhitelist;
            cfg.InfiniteInvasion = infiniteInvasion;
            cfg.AlwaysPvP = permaPvp;
            cfg.KickCheaters = kickCheater;
            cfg.BanCheaters = banCheater;
            cfg.KickGriefers = kickGriefer;
            cfg.BanGriefers = banGriefer;
            cfg.BanKillTileAbusers = banGriefer;
            cfg.KickKillTileAbusers = kickGriefer;
            cfg.BanExplosives = banBoom;
            cfg.KickExplosives = kickBoom;
            cfg.SpawnProtection = spawnProtect;
            cfg.SpawnProtectionRadius = spawnProtectRadius;
            cfg.MaxSlots = maxSlots;
            cfg.RangeChecks = rangeChecks;
            cfg.SpamChecks = spamChecks;
            cfg.DisableBuild = disableBuild;
            cfg.AdminChatRGB = adminChatRGB;
            cfg.AdminChatPrefix = adminChatPrefix;
            string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            TextWriter tr = new StreamWriter(FileTools.ConfigPath);
            tr.Write(json);
            tr.Close();
        }
    }
}