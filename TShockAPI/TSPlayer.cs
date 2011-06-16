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

namespace TShockAPI
{
    public class TSPlayer
    {
        public static readonly TSPlayer Server = new ServerPlayer();
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
            NetMessage.SendData(0x19, Index, -1, msg, 255, red, green, blue);
        }

    }


    public class ServerPlayer : TSPlayer
    {
        public ServerPlayer() : base("Server")
        {
            Group = new SuperAdminGroup();
        }

        public override void SendMessage(string msg)
        {
            Console.WriteLine(msg);
        }
        public override void SendMessage(string msg, byte red, byte green, byte blue)
        {
            SendMessage(msg);
        }
        public override void SendMessage(string msg, Color color)
        {
            SendMessage(msg);
        }
    }
}