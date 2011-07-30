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
using System.IO;
using Newtonsoft.Json;

namespace TShockAPI
{
    public class ConfigFile
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
        public bool BanKillTileAbusers = true;
        public bool KickKillTileAbusers = true;
        public bool BanExplosives = true;
        public bool KickExplosives = true;
        public bool DisableExplosives = true;
        public bool SpawnProtection = true;
        public int SpawnProtectionRadius = 5;
        public string DistributationAgent = "facepunch";
        public int MaxSlots = 8;
        public bool RangeChecks = true;
        public bool SpamChecks;
        public bool DisableBuild;
        public int TileThreshold = 60;

        public float[] AdminChatRGB = { 255, 0, 0 };
        public string AdminChatPrefix = "(Admin) ";
        public bool AdminChatEnabled = true;

        public int PvpThrottle;

        public int BackupInterval;
        public int BackupKeepFor = 60;

        public bool RememberLeavePos;

        public bool HardcoreOnly;
        public bool KickOnHardcoreDeath;
        public bool BanOnHardcoreDeath;

        public bool AutoSave = true;

        public int MaximumLoginAttempts = 3;

        public string RconPassword = "";
        public int RconPort = 7777;
        public string ServerName = "";
        public string MasterServer = "127.0.0.1";

        /// <summary>
        /// Valid types are "sqlite" and "mysql"
        /// </summary>
        public string StorageType = "sqlite";

        public string MySqlHost = "localhost:3306";
        public string MySqlDbName = "";
        public string MySqlUsername = "";
        public string MySqlPassword = "";

        public string RangeCheckBanReason = "Placing impossible to place blocks.";
        public string SendSectionAbuseReason = "SendSection abuse.";
        public string NPCSpawnAbuseReason = "Spawn NPC abuse";
        public string UpdatePlayerAbuseReason = "Update Player abuse";
        public string ExplosiveAbuseReason = "Throwing an explosive device.";
        public string KillMeAbuseReason = "Trying to execute KillMe on someone else.";
        public string IllogicalLiquidUseReason = "Manipulating liquid without bucket.";
        public string LiquidAbuseReason = "Placing impossible to place liquid.";
        public string TileKillAbuseReason = "Tile Kill abuse ({0})";
        public string HardcoreBanReason = "Death results in a ban";
        public string HardcoreKickReason = "Death results in a kick";

        public bool EnableDNSHostResolution;

        public bool EnableBanOnUsernames;

        public bool EnableAntiLag = true;

        public string DefaultRegistrationGroupName = "default";

        public bool DisableSpewLogs = true;

        /// <summary>
        /// Valid types are "sha512", "sha256", "md5"
        /// </summary>
        public string HashAlgorithm = "sha512";

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
            using(var sr = new StreamReader(stream))
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
    }
}