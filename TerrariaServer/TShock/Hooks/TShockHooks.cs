using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TShock.Hooks;
using TShock.Hooks.Player;
using Terraria;

namespace TShock.Hooks
{
	internal class TShockHooks : IHooks
	{
		internal TShockHooks()
		{
			PlayerHooks = new PlayerHooks();
			TerrariaServer.Hooks.ServerHooks.Join += ServerHooks_Join;
		}

		void ServerHooks_Join(int arg1, HandledEventArgs arg2)
		{
			var e = new PlayerEventArgs(new TShockPlayer(arg1));
			PlayerHooks.Join.Invoke(this, e);
			arg2.Handled = e.Handled;
		}

		public PlayerHooks PlayerHooks { get; protected set; }
	}
}
