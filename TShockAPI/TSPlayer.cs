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
        public uint TileThreshold { get; set; }
        public Dictionary<Vector2, Tile> TilesDestroyed { get; set; }
        public bool SyncHP { get; set; }
        public bool SyncMP { get; set; }
        public Group Group { get; set; }
        public bool ReceivedInfo { get; set; }
        public int Index { get; protected set; }
        public bool RealPlayer
        {
            get { return Index >= 0 && Index < Main.maxNetPlayers; }
        }
        public bool ConnectionAlive
        {
            get { return RealPlayer ? Netplay.serverSock[Index].active && !Netplay.serverSock[Index].kill : false; }
        }
        /// <summary>
        /// Terraria Player
        /// </summary>
        public Player TPlayer { get; protected set; }
        public string Name
        {
            get { return TPlayer.name; }
        }
        public string IP 
        {
            get 
            {
                return RealPlayer ? Tools.GetRealIP(Netplay.serverSock[Index].tcpClient.Client.RemoteEndPoint.ToString()) : ""; 
            }
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
            if (Index == -1)
                return;

            NetMessage.SendData((int)PacketTypes.Disconnect, Index, -1, reason, 0x0, 0f, 0f, 0f);
        }

        public virtual void SendTileSquare(int x, int y, int size = 10)
        {
            if (Index == -1)
                return;

            NetMessage.SendData((int)PacketTypes.TileSendSquare, Index, -1, "", size, (float)(x - (size / 2)), (float)(y - (size / 2)), 0f);
        }

        public virtual void GiveItem(int type, string name, int width, int height, int stack)
        {
            int itemid = Terraria.Item.NewItem((int)X, (int)Y, width, height, type, stack, true);
            // This is for special pickaxe/hammers/swords etc
            Main.item[itemid].SetDefaults(name);
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
            NetMessage.SendData((int)PacketTypes.ChatText, Index, -1, msg, 255, red, green, blue);
        }

        public virtual void DamagePlayer(int damage)
        {
            if (Index == -1)
                return;

            NetMessage.SendData((int)PacketTypes.PlayerDamage, -1, -1, "", Index, ((new Random()).Next(-1, 1)), damage, (float)0);
        }

        public virtual void SetPvP(bool pvp)
        {
            if (Index == -1)
                return;

            if (TPlayer.hostile != pvp)
            {
                TPlayer.hostile = pvp;
                NetMessage.SendData((int)PacketTypes.TogglePVP, -1, -1, "", Index);
                All.SendMessage(string.Format("{0} has {1} PvP!", Name, pvp ? "enabled" : "disabled"), Main.teamColor[Team]);
            }
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

        public void SpawnNPC(int type, string name, int amount, int startTileX, int startTileY, int tileXRange = 50, int tileYRange = 20)
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
    }
}