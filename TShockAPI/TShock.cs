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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;
using System.Text;

namespace TShockAPI
{
    [APIVersion(1, 5)]
    public class TShock : TerrariaPlugin
    {
        public static readonly Version VersionNum = Assembly.GetExecutingAssembly().GetName().Version;
        public static readonly string VersionCodename = "This is the part where we fix TShock";

        public static string SavePath = "tshock";

        public static TSPlayer[] Players = new TSPlayer[Main.maxPlayers];
        public static BanManager Bans;
        public static BackupManager Backups;

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
        }

        public override void Initialize()
        {
            HandleCommandLine(Environment.GetCommandLineArgs());

            Bans = new BanManager(FileTools.BansPath);
            Backups = new BackupManager(Path.Combine(SavePath, "backups"));

            FileTools.SetupConfig();

#if DEBUG
            Log.Initialize(Path.Combine(SavePath, "log.txt"), LogLevel.All, false);
#else
            Log.Initialize(Path.Combine(SavePath, "log.txt"), LogLevel.All & ~LogLevel.Debug, false);
#endif

            Log.ConsoleInfo(string.Format("TShock Version {0} ({1}) now running.", Version, VersionCodename));
            Log.Info("Starting...");

            GameHooks.PostInitialize += OnPostInit;
            GameHooks.Update += OnUpdate;
            ServerHooks.Join += OnJoin;
            ServerHooks.Leave += OnLeave;
            ServerHooks.Chat += OnChat;
            ServerHooks.Command += ServerHooks_OnCommand;
            NetHooks.GetData += GetData;
            NetHooks.GreetPlayer += OnGreetPlayer;
            NpcHooks.StrikeNpc += NpcHooks_OnStrikeNpc;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Log.Info("Hooks initialized");

            Bans.LoadBans();
            Log.Info("Bans initialized");

            GetDataHandlers.InitGetDataHandler();
            Log.Info("Get data handlers initialized");

            Commands.InitCommands();
            Log.Info("Commands initialized");

            RegionManager.ReadAllSettings();
            WarpsManager.ReadAllSettings();
            ItemManager.LoadBans();


            Main.autoSave = ConfigurationManager.AutoSave;
            Backups.KeepFor = ConfigurationManager.BackupKeepFor;
            Backups.Interval = ConfigurationManager.BackupInterval;

            Log.ConsoleInfo("AutoSave " + (ConfigurationManager.AutoSave ? "Enabled" : "Disabled"));
            Log.ConsoleInfo("Backups " + (Backups.Interval > 0 ? "Enabled" : "Disabled"));
        }

        public override void DeInitialize()
        {
            Bans.SaveBans();
            GameHooks.PostInitialize -= OnPostInit;
            GameHooks.Update -= OnUpdate;
            ServerHooks.Join -= OnJoin;
            ServerHooks.Leave -= OnLeave;
            ServerHooks.Chat -= OnChat;
            ServerHooks.Command -= ServerHooks_OnCommand;
            NetHooks.GetData -= GetData;
            NetHooks.GreetPlayer -= OnGreetPlayer;
            NpcHooks.StrikeNpc -= NpcHooks_OnStrikeNpc;
        }

