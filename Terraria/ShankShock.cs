using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace Terraria
{
	class ShankShock
	{
        private static double version = 2;
        private static bool shownVersion = false;

        public static bool enableGuide = true;
        public static int invasionMultiplier = 1;
        public static int defaultMaxSpawns = 4;
        public static int defaultSpawnRate = 700;
        public static bool kickCheater = true;
        public static bool banCheater = true;
        public static int serverPort = 7777;
        public static bool enableWhitelist = false;
        public static bool infinateInvasion = false;
        public static bool permaPvp = false;
        private static string saveDir = "./tshock/";
        public static int killCount = 0;
        public static bool shownOneTimeInvasionMinder = false;

        public static string tileWhitelist = "";
        private static bool banTnt = false;
        private static bool kickTnt = false;

        public enum NPCList : int
        {
            WORLD_EATER=0,
            EYE=1,
            SKELETRON=2
        }

        public static void showUpdateMinder(int ply)
        {
            if (!shownVersion)
            {
                if (isAdmin(findPlayer(ply)))
                {
                    WebClient client = new WebClient();
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705;)");
                    try
                    {
                        string updateVersion = client.DownloadString("http://shankshock.com/tshock.txt");
                        string[] changes = updateVersion.Split(',');
                        float[] color = { 255, 255, 000 };
                        if (Convert.ToDouble(changes[0]) > version)
                        {
                            sendMessage(ply, "This server is out of date. Version " + changes[0] + " is out.", color);
                            for (int i = 1; i <= changes.Length; i++)
                            {
                                sendMessage(ply, changes[i], color);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _writeError(e.Message);
                    }
                    shownVersion = true;
                }
            }
        }

        public static void incrementKills()
        {
            killCount++;
            Random r = new Random();
            int random = r.Next(5);
            if (killCount % 100 == 0)
            {
                switch (random)
                {
                    case 0:
                        ShankShock.broadcast("You call that a lot? " + killCount + " goblins killed!");
                        break;
                    case 1:
                        ShankShock.broadcast("Fatality! " + killCount + " goblins killed!");
                        break;
                    case 2:
                        ShankShock.broadcast("Number of 'noobs' killed to date: " + killCount);
                        break;
                    case 3:
                        ShankShock.broadcast("Duke Nukem would be proud. " + killCount + " goblins killed.");
                        break;
                    case 4:
                        ShankShock.broadcast("You call that a lot? " + killCount + " goblins killed!");
                        break;
                    case 5:
                        ShankShock.broadcast(killCount + " copies of Call of Duty smashed.");
                        break;
                }

            }
        }

        public static bool onWhitelist(string ip)
        {
            if (!enableWhitelist) { return true; }
            if (!System.IO.File.Exists(saveDir +"whitelist.txt")) { createFile(saveDir + "whitelist.txt"); TextWriter tw = new StreamWriter(saveDir + "whitelist.txt"); tw.WriteLine("127.0.0.1"); tw.Close(); }
            TextReader tr = new StreamReader(saveDir + "whitelist.txt");
            string whitelist = tr.ReadToEnd();
            ip = getRealIP(ip);
            if (whitelist.Contains(ip)) { return true; } else { return false; }
        }

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

        private static string getPlayers()
        {
            string str = "";
            for (int i = 0; i < 8; i++)
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

        public static void showmotd(int ply)
        {
            string foo = "";
            TextReader tr = new StreamReader(saveDir + "motd.txt");
            while ((foo = tr.ReadLine()) != null)
            {
                foo = foo.Replace("%map%", Main.worldName);
                foo = foo.Replace("%players%", getPlayers());
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
                            pC[0] = Clamp(Convert.ToInt32(pCc[0]), 255, 0);
                            pC[1] = Clamp(Convert.ToInt32(pCc[1]), 255, 0);
                            pC[2] = Clamp(Convert.ToInt32(pCc[2]), 255, 0);
                            sendMessage(ply, foo, pC);
                            continue;
                        }
                        catch (Exception e)
                        {
                            _writeError(e.Message);
                        }
                    }
                }
                sendMessage(ply, foo);
            }
            tr.Close();
        }

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

        private static void createFile(string file)
        {
            using (FileStream fs = File.Create(file)) { }
        }

        private static void keepTilesUpToDate()
        {
            TextReader tr = new StreamReader(saveDir + "tiles.txt");
            string file = tr.ReadToEnd();
            tr.Close();
            if (!file.Contains("0x3d"))
            {
                System.IO.File.Delete(saveDir + "tiles.txt");
                createFile(saveDir + "tiles.txt");
                TextWriter tw = new StreamWriter(saveDir + "tiles.txt");
                tw.Write("0x03, 0x05, 0x14, 0x25, 0x18, 0x18, 0x20, 0x1b, 0x34, 0x48, 0x33, 0x3d, 0x47, 0x49, 0x4a, 0x35, 0x3d, 0x3e, 0x45, 0x47, 0x49, 0x4a,");
                tw.Close();
            }
        }

        public static void setupConfiguration()
        {
            if (!System.IO.Directory.Exists(saveDir)) { System.IO.Directory.CreateDirectory(saveDir); }
            if (!System.IO.File.Exists(saveDir + "tiles.txt"))
            {
                createFile(saveDir + "tiles.txt");
                TextWriter tw = new StreamWriter(saveDir + "tiles.txt");
                tw.Write("0x03, 0x05, 0x14, 0x25, 0x18, 0x18, 0x20, 0x1b, 0x34, 0x48, 0x33, 0x3d, 0x47, 0x49, 0x4a, 0x35, 0x3d, 0x3e, 0x45, 0x47, 0x49, 0x4a,");
                tw.Close();
            }
            if (!System.IO.File.Exists(saveDir + "motd.txt")) { 
                createFile(saveDir + "motd.txt"); 
                TextWriter tw = new StreamWriter(saveDir + "motd.txt"); 
                tw.WriteLine("This server is running TShock. Type /help for a list of commands.");
                tw.WriteLine("%255,000,000%Current map: %map%");
                tw.WriteLine("Current players: %players%");
                tw.Close(); }
            if (!System.IO.File.Exists(saveDir +"bans.txt")) { createFile(saveDir + "bans.txt"); }
            if (!System.IO.File.Exists(saveDir +"cheaters.txt")) { createFile(saveDir + "cheaters.txt"); }
            if (!System.IO.File.Exists(saveDir +"admins.txt")) { createFile(saveDir + "admins.txt");  }
            if (!System.IO.File.Exists(saveDir + "grief.txt")) { createFile(saveDir + "grief.txt"); }
            if (!System.IO.File.Exists(saveDir +"config.txt"))
            {
                createFile(saveDir + "config.txt");
                TextWriter tw = new StreamWriter(saveDir + "config.txt");
                tw.WriteLine("true,50,4,700,true,true,7777,false,false,false,false,false");
                tw.Close();
            }
            keepTilesUpToDate();
            TextReader tr = new StreamReader(saveDir + "config.txt");
            string config = tr.ReadToEnd();
            config = config.Replace("\n", "");
            config = config.Replace("\r", "");
            config = config.Replace(" ", "");
            tr.Close();
            string[] configuration = config.Split(',');
            try
            {
                enableGuide = Convert.ToBoolean(configuration[0]);
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
                    Main.startInv();
                }
            }
            catch (Exception e)
            {
                _writeError(e.Message);
            }

            Netplay.serverPort = serverPort;
        }

        public static bool tileOnWhitelist(byte tile)
        {
            int _tile = (int)tile;
            TextReader tr2 = new StreamReader(saveDir + "tiles.txt");
            tileWhitelist = tr2.ReadToEnd(); tr2.Close();
            string hexValue = _tile.ToString("X");
            return tileWhitelist.Contains(hexValue);
        }

        public static void setupJsonConfiguration()
        {
            throw new NotImplementedException();
        }

        public static void handleGrief(int ply)
        {
            if (banTnt == false && kickTnt == false) { return; }
            if (banTnt) { _writeGrief(ply); }
            ShankShock.broadcast(findPlayer(ply) + " was " + (banTnt ? "banned " : "kicked ") + "for kill tile abuse.");
            if (kickTnt) { kick(ply); }
        }

        private static void _writeGrief(int ply)
        {
            TextWriter tw = new StreamWriter(saveDir + "grief.txt", true);
            tw.WriteLine("[" + Main.player[ply].name + "] [" + getRealIP(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint.ToString()) + "]");
            tw.Close();
        }

        private static void _writeError(string err)
        {
            if (System.IO.File.Exists(saveDir +"errors.txt"))
            {
                TextWriter tw = new StreamWriter(saveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
            else
            {
                createFile(saveDir + "errors.txt");
                TextWriter tw = new StreamWriter(saveDir + "errors.txt", true);
                tw.WriteLine(err);
                tw.Close();
            }
        }

        public static void _writeban(int ply)
        {
            string ip = ShankShock.getRealIP(Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));
            TextWriter tw = new StreamWriter(saveDir + "bans.txt", true);
            tw.WriteLine("[" + Main.player[ply].name + "] " + "[" + ip + "]");
            tw.Close();
        }

        public static void _writecheater(int ply)
        {
            string ip = ShankShock.getRealIP(Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));
            string cheaters = "";
            TextReader tr = new StreamReader(saveDir + "cheaters.txt");
            cheaters = tr.ReadToEnd();
            tr.Close();
            if (cheaters.Contains(Main.player[ply].name) && cheaters.Contains(ip)) { return; }
            TextWriter sw = new StreamWriter(saveDir + "cheaters.txt", true);
            sw.WriteLine("[" + Main.player[ply].name + "] " + "[" + ip + "]");
            sw.Close();
        }

        public static void kick(int ply)
        {
            Netplay.serverSock[ply].kill = true;
            Netplay.serverSock[ply].Reset();
            NetMessage.syncPlayers();
        }

        public static void handleCheater(int ply)
        {
            string cheater = ShankShock.findPlayer(ply);
            string ip = ShankShock.getRealIP(Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint));

            _writecheater(ply);
            if (!kickCheater) { return; }
            Netplay.serverSock[ply].kill = true;
            Netplay.serverSock[ply].Reset();
            NetMessage.syncPlayers();
            ShankShock.broadcast(cheater + " was " + (banCheater ? "banned " : "kicked ") +  "for cheating.");

        }

        public static bool checkGrief(String ip)
        {
            ip = getRealIP(ip);
            if (!banTnt) { return false; }
            TextReader tr = new StreamReader(saveDir + "grief.txt");
            string list = tr.ReadToEnd();
            tr.Close();

            return list.Contains(ip);
        }

        public static bool checkCheat(String ip)
        {
            ip = getRealIP(ip);
            if (!banCheater) { return false; }
            TextReader tr = new StreamReader(saveDir + "cheaters.txt");
            string trr = tr.ReadToEnd();
            tr.Close();
            if (trr.Contains(ip)) {
                return true;
            }
            return false;
        }

        public static bool checkBanned(String p)
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

        public static void sendMessage(int ply, string msg, float[] color)
        {
            NetMessage.SendData(0x19, ply, -1, msg, 8, color[0], color[1], color[2]);
        }

        public static void sendMessage(int ply, string message)
        {
            NetMessage.SendData(0x19, ply, -1, message, 8, 0f, 255f, 0f);
        }

        public static int findPlayer(string ply)
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

        public static string findPlayer(int ply)
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

        public static void broadcast(string msg)
        {
            for (int i = 0; i < Main.player.Length; i++)
            {
                sendMessage(i, msg);
            }
        }

        public static bool isAdmin(string ply)
        {
            string remoteEndPoint = Convert.ToString((Netplay.serverSock[ShankShock.findPlayer(ply)].tcpClient.Client.RemoteEndPoint));
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

        public static string getRealIP(string mess)
        {
            return mess.Split(':')[0];
        }
	}
}
