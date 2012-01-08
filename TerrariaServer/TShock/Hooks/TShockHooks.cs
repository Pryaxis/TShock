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
		}
		public PlayerHooks PlayerHooks { get; protected set; }
	}
}
