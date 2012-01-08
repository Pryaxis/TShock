using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShock;
using TShock.Hooks.Player;

namespace LuaPlugin
{
	public class LuaPlugin : Plugin
	{
		public override string Name
		{
			get { return "Lua Loader"; }
		}

		public override Version Version
		{
			get { return new Version(1, 0); }
		}

		public override Version ApiVersion
		{
			get { return new Version(1, 10); }
		}

		public override string Author
		{
			get { return "The Nyx Team"; }
		}

		public override string Description
		{
			get { return "Adds Lua support for Terraria API"; }
		}

		public override bool Enabled
		{
			get;
			set;
		}

		public override void Initialize()
		{
		}
	}
}
