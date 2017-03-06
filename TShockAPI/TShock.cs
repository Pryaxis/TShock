/*
TShock, a server mod for Terraria
Copyright (C) 2011-2016 Nyx Studios (fka. The TShock Team)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using MaxMind;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Rests;
using Terraria;
using Terraria.ID;
using TerrariaApi.Server;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.ServerSideCharacters;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using TShockAPI.Sockets;

namespace TShockAPI
{
	/// <summary>
	/// This is the TShock main class. TShock is a plugin on the TerrariaServerAPI, so it extends the base TerrariaPlugin.
	/// TShock also complies with the API versioning system, and defines its required API version here.
	/// </summary>
	[ApiVersion(2, 0)]
	public class TShock : TerrariaPlugin
	{
		/// <summary>VersionNum - The version number the TerrariaAPI will return back to the API. We just use the Assembly info.</summary>
		public static readonly Version VersionNum = Assembly.GetExecutingAssembly().GetName().Version;
		/// <summary>VersionCodename - The version codename is displayed when the server starts. Inspired by software codenames conventions.</summary>
		public static readonly string VersionCodename = "Mintaka";

		/// <summary>SavePath - This is the path TShock saves its data in. This path is relative to the TerrariaServer.exe (not in ServerPlugins).</summary>
		public static string SavePath = "tshock";
		/// <summary>LogFormatDefault - This is the default log file naming format. Actually, this is the only log format, because it never gets set again.</summary>
		private const string LogFormatDefault = "yyyy-MM-dd_HH-mm-ss";
		//TODO: Set the log path in the config file.
		/// <summary>LogFormat - This is the log format, which is never set again.</summary>
		private static string LogFormat = LogFormatDefault;
		/// <summary>LogPathDefault - The default log path.</summary>
		private const string LogPathDefault = "tshock";
		/// <summary>This is the log path, which is initially set to the default log path, and then to the config file log path later.</summary>
		private static string LogPath = LogPathDefault;
		/// <summary>LogClear - Determines whether or not the log file should be cleared on initialization.</summary>
		private static bool LogClear;

		/// <summary>
		/// Set by the command line, disables the '/restart' command.
		/// </summary>
		internal static bool NoRestart;

		/// <summary>Will be set to true once Utils.StopServer() is called.</summary>
		public static bool ShuttingDown;

		/// <summary>Players - Contains all TSPlayer objects for accessing TSPlayers currently on the server</summary>
		public static TSPlayer[] Players = new TSPlayer[Main.maxPlayers];
		/// <summary>Bans - Static reference to the ban manager for accessing bans & related functions.</summary>
		public static BanManager Bans;
		/// <summary>Warps - Static reference to the warp manager for accessing the warp system.</summary>
		public static WarpManager Warps;
		/// <summary>Regions - Static reference to the region manager for accessing the region system.</summary>
		public static RegionManager Regions;
		/// <summary>Backups - Static reference to the backup manager for accessing the backup system.</summary>
		public static BackupManager Backups;
		/// <summary>Groups - Static reference to the group manager for accessing the group system.</summary>
		public static GroupManager Groups;
		/// <summary>Users - Static reference to the user manager for accessing the user database system.</summary>
		public static UserManager Users;
		/// <summary>Itembans - Static reference to the item ban system.</summary>
		public static ItemManager Itembans;
		/// <summary>ProjectileBans - Static reference to the projectile ban system.</summary>
		public static ProjectileManagager ProjectileBans;
		/// <summary>TileBans - Static reference to the tile ban system.</summary>
		public static TileManager TileBans;
		/// <summary>RememberedPos - Static reference to the remembered position manager.</summary>
		public static RememberedPosManager RememberedPos;
		/// <summary>CharacterDB - Static reference to the SSC character manager.</summary>
		public static CharacterManager CharacterDB;
		/// <summary>Config - Static reference to the config system, for accessing values set in users' config files.</summary>
		public static ConfigFile Config { get; set; }
		/// <summary>ServerSideCharacterConfig - Static reference to the server side character config, for accessing values set by users to modify SSC.</summary>
		public static ServerSideConfig ServerSideCharacterConfig;
		/// <summary>DB - Static reference to the database.</summary>
		public static IDbConnection DB;
		/// <summary>OverridePort - Determines if TShock should override the server port.</summary>
		public static bool OverridePort;
		/// <summary>Geo - Static reference to the GeoIP system which determines the location of an IP address.</summary>
		public static GeoIPCountry Geo;
		/// <summary>RestApi - Static reference to the Rest API authentication manager.</summary>
		public static SecureRest RestApi;
		/// <summary>RestManager - Static reference to the Rest API manager.</summary>
		public static RestManager RestManager;
		/// <summary>Utils - Static reference to the utilities class, which contains a variety of utility functions.</summary>
		public static Utils Utils = Utils.Instance;
		/// <summary>StatTracker - Static reference to the stat tracker, which sends some server metrics every 5 minutes.</summary>
		public static StatTracker StatTracker = new StatTracker();
		/// <summary>UpdateManager - Static reference to the update checker, which checks for updates and notifies server admins of updates.</summary>
		public static UpdateManager UpdateManager;
		/// <summary>Log - Static reference to the log system, which outputs to either SQL or a text file, depending on user config.</summary>
		public static ILog Log;
		/// <summary>instance - Static reference to the TerrariaPlugin instance.</summary>
		public static TerrariaPlugin instance;
		/// <summary>
		/// Used for implementing REST Tokens prior to the REST system starting up.
		/// </summary>
		public static Dictionary<string, SecureRest.TokenData> RESTStartupTokens = new Dictionary<string, SecureRest.TokenData>();

		/// <summary>
		/// Called after TShock is initialized. Useful for plugins that needs hooks before tshock but also depend on tshock being loaded.
		/// </summary>
		public static event Action Initialized;

		/// <summary>Version - The version required by the TerrariaAPI to be passed back for checking & loading the plugin.</summary>
		/// <value>value - The version number specified in the Assembly, based on the VersionNum variable set in this class.</value>
		public override Version Version
		{
			get { return VersionNum; }
		}

		/// <summary>Name - The plugin name.</summary>
		/// <value>value - "TShock"</value>
		public override string Name
		{
			get { return "TShock"; }
		}

		/// <summary>Author - The author of the plugin.</summary>
		/// <value>value - "The TShock Team"</value>
		public override string Author
		{
			get { return "The TShock Team"; }
		}

		/// <summary>Description - The plugin description.</summary>
		/// <value>value - "The administration modification of the future."</value>
		public override string Description
		{
			get { return "The administration modification of the future."; }
		}

		/// <summary>TShock - The constructor for the TShock plugin.</summary>
		/// <param name="game">game - The Terraria main game.</param>
		public TShock(Main game)
			: base(game)
		{
			Config = new ConfigFile();
			ServerSideCharacterConfig = new ServerSideConfig();
			ServerSideCharacterConfig.StartingInventory.Add(new NetItem(-15, 1, 0));
			ServerSideCharacterConfig.StartingInventory.Add(new NetItem(-13, 1, 0));
			ServerSideCharacterConfig.StartingInventory.Add(new NetItem(-16, 1, 0));
			Order = 0;
			instance = this;
		}

		/// <summary>Initialize - Called by the TerrariaServerAPI during initialization.</summary>
		[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		public override void Initialize()
		{
			string logFilename;
			string logPathSetupWarning;

			OTAPI.Hooks.Net.Socket.Create = () =>
			{
				//Console.WriteLine($"Creating socket {nameof(LinuxTcpSocket)}");
				return new LinuxTcpSocket();
				//return new OTAPI.Sockets.PoolSocket();
				//return new Terraria.Net.Sockets.TcpSocket();
			};
			OTAPI.Hooks.Player.Announce = (int playerId) =>
			{
				//TShock handles this
				return OTAPI.HookResult.Cancel;
			};

			Main.SettingsUnlock_WorldEvil = true;

			TerrariaApi.Reporting.CrashReporter.HeapshotRequesting += CrashReporter_HeapshotRequesting;

			try
			{
				HandleCommandLine(Environment.GetCommandLineArgs());

				if (!Directory.Exists(SavePath))
					Directory.CreateDirectory(SavePath);

				ConfigFile.ConfigRead += OnConfigRead;
				FileTools.SetupConfig();

				Main.ServerSideCharacter = ServerSideCharacterConfig.Enabled;

				//TSAPI previously would do this automatically, but the vanilla server wont
				if (Netplay.ServerIP == null)
					Netplay.ServerIP = IPAddress.Any;

				DateTime now = DateTime.Now;
				// Log path was not already set by the command line parameter?
				if (LogPath == LogPathDefault)
					LogPath = Config.LogPath;
				try
				{
					logFilename = Path.Combine(LogPath, now.ToString(LogFormat) + ".log");
					if (!Directory.Exists(LogPath))
						Directory.CreateDirectory(LogPath);
				}
				catch (Exception ex)
				{
					logPathSetupWarning =
						"Could not apply the given log path / log format, defaults will be used. Exception details:\n" + ex;

					ServerApi.LogWriter.PluginWriteLine(this, logPathSetupWarning, TraceLevel.Error);

					// Problem with the log path or format use the default
					logFilename = Path.Combine(LogPathDefault, now.ToString(LogFormatDefault) + ".log");
				}

				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			}
			catch (Exception ex)
			{
				// Will be handled by the server api and written to its crashlog.txt.
				throw new Exception("Fatal TShock initialization exception. See inner exception for details.", ex);
			}

			// Further exceptions are written to TShock's log from now on.
			try
			{
				if (Config.StorageType.ToLower() == "sqlite")
				{
					string sql = Path.Combine(SavePath, "tshock.sqlite");
					DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
				}
				else if (Config.StorageType.ToLower() == "mysql")
				{
					try
					{
						var hostport = Config.MySqlHost.Split(':');
						DB = new MySqlConnection();
						DB.ConnectionString =
							String.Format("Server={0}; Port={1}; Database={2}; Uid={3}; Pwd={4};",
								hostport[0],
								hostport.Length > 1 ? hostport[1] : "3306",
								Config.MySqlDbName,
								Config.MySqlUsername,
								Config.MySqlPassword
								);
					}
					catch (MySqlException ex)
					{
						ServerApi.LogWriter.PluginWriteLine(this, ex.ToString(), TraceLevel.Error);
						throw new Exception("MySql not setup correctly");
					}
				}
				else
				{
					throw new Exception("Invalid storage type");
				}

				if (Config.UseSqlLogs)
					Log = new SqlLog(DB, logFilename, LogClear);
				else
					Log = new TextLog(logFilename, LogClear);

				if (File.Exists(Path.Combine(SavePath, "tshock.pid")))
				{
					Log.ConsoleInfo(
						"TShock was improperly shut down. Please use the exit command in the future to prevent this.");
					File.Delete(Path.Combine(SavePath, "tshock.pid"));
				}
				File.WriteAllText(Path.Combine(SavePath, "tshock.pid"),
					Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));

				HandleCommandLinePostConfigLoad(Environment.GetCommandLineArgs());

				Backups = new BackupManager(Path.Combine(SavePath, "backups"));
				Backups.KeepFor = Config.BackupKeepFor;
				Backups.Interval = Config.BackupInterval;
				Bans = new BanManager(DB);
				Warps = new WarpManager(DB);
				Regions = new RegionManager(DB);
				Users = new UserManager(DB);
				Groups = new GroupManager(DB);
				Itembans = new ItemManager(DB);
				ProjectileBans = new ProjectileManagager(DB);
				TileBans = new TileManager(DB);
				RememberedPos = new RememberedPosManager(DB);
				CharacterDB = new CharacterManager(DB);
				RestApi = new SecureRest(Netplay.ServerIP, Config.RestApiPort);
				RestManager = new RestManager(RestApi);
				RestManager.RegisterRestfulCommands();

				var geoippath = "GeoIP.dat";
				if (Config.EnableGeoIP && File.Exists(geoippath))
					Geo = new GeoIPCountry(geoippath);

				Log.ConsoleInfo("TShock {0} ({1}) now running.", Version, VersionCodename);

				ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInit);
				ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
				ServerApi.Hooks.GameHardmodeTileUpdate.Register(this, OnHardUpdate);
				ServerApi.Hooks.GameStatueSpawn.Register(this, OnStatueSpawn);
				ServerApi.Hooks.ServerConnect.Register(this, OnConnect);
				ServerApi.Hooks.ServerJoin.Register(this, OnJoin);
				ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
				ServerApi.Hooks.ServerChat.Register(this, OnChat);
				ServerApi.Hooks.ServerCommand.Register(this, ServerHooks_OnCommand);
				ServerApi.Hooks.NetGetData.Register(this, OnGetData);
				ServerApi.Hooks.NetSendData.Register(this, NetHooks_SendData);
				ServerApi.Hooks.NetGreetPlayer.Register(this, OnGreetPlayer);
				ServerApi.Hooks.NpcStrike.Register(this, NpcHooks_OnStrikeNpc);
				ServerApi.Hooks.ProjectileSetDefaults.Register(this, OnProjectileSetDefaults);
				ServerApi.Hooks.WorldStartHardMode.Register(this, OnStartHardMode);
				ServerApi.Hooks.WorldSave.Register(this, SaveManager.Instance.OnSaveWorld);
				ServerApi.Hooks.WorldChristmasCheck.Register(this, OnXmasCheck);
				ServerApi.Hooks.WorldHalloweenCheck.Register(this, OnHalloweenCheck);
				ServerApi.Hooks.NetNameCollision.Register(this, NetHooks_NameCollision);
				ServerApi.Hooks.ItemForceIntoChest.Register(this, OnItemForceIntoChest);
				Hooks.PlayerHooks.PlayerPreLogin += OnPlayerPreLogin;
				Hooks.PlayerHooks.PlayerPostLogin += OnPlayerLogin;
				Hooks.AccountHooks.AccountDelete += OnAccountDelete;
				Hooks.AccountHooks.AccountCreate += OnAccountCreate;

				GetDataHandlers.InitGetDataHandler();
				Commands.InitCommands();

				if (Config.RestApiEnabled)
					RestApi.Start();

				Log.ConsoleInfo("AutoSave " + (Config.AutoSave ? "Enabled" : "Disabled"));
				Log.ConsoleInfo("Backups " + (Backups.Interval > 0 ? "Enabled" : "Disabled"));

				if (Initialized != null)
					Initialized();

				Log.ConsoleInfo("Welcome to TShock for Terraria. Initialization complete.");
			}
			catch (Exception ex)
			{
				Log.Error("Fatal Startup Exception");
				Log.Error(ex.ToString());
				Environment.Exit(1);
			}
		}

		protected void CrashReporter_HeapshotRequesting(object sender, EventArgs e)
		{
			foreach (TSPlayer player in TShock.Players)
			{
				player.User = null;
			}
		}

		/// <summary>Dispose - Called when disposing.</summary>
		/// <param name="disposing">disposing - If set, disposes of all hooks and other systems.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				// NOTE: order is important here
				if (Geo != null)
				{
					Geo.Dispose();
				}
				SaveManager.Instance.Dispose();

				ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInit);
				ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
				ServerApi.Hooks.GameHardmodeTileUpdate.Deregister(this, OnHardUpdate);
				ServerApi.Hooks.GameStatueSpawn.Deregister(this, OnStatueSpawn);
				ServerApi.Hooks.ServerConnect.Deregister(this, OnConnect);
				ServerApi.Hooks.ServerJoin.Deregister(this, OnJoin);
				ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
				ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
				ServerApi.Hooks.ServerCommand.Deregister(this, ServerHooks_OnCommand);
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
				ServerApi.Hooks.NetSendData.Deregister(this, NetHooks_SendData);
				ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);
				ServerApi.Hooks.NpcStrike.Deregister(this, NpcHooks_OnStrikeNpc);
				ServerApi.Hooks.ProjectileSetDefaults.Deregister(this, OnProjectileSetDefaults);
				ServerApi.Hooks.WorldStartHardMode.Deregister(this, OnStartHardMode);
				ServerApi.Hooks.WorldSave.Deregister(this, SaveManager.Instance.OnSaveWorld);
				ServerApi.Hooks.WorldChristmasCheck.Deregister(this, OnXmasCheck);
				ServerApi.Hooks.WorldHalloweenCheck.Deregister(this, OnHalloweenCheck);
				ServerApi.Hooks.NetNameCollision.Deregister(this, NetHooks_NameCollision);
				ServerApi.Hooks.ItemForceIntoChest.Deregister(this, OnItemForceIntoChest);
				TShockAPI.Hooks.PlayerHooks.PlayerPostLogin -= OnPlayerLogin;

				if (File.Exists(Path.Combine(SavePath, "tshock.pid")))
				{
					File.Delete(Path.Combine(SavePath, "tshock.pid"));
				}

				RestApi.Dispose();
				Log.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>OnPlayerLogin - Fires the PlayerLogin hook to listening plugins.</summary>
		/// <param name="args">args - The PlayerPostLoginEventArgs object.</param>
		private void OnPlayerLogin(PlayerPostLoginEventArgs args)
		{
			List<String> KnownIps = new List<string>();
			if (!string.IsNullOrWhiteSpace(args.Player.User.KnownIps))
			{
				KnownIps = JsonConvert.DeserializeObject<List<String>>(args.Player.User.KnownIps);
			}

			if (KnownIps.Count == 0)
			{
				KnownIps.Add(args.Player.IP);
			}
			else
			{
				bool last = KnownIps.Last() == args.Player.IP;
				if (!last)
				{
					if (KnownIps.Count == 100)
					{
						KnownIps.RemoveAt(0);
					}

					KnownIps.Add(args.Player.IP);
				}
			}

			args.Player.User.KnownIps = JsonConvert.SerializeObject(KnownIps, Formatting.Indented);
			Users.UpdateLogin(args.Player.User);
		}

		/// <summary>OnAccountDelete - Internal hook fired on account delete.</summary>
		/// <param name="args">args - The AccountDeleteEventArgs object.</param>
		private void OnAccountDelete(Hooks.AccountDeleteEventArgs args)
		{
			CharacterDB.RemovePlayer(args.User.ID);
		}

		/// <summary>OnAccountCreate - Internal hook fired on account creation.</summary>
		/// <param name="args">args - The AccountCreateEventArgs object.</param>
		private void OnAccountCreate(Hooks.AccountCreateEventArgs args)
		{
			CharacterDB.SeedInitialData(Users.GetUser(args.User));
		}

		/// <summary>OnPlayerPreLogin - Internal hook fired when on player pre login.</summary>
		/// <param name="args">args - The PlayerPreLoginEventArgs object.</param>
		private void OnPlayerPreLogin(Hooks.PlayerPreLoginEventArgs args)
		{
			if (args.Player.IsLoggedIn)
				args.Player.SaveServerCharacter();
		}

		/// <summary>NetHooks_NameCollision - Internal hook fired when a name collision happens.</summary>
		/// <param name="args">args - The NameCollisionEventArgs object.</param>
		private void NetHooks_NameCollision(NameCollisionEventArgs args)
		{
			string ip = Utils.GetRealIP(Netplay.Clients[args.Who].Socket.GetRemoteAddress().ToString());

			var player = Players.First(p => p != null && p.Name == args.Name && p.Index != args.Who);
			if (player != null)
			{
				if (player.IP == ip)
				{
					Netplay.Clients[player.Index].PendingTermination = true;
					args.Handled = true;
					return;
				}
				if (player.IsLoggedIn)
				{
					var ips = JsonConvert.DeserializeObject<List<string>>(player.User.KnownIps);
					if (ips.Contains(ip))
					{
						Netplay.Clients[player.Index].PendingTermination = true;
						args.Handled = true;
					}
				}
			}
		}

		/// <summary>OnItemForceIntoChest - Internal hook fired when a player quick stacks items into a chest.</summary>
		/// <param name="args">The <see cref="ForceItemIntoChestEventArgs"/> object.</param>
		private void OnItemForceIntoChest(ForceItemIntoChestEventArgs args)
		{
			if (args.Handled)
			{
				return;
			}

			if (args.Player == null)
			{
				args.Handled = true;
				return;
			}

			TSPlayer tsplr = Players[args.Player.whoAmI];
			if (tsplr == null)
			{
				args.Handled = true;
				return;
			}

			if (args.Chest != null)
			{
				if (Config.RegionProtectChests && !Regions.CanBuild((int)args.WorldPosition.X, (int)args.WorldPosition.Y, tsplr))
				{
					args.Handled = true;
					return;
				}

				if (CheckRangePermission(tsplr, args.Chest.x, args.Chest.y))
				{
					args.Handled = true;
					return;
				}
			}
		}

		/// <summary>OnXmasCheck - Internal hook fired when the XMasCheck happens.</summary>
		/// <param name="args">args - The ChristmasCheckEventArgs object.</param>
		private void OnXmasCheck(ChristmasCheckEventArgs args)
		{
			if (args.Handled)
				return;

			if (Config.ForceXmas)
			{
				args.Xmas = true;
				args.Handled = true;
			}
		}

		/// <summary>OnHalloweenCheck - Internal hook fired when the HalloweenCheck happens.</summary>
		/// <param name="args">args - The HalloweenCheckEventArgs object.</param>
		private void OnHalloweenCheck(HalloweenCheckEventArgs args)
		{
			if (args.Handled)
				return;

			if (Config.ForceHalloween)
			{
				args.Halloween = true;
				args.Handled = true;
			}
		}

		/// <summary>
		/// Handles exceptions that we didn't catch earlier in the code, or in Terraria.
		/// </summary>
		/// <param name="sender">sender - The object that sent the exception.</param>
		/// <param name="e">e - The UnhandledExceptionEventArgs object.</param>
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Log.Error(e.ExceptionObject.ToString());

			if (e.ExceptionObject.ToString().Contains("Terraria.Netplay.ListenForClients") ||
				e.ExceptionObject.ToString().Contains("Terraria.Netplay.ServerLoop"))
			{
				var sb = new List<string>();
				for (int i = 0; i < Netplay.Clients.Length; i++)
				{
					if (Netplay.Clients[i] == null)
					{
						sb.Add("Client[" + i + "]");
					}
					else if (Netplay.Clients[i].Socket == null)
					{
						sb.Add("Tcp[" + i + "]");
					}
				}
				Log.Error(string.Join(", ", sb));
			}

			if (e.IsTerminating)
			{
				if (Main.worldPathName != null && Config.SaveWorldOnCrash)
				{
					Main.ActiveWorldFileData._path += ".crash";
					SaveManager.Instance.SaveWorld();
				}
			}
		}

		/// <summary>HandleCommandLine - Handles the command line parameters passed to the server.</summary>
		/// <param name="parms">parms - The array of arguments passed in through the command line.</param>
		private void HandleCommandLine(string[] parms)
		{
			string path;
			for (int i = 0; i < parms.Length; i++)
			{
				switch (parms[i].ToLower())
				{
					case "-configpath":
						{
							path = parms[++i];
							if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
							{
								SavePath = path;
								ServerApi.LogWriter.PluginWriteLine(this, "Config path has been set to " + path, TraceLevel.Info);
							}
							break;
						}
					case "-worldpath":
						{
							path = parms[++i];
							if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
							{
								Main.WorldPath = path;
								ServerApi.LogWriter.PluginWriteLine(this, "World path has been set to " + path, TraceLevel.Info);
							}
							break;
						}
					case "-logpath":
						{
							path = parms[++i];
							if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
							{
								LogPath = path;
								ServerApi.LogWriter.PluginWriteLine(this, "Log path has been set to " + path, TraceLevel.Info);
							}
							break;
						}
					case "-logformat":
						{
							LogFormat = parms[++i];
							break;
						}
					case "-logclear":
						{
							bool.TryParse(parms[++i], out LogClear);
							break;
						}
					case "-dump":
						{
							Utils.PrepareLangForDump();
							Lang.setLang(true);
							ConfigFile.DumpDescriptions();
							Permissions.DumpDescriptions();
							ServerSideConfig.DumpDescriptions();
							RestManager.DumpDescriptions();
							Utils.DumpBuffs("BuffList.txt");
							Utils.DumpItems("Items-1_0.txt", -48, 235);
							Utils.DumpItems("Items-1_1.txt", 235, 604);
							Utils.DumpItems("Items-1_2.txt", 604, 2749);
							Utils.DumpItems("Items-1_3.txt", 2749, Main.maxItemTypes);
							Utils.DumpNPCs("NPCs.txt");
							Utils.DumpProjectiles("Projectiles.txt");
							Utils.DumpPrefixes("Prefixes.txt");
							Environment.Exit(1);
							break;
						}
					case "-config":
						{
							string filePath = parms[++i];
							ServerApi.LogWriter.PluginWriteLine(this, string.Format("Loading dedicated config file: {0}", filePath), TraceLevel.Verbose);
							Main.instance.LoadDedConfig(filePath);
							break;
						}
					case "-port":
						{
							int serverPort;
							if (int.TryParse(parms[++i], out serverPort))
							{
								Netplay.ListenPort = serverPort;
								ServerApi.LogWriter.PluginWriteLine(this, string.Format("Listening on port {0}.", serverPort), TraceLevel.Verbose);
							}
							else
							{
								// The server should not start up if this argument is invalid.
								throw new InvalidOperationException("Invalid value given for command line argument \"-ip\".");
							}

							break;
						}
					case "-worldname":
						{
							string worldName = parms[++i];
							Main.instance.SetWorldName(worldName);
							ServerApi.LogWriter.PluginWriteLine(this, string.Format("World name will be overridden by: {0}", worldName), TraceLevel.Verbose);

							break;
						}
					case "-autoshutdown":
						{
							Main.instance.EnableAutoShutdown();
							break;
						}
					case "-autocreate":
						{
							string newOpt = parms[++i];
							Main.instance.autoCreate(newOpt);
							break;
						}
					case "-ip":
						{
							IPAddress ip;
							if (IPAddress.TryParse(parms[++i], out ip))
							{
								Netplay.ServerIP = ip;
								ServerApi.LogWriter.PluginWriteLine(this, string.Format("Listening on IP {0}.", ip), TraceLevel.Verbose);
							}
							else
							{
								// The server should not start up if this argument is invalid.
								throw new InvalidOperationException("Invalid value given for command line argument \"-ip\".");
							}

							break;
						}
					case "-connperip":
						{
							int limit;
							if (int.TryParse(parms[++i], out limit))
							{
								/* Todo - Requires an OTAPI modification
								Netplay.MaxConnections = limit;
								ServerApi.LogWriter.PluginWriteLine(this, string.Format(
									"Connections per IP have been limited to {0} connections.", limit), TraceLevel.Verbose);*/
								ServerApi.LogWriter.PluginWriteLine(this, "\"-connperip\" is not supported in this version of TShock.", TraceLevel.Verbose);
							}
							else
								ServerApi.LogWriter.PluginWriteLine(this, "Invalid value given for command line argument \"-connperip\".", TraceLevel.Warning);

							break;
						}
					case "-killinactivesocket":
						{
							//							Netplay.killInactive = true;
							ServerApi.LogWriter.PluginWriteLine(this, "The argument -killinactivesocket is no longer present in Terraria.", TraceLevel.Warning);
							break;
						}
					case "-lang":
						{
							int langIndex;
							if (int.TryParse(parms[++i], out langIndex))
							{
								Lang.lang = langIndex;
								ServerApi.LogWriter.PluginWriteLine(this, string.Format("Language index set to {0}.", langIndex), TraceLevel.Verbose);
							}
							else
								ServerApi.LogWriter.PluginWriteLine(this, "Invalid value given for command line argument \"-lang\".", TraceLevel.Warning);

							break;
						}
					case "--provider-token":
						{
							StatTracker.ProviderToken = parms[++i];
							break;
						}
					case "--stats-optout":
						{
							StatTracker.OptOut = true;
							break;
						}
					case "--no-restart":
						{
							TShock.NoRestart = true;
							break;
						}
				}
			}
		}

		/// <summary>HandleCommandLinePostConfigLoad - Handles additional command line options after the config file is read.</summary>
		/// <param name="parms">parms - The array of arguments passed in through the command line.</param>
		public static void HandleCommandLinePostConfigLoad(string[] parms)
		{
			for (int i = 0; i < parms.Length; i++)
			{
				switch (parms[i].ToLower())
				{
					case "-port":
						int port = Convert.ToInt32(parms[++i]);
						Netplay.ListenPort = port;
						Config.ServerPort = port;
						OverridePort = true;
						Log.ConsoleInfo("Port overridden by startup argument. Set to " + port);
						break;
					case "-rest-token":
						string token = Convert.ToString(parms[++i]);
						RESTStartupTokens.Add(token, new SecureRest.TokenData { Username = "null", UserGroupName = "superadmin" });
						Console.WriteLine("Startup parameter overrode REST token.");
						break;
					case "-rest-enabled":
						Config.RestApiEnabled = Convert.ToBoolean(parms[++i]);
						Console.WriteLine("Startup parameter overrode REST enable.");
						break;
					case "-rest-port":
						Config.RestApiPort = Convert.ToInt32(parms[++i]);
						Console.WriteLine("Startup parameter overrode REST port.");
						break;
					case "-maxplayers":
					case "-players":
						Config.MaxSlots = Convert.ToInt32(parms[++i]);
						Console.WriteLine("Startup parameter overrode maximum player slot configuration value.");
						break;
				}
			}
		}

		/// <summary>AuthToken - The auth token used by the /auth system to grant temporary superadmin access to new admins.</summary>
		public static int AuthToken = -1;
		private string _cliPassword = null;

		/// <summary>OnPostInit - Fired when the server loads a map, to perform world specific operations.</summary>
		/// <param name="args">args - The EventArgs object.</param>
		private void OnPostInit(EventArgs args)
		{
			SetConsoleTitle(false);

			//This is to prevent a bug where a CLI-defined password causes packets to be
			//sent in an unexpected order, resulting in clients being unable to connect
			if (!string.IsNullOrEmpty(Netplay.ServerPassword))
			{
				//CLI defined password overrides a config password
				_cliPassword = Netplay.ServerPassword;
				Netplay.ServerPassword = "";
				Config.ServerPassword = _cliPassword;
			}

			// Disable the auth system if "auth.lck" is present or a superadmin exists
			if (File.Exists(Path.Combine(SavePath, "auth.lck")) || Users.GetUsers().Exists(u => u.Group == new SuperAdminGroup().Name))
			{
				AuthToken = 0;

				if (File.Exists(Path.Combine(SavePath, "authcode.txt")))
				{
					Log.ConsoleInfo("A superadmin account has been detected in the user database, but authcode.txt is still present.");
					Log.ConsoleInfo("TShock will now disable the auth system and remove authcode.txt as it is no longer needed.");
					File.Delete(Path.Combine(SavePath, "authcode.txt"));
				}

				if (!File.Exists(Path.Combine(SavePath, "auth.lck")))
				{
					// This avoids unnecessary database work, which can get ridiculously high on old servers as all users need to be fetched
					File.Create(Path.Combine(SavePath, "auth.lck"));
				}
			}
			else if (!File.Exists(Path.Combine(SavePath, "authcode.txt")))
			{
				var r = new Random((int)DateTime.Now.ToBinary());
				AuthToken = r.Next(100000, 10000000);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("TShock Notice: To become SuperAdmin, join the game and type {0}auth {1}", Commands.Specifier, AuthToken);
				Console.WriteLine("This token will display until disabled by verification. ({0}auth)", Commands.Specifier);
				Console.ResetColor();
				File.WriteAllText(Path.Combine(SavePath, "authcode.txt"), AuthToken.ToString());
			}
			else
			{
				AuthToken = Convert.ToInt32(File.ReadAllText(Path.Combine(SavePath, "authcode.txt")));
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("TShock Notice: authcode.txt is still present, and the AuthToken located in that file will be used.");
				Console.WriteLine("To become superadmin, join the game and type {0}auth {1}", Commands.Specifier, AuthToken);
				Console.WriteLine("This token will display until disabled by verification. ({0}auth)", Commands.Specifier);
				Console.ResetColor();
			}

			Regions.Reload();
			Warps.ReloadWarps();

			ComputeMaxStyles();
			FixChestStacks();

			Utils.UpgradeMotD();

			if (Config.UseServerName)
			{
				Main.worldName = Config.ServerName;
			}

			UpdateManager = new UpdateManager();
			StatTracker.Start();
		}

		/// <summary>ComputeMaxStyles - Computes the max styles...</summary>
		private void ComputeMaxStyles()
		{
			var item = new Item();
			for (int i = 0; i < Main.maxItemTypes; i++)
			{
				item.netDefaults(i);
				if (item.placeStyle > 0)
				{
					if (GetDataHandlers.MaxPlaceStyles.ContainsKey(item.createTile))
					{
						if (item.placeStyle > GetDataHandlers.MaxPlaceStyles[item.createTile])
							GetDataHandlers.MaxPlaceStyles[item.createTile] = item.placeStyle;
					}
					else
						GetDataHandlers.MaxPlaceStyles.Add(item.createTile, item.placeStyle);
				}
			}
		}

		/// <summary>FixChestStacks - Verifies that each stack in each chest is valid and not over the max stack count.</summary>
		private void FixChestStacks()
		{
			if (Config.IgnoreChestStacksOnLoad)
				return;

			foreach (Chest chest in Main.chest)
			{
				if (chest != null)
				{
					foreach (Item item in chest.item)
					{
						if (item != null && item.stack > item.maxStack)
							item.stack = item.maxStack;
					}
				}
			}
		}

		/// <summary>LastCheck - Used to keep track of the last check for basically all time based checks.</summary>
		private DateTime LastCheck = DateTime.UtcNow;

		/// <summary>LastSave - Used to keep track of SSC save intervals.</summary>
		private DateTime LastSave = DateTime.UtcNow;

		/// <summary>OnUpdate - Called when ever the server ticks.</summary>
		/// <param name="args">args - EventArgs args</param>
		private void OnUpdate(EventArgs args)
		{
			if (Backups.IsBackupTime)
				Backups.Backup();
			//call these every second, not every update
			if ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1)
			{
				OnSecondUpdate();
				LastCheck = DateTime.UtcNow;
			}

			if (Main.ServerSideCharacter && (DateTime.UtcNow - LastSave).TotalMinutes >= ServerSideCharacterConfig.ServerSideCharacterSave)
			{
				foreach (TSPlayer player in Players)
				{
					// prevent null point exceptions
					if (player != null && player.IsLoggedIn && !player.IgnoreActionsForClearingTrashCan)
					{

						CharacterDB.InsertPlayerData(player);
					}
				}
				LastSave = DateTime.UtcNow;
			}
		}

		/// <summary>OnSecondUpdate - Called effectively every second for all time based checks.</summary>
		private void OnSecondUpdate()
		{
			DisableFlags flags = Config.DisableSecondUpdateLogs ? DisableFlags.WriteToConsole : DisableFlags.WriteToLogAndConsole;

			if (Config.ForceTime != "normal")
			{
				switch (Config.ForceTime)
				{
					case "day":
						TSPlayer.Server.SetTime(true, 27000.0);
						break;
					case "night":
						TSPlayer.Server.SetTime(false, 16200.0);
						break;
				}
			}

			foreach (TSPlayer player in Players)
			{
				if (player != null && player.Active)
				{
					if (player.TilesDestroyed != null)
					{
						if (player.TileKillThreshold >= Config.TileKillThreshold)
						{
							player.Disable("Reached TileKill threshold.", flags);
							TSPlayer.Server.RevertTiles(player.TilesDestroyed);
							player.TilesDestroyed.Clear();
						}
					}
					if (player.TileKillThreshold > 0)
					{
						player.TileKillThreshold = 0;
						//We don't want to revert the entire map in case of a disable.
						lock (player.TilesDestroyed)
							player.TilesDestroyed.Clear();
					}

					if (player.TilesCreated != null)
					{
						if (player.TilePlaceThreshold >= Config.TilePlaceThreshold)
						{
							player.Disable("Reached TilePlace threshold", flags);
							lock (player.TilesCreated) {
								TSPlayer.Server.RevertTiles(player.TilesCreated);
								player.TilesCreated.Clear();
							}
						}
					}
					if (player.TilePlaceThreshold > 0)
					{
						player.TilePlaceThreshold = 0;
					}

					if (player.RecentFuse > 0)
						player.RecentFuse--;

					if ((Main.ServerSideCharacter) && (player.TPlayer.SpawnX > 0) && (player.sX != player.TPlayer.SpawnX))
					{
						player.sX = player.TPlayer.SpawnX;
						player.sY = player.TPlayer.SpawnY;
					}

					if ((Main.ServerSideCharacter) && (player.sX > 0) && (player.sY > 0) && (player.TPlayer.SpawnX < 0))
					{
						player.TPlayer.SpawnX = player.sX;
						player.TPlayer.SpawnY = player.sY;
					}

					if (player.RPPending > 0)
					{
						if (player.RPPending == 1)
						{
							var pos = RememberedPos.GetLeavePos(player.Name, player.IP);
							player.Teleport(pos.X * 16, pos.Y * 16);
							player.RPPending = 0;
						}
						else
						{
							player.RPPending--;
						}
					}

					if (player.TileLiquidThreshold >= Config.TileLiquidThreshold)
					{
						player.Disable("Reached TileLiquid threshold", flags);
					}
					if (player.TileLiquidThreshold > 0)
					{
						player.TileLiquidThreshold = 0;
					}

					if (player.ProjectileThreshold >= Config.ProjectileThreshold)
					{
						player.Disable("Reached projectile threshold", flags);
					}
					if (player.ProjectileThreshold > 0)
					{
						player.ProjectileThreshold = 0;
					}

					if (player.PaintThreshold >= Config.TilePaintThreshold)
					{
						player.Disable("Reached paint threshold", flags);
					}
					if (player.PaintThreshold > 0)
					{
						player.PaintThreshold = 0;
					}

					if (player.HealOtherThreshold >= TShock.Config.HealOtherThreshold)
					{
						player.Disable("Reached HealOtherPlayer threshold", flags);
					}
					if (player.HealOtherThreshold > 0)
					{
						player.HealOtherThreshold = 0;
					}

					if (player.RespawnTimer > 0 && --player.RespawnTimer == 0 && player.Difficulty != 2)
					{
						player.Spawn();
					}

					if (Main.ServerSideCharacter && !player.IsLoggedIn)
					{
						if (CheckIgnores(player))
						{
							player.Disable(flags: flags);
						}
						else if (Itembans.ItemIsBanned(player.TPlayer.inventory[player.TPlayer.selectedItem].name, player))
						{
							player.Disable($"holding banned item: {player.TPlayer.inventory[player.TPlayer.selectedItem].name}", flags);
							player.SendErrorMessage($"You are holding a banned item: {player.TPlayer.inventory[player.TPlayer.selectedItem].name}");
						}
					}
					else if (!Main.ServerSideCharacter || (Main.ServerSideCharacter && player.IsLoggedIn))
					{
						string check = "none";
						foreach (Item item in player.TPlayer.inventory)
						{
							if (!player.HasPermission(Permissions.ignorestackhackdetection) && (item.stack > item.maxStack || item.stack < 0) &&
								item.type != 0)
							{
								check = "Remove item " + item.name + " (" + item.stack + ") exceeds max stack of " + item.maxStack;
								player.SendErrorMessage(check);
								break;
							}
						}
						player.IgnoreActionsForCheating = check;
						check = "none";
						// Please don't remove this for the time being; without it, players wearing banned equipment will only get debuffed once
						foreach (Item item in player.TPlayer.armor)
						{
							if (Itembans.ItemIsBanned(item.name, player))
							{
								player.SetBuff(BuffID.Frozen, 330, true);
								player.SetBuff(BuffID.Stoned, 330, true);
								player.SetBuff(BuffID.Webbed, 330, true);
								check = "Remove armor/accessory " + item.name;

								player.SendErrorMessage("You are wearing banned equipment. {0}", check);
								break;
							}
						}
						foreach (Item item in player.TPlayer.dye)
						{
							if (Itembans.ItemIsBanned(item.name, player))
							{
								player.SetBuff(BuffID.Frozen, 330, true);
								player.SetBuff(BuffID.Stoned, 330, true);
								player.SetBuff(BuffID.Webbed, 330, true);
								check = "Remove dye " + item.name;

								player.SendErrorMessage("You are wearing banned equipment. {0}", check);
								break;
							}
						}
						foreach (Item item in player.TPlayer.miscEquips)
						{
							if (Itembans.ItemIsBanned(item.name, player))
							{
								player.SetBuff(BuffID.Frozen, 330, true);
								player.SetBuff(BuffID.Stoned, 330, true);
								player.SetBuff(BuffID.Webbed, 330, true);
								check = "Remove misc equip " + item.name;

								player.SendErrorMessage("You are wearing banned equipment. {0}", check);
								break;
							}
						}
						foreach (Item item in player.TPlayer.miscDyes)
						{
							if (Itembans.ItemIsBanned(item.name, player))
							{
								player.SetBuff(BuffID.Frozen, 330, true);
								player.SetBuff(BuffID.Stoned, 330, true);
								player.SetBuff(BuffID.Webbed, 330, true);
								check = "Remove misc dye " + item.name;

								player.SendErrorMessage("You are wearing banned equipment. {0}", check);
								break;
							}
						}
						player.IgnoreActionsForDisabledArmor = check;

						if (CheckIgnores(player))
						{
							player.Disable(flags: flags);
						}
						else if (Itembans.ItemIsBanned(player.TPlayer.inventory[player.TPlayer.selectedItem].name, player))
						{
							player.Disable($"holding banned item: {player.TPlayer.inventory[player.TPlayer.selectedItem].name}", flags);
							player.SendErrorMessage($"You are holding a banned item: {player.TPlayer.inventory[player.TPlayer.selectedItem].name}");
						}
					}

					var oldRegion = player.CurrentRegion;
					player.CurrentRegion = Regions.GetTopRegion(Regions.InAreaRegion(player.TileX, player.TileY));

					if (oldRegion != player.CurrentRegion)
					{
						if (oldRegion != null)
						{
							RegionHooks.OnRegionLeft(player, oldRegion);
						}

						if (player.CurrentRegion != null)
						{
							RegionHooks.OnRegionEntered(player, player.CurrentRegion);
						}
					}
				}
			}
			SetConsoleTitle(false);
		}

		/// <summary>SetConsoleTitle - Updates the console title with some pertinent information.</summary>
		/// <param name="empty">empty - True/false if the server is empty; determines if we should use Utils.ActivePlayers() for player count or 0.</param>
		private void SetConsoleTitle(bool empty)
		{
			Console.Title = string.Format("{0}{1}/{2} on {3} @ {4}:{5} (TShock for Terraria v{6})",
					!string.IsNullOrWhiteSpace(Config.ServerName) ? Config.ServerName + " - " : "",
					empty ? 0 : Utils.ActivePlayers(),
					Config.MaxSlots, Main.worldName, Netplay.ServerIP.ToString(), Netplay.ListenPort, Version);
		}

		/// <summary>OnHardUpdate - Fired when a hardmode tile update event happens.</summary>
		/// <param name="args">args - The HardmodeTileUpdateEventArgs object.</param>
		private void OnHardUpdate(HardmodeTileUpdateEventArgs args)
		{
			if (args.Handled)
				return;

			if (!Config.AllowCrimsonCreep && (args.Type == TileID.Dirt || args.Type == TileID.FleshWeeds
				|| TileID.Sets.Crimson[args.Type]))
			{
				args.Handled = true;
				return;
			}

			if (!Config.AllowCorruptionCreep && (args.Type == TileID.Dirt || args.Type == TileID.CorruptThorns
				|| TileID.Sets.Corrupt[args.Type]))
			{
				args.Handled = true;
				return;
			}

			if (!Config.AllowHallowCreep && (TileID.Sets.Hallow[args.Type]))
			{
				args.Handled = true;
			}
		}

		/// <summary>OnStatueSpawn - Fired when a statue spawns.</summary>
		/// <param name="args">args - The StatueSpawnEventArgs object.</param>
		private void OnStatueSpawn(StatueSpawnEventArgs args)
		{
			if (args.Within200 < Config.StatueSpawn200 && args.Within600 < Config.StatueSpawn600 && args.WorldWide < Config.StatueSpawnWorld)
			{
				args.Handled = true;
			}
			else
			{
				args.Handled = false;
			}
		}

		/// <summary>OnConnect - Fired when a player connects to the server.</summary>
		/// <param name="args">args - The ConnectEventArgs object.</param>
		private void OnConnect(ConnectEventArgs args)
		{
			if (ShuttingDown)
			{
				NetMessage.SendData((int)PacketTypes.Disconnect, args.Who, -1, "Server is shutting down...");
				args.Handled = true;
				return;
			}

			var player = new TSPlayer(args.Who);

			if (Utils.ActivePlayers() + 1 > Config.MaxSlots + Config.ReservedSlots)
			{
				Utils.ForceKick(player, Config.ServerFullNoReservedReason, true, false);
				args.Handled = true;
				return;
			}

			if (!FileTools.OnWhitelist(player.IP))
			{
				Utils.ForceKick(player, Config.WhitelistKickReason, true, false);
				args.Handled = true;
				return;
			}

			if (Geo != null)
			{
				var code = Geo.TryGetCountryCode(IPAddress.Parse(player.IP));
				player.Country = code == null ? "N/A" : GeoIPCountry.GetCountryNameByCode(code);
				if (code == "A1")
				{
					if (Config.KickProxyUsers)
					{
						Utils.ForceKick(player, "Proxies are not allowed.", true, false);
						args.Handled = true;
						return;
					}
				}
			}
			Players[args.Who] = player;
		}

		/// <summary>OnJoin - Internal hook called when a player joins. This is called after OnConnect.</summary>
		/// <param name="args">args - The JoinEventArgs object.</param>
		private void OnJoin(JoinEventArgs args)
		{
			var player = Players[args.Who];
			if (player == null)
			{
				args.Handled = true;
				return;
			}

			if (Config.KickEmptyUUID && String.IsNullOrWhiteSpace(player.UUID))
			{
				Utils.ForceKick(player, "Your client did not send a UUID, this server is not configured to accept such a client.", true);
				args.Handled = true;
				return;
			}

			Ban ban = null;
			if (Config.EnableBanOnUsernames)
			{
				var newban = Bans.GetBanByName(player.Name);
				if (null != newban)
					ban = newban;
			}

			if (Config.EnableIPBans && null == ban)
			{
				ban = Bans.GetBanByIp(player.IP);
			}

			if (Config.EnableUUIDBans && null == ban && !String.IsNullOrWhiteSpace(player.UUID))
			{
				ban = Bans.GetBanByUUID(player.UUID);
			}

			if (ban != null)
			{
				if (!Utils.HasBanExpired(ban))
				{
					DateTime exp;
					if (!DateTime.TryParse(ban.Expiration, out exp))
					{
						player.Disconnect("You are banned forever: " + ban.Reason);
					}
					else
					{
						TimeSpan ts = exp - DateTime.UtcNow;
						int months = ts.Days / 30;
						if (months > 0)
						{
							player.Disconnect(String.Format("You are banned for {0} month{1} and {2} day{3}: {4}",
								months, months == 1 ? "" : "s", ts.Days, ts.Days == 1 ? "" : "s", ban.Reason));
						}
						else if (ts.Days > 0)
						{
							player.Disconnect(String.Format("You are banned for {0} day{1} and {2} hour{3}: {4}",
								ts.Days, ts.Days == 1 ? "" : "s", ts.Hours, ts.Hours == 1 ? "" : "s", ban.Reason));
						}
						else if (ts.Hours > 0)
						{
							player.Disconnect(String.Format("You are banned for {0} hour{1} and {2} minute{3}: {4}",
								ts.Hours, ts.Hours == 1 ? "" : "s", ts.Minutes, ts.Minutes == 1 ? "" : "s", ban.Reason));
						}
						else if (ts.Minutes > 0)
						{
							player.Disconnect(String.Format("You are banned for {0} minute{1} and {2} second{3}: {4}",
								ts.Minutes, ts.Minutes == 1 ? "" : "s", ts.Seconds, ts.Seconds == 1 ? "" : "s", ban.Reason));
						}
						else
						{
							player.Disconnect(String.Format("You are banned for {0} second{1}: {2}",
								ts.Seconds, ts.Seconds == 1 ? "" : "s", ban.Reason));
						}
					}
					args.Handled = true;
				}
			}
		}
		
		/// <summary>OnLeave - Called when a player leaves the server.</summary>
		/// <param name="args">args - The LeaveEventArgs object.</param>
		private void OnLeave(LeaveEventArgs args)
		{
			if (args.Who >= Players.Length || args.Who < 0)
			{
				//Something not right has happened
				return;
			}

			var tsplr = Players[args.Who];
			Players[args.Who] = null;

			if (tsplr != null && tsplr.ReceivedInfo)
			{
				if (!tsplr.SilentKickInProgress && tsplr.State >= 3)
					Utils.Broadcast(tsplr.Name + " has left.", Color.Yellow);
				Log.Info("{0} disconnected.", tsplr.Name);

				if (tsplr.IsLoggedIn && !tsplr.IgnoreActionsForClearingTrashCan && Main.ServerSideCharacter && (!tsplr.Dead || tsplr.TPlayer.difficulty != 2))
				{
					tsplr.PlayerData.CopyCharacter(tsplr);
					CharacterDB.InsertPlayerData(tsplr);
				}

				if (Config.RememberLeavePos && !tsplr.LoginHarassed)
				{
					RememberedPos.InsertLeavePos(tsplr.Name, tsplr.IP, (int)(tsplr.X / 16), (int)(tsplr.Y / 16));
				}

				if (tsplr.tempGroupTimer != null)
				{
					tsplr.tempGroupTimer.Stop();
				}
			}

			// Fire the OnPlayerLogout hook too, if the player was logged in and they have a TSPlayer object.
			if (tsplr != null && tsplr.IsLoggedIn)
			{
				Hooks.PlayerHooks.OnPlayerLogout(tsplr);
			}

			// The last player will leave after this hook is executed.
			if (Utils.ActivePlayers() == 1)
			{
				if (Config.SaveWorldOnLastPlayerExit)
					SaveManager.Instance.SaveWorld();
				SetConsoleTitle(true);
			}
		}

		/// <summary>OnChat - Fired when a player chats. Used for handling chat and commands.</summary>
		/// <param name="args">args - The ServerChatEventArgs object.</param>
		private void OnChat(ServerChatEventArgs args)
		{
			if (args.Handled)
				return;

			var tsplr = Players[args.Who];
			if (tsplr == null)
			{
				args.Handled = true;
				return;
			}

			if (args.Text.Length > 500)
			{
				Utils.Kick(tsplr, "Crash attempt via long chat packet.", true);
				args.Handled = true;
				return;
			}

			if ((args.Text.StartsWith(Config.CommandSpecifier) || args.Text.StartsWith(Config.CommandSilentSpecifier))
				&& !string.IsNullOrWhiteSpace(args.Text.Substring(1)))
			{
				try
				{
					args.Handled = true;
					if (!Commands.HandleCommand(tsplr, args.Text))
					{
						// This is required in case anyone makes HandleCommand return false again
						tsplr.SendErrorMessage("Unable to parse command. Please contact an administrator for assistance.");
						Log.ConsoleError("Unable to parse command '{0}' from player {1}.", args.Text, tsplr.Name);
					}
				}
				catch (Exception ex)
				{
					Log.ConsoleError("An exception occurred executing a command.");
					Log.Error(ex.ToString());
				}
			}
			else
			{
				if (!tsplr.HasPermission(Permissions.canchat))
				{
					args.Handled = true;
				}
				else if (tsplr.mute)
				{
					tsplr.SendErrorMessage("You are muted!");
					args.Handled = true;
				}
				else if (!TShock.Config.EnableChatAboveHeads)
				{
					var text = String.Format(Config.ChatFormat, tsplr.Group.Name, tsplr.Group.Prefix, tsplr.Name, tsplr.Group.Suffix,
											 args.Text);
					Hooks.PlayerHooks.OnPlayerChat(tsplr, args.Text, ref text);
					Utils.Broadcast(text, tsplr.Group.R, tsplr.Group.G, tsplr.Group.B);
					args.Handled = true;
				}
				else
				{
					Player ply = Main.player[args.Who];
					string name = ply.name;
					ply.name = String.Format(Config.ChatAboveHeadsFormat, tsplr.Group.Name, tsplr.Group.Prefix, tsplr.Name, tsplr.Group.Suffix);
					NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, ply.name, args.Who, 0, 0, 0, 0);
					ply.name = name;
					var text = args.Text;
					Hooks.PlayerHooks.OnPlayerChat(tsplr, args.Text, ref text);
					NetMessage.SendData((int)PacketTypes.ChatText, -1, args.Who, text, args.Who, tsplr.Group.R, tsplr.Group.G, tsplr.Group.B);
					NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, name, args.Who, 0, 0, 0, 0);

					string msg = String.Format("<{0}> {1}",
						String.Format(Config.ChatAboveHeadsFormat, tsplr.Group.Name, tsplr.Group.Prefix, tsplr.Name, tsplr.Group.Suffix),
						text);

					tsplr.SendMessage(msg, tsplr.Group.R, tsplr.Group.G, tsplr.Group.B);

					TSPlayer.Server.SendMessage(msg, tsplr.Group.R, tsplr.Group.G, tsplr.Group.B);
					Log.Info("Broadcast: {0}", msg);
					args.Handled = true;
				}
			}
		}

		/// <summary>
		/// Called when a command is issued from the server console.
		/// </summary>
		/// <param name="args">The CommandEventArgs object</param>
		private void ServerHooks_OnCommand(CommandEventArgs args)
		{
			if (args.Handled)
				return;

			if (string.IsNullOrWhiteSpace(args.Command))
			{
				args.Handled = true;
				return;
			}

			// Damn you ThreadStatic and Redigit
			if (Main.rand == null)
			{
				Main.rand = new UnifiedRandom();
			}

			if (args.Command == "autosave")
			{
				Main.autoSave = Config.AutoSave = !Config.AutoSave;
				Log.ConsoleInfo("AutoSave " + (Config.AutoSave ? "Enabled" : "Disabled"));
			}
			else if (args.Command.StartsWith(Commands.Specifier) || args.Command.StartsWith(Commands.SilentSpecifier))
			{
				Commands.HandleCommand(TSPlayer.Server, args.Command);
			}
			else
			{
				Commands.HandleCommand(TSPlayer.Server, "/" + args.Command);
			}
			args.Handled = true;
		}

		/// <summary>OnGetData - Called when the server gets raw data packets.</summary>
		/// <param name="e">e - The GetDataEventArgs object.</param>
		private void OnGetData(GetDataEventArgs e)
		{
			if (e.Handled)
				return;

			PacketTypes type = e.MsgID;

			Debug.WriteLine("Recv: {0:X}: {2} ({1:XX})", e.Msg.whoAmI, (byte)type, type);

			var player = Players[e.Msg.whoAmI];
			if (player == null || !player.ConnectionAlive)
			{
				e.Handled = true;
				return;
			}

			if (player.RequiresPassword && type != PacketTypes.PasswordSend)
			{
				e.Handled = true;
				return;
			}

			if ((player.State < 10 || player.Dead) && (int)type > 12 && (int)type != 16 && (int)type != 42 && (int)type != 50 &&
				(int)type != 38 && (int)type != 21 && (int)type != 22)
			{
				e.Handled = true;
				return;
			}

			int length = e.Length - 1;
			if (length < 0)
			{
				length = 0;
			}
			using (var data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length - 1))
			{
				// Exceptions are already handled
				e.Handled = GetDataHandlers.HandlerGetData(type, player, data);
			}
		}

		/// <summary>OnGreetPlayer - Fired when a player is greeted by the server. Handles things like the MOTD, join messages, etc.</summary>
		/// <param name="args">args - The GreetPlayerEventArgs object.</param>
		private void OnGreetPlayer(GreetPlayerEventArgs args)
		{
			var player = Players[args.Who];
			if (player == null)
			{
				args.Handled = true;
				return;
			}

			player.LoginMS = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

			if (Config.EnableGeoIP && TShock.Geo != null)
			{
				Log.Info("{0} ({1}) from '{2}' group from '{3}' joined. ({4}/{5})", player.Name, player.IP,
									   player.Group.Name, player.Country, TShock.Utils.ActivePlayers(),
									   TShock.Config.MaxSlots);
				if (!player.SilentJoinInProgress)
					Utils.Broadcast(string.Format("{0} ({1}) has joined.", player.Name, player.Country), Color.Yellow);
			}
			else
			{
				Log.Info("{0} ({1}) from '{2}' group joined. ({3}/{4})", player.Name, player.IP,
									   player.Group.Name, TShock.Utils.ActivePlayers(), TShock.Config.MaxSlots);
				if (!player.SilentJoinInProgress)
					Utils.Broadcast(player.Name + " has joined.", Color.Yellow);
			}

			if (Config.DisplayIPToAdmins)
				Utils.SendLogs(string.Format("{0} has joined. IP: {1}", player.Name, player.IP), Color.Blue);

			Utils.ShowFileToUser(player, FileTools.MotdPath);

			string pvpMode = Config.PvPMode.ToLowerInvariant();
			if (pvpMode == "always")
			{
				player.TPlayer.hostile = true;
				player.SendData(PacketTypes.TogglePvp, "", player.Index);
				TSPlayer.All.SendData(PacketTypes.TogglePvp, "", player.Index);
			}

			if (!player.IsLoggedIn)
			{
				if (Main.ServerSideCharacter)
				{
					player.SendErrorMessage(
						player.IgnoreActionsForInventory = String.Format("Server side characters is enabled! Please {0}register or {0}login to play!", Commands.Specifier));
					player.LoginHarassed = true;
				}
				else if (Config.RequireLogin)
				{
					player.SendErrorMessage("Please {0}register or {0}login to play!", Commands.Specifier);
					player.LoginHarassed = true;
				}
			}

			player.LastNetPosition = new Vector2(Main.spawnTileX * 16f, Main.spawnTileY * 16f);

			if (Config.RememberLeavePos && (RememberedPos.GetLeavePos(player.Name, player.IP) != Vector2.Zero) && !player.LoginHarassed)
			{
				player.RPPending = 3;
				player.SendInfoMessage("You will be teleported to your last known location...");
			}

			args.Handled = true;
		}

		/// <summary>NpcHooks_OnStrikeNpc - Fired when an NPC strike packet happens.</summary>
		/// <param name="e">e - The NpcStrikeEventArgs object.</param>
		private void NpcHooks_OnStrikeNpc(NpcStrikeEventArgs e)
		{
			if (Config.InfiniteInvasion)
			{
				if (Main.invasionSize < 10)
				{
					Main.invasionSize = 20000000;
				}
			}
		}

		/// <summary>OnProjectileSetDefaults - Called when a projectile sets the default attributes for itself.</summary>
		/// <param name="e">e - The SetDefaultsEventArgs object praameterized with Projectile and int.</param>
		private void OnProjectileSetDefaults(SetDefaultsEventArgs<Projectile, int> e)
		{
			//tombstone fix.
			if (e.Info == 43 || (e.Info >= 201 && e.Info <= 205) || (e.Info >= 527 && e.Info <= 531))
				if (Config.DisableTombstones)
					e.Object.SetDefaults(0);
			if (e.Info == 75)
				if (Config.DisableClownBombs)
					e.Object.SetDefaults(0);
			if (e.Info == 109)
				if (Config.DisableSnowBalls)
					e.Object.SetDefaults(0);
		}

		/// <summary>NetHooks_SendData - Fired when the server sends data.</summary>
		/// <param name="e">e - The SendDataEventArgs object.</param>
		private void NetHooks_SendData(SendDataEventArgs e)
		{
			if (e.MsgId == PacketTypes.PlayerHp)
			{
				if (Main.player[(byte)e.number].statLife <= 0)
				{
					e.Handled = true;
					return;
				}
			}
		}

		/// <summary>OnStartHardMode - Fired when hard mode is started.</summary>
		/// <param name="e">e - The HandledEventArgs object.</param>
		private void OnStartHardMode(HandledEventArgs e)
		{
			if (Config.DisableHardmode)
				e.Handled = true;
		}


		/// <summary>StartInvasion - Starts an invasion on the server.</summary>
		/// <param name="type">type - The invasion type id.</param>
		//TODO: Why is this in TShock's main class?
		public static void StartInvasion(int type)
		{
			int invasionSize = 0;

			if (Config.InfiniteInvasion)
			{
				invasionSize = 20000000;
			}
			else
			{
				invasionSize = 100 + (Config.InvasionMultiplier * Utils.ActivePlayers());
			}

			// Note: This is a workaround to previously providing the size as a parameter in StartInvasion
			Main.invasionSize = invasionSize;

			Main.StartInvasion(type);
		}

		/// <summary>CheckProjectilePermission - Checks if a projectile is banned.</summary>
		/// <param name="player">player - The TSPlayer object that created the projectile.</param>
		/// <param name="index">index - The projectile index.</param>
		/// <param name="type">type - The projectile type.</param>
		/// <returns>bool - True if the player does not have permission to use a projectile.</returns>
		public static bool CheckProjectilePermission(TSPlayer player, int index, int type)
		{
			if (type == 43)
			{
				return true;
			}

			if (type == 17 && Itembans.ItemIsBanned("Dirt Rod", player))
			//Dirt Rod Projectile
			{
				return true;
			}

			if ((type == 42 || type == 65 || type == 68) && Itembans.ItemIsBanned("Sandgun", player)) //Sandgun Projectiles
			{
				return true;
			}

			Projectile proj = new Projectile();
			proj.SetDefaults(type);

			if (Main.projHostile[type])
			{
				//player.SendMessage( proj.name, Color.Yellow);
				return true;
			}

			return false;
		}

		/// <summary>CheckRangePermission - Checks if a player has permission to modify a tile dependent on range checks.</summary>
		/// <param name="player">player - The TSPlayer object.</param>
		/// <param name="x">x - The x coordinate of the tile.</param>
		/// <param name="y">y - The y coordinate of the tile.</param>
		/// <param name="range">range - The range to check for.</param>
		/// <returns>bool - True if the player should not be able to place the tile. False if they can, or if range checks are off.</returns>
		public static bool CheckRangePermission(TSPlayer player, int x, int y, int range = 32)
		{
			if (Config.RangeChecks && ((Math.Abs(player.TileX - x) > range) || (Math.Abs(player.TileY - y) > range)))
			{
				return true;
			}
			return false;
		}

		/// <summary>CheckTilePermission - Checks to see if a player has permission to modify a tile in general.</summary>
		/// <param name="player">player - The TSPlayer object.</param>
		/// <param name="tileX">tileX - The x coordinate of the tile.</param>
		/// <param name="tileY">tileY - The y coordinate of the tile.</param>
		/// <param name="tileType">tileType - The tile type.</param>
		/// <param name="actionType">actionType - The type of edit that took place.</param>
		/// <returns>bool - True if the player should not be able to modify a tile.</returns>
		public static bool CheckTilePermission(TSPlayer player, int tileX, int tileY, short tileType, GetDataHandlers.EditAction actionType)
		{
			if (!player.HasPermission(Permissions.canbuild))
			{
				if (TShock.Config.AllowIce && actionType != GetDataHandlers.EditAction.PlaceTile)
				{
					foreach (Point p in player.IceTiles)
					{
						if (p.X == tileX && p.Y == tileY && (Main.tile[p.X, p.Y].type == 0 || Main.tile[p.X, p.Y].type == 127))
						{
							player.IceTiles.Remove(p);
							return false;
						}
					}

					if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.BPm) > 2000)
					{
						player.SendErrorMessage("You do not have permission to build!");
						player.BPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					}
					return true;
				}

				if (TShock.Config.AllowIce && actionType == GetDataHandlers.EditAction.PlaceTile && tileType == 127)
				{
					player.IceTiles.Add(new Point(tileX, tileY));
					return false;
				}

				if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.BPm) > 2000)
				{
					player.SendErrorMessage("You do not have permission to build!");
					player.BPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				}
				return true;
			}

			if (!Regions.CanBuild(tileX, tileY, player))
			{
				if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.RPm) > 2000)
				{
					player.SendErrorMessage("This region is protected from changes.");
					player.RPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				}
				return true;
			}

			if (Config.DisableBuild)
			{
				if (!player.HasPermission(Permissions.antibuild))
				{
					if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.WPm) > 2000)
					{
						player.SendErrorMessage("The world is protected from changes.");
						player.WPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					}
					return true;
				}
			}

			if (Config.SpawnProtection)
			{
				if (!player.HasPermission(Permissions.editspawn))
				{
					if (CheckSpawn(tileX, tileY))
					{
						if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.SPm) > 2000)
						{
							player.SendErrorMessage("Spawn is protected from changes.");
							player.SPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
						}
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>CheckTilePermission - Checks to see if a player has the ability to modify a tile at a given position.</summary>
		/// <param name="player">player - The TSPlayer object.</param>
		/// <param name="tileX">tileX - The x coordinate of the tile.</param>
		/// <param name="tileY">tileY - The y coordinate of the tile.</param>
		/// <param name="paint">paint - Whether or not the tile is paint.</param>
		/// <returns>bool - True if the player should not be able to modify the tile.</returns>
		public static bool CheckTilePermission(TSPlayer player, int tileX, int tileY, bool paint = false)
		{
			if ((!paint && !player.HasPermission(Permissions.canbuild)) ||
				(paint && !player.HasPermission(Permissions.canpaint)))
			{
				if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.BPm) > 2000)
				{
					if (paint)
					{
						player.SendErrorMessage("You do not have permission to paint!");
					}
					else
					{
						player.SendErrorMessage("You do not have permission to build!");
					}
					player.BPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				}
				return true;
			}

			if (!Regions.CanBuild(tileX, tileY, player))
			{
				if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.RPm) > 2000)
				{
					player.SendErrorMessage("This region is protected from changes.");
					player.RPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				}
				return true;
			}

			if (Config.DisableBuild)
			{
				if (!player.HasPermission(Permissions.antibuild))
				{
					if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.WPm) > 2000)
					{
						player.SendErrorMessage("The world is protected from changes.");
						player.WPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					}
					return true;
				}
			}

			if (Config.SpawnProtection)
			{
				if (!player.HasPermission(Permissions.editspawn))
				{
					if (CheckSpawn(tileX, tileY))
					{
						if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.SPm) > 1000)
						{
							player.SendErrorMessage("Spawn is protected from changes.");
							player.SPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
						}
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>CheckSpawn - Checks to see if a location is inside the spawn protection zone.</summary>
		/// <param name="x">x - The x coordinate to check.</param>
		/// <param name="y">y - The y coordinate to check.</param>
		/// <returns>bool - True if the location is inside the spawn protection zone.</returns>
		public static bool CheckSpawn(int x, int y)
		{
			Vector2 tile = new Vector2(x, y);
			Vector2 spawn = new Vector2(Main.spawnTileX, Main.spawnTileY);
			return Distance(spawn, tile) <= Config.SpawnProtectionRadius;
		}

		/// <summary>Distance - Determines the distance between two vectors.</summary>
		/// <param name="value1">value1 - The first vector location.</param>
		/// <param name="value2">value2 - The second vector location.</param>
		/// <returns>float - The distance between the two vectors.</returns>
		public static float Distance(Vector2 value1, Vector2 value2)
		{
			float num2 = value1.X - value2.X;
			float num = value1.Y - value2.Y;
			float num3 = (num2 * num2) + (num * num);
			return (float)Math.Sqrt(num3);
		}

		/// <summary>HackedInventory - Checks to see if a user has a hacked inventory. In addition, messages players the result.</summary>
		/// <param name="player">player - The TSPlayer object.</param>
		/// <returns>bool - True if the player has a hacked inventory.</returns>
		public static bool HackedInventory(TSPlayer player)
		{
			bool check = false;

			Item[] inventory = player.TPlayer.inventory;
			Item[] armor = player.TPlayer.armor;
			Item[] dye = player.TPlayer.dye;
			Item[] miscEquips = player.TPlayer.miscEquips;
			Item[] miscDyes = player.TPlayer.miscDyes;
			Item[] piggy = player.TPlayer.bank.item;
			Item[] safe = player.TPlayer.bank2.item;
			Item[] forge = player.TPlayer.bank3.item;
			Item trash = player.TPlayer.trashItem;
			for (int i = 0; i < NetItem.MaxInventory; i++)
			{
				if (i < NetItem.InventoryIndex.Item2)
				{
					//0-58
					Item item = new Item();
					if (inventory[i] != null && inventory[i].netID != 0)
					{
						item.netDefaults(inventory[i].netID);
						item.Prefix(inventory[i].prefix);
						item.AffixName();
						if (inventory[i].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove item {0} ({1}) and then rejoin", item.name, inventory[i].stack),
								Color.Cyan);
						}
					}
				}
				else if (i < NetItem.ArmorIndex.Item2)
				{
					//59-78
					var index = i - NetItem.ArmorIndex.Item1;
					Item item = new Item();
					if (armor[index] != null && armor[index].netID != 0)
					{
						item.netDefaults(armor[index].netID);
						item.Prefix(armor[index].prefix);
						item.AffixName();
						if (armor[index].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove armor {0} ({1}) and then rejoin", item.name, armor[index].stack),
								Color.Cyan);
						}
					}
				}
				else if (i < NetItem.DyeIndex.Item2)
				{
					//79-88
					var index = i - NetItem.DyeIndex.Item1;
					Item item = new Item();
					if (dye[index] != null && dye[index].netID != 0)
					{
						item.netDefaults(dye[index].netID);
						item.Prefix(dye[index].prefix);
						item.AffixName();
						if (dye[index].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove dye {0} ({1}) and then rejoin", item.name, dye[index].stack),
								Color.Cyan);
						}
					}
				}
				else if (i < NetItem.MiscEquipIndex.Item2)
				{
					//89-93
					var index = i - NetItem.MiscEquipIndex.Item1;
					Item item = new Item();
					if (miscEquips[index] != null && miscEquips[index].netID != 0)
					{
						item.netDefaults(miscEquips[index].netID);
						item.Prefix(miscEquips[index].prefix);
						item.AffixName();
						if (miscEquips[index].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove item {0} ({1}) and then rejoin", item.name, miscEquips[index].stack),
								Color.Cyan);
						}
					}
				}
				else if (i < NetItem.MiscDyeIndex.Item2)
				{
					//93-98
					var index = i - NetItem.MiscDyeIndex.Item1;
					Item item = new Item();
					if (miscDyes[index] != null && miscDyes[index].netID != 0)
					{
						item.netDefaults(miscDyes[index].netID);
						item.Prefix(miscDyes[index].prefix);
						item.AffixName();
						if (miscDyes[index].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove item dye {0} ({1}) and then rejoin", item.name, miscDyes[index].stack),
								Color.Cyan);
						}
					}
				}
				else if (i < NetItem.PiggyIndex.Item2)
				{
					//98-138
					var index = i - NetItem.PiggyIndex.Item1;
					Item item = new Item();
					if (piggy[index] != null && piggy[index].netID != 0)
					{
						item.netDefaults(piggy[index].netID);
						item.Prefix(piggy[index].prefix);
						item.AffixName();

						if (piggy[index].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove Piggy-bank item {0} ({1}) and then rejoin", item.name, piggy[index].stack),
								Color.Cyan);
						}
					}
				}
				else if (i < NetItem.SafeIndex.Item2)
				{
					//138-178
					var index = i - NetItem.SafeIndex.Item1;
					Item item = new Item();
					if (safe[index] != null && safe[index].netID != 0)
					{
						item.netDefaults(safe[index].netID);
						item.Prefix(safe[index].prefix);
						item.AffixName();

						if (safe[index].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove Safe item {0} ({1}) and then rejoin", item.name, safe[index].stack),
								Color.Cyan);
						}
					}
				}
				else if (i < NetItem.TrashIndex.Item2)
				{
					//179-219
					Item item = new Item();
					if (trash != null && trash.netID != 0)
					{
						item.netDefaults(trash.netID);
						item.Prefix(trash.prefix);
						item.AffixName();

						if (trash.stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove trash item {0} ({1}) and then rejoin", item.name, trash.stack),
								Color.Cyan);
						}
					}
				}
				else
				{
					//220
					var index = i - NetItem.ForgeIndex.Item1;
					Item item = new Item();
					if (forge[index] != null && forge[index].netID != 0)
					{
						item.netDefaults(forge[index].netID);
						item.Prefix(forge[index].prefix);
						item.AffixName();

						if (forge[index].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove Defender's Forge item {0} ({1}) and then rejoin", item.name, forge[index].stack),
								Color.Cyan);
						}
					}

				}
			}

			return check;
		}

		/// <summary>CheckIgnores - Checks a players ignores...?</summary>
		/// <param name="player">player - The TSPlayer object.</param>
		/// <returns>bool - True if any ignore is not none, false, or login state differs from the required state.</returns>
		public static bool CheckIgnores(TSPlayer player)
		{
			return player.IgnoreActionsForInventory != "none" || player.IgnoreActionsForCheating != "none" || player.IgnoreActionsForDisabledArmor != "none" || player.IgnoreActionsForClearingTrashCan || !player.IsLoggedIn && Config.RequireLogin;
		}

		/// <summary>OnConfigRead - Fired when the config file has been read.</summary>
		/// <param name="file">file - The config file object.</param>
		public void OnConfigRead(ConfigFile file)
		{
			NPC.defaultMaxSpawns = file.DefaultMaximumSpawns;
			NPC.defaultSpawnRate = file.DefaultSpawnRate;

			Main.autoSave = file.AutoSave;
			if (Backups != null)
			{
				Backups.KeepFor = file.BackupKeepFor;
				Backups.Interval = file.BackupInterval;
			}
			if (!OverridePort)
			{
				Netplay.ListenPort = file.ServerPort;
			}

			if (file.MaxSlots > 235)
				file.MaxSlots = 235;
			Main.maxNetPlayers = file.MaxSlots + 20;

			Netplay.ServerPassword = "";
			if (!string.IsNullOrEmpty(_cliPassword))
			{
				//This prevents a config reload from removing/updating a CLI-defined password
				file.ServerPassword = _cliPassword;
			}

			Netplay.spamCheck = false;
		}
	}
}
