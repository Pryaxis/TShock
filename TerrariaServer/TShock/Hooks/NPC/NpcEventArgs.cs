using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TShock;

namespace TShock.Hooks.NPC
{
    public class NpcEventArgs : HandledEventArgs
	{
		public NpcEventArgs(Terraria.NPC npc)
		{
			Npc = npc;
		}
        public Terraria.NPC Npc { get; protected set; }
	}
}
