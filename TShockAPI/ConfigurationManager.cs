using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TShockAPI
{
    /// <summary>
    /// Provides all the stupid little variables a home away from home.
    /// </summary>
    class ConfigurationManager
    {
        public static int invasionMultiplier = 1;
        public static int defaultMaxSpawns = 4;
        public static int defaultSpawnRate = 700;
        public static int serverPort = 7777;
        public static bool enableWhitelist = false;
        public static bool infiniteInvasion = false;
        public static bool permaPvp = false;
        public static int killCount = 0;
        public static bool startedInvasion = false;
        public static bool kickCheater = true;
        public static bool banCheater = true;
        public static bool kickGriefer = true;
        public static bool banGriefer = true;
        public static bool banTnt = false;
        public static bool kickTnt = false;
        public static bool banBoom = true;
        public static bool kickBoom = true;
        public static bool spawnProtect = true;
        public static int spawnProtectRadius = 5;
        public static string distributationAgent = "facepunch";
        public static int authToken = 0;
        public static int maxSlots = 8;

        public enum NPCList : int
        {
            WORLD_EATER = 0,
            EYE = 1,
            SKELETRON = 2
        }

        public static void ReadJsonConfiguration()
        {
            TextReader tr = new StreamReader(FileTools.SaveDir + "config.json");
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
            Terraria.NPC.maxSpawns = defaultMaxSpawns;
            Terraria.NPC.defaultSpawnRate = defaultSpawnRate;
        }

        public static void WriteJsonConfiguration()
        {
            if (!System.IO.Directory.Exists(FileTools.SaveDir))
            {
                System.IO.Directory.CreateDirectory(FileTools.SaveDir);
            }
            if (System.IO.File.Exists(FileTools.SaveDir + "config.json"))
            {
                return;
            }
            else
                FileTools.CreateFile(FileTools.SaveDir + "config.json");
            ConfigFile cfg = new ConfigFile();
            cfg.InvasionMultiplier = 50;
            cfg.DefaultMaximumSpawns = 4;
            cfg.DefaultSpawnRate = 700;
            cfg.ServerPort = 7777;
            cfg.EnableWhitelist = false;
            cfg.InfiniteInvasion = false;
            cfg.AlwaysPvP = false;
            cfg.KickCheaters = kickCheater;
            cfg.BanCheaters = banCheater;
            cfg.KickGriefers = kickGriefer;
            cfg.BanGriefers = banGriefer;
            cfg.BanKillTileAbusers = true;
            cfg.KickKillTileAbusers = true;
            cfg.BanExplosives = true;
            cfg.KickExplosives = true;
            cfg.SpawnProtection = true;
            cfg.SpawnProtectionRadius = 5;
            cfg.MaxSlots = maxSlots;

            string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            TextWriter tr = new StreamWriter(FileTools.SaveDir + "config.json");
            tr.Write(json);
            tr.Close();
        }
    }
}