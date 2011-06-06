using System.IO;
using Terraria;

namespace TShockAPI
{
    internal class FileTools
    {
        public static string SaveDir = "./tshock/";

        public static void CreateFile(string file)
        {
            using (FileStream fs = File.Create(file))
            {
            }
        }

        /// <summary>
        /// Writes an error message to errors.txt
        /// </summary>
        /// <param name="err">string message</param>
        public static void WriteError(string err)
        {
            if (File.Exists(SaveDir + "errors.txt"))
            {
                TextWriter tw = new StreamWriter(SaveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
            else
            {
                CreateFile(SaveDir + "errors.txt");
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
            if (!Directory.Exists(SaveDir))
            {
                Directory.CreateDirectory(SaveDir);
            }
            if (!File.Exists(SaveDir + "motd.txt"))
            {
                CreateFile(SaveDir + "motd.txt");
                TextWriter tw = new StreamWriter(SaveDir + "motd.txt");
                tw.WriteLine("This server is running TShock. Type /help for a list of commands.");
                tw.WriteLine("%255,000,000%Current map: %map%");
                tw.WriteLine("Current players: %players%");
                tw.Close();
            }
            if (!File.Exists(SaveDir + "bans.txt"))
            {
                CreateFile(SaveDir + "bans.txt");
            }
            if (!File.Exists(SaveDir + "whitelist.txt"))
            {
                CreateFile(SaveDir + "whitelist.txt");
            }
            if (!File.Exists(SaveDir + "groups.txt"))
            {
                CreateFile(SaveDir + "groups.txt");
                StreamWriter sw = new StreamWriter(SaveDir + "groups.txt");
                sw.Write(Resources.groups);
                sw.Close();
            }
            if (!File.Exists(SaveDir + "users.txt"))
            {
                CreateFile(SaveDir + "users.txt");
                StreamWriter sw = new StreamWriter(SaveDir + "users.txt");
                sw.Write(Resources.users);
                sw.Close();
            }
            ConfigurationManager.WriteJsonConfiguration();
            ConfigurationManager.ReadJsonConfiguration();
            Netplay.serverPort = ConfigurationManager.serverPort;
        }

        /// <summary>
        /// Tells if a user is on the whitelist
        /// </summary>
        /// <param name="ip">string ip of the user</param>
        /// <returns>true/false</returns>
        public static bool OnWhitelist(string ip)
        {
            if (!ConfigurationManager.enableWhitelist)
            {
                return true;
            }
            if (!File.Exists(SaveDir + "whitelist.txt"))
            {
                CreateFile(SaveDir + "whitelist.txt");
                TextWriter tw = new StreamWriter(SaveDir + "whitelist.txt");
                tw.WriteLine("127.0.0.1");
                tw.Close();
            }
            TextReader tr = new StreamReader(SaveDir + "whitelist.txt");
            string whitelist = tr.ReadToEnd();
            ip = Tools.GetRealIP(ip);
            return whitelist.Contains(ip);
        }
    }
}