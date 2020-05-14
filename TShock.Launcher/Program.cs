using System;
using System.Linq;
using System.Reflection;
using OTAPI;
using OTAPI.Shims.TShock;
using TerrariaApi.Server;

namespace TShock.Launcher
{
	class Program
	{
		static void Main(string[] args)
		{
			ServerApi.PluginLoader = new PluginLoader(args.Any(a => a == "-ignoreversion"));

			Launch.Start(args);
		}
	}
}
