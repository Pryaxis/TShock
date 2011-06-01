using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static bool banTnt = false;
        public static bool kickTnt = false;
        public enum NPCList : int
        {
            WORLD_EATER = 0,
            EYE = 1,
            SKELETRON = 2
        }
    }
}
