using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaAPI;
using TerrariaAPI.Hooks;

namespace TShockAPI
{
    public class TShock : TerrariaPlugin
    {
        uint[] tileThreshold = new uint[Main.maxPlayers];

        public static string saveDir = "./tshock/";

        public static Version VersionNum = new Version(1, 4, 0, 0);

        public static bool shownVersion = false;

        public static Dictionary<string, Commands.CommandDelegate> admincommandList = new Dictionary<string, Commands.CommandDelegate>();

        public static Dictionary<string, Commands.CommandDelegate> commandList = new Dictionary<string, Commands.CommandDelegate>();

        static bool[] BlacklistTiles;

        public override Version Version
        {
            get { return VersionNum; }
        }

        public override Version APIVersion
        {
            get { return new Version(1, 1); }
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
            GameHooks.OnPreInitialize += OnPreInit;
            GameHooks.OnPostInitialize += OnPostInit;
            GameHooks.OnUpdate += new Action<Microsoft.Xna.Framework.GameTime>(OnUpdate);
            GameHooks.OnLoadContent += new Action<Microsoft.Xna.Framework.Content.ContentManager>(OnLoadContent);
            ServerHooks.OnChat += new Action<int, string, HandledEventArgs>(OnChat);
            ServerHooks.OnJoin += new Action<int, AllowEventArgs>(OnJoin);
            NetHooks.OnPreGetData += GetData;
            NetHooks.OnGreetPlayer += new NetHooks.GreetPlayerD(OnGreetPlayer);
            NpcHooks.OnStrikeNpc += new NpcHooks.StrikeNpcD(NpcHooks_OnStrikeNpc);
            Commands.InitCommands();
        }

        public override void DeInitialize()
        {
            GameHooks.OnPreInitialize -= OnPreInit;
            GameHooks.OnPostInitialize -= OnPostInit;
            GameHooks.OnUpdate -= new Action<Microsoft.Xna.Framework.GameTime>(OnUpdate);
            GameHooks.OnLoadContent -= new Action<Microsoft.Xna.Framework.Content.ContentManager>(OnLoadContent);
            ServerHooks.OnChat -= new Action<int, string, HandledEventArgs>(OnChat);
            ServerHooks.OnJoin -= new Action<int, AllowEventArgs>(OnJoin);
            NetHooks.OnPreGetData -= GetData;
            NetHooks.OnGreetPlayer -= new NetHooks.GreetPlayerD(OnGreetPlayer);
            NpcHooks.OnStrikeNpc -= new NpcHooks.StrikeNpcD(NpcHooks_OnStrikeNpc);
        }

        /*
         * Hooks:
         * */

        void NpcHooks_OnStrikeNpc(NpcStrikeEventArgs e)
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

        void GetData(GetDataEventArgs e)
        {
            if (Main.netMode != 2) { return; }
            if (e.MsgID == 17)
            {
                byte type;
                int x = 0;
                int y = 0;
                using (var br = new BinaryReader(new MemoryStream(e.Msg.readBuffer, e.Index, e.Length)))
                {
                    type = br.ReadByte();
                    x = br.ReadInt32();
                    y = br.ReadInt32();
                }
                if (type == 0 && BlacklistTiles[Main.tile[x, y].type] && Main.player[e.Msg.whoAmI].active)
                {
                    tileThreshold[e.Msg.whoAmI]++;
                }
                return;
            }
            if (e.MsgID == 0x1e)
            {
                Main.player[e.Msg.whoAmI].hostile = true;
                NetMessage.SendData(30, -1, -1, "", e.Msg.whoAmI);
                e.Handled = true;
            }
            if (e.MsgID == 0x0A) //SendSection
            {
                Tools.Broadcast(string.Format("{0}({1}) attempted sending a section", Main.player[e.Msg.whoAmI].name, e.Msg.whoAmI));
                e.Handled = true;
            }
            if (e.MsgID == 0x17) //Npc Data
            {
                Tools.Broadcast(string.Format("{0}({1}) attempted spawning an NPC", Main.player[e.Msg.whoAmI].name, e.Msg.whoAmI));
                e.Handled = true;
            }
        }

        void OnGreetPlayer(int who, HandledEventArgs e)
        {
            if (Main.netMode != 2) { return; }
            int plr = who; //legacy support
            Tools.ShowMOTD(who);
            if (Main.player[plr].statLifeMax > 400 || Main.player[plr].statManaMax > 200 || Main.player[plr].statLife > 400 || Main.player[plr].statMana > 200 || CheckInventory(plr))
            {
                Tools.HandleCheater(plr);
            }
            if (ConfigurationManager.permaPvp)
            {
                Main.player[who].hostile = true;
                NetMessage.SendData(30, -1, -1, "", who);
            }
            if (Tools.IsAdmin(who) && ConfigurationManager.infiniteInvasion && !ConfigurationManager.startedInvasion)
            {
                StartInvasion();
            }
            ShowUpdateReminder(who);
            e.Handled = true;
        }

        void OnChat(int ply, string msg, HandledEventArgs handler)
        {
            if (Main.netMode != 2) { return; }
            int x = (int)Main.player[ply].position.X;
            int y = (int)Main.player[ply].position.Y;

            if (msg.StartsWith("/"))
            {
                Commands.CommandArgs args = new Commands.CommandArgs(msg, x, y, ply);
                var commands = commandList;
                if (Tools.IsAdmin(ply))
                    commands = admincommandList;

                Commands.CommandDelegate command;
                if (commands.TryGetValue(msg.Split(' ')[0].TrimStart('/'), out command))
                    command.Invoke(args);
                else
                    Tools.SendMessage(ply, "Invalid command or no permissions! Try /help.", new float[] { 255f, 0f, 0f });
                handler.Handled = true;
            }
        }

