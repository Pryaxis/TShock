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
			Update = new HandlerList<HandledEventArgs>();

			TerrariaServer.Hooks.GameHooks.Update += GameHooks_Update;
			TerrariaServer.Hooks.GameHooks.PostUpdate += GameHooks_PostUpdate;
			TerrariaServer.Hooks.GameHooks.Initialize += GameHooks_Initialize;
			TerrariaServer.Hooks.GameHooks.PostInitialize += GameHooks_PostInitialize;
		}

		void GameHooks_Update(HandledEventArgs args)
		{
			var e = new HandledEventArgs();
			Update.Invoke(this, e);
			args.Handled = e.Handled;
		}

		void GameHooks_PostUpdate(HandledEventArgs args)
		{
			var e = new HandledEventArgs();
			PostUpdate.Invoke(this, e);
			args.Handled = e.Handled;
		}
		
		void GameHooks_Initialize(HandledEventArgs args)
		{
			var e = new HandledEventArgs();
			Initialize.Invoke(this, e);
			args.Handled = e.Handled;
		}

		void GameHooks_PostInitialize(HandledEventArgs args)
		{
			var e = new HandledEventArgs();
			PostInitialize.Invoke(this, e);
			args.Handled = e.Handled;
		}

		/// <summary>
		/// Called Before Update
		/// </summary>
		public HandlerList<HandledEventArgs> Update { get; set; }
		/// <summary>
		/// Called After Update
		/// </summary>
		public HandlerList<HandledEventArgs> PostUpdate { get; set; }
		/// <summary>
		/// Called before Initializing
		/// </summary>
		public HandlerList<HandledEventArgs> Initialize { get; set; }
		/// <summary>
		/// Called After Initializing
		/// </summary>
		public HandlerList<HandledEventArgs> PostInitialize { get; set; }
	}
}
