using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShock;
using TShock.Hooks.Player;

namespace TShockCore
{
	internal class TShockCorePlugin : Plugin
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
			get { return new Version(1, 10); }
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
			Hooks.PlayerHooks.Join.Register(OnJoin, HandlerPriority.High);
			Hooks.PlayerHooks.Greet.Register(OnGreet); 
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Hooks.PlayerHooks.Join -= OnJoin;
				Hooks.PlayerHooks.Greet -= OnGreet;
			}
			base.Dispose(disposing);
		}


		void OnGreet(object sender, PlayerEventArgs e)
		{
			Game.SendMessage(e.Player, "Hi", Color.Red);
		}

		void OnJoin(object sender, PlayerEventArgs e)
		{
			Console.WriteLine(e.Player.Name + " Joined");
		}
	}
}
