using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI
{
    public class TSPlayer
    {
        public uint tileThreshold;
        public bool syncHP = false;
        public bool syncMP = false;
        public Group group;

        private int player;

        public TSPlayer(int ply)
        {
            player = ply;
        }

        public Terraria.Player GetPlayer()
        {
            return Terraria.Main.player[player];
        }

        public int GetPlayerID()
        {
            return player;
        }
    }
}
