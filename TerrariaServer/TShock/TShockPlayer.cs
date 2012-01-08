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

        public string IP
        {
            get { return Socket.tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]; }
            set { this.IP = value; }
        }

		public void Damage (int amount)
		{
			NetMessage.SendData((int)PacketTypes.PlayerDamage, -1, -1, "", Id, ((new Random()).Next(-1, 1)), amount, (float)0);
		}

		public void SendMessage(string msg, Color color)
		{
			if (color == default(Color))
				color = Color.White;
			NetMessage.SendData((int)PacketTypes.ChatText, Id, -1, msg, 0xFF, color.R, color.B, color.G);
		}
	}
}
