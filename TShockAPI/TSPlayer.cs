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
        public uint tileThreshold;
        public Dictionary<TShock.Position, Tile> tilesDestroyed = new Dictionary<TShock.Position, Tile>();
        public bool syncHP;
        public bool syncMP;
        public Group group;
        public bool receivedInfo;
        private int player;

        public TSPlayer(int ply)
        {
            player = ply;
        }

        public Player GetPlayer()
        {
            return Main.player[player];
        }

        public int GetPlayerID()
        {
            return player;
        }
    }
}