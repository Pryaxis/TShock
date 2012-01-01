using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Terraria
{
    internal class ProgramServer
    {
        public const string PluginsPath = "ServerPlugins";
        public static readonly Version ApiVersion = new Version(1, 10, 0, 3);
        public static List<PluginContainer> Plugins = new List<PluginContainer>();
        public static Dictionary<string, Assembly> LoadedAssemblies = new Dictionary<string, Assembly>();
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
            if (!Directory.Exists("ServerPlugins"))
            {
                Directory.CreateDirectory("ServerPlugins");
            }
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            List<FileInfo> files = new DirectoryInfo("ServerPlugins").GetFiles("*.dll").ToList();
            files.AddRange(new DirectoryInfo("ServerPlugins").GetFiles("*.dll-plugin"));
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
                        if (type.BaseType == typeof (TerrariaPlugin)) // Mono has this as a TODO.
                        {
                            if (Compatible(type))
                            {
                                Plugins.Add(new PluginContainer((TerrariaPlugin) Activator.CreateInstance(type, new object[]
                                                                                                                    {
                                                                                                                        main
                                                                                                                    })));
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
                        var exception = (ReflectionTypeLoadException) innerException;
                    }
                    AppendLog(fileInfo.Name, innerException);
                    Console.WriteLine("Plugin {0} failed to load", fileInfo.Name);
                }
            }
            IOrderedEnumerable<PluginContainer> orderedEnumerable =
                from x in Plugins
                orderby x.Plugin.Order , x.Plugin.Name
                select x;
            foreach (PluginContainer current in orderedEnumerable)
            {
                current.Initialize();
                Console.WriteLine("{0} v{1} ({2}) initiated.", current.Plugin.Name, current.Plugin.Version, current.Plugin.Author);
            }
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string text = args.Name.Split(new[]
                                              {
                                                  ','
                                              })[0];
            string path = Path.Combine("ServerPlugins", text + ".dll");
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

        /*private static Assembly Compile(string name, string data, bool addfail = true)
        {
            var prov = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });
            var cp = new CompilerParameters();
            cp.GenerateInMemory = true;
            cp.GenerateExecutable = false;
            cp.CompilerOptions = "/d:TERRARIA_API /unsafe";

            foreach (var a in Assemblies)
            {
                if (!cp.ReferencedAssemblies.Contains(a.Location))
                    cp.ReferencedAssemblies.Add(a.Location);
            }
            var r = prov.CompileAssemblyFromSource(cp, data);
            if (r.Errors.Count > 0)
            {
                for (int i = 0; i < r.Errors.Count; i++)
                {
                    AppendLog("Error compiling: {0}\r\nLine number: {1}, Error number: {2}, Error text: {3}\r\n",
                        name, r.Errors[i].Line, r.Errors[i].ErrorNumber, r.Errors[i].ErrorText);
                }
                return null;
            }
            return r.CompiledAssembly;
        }*/

        private static bool Compatible(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes(typeof (APIVersionAttribute), false);
            if (customAttributes.Length != 1)
            {
                return false;
            }
            var aPIVersionAttribute = (APIVersionAttribute) customAttributes[0];
            Version apiVersion = aPIVersionAttribute.ApiVersion;
            return apiVersion.Major == ApiVersion.Major && apiVersion.Minor == ApiVersion.Minor;
        }

        public static void DeInitialize()
        {
            foreach (PluginContainer current in Plugins)
            {
                current.DeInitialize();
            }
            foreach (PluginContainer current2 in Plugins)
            {
                current2.Dispose();
            }
        }

        public static void ReloadPlugins()
        {
            for (int index = 0; index < Plugins.Count; index++)
            {
                var p = Plugins[index];
                p.DeInitialize();
                p.Dispose();
                Plugins.RemoveAt(index);
            }
            LoadedAssemblies.Clear();
            Initialize(Game);
        }
    }
}