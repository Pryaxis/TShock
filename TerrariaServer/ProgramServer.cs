using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using TShock;

namespace Terraria
{
	internal class ProgramServer
	{
		public const string PluginsPath = "Plugins";
		public static readonly Version ApiVersion = new Version(1, 10, 0, 3);
		public static List<TShockPlugin> Plugins = new List<TShockPlugin>();
		public static Dictionary<string, Assembly> LoadedAssemblies = new Dictionary<string, Assembly>();
		public static List<object> PluginInterfaces = new List<object>();
		private static Main Game;

		private static void Main(string[] args)
		{
			try
			{
				Game = new Main();
				for (int i = 0; i < args.Length; i++)
				{
					switch (args[i].ToLower())
					{
						case "-config":
							i++;
							Game.LoadDedConfig(args[i]);
							break;
						case "-port":
							i++;
							try
							{
								int serverPort = Convert.ToInt32(args[i]);
								Netplay.serverPort = serverPort;
							}
							catch
							{
							}
							break;
						case "-world":
							i++;
							Game.SetWorld(args[i]);
							break;
						case "-worldname":
							i++;
							Game.SetWorldName(args[i]);
							break;
						case "-autoshutdown":
							Game.autoShut();
							break;
						case "-autocreate":
							i++;
							string newOpt = args[i];
							Game.autoCreate(newOpt);
							break;
						case "-ip":
							IPAddress ip;
							if (IPAddress.TryParse(args[++i], out ip))
							{
								Netplay.serverListenIP = ip;
								Console.Write("Using IP: {0}", ip);
							}
							else
								Console.WriteLine("Bad IP: {0}", args[i]);
							break;
						case "-connperip":
							int limit;
							if (int.TryParse(args[++i], out limit))
							{
								Netplay.connectionLimit = limit;
								Console.WriteLine("Each IP is now limited to {0} connections", limit);
							}
							else
								Console.WriteLine("Bad int for -connperip");
							break;
						case "-killinactivesocket":
							Netplay.killInactive = true;
							break;
					}
				}
				if (Environment.OSVersion.Platform == PlatformID.Unix)
					Terraria.Main.SavePath = "Terraria";
				else
					Terraria.Main.SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "My Games", "Terraria");
				Terraria.Main.WorldPath = Path.Combine(Terraria.Main.SavePath, "Worlds");
				Terraria.Main.PlayerPath = Path.Combine(Terraria.Main.SavePath, "Players");
				Initialize(Game);
				Game.DedServ();
				DeInitialize();
			}
			catch (Exception value)
			{
				try
				{
					using (var streamWriter = new StreamWriter("crashlog.txt", true))
					{
						streamWriter.WriteLine(DateTime.Now);
						streamWriter.WriteLine(value);
						streamWriter.WriteLine("");
					}
					Console.WriteLine("Server crash: " + DateTime.Now);
					Console.WriteLine(value);
					Console.WriteLine("");
					Console.WriteLine("Please send crashlog.txt to support@terraria.org");
				}
				catch
				{
				}
			}
		}

		public static void Initialize(Main main)
		{
			if (!Directory.Exists(PluginsPath))
			{
				Directory.CreateDirectory(PluginsPath);
			}
			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
			List<FileInfo> files = new DirectoryInfo(PluginsPath).GetFiles("*.dll").ToList();
			files.AddRange(new DirectoryInfo(PluginsPath).GetFiles("*.dll-plugin"));
			for (int i = 0; i < files.Count; i++)
			{
				FileInfo fileInfo = files[i];
				try
				{
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
					Assembly assembly;
					if (!LoadedAssemblies.TryGetValue(fileNameWithoutExtension, out assembly))
					{
						assembly = Assembly.Load(File.ReadAllBytes(fileInfo.FullName));
						LoadedAssemblies.Add(fileNameWithoutExtension, assembly);
					}
					Type[] types = assembly.GetTypes();
					for (int j = 0; j < types.Length; j++)
					{
						Type type = types[j];
						if (type.BaseType == typeof(TShockPlugin)) // Mono has this as a TODO.
						{
							var plugin = (TShockPlugin)Activator.CreateInstance(type);
							if (Compatible(plugin))
							{
								PluginInterfaces.AddRange(plugin.CreateInterfaces());
								Plugins.Add(plugin);
							}
							else
							{
								Console.WriteLine("Outdated plugin: {0} ({1})", fileInfo.Name, type);
								File.AppendAllText("ErrorLog.txt", string.Format("Outdated plugin: {0} ({1})\n", fileInfo.Name, type));
							}
						}
					}
				}
				catch (Exception innerException)
				{
					if (innerException is TargetInvocationException)
					{
						innerException = (innerException).InnerException;
					}
					else if (innerException is ReflectionTypeLoadException)
					{
						var exception = (ReflectionTypeLoadException)innerException;
					}
					AppendLog(fileInfo.Name, innerException);
					Console.WriteLine("Plugin {0} failed to load", fileInfo.Name);
				}
			}
			IOrderedEnumerable<TShockPlugin> orderedEnumerable =
				from x in Plugins
				orderby x.Order, x.Name
				select x;
			foreach (var current in orderedEnumerable)
			{
				current.SetInterfaces(PluginInterfaces);
			}
			foreach (var current in orderedEnumerable)
			{
				current.Initialize();
				Console.WriteLine("{0} v{1} ({2}) initiated.", current.Name, current.Version, current.Author);
			}
		}

		private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string text = args.Name.Split(new[]	{ ',' })[0];
			string path = Path.Combine(PluginsPath, text + ".dll");
			try
			{
				if (File.Exists(path))
				{
					Assembly assembly;
					if (!LoadedAssemblies.TryGetValue(text, out assembly))
					{
						assembly = Assembly.Load(File.ReadAllBytes(path));
						LoadedAssemblies.Add(text, assembly);
					}
					return assembly;
				}
			}
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
			return null;
		}

		private static void AppendLog(string format, params object[] args)
		{
			string text = string.Format(format, args);
			Console.WriteLine(text);
			File.AppendAllText("ErrorLog.txt", text + Environment.NewLine);
		}

		private static void AppendLog(string name, Exception e)
		{
			AppendLog("Exception while trying to load: {0}\r\n{1}\r\nStack trace:\r\n{2}\r\n", name, e.Message, e.StackTrace);
		}

		private static bool Compatible(TShockPlugin plugin)
		{
			return plugin.ApiVersion.Major == ApiVersion.Major && plugin.ApiVersion.Minor == ApiVersion.Minor;
		}

		public static void DeInitialize()
		{
			foreach (var current in Plugins)
			{
				current.Dispose();
			}
		}
	}
}