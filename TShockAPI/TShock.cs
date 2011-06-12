/*   
TShock, a server mod for Terraria
Copyright (C) <year>  <name of author>

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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StreamBinary;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;

namespace TShockAPI
{
    public class TShock : TerrariaPlugin
    {
        public static TSPlayer[] players = new TSPlayer[Main.maxPlayers];

        public static string saveDir = "./tshock/";

        public static Version VersionNum = new Version(2, 1, 0, 0);

        public static string VersionCodename = "Versioning fails";

        private static bool[] BlacklistTiles;

        public static BanManager Bans = new BanManager(Path.Combine(saveDir, "bans.txt"));

        delegate bool HandleGetDataD(MemoryStream data, GetDataEventArgs e);
        Dictionary<byte, HandleGetDataD> GetDataFuncs;

        public override Version Version
        {
            get { return VersionNum; }
        }

        public override Version APIVersion
        {
            get { return new Version(1, 2); }
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

        static TShock()
        {
            //Tools.LoadGroups();

            #region Blacklisted tiles

            BlacklistTiles = new bool[0x80];
            BlacklistTiles[0] = true;
            BlacklistTiles[1] = true;
            BlacklistTiles[2] = true;
            BlacklistTiles[6] = true;
            BlacklistTiles[7] = true;
            BlacklistTiles[8] = true;
            BlacklistTiles[9] = true;
            BlacklistTiles[22] = true;
            BlacklistTiles[23] = true;
            BlacklistTiles[25] = true;
            BlacklistTiles[30] = true;
            BlacklistTiles[37] = true;
            BlacklistTiles[38] = true;
            BlacklistTiles[39] = true;
            BlacklistTiles[40] = true;
            BlacklistTiles[41] = true;
            BlacklistTiles[43] = true;
            BlacklistTiles[44] = true;
            BlacklistTiles[45] = true;
            BlacklistTiles[46] = true;
            BlacklistTiles[47] = true;
            BlacklistTiles[53] = true;
            BlacklistTiles[54] = true;
            BlacklistTiles[56] = true;
            BlacklistTiles[57] = true;
            BlacklistTiles[58] = true;
            BlacklistTiles[59] = true;
            BlacklistTiles[60] = true;
            BlacklistTiles[63] = true;
            BlacklistTiles[64] = true;
            BlacklistTiles[65] = true;
            BlacklistTiles[66] = true;
            BlacklistTiles[67] = true;
            BlacklistTiles[68] = true;
            BlacklistTiles[70] = true;
            BlacklistTiles[75] = true;
            BlacklistTiles[76] = true;

            #endregion Blacklisted tiles
        }

        public TShock(Main game)
            : base(game)
        {

            GetDataFuncs = new Dictionary<byte, HandleGetDataD>
            {
                {0x4, HandlePlayerInfo},
                {0xA, HandleSendSection},
                {0xD, HandlePlayerUpdate},
                {0x11, HandleTile},
                {0x14, HandleSendTileSquare},
                {0x17, HandleNpcUpdate},
                {0x1A, HandlePlayerDamage},
                {0x1B, HandleProjectileNew},
                {0x1E, HandleTogglePvp},
                {0x22, HandleTileKill},
                {0x2C, HandlePlayerKillMe},
                {0x30, HandleLiquidSet},
            };
        }

        public override void Initialize()
        {
            try
            {
                FileTools.SetupConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            string version = string.Format("TShock Version {0} ({1}) now running.", Version, VersionCodename);
            Console.WriteLine(version);
            Log.Initialize(FileTools.SaveDir + "log.txt", LogLevel.All, false);
            Log.Info(version);
            Log.Info("Starting...");

            GameHooks.PostInitialize += OnPostInit;
            GameHooks.Update += OnUpdate;
            ServerHooks.Chat += OnChat;
            ServerHooks.Join += OnJoin;
            NetHooks.GetData += GetData;
            NetHooks.GreetPlayer += OnGreetPlayer;
            NpcHooks.StrikeNpc += NpcHooks_OnStrikeNpc;
            ServerHooks.Command += ServerHooks_OnCommand;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Bans.LoadBans();

            Log.Info("Hooks initialized");
            Commands.InitCommands();
            Log.Info("Commands initialized");
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

        /// <summary>
        /// When a server command is run.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="e"></param>
        private void ServerHooks_OnCommand(string cmd, HandledEventArgs e)
        {
        }

        public override void DeInitialize()
        {
            Bans.SaveBans();

            GameHooks.PostInitialize -= OnPostInit;
            GameHooks.Update -= OnUpdate;
            ServerHooks.Chat -= OnChat;
            ServerHooks.Join -= OnJoin;
            ServerHooks.Command -= ServerHooks_OnCommand;
            NetHooks.GetData -= GetData;
            NetHooks.GreetPlayer -= OnGreetPlayer;
            NpcHooks.StrikeNpc -= NpcHooks_OnStrikeNpc;
        }

        /*
         * Hooks:
         * */

        private void NpcHooks_OnStrikeNpc(NpcStrikeEventArgs e)
        {
            if (ConfigurationManager.infiniteInvasion)
            {
                IncrementKills();
                if (Main.invasionSize < 10)
                {
                    Main.invasionSize = 20000000;
                }
            }
        }

        private void GetData(GetDataEventArgs e)
        {
            if (!Netplay.serverSock[e.Msg.whoAmI].active || Netplay.serverSock[e.Msg.whoAmI].kill)
                return;

            if (Main.verboseNetplay)
                Debug.WriteLine("{0:X} ({2}): {3} ({1:XX})", e.Msg.whoAmI, e.MsgID, Main.player[e.Msg.whoAmI].dead ? "dead " : "alive", MsgNames[e.MsgID]);

            HandleGetDataD func;
            if (GetDataFuncs.TryGetValue(e.MsgID, out func))
            {
                using (var data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
                {
                    e.Handled = func(data, e);
                }
            }
        }

        bool HandlePlayerInfo(MemoryStream data, GetDataEventArgs e)
        {
            var ban = Bans.GetBanByName(Main.player[e.Msg.whoAmI].name);
            if (ban != null)
            {
                Tools.ForceKick(e.Msg.whoAmI, string.Format("You are banned: {0}", ban.Reason));
                return true;
            }
            string name = Encoding.ASCII.GetString(e.Msg.readBuffer, e.Index + 23, (e.Length - (e.Index + 23)) + e.Index - 1);
            if (name.Length > 32)
            {
                Tools.ForceKick(e.Msg.whoAmI, "Name exceeded 32 characters.");
                return true;
            }
            if (name.Trim().Length == 0)
            {
                Tools.ForceKick(e.Msg.whoAmI, "Empty Name.");
                return true;
            }
            if (players[e.Msg.whoAmI] == null)
            {
                Tools.ForceKick(e.Msg.whoAmI, "Player doesn't exist");
                return true;
            }
            if (players[e.Msg.whoAmI].receivedInfo)
            {
                return Tools.HandleGriefer(e.Msg.whoAmI, "Sent client info more than once");
            }

            players[e.Msg.whoAmI].receivedInfo = true;
            return false;
        }

        bool HandleSendTileSquare(MemoryStream data, GetDataEventArgs e)
        {
            short size = data.ReadInt16();
            int x = data.ReadInt32();
            int y = data.ReadInt32();
            int plyX = Math.Abs((int)Main.player[e.Msg.whoAmI].position.X / 16);
            int plyY = Math.Abs((int)Main.player[e.Msg.whoAmI].position.Y / 16);
            int tileX = Math.Abs(x);
            int tileY = Math.Abs(y);
            if (size > 5 || (ConfigurationManager.rangeChecks && (Math.Abs(plyX - tileX) > 32 || Math.Abs(plyY - tileY) > 32)))
            {
                Log.Debug(string.Format("SendTileSquare(PlyXY:{0}_{1}, TileXY:{2}_{3}, Result:{4}_{5}, Size:{6})",
                        plyX, plyY,
                        tileX, tileY,
                        Math.Abs(plyX - tileX), Math.Abs(plyY - tileY),
                        size
                    ));
                return Tools.HandleGriefer(e.Msg.whoAmI, "Send Tile Square Abuse");
            }
            return false;
        }
        bool HandleTile(MemoryStream data, GetDataEventArgs e)
        {
            byte type = data.ReadInt8();
            int x = data.ReadInt32();
            int y = data.ReadInt32();
            byte tiletype = data.ReadInt8();
            if (type == 1 || type == 3)
            {
                int plyX = Math.Abs((int)Main.player[e.Msg.whoAmI].position.X / 16);
                int plyY = Math.Abs((int)Main.player[e.Msg.whoAmI].position.Y / 16);
                int tileX = Math.Abs(x);
                int tileY = Math.Abs(y);

                if (Main.player[e.Msg.whoAmI].selectedItem == 0x72) //Dirt Rod
                {
                    return Tools.Kick(e.Msg.whoAmI, "Using dirt rod");
                }

                if (ConfigurationManager.rangeChecks && ((Math.Abs(plyX - tileX) > 32) || (Math.Abs(plyY - tileY) > 32)))
                {
                    Log.Debug(string.Format("TilePlaced(PlyXY:{0}_{1}, TileXY:{2}_{3}, Result:{4}_{5}, Type:{6})",
                        plyX, plyY,
                        tileX, tileY,
                        Math.Abs(plyX - tileX), Math.Abs(plyY - tileY),
                        tiletype
                    ));
                    return Tools.HandleGriefer(e.Msg.whoAmI, "Placing impossible to place blocks.");
                }
            }
            if (type == 0 || type == 1)
            {
                if (ConfigurationManager.spawnProtect)
                {
                    if (!players[e.Msg.whoAmI].group.HasPermission("editspawn"))
                    {
                        var flag = CheckSpawn(x, y);
                        if (flag)
                        {
                            Tools.SendMessage(e.Msg.whoAmI, "Spawn protected from changes.", Color.Red);
                            return true;
                        }
                    }
                }
            }

            if (type == 0 && BlacklistTiles[Main.tile[x, y].type] && Main.player[e.Msg.whoAmI].active)
            {
                players[e.Msg.whoAmI].tileThreshold++;
                players[e.Msg.whoAmI].tilesDestroyed.Add(new Position(x, y), Main.tile[x, y]);
            }

            return false;
        }

        bool HandleTogglePvp(MemoryStream data, GetDataEventArgs e)
        {
            int id = data.ReadByte();
            bool pvp = data.ReadBoolean();

            Main.player[e.Msg.whoAmI].hostile = pvp;
            if (id != e.Msg.whoAmI)
                Main.player[e.Msg.whoAmI].hostile = true;
            if (ConfigurationManager.permaPvp)
                Main.player[e.Msg.whoAmI].hostile = true;
            NetMessage.SendData(30, -1, -1, "", e.Msg.whoAmI);
            return true;
        }

        bool HandleSendSection(MemoryStream data, GetDataEventArgs e)
        {
            return Tools.HandleGriefer(e.Msg.whoAmI, "SendSection abuse.");
        }

        bool HandleNpcUpdate(MemoryStream data, GetDataEventArgs e)
        {
            return Tools.HandleGriefer(e.Msg.whoAmI, "Spawn NPC abuse");
        }

        bool HandlePlayerUpdate(MemoryStream data, GetDataEventArgs e)
        {
            byte plr = data.ReadInt8();
            byte control = data.ReadInt8();
            byte item = data.ReadInt8();
            float posx = data.ReadSingle();
            float posy = data.ReadSingle();
            float velx = data.ReadSingle();
            float vely = data.ReadSingle();

            if (Main.verboseNetplay)
                Debug.WriteLine("Update: {{{0},{1}}} {{{2}, {3}}}", (int)posx, (int)posy, (int)velx, (int)vely);

            if (plr != e.Msg.whoAmI)
            {
                return Tools.HandleGriefer(e.Msg.whoAmI, "Update Player abuse");
            }
            return false;
        }

        bool HandleProjectileNew(MemoryStream data, GetDataEventArgs e)
        {
            short ident = data.ReadInt16();
            float posx = data.ReadSingle();
            float posy = data.ReadSingle();
            float velx = data.ReadSingle();
            float vely = data.ReadSingle();
            float knockback = data.ReadSingle();
            short dmg = data.ReadInt16();
            byte owner = data.ReadInt8();
            byte type = data.ReadInt8();

            if (type == 29 || type == 28 || type == 37)
            {
                var plr = Main.player[e.Msg.whoAmI];
                Log.Debug(string.Format("Explosive(PlyXY:{0}_{1}, Type:{2})",
                        (int)(plr.position.X / 16),
                        (int)(plr.position.Y / 16),
                        type
                    ));
                return Tools.HandleExplosivesUser(e.Msg.whoAmI, "Throwing an explosive device.");
            }
            return false;
        }

        bool HandlePlayerKillMe(MemoryStream data, GetDataEventArgs e)
        {
            byte id = data.ReadInt8();
            byte hitdirection = data.ReadInt8();
            short dmg = data.ReadInt16();
            bool pvp = data.ReadBoolean();

            if (id != e.Msg.whoAmI)
            {
                return Tools.HandleGriefer(e.Msg.whoAmI, "Trying to execute KillMe on someone else.");
            }
            return false;
        }

        bool HandlePlayerDamage(MemoryStream data, GetDataEventArgs e)
        {
            byte playerid = data.ReadInt8();
            byte direction = data.ReadInt8();
            Int16 damage = data.ReadInt16();
            byte pvp = data.ReadInt8();

            return !Main.player[playerid].hostile;
        }

        bool HandleLiquidSet(MemoryStream data, GetDataEventArgs e)
        {
            int x = data.ReadInt32();
            int y = data.ReadInt32();
            byte liquid = data.ReadInt8();
            bool lava = data.ReadBoolean();

            //The liquid was picked up.
            if (liquid == 0)
                return false;

            int plyX = Math.Abs((int)Main.player[e.Msg.whoAmI].position.X / 16);
            int plyY = Math.Abs((int)Main.player[e.Msg.whoAmI].position.Y / 16);
            int tileX = Math.Abs(x);
            int tileY = Math.Abs(y);

            bool bucket = false;
            for (int i = 0; i < 44; i++)
            {
                if (Main.player[e.Msg.whoAmI].inventory[i].type >= 205 && Main.player[e.Msg.whoAmI].inventory[i].type <= 207)
                {
                    bucket = true;
                    break;
                }
            }

            if (lava && !players[e.Msg.whoAmI].group.HasPermission("canlava"))
            {
                Tools.SendMessage(e.Msg.whoAmI, "You do not have permission to use lava", Color.Red);
                Tools.SendLogs(string.Format("{0} tried using lava", Main.player[e.Msg.whoAmI].name), Color.Red);

                return true;
            }
            if (!lava && !players[e.Msg.whoAmI].group.HasPermission("canwater"))
            {
                Tools.SendMessage(e.Msg.whoAmI, "You do not have permission to use water", Color.Red);
                Tools.SendLogs(string.Format("{0} tried using water", Main.player[e.Msg.whoAmI].name), Color.Red);
                return true;
            }

            if (!bucket)
            {
                Log.Debug(string.Format("{0}(PlyXY:{1}_{2}, TileXY:{3}_{4}, Result:{5}_{6}, Amount:{7})",
                    lava ? "Lava" : "Water",
                        plyX, plyY,
                        tileX, tileY,
                        Math.Abs(plyX - tileX), Math.Abs(plyY - tileY),
                           liquid
                    ));
                return Tools.HandleGriefer(e.Msg.whoAmI, "Manipulating liquid without bucket."); ;
            }
            if (ConfigurationManager.rangeChecks && ((Math.Abs(plyX - tileX) > 32) || (Math.Abs(plyY - tileY) > 32)))
            {
                Log.Debug(string.Format("Liquid(PlyXY:{0}_{1}, TileXY:{2}_{3}, Result:{4}_{5}, Amount:{6})",
                           plyX, plyY,
                           tileX, tileY,
                           Math.Abs(plyX - tileX), Math.Abs(plyY - tileY),
                           liquid
                       ));
                return Tools.HandleGriefer(e.Msg.whoAmI, "Placing impossible to place liquid."); ;
            }

            if (ConfigurationManager.spawnProtect)
            {
                if (!players[e.Msg.whoAmI].group.HasPermission("editspawn"))
                {
                    var flag = CheckSpawn(x, y);
                    if (flag)
                    {
                        Tools.SendMessage(e.Msg.whoAmI, "The spawn is protected!", Color.Red);
                        return true;
                    }
                }
            }
            return false;
        }

        bool HandleTileKill(MemoryStream data, GetDataEventArgs e)
        {
            int tilex = data.ReadInt32();
            int tiley = data.ReadInt32();
            if (tilex < 0 || tilex >= Main.maxTilesX || tiley < 0 || tiley >= Main.maxTilesY)
                return false;

            if (Main.tile[tilex, tiley].type != 0x15) //Chest
            {
                Log.Debug(string.Format("TileKill(TileXY:{0}_{1}, Type:{2})",
                        tilex, tiley,
                        Main.tile[tilex, tiley].type
                    ));
                Tools.ForceKick(e.Msg.whoAmI, string.Format("Tile Kill abuse ({0})", Main.tile[tilex, tiley].type));
                return true;
            }
            return false;
        }

        private void OnGreetPlayer(int who, HandledEventArgs e)
        {
            if (Main.netMode != 2)
                return;

            Log.Info(string.Format("{0} ({1}) from '{2}' group joined.", Tools.FindPlayer(who), Tools.GetPlayerIP(who), players[who].group.GetName()));

            Tools.ShowMOTD(who);
            if (HackedHealth(who))
            {
                Tools.HandleCheater(who, "Hacked health.");
            }
            if (ConfigurationManager.permaPvp)
            {
                Main.player[who].hostile = true;
                NetMessage.SendData(30, -1, -1, "", who);
            }
            if (players[who].group.HasPermission("causeevents") && ConfigurationManager.infiniteInvasion)
            {
                StartInvasion();
            }
            e.Handled = true;
        }

        private void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
        {
            if (Main.netMode != 2)
                return;

            if (msg.whoAmI != ply)
            {
                e.Handled = Tools.HandleGriefer(ply, "Faking Chat");
                return;
            }

            int x = (int)Main.player[ply].position.X;
            int y = (int)Main.player[ply].position.Y;

            if (text.StartsWith("/"))
            {
                text = text.Remove(0, 1);

                var args = Commands.ParseParameters(text);
                if (args.Count < 1)
                    return;

                string scmd = args[0];
                args.RemoveAt(0);

                Commands.Command cmd = null;
                for (int i = 0; i < Commands.commands.Count; i++)
                {
                    if (Commands.commands[i].Name().Equals(scmd))
                    {
                        cmd = Commands.commands[i];
                    }
                }

                if (cmd == null)
                {
                    Tools.SendMessage(ply, "That command does not exist, try /help", Color.Red);
                }
                else
                {
                    if (!cmd.CanRun(players[ply]))
                    {
                        Tools.SendLogs(string.Format("{0} tried to execute {1}", Tools.FindPlayer(ply), cmd.Name()), Color.Red);
                        Tools.SendMessage(ply, "You do not have access to that command.", Color.Red);
                    }
                    else
                    {
                        Tools.SendLogs(string.Format("{0} executed: /{1}", Tools.FindPlayer(ply), text), Color.Red);
                        cmd.Run(text, players[ply], args);
                    }
                }
                e.Handled = true;
            }
        }

        private void OnJoin(int ply, HandledEventArgs handler)
        {
            if (Main.netMode != 2)
            {
                return;
            }

            string ip = Tools.GetPlayerIP(ply);
            players[ply] = new TSPlayer(ply);
            players[ply].group = Tools.GetGroupForIP(ip);

            if (Tools.activePlayers() + 1 > ConfigurationManager.maxSlots &&
                !players[ply].group.HasPermission("reservedslot"))
            {
                Tools.ForceKick(ply, "Server is full");
                handler.Handled = true;
            }
            else
            {
                var ban = Bans.GetBanByIp(ip);
                if (ban != null)
                {
                    Tools.ForceKick(ply, string.Format("You are banned: {0}", ban.Reason));
                    handler.Handled = true;
                }
                else if (!FileTools.OnWhitelist(ip))
                {
                    Tools.ForceKick(ply, "Not on whitelist.");
                    handler.Handled = true;
                }
            }

            Netplay.serverSock[ply].spamCheck = ConfigurationManager.spamChecks;
        }

        private void OnPostInit()
        {
            if (!File.Exists(FileTools.SaveDir + "auth.lck"))
            {
                Random r = new Random((int)DateTime.Now.ToBinary());
                ConfigurationManager.authToken = r.Next(100000, 10000000);
                Console.WriteLine("TShock Notice: To become SuperAdmin, join the game and type /auth " +
                                  ConfigurationManager.authToken);
                Console.WriteLine("This token will only display ONCE. This only works ONCE. If you don't use it and the server goes down, delete auth.lck.");
                FileTools.CreateFile(FileTools.SaveDir + "auth.lck");
            }
        }

        private void OnUpdate(GameTime time)
        {
            UpdateManager.UpdateProcedureCheck();
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active == false)
                    continue;
                if (players[i].tileThreshold >= 20)
                {
                    if (Tools.HandleTntUser(i, "Kill tile abuse detected."))
                    {
                        RevertKillTile(i);
                        players[i].tileThreshold = 0;
                        players[i].tilesDestroyed.Clear();
                    }
                    else if (players[i].tileThreshold > 0)
                    {
                        players[i].tileThreshold = 0;
                        players[i].tilesDestroyed.Clear();
                    }

                }
                else if (players[i].tileThreshold > 0)
                {
                    players[i].tileThreshold = 0;
                }
            }
        }

        /*
         * Useful stuff:
         * */

        public static void Teleport(int ply, int x, int y)
        {
            Main.player[ply].position.X = x;
            Main.player[ply].position.Y = y;
            NetMessage.SendData(0x0d, -1, ply, "", ply);
            NetMessage.SendData(0x0d, -1, -1, "", ply);
            NetMessage.syncPlayers();
        }

        public static void Teleport(int ply, float x, float y)
        {
            Main.player[ply].position.X = x;
            Main.player[ply].position.Y = y;
            NetMessage.SendData(0x0d, -1, ply, "", ply);
            NetMessage.SendData(0x0d, -1, -1, "", ply);
            NetMessage.syncPlayers();
        }

        public static void StartInvasion()
        {
            Main.invasionType = 1;
            if (ConfigurationManager.infiniteInvasion)
            {
                Main.invasionSize = 20000000;
            }
            else
            {
                Main.invasionSize = 100 + (ConfigurationManager.invasionMultiplier * Tools.activePlayers());
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
            ConfigurationManager.killCount++;
            Random r = new Random();
            int random = r.Next(5);
            if (ConfigurationManager.killCount % 100 == 0)
            {
                switch (random)
                {
                    case 0:
                        Tools.Broadcast(string.Format("You call that a lot? {0} goblins killed!", ConfigurationManager.killCount));
                        break;
                    case 1:
                        Tools.Broadcast(string.Format("Fatality! {0} goblins killed!", ConfigurationManager.killCount));
                        break;
                    case 2:
                        Tools.Broadcast(string.Format("Number of 'noobs' killed to date: {0}", ConfigurationManager.killCount));
                        break;
                    case 3:
                        Tools.Broadcast(string.Format("Duke Nukem would be proud. {0} goblins killed.", ConfigurationManager.killCount));
                        break;
                    case 4:
                        Tools.Broadcast(string.Format("You call that a lot? {0} goblins killed!", ConfigurationManager.killCount));
                        break;
                    case 5:
                        Tools.Broadcast(string.Format("{0} copies of Call of Duty smashed.", ConfigurationManager.killCount));
                        break;
                }
            }
        }

        public static void UpdateInventories()
        {
            for (int i = 0; i < Main.player.Length; i++)
            {
                for (int j = 0; j < 44; j++)
                {
                    for (int h = 0; h < Main.player.Length; h++)
                        NetMessage.SendData(5, h, i, Main.player[i].inventory[j].name, i, j, 0f, 0f);
                }
            }
        }

        public static void UpdatePlayers()
        {
            for (int i = 0; i < Main.player.Length; i++)
            {
                for (int h = 0; h < Main.player.Length; h++)
                    NetMessage.SendData(0x0d, i, -1, "", h);
            }
        }

        public static void KillMe(int plr)
        {
            for (int i = 0; i < Main.player.Length; i++)
                NetMessage.SendData(44, i, -1, "", plr, 1, 9999999, (float)0);
        }

        //TODO : Notify the player if there is more than one match. (or do we want a First() kinda thing?)
        public static int GetNPCID(string name, bool exact = false)
        {
            NPC npc = new NPC();
            for (int i = 1; i < Main.maxNPCTypes; i++)
            {
                if (exact)
                {
                    //Method #1 - must be exact match, allows support for different coloured slimes
                    npc.SetDefaults(name);
                    if (npc.name == name)
                        return i;
                }
                else
                {
                    //Method #2 - allows impartial matching
                    name = name.ToLower();
                    npc.SetDefaults(i);
                    if (npc.name.ToLower().StartsWith(name))
                        return i;
                }
            }
            return -1;
        }

        public static int GetItemID(string name)
        {
            Item item = new Item();
            name = name.ToLower();
            for (int i = 1; i < Main.maxItemTypes; i++)
            {
                item.SetDefaults(i);
                if (item.name.ToLower().StartsWith(name))
                    return i;
            }
            return -1;
        }

        public static bool CheckSpawn(int x, int y)
        {
            Vector2 tile = new Vector2(x, y);
            Vector2 spawn = new Vector2(Main.spawnTileX, Main.spawnTileY);
            var distance = Vector2.Distance(spawn, tile);
            if (distance > ConfigurationManager.spawnProtectRadius)
                return false;
            else
                return true;
        }

        public class Position
        {
            public float X;
            public float Y;

            public Position(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

        public static void RevertKillTile(int ply)
        {
            Tile[] tiles = new Tile[players[ply].tilesDestroyed.Count];
            players[ply].tilesDestroyed.Values.CopyTo(tiles, 0);
            Position[] positions = new Position[players[ply].tilesDestroyed.Count];
            players[ply].tilesDestroyed.Keys.CopyTo(positions, 0);
            for (int i = (players[ply].tilesDestroyed.Count - 1); i >= 0; i--)
            {
                Main.tile[(int)positions[i].X, (int)positions[i].Y] = tiles[i];
                NetMessage.SendData(17, -1, -1, "", 1, positions[i].X, positions[i].Y, (float)0);
            }
        }

        public static bool HackedHealth(int ply)
        {
            return (Main.player[ply].statManaMax > 200) ||
                    (Main.player[ply].statMana > 200) ||
                    (Main.player[ply].statLifeMax > 400) ||
                    (Main.player[ply].statLife > 400);
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