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
using System.Text;
using Microsoft.Xna.Framework;
using Terraria;

namespace TShockAPI
{
    internal enum NPCList
    {
        WORLD_EATER = 0,
        EYE = 1,
        SKELETRON = 2
    }

    internal class Tools
    {
        private static List<Group> groups = new List<Group>();

        /// <summary>
        /// Provides the real IP address from a RemoteEndPoint string that contains a port and an IP
        /// </summary>
        /// <param name="mess">A string IPv4 address in IP:PORT form.</param>
        /// <returns>A string IPv4 address.</returns>
        public static string GetRealIP(string mess)
        {
            return mess.Split(':')[0];
        }

        /// <summary>
        /// Used for some places where a list of players might be used.
        /// </summary>
        /// <returns>String of players seperated by commas.</returns>
        public static string GetPlayers()
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
        /// It's a clamp function
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">Value to clamp</param>
        /// <param name="max">Maximum bounds of the clamp</param>
        /// <param name="min">Minimum bounds of the clamp</param>
        /// <returns></returns>
        public static T Clamp<T>(T value, T max, T min)
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
        /// Broadcasts a message to all players
        /// </summary>
        /// <param name="msg">string message</param>
        public static void Broadcast(string msg)
        {
            Broadcast(msg, Color.Green);
        }

        public static void Broadcast(string msg, byte red, byte green, byte blue)
        {
            TSPlayer.All.SendMessage(msg, red, green, blue);
            TSPlayer.Server.SendMessage(msg, red, green, blue);
            Log.Info(string.Format("Broadcast: {0}", msg));

        }
        public static void Broadcast(string msg, Color color)
        {
            Broadcast(msg, color.R, color.G, color.B);
        }

