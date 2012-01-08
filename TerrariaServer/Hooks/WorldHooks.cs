using System.ComponentModel;

namespace TerrariaServer.Hooks
{
	public static class WorldHooks
	{
		public delegate void SaveWorldD(bool resettime, HandledEventArgs e);
        public delegate void StartHardModeD(HandledEventArgs e);
		public static event WorldHooks.SaveWorldD SaveWorld;
	    public static event StartHardModeD StartHardMode;
		public static bool OnSaveWorld(bool resettime)
		{
			if (WorldHooks.SaveWorld == null)
			{
				return false;
			}
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			WorldHooks.SaveWorld(resettime, handledEventArgs);
			return handledEventArgs.Handled;
		}
        public static bool OnStartHardMode()
        {
            if (StartHardMode == null)
                return false;
            HandledEventArgs handledEventArgs = new HandledEventArgs();
            WorldHooks.StartHardMode(handledEventArgs);
            return handledEventArgs.Handled;
        }
	}
}
