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

using System.IO;
using System.Threading;
using Terraria;

using TShockAPI.Net;

namespace TShockAPI
{
    public class TSPlayer
    {
        public static readonly TSServerPlayer Server = new TSServerPlayer();
        public static readonly TSPlayer All = new TSPlayer("All");
        public int TileKillThreshold { get; set; }
        public int TilePlaceThreshold { get; set; }
        public int TileLiquidThreshold { get; set; }
        public int ProjectileThreshold { get; set; }
        public Dictionary<Vector2, TileData> TilesDestroyed { get; protected set; }
        public Dictionary<Vector2, TileData> TilesCreated { get; protected set; }
        public int FirstMaxHP { get; set; }
        public int FirstMaxMP { get; set; }
        public Group Group { get; set; }
        public bool ReceivedInfo { get; set; }
        public int Index { get; protected set; }
        public DateTime LastPvpChange;
        public Point[] TempPoints = new Point[2];
        public int AwaitingTempPoint { get; set; }
        public bool AwaitingName { get; set; }
        public DateTime LastThreat { get; set; }
        public DateTime LastTileChangeNotify { get; set; }
        public bool InitSpawn;
        public bool DisplayLogs = true;
        public Vector2 oldSpawn = Vector2.Zero;
        public TSPlayer LastWhisper;
        public int LoginAttempts { get; set; }
        public Vector2 TeleportCoords = new Vector2(-1, -1);
        public Vector2 LastNetPosition = Vector2.Zero; 
        public string UserAccountName { get; set; }
        public bool HasBeenSpammedWithBuildMessage;
        public bool IsLoggedIn;
        public int UserID = -1;
        public bool HasBeenNaggedAboutLoggingIn;
        public bool TPAllow = true;
        public bool mute = false;
        public bool TpLock = false;
        Player FakePlayer;
        public bool RequestedSection = false;
        public DateTime LastDeath { get; set; }
        public bool Dead = false;
        public string Country = "??";
        public int Difficulty;
        private string CacheIP;
        public bool IgnoreActionsForPvP = false;
        public bool IgnoreActionsForInventory = false;
        public string IgnoreActionsForCheating = "none";
        public bool IgnoreActionsForClearingTrashCan = false;
        public PlayerData PlayerData;
        public bool RequiresPassword = false;

        public bool RealPlayer
        {
            get { return Index >= 0 && Index < Main.maxNetPlayers && Main.player[Index] != null; }
        }
        public bool ConnectionAlive
        {
            get { return RealPlayer && (Netplay.serverSock[Index] != null && Netplay.serverSock[Index].active && !Netplay.serverSock[Index].kill); }
        }

        public int State
        {
            get { return Netplay.serverSock[Index].state; }
            set { Netplay.serverSock[Index].state = value; }
        }

