using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria;

namespace TShockAPI
{
    class FileTools
    {
        public static string saveDir = "./tshock/";
        public static void CreateFile(string file)
        {
            using (FileStream fs = File.Create(file)) { }
        }
        /// <summary>
        /// Adds a 'cheater' to cheaters.txt
        /// </summary>
        /// <param name="ply">You should know what this does by now.</param>
        public static void WriteCheater(int ply)
        {
            string ip = Tools.GetRealIP(Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));
            string cheaters = "";
            TextReader tr = new StreamReader(saveDir + "cheaters.txt");
            cheaters = tr.ReadToEnd();
            tr.Close();
            if (cheaters.Contains(Main.player[ply].name) && cheaters.Contains(ip)) { return; }
            TextWriter sw = new StreamWriter(saveDir + "cheaters.txt", true);
            sw.WriteLine("[" + Main.player[ply].name + "] " + "[" + ip + "]");
            sw.Close();
        }
        /// <summary>
        /// Writes a 'banned idiot' to the ban list
        /// </summary>
        /// <param name="ply"></param>
        public static void WriteBan(int ply)
        {
            string ip = Tools.GetRealIP(Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));
            TextWriter tw = new StreamWriter(saveDir + "bans.txt", true);
            tw.WriteLine("[" + Main.player[ply].name + "] " + "[" + ip + "]");
            tw.Close();
        }
        /// <summary>
        /// Writes a tnt user to grief.txt
        /// </summary>
        /// <param name="ply">int player</param>
        public static void WriteGrief(int ply)
        {
            TextWriter tw = new StreamWriter(saveDir + "grief.txt", true);
            tw.WriteLine("[" + Main.player[ply].name + "] [" + Tools.GetRealIP(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint.ToString()) + "]");
            tw.Close();
        }
        /// <summary>
        /// Writes an error message to errors.txt
        /// </summary>
        /// <param name="err">string message</param>
        public static void WriteError(string err)
        {
            if (System.IO.File.Exists(saveDir + "errors.txt"))
            {
                TextWriter tw = new StreamWriter(saveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
            else
            {
                FileTools.CreateFile(saveDir + "errors.txt");
                TextWriter tw = new StreamWriter(saveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
        }
        /// <summary>
        /// Sets up the configuration file for all variables, and creates any missing files.
        /// </summary>
        public static void SetupConfig()
        {
            if (!System.IO.Directory.Exists(saveDir)) { System.IO.Directory.CreateDirectory(saveDir); }
            if (System.IO.File.Exists(saveDir + "tiles.txt"))
            {
                System.IO.File.Delete(saveDir + "tiles.txt");
            }
            if (!System.IO.File.Exists(saveDir + "motd.txt"))
            {
                FileTools.CreateFile(saveDir + "motd.txt");
                TextWriter tw = new StreamWriter(saveDir + "motd.txt");
                tw.WriteLine("This server is running TShock. Type /help for a list of commands.");
                tw.WriteLine("%255,000,000%Current map: %map%");
                tw.WriteLine("Current players: %players%");
                tw.Close();
            }
            if (!System.IO.File.Exists(saveDir + "bans.txt")) { FileTools.CreateFile(saveDir + "bans.txt"); }
            if (!System.IO.File.Exists(saveDir + "cheaters.txt")) { FileTools.CreateFile(saveDir + "cheaters.txt"); }
            if (!System.IO.File.Exists(saveDir + "admins.txt")) { FileTools.CreateFile(saveDir + "admins.txt"); }
            if (!System.IO.File.Exists(saveDir + "grief.txt")) { FileTools.CreateFile(saveDir + "grief.txt"); }
            if (!System.IO.File.Exists(saveDir + "whitelist.txt")) { FileTools.CreateFile(saveDir + "whitelist.txt"); }
            if (!System.IO.File.Exists(saveDir + "config.txt"))
            {
                FileTools.CreateFile(saveDir + "config.txt");
                TextWriter tw = new StreamWriter(saveDir + "config.txt");
                tw.WriteLine("true,50,4,700,true,true,7777,false,false,false,false,false");
                tw.Close();
            }
            TextReader tr = new StreamReader(saveDir + "config.txt");
            string config = tr.ReadToEnd();
            config = config.Replace("\n", "");
            config = config.Replace("\r", "");
            config = config.Replace(" ", "");
            tr.Close();
            string[] configuration = config.Split(',');
            try
            {
                ConfigurationManager.invasionMultiplier = Convert.ToInt32(configuration[1]);
                ConfigurationManager.defaultMaxSpawns = Convert.ToInt32(configuration[2]);
                ConfigurationManager.defaultSpawnRate = Convert.ToInt32(configuration[3]);
                ConfigurationManager.kickCheater = Convert.ToBoolean(configuration[4]);
                ConfigurationManager.banCheater = Convert.ToBoolean(configuration[5]);
                ConfigurationManager.serverPort = Convert.ToInt32(configuration[6]);
                ConfigurationManager.enableWhitelist = Convert.ToBoolean(configuration[7]);
                ConfigurationManager.infiniteInvasion = Convert.ToBoolean(configuration[8]);
                ConfigurationManager.permaPvp = Convert.ToBoolean(configuration[9]);
                ConfigurationManager.kickTnt = Convert.ToBoolean(configuration[10]);
                ConfigurationManager.banTnt = Convert.ToBoolean(configuration[11]);
                NPC.defaultMaxSpawns = ConfigurationManager.defaultMaxSpawns;
                NPC.defaultSpawnRate = ConfigurationManager.defaultSpawnRate;
            }
            catch (Exception e)
            {
                FileTools.WriteError(e.Message);
            }

            Netplay.serverPort = ConfigurationManager.serverPort;
        }
        /// <summary>
        /// Checks if a user is banned
        /// </summary>
        /// <param name="p">string ip</param>
        /// <returns>true/false</returns>
        public static bool CheckBanned(String p)
        {
            String ip = p.Split(':')[0];
            TextReader tr = new StreamReader(saveDir + "bans.txt");
            string banlist = tr.ReadToEnd();
            tr.Close();
            banlist = banlist.Trim();
            if (banlist.Contains(ip))
                return true;
            return false;
        }
        /// <summary>
        /// Tells if a user is on the whitelist
        /// </summary>
        /// <param name="ip">string ip of the user</param>
        /// <returns>true/false</returns>
        public static bool OnWhitelist(string ip)
        {
            if (!ConfigurationManager.enableWhitelist) { return true; }
            if (!System.IO.File.Exists(saveDir + "whitelist.txt")) { FileTools.CreateFile(saveDir + "whitelist.txt"); TextWriter tw = new StreamWriter(saveDir + "whitelist.txt"); tw.WriteLine("127.0.0.1"); tw.Close(); }
            TextReader tr = new StreamReader(saveDir + "whitelist.txt");
            string whitelist = tr.ReadToEnd();
            ip = Tools.GetRealIP(ip);
            if (whitelist.Contains(ip)) { return true; } else { return false; }
        }
        /// <summary>
        /// Tells if the user is on grief.txt
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool CheckGreif(String ip)
        {
            ip = Tools.GetRealIP(ip);
            if (!ConfigurationManager.banTnt) { return false; }
            TextReader tr = new StreamReader(saveDir + "grief.txt");
            string list = tr.ReadToEnd();
            tr.Close();

            return list.Contains(ip);
        }

        public static bool CheckCheat(String ip)
        {
            ip = Tools.GetRealIP(ip);
            if (!ConfigurationManager.banCheater) { return false; }
            TextReader tr = new StreamReader(saveDir + "cheaters.txt");
            string trr = tr.ReadToEnd();
            tr.Close();
            if (trr.Contains(ip))
            {
                return true;
            }
            return false;
        }
        public FileTools() { }
    }
}
