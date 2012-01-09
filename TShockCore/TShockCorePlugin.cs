using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShock;
using TShock.Hooks.NPC;
using TShock.Hooks.Player;

namespace TShockCore
{
	internal class TShockCorePlugin : Plugin
	{
		public override string Name
		{
			get { return "TShock Core"; }
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
			get { return "Core plugin for TShock."; }
		}

		public override bool Enabled
		{
			get;
			set;
		}

		public override void Initialize()
		{
			Hooks.PlayerHooks.Connect.Register(OnConnect);
			Hooks.PlayerHooks.Join.Register(OnJoin, HandlerPriority.High);
			Hooks.PlayerHooks.Greet.Register(OnGreet);
			Hooks.PlayerHooks.Leave.Register(OnLeave);
			Hooks.PlayerHooks.Chat.Register(OnChat);
			Hooks.NpcHooks.Spawn.Register(OnNpcSpawn);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Hooks.PlayerHooks.Connect -= OnConnect;
				Hooks.PlayerHooks.Join -= OnJoin;
				Hooks.PlayerHooks.Greet -= OnGreet;
				Hooks.PlayerHooks.Leave -= OnLeave;
				Hooks.PlayerHooks.Chat -= OnChat;
				Hooks.NpcHooks.Spawn -= OnNpcSpawn;
			}
			base.Dispose(disposing);
		}

		void OnConnect(object sender, PlayerEventArgs e)
		{
			if (e.Player == null)
			{
				e.Handled = true;
				return;
			}
			Console.WriteLine("{0} is connecting", e.Player.IP);
		}

		void OnGreet(object sender, PlayerEventArgs e)
		{
			e.Player.SendMessage("Hi", Color.Green);
		}

		void OnJoin(object sender, PlayerEventArgs e)
		{
			Console.WriteLine(e.Player.Name + " Joined");
		}

		void OnLeave(object sender, PlayerEventArgs e)
		{
			Console.WriteLine(e.Player.Name + " has left");
		}

		void OnChat(object sender, PlayerChatEventArgs e)
		{
			var tsplr = e.Player;
			if (tsplr == null)
			{
				e.Handled = true;
				return;
			}

			if (e.Text.StartsWith("/"))
			{
				Console.WriteLine("{0} tried to execute: {1}", tsplr.Name, e.Text);
			}
			else
			{
				Console.WriteLine("{0}:{1}", tsplr.Name, e.Text);
			}
		}

		private void OnNpcSpawn(object sender, NpcEventArgs e)
		{
			Console.WriteLine(e.Npc.name);
		}
	}
}
