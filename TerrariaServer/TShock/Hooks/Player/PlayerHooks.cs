using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TShock.Hooks.Player
{
	public class PlayerHooks
	{
		internal PlayerHooks()
		{
			Join = new HandlerList<PlayerEventArgs>();
			Greet = new HandlerList<PlayerEventArgs>();

			TerrariaServer.Hooks.ServerHooks.Join += ServerHooks_Join;
			TerrariaServer.Hooks.NetHooks.GreetPlayer += NetHooks_GreetPlayer;
		}

		void NetHooks_GreetPlayer(int who, HandledEventArgs arg2)
		{
			var e = new PlayerEventArgs(new TShockPlayer(who));
			Greet.Invoke(this, e);
			arg2.Handled = e.Handled;
		}

		void ServerHooks_Join(int arg1, HandledEventArgs arg2)
		{
			var e = new PlayerEventArgs(new TShockPlayer(arg1));
			Join.Invoke(this, e);
			arg2.Handled = e.Handled;
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
