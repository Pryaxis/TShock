using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TShock;

namespace TShock.Hooks.Player
{
	public class PlayerEventArgs : HandledEventArgs
	{
		public PlayerEventArgs(IPlayer player)
		{
			Player = player;
		}
		public IPlayer Player { get; protected set; }
	}
}
