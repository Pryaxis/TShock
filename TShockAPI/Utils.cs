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

using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using Terraria;


namespace TShockAPI
{
    public class Utils
    {
        public Random Random = new Random();
        //private static List<Group> groups = new List<Group>();

        /// <summary>
        /// Provides the real IP address from a RemoteEndPoint string that contains a port and an IP
        /// </summary>
        /// <param name="mess">A string IPv4 address in IP:PORT form.</param>
        /// <returns>A string IPv4 address.</returns>
        public string GetRealIP(string mess)
        {
            return mess.Split(':')[0];
        }

        /// <summary>
        /// Used for some places where a list of players might be used.
        /// </summary>
        /// <returns>String of players seperated by commas.</returns>
        public string GetPlayers()
        {
            var sb = new StringBuilder();
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active)
                {
                    if (sb.Length != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(player.Name);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Finds a player and gets IP as string
        /// </summary>
        /// <param name="msg">Player name</param>
        public string GetPlayerIP(string playername)
        {
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active)
                {
                    if (playername.ToLower() == player.Name.ToLower())
                    {
                        return player.IP;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// It's a clamp function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value to clamp</param>
        /// <param name="max">Maximum bounds of the clamp</param>
        /// <param name="min">Minimum bounds of the clamp</param>
        /// <returns></returns>
        public T Clamp<T>(T value, T max, T min)
            where T : IComparable<T>
        {
            T result = value;
            if (value.CompareTo(max) > 0)
                result = max;
            if (value.CompareTo(min) < 0)
                result = min;
            return result;
        }

        /// <summary>
        /// Saves the map data
        /// </summary>
        public void SaveWorld()
        {
            WorldGen.saveWorld();
            Broadcast("World saved.", Color.Yellow);
            Log.Info(string.Format("World saved at ({0})", Main.worldPathName));
        }

        /// <summary>
        /// Broadcasts a message to all players
        /// </summary>
        /// <param name="msg">string message</param>
        public void Broadcast(string msg)
        {
            Broadcast(msg, Color.Green);
        }

        public void Broadcast(string msg, byte red, byte green, byte blue)
        {
            TSPlayer.All.SendMessage(msg, red, green, blue);
            TSPlayer.Server.SendMessage(msg, red, green, blue);
            Log.Info(string.Format("Broadcast: {0}", msg));
        }

        public void Broadcast(string msg, Color color)
        {
            Broadcast(msg, color.R, color.G, color.B);
        }

        /// <summary>
        /// Sends message to all users with 'logs' permission.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="color"></param>
        public void SendLogs(string log, Color color)
        {
            Log.Info(log);
            TSPlayer.Server.SendMessage(log, color);
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active && player.Group.HasPermission(Permissions.logs) && player.DisplayLogs && TShock.Config.DisableSpewLogs == false)
                    player.SendMessage(log, color);
            }
        }

        /// <summary>
        /// The number of active players on the server.
        /// </summary>
        /// <returns>int playerCount</returns>
        public int ActivePlayers()
        {
            int num = 0;
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active)
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ply"></param>
        /// <returns></returns>
        public List<TSPlayer> FindPlayer(string ply)
        {
            var found = new List<TSPlayer>();
            ply = ply.ToLower();
            foreach (TSPlayer player in TShock.Players)
            {
                if (player == null)
                    continue;

                string name = player.Name.ToLower();
                if (name.Equals(ply))
                    return new List<TSPlayer> { player };
                if (name.Contains(ply))
                    found.Add(player);
            }
            return found;
        }

        public void GetRandomClearTileWithInRange(int startTileX, int startTileY, int tileXRange, int tileYRange, out int tileX, out int tileY)
        {
            int j = 0;
            do
            {
                if (j == 100)
                {
                    tileX = startTileX;
                    tileY = startTileY;
                    break;
                }

                tileX = startTileX + Random.Next(tileXRange * -1, tileXRange);
                tileY = startTileY + Random.Next(tileYRange * -1, tileYRange);
                j++;
            }
            while (TileValid(tileX, tileY) && !TileClear(tileX, tileY));
        }

        private bool TileValid(int tileX, int tileY)
        {
            return tileX >= 0 && tileX <= Main.maxTilesX && tileY >= 0 && tileY <= Main.maxTilesY;
        }

        private bool TileClear(int tileX, int tileY)
        {
            return !Main.tile[tileX, tileY].active;
        }

        public List<Item> GetItemByIdOrName(string idOrName)
        {
            int type = -1;
            if (int.TryParse(idOrName, out type))
            {
                return new List<Item> { GetItemById(type) };
            }
            return GetItemByName(idOrName);
        }

        public Item GetItemById(int id)
        {
            Item item = new Item();
            item.netDefaults(id);
            return item;
        }

        public List<Item> GetItemByName(string name)
        {
            //Method #1 - must be exact match, allows support for different pickaxes/hammers/swords etc
            for (int i = 1; i < Main.maxItemTypes; i++)
            {
                Item item = new Item();
                item.SetDefaults(name);
                if (item.name == name)
                    return new List<Item> { item };
            }
            //Method #2 - allows impartial matching
            var found = new List<Item>();
            for (int i = 1; i < Main.maxItemTypes; i++)
            {
                try
                {
                    Item item = new Item();
                    item.netDefaults(i);
                    if (item.name.ToLower() == name.ToLower())
                        return new List<Item> { item };
                    if (item.name.ToLower().StartsWith(name.ToLower()))
                        found.Add(item);
                }
                catch { }
            }
            return found;
        }

        public List<NPC> GetNPCByIdOrName(string idOrName)
        {
            int type = -1;
            if (int.TryParse(idOrName, out type))
            {
                return new List<NPC> { GetNPCById(type) };
            }
            return GetNPCByName(idOrName);
        }

        public NPC GetNPCById(int id)
        {
            NPC npc = new NPC();
            npc.netDefaults(id);
            return npc;
        }

        public List<NPC> GetNPCByName(string name)
        {
            //Method #1 - must be exact match, allows support for different coloured slimes
            for (int i = 1; i < Main.maxNPCTypes; i++)
            {
                NPC npc = new NPC();
                npc.SetDefaults(name);
                if (npc.name == name)
                    return new List<NPC> { npc };
            }
            //Method #2 - allows impartial matching
            var found = new List<NPC>();
            for (int i = 1; i < Main.maxNPCTypes; i++)
            {
                NPC npc = new NPC();
                npc.netDefaults(i);
                if (npc.name.ToLower() == name.ToLower())
                    return new List<NPC> { npc };
                if (npc.name.ToLower().StartsWith(name.ToLower()))
                    found.Add(npc);
            }
            return found;
        }

        public string GetBuffName(int id)
        {
            return (id > 0 && id < Main.maxBuffs) ? Main.buffName[id] : "null";
        }

        public string GetBuffDescription(int id)
        {
            return (id > 0 && id < Main.maxBuffs) ? Main.buffTip[id] : "null";
        }

        public List<int> GetBuffByName(string name)
        {
            for (int i = 1; i < Main.maxBuffs; i++)
            {
                if (Main.buffName[i].ToLower() == name)
                    return new List<int> { i };
            }
            var found = new List<int>();
            for (int i = 1; i < Main.maxBuffs; i++)
            {
                if (Main.buffName[i].ToLower().StartsWith(name.ToLower()))
                    found.Add(i);
            }
            return found;
        }

        public string GetPrefixById(int id)
        {
            var item = new Item();
            item.SetDefaults(0);
            item.prefix = (byte)id;
            item.AffixName();
            return item.name.Trim();
        }

        public List<int> GetPrefixByName(string name)
        {
            Item item = new Item();
            for (int i = 1; i < 83; i++)
            {
                item.prefix = (byte) i;
                item.AffixName();
                if (item.name.Trim() == name)
                    return new List<int> { i };
            }
            var found = new List<int>();
            for (int i = 1; i < 83; i++)
            {
                try
                {
                    item.prefix = (byte) i;
                    if (item.name.Trim().ToLower() == name.ToLower())
                        return new List<int> { i };
                    if (item.name.Trim().ToLower().StartsWith(name.ToLower()))
                        found.Add(i);
                }
                catch { }
            }
            return found;
        }

        public List<int> GetPrefixByIdOrName(string idOrName)
        {
            int type = -1;
            if (int.TryParse(idOrName, out type) && type > 0 && type < 84)
            {
                return new List<int> {type};
            }
            return GetPrefixByName(idOrName);
        }

        /// <summary>
        /// Kicks all player from the server without checking for immunetokick permission.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public void ForceKickAll(string reason)
        {
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active)
                {
                    ForceKick(player, reason);
                }
            }
        }

