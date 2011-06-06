namespace TShockAPI
{
    internal class ConfigFile
    {
        public int InvasionMultiplier = 1;
        public int DefaultMaximumSpawns = 4;
        public int DefaultSpawnRate = 700;
        public int ServerPort = 7777;
        public bool EnableWhitelist;
        public bool InfiniteInvasion;
        public bool AlwaysPvP;
        public bool KickCheaters = true;
        public bool BanCheaters = true;
        public bool KickGriefers = true;
        public bool BanGriefers = true;
        public bool BanKillTileAbusers;
        public bool KickKillTileAbusers;
        public bool BanExplosives = true;
        public bool KickExplosives = true;
        public bool SpawnProtection = true;
        public int SpawnProtectionRadius = 5;
        public string DistributationAgent = "facepunch";
        public int MaxSlots = 8;
    }
}