        /// <summary>
        /// Sends message to all users with 'logs' permission.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="color"></param>
        public static void SendLogs(string log, Color color)
        {
            Log.Info(log);
            TSPlayer.Server.SendMessage(log, color);
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Group.HasPermission("logs"))
                    player.SendMessage(log, color);
            }
        }

        /// <summary>
        /// The number of active players on the server.
        /// </summary>
        /// <returns>int playerCount</returns>
        public static int ActivePlayers()
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
        public static List<TSPlayer> FindPlayer(string ply)
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

        /// <summary>
        /// Creates an NPC
        /// </summary>
        /// <param name="type">Type is defined in the enum NPC list</param>
        /// <param name="player">int player that the npc targets</param>
        public static void NewNPC(NPCList type, TSPlayer player)
        {
            switch (type)
            {
                case NPCList.WORLD_EATER:
                    WorldGen.shadowOrbSmashed = true;
                    WorldGen.shadowOrbCount = 3;
                    int w = NPC.NewNPC((int)player.X, (int)player.Y, 13, 1);
                    Main.npc[w].target = player.Index;
                    break;
                case NPCList.EYE:
                    Main.time = 4861;
                    Main.dayTime = false;
                    WorldGen.spawnEye = true;
                    break;
                case NPCList.SKELETRON:
                    int enpeecee = NPC.NewNPC((int)player.X, (int)player.Y, 0x23, 0);
                    Main.npc[enpeecee].netUpdate = true;
                    break;
            }
        }

        /// <summary>
        /// Kicks all player from the server without checking for immunetokick permission.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public static void ForceKickAll(string reason)
        {
            foreach(TSPlayer player in TShock.Players)
            {
                if (player != null && player.TPlayer.active)
                {
                    Tools.ForceKick(player, reason);
                }
            }
        }

        /// <summary>
        /// Kicks a player from the server without checking for immunetokick permission.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public static void ForceKick(TSPlayer player, string reason)
        {
            if (!Netplay.serverSock[player.Index].active || Netplay.serverSock[player.Index].kill)
                return;
            NetMessage.SendData(0x2, player.Index, -1, reason, 0x0, 0f, 0f, 0f);
            Log.Info(string.Format("{0} was force kicked for : {1}", player.IP, reason));
        }

        /// <summary>
        /// Kicks a player from the server.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public static bool Kick(TSPlayer player, string reason, string adminUserName = "")
        {
            if (!Netplay.serverSock[player.Index].active || Netplay.serverSock[player.Index].kill)
                return true;
            if (!player.Group.HasPermission("immunetokick"))
            {
                string playerName = player.Name;
                NetMessage.SendData(0x2, player.Index, -1, string.Format("Kicked: {0}", reason), 0x0, 0f, 0f, 0f);
                Log.Info(string.Format("Kicked {0} for : {1}", playerName, reason));
                if (adminUserName.Length == 0)
                    Broadcast(string.Format("{0} was kicked for {1}", playerName, reason.ToLower()));
                else
                    Tools.Broadcast(string.Format("{0} kicked {1} for {2}", adminUserName, playerName, reason.ToLower()));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Bans and kicks a player from the server.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public static bool Ban(TSPlayer player, string reason, string adminUserName = "")
        {
            if (!Netplay.serverSock[player.Index].active || Netplay.serverSock[player.Index].kill)
                return true;
            if (!player.Group.HasPermission("immunetoban"))
            {
                string ip = player.IP;
                string playerName = player.Name;
                TShock.Bans.AddBan(ip, playerName, reason);
                NetMessage.SendData(0x2, player.Index, -1, string.Format("Banned: {0}", reason), 0x0, 0f, 0f, 0f);
                Log.Info(string.Format("Banned {0} for : {1}", playerName, reason));
                if (adminUserName.Length == 0)
                    Broadcast(string.Format("{0} was banned for {1}", playerName, reason.ToLower()));
                else
                    Tools.Broadcast(string.Format("{0} banned {1} for {2}", adminUserName, playerName, reason.ToLower()));
                return true;
            }
            return false;
        }

        public static bool HandleCheater(TSPlayer player, string reason)
        {
            return HandleBadPlayer(player, "ignorecheatdetection", ConfigurationManager.BanCheater, ConfigurationManager.KickCheater, reason);
        }

        public static bool HandleGriefer(TSPlayer player, string reason)
        {
            return HandleBadPlayer(player, "ignoregriefdetection", ConfigurationManager.BanGriefer, ConfigurationManager.KickGriefer, reason);
        }

        public static bool HandleTntUser(TSPlayer player, string reason)
        {
            return HandleBadPlayer(player, "ignoregriefdetection", ConfigurationManager.BanTnt, ConfigurationManager.KickTnt, reason);
        }

        public static bool HandleExplosivesUser(TSPlayer player, string reason)
        {
            return HandleBadPlayer(player, "ignoregriefdetection", ConfigurationManager.BanBoom, ConfigurationManager.KickBoom, reason);
        }

        private static bool HandleBadPlayer(TSPlayer player, string overridePermission, bool ban, bool kick, string reason)
        {
            if (!player.Group.HasPermission(overridePermission))
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
        public static void ShowFileToUser(TSPlayer player, string file)
        {
            string foo = "";
            TextReader tr = new StreamReader(Path.Combine(TShock.SavePath, file));
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
                            player.SendMessage(foo, (byte)Convert.ToInt32(pCc[0]), (byte)Convert.ToInt32(pCc[1]), (byte)Convert.ToInt32(pCc[2]));
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
            tr.Close();
        }

        public static void LoadGroups()
        {
            groups = new List<Group>();
            groups.Add(new SuperAdminGroup());

            StreamReader sr = new StreamReader(FileTools.GroupsPath);
            string data = sr.ReadToEnd();
            data = data.Replace("\r", "");
            string[] lines = data.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#"))
                {
                    continue;
                }
                string[] args = lines[i].Split(' ');
                if (args.Length < 2)
                {
                    continue;
                }
                string name = args[0];
                string parent = args[1];
                Group group = null;
                if (parent.Equals("null"))
                {
                    group = new Group(name);
                }
                else
                {
                    for (int j = 0; j < groups.Count; j++)
                    {
                        if (groups[j].Name.Equals(parent))
                        {
                            group = new Group(name, groups[j]);
                            break;
                        }
                    }
                }
                if (group == null)
                {
                    throw new Exception("Something in the groups.txt is fucked up");
                }
                else
                {
                    for (int j = 2; j < args.Length; j++)
                    {
                        string permission = args[j];
                        if (permission.StartsWith("!"))
                        {
                            group.NegatePermission(args[j].Replace("!", ""));
                        }
                        else
                        {
                            group.AddPermission(args[j]);
                        }
                    }
                }
                groups.Add(group);
            }
            sr.Close();
        }

        /// <summary>
        /// Returns a Group from the name of the group
        /// </summary>
        /// <param name="ply">string groupName</param>
        public static Group GetGroup(string groupName)
        {
            //first attempt on cached groups
            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].Name.Equals(groupName))
                {
                    return groups[i];
                }
            }
            //shit, it didnt work, reload and try again
            LoadGroups();

            for (int i = 0; i < groups.Count; i++)
            {
                if (groups[i].Name.Equals(groupName))
                {
                    return groups[i];
                }
            }

            //sigh :(, ok, you fucked up the config files, congrats.
            return null;
        }

        /// <summary>
        /// Returns a Group for a ip from users.txt
        /// </summary>
        /// <param name="ply">string ip</param>
        public static Group GetGroupForIP(string ip)
        {
            ip = GetRealIP(ip);

            StreamReader sr = new StreamReader(FileTools.UsersPath);
            string data = sr.ReadToEnd();
            data = data.Replace("\r", "");
            string[] lines = data.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string[] args = lines[i].Split(' ');
                if (args.Length < 2)
                {
                    continue;
                }
                if (lines[i].StartsWith("#"))
                {
                    continue;
                }
                if (args[0].Equals(ip))
                {
                    return GetGroup(args[1]);
                }
            }
            sr.Close();
            return GetGroup("default");
        }
    }
}