        /// <summary>
        /// Kicks a player from the server without checking for immunetokick permission.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public void ForceKick(TSPlayer player, string reason)
        {
            if (!player.ConnectionAlive)
                return;
            player.Disconnect(reason);
            Log.ConsoleInfo(string.Format("{0} was force kicked for : {1}", player.IP, reason));
        }

        /// <summary>
        /// Kicks a player from the server.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public bool Kick(TSPlayer player, string reason, string adminUserName = "")
        {
            if (!player.ConnectionAlive)
                return true;
            if (!player.Group.HasPermission(Permissions.immunetokick))
            {
                string playerName = player.Name;
                player.Disconnect(string.Format("Kicked: {0}", reason));
                Log.ConsoleInfo(string.Format("Kicked {0} for : {1}", playerName, reason));
                if (adminUserName.Length == 0)
                    Broadcast(string.Format("{0} was kicked for {1}", playerName, reason.ToLower()));
                else
                    Broadcast(string.Format("{0} kicked {1} for {2}", adminUserName, playerName, reason.ToLower()));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Bans and kicks a player from the server.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public bool Ban(TSPlayer player, string reason, string adminUserName = "")
        {
            if (!player.ConnectionAlive)
                return true;
            if (!player.Group.HasPermission(Permissions.immunetoban))
            {
                string ip = player.IP;
                string playerName = player.Name;
                TShock.Bans.AddBan(ip, playerName, reason);
                player.Disconnect(string.Format("Banned: {0}", reason));
                Log.ConsoleInfo(string.Format("Banned {0} for : {1}", playerName, reason));
                if (adminUserName.Length == 0)
                    Broadcast(string.Format("{0} was banned for {1}", playerName, reason.ToLower()));
                else
                    Broadcast(string.Format("{0} banned {1} for {2}", adminUserName, playerName, reason.ToLower()));
                return true;
            }
            return false;
        }

        public bool HandleCheater(TSPlayer player, string reason)
        {
            return HandleBadPlayer(player, "ignorecheatdetection", TShock.Config.BanCheaters, TShock.Config.KickCheaters, reason);
        }

        public bool HandleGriefer(TSPlayer player, string reason)
        {
            return HandleBadPlayer(player, Permissions.ignoregriefdetection, TShock.Config.BanGriefers, TShock.Config.KickGriefers, reason);
        }

        public bool HandleTntUser(TSPlayer player, string reason)
        {
            return HandleBadPlayer(player, Permissions.ignoregriefdetection, TShock.Config.BanKillTileAbusers, TShock.Config.KickKillTileAbusers, reason);
        }

        public bool HandleExplosivesUser(TSPlayer player, string reason)
        {
            return HandleBadPlayer(player, Permissions.ignoregriefdetection, TShock.Config.BanExplosives, TShock.Config.KickExplosives, reason);
        }

        private bool HandleBadPlayer(TSPlayer player, string overridePermission, bool ban, bool kick, string reason)
        {
            if (!player.Group.HasPermission(overridePermission) || !(player.Group.Name == "superadmin"))
            {
                if (ban)
                {
                    return Ban(player, reason);
                }
                if (kick)
                {
                    return Kick(player, reason);
                }
            }
            return false;
        }

        /// <summary>
        /// Shows a file to the user.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="file">string filename reletave to savedir</param>
        //Todo: Fix this
        public void ShowFileToUser(TSPlayer player, string file)
        {
            string foo = "";
            using (var tr = new StreamReader(Path.Combine(TShock.SavePath, file)))
            {
                while ((foo = tr.ReadLine()) != null)
                {
                    foo = foo.Replace("%map%", Main.worldName);
                    foo = foo.Replace("%players%", GetPlayers());
                    if (foo.Substring(0, 1) == "%" && foo.Substring(12, 1) == "%") //Look for a beginning color code.
                    {
                        string possibleColor = foo.Substring(0, 13);
                        foo = foo.Remove(0, 13);
                        float[] pC = { 0, 0, 0 };
                        possibleColor = possibleColor.Replace("%", "");
                        string[] pCc = possibleColor.Split(',');
                        if (pCc.Length == 3)
                        {
                            try
                            {
                                player.SendMessage(foo, (byte)Convert.ToInt32(pCc[0]), (byte)Convert.ToInt32(pCc[1]),
                                                   (byte)Convert.ToInt32(pCc[2]));
                                continue;
                            }
                            catch (Exception e)
                            {
                                Log.Error(e.ToString());
                            }
                        }
                    }
                    player.SendMessage(foo);
                }
            }
        }

        /// <summary>
        /// Returns a Group from the name of the group
        /// </summary>
        /// <param name="ply">string groupName</param>
        public Group GetGroup(string groupName)
        {
            //first attempt on cached groups
            for (int i = 0; i < TShock.Groups.groups.Count; i++)
            {
                if (TShock.Groups.groups[i].Name.Equals(groupName))
                {
                    return TShock.Groups.groups[i];
                }
            }
            return new Group("default");
        }

        /// <summary>
        /// Returns an IPv4 address from a DNS query
        /// </summary>
        /// <param name="hostname">string ip</param>
        public string GetIPv4Address(string hostname)
        {
            try
            {
                //Get the ipv4 address from GetHostAddresses, if an ip is passed it will return that ip
                var ip = Dns.GetHostAddresses(hostname).FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork);
                //if the dns query was successful then return it, otherwise return an empty string
                return ip != null ? ip.ToString() : "";
            }
            catch (SocketException)
            {
            }
            return "";
        }

        public string HashAlgo = "md5";

        public readonly Dictionary<string, Func<HashAlgorithm>> HashTypes = new Dictionary<string, Func<HashAlgorithm>>
        {
            {"sha512", () => new SHA512Managed()},
            {"sha256", () => new SHA256Managed()},
            {"md5", () => new MD5Cng()},
            {"sha512-xp", () => SHA512.Create()},
            {"sha256-xp", () => SHA256.Create()},
            {"md5-xp", () => MD5.Create()},
        };

        /// <summary>
        /// Returns a Sha256 string for a given string
        /// </summary>
        /// <param name="bytes">bytes to hash</param>
        /// <returns>string sha256</returns>
        public string HashPassword(byte[] bytes)
        {
            if (bytes == null)
                throw new NullReferenceException("bytes");
            Func<HashAlgorithm> func;
            if (!HashTypes.TryGetValue(HashAlgo.ToLower(), out func))
                throw new NotSupportedException("Hashing algorithm {0} is not supported".SFormat(HashAlgo.ToLower()));

            using (var hash = func())
            {
                var ret = hash.ComputeHash(bytes);
                return ret.Aggregate("", (s, b) => s + b.ToString("X2"));
            }
        }
        /// <summary>
        /// Returns a Sha256 string for a given string
        /// </summary>
        /// <param name="bytes">bytes to hash</param>
        /// <returns>string sha256</returns>
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password == "non-existant password")
                return "non-existant password";
            return HashPassword(Encoding.UTF8.GetBytes(password));
        }

        /// <summary>
        /// Checks if the string contains any unprintable characters
        /// </summary>
        /// <param name="str">String to check</param>
        /// <returns>True if the string only contains printable characters</returns>
        public bool ValidString(string str)
        {
            foreach (var c in str)
            {
                if (c < 0x20 || c > 0xA9)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if world has hit the max number of chests
        /// </summary>
        /// <returns>True if the entire chest array is used</returns>
        public bool MaxChests()
        {
            for (int i = 0; i < Main.chest.Length; i++)
            {
                if (Main.chest[i] == null)
                    return false;
            }
            return true;
        }
    }
}
