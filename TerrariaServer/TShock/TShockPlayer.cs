using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace TShock
{
	internal class TShockPlayer : IPlayer
	{
		ServerSock Socket;
		Player Player;
		internal TShockPlayer(int id)
		{
			Id = id;
			Player = Main.player[id];
			Socket = Netplay.serverSock[id];
		}

		public int Id
		{
			get;
			protected set;
		}

		public string Name
		{
			get { return Player.name; }
			set { Player.name = value; }
		}
	}
}
