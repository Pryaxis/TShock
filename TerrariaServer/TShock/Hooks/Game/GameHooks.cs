using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TerrariaServer.Hooks.Classes;

namespace TShock.Hooks.Game
{
	public class GameHooks
	{
		internal GameHooks()
		{
			Update = new HandlerList<GameEventArgs>();

			TerrariaServer.Hooks.GameHooks.Update += GameHooks_Update;
			TerrariaServer.Hooks.GameHooks.PostUpdate += GameHooks_PostUpdate;
			TerrariaServer.Hooks.GameHooks.Initialize += GameHooks_Initialize;
			TerrariaServer.Hooks.GameHooks.PostInitialize += GameHooks_PostInitialize;
		}

		void GameHooks_Update(GameEventArgs args)
		{
			var e = new GameEventArgs();
			Update.Invoke(this, e);
			args.Handled = e.Handled;
		}

		void GameHooks_PostUpdate(GameEventArgs args)
		{
			var e = new GameEventArgs();
			PostUpdate.Invoke(this, e);
			args.Handled = e.Handled;
		}
		
		void GameHooks_Initialize(GameEventArgs args)
		{
			var e = new GameEventArgs();
			Initialize.Invoke(this, e);
			args.Handled = e.Handled;
		}

		void GameHooks_PostInitialize(GameEventArgs args)
		{
			var e = new GameEventArgs();
			PostInitialize.Invoke(this, e);
			args.Handled = e.Handled;
		}

		/// <summary>
		/// Called Before Update
		/// </summary>
		public HandlerList<GameEventArgs> Update { get; set; }
		/// <summary>
		/// Called After Update
		/// </summary>
		public HandlerList<GameEventArgs> PostUpdate { get; set; }
		/// <summary>
		/// Called Before Update
		/// </summary>
		public HandlerList<GameEventArgs> Initialize { get; set; }
		/// <summary>
		/// Called After Update
		/// </summary>
		public HandlerList<GameEventArgs> PostInitialize { get; set; }
	}
}
