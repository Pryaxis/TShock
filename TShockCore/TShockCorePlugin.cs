using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShock;

namespace TShockCore
{
	internal class TShockCorePlugin : TShockPlugin
	{
		public override string Name
		{
			get { return "Core"; }
		}

		public override Version Version
		{
			get { return new Version(1, 0); }
		}

		public override Version ApiVersion
		{
			get { return new Version(1, 0); }
		}

		public override string Author
		{
			get { return "Nyx"; }
		}

		public override string Description
		{
			get { return "Core plugin for tshock"; }
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
