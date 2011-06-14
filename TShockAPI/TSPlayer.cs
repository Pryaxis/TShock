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
using Terraria;

namespace TShockAPI
{
    public class TSPlayer
    {
        public uint TileThreshold { get; set; }
        public Dictionary<TShock.Position, Tile> TilesDestroyed { get; set; }
        public bool SyncHP { get; set; }
        public bool SyncMP { get; set; }
        public Group Group { get; set; }
        public bool ReceivedInfo { get; set; }

        public int Index {get;private set;}
        public Player Player
        {
            get { return Main.player[Index]; }
        }

        public TSPlayer(int ply)
        {
            TilesDestroyed = new Dictionary<TShock.Position, Tile>();
        }
    }
}