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
			Greet = new HandlerList<PlayerEventArgs>();
		}

		/// <summary>
		/// Called when the player first connects. They are not fully in the game yet, for that see Greet.
		/// </summary>
		public HandlerList<PlayerEventArgs> Join { get; set; }
		/// <summary>
		/// Called when the player is actually loaded into the game.
		/// </summary>
		public HandlerList<PlayerEventArgs> Greet { get; set; }
	}
}
