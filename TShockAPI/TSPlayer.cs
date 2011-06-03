using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI
{
    public class TSPlayer
    {
        public uint tileThreshold;
        private int player;
        private bool admin;
        private bool adminSet;

        public TSPlayer(int ply)
        {
            player = ply;
        }

        public Terraria.Player GetPlayer()
        {
            return Terraria.Main.player[player];
        }

        public bool IsAdmin()
        {
            if (adminSet)
            {
                return admin;
            }
            admin = Tools.IsAdmin(player);
            adminSet = true;
            return admin;
        }
    }
}
