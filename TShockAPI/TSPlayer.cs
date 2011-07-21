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
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaAPI;
using TShockAPI.DB;
using TShockAPI.Net;

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
        public DateTime LastTileChangeNotify { get; set; }
        public bool InitSpawn = false;
        public bool DisplayLogs = true;
        public Vector2 oldSpawn = Vector2.Zero;
        public TSPlayer LastWhisper;
        public int LoginAttempts { get; set; }
        public Vector2 TeleportCoords = new Vector2(-1, -1);
        public string UserAccountName { get; set; }
        public bool HasBeenSpammedWithBuildMessage = false;
        public bool IsLoggedIn = false;
        public int UserID = -1;
        Player FakePlayer = null;

        public bool RealPlayer
        {
            get { return Index >= 0 && Index < Main.maxNetPlayers && Main.player[Index] != null; }
        }
        public bool ConnectionAlive
        {
            get { return RealPlayer ? Netplay.serverSock[Index] != null && Netplay.serverSock[Index].active && !Netplay.serverSock[Index].kill : false; }
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
                    for (int i = 0; i < 40; i++)
                    {
                        if (TPlayer.inventory[i] == null || !TPlayer.inventory[i].active)
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
            Group = new Group("null");
        }

        protected TSPlayer(String playerName)
        {
            TilesDestroyed = new Dictionary<Vector2, Tile>();
            Index = -1;
            FakePlayer = new Player { name = playerName, whoAmi = -1 };
            Group = new Group("null");
        }

        public virtual void Disconnect(string reason)
        {
            SendData(PacketTypes.Disconnect, reason);
        }

        void SendTeleport(int tilex, int tiley)
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
                WorldID = Main.worldID,
                WorldFlags = (WorldGen.shadowOrbSmashed ? WorldInfoFlag.OrbSmashed : WorldInfoFlag.None) |
                (NPC.downedBoss1 ? WorldInfoFlag.DownedBoss1 : WorldInfoFlag.None) |
                (NPC.downedBoss2 ? WorldInfoFlag.DownedBoss2 : WorldInfoFlag.None) |
                (NPC.downedBoss3 ? WorldInfoFlag.DownedBoss3 : WorldInfoFlag.None),
                WorldName = Main.worldName
            };


            var ms = new MemoryStream();
            msg.PackFull(ms);
            SendRawData(ms.ToArray());
        }

        public bool Teleport(int tilex, int tiley)
        {
            InitSpawn = false;

            SendTeleport(tilex, tiley);

            //150 Should avoid all client crash errors
            //The error occurs when a tile trys to update which the client hasnt load yet, Clients only update tiles withen 150 blocks
            //Try 300 if it does not work (Higher number - Longer load times - Less chance of error)
            if (!SendTileSquare(tilex, tiley, 150))
            {
                SendMessage("Warning, teleport failed due to being too close to the edge of the map.", Color.Red);
                return false;
            }

            if (TPlayer.SpawnX > 0 && TPlayer.SpawnY > 0)
            {
                int spX = TPlayer.SpawnX;
                int spY = TPlayer.SpawnY;
                Main.tile[spX, spY].active = false;
                SendTileSquare(spX, spY);
                Spawn();
                Main.tile[spX, spY].active = true;
                SendTileSquare(spX, spY);
                oldSpawn = new Vector2(spX, spY);
            }
            else
            {
                //Checks if Player has spawn point set (Server may think player does not have spawn)
                if (oldSpawn != Vector2.Zero)
                {
                    Main.tile[(int)oldSpawn.X, (int)oldSpawn.Y].active = false;
                    SendTileSquare((int)oldSpawn.X, (int)oldSpawn.Y);
                    Spawn();
                    Main.tile[(int)oldSpawn.X, (int)oldSpawn.Y].active = true;
                    SendTileSquare((int)oldSpawn.X, (int)oldSpawn.Y);
                    NetMessage.syncPlayers();
                }
                //Player has no spawn point set
                else
                {
                    Spawn();
                }
            }

            SendTeleport(Main.spawnTileX, Main.spawnTileY);

            return true;
        }

        public void Spawn()
        {
            SendData(PacketTypes.PlayerSpawn, "", Index, 0.0f, 0.0f, 0.0f);
        }

        public virtual bool SendTileSquare(int x, int y, int size = 10)
        {
            try
            {
                SendData(PacketTypes.TileSendSquare, "", size, (float)(x - (size / 2)), (float)(y - (size / 2)));
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
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

        public virtual void Whoopie(object time)
        {
            var time2 = (int)time;
            var launch = DateTime.UtcNow;
            var player = 0;
            for (int i = 0; i < Main.maxPlayers; i++)
                if (Main.player[i] != null & Main.player[i].active && i != Index)
                    player = i;
            SendMessage("You are now being annoyed.", Color.Red);
            var oriinv = Main.player[0].inventory[player];
            while ((DateTime.UtcNow - launch).TotalSeconds < time2)
            {
                Main.player[0].inventory[player].SetDefaults("Whoopie Cushion");
                Main.player[0].inventory[player].stack = 1;
                SendData(TerrariaAPI.PacketTypes.PlayerSlot, "Whoopie Cushion", player, 0f);
                Main.player[player].position = TPlayer.position;
                Main.player[player].selectedItem = 0;
                Main.player[player].controlUseItem = true;
                SendData(TerrariaAPI.PacketTypes.PlayerUpdate, number: player);
                System.Threading.Thread.Sleep(500);
                Main.player[player].controlUseItem = false;
                SendData(TerrariaAPI.PacketTypes.PlayerUpdate, number: player);
                System.Threading.Thread.Sleep(50);
            }
            Main.player[0].inventory[0] = oriinv;
            SendData(TerrariaAPI.PacketTypes.PlayerSlot, oriinv.name, player, 0f);
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

            try
            {
                if (Netplay.serverSock[Index].tcpClient.Connected)
                {
                    Netplay.serverSock[Index].networkStream.Write(data, 0, data.Length);
                    return true;
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
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
            RconHandler.Response += msg + "\n";
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