        public string IP
        {
            get
            {
                if (string.IsNullOrEmpty(CacheIP))
                    return CacheIP = RealPlayer ? (Netplay.serverSock[Index].tcpClient.Connected ? TShock.Utils.GetRealIP(Netplay.serverSock[Index].tcpClient.Client.RemoteEndPoint.ToString()) : "") : "";
                else
                    return CacheIP;
            }
        }
        /// <summary>
        /// Terraria Player
        /// </summary>
        public Player TPlayer
        {
            get
            {
                return FakePlayer ?? Main.player[Index];
            }
        }
        public string Name
        {
            get { return TPlayer.name; }
        }
        public bool Active
        {
            get { return TPlayer != null && TPlayer.active; }
        }
        public int Team
        {
            get { return TPlayer.team; }
        }
        public float X
        {
            get
            {

                return RealPlayer ? TPlayer.position.X : Main.spawnTileX * 16;
            }
        }
        public float Y
        {
            get
            {
                return RealPlayer ? TPlayer.position.Y : Main.spawnTileY * 16;
            }
        }
        public int TileX
        {
            get { return (int)(X / 16); }
        }
        public int TileY
        {
            get { return (int)(Y / 16); }
        }
        public bool InventorySlotAvailable
        {
            get
            {
                bool flag = false;
                if (RealPlayer)
                {
                    for (int i = 0; i < 40; i++) //41 is trash can, 42-45 is coins, 46-49 is ammo
                    {
                        if (TPlayer.inventory[i] == null || !TPlayer.inventory[i].active || TPlayer.inventory[i].name == "")
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                return flag;
            }
        }

        public TSPlayer(int index)
        {
            TilesDestroyed = new Dictionary<Vector2, TileData>();
            TilesCreated = new Dictionary<Vector2, TileData>();
            Index = index;
            Group = new Group(TShock.Config.DefaultGuestGroupName);
        }

        protected TSPlayer(String playerName)
        {
            TilesDestroyed = new Dictionary<Vector2, TileData>();
            TilesCreated = new Dictionary<Vector2, TileData>();
            Index = -1;
            FakePlayer = new Player { name = playerName, whoAmi = -1 };
            Group = new Group(TShock.Config.DefaultGuestGroupName);
        }

        public virtual void Disconnect(string reason)
        {
            SendData(PacketTypes.Disconnect, reason);
        }

        public virtual void Flush()
        {
            var sock = Netplay.serverSock[Index];
            if (sock == null)
                return;

            TShock.PacketBuffer.Flush(sock);
        }


        void SendWorldInfo(int tilex, int tiley, bool fakeid)
        {
            using (var ms = new MemoryStream())
            {
                var msg = new WorldInfoMsg
                {
                    Time = (int)Main.time,
                    DayTime = Main.dayTime,
                    MoonPhase = (byte)Main.moonPhase,
                    BloodMoon = Main.bloodMoon,
                    MaxTilesX = Main.maxTilesX,
                    MaxTilesY = Main.maxTilesY,
                    SpawnX = tilex,
                    SpawnY = tiley,
                    WorldSurface = (int)Main.worldSurface,
                    RockLayer = (int)Main.rockLayer,
                    //Sending a fake world id causes the client to not be able to find a stored spawnx/y.
                    //This fixes the bed spawn point bug. With a fake world id it wont be able to find the bed spawn.
                    WorldID = !fakeid ? Main.worldID : -1,
                    WorldFlags = (WorldGen.shadowOrbSmashed ? WorldInfoFlag.OrbSmashed : WorldInfoFlag.None) |
                    (NPC.downedBoss1 ? WorldInfoFlag.DownedBoss1 : WorldInfoFlag.None) |
                    (NPC.downedBoss2 ? WorldInfoFlag.DownedBoss2 : WorldInfoFlag.None) |
                    (NPC.downedBoss3 ? WorldInfoFlag.DownedBoss3 : WorldInfoFlag.None) |
                    (Main.hardMode ? WorldInfoFlag.HardMode : WorldInfoFlag.None) |
                    (NPC.downedClown ? WorldInfoFlag.DownedClown : WorldInfoFlag.None), 
                    WorldName = Main.worldName
                };
                msg.PackFull(ms);
                SendRawData(ms.ToArray());
            }
        }

        public bool Teleport(int tilex, int tiley)
        {
            InitSpawn = false;


            SendWorldInfo(tilex, tiley, true);

            //150 Should avoid all client crash errors
            //The error occurs when a tile trys to update which the client hasnt load yet, Clients only update tiles withen 150 blocks
            //Try 300 if it does not work (Higher number - Longer load times - Less chance of error)
            //Should we properly send sections so that clients don't get tiles twice?
            if (!SendTileSquare(tilex, tiley))
            {
                InitSpawn = true;
                SendWorldInfo(Main.spawnTileX, Main.spawnTileY, false);
                SendMessage("Warning, teleport failed due to being too close to the edge of the map.", Color.Red);
                return false;
            }

            Spawn(-1, -1);

            SendWorldInfo(Main.spawnTileX, Main.spawnTileY, false);

            TPlayer.position.X = tilex;
            TPlayer.position.Y = tiley;

            return true;
        }

        public void Spawn()
        {
            Spawn(TPlayer.SpawnX, TPlayer.SpawnY);
        }

        public void Spawn(int tilex, int tiley)
        {
            using (var ms = new MemoryStream())
            {
                var msg = new SpawnMsg()
                {
                    PlayerIndex = (byte)Index,
                    TileX = tilex,
                    TileY = tiley
                };
                msg.PackFull(ms);
                SendRawData(ms.ToArray());
            }
        }

        public virtual bool SendTileSquare(int x, int y, int size = 10)
        {
            try
            {
                int num = (size - 1) / 2;
                SendData(PacketTypes.TileSendSquare, "", size, (float)(x - num), (float)(y - num));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
        }

        public virtual void GiveItem(int type, string name, int width, int height, int stack, int prefix = 0)
        {
            int itemid = Item.NewItem((int)X, (int)Y, width, height, type, stack, true, prefix);
            // This is for special pickaxe/hammers/swords etc
            Main.item[itemid].SetDefaults(name);
            // The set default overrides the wet and stack set by NewItem
            Main.item[itemid].wet = Collision.WetCollision(Main.item[itemid].position, Main.item[itemid].width, Main.item[itemid].height);
            Main.item[itemid].stack = stack;
            Main.item[itemid].owner = Index;
            Main.item[itemid].prefix = (byte) prefix;
            NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, "", itemid, 0f, 0f, 0f);
            NetMessage.SendData((int)PacketTypes.ItemOwner, -1, -1, "", itemid, 0f, 0f, 0f);
        }

        public virtual void SendMessage(string msg)
        {
            SendMessage(msg, 0, 255, 0);
        }

        public virtual void SendMessage(string msg, Color color)
        {
            SendMessage(msg, color.R, color.G, color.B);
        }

        public virtual void SendMessage(string msg, byte red, byte green, byte blue)
        {
            SendData(PacketTypes.ChatText, msg, 255, red, green, blue);
        }

        public virtual void DamagePlayer(int damage)
        {
            NetMessage.SendData((int)PacketTypes.PlayerDamage, -1, -1, "", Index, ((new Random()).Next(-1, 1)), damage, (float)0);
        }

        public virtual void SetTeam(int team)
        {
            Main.player[Index].team = team;
            SendData(PacketTypes.PlayerTeam, "", Index);
        }

        public virtual void Whoopie(object time)
        {
            var time2 = (int)time;
            var launch = DateTime.UtcNow;
            var startname = Name;
            SendMessage("You are now being annoyed.", Color.Red);
            while ((DateTime.UtcNow - launch).TotalSeconds < time2 && startname == Name)
            {
                SendData(PacketTypes.NpcSpecial, number: Index, number2: 2f);
                Thread.Sleep(50);
            }
        }

        public virtual void SetBuff(int type, int time = 3600)
        {
            SendData(PacketTypes.PlayerAddBuff, number: Index, number2: (float)type, number3: (float)time);
        }

        //Todo: Separate this into a few functions. SendTo, SendToAll, etc
        public virtual void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            if (RealPlayer && !ConnectionAlive)
                return;

            NetMessage.SendData((int)msgType, Index, -1, text, number, number2, number3, number4, number5);
        }

        public virtual bool SendRawData(byte[] data)
        {
            if (!RealPlayer || !ConnectionAlive)
                return false;

            return TShock.SendBytes(Netplay.serverSock[Index], data);
        }
    }

