using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;
using Microsoft.Xna.Framework;

namespace TShockAPI
{
    public class TShock : TerrariaPlugin
    {
        private uint[] tileThreshold = new uint[Main.maxPlayers];

        public static string saveDir = "./tshock/";

        public override Version Version
        {
            get { return new Version(0, 1); }
        }

        public override string Name
        {
            get { return "TShock"; }
        }

        public override string Author
        {
            get { return "nicatronTg, High, Mav, and Zach"; }
        }

        public override string Description
        {
            get { return "The administration modification of the future."; }
        }

        public TShock(Main game) : base (game)
        {
            GameHooks.OnPreInitialize += OnPreInit;
            GameHooks.OnPostInitialize += OnPostInit;
            GameHooks.OnUpdate += new Action<Microsoft.Xna.Framework.GameTime>(OnUpdate);
            GameHooks.OnLoadContent += new Action<Microsoft.Xna.Framework.Content.ContentManager>(OnLoadContent);
            ServerHooks.OnChat += new Action<int, string, HandledEventArgs>(OnChat);
            NetHooks.OnPreGetData += GetData;
        }

        /*
         * Hooks:
         * */

        void GetData(byte id, messageBuffer msg, int idx, int length, HandledEventArgs e)
        {
            int n = 5;
            byte[] buf = msg.readBuffer;
            if (id == 17)
            {
                byte type = buf[n];
                n++;
                if (type == 0)
                {
                    tileThreshold[msg.whoAmI]++;
                }
            }
            return;
        }

        void OnChat(int ply, string msg, HandledEventArgs handler)
        {
            if (IsAdmin(ply))
            {
                if (msg.Length > 5 && msg.Substring(0, 5) == "/kick")
                {
                    string plStr = msg.Remove(0, 5).Trim();
                    if (FindPlayer(plStr) == -1 || plStr == "")
                    {
                        Kick(FindPlayer(plStr), "You were kicked.");
                        Broadcast(plStr + " was kicked by " + FindPlayer(ply));
                    }
                    handler.Handled = true;
                }

                if (msg.Length > 4 && msg.Substring(0, 4) == "/ban")
                {
                    string plStr = msg.Remove(0, 4).Trim();
                    if (!(FindPlayer(plStr) == -1 || plStr == ""))
                    {
                        WriteBan(FindPlayer(plStr));
                        Kick(FindPlayer(plStr), "You were banned.");
                    }
                    handler.Handled = true;
                }
            }
        }

        void OnLoadContent(Microsoft.Xna.Framework.Content.ContentManager obj)
        {
            
        }

        void OnPreInit()
        {

        }

        void OnPostInit()
        {

        }

        void OnUpdate(GameTime time)
        {
            for (uint i = 0; i < Main.maxPlayers; i++)
            {
                if (tileThreshold[i] >= 5)
                {
                    if (Main.player[i] != null)
                    {
                        WriteGrief((int)i);
                        Kick((int)i, "Fuck you bomb spam or some other fucking shit");
                    }
                    tileThreshold[i] = 0;
                }
                else if (tileThreshold[i] > 0)
                {
                    tileThreshold[i]--;
                }
            }
        }

        /*
         * Useful stuff:
         * */

        public static void Kick(int ply, string reason)
        {
            NetMessage.SendData(0x2, ply, -1, reason, 0x0, 0f, 0f, 0f);
            Netplay.serverSock[ply].kill = true;
            NetMessage.syncPlayers();
        }

        public static bool IsAdmin(string ply)
        {
            string remoteEndPoint = Convert.ToString((Netplay.serverSock[FindPlayer(ply)].tcpClient.Client.RemoteEndPoint));
            string[] remoteEndPointIP = remoteEndPoint.Split(':');
            TextReader tr = new StreamReader(saveDir + "admins.txt");
            string adminlist = tr.ReadToEnd();
            tr.Close();
            if (adminlist.Contains(remoteEndPointIP[0]))
            {
                return true;
            }
            return false;
        }

        public static bool IsAdmin(int ply)
        {
            string remoteEndPoint = Convert.ToString((Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));
            string[] remoteEndPointIP = remoteEndPoint.Split(':');
            TextReader tr = new StreamReader(saveDir + "admins.txt");
            string adminlist = tr.ReadToEnd();
            tr.Close();
            if (adminlist.Contains(remoteEndPointIP[0]))
            {
                return true;
            }
            return false;
        }

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

        public static void Broadcast(string msg)
        {
            for (int i = 0; i < Main.player.Length; i++)
            {
                SendMessage(i, msg);
            }
        }

        public static string GetRealIP(string mess)
        {
            return mess.Split(':')[0];
        }

        public static void SendMessage(int ply, string msg, float[] color)
        {
            NetMessage.SendData(0x19, ply, -1, msg, 8, color[0], color[1], color[2]);
        }

        public static void SendMessage(int ply, string message)
        {
            NetMessage.SendData(0x19, ply, -1, message, 8, 0f, 255f, 0f);
        }

        private static void WriteGrief(int ply)
        {
            TextWriter tw = new StreamWriter(saveDir + "grief.txt", true);
            tw.WriteLine("[" + Main.player[ply].name + "] [" + GetRealIP(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint.ToString()) + "]");
            tw.Close();
        }

        private static void WriteError(string err)
        {
            if (System.IO.File.Exists(saveDir + "errors.txt"))
            {
                TextWriter tw = new StreamWriter(saveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
            else
            {
                CreateFile(saveDir + "errors.txt");
                TextWriter tw = new StreamWriter(saveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
        }

        public static void WriteBan(int ply)
        {
            string ip = GetRealIP(Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));
            TextWriter tw = new StreamWriter(saveDir + "bans.txt", true);
            tw.WriteLine("[" + Main.player[ply].name + "] " + "[" + ip + "]");
            tw.Close();
        }

        private static void CreateFile(string file)
        {
            using (FileStream fs = File.Create(file)) { }
        }
    }
}