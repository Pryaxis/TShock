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
        public static string saveDir = "./tshock/";
        private static double version = 3;
        private static bool shownVersion = false;

        public static bool killGuide = true;
        public static int invasionMultiplier = 1;
        public static int defaultMaxSpawns = 4;
        public static int defaultSpawnRate = 700;
        public static bool kickCheater = true;
        public static bool banCheater = true;
        public static int serverPort = 7777;
        public static bool enableWhitelist = false;
        public static bool infinateInvasion = false;
        public static bool permaPvp = false;
        public static int killCount = 0;
        public static bool shownOneTimeInvasionMinder = false;

        public static string tileWhitelist = "";
        private static bool banTnt = false;
        private static bool kickTnt = false;

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
        }

        /*
         * Hooks:
         * */

        void OnChat(int ply, string msg, HandledEventArgs handler)
        {
            if (IsAdmin(ply))
            {
                if (msg.Length > 5 && msg.Substring(0, 5) == "/kick")
                {
                    string plStr = msg.Remove(0, 5).Trim();
                    if (!(FindPlayer(plStr) == -1 || plStr == ""))
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
            SetupConfig();
        }

        void OnPostInit()
        {

        }

        void OnUpdate(GameTime time)
        {

        }

        /*
         * Useful stuff:
         * */

        private static void KeepTilesUpToDate()
        {
            TextReader tr = new StreamReader(saveDir + "tiles.txt");
            string file = tr.ReadToEnd();
            tr.Close();
            if (!file.Contains("0x3d"))
            {
                System.IO.File.Delete(saveDir + "tiles.txt");
                CreateFile(saveDir + "tiles.txt");
                TextWriter tw = new StreamWriter(saveDir + "tiles.txt");
                tw.Write("0x03, 0x05, 0x14, 0x25, 0x18, 0x18, 0x20, 0x1b, 0x34, 0x48, 0x33, 0x3d, 0x47, 0x49, 0x4a, 0x35, 0x3d, 0x3e, 0x45, 0x47, 0x49, 0x4a,");
                tw.Close();
            }
        }

        public static void SetupConfig()
        {
            if (!System.IO.Directory.Exists(saveDir)) { System.IO.Directory.CreateDirectory(saveDir); }
            if (!System.IO.File.Exists(saveDir + "tiles.txt"))
            {
                CreateFile(saveDir + "tiles.txt");
                TextWriter tw = new StreamWriter(saveDir + "tiles.txt");
                tw.Write("0x03, 0x05, 0x14, 0x25, 0x18, 0x18, 0x20, 0x1b, 0x34, 0x48, 0x33, 0x3d, 0x47, 0x49, 0x4a, 0x35, 0x3d, 0x3e, 0x45, 0x47, 0x49, 0x4a,");
                tw.Close();
            }
            if (!System.IO.File.Exists(saveDir + "motd.txt"))
            {
                CreateFile(saveDir + "motd.txt");
                TextWriter tw = new StreamWriter(saveDir + "motd.txt");
                tw.WriteLine("This server is running TShock. Type /help for a list of commands.");
                tw.WriteLine("%255,000,000%Current map: %map%");
                tw.WriteLine("Current players: %players%");
                tw.Close();
            }
            if (!System.IO.File.Exists(saveDir + "bans.txt")) { CreateFile(saveDir + "bans.txt"); }
            if (!System.IO.File.Exists(saveDir + "cheaters.txt")) { CreateFile(saveDir + "cheaters.txt"); }
            if (!System.IO.File.Exists(saveDir + "admins.txt")) { CreateFile(saveDir + "admins.txt"); }
            if (!System.IO.File.Exists(saveDir + "grief.txt")) { CreateFile(saveDir + "grief.txt"); }
            if (!System.IO.File.Exists(saveDir + "config.txt"))
            {
                CreateFile(saveDir + "config.txt");
                TextWriter tw = new StreamWriter(saveDir + "config.txt");
                tw.WriteLine("true,50,4,700,true,true,7777,false,false,false,false,false");
                tw.Close();
            }
            KeepTilesUpToDate();
            TextReader tr = new StreamReader(saveDir + "config.txt");
            string config = tr.ReadToEnd();
            config = config.Replace("\n", "");
            config = config.Replace("\r", "");
            config = config.Replace(" ", "");
            tr.Close();
            string[] configuration = config.Split(',');
            try
            {
                killGuide = Convert.ToBoolean(configuration[0]);
                invasionMultiplier = Convert.ToInt32(configuration[1]);
                defaultMaxSpawns = Convert.ToInt32(configuration[2]);
                defaultSpawnRate = Convert.ToInt32(configuration[3]);
                kickCheater = Convert.ToBoolean(configuration[4]);
                banCheater = Convert.ToBoolean(configuration[5]);
                serverPort = Convert.ToInt32(configuration[6]);
                enableWhitelist = Convert.ToBoolean(configuration[7]);
                infinateInvasion = Convert.ToBoolean(configuration[8]);
                permaPvp = Convert.ToBoolean(configuration[9]);
                kickTnt = Convert.ToBoolean(configuration[10]);
                banTnt = Convert.ToBoolean(configuration[11]);
                if (infinateInvasion)
                {
                    //Main.startInv();
                }
            }
            catch (Exception e)
            {
                WriteError(e.Message);
            }

            Netplay.serverPort = serverPort;
        }

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