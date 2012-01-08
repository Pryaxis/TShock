using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace TShock
{
	public class TShockPlayer : IPlayer
	{
		Player player;
		public TShockPlayer(Player ply)
		{
			player = ply;
		}

		public string Name
		{
			get { return player.name; }
			set { player.name = value; }
		}
	}
}
