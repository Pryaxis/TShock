using System;
using System.ComponentModel;
using Terraria;
using TerrariaServer.Hooks.Classes;
using TShock.Hooks.Game;

namespace TerrariaServer.Hooks
{
	public static class GameHooks
	{
		public delegate void UpdateD(HandledEventArgs e);
		public delegate void PostUpdateD(HandledEventArgs e);
		public delegate void InitializeD(HandledEventArgs e);
		public delegate void PostInitializeD(HandledEventArgs e);

		public static event UpdateD Update;
		public static event PostUpdateD PostUpdate;
		public static event InitializeD Initialize;
		public static event PostInitializeD PostInitialize;

		public static void OnUpdate(bool pre)
		{
			if (pre)
			{
				if (GameHooks.Update != null)
				{
					GameHooks.Update(new HandledEventArgs());
					return;
				}
			}
			else
			{
				if (GameHooks.PostUpdate != null)
				{
					GameHooks.PostUpdate(new HandledEventArgs());
				}
			}
		}
		public static void OnInitialize(bool pre)
		{
			if (pre)
			{
				if (GameHooks.Initialize != null)
				{
					GameHooks.Initialize(new HandledEventArgs());
					return;
				}
			}
			else
			{
				if (GameHooks.PostInitialize != null)
				{
					GameHooks.PostInitialize(new HandledEventArgs());
				}
			}
		}
	}
}
