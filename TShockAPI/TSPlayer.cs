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