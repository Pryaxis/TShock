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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TShockAPI
{
    public class ConfigFile
    {
        [Description("The equation for calculating invasion size is 100 + (multiplier * (number of active players with greater than 200 health))")]
        public int InvasionMultiplier = 1;
        [Description("The default maximum mobs that will spawn per wave. Higher means more mobs in that wave.")]
        public int DefaultMaximumSpawns = 4;
        [Description("The delay between waves. Shorter values lead to less mobs.")]
        public int DefaultSpawnRate = 700;
        [Description("The port the server runs on.")]
        public int ServerPort = 7777;
        [Description("Enable or disable the whitelist based on IP addresses in whitelist.txt")]
        public bool EnableWhitelist;
        [Description("Enable the ability for invaison size to never decrease. Make sure to run /invade, and note that this adds 2 million+ goblins to the spawn que for the map.")]
        public bool InfiniteInvasion;
        [Description("Enable or disable perma pvp.")]
        public bool AlwaysPvP;
        [Description("Kicks players who edit save files for cheating.")]
        public bool KickCheaters = true;
        [Description("")]
        public bool BanCheaters = true;
        [Description("")]
        public bool KickGriefers = true;
        [Description("")]
        public bool BanGriefers = true;
        [Description("Bans players who attempt to kill tiles using TNT or exploits. Requires kick to be enabled.")]
        public bool BanKillTileAbusers = true;
        [Description("Kicks players who attempt to kill tiles using TNT or exploits.")]
        public bool KickKillTileAbusers = true;
        [Description("Bans players who attempt to throw a bomb or dynamite.")]
        public bool BanExplosives = true;
        [Description("Kicks players who attempt to throw a bomb or dynamite.")]
        public bool KickExplosives = true;
        [Description("Explosives won't break blocks")]
        public bool DisableExplosives = true;
        [Description("Prevents tiles from being placed within SpawnProtectionRadius of the default spawn.")]
        public bool SpawnProtection = true;
        [Description("Radius from spawn tile for SpawnProtection.")]
        public int SpawnProtectionRadius = 5;
        [Description("Max slots for the server. If you want people to be kicked with \"Server is full\" set this to how many players you want max and then set Terraria max players to 2 higher.")]
        public int MaxSlots = 8;
        [Description("Global protection agent for any block distance based anti-grief check.")]
        public bool RangeChecks = true;
        [Description("Enable/Disable Terrarias built in spam checks.")]
        public bool SpamChecks;
        [Description("Disables any building; placing of blocks")]
        public bool DisableBuild;
        [Description("Kick a player if they exceed this number of tile kills within 1 second.")]
        public int TileThreshold = 60;
        [Description("#.#.#. = Red/Blue/Green - RGB Colors for the Admin Chat Color. Max value: 255")]
        public float[] SuperAdminChatRGB = { 255, 0, 0 };
        [Description("The Chat Prefix before an admin speaks. eg. *The prefix was set to \"(Admin) \", so.. (Admin) : Hi! Note: If you put a space after the prefix, it will look like this: (Admin) <TerrariaDude): Hi!")]
        public string AdminChatPrefix = "(Admin) ";
        [Description("")]
        public bool AdminChatEnabled = true;

        [Description("Seconds a player has to wait between pvp toggles.")]
        public int PvpThrottle;

        [Description("Backup frequency in minutes. So, a value of 60 = 60 minutes. Backups are stored in the \\tshock\\backups folder.")]
        public int BackupInterval;
        [Description("How long backups are kept in minutes. 2880 = 2 days.")]
        public int BackupKeepFor = 60;

        [Description("Remembers where a player left off. It works by remembering the IP, NOT the character.  \neg. When you try to disconnect, and reconnect to be automatically placed at spawn, you'll be at your last location. Note: Won't save after server restarts.")]
        public bool RememberLeavePos;
        [Description("Hardcore players ONLY. This means softcore players cannot join.")]
        public bool HardcoreOnly;
        [Description("Mediumcore players ONLY. This means softcore players cannot join.")]
        public bool MediumcoreOnly;
        [Description("Kicks a Hardcore player on death.")]
        public bool KickOnMediumcoreDeath;
        [Description("Bans a Hardcore player on death.")]
        public bool BanOnMediumcoreDeath;

        [Description("Enable/Disable Terrarias built in auto save")]
        public bool AutoSave = true;

        [Description("Number of failed login attempts before kicking the player.")]
        public int MaximumLoginAttempts = 3;

        [Description("Not implemented")]
        public string RconPassword = "";
        [Description("Not implemented")]
        public int RconPort = 7777;

        [Description("Not implemented")]
        public string ServerName = "";
        [Description("Not implemented")]
        public string MasterServer = "127.0.0.1";

        [Description("Valid types are \"sqlite\" and \"mysql\"")]
        public string StorageType = "sqlite";

        [Description("")]
        public string MySqlHost = "localhost:3306";
        [Description("")]
        public string MySqlDbName = "";
        [Description("")]
        public string MySqlUsername = "";
        [Description("")]
        public string MySqlPassword = "";
        [Description("")]
        public string RangeCheckBanReason = "Placing impossible to place blocks.";
        [Description("")]
        public string SendSectionAbuseReason = "SendSection abuse.";
        [Description("")]
        public string NPCSpawnAbuseReason = "Spawn NPC abuse";
        [Description("")]
        public string UpdatePlayerAbuseReason = "Update Player abuse";
        [Description("")]
        public string ExplosiveAbuseReason = "Throwing an explosive device.";
        [Description("")]
        public string KillMeAbuseReason = "Trying to execute KillMe on someone else.";
        [Description("")]
        public string IllogicalLiquidUseReason = "Manipulating liquid without bucket.";
        [Description("")]
        public string LiquidAbuseReason = "Placing impossible to place liquid.";
        [Description("")]
        public string TileKillAbuseReason = "Tile Kill abuse ({0})";
        [Description("")]
        public string MediumcoreBanReason = "Death results in a ban";
        [Description("")]
        public string MediumcoreKickReason = "Death results in a kick";
        [Description("")]
        public string ProjectileAbuseReason = "Projectile abuse";
        [Description("")]
        public string TileAbuseReason = "Tile abuse ({0})";
        [Description("")]
        public string GriefClientReason = "Grief client detected ({0})";
        [Description("")]
        public bool EnableDNSHostResolution;
        [Description("")]
        public bool EnableBanOnUsernames;
        [Description("")]
        public bool EnableAntiLag = true;

        public string DefaultRegistrationGroupName = "default";

        [Description("")]
        public bool DisableSpewLogs = true;

        [Description("Valid types are \"sha512\", \"sha256\", \"md5\", append with \"-xp\" for the xp supported algorithms")]
        public string HashAlgorithm = "sha512";

        [Description("Buffers up the packets and sends them out at the end of each frame")]
        public bool BufferPackets = true;

        [Description("Display the users group when they chat.")]
        public bool ChatDisplayGroup = false;

        [Description("String that is used when kicking people when the server is full.")]
        public string ServerFullReason = "Server is full";

        [Description("This will save the world if Terraria crashes from an unhandled exception.")]
        public bool SaveWorldOnCrash = true;

        [Description("This is kick players who have custom items in their inventory (via a mod)")]
        public bool KickCustomItems = false;

        [Description("This will announce a player's location on join")]
        public bool EnableGeoIP = false;

        [Description("This will turn on a token requirement for the /status API endpoint.")]
        public bool EnableTokenEndpointAuthentication = false;

        [Description("This is used when the API endpoint /status is queried.")]
        public string ServerNickname = "TShock Server";

        [Description("Enable/Disable the rest api.")]
        public bool RestApiEnabled = false;

        [Description("This is the port which the rest api will listen on.")]
        public int RestApiPort = 7878;

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


        static void DumpDescriptions()
        {
            var sb = new StringBuilder();
            var defaults = new ConfigFile();

            foreach (var field in defaults.GetType().GetFields())
            {
                if (field.IsStatic)
                    continue;

                var name = field.Name;
                var type = field.FieldType.Name;

                var descattr = field.GetCustomAttributes(false).FirstOrDefault(o => o is DescriptionAttribute) as DescriptionAttribute;
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