    public class TSServerPlayer : TSPlayer
    {
        public TSServerPlayer()
            : base("Server")
        {
            Group = new SuperAdminGroup();
        }

        public override void SendMessage(string msg)
        {
            SendMessage(msg, 0, 255, 0);
        }

        public override void SendMessage(string msg, Color color)
        {
            SendMessage(msg, color.R, color.G, color.B);
        }

        public override void SendMessage(string msg, byte red, byte green, byte blue)
        {
            Console.WriteLine(msg);
            //RconHandler.Response += msg + "\n";
        }
        
        public void SetFullMoon(bool fullmoon)
        {
            Main.moonPhase = 0;
            SetTime(false, 0);
        }

        public void SetBloodMoon(bool bloodMoon)
        {
            Main.bloodMoon = bloodMoon;
            SetTime(false, 0);
        }

        public void SetTime(bool dayTime, double time)
        {
            Main.dayTime = dayTime;
            Main.time = time;
            NetMessage.SendData((int)PacketTypes.TimeSet, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
            NetMessage.syncPlayers();
        }

        public void SpawnNPC(int type, string name, int amount, int startTileX, int startTileY, int tileXRange = 100, int tileYRange = 50)
        {
            for (int i = 0; i < amount; i++)
            {
                int spawnTileX;
                int spawnTileY;
                TShock.Utils.GetRandomClearTileWithInRange(startTileX, startTileY, tileXRange, tileYRange, out spawnTileX, out spawnTileY);
                int npcid = NPC.NewNPC(spawnTileX * 16, spawnTileY * 16, type, 0);
                // This is for special slimes
                Main.npc[npcid].SetDefaults(name);
            }
        }

        public void StrikeNPC(int npcid, int damage, float knockBack, int hitDirection)
        {
            Main.npc[npcid].StrikeNPC(damage, knockBack, hitDirection);
            NetMessage.SendData((int)PacketTypes.NpcStrike, -1, -1, "", npcid, damage, knockBack, hitDirection);
        }

        public void RevertTiles(Dictionary<Vector2, TileData> tiles)
        {
            // Update Main.Tile first so that when tile sqaure is sent it is correct
            foreach (KeyValuePair<Vector2, TileData> entry in tiles)
            {
                Main.tile[(int)entry.Key.X, (int)entry.Key.Y].Data = entry.Value;
            }
            // Send all players updated tile sqaures
            foreach (Vector2 coords in tiles.Keys)
            {
                All.SendTileSquare((int)coords.X, (int)coords.Y, 3);
            }
        }
    }

