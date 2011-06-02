using System;
using System.IO;
using Terraria;
using System.Web;

namespace TShockAPI
{
    class FileTools
    {
        public static string SaveDir = "./tshock/";
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
            TextReader tr = new StreamReader(SaveDir + "cheaters.txt");
            cheaters = tr.ReadToEnd();
            tr.Close();
            if (cheaters.Contains(Main.player[ply].name) && cheaters.Contains(ip)) { return; }
            TextWriter sw = new StreamWriter(SaveDir + "cheaters.txt", true);
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
            TextWriter tw = new StreamWriter(SaveDir + "bans.txt", true);
            tw.WriteLine("[" + Main.player[ply].name + "] " + "[" + ip + "]");
            tw.Close();
        }
        /// <summary>
        /// Writes a tnt user to grief.txt
        /// </summary>
        /// <param name="ply">int player</param>
        public static void WriteGrief(int ply)
        {
            TextWriter tw = new StreamWriter(SaveDir + "grief.txt", true);
            tw.WriteLine("[" + Main.player[ply].name + "] [" + Tools.GetRealIP(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint.ToString()) + "]");
            tw.Close();
        }
        /// <summary>
        /// Writes an error message to errors.txt
        /// </summary>
        /// <param name="err">string message</param>
        public static void WriteError(string err)
        {
            if (System.IO.File.Exists(SaveDir + "errors.txt"))
            {
                TextWriter tw = new StreamWriter(SaveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
            else
            {
                FileTools.CreateFile(SaveDir + "errors.txt");
                TextWriter tw = new StreamWriter(SaveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
        }
        /// <summary>
        /// Sets up the configuration file for all variables, and creates any missing files.
        /// </summary>
        public static void SetupConfig()
        {
            if (!System.IO.Directory.Exists(SaveDir)) { System.IO.Directory.CreateDirectory(SaveDir); }
            if (!System.IO.File.Exists(SaveDir + "motd.txt"))
            {
                FileTools.CreateFile(SaveDir + "motd.txt");
                TextWriter tw = new StreamWriter(SaveDir + "motd.txt");
                tw.WriteLine("This server is running TShock. Type /help for a list of commands.");
                tw.WriteLine("%255,000,000%Current map: %map%");
                tw.WriteLine("Current players: %players%");
                tw.Close();
            }
            if (!System.IO.File.Exists(SaveDir + "bans.txt")) { FileTools.CreateFile(SaveDir + "bans.txt"); }
            if (!System.IO.File.Exists(SaveDir + "cheaters.txt")) { FileTools.CreateFile(SaveDir + "cheaters.txt"); }
            if (!System.IO.File.Exists(SaveDir + "admins.txt")) { FileTools.CreateFile(SaveDir + "admins.txt"); }
            if (!System.IO.File.Exists(SaveDir + "grief.txt")) { FileTools.CreateFile(SaveDir + "grief.txt"); }
            if (!System.IO.File.Exists(SaveDir + "whitelist.txt")) { FileTools.CreateFile(SaveDir + "whitelist.txt"); }
            ConfigurationManager.WriteJsonConfiguration();
            ConfigurationManager.ReadJsonConfiguration();
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
            TextReader tr = new StreamReader(SaveDir + "bans.txt");
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
            if (!System.IO.File.Exists(SaveDir + "whitelist.txt")) { FileTools.CreateFile(SaveDir + "whitelist.txt"); TextWriter tw = new StreamWriter(SaveDir + "whitelist.txt"); tw.WriteLine("127.0.0.1"); tw.Close(); }
            TextReader tr = new StreamReader(SaveDir + "whitelist.txt");
            string whitelist = tr.ReadToEnd();
            ip = Tools.GetRealIP(ip);
            return whitelist.Contains(ip);
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
            TextReader tr = new StreamReader(SaveDir + "grief.txt");
            string list = tr.ReadToEnd();
            tr.Close();

            return list.Contains(ip);
        }

        public static bool CheckCheat(String ip)
        {
            ip = Tools.GetRealIP(ip);
            if (!ConfigurationManager.banCheater) { return false; }
            TextReader tr = new StreamReader(SaveDir + "cheaters.txt");
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
