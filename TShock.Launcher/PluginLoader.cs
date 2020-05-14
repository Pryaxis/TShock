using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using Terraria;
using TerrariaApi.Server;
#if NETCOREAPP
using System.Runtime.Loader;
#endif

namespace TShock.Launcher
{
	public class PluginLoader : IPluginLoader
	{
		public const string PluginsPath = "ServerPlugins";

		public string ServerPluginsDirectoryPath { get; private set; }

		public static bool IgnoreVersion { get; set; }

		private static Logger Log = LogManager.GetLogger("PluginLoader");

		private readonly Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

		public PluginLoader(bool ignoreVersion)
		{
			IgnoreVersion = ignoreVersion;

			ServerPluginsDirectoryPath = Path.Combine(Environment.CurrentDirectory, PluginsPath);

			if (!Directory.Exists(ServerPluginsDirectoryPath))
			{
				string lcDirectoryPath =
					Path.Combine(Path.GetDirectoryName(ServerPluginsDirectoryPath), PluginsPath.ToLower());

				if (Directory.Exists(lcDirectoryPath))
				{
					Directory.Move(lcDirectoryPath, ServerPluginsDirectoryPath);
					Log.Warn("Case sensitive filesystem detected, serverplugins directory has been renamed.");
				}
				else
				{
					Directory.CreateDirectory(ServerPluginsDirectoryPath);
				}
			}

			// Add assembly resolver instructing it to use the server plugins directory as a search path.
			// TODO: Either adding the server plugins directory to PATH or as a privatePath node in the assembly config should do too.
			AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveHandler;
		}

		public ICollection<IPluginContainer> LoadPlugins()
		{
			string ignoredPluginsFilePath = Path.Combine(ServerPluginsDirectoryPath, "ignoredplugins.txt");

			var ignoredFiles = new List<string>();
			if (File.Exists(ignoredPluginsFilePath))
				ignoredFiles.AddRange(File.ReadAllLines(ignoredPluginsFilePath));

			var fileInfos = new DirectoryInfo(ServerPluginsDirectoryPath).GetFiles("*.dll").ToList();
			fileInfos.AddRange(new DirectoryInfo(ServerPluginsDirectoryPath).GetFiles("*.dll-plugin"));

			var plugins = new List<IPluginContainer>();
			plugins.Add(LoadTShockAPI());

			foreach (var fileInfo in fileInfos)
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfo.Name);
				if (ignoredFiles.Contains(fileNameWithoutExtension))
				{
					Log.Debug($"{fileNameWithoutExtension} was ignored from being loaded.");
					continue;
				}

				if (LoadPlugin(fileInfo) is IPluginContainer plugin)
				{
					plugins.Add(plugin);
				}
			}

			IOrderedEnumerable<IPluginContainer> orderedPluginSelector =
				from x in plugins
				orderby x.Plugin.Order, x.Plugin.Name
				select x;

			foreach (var current in orderedPluginSelector)
			{
				try
				{
					current.Initialize();
				}
				catch (Exception ex)
				{
					// Broken plugins better stop the entire server init.
					throw new InvalidOperationException(string.Format(
						"Plugin \"{0}\" has thrown an exception during initialization.", current.Plugin.Name), ex);
				}
				Log.Info($"Plugin {current.Plugin.Name} v{current.Plugin.Version} (by {current.Plugin.Author}) initiated.");
			}

			return plugins;
		}

		private IPluginContainer LoadTShockAPI()
		{
			return new PluginContainer(new TShockAPI.TShock(Main.instance));
		}

		private IPluginContainer LoadPlugin(FileInfo fileInfo)
		{
			var fileNameWithoutExtension = $"{Path.Combine(fileInfo.Directory.FullName, fileInfo.Name)}";
			// The plugin assembly might have been resolved by another plugin assembly already, so no use to
			// load it again, but we do still have to verify it and create plugin instances.
			if (!loadedAssemblies.TryGetValue(fileNameWithoutExtension, out var assembly))
			{
				try
				{
#if NETCOREAPP
					using (var ms = new MemoryStream())
					{
						ms.Write(File.ReadAllBytes(fileInfo.FullName));
						ms.Seek(0, SeekOrigin.Begin);
						assembly = new PluginLoadContext(fileInfo.FullName).LoadFromStream(ms);
					}
#else
					assembly = Assembly.Load(File.ReadAllBytes(fileInfo.FullName));
#endif
				}
				catch (BadImageFormatException) { }
				catch (Exception e)
				{
					Log.Error($"Unable to load plugin assembly \"{fileInfo.Name}\", Error: {e}");
					return null;
				}

				if (!InvalidateAssembly(assembly, fileInfo.Name))
					return null;

				foreach (var type in assembly.GetExportedTypes())
				{
					if (!type.IsSubclassOf(typeof(TerrariaPlugin)) || !type.IsPublic || type.IsAbstract)
						continue;
					if (!IgnoreVersion && type.GetCustomAttribute<ApiVersionAttribute>() is ApiVersionAttribute attribute)
					{
						var apiVersion = attribute.ApiVersion;
						if (apiVersion.Major != ServerApi.ApiVersion.Major
						    || apiVersion.Minor != ServerApi.ApiVersion.Minor)
						{
							Log.Warn(
								$"Plugin \"{type.FullName}\" is designed for a different Server API version ({apiVersion.ToString(2)}) and was ignored.");
							continue;
						}
					}

					TerrariaPlugin pluginInstance;
					try
					{
						pluginInstance = (TerrariaPlugin) Activator.CreateInstance(type, Main.instance);
					}
					catch (Exception ex)
					{
						// Broken plugins better stop the entire server init.
						throw new InvalidOperationException(
							$"Could not create an instance of plugin class \"{type.FullName}\".", ex);
					}

					return new PluginContainer(pluginInstance);
				}

				loadedAssemblies.Add(fileNameWithoutExtension, assembly);
			}

			return null;
		}

		public void UnloadPlugin(IPluginContainer plugin)
		{
			plugin.DeInitialize();
			plugin.Dispose();
		}

		// Many types have changed with 1.14 and thus we won't even be able to check the ApiVersionAttribute of
		// plugin classes of assemblies targeting a TerrariaServer prior 1.14 as they can not be loaded at all.
		// We work around this by checking the referenced assemblies, if we notice a reference to the old
		// TerrariaServer assembly, we expect the plugin assembly to be outdated.
		private static bool InvalidateAssembly(Assembly assembly, string fileName)
		{
			AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
			AssemblyName terrariaServerReference = referencedAssemblies.FirstOrDefault(an => an.Name == "TerrariaServer");
			if (terrariaServerReference != null && terrariaServerReference.Version == new Version(0, 0, 0, 0))
			{
				Log.Warn($"Plugin assembly \"{fileName}\" was compiled for a Server API version prior 1.14 and was ignored.");
				return false;
			}

			return true;
		}


		private Assembly AssemblyResolveHandler(object sender, ResolveEventArgs args)
		{
			string fileName = args.Name.Split(',')[0];
			string path = Path.Combine(ServerPluginsDirectoryPath, fileName + ".dll");
			try
			{
				if (File.Exists(path))
				{
					Assembly assembly;
					if (!loadedAssemblies.TryGetValue(fileName, out assembly))
					{
						assembly = Assembly.Load(File.ReadAllBytes(path));
						loadedAssemblies.Add(fileName, assembly);
					}
					return assembly;
				}
			}
			catch (Exception) { }
			return null;
		}
	}
}
