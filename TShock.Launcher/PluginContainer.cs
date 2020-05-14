using TerrariaApi.Server;

namespace TShock.Launcher
{
	public class PluginContainer : IPluginContainer
	{
		public TerrariaPlugin Plugin { get; protected set; }
		public bool Initialized { get; protected set; }
		public bool Dll { get; set; }

		public PluginContainer(TerrariaPlugin plugin): this(plugin, true)
		{
		}

		public PluginContainer(TerrariaPlugin plugin, bool dll)
		{
			Plugin = plugin;
			Initialized = false;
			Dll = dll;
		}

		public void Initialize()
		{
			Plugin.Initialize();
			Initialized = true;
		}

		public void DeInitialize()
		{
			Initialized = false;
		}

		public void Dispose()
		{
			Plugin.Dispose();
		}
	}
}
