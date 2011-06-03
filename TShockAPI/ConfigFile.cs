using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI
{
    class ConfigFile
    {
        public ConfigFile() { }
        public int InvasionMultiplier = 1;
        public int DefaultMaximumSpawns = 4;
        public int DefaultSpawnRate = 700;
        public int ServerPort = 7777;
        public bool EnableWhitelist = false;
        public bool InfiniteInvasion = false;
        public bool AlwaysPvP = false;
        public bool KickCheaters = true;
        public bool BanCheaters = true;
        public bool KickGriefers = true;
        public bool BanGriefers = true;
        public bool BanKillTileAbusers = false;
        public bool KickKillTileAbusers = false;
    }
}
