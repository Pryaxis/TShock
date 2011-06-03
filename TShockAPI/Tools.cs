using System;
using System.IO;
using Terraria;

namespace TShockAPI
{
    class Tools
    {
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
            string str = "";
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    if (str == "")
                    {
                        str = str + Main.player[i].name;
                    }
                    else
                    {
                        str = str + ", " + Main.player[i].name;
                    }
                }
            }
            return str;
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
    where T : System.IComparable<T>
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
            for (int i = 0; i < Main.player.Length; i++)
            {
                SendMessage(i, msg);
            }
        }

        /// <summary>
        /// Sends a message out to a single player
        /// </summary>
        /// <param name="ply">int socket thingy for the player from the server socket</param>
        /// <param name="msg">String message</param>
        /// <param name="color">Float containing red, blue, and green color values</param>
        public static void SendMessage(int ply, string msg, float[] color)
        {
            NetMessage.SendData(0x19, ply, -1, msg, 255, color[0], color[1], color[2]);
        }

        /// <summary>
        /// Sends a green message to a player
        /// </summary>
        /// <param name="ply">int socket thingy for the player from the server socket</param>
        /// <param name="message">string message</param>
        public static void SendMessage(int ply, string message)
        {
            NetMessage.SendData(0x19, ply, -1, message, 255, 0f, 255f, 0f);
        }

        /// <summary>
        /// The number of active players on the server.
        /// </summary>
        /// <returns>int playerCount</returns>
        public static int activePlayers()
        {
            int num = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    num++;
                }
            }
            return num;
        }

        /// <summary>
        /// Finds the name of the player of the int given
        /// </summary>
        /// <param name="ply">string player name</param>
        /// <returns>int player</returns>
        public static int FindPlayer(string ply)
        {
            int pl = -1;
            for (int i = 0; i < Main.player.Length; i++)
            {
                if ((ply.ToLower()) == Main.player[i].name.ToLower())
                {
                    pl = i;
                    break;
                }
            }
            return pl;
        }

        /// <summary>
        /// Gets the given player's name
        /// </summary>
        /// <param name="ply">int player</param>
        /// <returns>string name</returns>
        public static string FindPlayer(int ply)
        {
            for (int i = 0; i < Main.player.Length; i++)
            {
                if (i == ply)
                {
                    return Main.player[i].name;
                }
            }
            return "null";
        }

        /// <summary>
        /// Creates an NPC
        /// </summary>
        /// <param name="type">Type is defined in the enum NPC list</param>
        /// <param name="x">X coord of the desired npc</param>
        /// <param name="y">Y coord of the desired npc</param>
        /// <param name="target">int player that the npc targets</param>
        public static void NewNPC(int type, int x, int y, int target)
        {
            switch (type)
            {
                case 0: //World Eater
                    WorldGen.shadowOrbSmashed = true;
                    WorldGen.shadowOrbCount = 3;
                    int w = NPC.NewNPC(x, y, 13, 1);
                    Main.npc[w].target = target;
                    break;
                case 1: //Eye
                    Main.time = 4861;
                    Main.dayTime = false;
                    WorldGen.spawnEye = true;
                    break;
                case 2: //Skeletron
                    int enpeecee = NPC.NewNPC(x, y, 0x23, 0);
                    Main.npc[enpeecee].netUpdate = true;
                    break;
            }
        }

        /// <summary>
        /// Finds a player, reads admins.txt, and determines if their IP address is on that list.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <returns>true/false</returns>
        public static bool IsAdmin(int ply)
        {
            string remoteEndPoint = Convert.ToString((Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));
            string[] remoteEndPointIP = remoteEndPoint.Split(':');
            TextReader tr = new StreamReader(FileTools.SaveDir + "admins.txt");
            string adminlist = tr.ReadToEnd();
            tr.Close();
            if (adminlist.Contains(remoteEndPointIP[0]))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Finds a player based on their name, reads admins.txt, and determines if thier IP address is on that list.
        /// </summary>
        /// <param name="ply"></param>
        /// <returns></returns>
        public static bool IsAdmin(string ply)
        {
            string remoteEndPoint = Convert.ToString((Netplay.serverSock[Tools.FindPlayer(ply)].tcpClient.Client.RemoteEndPoint));
            string[] remoteEndPointIP = remoteEndPoint.Split(':');
            TextReader tr = new StreamReader(FileTools.SaveDir + "admins.txt");
            string adminlist = tr.ReadToEnd();
            tr.Close();
            if (adminlist.Contains(remoteEndPointIP[0]))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Kicks a player from the server.
        /// </summary>
        /// <param name="ply">int player</param>
        /// <param name="reason">string reason</param>
        public static void Kick(int ply, string reason)
        {
            NetMessage.SendData(0x2, ply, -1, reason, 0x0, 0f, 0f, 0f);
        }

        /// <summary>
        /// Adds someone to cheaters.txt
        /// </summary>
        /// <param name="ply">int player</param>
        public static void HandleCheater(int ply)
        {
            if (!TShock.players[ply].IsAdmin())
            {
                string cheater = Tools.FindPlayer(ply);
                string ip = Tools.GetRealIP(Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));

                FileTools.WriteCheater(ply);
                if (!ConfigurationManager.kickCheater) { return; }
                Netplay.serverSock[ply].kill = true;
                Netplay.serverSock[ply].Reset();
                NetMessage.syncPlayers();
                Tools.Broadcast(cheater + " was " + (ConfigurationManager.banCheater ? "banned " : "kicked ") + "for cheating.");
            }
        }

        /// <summary>
        /// Adds someone to greifers.txt
        /// </summary>
        /// <param name="ply">int player</param>
        public static void HandleGreifer(int ply)
        {
            if (!TShock.players[ply].IsAdmin())
            {
                string cheater = Tools.FindPlayer(ply);
                string ip = Tools.GetRealIP(Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));

                FileTools.WriteGrief(ply);
                if (!ConfigurationManager.kickGriefer) { return; }
                Netplay.serverSock[ply].kill = true;
                Netplay.serverSock[ply].Reset();
                NetMessage.syncPlayers();
                Tools.Broadcast(cheater + " was " + (ConfigurationManager.banCheater ? "banned " : "kicked ") + "for greifing.");
            }
        }

        /// <summary>
        /// Shows a MOTD to the player
        /// </summary>
        /// <param name="ply">int player</param>
        public static void ShowMOTD(int ply)
        {
            string foo = "";
            TextReader tr = new StreamReader(FileTools.SaveDir + "motd.txt");
            while ((foo = tr.ReadLine()) != null)
            {
                foo = foo.Replace("%map%", Main.worldName);
                foo = foo.Replace("%players%", Tools.GetPlayers());
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
                            pC[0] = Tools.Clamp(Convert.ToInt32(pCc[0]), 255, 0);
                            pC[1] = Tools.Clamp(Convert.ToInt32(pCc[1]), 255, 0);
                            pC[2] = Tools.Clamp(Convert.ToInt32(pCc[2]), 255, 0);
                            Tools.SendMessage(ply, foo, pC);
                            continue;
                        }
                        catch (Exception e)
                        {
                            FileTools.WriteError(e.Message);
                        }
                    }
                }
                Tools.SendMessage(ply, foo);
            }
            tr.Close();
        }

        public Tools() { }
    }
}