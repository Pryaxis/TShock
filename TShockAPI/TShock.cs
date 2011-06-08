using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;

namespace TShockAPI
{
    public class TShock : TerrariaPlugin
    {
        public static TSPlayer[] players = new TSPlayer[Main.maxPlayers];

        public static string saveDir = "./tshock/";

        public static Version VersionNum = new Version(2, 0, 0, 1);

        public static string VersionCodename = "UnrealIRCd ftw (irc.shankshock.com #terraria)";

        public static bool shownVersion;

        private static bool[] BlacklistTiles;

        public static bool updateCmd;

        public static BanManager Bans = new BanManager(Path.Combine(saveDir, "bans.txt"));

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
            Console.WriteLine("TShock Version " + Version.Major + "." + Version.Minor + "." + Version.Build + "." +
                              Version.Revision + " (" + VersionCodename + ") now running.");
            Log.Initialize(FileTools.SaveDir + "log.txt", LogLevel.All, true);
            Log.Info("Starting...");
            GameHooks.Initialize += OnPreInit;
            GameHooks.PostInitialize += OnPostInit;
            GameHooks.Update += OnUpdate;
            GameHooks.LoadContent += OnLoadContent;
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

            GameHooks.Initialize -= OnPreInit;
            GameHooks.PostInitialize -= OnPostInit;
            GameHooks.Update -= OnUpdate;
            GameHooks.LoadContent -= OnLoadContent;
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
            e.Handled = e.Handled || HandleGetData(e);
        }

