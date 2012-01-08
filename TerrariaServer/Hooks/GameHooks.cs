using System;
using System.ComponentModel;
using Terraria;

namespace TerrariaServer.Hooks
{
	public static class GameHooks
	{
		private static bool oldGameMenu;
		public static event Action Update;
		public static event Action PostUpdate;
		public static event Action Initialize;
		public static event Action PostInitialize;
		public static event Action WorldConnect;
		public static event Action WorldDisconnect;
		public static event Action<HandledEventArgs> GetKeyState;
		public static bool IsWorldRunning
		{
			get;
			private set;
		}
		static GameHooks()
		{
			GameHooks.oldGameMenu = true;
			GameHooks.Update += new Action(GameHooks.GameHooks_Update);
		}
		public static void OnUpdate(bool pre)
		{
			if (pre)
			{
				if (GameHooks.Update != null)
				{
					GameHooks.Update();
					return;
				}
			}
			else
			{
				if (GameHooks.PostUpdate != null)
				{
					GameHooks.PostUpdate();
				}
			}
		}
		public static void OnInitialize(bool pre)
		{
			if (pre)
			{
				if (GameHooks.Initialize != null)
				{
					GameHooks.Initialize();
					return;
				}
			}
			else
			{
				if (GameHooks.PostInitialize != null)
				{
					GameHooks.PostInitialize();
				}
			}
		}
		private static void GameHooks_Update()
		{
			if (GameHooks.oldGameMenu != Main.gameMenu)
			{
				GameHooks.oldGameMenu = Main.gameMenu;
				if (Main.gameMenu)
				{
					GameHooks.OnWorldDisconnect();
				}
				else
				{
					GameHooks.OnWorldConnect();
				}
				GameHooks.IsWorldRunning = !Main.gameMenu;
			}
		}
		public static void OnWorldConnect()
		{
			if (GameHooks.WorldConnect != null)
			{
				GameHooks.WorldConnect();
			}
		}
		public static void OnWorldDisconnect()
		{
			if (GameHooks.WorldDisconnect != null)
			{
				GameHooks.WorldDisconnect();
			}
		}
		public static bool OnGetKeyState()
		{
			if (GameHooks.GetKeyState == null)
			{
				return false;
			}
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			GameHooks.GetKeyState(handledEventArgs);
			return handledEventArgs.Handled;
		}
	}
}
