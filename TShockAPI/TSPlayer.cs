using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI
{
    class TSPlayer
    {
        public uint tileThreshold;
        private int player;

        public TSPlayer(int ply)
        {
            player = ply;
        }

        public Terraria.Player GetPlayer()
        {
            return Terraria.Main.player[player];
        }
    }
}