        private bool HandleGetData(GetDataEventArgs e)
        {
            if (!Netplay.serverSock[e.Msg.whoAmI].active || Netplay.serverSock[e.Msg.whoAmI].kill)
                return true;

            if (Main.verboseNetplay)
                Debug.WriteLine("{0:X} ({2}): {3} ({1:XX})", e.Msg.whoAmI, e.MsgID, Main.player[e.Msg.whoAmI].dead ? "dead " : "alive", MsgNames[e.MsgID]);

            if (e.MsgID == 4)
            {
                var ban = Bans.GetBanByName(Main.player[e.Msg.whoAmI].name);
                if (ban != null)
                {
                    Tools.ForceKick(e.Msg.whoAmI, "You are banned: " + ban.Reason);
                    return true;
                }
                string name = Encoding.ASCII.GetString(e.Msg.readBuffer, e.Index + 23, (e.Length - (e.Index + 23)) + e.Index - 1);
                if (name.Length > 32)
                {
                    Tools.ForceKick(e.Msg.whoAmI, "Name exceeded 32 characters.");
                    return true;
                }
                if (players[e.Msg.whoAmI] == null)
                {
                    Tools.ForceKick(e.Msg.whoAmI, "Player doesn't exist");
                    return true;
                }
                else if (players[e.Msg.whoAmI].receivedInfo)
                {
                    return Tools.HandleGriefer(e.Msg.whoAmI, "Sent client info more than once");
                }
                else
                {
                    players[e.Msg.whoAmI].receivedInfo = true;
                }
            }
            else if (e.MsgID == 0x14)
            {
                using (var br = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                {
                    short size = br.ReadInt16();
                    int x = br.ReadInt32();
                    int y = br.ReadInt32();
                    int plyX = Math.Abs((int)Main.player[e.Msg.whoAmI].position.X / 16);
                    int plyY = Math.Abs((int)Main.player[e.Msg.whoAmI].position.Y / 16);
                    int tileX = Math.Abs(x);
                    int tileY = Math.Abs(y);
                    if (size > 5 || Math.Abs(plyX - tileX) > 12 || Math.Abs(plyY - tileY) > 12)
                    {
                        return Tools.HandleGriefer(e.Msg.whoAmI, "Send Tile Square Abuse");
                    }
                }
            }
            else if (e.MsgID == 17)
            {
                using (var br = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                {
                    byte type = br.ReadByte();
                    int x = br.ReadInt32();
                    int y = br.ReadInt32();
                    byte typetile = br.ReadByte();
                    if (type == 1 || type == 3)
                    {
                        int plyX = Math.Abs((int)Main.player[e.Msg.whoAmI].position.X / 16);
                        int plyY = Math.Abs((int)Main.player[e.Msg.whoAmI].position.Y / 16);
                        int tileX = Math.Abs(x);
                        int tileY = Math.Abs(y);

                        if ((Math.Abs(plyX - tileX) > 8) || (Math.Abs(plyY - tileY) > 8))
                        {
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
                                    Tools.SendMessage(e.Msg.whoAmI, "Spawn protected from changes.",
                                                      new[] { 255f, 0f, 0f });
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
                }
            }
            else if (e.MsgID == 0x1e)
            {
                using (var br = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                {
                    int id = br.ReadByte();
                    bool pvp = br.ReadBoolean();

                    Main.player[e.Msg.whoAmI].hostile = pvp;
                    if (id != e.Msg.whoAmI)
                        Main.player[e.Msg.whoAmI].hostile = true;
                    if (ConfigurationManager.permaPvp)
                        Main.player[e.Msg.whoAmI].hostile = true;
                    NetMessage.SendData(30, -1, -1, "", e.Msg.whoAmI);
                    return true;
                }
            }
            else if (e.MsgID == 0x0A) //SendSection
            {
                return Tools.HandleGriefer(e.Msg.whoAmI, "SendSection abuse.");
            }
            else if (e.MsgID == 0x17) //Npc Data
            {
                return Tools.HandleGriefer(e.Msg.whoAmI, "Spawn NPC abuse");
            }
            else if (e.MsgID == 0x0D) //Update Player
            {
                byte plr = e.Msg.readBuffer[e.Index];
                if (plr != e.Msg.whoAmI)
                {
                    return Tools.HandleGriefer(e.Msg.whoAmI, "Update Player abuse");
                }
            }
            else if (e.MsgID == 0x1B) // New Projectile
            {
                using (var br = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                {
                    short ident = br.ReadInt16();
                    float posx = br.ReadSingle();
                    float posy = br.ReadSingle();
                    float velx = br.ReadSingle();
                    float vely = br.ReadSingle();
                    float knockback = br.ReadSingle();
                    short dmg = br.ReadInt16();
                    byte owner = br.ReadByte();
                    byte type = br.ReadByte();

                    if (type == 29 || type == 28 || type == 37)
                    {
                        return Tools.HandleExplosivesUser(e.Msg.whoAmI, "Throwing an explosive device.");
                    }
                }
            }
            else if (e.MsgID == 0x2C) // KillMe
            {
                using (var br = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                {
                    byte id = br.ReadByte();
                    byte hitdirection = br.ReadByte();
                    short dmg = br.ReadInt16();
                    bool pvp = br.ReadBoolean();

                    if (id != e.Msg.whoAmI)
                    {
                        return Tools.HandleGriefer(e.Msg.whoAmI, "Trying to execute KillMe on someone else.");
                    }
                }
            }
            else if (e.MsgID == 0x1a) //PlayerDamage
            {
                using (var br = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                {
                    byte playerid = br.ReadByte();
                    byte direction = br.ReadByte();
                    Int16 damage = br.ReadInt16();
                    byte pvp = br.ReadByte();

                    if (!Main.player[playerid].hostile)
                        return true;
                }
            }
            else if (e.MsgID == 0x30)
            {
                using (var br = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                {
                    int x = br.ReadInt32();
                    int y = br.ReadInt32();
                    byte liquid = br.ReadByte();
                    bool lava = br.ReadBoolean();

                    //The liquid was picked up.
                    if (liquid == 0)
                        return false;

                    if (Main.player[e.Msg.whoAmI].selectedItem == 0x72) //Dirt Rod
                    {
                        return Tools.Kick(e.Msg.whoAmI, "Using dirt rod");
                    }

                    int plyX = Math.Abs((int)Main.player[e.Msg.whoAmI].position.X / 16);
                    int plyY = Math.Abs((int)Main.player[e.Msg.whoAmI].position.Y / 16);
                    int tileX = Math.Abs(x);
                    int tileY = Math.Abs(y);

                    int lavacount = 0;
                    int watercount = 0;

                    for (int i = 0; i < 44; i++)
                    {
                        if (Main.player[e.Msg.whoAmI].inventory[i].name == "Lava Bucket")
                            lavacount++;
                        else if (Main.player[e.Msg.whoAmI].inventory[i].name == "Water Bucket")
                            watercount++;
                    }

                    if (lava && lavacount <= 0)
                    {
                        return Tools.HandleGriefer(e.Msg.whoAmI, "Placing lava they didn't have."); ;
                    }
                    else if (!lava && watercount <= 0)
                    {
                        return Tools.HandleGriefer(e.Msg.whoAmI, "Placing water they didn't have.");
                    }
                    if ((Math.Abs(plyX - tileX) > 6) || (Math.Abs(plyY - tileY) > 6))
                    {
                        return Tools.HandleGriefer(e.Msg.whoAmI, "Placing impossible to place liquid."); ;
                    }

                    if (ConfigurationManager.spawnProtect)
                    {
                        if (!players[e.Msg.whoAmI].group.HasPermission("editspawn"))
                        {
                            var flag = CheckSpawn(x, y);
                            if (flag)
                            {
                                Tools.SendMessage(e.Msg.whoAmI, "The spawn is protected!", new[] { 255f, 0f, 0f });
                                return true;
                            }
                        }
                    }
                }
            }
            else if (e.MsgID == 0x22) // Client only KillTile
            {
                return true;  // Client only uses it for chests, but sends regular 17 as well.
            }

            return false;
        }

        private void OnGreetPlayer(int who, HandledEventArgs e)
        {
            if (Main.netMode != 2)
            {
                return;
            }
            int plr = who; //legacy support
            Log.Info(Tools.FindPlayer(who) + " (" + Tools.GetPlayerIP(who) + ") from '" + players[who].group.GetName() + "' group joined.");
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
            ShowUpdateReminder(who);
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
                //Commands.CommandArgs args = new Commands.CommandArgs(msg, x, y, ply);
                Commands.Command cmd = null;
                for (int i = 0; i < Commands.commands.Count; i++)
                {
                    if (Commands.commands[i].Name().Equals(text.Split(' ')[0].TrimStart('/')))
                    {
                        cmd = Commands.commands[i];
                    }
                }

                if (cmd == null)
                {
                    Tools.SendMessage(ply, "That command does not exist, try /help", new float[] { 255, 0, 0 });
                }
                else
                {
                    if (!cmd.Run(text, players[ply]))
                    {
                        Log.Info(Tools.FindPlayer(ply) + " tried to execute " + cmd.Name() +
                                 " that s/he did not have access to!");
                        Tools.SendMessage(ply, "You do not have access to that command.", new float[] { 255, 0, 0 });
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
                    Tools.ForceKick(ply, "You are banned: " + ban.Reason);
                    handler.Handled = true;
                }
                else if (!FileTools.OnWhitelist(ip))
                {
                    Tools.ForceKick(ply, "Not on whitelist.");
                    handler.Handled = true;
                }
            }
        }

        private void OnLoadContent(ContentManager obj)
        {
        }

        private void OnPreInit()
        {
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

            //ConfigurationManager.maxSlots = Main.maxPlayers - 1;
        }

        private void OnUpdate(GameTime time)
        {
            if (Main.netMode != 2)
            {
                return;
            }
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active == false)
                {
                    continue;
                }
                if (players[i].tileThreshold >= 20)
                {
                    if (Main.player[i] != null)
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

        public static void ShowUpdateReminder(int ply)
        {
            if (!shownVersion)
            {
                if (players[ply].group.HasPermission("maintenance"))
                {
                    WebClient client = new WebClient();
                    client.Headers.Add("user-agent",
                                       "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705;)");
                    try
                    {
                        string updateString = client.DownloadString("http://shankshock.com/tshock-update.txt");
                        string[] changes = updateString.Split(',');
                        Version updateVersion = new Version(Convert.ToInt32(changes[0]), Convert.ToInt32(changes[1]),
                                                            Convert.ToInt32(changes[2]), Convert.ToInt32(changes[3]));
                        float[] color = { 255, 255, 000 };
                        if (VersionNum.CompareTo(updateVersion) < 0)
                        {
                            Tools.SendMessage(ply, "This server is out of date, to update now type /updatenow");
                            if (!updateCmd)
                            {
                                Commands.commands.Add(new Commands.Command("updatenow", "maintenance", Commands.UpdateNow));
                                updateCmd = true;
                            }
                            for (int i = 4; i <= changes.Length; i++)
                            {
                                Tools.SendMessage(ply, changes[i], color);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        FileTools.WriteError(e.Message);
                    }
                    shownVersion = true;
                }
            }
        }

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
                        Tools.Broadcast("You call that a lot? " + ConfigurationManager.killCount + " goblins killed!");
                        break;
                    case 1:
                        Tools.Broadcast("Fatality! " + ConfigurationManager.killCount + " goblins killed!");
                        break;
                    case 2:
                        Tools.Broadcast("Number of 'noobs' killed to date: " + ConfigurationManager.killCount);
                        break;
                    case 3:
                        Tools.Broadcast("Duke Nukem would be proud. " + ConfigurationManager.killCount +
                                        " goblins killed.");
                        break;
                    case 4:
                        Tools.Broadcast("You call that a lot? " + ConfigurationManager.killCount + " goblins killed!");
                        break;
                    case 5:
                        Tools.Broadcast(ConfigurationManager.killCount + " copies of Call of Duty smashed.");
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
            for (int i = 1; i <= 45; i++)
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
            for (int i = 1; i <= 238; i++)
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