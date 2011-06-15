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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;

namespace TShockAPI
{
    public class TSPlayer
    {
        public static readonly TSPlayer Server = new TSPlayer(new Player { name = "Server" });
        public static readonly TSPlayer All = new TSPlayer(new Player { name = "All", whoAmi = -1 });


        public uint TileThreshold { get; set; }
        public Dictionary<Vector2, Tile> TilesDestroyed { get; set; }
        public bool SyncHP { get; set; }
        public bool SyncMP { get; set; }
        public Group Group { get; set; }
        public bool ReceivedInfo { get; set; }

        public int Index { get { return TPlayer.whoAmi; } }

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

        public float X
        {
            get { return TPlayer.position.X; }
        }
        public float Y
        {
            get { return TPlayer.position.Y; }
        }
        public int TileX
        {
            get { return (int)(X / 16); }
        }
        public int TileY
        {
            get { return (int)(X / 16); }
        }

        public TSPlayer(Player ply)
        {
            TilesDestroyed = new Dictionary<Vector2, Tile>();
            TPlayer = ply;
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
}