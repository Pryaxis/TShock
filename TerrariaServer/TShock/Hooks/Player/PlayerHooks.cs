using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShock.Hooks.Player
{
	public class PlayerHooks
	{
		public PlayerHooks()
		{
			Join = new HandlerList<PlayerEventArgs>();
		}

		public HandlerList<PlayerEventArgs> Join { get; set; }
	}
}