        /// <summary>
        /// Handles exceptions that we didn't catch or that Red fucked up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject.ToString());
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
            }
        }

        /*
         * Hooks:
         * 
         */

        private void OnPostInit()
        {
            if (!File.Exists(Path.Combine(SavePath, "auth.lck")))
            {
                var r = new Random((int)DateTime.Now.ToBinary());
                ConfigurationManager.AuthToken = r.Next(100000, 10000000);
                Console.WriteLine("TShock Notice: To become SuperAdmin, join the game and type /auth " +
                                  ConfigurationManager.AuthToken);
                Console.WriteLine("This token will only display ONCE. This only works ONCE. If you don't use it and the server goes down, delete auth.lck.");
                FileTools.CreateFile(Path.Combine(SavePath, "auth.lck"));
            }
        }

        private void OnUpdate(GameTime time)
        {
            UpdateManager.UpdateProcedureCheck();

            if (Backups.IsBackupTime)
                Backups.Backup();

            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active)
                {
                    if (player.TilesDestroyed != null)
                    {
                        if (player.TileThreshold >= ConfigurationManager.TileThreshold)
                        {
                            if (Tools.HandleTntUser(player, "Kill tile abuse detected."))
                            {
                                TSPlayer.Server.RevertKillTile(player.TilesDestroyed);
                            }
                            else if (player.TileThreshold > 0)
                            {
                                player.TileThreshold = 0;
                                player.TilesDestroyed.Clear();
                            }

                        }
                        else if (player.TileThreshold > 0)
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
                            if (inv[i] != null && ItemManager.ItemIsBanned(inv[i].name))
                            {
                                player.Disconnect("Using banned item: " + inv[i].name + ", remove it and rejoin");
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void OnJoin(int ply, HandledEventArgs handler)
        {
            if (Main.netMode != 2 || handler.Handled)
                return;

            var player = new TSPlayer(ply);
            player.Group = Tools.GetGroupForIP(player.IP);

            if (Tools.ActivePlayers() + 1 > ConfigurationManager.MaxSlots && !player.Group.HasPermission("reservedslot"))
            {
                Tools.ForceKick(player, "Server is full");
                handler.Handled = true;
            }
            else
            {
                var ban = Bans.GetBanByIp(player.IP);
                if (ban != null)
                {
                    Tools.ForceKick(player, string.Format("You are banned: {0}", ban.Reason));
                    handler.Handled = true;
                }
                else if (!FileTools.OnWhitelist(player.IP))
                {
                    Tools.ForceKick(player, "Not on whitelist.");
                    handler.Handled = true;
                }
            }

            Players[ply] = player;
            Players[ply].InitSpawn = false;

            Netplay.spamCheck = ConfigurationManager.SpamChecks;
        }

        private void OnLeave(int ply)
        {
            if (Main.netMode != 2)
                return;

            var tsplr = Players[ply];
            if (tsplr != null && tsplr.ReceivedInfo)
                Log.Info(string.Format("{0} left.", tsplr.Name));

            if (ConfigurationManager.RememberLeavePos)
            {
                RemeberedPosManager.RemeberedPosistions.Add(new RemeberedPos(Players[ply].IP, new Vector2(Players[ply].X / 16, (Players[ply].Y / 16) + 3)));
                RemeberedPosManager.WriteSettings();
            }

            Players[ply] = null;
        }

        private void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
        {
            if (Main.netMode != 2 || e.Handled)
                return;

            var tsplr = Players[msg.whoAmI];

            if (msg.whoAmI != ply || tsplr == null)
            {
                e.Handled = Tools.HandleGriefer(Players[ply], "Faking Chat");
                return;
            }

            if (tsplr.Group.HasPermission("adminchat") && !text.StartsWith("/") && ConfigurationManager.AdminChatOptional)
            {
                Tools.Broadcast(ConfigurationManager.AdminChatPrefix + "<" + tsplr.Name + "> " + text,
                                (byte)ConfigurationManager.AdminChatRGB[0], (byte)ConfigurationManager.AdminChatRGB[1], (byte)ConfigurationManager.AdminChatRGB[2]);
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
            }
            else if (text.StartsWith("playing") || text.StartsWith("/playing"))
            {
                int count = 0;
                foreach (TSPlayer player in Players)
                {
                    if (player != null && player.Active)
                    {
                        count++;
                        TSPlayer.Server.SendMessage(string.Format("{0} ({1}) [{2}]", player.Name, player.IP, player.Group.Name));
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
                Main.autoSave = ConfigurationManager.AutoSave = !ConfigurationManager.AutoSave;
                Log.ConsoleInfo("AutoSave " + (ConfigurationManager.AutoSave ? "Enabled" : "Disabled"));
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
            if (Main.netMode != 2 || e.Handled)
                return;

            PacketTypes type = e.MsgID;
            TSPlayer player = Players[e.Msg.whoAmI];

            if (!player.ConnectionAlive)
            {
                e.Handled = true;
                return;
            }

            if (Main.verboseNetplay)
                Debug.WriteLine("{0:X} ({2}): {3} ({1:XX})", player.Index, (byte)type, player.TPlayer.dead ? "dead " : "alive", type.ToString());

            // Stop accepting updates from player as this player is going to be kicked/banned during OnUpdate (different thread so can produce race conditions)
            if ((ConfigurationManager.BanTnt || ConfigurationManager.KickTnt) && player.TileThreshold >= ConfigurationManager.TileThreshold && !player.Group.HasPermission("ignoregriefdetection"))
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
            if (Main.netMode != 2 || e.Handled)
                return;

            TSPlayer player = Players[who];
            Log.Info(string.Format("{0} ({1}) from '{2}' group joined.", player.Name, player.IP, player.Group.Name));

            Tools.ShowFileToUser(player, "motd.txt");
            if (HackedHealth(player))
            {
                Tools.HandleCheater(player, "Hacked health.");
            }
            if (ConfigurationManager.PermaPvp)
            {
                player.SetPvP(true);
            }
            if (Players[who].Group.HasPermission("causeevents") && ConfigurationManager.InfiniteInvasion)
            {
                StartInvasion();
            }
            if (ConfigurationManager.RememberLeavePos)
            {
                foreach (RemeberedPos playerIP in RemeberedPosManager.RemeberedPosistions)
                {
                    if (playerIP.IP == player.IP)
                    {
                        player.Teleport((int)playerIP.Pos.X, (int)playerIP.Pos.Y);
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
            if (ConfigurationManager.InfiniteInvasion)
            {
                IncrementKills();
                if (Main.invasionSize < 10)
                {
                    Main.invasionSize = 20000000;
                }
            }
        }

        /*
         * Useful stuff:
         * */

        public static void StartInvasion()
        {
            Main.invasionType = 1;
            if (ConfigurationManager.InfiniteInvasion)
            {
                Main.invasionSize = 20000000;
            }
            else
            {
                Main.invasionSize = 100 + (ConfigurationManager.InvasionMultiplier * Tools.ActivePlayers());
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

        public static void IncrementKills()
        {
            ConfigurationManager.KillCount++;
            Random r = new Random();
            int random = r.Next(5);
            if (ConfigurationManager.KillCount % 100 == 0)
            {
                switch (random)
                {
                    case 0:
                        Tools.Broadcast(string.Format("You call that a lot? {0} goblins killed!", ConfigurationManager.KillCount));
                        break;
                    case 1:
                        Tools.Broadcast(string.Format("Fatality! {0} goblins killed!", ConfigurationManager.KillCount));
                        break;
                    case 2:
                        Tools.Broadcast(string.Format("Number of 'noobs' killed to date: {0}", ConfigurationManager.KillCount));
                        break;
                    case 3:
                        Tools.Broadcast(string.Format("Duke Nukem would be proud. {0} goblins killed.", ConfigurationManager.KillCount));
                        break;
                    case 4:
                        Tools.Broadcast(string.Format("You call that a lot? {0} goblins killed!", ConfigurationManager.KillCount));
                        break;
                    case 5:
                        Tools.Broadcast(string.Format("{0} copies of Call of Duty smashed.", ConfigurationManager.KillCount));
                        break;
                }
            }
        }

        public static bool CheckSpawn(int x, int y)
        {
            Vector2 tile = new Vector2(x, y);
            Vector2 spawn = new Vector2(Main.spawnTileX, Main.spawnTileY);
            return Vector2.Distance(spawn, tile) <= ConfigurationManager.SpawnProtectRadius;
        }

        public static bool HackedHealth(TSPlayer player)
        {
            return (player.TPlayer.statManaMax > 200) ||
                    (player.TPlayer.statMana > 200) ||
                    (player.TPlayer.statLifeMax > 400) ||
                    (player.TPlayer.statLife > 400);
        }


        static readonly Dictionary<byte, string> MsgNames = new Dictionary<byte, string>()
        {
            {1, "Connect Request"},
            {2, "Disconnect"},
            {3, "Continue Connecting"},
            {4, "Player Info"},
            {5, "Player Slot"},
            {6, "Continue Connecting (2)"},
            {7, "World Info"},
            {8, "Tile Get Section"},
            {9, "Status"},
            {10, "Tile Send Section"},
            {11, "Tile Frame Section"},
            {12, "Player Spawn"},
            {13, "Player Update"},
            {14, "Player Active"},
            {15, "Sync Players"},
            {16, "Player HP"},
            {17, "Tile"},
            {18, "Time Set"},
            {19, "Door Use"},
            {20, "Tile Send Square"},
            {21, "Item Drop"},
            {22, "Item Owner"},
            {23, "Npc Update"},
            {24, "Npc Item Strike"},
            {25, "Chat Text"},
            {26, "Player Damage"},
            {27, "Projectile New"},
            {28, "Npc Strike"},
            {29, "Projectile Destroy"},
            {30, "Toggle PVP"},
            {31, "Chest Get Contents"},
            {32, "Chest Item"},
            {33, "Chest Open"},
            {34, "Tile Kill"},
            {35, "Effect Heal"},
            {36, "Zones"},
            {37, "Password Requied"},
            {38, "Password Send"},
            {39, "Item Unown"},
            {40, "Npc Talk"},
            {41, "Player Animation"},
            {42, "Player Mana"},
            {43, "Effect Mana"},
            {44, "Player Kill Me"},
            {45, "Player Team"},
            {46, "Sign Read"},
            {47, "Sign New"},
            {48, "Liquid Set"},
            {49, "Player Spawn Self"},
        };
    }
}