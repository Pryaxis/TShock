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
/* TShock wouldn't be possible without:
 * Github
 * Microsoft Visual Studio 2010
 * HostPenda
 * And you, for your continued support and devotion to the evolution of TShock
 * Kerplunc Gaming
 * TerrariaGSP
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Community.CsharpSqlite.SQLiteClient;
using Microsoft.Xna.Framework;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;
using TShockAPI.DB;

namespace TShockAPI
{
    [APIVersion(1, 5)]
    public class TShock : TerrariaPlugin
    {
        public static readonly Version VersionNum = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string VersionCodename = "Milestone 3";

        public static string SavePath = "tshock";

        public static TSPlayer[] Players = new TSPlayer[Main.maxPlayers];
        public static BanManager Bans;
        public static WarpManager Warps;
        public static RegionManager Regions;
        public static BackupManager Backups;
        public static GroupManager Groups;
        public static UserManager Users;
        public static ItemManager Itembans;

        public static ConfigFile Config { get; set; }

        public static IDbConnection DB;

        public static Process TShockProcess;
        public static bool OverridePort;

        public static double ElapsedTime;

        public override Version Version
        {
            get { return VersionNum; }
        }

        public override string Name
        {
            get { return "TShock"; }
        }

        public override string Author
        {
            get { return "The TShock Team"; }
        }

        public override string Description
        {
            get { return "The administration modification of the future."; }
        }

        public TShock(Main game)
            : base(game)
        {
            Config = new ConfigFile();
            Order = 0;
        }

        public override void Initialize()
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

#if DEBUG
            Log.Initialize(Path.Combine(SavePath, "log.txt"), LogLevel.All, false);
#else
            Log.Initialize(Path.Combine(SavePath, "log.txt"), LogLevel.All & ~LogLevel.Debug, false);
#endif
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;


            if (File.Exists(Path.Combine(SavePath, "tshock.pid")))
            {
                Log.ConsoleInfo("TShock was improperly shut down. Deleting invalid pid file...");
                File.Delete(Path.Combine(SavePath, "tshock.pid"));
            }
            File.WriteAllText(Path.Combine(SavePath, "tshock.pid"), Process.GetCurrentProcess().Id.ToString());

            ConfigFile.ConfigRead += OnConfigRead;
            FileTools.SetupConfig();

            HandleCommandLine(Environment.GetCommandLineArgs());

            if (Config.StorageType.ToLower() == "sqlite")
            {
                string sql = Path.Combine(SavePath, "tshock.sqlite");
                DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
                DB.Open();
            }
            else if (Config.StorageType.ToLower() == "mysql")
            {
                try
                {
                    var hostport = Config.MySqlHost.Split(':');
                    DB = new MySqlConnection();
                    DB.ConnectionString =
                        String.Format("Server='{0}'; Port='{1}'; Database='{2}'; Uid='{3}'; Pwd='{4}';",
                                      hostport[0],
                                      hostport.Length > 1 ? hostport[1] : "3306",
                                      Config.MySqlDbName,
                                      Config.MySqlUsername,
                                      Config.MySqlPassword
                            );
                    DB.Open();
                }
                catch (MySqlException ex)
                {
                    Log.Error(ex.ToString());
                    throw new Exception("MySql not setup correctly");
                }
            }
            else
            {
                throw new Exception("Invalid storage type");
            }

            DBTools.database = DB;

            Backups = new BackupManager(Path.Combine(SavePath, "backups"));
            Backups.KeepFor = Config.BackupKeepFor;
            Backups.Interval = Config.BackupInterval;
            Bans = new BanManager(DB);
            Warps = new WarpManager(DB);
            Users = new UserManager(DB);
            Groups = new GroupManager(DB);
            Groups.LoadPermisions();
            Regions = new RegionManager(DB);
            Itembans = new ItemManager(DB);

            Log.ConsoleInfo(string.Format("TShock Version {0} ({1}) now running.", Version, VersionCodename));

            GameHooks.PostInitialize += OnPostInit;
            GameHooks.Update += OnUpdate;
            ServerHooks.Join += OnJoin;
            ServerHooks.Leave += OnLeave;
            ServerHooks.Chat += OnChat;
            ServerHooks.Command += ServerHooks_OnCommand;
            NetHooks.GetData += GetData;
            NetHooks.GreetPlayer += OnGreetPlayer;
            NpcHooks.StrikeNpc += NpcHooks_OnStrikeNpc;
            NetHooks.SendData += NetHooks_SendData;

            GetDataHandlers.InitGetDataHandler();
            Commands.InitCommands();
            //RconHandler.StartThread();

            Log.ConsoleInfo("AutoSave " + (Config.AutoSave ? "Enabled" : "Disabled"));
            Log.ConsoleInfo("Backups " + (Backups.Interval > 0 ? "Enabled" : "Disabled"));
        }

        private void NetHooks_SendData(SendDataEventArgs e)
        {
            if (e.MsgID == PacketTypes.PlayerActive)
            {
                //Debug.WriteLine("Send: {0} ({1:X2})", (byte)e.MsgID, e.MsgID.ToString());
            }
        }

        public override void DeInitialize()
        {
            DB.Close();
            GameHooks.PostInitialize -= OnPostInit;
            GameHooks.Update -= OnUpdate;
            ServerHooks.Join -= OnJoin;
            ServerHooks.Leave -= OnLeave;
            ServerHooks.Chat -= OnChat;
            ServerHooks.Command -= ServerHooks_OnCommand;
            NetHooks.GetData -= GetData;
            NetHooks.GreetPlayer -= OnGreetPlayer;
            NpcHooks.StrikeNpc -= NpcHooks_OnStrikeNpc;
            if (File.Exists(Path.Combine(SavePath, "tshock.pid")))
            {
                Console.WriteLine("Thanks for using TShock! Process ID file is now being destroyed.");
                File.Delete(Path.Combine(SavePath, "tshock.pid"));
            }
        }

        /// <summary>
        /// Handles exceptions that we didn't catch or that Red fucked up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject.ToString());

            if (e.ExceptionObject.ToString().Contains("Terraria.Netplay.ListenForClients") ||
                e.ExceptionObject.ToString().Contains("Terraria.Netplay.ServerLoop"))
            {
                var sb = new List<string>();
                for (int i = 0; i < Netplay.serverSock.Length; i++)
                {
                    if (Netplay.serverSock[i] == null)
                    {
                        sb.Add("Sock[" + i + "]");
                    }
                    else if (Netplay.serverSock[i].tcpClient == null)
                    {
                        sb.Add("Tcp[" + i + "]");
                    }
                }
                Log.Error(string.Join(", ", sb));
            }

            if (e.IsTerminating)
            {
                if (Main.worldPathName != null)
                {
                    Main.worldPathName += ".crash";
                    WorldGen.saveWorld();
                }
                DeInitialize();
            }
        }

        private void HandleCommandLine(string[] parms)
        {
            for (int i = 0; i < parms.Length; i++)
            {
                if (parms[i].ToLower() == "-ip")
                {
                    IPAddress ip;
                    if (IPAddress.TryParse(parms[++i], out ip))
                    {
                        Netplay.serverListenIP = ip;
                        Console.Write("Using IP: {0}", ip);
                    }
                    else
                    {
                        Console.WriteLine("Bad IP: {0}", parms[i]);
                    }
                }
                if (parms[i].ToLower() == "-configpath")
                {
                    var path = parms[++i];
                    if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                    {
                        SavePath = path;
                        Log.ConsoleInfo("Config path has been set to " + path);
                    }
                }
                if (parms[i].ToLower() == "-worldpath")
                {
                    var path = parms[++i];
                    if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                    {
                        Main.WorldPath = path;
                        Log.ConsoleInfo("World path has been set to " + path);
                    }
                }
                if (parms[i].ToLower() == "-port")
                {
                    int port = Convert.ToInt32(parms[++i]);
                    Netplay.serverPort = port;
                    Config.ServerPort = port;
                    OverridePort = true;
                    Log.ConsoleInfo("Port overridden by startup argument. Set to " + port);
                }
            }
        }

        /*
         * Hooks:
         * 
         */

        public static int AuthToken = -1;

        private void OnPostInit()
        {
            if (!File.Exists(Path.Combine(SavePath, "auth.lck")) && !File.Exists(Path.Combine(SavePath, "authcode.txt")))
            {
                var r = new Random((int) DateTime.Now.ToBinary());
                AuthToken = r.Next(100000, 10000000);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("TShock Notice: To become SuperAdmin, join the game and type /auth " + AuthToken);
                Console.WriteLine("This token will display until disabled by verification. (/auth-verify)");
                Console.ForegroundColor = ConsoleColor.Gray;
                FileTools.CreateFile(Path.Combine(SavePath, "authcode.txt"));
                TextWriter tw = new StreamWriter(Path.Combine(SavePath, "authcode.txt"));
                tw.WriteLine(AuthToken);
                tw.Close();
            }
            else if (File.Exists(Path.Combine(SavePath, "authcode.txt")))
            {
                TextReader tr = new StreamReader(Path.Combine(SavePath, "authcode.txt"));
                AuthToken = Convert.ToInt32(tr.ReadLine());
                tr.Close();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(
                    "TShock Notice: authcode.txt is still present, and the AuthToken located in that file will be used.");
                Console.WriteLine("To become superadmin, join the game and type /auth " + AuthToken);
                Console.WriteLine("This token will display until disabled by verification. (/auth-verify)");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            else
            {
                AuthToken = 0;
            }
            Regions.ReloadAllRegions();
        }


        private DateTime LastCheck = DateTime.UtcNow;

        private void OnUpdate(GameTime time)
        {
            UpdateManager.UpdateProcedureCheck();

            if (Backups.IsBackupTime)
                Backups.Backup();

            //call these every second, not every update
            if ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1)
            {
                LastCheck = DateTime.UtcNow;
                foreach (TSPlayer player in Players)
                {
                    if (player != null && player.Active)
                    {
                        if (player.TilesDestroyed != null)
                        {
                            if (player.TileThreshold >= Config.TileThreshold)
                            {
                                if (Tools.HandleTntUser(player, "Kill tile abuse detected."))
                                {
                                    TSPlayer.Server.RevertKillTile(player.TilesDestroyed);
                                }
                            }
                            if (player.TileThreshold > 0)
                            {
                                player.TileThreshold = 0;
                                player.TilesDestroyed.Clear();
                            }
                        }

                        if (!player.Group.HasPermission("usebanneditem"))
                        {
                            var inv = player.TPlayer.inventory;

                            for (int i = 0; i < inv.Length; i++)
                            {
                                if (inv[i] != null && Itembans.ItemIsBanned(inv[i].name))
                                {
                                    player.Disconnect("Using banned item: " + inv[i].name + ", remove it and rejoin");
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnJoin(int ply, HandledEventArgs handler)
        {
            var player = new TSPlayer(ply);
            if (Config.EnableDNSHostResolution)
            {
                player.Group = Users.GetGroupForIPExpensive(player.IP);
            }
            else
            {
                player.Group = Users.GetGroupForIP(player.IP);
            }

            if (Tools.ActivePlayers() + 1 > Config.MaxSlots && !player.Group.HasPermission("reservedslot"))
            {
                Tools.ForceKick(player, "Server is full");
                handler.Handled = true;
                return;
            }

            var ban = Bans.GetBanByIp(player.IP);
            if (ban != null)
            {
                Tools.ForceKick(player, string.Format("You are banned: {0}", ban.Reason));
                handler.Handled = true;
                return;
            }

            if (!FileTools.OnWhitelist(player.IP))
            {
                Tools.ForceKick(player, "Not on whitelist.");
                handler.Handled = true;
                return;
            }

            Players[ply] = player;
        }

        private void OnLeave(int ply)
        {
            var tsplr = Players[ply];
            Players[ply] = null;

            if (tsplr != null && tsplr.ReceivedInfo)
            {
                Log.Info(string.Format("{0} left.", tsplr.Name));

                if (Config.RememberLeavePos)
                {
                    RemeberedPosManager.RemeberedPosistions.Add(new RemeberedPos(tsplr.IP,
                                                                                 new Vector2(tsplr.X/16,
                                                                                             (tsplr.Y/16) + 3)));
                    RemeberedPosManager.WriteSettings();
                }
            }
        }

        private void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
        {
            var tsplr = Players[msg.whoAmI];
            if (tsplr == null)
            {
                e.Handled = true;
                return;
            }

            if (!Tools.ValidString(text))
            {
                Tools.Kick(tsplr, "Unprintable character in chat");
                e.Handled = true;
                return;
            }

            if (msg.whoAmI != ply)
            {
                e.Handled = Tools.HandleGriefer(tsplr, "Faking Chat");
                return;
            }

            if (tsplr.Group.HasPermission("adminchat") && !text.StartsWith("/") && Config.AdminChatEnabled)
            {
                Tools.Broadcast(Config.AdminChatPrefix + "<" + tsplr.Name + "> " + text,
                                (byte) Config.AdminChatRGB[0], (byte) Config.AdminChatRGB[1],
                                (byte) Config.AdminChatRGB[2]);
                e.Handled = true;
                return;
            }

            if (text.StartsWith("/"))
            {
                try
                {
                    e.Handled = Commands.HandleCommand(tsplr, text);
                }
                catch (Exception ex)
                {
                    Log.ConsoleError("Command exception");
                    Log.Error(ex.ToString());
                }
            }
            else
            {
                Log.Info(string.Format("{0} said: {1}", tsplr.Name, text));
            }
        }

        /// <summary>
        /// When a server command is run.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="e"></param>
        private void ServerHooks_OnCommand(string text, HandledEventArgs e)
        {
            if (e.Handled)
                return;

            // Damn you ThreadStatic and Redigit
            if (Main.rand == null)
            {
                Main.rand = new Random();
            }
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random();
            }

            if (text.StartsWith("exit"))
            {
                Tools.ForceKickAll("Server shutting down!");
                var sb = new StringBuilder();
                for (int i = 0; i < Main.maxItemTypes; i++)
                {
                    string itemName = Main.itemName[i];
                    string itemID = (i).ToString();
                    sb.Append("ItemList.Add(\"" + itemName + "\");").AppendLine();
                }

                File.WriteAllText("item.txt", sb.ToString());
            }
            else if (text.StartsWith("playing") || text.StartsWith("/playing"))
            {
                int count = 0;
                foreach (TSPlayer player in Players)
                {
                    if (player != null && player.Active)
                    {
                        count++;
                        TSPlayer.Server.SendMessage(string.Format("{0} ({1}) [{2}]", player.Name, player.IP,
                                                                  player.Group.Name));
                    }
                }
                TSPlayer.Server.SendMessage(string.Format("{0} players connected.", count));
                e.Handled = true;
            }
            else if (text.StartsWith("say "))
            {
                Log.Info(string.Format("Server said: {0}", text.Remove(0, 4)));
            }
            else if (text == "autosave")
            {
                Main.autoSave = Config.AutoSave = !Config.AutoSave;
                Log.ConsoleInfo("AutoSave " + (Config.AutoSave ? "Enabled" : "Disabled"));
                e.Handled = true;
            }
            else if (text.StartsWith("/"))
            {
                if (Commands.HandleCommand(TSPlayer.Server, text))
                    e.Handled = true;
            }
        }

        private void GetData(GetDataEventArgs e)
        {
            PacketTypes type = e.MsgID;
            var player = Players[e.Msg.whoAmI];
            if (player == null)
            {
                e.Handled = true;
                return;
            }

            if (!player.ConnectionAlive)
            {
                e.Handled = true;
                return;
            }

            //if (type == PacketTypes.SyncPlayers)
            //Debug.WriteLine("Recv: {0:X} ({2}): {3} ({1:XX})", player.Index, (byte)type, player.TPlayer.dead ? "dead " : "alive", type.ToString());

            // Stop accepting updates from player as this player is going to be kicked/banned during OnUpdate (different thread so can produce race conditions)
            if ((Config.BanKillTileAbusers || Config.KickKillTileAbusers) &&
                player.TileThreshold >= Config.TileThreshold && !player.Group.HasPermission("ignoregriefdetection"))
            {
                Log.Debug("Rejecting " + type + " from " + player.Name + " as this player is about to be kicked");
                e.Handled = true;
            }
            else
            {
                using (var data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
                {
                    try
                    {
                        if (GetDataHandlers.HandlerGetData(type, player, data))
                            e.Handled = true;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                    }
                }
            }
        }

        private void OnGreetPlayer(int who, HandledEventArgs e)
        {
            var player = Players[who];
            if (player == null)
            {
                e.Handled = true;
                return;
            }

            Log.Info(string.Format("{0} ({1}) from '{2}' group joined.", player.Name, player.IP, player.Group.Name));

            Tools.ShowFileToUser(player, "motd.txt");
            if (HackedHealth(player))
            {
                Tools.HandleCheater(player, "Hacked health.");
            }
            if (Config.AlwaysPvP)
            {
                player.SetPvP(true);
                player.SendMessage(
                    "PvP is forced! Enable PvP else you can't deal damage to other people. (People can kill you)",
                    Color.Red);
            }
            if (player.Group.HasPermission("causeevents") && Config.InfiniteInvasion)
            {
                StartInvasion();
            }
            if (Config.RememberLeavePos)
            {
                foreach (RemeberedPos playerIP in RemeberedPosManager.RemeberedPosistions)
                {
                    if (playerIP.IP == player.IP)
                    {
                        player.Teleport((int) playerIP.Pos.X, (int) playerIP.Pos.Y);
                        RemeberedPosManager.RemeberedPosistions.Remove(playerIP);
                        RemeberedPosManager.WriteSettings();
                        break;
                    }
                }
            }
            e.Handled = true;
        }

        private void NpcHooks_OnStrikeNpc(NpcStrikeEventArgs e)
        {
            if (Config.InfiniteInvasion)
            {
                IncrementKills();
                if (Main.invasionSize < 10)
                {
                    Main.invasionSize = 20000000;
                }
            }
        }

        private void OnSaveWorld(bool resettime, HandledEventArgs e)
        {
            Tools.Broadcast("Saving world. Momentary lag might result from this.", Color.Red);
            Thread SaveWorld = new Thread(Tools.SaveWorld);
            SaveWorld.Start();
            e.Handled = true;
        }

        /*
         * Useful stuff:
         * */

        public static void StartInvasion()
        {
            Main.invasionType = 1;
            if (Config.InfiniteInvasion)
            {
                Main.invasionSize = 20000000;
            }
            else
            {
                Main.invasionSize = 100 + (Config.InvasionMultiplier*Tools.ActivePlayers());
            }

            Main.invasionWarn = 0;
            if (new Random().Next(2) == 0)
            {
                Main.invasionX = 0.0;
            }
            else
            {
                Main.invasionX = Main.maxTilesX;
            }
        }

        private static int KillCount;

        public static void IncrementKills()
        {
            KillCount++;
            Random r = new Random();
            int random = r.Next(5);
            if (KillCount%100 == 0)
            {
                switch (random)
                {
                    case 0:
                        Tools.Broadcast(string.Format("You call that a lot? {0} goblins killed!", KillCount));
                        break;
                    case 1:
                        Tools.Broadcast(string.Format("Fatality! {0} goblins killed!", KillCount));
                        break;
                    case 2:
                        Tools.Broadcast(string.Format("Number of 'noobs' killed to date: {0}", KillCount));
                        break;
                    case 3:
                        Tools.Broadcast(string.Format("Duke Nukem would be proud. {0} goblins killed.", KillCount));
                        break;
                    case 4:
                        Tools.Broadcast(string.Format("You call that a lot? {0} goblins killed!", KillCount));
                        break;
                    case 5:
                        Tools.Broadcast(string.Format("{0} copies of Call of Duty smashed.", KillCount));
                        break;
                }
            }
        }

        public static bool CheckSpawn(int x, int y)
        {
            Vector2 tile = new Vector2(x, y);
            Vector2 spawn = new Vector2(Main.spawnTileX, Main.spawnTileY);
            return Vector2.Distance(spawn, tile) <= Config.SpawnProtectionRadius;
        }

        public static bool HackedHealth(TSPlayer player)
        {
            return (player.TPlayer.statManaMax > 200) ||
                   (player.TPlayer.statMana > 200) ||
                   (player.TPlayer.statLifeMax > 400) ||
                   (player.TPlayer.statLife > 400);
        }

        public void OnConfigRead(ConfigFile file)
        {
            NPC.defaultMaxSpawns = file.DefaultMaximumSpawns;
            NPC.defaultSpawnRate = file.DefaultSpawnRate;

            Main.autoSave = file.AutoSave;
            if (Backups != null)
            {
                Backups.KeepFor = file.BackupKeepFor;
                Backups.Interval = file.BackupInterval;
            }
            if (!OverridePort)
            {
                Netplay.serverPort = file.ServerPort;
            }

            Netplay.spamCheck = file.SpamChecks;

            RconHandler.Password = file.RconPassword;
            RconHandler.ListenPort = file.RconPort;

            Type hash;
            if (Tools.HashTypes.TryGetValue(file.HashAlgorithm, out hash))
            {
                lock (Tools.HashAlgo)
                {
                    if (!Tools.HashAlgo.GetType().Equals(hash))
                    {
                        Tools.HashAlgo.Dispose();
                        Tools.HashAlgo = (HashAlgorithm) Activator.CreateInstance(Tools.HashTypes[file.HashAlgorithm]);
                    }
                }
            }
            else
            {
                Log.ConsoleError("Invalid or not supported hashing algorithm: " + file.HashAlgorithm);
            }
        }
    }
}