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
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaAPI;

namespace TShockAPI
{
    public class TSPlayer
    {
        public static readonly TSServerPlayer Server = new TSServerPlayer();
        public static readonly TSPlayer All = new TSPlayer("All");
        public int TileThreshold { get; set; }
        public Dictionary<Vector2, Tile> TilesDestroyed { get; protected set; }
        public bool SyncHP { get; set; }
        public bool SyncMP { get; set; }
        public Group Group { get; set; }
        public bool ReceivedInfo { get; set; }
        public int Index { get; protected set; }
        public DateTime LastPvpChange { get; protected set; }
        public Rectangle TempArea = new Rectangle();
        public DateTime LastExplosive { get; set; }
        public bool InitSpawn = false;
        public bool DisplayLogs = true;

        public bool RealPlayer
        {
            get { return Index >= 0 && Index < Main.maxNetPlayers; }
        }
        public bool ConnectionAlive
        {
            get { return RealPlayer ? Netplay.serverSock[Index].active && !Netplay.serverSock[Index].kill : false; }
        }
        public string IP
        {
            get
            {
                return RealPlayer ? Tools.GetRealIP(Netplay.serverSock[Index].tcpClient.Client.RemoteEndPoint.ToString()) : "";
            }
        }
        /// <summary>
        /// Terraria Player
        /// </summary>
        public Player TPlayer { get; protected set; }
        public string Name
        {
            get { return TPlayer.name; }
        }
        public bool Active
        {
            get { return TPlayer.active; }
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
                return RealPlayer ?  TPlayer.position.Y : Main.spawnTileY * 16;
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
                    for (int i = 0; i < 40; i++)
                    {
                        if (!TPlayer.inventory[i].active)
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
            TilesDestroyed = new Dictionary<Vector2, Tile>();
            Index = index;
            TPlayer = Main.player[index];
            Group = new Group("null");
        }

        protected TSPlayer(String playerName)
        {
            TilesDestroyed = new Dictionary<Vector2, Tile>();
            Index = -1;
            TPlayer = new Player { name = playerName, whoAmi = -1 };
            Group = new Group("null");
        }

        public virtual void Disconnect(string reason)
        {
            SendData(PacketTypes.Disconnect, reason);
        }

        public bool Teleport(int tileX, int tileY)
        {
            this.InitSpawn = false;
            int spawnTileX = Main.spawnTileX;
            int spawnTileY = Main.spawnTileY;
            Main.spawnTileX = tileX;
            Main.spawnTileY = tileY;
            SendData(PacketTypes.WorldInfo);
            SendTileSquare(tileX, tileY, 50);

            if (TPlayer.SpawnX > 0 && TPlayer.SpawnY > 0)
            {
                Main.tile[TPlayer.SpawnX, TPlayer.SpawnY].active = false;
                NetMessage.SendTileSquare(Index, TPlayer.SpawnX, TPlayer.SpawnY, 1);
                Spawn();
                Main.tile[TPlayer.SpawnX, TPlayer.SpawnY].active = true;
                NetMessage.SendTileSquare(Index, TPlayer.SpawnX, TPlayer.SpawnY, 1);
                SendMessage("Warning! Your bed spawn point has been destroyed because of warp", Color.Red);
            }
            else
            {
                Spawn();
            }

            Main.spawnTileX = spawnTileX;
            Main.spawnTileY = spawnTileY;
            SendData(PacketTypes.WorldInfo);
            return true;
        }

        public void Spawn()
        {
            SendData(PacketTypes.PlayerSpawn,  "", Index, 0.0f, 0.0f, 0.0f);
        }

        public virtual void SendTileSquare(int x, int y, int size = 10)
        {
            SendData(PacketTypes.TileSendSquare, "", size, (float)(x - (size / 2)), (float)(y - (size / 2)), 0f);
        }

        public virtual void GiveItem(int type, string name, int width, int height, int stack)
        {
            int itemid = Terraria.Item.NewItem((int)X, (int)Y, width, height, type, stack, true);
            // This is for special pickaxe/hammers/swords etc
            Main.item[itemid].SetDefaults(name);
            // The set default overrides the wet and stack set by NewItem
            Main.item[itemid].wet = Collision.WetCollision(Main.item[itemid].position, Main.item[itemid].width, Main.item[itemid].height);
            Main.item[itemid].stack = stack;
            Main.item[itemid].owner = Index;
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

        public virtual void SetPvP(bool pvp)
        {
            if (TPlayer.hostile != pvp)
            {
                LastPvpChange = DateTime.UtcNow;
                TPlayer.hostile = pvp;
                All.SendMessage(string.Format("{0} has {1} PvP!", Name, pvp ? "enabled" : "disabled"), Main.teamColor[Team]);
            }
            //Broadcast anyways to keep players synced
            NetMessage.SendData((int)PacketTypes.TogglePVP, -1, -1, "", Index);
        }

        //Todo: Separate this into a few functions. SendTo, SendToAll, etc
        public void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            if (RealPlayer && !ConnectionAlive)
                return;
            NetMessage.SendData((int)msgType, Index, -1, text, number, number2, number3, number4, number5);
        }
    }

    public class TSServerPlayer : TSPlayer
    {
        public TSServerPlayer() : base("Server")
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
                Tools.GetRandomClearTileWithInRange(startTileX, startTileY, tileXRange, tileYRange, out spawnTileX, out spawnTileY);
                int npcid = NPC.NewNPC(spawnTileX * 16, spawnTileY * 16, type, 0);
                // This is for special slimes
                Main.npc[npcid].SetDefaults(name);
            }
        }

        public void StrikeNPC(int npcid, int damage, float knockBack, int hitDirection)
        {
            Main.npc[npcid].StrikeNPC(damage, knockBack, hitDirection);
            NetMessage.SendData((int)PacketTypes.NPCStrike, -1, -1, "", npcid, damage, knockBack, hitDirection);
        }

        public void RevertKillTile(Dictionary<Vector2, Tile> destroyedTiles)
        {
            // Update Main.Tile first so that when tile sqaure is sent it is correct
            foreach (KeyValuePair<Vector2, Tile> entry in destroyedTiles)
            {
                Main.tile[(int)entry.Key.X, (int)entry.Key.Y] = entry.Value;
                Log.Debug(string.Format("Reverted DestroyedTile(TileXY:{0}_{1}, Type:{2})", 
                                        entry.Key.X, entry.Key.Y, Main.tile[(int)entry.Key.X, (int)entry.Key.Y].type));
            }
            // Send all players updated tile sqaures
            foreach (Vector2 coords in destroyedTiles.Keys)
            {
                TSPlayer.All.SendTileSquare((int)coords.X, (int)coords.Y, 3);
            }
        }
    }
}