        void OnJoin(int ply, AllowEventArgs handler)
        {
            if (Main.netMode != 2) { return; }
            string ip = Tools.GetRealIP((Convert.ToString(Netplay.serverSock[ply].tcpClient.Client.RemoteEndPoint)));
            if (FileTools.CheckBanned(ip) || FileTools.CheckCheat(ip) || FileTools.CheckGreif(ip))
            {
                Tools.Kick(ply, "You are banned.");
            }
            if (!FileTools.OnWhitelist(ip))
            {
                Tools.Kick(ply, "Not on whitelist.");
            }
        }

        void OnLoadContent(Microsoft.Xna.Framework.Content.ContentManager obj)
        {
        }

        void OnPreInit()
        {
            FileTools.SetupConfig();
        }

        void OnPostInit()
        {
        }

        void OnUpdate(GameTime time)
        {
            if (Main.netMode != 2) { return; }
            for (uint i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active == false) { continue; }
                if (tileThreshold[i] >= 5)
                {
                    if (Main.player[i] != null)
                    {
                        FileTools.WriteGrief((int)i);
                        Tools.Kick((int)i, "Kill tile abuse detected.");
                    }
                    tileThreshold[i] = 0;
                }
                else if (tileThreshold[i] > 0)
                {
                    tileThreshold[i] = 0;
                }
            }
            //writestuff();
        }

        /*
         * Useful stuff:
         * */

        public static void ShowUpdateReminder(int ply)
        {
            if (!shownVersion)
            {
                if (Tools.IsAdmin(Tools.FindPlayer(ply)))
                {
                    WebClient client = new WebClient();
                    client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705;)");
                    try
                    {
                        string updateString = client.DownloadString("http://shankshock.com/tshock-update.txt");
                        string[] changes = updateString.Split(',');
                        Version updateVersion = new Version(Convert.ToInt32(changes[0]), Convert.ToInt32(changes[1]), Convert.ToInt32(changes[2]), Convert.ToInt32(changes[3]));
                        float[] color = { 255, 255, 000 };
                        if (VersionNum.CompareTo(updateVersion) < 0)
                        {
                            Tools.SendMessage(ply, "This server is out of date.");
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
            Main.player[ply].velocity = new Vector2(0, 0);
            NetMessage.SendData(0x0d, -1, -1, "", ply);
            Main.player[ply].position.X = x;
            Main.player[ply].position.Y = y - 0x2a;
            NetMessage.SendData(0x0d, -1, -1, "", ply);
            UpdatePlayers();
        }

        public static void Teleport(int ply, float x, float y)
        {
            Main.player[ply].position.X = x;
            Main.player[ply].position.Y = y - 0x2a;
            NetMessage.SendData(0x0d, -1, -1, "", ply);
            int oldx = Main.player[ply].SpawnX;
            int oldy = Main.player[ply].SpawnY;
            Main.player[ply].SpawnX = (int)(x / 16);
            Main.player[ply].SpawnY = (int)((y - 0x2a) / 16);
            NetMessage.SendData(0xC, -1, -1, "", ply);
            Main.player[ply].SpawnX = oldx;
            Main.player[ply].SpawnY = oldy;
            UpdatePlayers();
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
                        Tools.Broadcast("Duke Nukem would be proud. " + ConfigurationManager.killCount + " goblins killed.");
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
                        NetMessage.SendData(5, h, i, Main.player[i].inventory[j].name, i, (float)j, 0f, 0f);
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
                NetMessage.SendData(44, i, -1, "", plr, (float)1, (float)9999999, (float)0);
        }

        //TODO : Notify the player if there is more than one match. (or do we want a First() kinda thing?)
        public static int GetNPCID(string name, bool exact = false)
        {
            NPC npc = new NPC();
            for (int i = 1; i <= 43; i++)
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
            for (int i = 1; i <= 235; i++)
            {
                item.SetDefaults(i);
                if (item.name.ToLower().StartsWith(name))
                    return i;
            }
            return -1;
        }

        public static bool CheckInventory(int plr)
        {
            for (int i = 0; i < 44; i++)
            {
                if (Main.player[plr].inventory[i].stack > Main.player[plr].inventory[i].maxStack)
                    return true;
            }
            return false;
        }

        public static void writestuff()
        {
            NPC npc = new NPC();
            string[] npclist = new string[44];
            int h = 0;
            for (int i = 1; i < 44; i++)
            {
                npc.SetDefaults(i);
                //npclist[h++] = string.Format("{0} - {1} - {2} - {3} - {4} - {5} - {6} - {7}", i, npc.name, npc.townNPC, npc.boss, npc.noGravity, npc.lifeMax, npc.damage, npc.defense);
                npclist[h++] = string.Format("{0}\t-\t{1}\t-\t{2}\t-\t{3}\t-\t{4}\t-\t{5}\t-\t{6}\t-\t{7}", i, npc.name, npc.townNPC, npc.boss, npc.noGravity, npc.lifeMax, npc.damage, npc.defense);
            }
            File.WriteAllLines("npclist.txt", npclist);
            Item item = new Item();
            string[] itemlist = new string[236];
            h = 0;
            for (int i = 1; i < 236; i++)
            {
                item.SetDefaults(i);
                //itemlist[h++] = string.Format("{0} - {1} - {2} - {3}", i, item.name, item.maxStack, item.rare);
                itemlist[h++] = string.Format("{0}\t-\t{1}\t-\t{2}\t-\t{3}", i, item.name, item.maxStack, item.rare);
            }
            File.WriteAllLines("itemlist.txt", itemlist);
        }
    }
}