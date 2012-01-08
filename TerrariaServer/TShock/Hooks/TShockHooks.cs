using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TShock.Hooks;
using TShock.Hooks.NPC;
using TShock.Hooks.Player;
using Terraria;

namespace TShock.Hooks
{
	internal class TShockHooks : IHooks
	{
		internal TShockHooks()
		{
			PlayerHooks = new PlayerHooks();
            NpcHooks = new NpcHooks();
		}
		public PlayerHooks PlayerHooks { get; protected set; }

	    public NpcHooks NpcHooks{get; protected set;}
	}
}
