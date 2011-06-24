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
        public static readonly string VersionCodename = "Lol, packet changes.";

        public static readonly string SavePath = "tshock";

        public static TSPlayer[] Players = new TSPlayer[Main.maxPlayers];
        public static BanManager Bans = new BanManager(Path.Combine(SavePath, "bans.txt"));
        public static BackupManager Backups = new BackupManager(Path.Combine(SavePath, "backups"));

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
            FileTools.SetupConfig();

            string version = string.Format("TShock Version {0} ({1}) now running.", Version, VersionCodename);
            Console.WriteLine(version);

            Log.Initialize(Path.Combine(SavePath, "log.txt"), LogLevel.All, false);

            Log.Info(version);
            Log.Info("Starting...");

            GameHooks.PostInitialize += OnPostInit;
            GameHooks.Update += OnUpdate;
            ServerHooks.Join += OnJoin;
            ServerHooks.Leave += OnLeave;
            ServerHooks.Chat += OnChat;
            ServerHooks.Command += ServerHooks_OnCommand;
            NetHooks.GetData += GetData;
            NetHooks.GreetPlayer += OnGreetPlayer;
            NetHooks.SendData += OnSendData;
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



            Backups.KeepFor = ConfigurationManager.BackupKeepFor;
            Backups.Interval = ConfigurationManager.BackupInterval;

            HandleCommandLine(Environment.GetCommandLineArgs());
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
            if (e.IsTerminating)
            {
                if (Main.worldPathName != null)
                {
                    Main.worldPathName += ".crash";
                    WorldGen.saveWorld();
                }
                DeInitialize();
            }
            Log.Error(e.ExceptionObject.ToString());
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
            ConfigurationManager.ReadJsonConfiguration();
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

                    if (!player.Group.HasPermission("usebanneditem"))
                    {
                        for (int i = 0; i < Main.player[player.Index].inventory.Length; i++)
                        {
                            if (ItemManager.ItemIsBanned(Main.player[player.Index].inventory[i].name))
                            {
                                player.Disconnect("Using banned item: " + Main.player[player.Index].inventory[i].name + ", remove it and rejoin");
                                break;
                            }
                        }
                    }
                }
            }
            var id = Main.worldID;
            if (ConfigurationManager.Spawn_WorldID != Main.worldID)
            {
                Main.spawnTileX = ConfigurationManager.originalSpawnX;
                Main.spawnTileY = ConfigurationManager.originalSpawnY;
                ConfigurationManager.spawnTileX = Main.spawnTileX;
                ConfigurationManager.spawnTileY = Main.spawnTileY;
                ConfigurationManager.Spawn_WorldID = Main.worldID;
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

            if (msg.whoAmI != ply)
            {
                e.Handled = Tools.HandleGriefer(Players[ply], "Faking Chat");
                return;
            }

            var tsplr = Players[msg.whoAmI];

            if (tsplr.Group.HasPermission("adminchat") && !text.StartsWith("/"))
            {
                Tools.Broadcast(ConfigurationManager.AdminChatPrefix + "<" + tsplr.Name + "> " + text,
                                (byte)ConfigurationManager.AdminChatRGB[0], (byte)ConfigurationManager.AdminChatRGB[1], (byte)ConfigurationManager.AdminChatRGB[2]);
                e.Handled = true;
                return;
            }

            if (text.StartsWith("/"))
            {
                if (Commands.HandleCommand(tsplr, text))
                    e.Handled = true;
            }
            else
            {
                Log.Info(string.Format("{0} said: {1}", tsplr.Name, text));
            }
        }

        private void OnSendData(SendDataEventArgs e)
        {
            int remoteClient = 256;
            if (e.remoteClient >= 0)
                remoteClient = e.remoteClient;
            int num2 = 5;
            int num3 = num2;
            if (e.MsgID == PacketTypes.WorldInfo)
            {
                byte[] bytes18 = BitConverter.GetBytes((int)e.MsgID);
                byte[] bytes19 = BitConverter.GetBytes((int)Main.time);
                byte b6 = 0;
                if (Main.dayTime)
                {
                    b6 = 1;
                }
                byte b7 = (byte)Main.moonPhase;
                byte b8 = 0;
                if (Main.bloodMoon)
                {
                    b8 = 1;
                }
                byte[] bytes20 = BitConverter.GetBytes(Main.maxTilesX);
                byte[] bytes21 = BitConverter.GetBytes(Main.maxTilesY);
                byte[] bytes22 = BitConverter.GetBytes(ConfigurationManager.spawnTileX);
                byte[] bytes23 = BitConverter.GetBytes(ConfigurationManager.spawnTileY);
                byte[] bytes24 = BitConverter.GetBytes((int)Main.worldSurface);
                byte[] bytes25 = BitConverter.GetBytes((int)Main.rockLayer);
                byte[] bytes26 = BitConverter.GetBytes(Main.worldID);
                byte[] bytes27 = Encoding.ASCII.GetBytes(Main.worldName);
                byte b9 = 0;
                if (WorldGen.shadowOrbSmashed)
                {
                    b9 += 1;
                }
                if (NPC.downedBoss1)
                {
                    b9 += 2;
                }
                if (NPC.downedBoss2)
                {
                    b9 += 4;
                }
                if (NPC.downedBoss3)
                {
                    b9 += 8;
                }
                num2 += bytes19.Length + 1 + 1 + 1 + bytes20.Length + bytes21.Length + bytes22.Length + bytes23.Length + bytes24.Length + bytes25.Length + bytes26.Length + 1 + bytes27.Length;
                byte[] bytes28 = BitConverter.GetBytes(num2 - 4);
                Buffer.BlockCopy(bytes28, 0, NetMessage.buffer[remoteClient].writeBuffer, 0, 4);
                Buffer.BlockCopy(bytes18, 0, NetMessage.buffer[remoteClient].writeBuffer, 4, 1);
                Buffer.BlockCopy(bytes19, 0, NetMessage.buffer[remoteClient].writeBuffer, 5, bytes19.Length);
                num3 += bytes19.Length;
                NetMessage.buffer[remoteClient].writeBuffer[num3] = b6;
                num3++;
                NetMessage.buffer[remoteClient].writeBuffer[num3] = b7;
                num3++;
                NetMessage.buffer[remoteClient].writeBuffer[num3] = b8;
                num3++;
                Buffer.BlockCopy(bytes20, 0, NetMessage.buffer[remoteClient].writeBuffer, num3, bytes20.Length);
                num3 += bytes20.Length;
                Buffer.BlockCopy(bytes21, 0, NetMessage.buffer[remoteClient].writeBuffer, num3, bytes21.Length);
                num3 += bytes21.Length;
                Buffer.BlockCopy(bytes22, 0, NetMessage.buffer[remoteClient].writeBuffer, num3, bytes22.Length);
                num3 += bytes22.Length;
                Buffer.BlockCopy(bytes23, 0, NetMessage.buffer[remoteClient].writeBuffer, num3, bytes23.Length);
                num3 += bytes23.Length;
                Buffer.BlockCopy(bytes24, 0, NetMessage.buffer[remoteClient].writeBuffer, num3, bytes24.Length);
                num3 += bytes24.Length;
                Buffer.BlockCopy(bytes25, 0, NetMessage.buffer[remoteClient].writeBuffer, num3, bytes25.Length);
                num3 += bytes25.Length;
                Buffer.BlockCopy(bytes26, 0, NetMessage.buffer[remoteClient].writeBuffer, num3, bytes26.Length);
                num3 += bytes26.Length;
                NetMessage.buffer[remoteClient].writeBuffer[num3] = b9;
                num3++;
                Buffer.BlockCopy(bytes27, 0, NetMessage.buffer[remoteClient].writeBuffer, num3, bytes27.Length);
                num3 += bytes27.Length;
                e.Handled = true;
            }
            if (e.Handled)
            {
                if (Main.netMode != 1)
                {
                    goto IL_34D2;
                }
                if (Netplay.clientSock.tcpClient.Connected)
                {
                    try
                    {
                        NetMessage.buffer[remoteClient].spamCount++;
                        Netplay.clientSock.networkStream.BeginWrite(NetMessage.buffer[remoteClient].writeBuffer, 0, num2, new AsyncCallback(Netplay.clientSock.ClientWriteCallBack), Netplay.clientSock.networkStream);
                        goto IL_3612;
                    }
                    catch
                    {
                        goto IL_3612;
                    }
                    goto IL_34D2;
                }
            IL_3612:
                if (Main.verboseNetplay)
                {
                    for (int num10 = 0; num10 < num2; num10++)
                    {
                    }
                    for (int num11 = 0; num11 < num2; num11++)
                    {
                        byte arg_3649_0 = NetMessage.buffer[remoteClient].writeBuffer[num11];
                    }
                    goto IL_365B;
                }
                goto IL_365B;
            IL_34D2:
                if (e.remoteClient == -1)
                {
                    for (int num12 = 0; num12 < 256; num12++)
                    {
                        if (num12 != e.ignoreClient && (NetMessage.buffer[num12].broadcast || (Netplay.serverSock[num12].state >= 3 && (int)e.MsgID == 10)) && Netplay.serverSock[num12].tcpClient.Connected)
                        {
                            try
                            {
                                NetMessage.buffer[num12].spamCount++;
                                Netplay.serverSock[num12].networkStream.BeginWrite(NetMessage.buffer[remoteClient].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num12].ServerWriteCallBack), Netplay.serverSock[num12].networkStream);
                            }
                            catch
                            {
                            }
                        }
                    }
                    goto IL_3612;
                }
                if (Netplay.serverSock[remoteClient].tcpClient.Connected)
                {
                    try
                    {
                        NetMessage.buffer[remoteClient].spamCount++;
                        Netplay.serverSock[remoteClient].networkStream.BeginWrite(NetMessage.buffer[remoteClient].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[remoteClient].ServerWriteCallBack), Netplay.serverSock[remoteClient].networkStream);
                    }
                    catch
                    {
                    }
                    goto IL_3612;
                }
                goto IL_3612;
            IL_365B:
                NetMessage.buffer[remoteClient].writeLocked = false;
                if ((int)e.MsgID == 2 && Main.netMode == 2)
                {
                    Netplay.serverSock[remoteClient].kill = true;
                }
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
                    if (playerIP.IP == Players[who].IP)
                    {
                        Players[who].Teleport((int)playerIP.Pos.X, (int)playerIP.Pos.Y);
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
            var distance = Vector2.Distance(spawn, tile);
            if (distance > ConfigurationManager.SpawnProtectRadius)
                return false;
            else
                return true;
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