    public class PlayerData
    {
        public NetItem[] inventory = new NetItem[NetItem.maxNetInventory];
        public int maxHealth = 100;
        //public int maxMana = 100;
        public bool exists = false;

        public PlayerData(TSPlayer player)
        {
            for (int i = 0; i < NetItem.maxNetInventory; i++)
            {
                this.inventory[i] = new NetItem();
            }
            this.inventory[0].netID = -15;
            this.inventory[0].stack = 1;
            if(player.TPlayer.inventory[0] != null && player.TPlayer.inventory[0].netID == -15)
                this.inventory[0].prefix = player.TPlayer.inventory[0].prefix;
            this.inventory[1].netID = -13;
            this.inventory[1].stack = 1;
            if (player.TPlayer.inventory[1] != null && player.TPlayer.inventory[1].netID == -13)
                this.inventory[1].prefix = player.TPlayer.inventory[1].prefix;
            this.inventory[2].netID = -16;
            this.inventory[2].stack = 1;
            if (player.TPlayer.inventory[2] != null && player.TPlayer.inventory[2].netID == -16)
                this.inventory[2].prefix = player.TPlayer.inventory[2].prefix;
        }

        public void StoreSlot(int slot, int netID, int prefix, int stack)
        {
            this.inventory[slot].netID = netID;
            if (this.inventory[slot].netID != 0)
            {
                this.inventory[slot].stack = stack;
                this.inventory[slot].prefix = prefix;
            }
            else
            {
                this.inventory[slot].stack = 0;
                this.inventory[slot].prefix = 0;
            }
        }

        public void CopyInventory(TSPlayer player)
        {
            this.maxHealth = player.TPlayer.statLifeMax;
            Item[] inventory = player.TPlayer.inventory;
            Item[] armor = player.TPlayer.armor;
            for (int i = 0; i < NetItem.maxNetInventory; i++)
            {
                if (i < 49)
                {
                    if (player.TPlayer.inventory[i] != null)
                    {
                        this.inventory[i].netID = inventory[i].netID;
                    }
                    else
                    {
                        this.inventory[i].netID = 0;
                    }

                    if (this.inventory[i].netID != 0)
                    {
                        this.inventory[i].stack = inventory[i].stack;
                        this.inventory[i].prefix = inventory[i].prefix;
                    }
                    else
                    {
                        this.inventory[i].stack = 0;
                        this.inventory[i].prefix = 0;
                    }
                }
                else
                {
                    if (player.TPlayer.armor[i - 48] != null)
                    {
                        this.inventory[i].netID = armor[i - 48].netID;
                    }
                    else
                    {
                        this.inventory[i].netID = 0;
                    }

                    if (this.inventory[i].netID != 0)
                    {
                        this.inventory[i].stack = armor[i - 48].stack;
                        this.inventory[i].prefix = armor[i - 48].prefix;
                    }
                    else
                    {
                        this.inventory[i].stack = 0;
                        this.inventory[i].prefix = 0;
                    }
                }
            }
        }
    }

    public class NetItem
    {
        public static int maxNetInventory = 59;
        public int netID = 0;
        public int stack = 0;
        public int prefix = 0;

        public static string ToString(NetItem[] inventory)
        {
            string inventoryString = "";
            for (int i = 0; i < NetItem.maxNetInventory; i++)
            {
                if (i != 0)
                    inventoryString += "~";
                inventoryString += inventory[i].netID;
                if (inventory[i].netID != 0)
                {
                    inventoryString += "," + inventory[i].stack;
                    inventoryString += "," + inventory[i].prefix;
                }
                else
                {
                    inventoryString += ",0,0";
                }
            }
            return inventoryString;
        }

        public static NetItem[] Parse(string data)
        {
            NetItem[] inventory = new NetItem[NetItem.maxNetInventory];
            int i;
            for (i = 0; i < NetItem.maxNetInventory; i++)
            {
                inventory[i] = new NetItem();
            }
            string[] items = data.Split('~');
            i = 0;
            foreach (string item in items)
            {
                string[] idata = item.Split(',');
                inventory[i].netID = int.Parse(idata[0]);
                inventory[i].stack = int.Parse(idata[1]);
                inventory[i].prefix = int.Parse(idata[2]);
                i++;
            }
            return inventory;
        }
    }
}