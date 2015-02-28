/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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
using TerrariaApi.Server;
using TShockAPI.DB;
using TShockAPI.Net;
using TShockAPI.ServerSideCharacters;

namespace TShockAPI
{
	[ApiVersion(1, 16)]
	public class TShock : TerrariaPlugin
	{
		public static readonly Version VersionNum = Assembly.GetExecutingAssembly().GetName().Version;
		public static readonly string VersionCodename = "2015!!";

		public static string SavePath = "tshock";
		private const string LogFormatDefault = "yyyy-MM-dd_HH-mm-ss";
		private static string LogFormat = LogFormatDefault;
		private const string LogPathDefault = "tshock";
		private static string LogPath = LogPathDefault;
		private static bool LogClear;

		public static TSPlayer[] Players = new TSPlayer[Main.maxPlayers];
		public static BanManager Bans;
		public static WarpManager Warps;
        public static RegionManager Regions;
		public static BackupManager Backups;
		public static GroupManager Groups;
		public static UserManager Users;
		public static ItemManager Itembans;
		public static ProjectileManagager ProjectileBans;
		public static TileManager TileBans;
		public static RememberedPosManager RememberedPos;
		public static CharacterManager CharacterDB;
		public static ConfigFile Config { get; set; }
		public static ServerSideConfig ServerSideCharacterConfig;
		public static IDbConnection DB;
		public static bool OverridePort;
		public static PacketBufferer PacketBuffer;
		public static GeoIPCountry Geo;
		public static SecureRest RestApi;
		public static RestManager RestManager;
		public static Utils Utils = Utils.Instance;
		public static UpdateManager UpdateManager;
		public static ILog Log;
		/// <summary>
		/// Used for implementing REST Tokens prior to the REST system starting up.
		/// </summary>
		public static Dictionary<string, SecureRest.TokenData> RESTStartupTokens = new Dictionary<string, SecureRest.TokenData>();

		/// <summary>
		/// Called after TShock is initialized. Useful for plugins that needs hooks before tshock but also depend on tshock being loaded.
		/// </summary>
		public static event Action Initialized;

		public override Version Version
		{
			get { return VersionNum; }
		}

		public override string Name
		{
			get { return "TShock"; }
		}

		public override string Author
		{
			get { return "The Nyx Team"; }
		}

		public override string Description
		{
			get { return "The administration modification of the future."; }
		}

        public override string UpdateURL
        {
            get { return ""; }
        }
		public TShock(Main game)
			: base(game)
		{
			Config = new ConfigFile();
			ServerSideCharacterConfig = new ServerSideConfig();
			ServerSideCharacterConfig.StartingInventory.Add(new NetItem { netID = -15, prefix = 0, stack = 1 });
			ServerSideCharacterConfig.StartingInventory.Add(new NetItem { netID = -13, prefix = 0, stack = 1 });
			ServerSideCharacterConfig.StartingInventory.Add(new NetItem { netID = -16, prefix = 0, stack = 1 });
			Order = 0;
		}


		[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		public override void Initialize()
		{
			string logFilename;
			string logPathSetupWarning;

			try
			{
				HandleCommandLine(Environment.GetCommandLineArgs());

				if (Version.Major >= 4)
					getTShockAscii();

				if (!Directory.Exists(SavePath))
					Directory.CreateDirectory(SavePath);

				ConfigFile.ConfigRead += OnConfigRead;
				FileTools.SetupConfig();

				Main.ServerSideCharacter = ServerSideCharacterConfig.Enabled;

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

#if DEBUG       
                var level = LogLevel.All;
#else
				var level = LogLevel.All & ~LogLevel.Debug;
#endif
				if (Config.UseSqlLogs)
					Log = new SqlLog(level, DB, logFilename, LogClear);
				else
					Log = new TextLog(logFilename, level, LogClear);

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
				RestApi = new SecureRest(Netplay.serverListenIP, Config.RestApiPort);
				RestApi.Port = Config.RestApiPort;
				RestManager = new RestManager(RestApi);
				RestManager.RegisterRestfulCommands();

				var geoippath = Path.Combine(SavePath, "GeoIP.dat");
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
				Hooks.PlayerHooks.PlayerPreLogin += OnPlayerPreLogin;
				Hooks.PlayerHooks.PlayerPostLogin += OnPlayerLogin;
				Hooks.AccountHooks.AccountDelete += OnAccountDelete;
				Hooks.AccountHooks.AccountCreate += OnAccountCreate;

				GetDataHandlers.InitGetDataHandler();
				Commands.InitCommands();
				//RconHandler.StartThread();

				if (Config.RestApiEnabled)
					RestApi.Start();

				if (Config.BufferPackets)
					PacketBuffer = new PacketBufferer(this);

				Log.ConsoleInfo("AutoSave " + (Config.AutoSave ? "Enabled" : "Disabled"));
				Log.ConsoleInfo("Backups " + (Backups.Interval > 0 ? "Enabled" : "Disabled"));

				if (Initialized != null)
					Initialized();
			}
			catch (Exception ex)
			{
				Log.Error("Fatal Startup Exception");
				Log.Error(ex.ToString());
				Environment.Exit(1);
			}
		}

		private static void getTShockAscii()
	    {
// ReSharper disable LocalizableElement
	        Console.Write("              ___          ___          ___          ___          ___ \n" +
	                      "     ___     /  /\\        /__/\\        /  /\\        /  /\\        /__/|    \n" +
	                      "    /  /\\   /  /:/_       \\  \\:\\      /  /::\\      /  /:/       |  |:|    \n" +
	                      "   /  /:/  /  /:/ /\\       \\__\\:\\    /  /:/\\:\\    /  /:/        |  |:|    \n" +
	                      "  /  /:/  /  /:/ /::\\  ___ /  /::\\  /  /:/  \\:\\  /  /:/  ___  __|  |:|    \n" +
	                      " /  /::\\ /__/:/ /:/\\:\\/__/\\  /:/\\:\\/__/:/ \\__\\:\\/__/:/  /  /\\/__/\\_|:|____\n" +
	                      "/__/:/\\:\\\\  \\:\\/:/~/:/\\  \\:\\/:/__\\/\\  \\:\\ /  /:/\\  \\:\\ /  /:/\\  \\:\\/:::::/\n" +
	                      "\\__\\/  \\:\\\\  \\::/ /:/  \\  \\::/      \\  \\:\\  /:/  \\  \\:\\  /:/  \\  \\::/~~~~ \n" +
	                      "     \\  \\:\\\\__\\/ /:/    \\  \\:\\       \\  \\:\\/:/    \\  \\:\\/:/    \\  \\:\\     \n" +
	                      "      \\__\\/  /__/:/      \\  \\:\\       \\  \\::/      \\  \\::/      \\  \\:\\    \n" +
	                      "             \\__\\/        \\__\\/        \\__\\/        \\__\\/        \\__\\/    \n" +
	                      "");
            Console.WriteLine("TShock for Terraria is open & free software. If you paid, you were scammed.");
// ReSharper restore LocalizableElement
	    }

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

	    private void OnPlayerLogin(Hooks.PlayerPostLoginEventArgs args)
	    {
	        User u = Users.GetUserByName(args.Player.UserAccountName);
            List<String> KnownIps = new List<string>();
	        if (!string.IsNullOrWhiteSpace(u.KnownIps))
	        {
                KnownIps = JsonConvert.DeserializeObject<List<String>>(u.KnownIps);
	        }

	        bool found = KnownIps.Any(s => s.Equals(args.Player.IP));
	        if (!found)
	        {
	            if (KnownIps.Count == 100)
	            {
	                KnownIps.RemoveAt(0);
	            }

                KnownIps.Add(args.Player.IP);
	        }

            u.KnownIps = JsonConvert.SerializeObject(KnownIps, Formatting.Indented);
	        Users.UpdateLogin(u);
	    }

		private void OnAccountDelete(Hooks.AccountDeleteEventArgs args)
		{
			CharacterDB.RemovePlayer(args.User.ID);
		}

		private void OnAccountCreate(Hooks.AccountCreateEventArgs args)
		{
			CharacterDB.SeedInitialData(Users.GetUser(args.User));
		}

		private void OnPlayerPreLogin(Hooks.PlayerPreLoginEventArgs args)
		{
			if (args.Player.IsLoggedIn)
				args.Player.SaveServerCharacter();

			if (args.Player.ItemInHand.type != 0)
			{
				args.Player.SendErrorMessage("Attempting to bypass SSC with item in hand.");
				args.Handled = true;
			}
		}

		private void NetHooks_NameCollision(NameCollisionEventArgs args)
		{
			string ip = TShock.Utils.GetRealIP(Netplay.serverSock[args.Who].tcpClient.Client.RemoteEndPoint.ToString());

			var player = TShock.Players.First(p => p != null && p.Name == args.Name && p.Index != args.Who);
			if (player != null)
			{
				if (player.IP == ip)
				{
					Netplay.serverSock[player.Index].kill = true;
					args.Handled = true;
					return;
				}
				else if (player.IsLoggedIn)
				{
					User user = TShock.Users.GetUserByName(player.UserAccountName);
					var ips = JsonConvert.DeserializeObject<List<string>>(user.KnownIps);
					if (ips.Contains(ip))
					{
						Netplay.serverSock[player.Index].kill = true;
						args.Handled = true;
						return;
					}
				}
			}
		}

    private void OnXmasCheck(ChristmasCheckEventArgs args)
    {
        if (args.Handled)
            return;

        if(Config.ForceXmas)
        {
            args.Xmas = true;
            args.Handled = true;
        }
    }

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
		/// Handles exceptions that we didn't catch or that Red fucked up
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Log.Error(e.ExceptionObject.ToString());

			if (e.ExceptionObject.ToString().Contains("Terraria.Netplay.ListenForClients") ||
				e.ExceptionObject.ToString().Contains("Terraria.Netplay.ServerLoop"))
			{
				var sb = new List<string>();
				for (int i = 0; i < Netplay.serverSock.Length; i++)
				{
					if (Netplay.serverSock[i] == null)
					{
						sb.Add("Sock[" + i + "]");
					}
					else if (Netplay.serverSock[i].tcpClient == null)
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
					Main.worldPathName += ".crash";
					SaveManager.Instance.SaveWorld();
				}
			}
		}

		private void HandleCommandLine(string[] parms)
		{
			string path;
			for (int i = 0; i < parms.Length; i++)
			{
				switch(parms[i].ToLower())
				{
					case "-configpath":
						path = parms[++i];
						if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
						{
							SavePath = path;
							Log.ConsoleInfo("Config path has been set to " + path);
						}
						break;

					case "-worldpath":
						path = parms[++i];
						if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
						{
							Main.WorldPath = path;
							Log.ConsoleInfo("World path has been set to " + path);
						}
						break;

					case "-logpath":
						path = parms[++i];
						if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
						{
							LogPath = path;
							Log.ConsoleInfo("Log path has been set to " + path);
						}
						break;

					case "-logformat":
						LogFormat = parms[++i];
						break;

					case "-logclear":
						bool.TryParse(parms[++i], out LogClear);
						break;

					case "-dump":
						ConfigFile.DumpDescriptions();
						Permissions.DumpDescriptions();
						break;
				}
			}
		}

		public static void HandleCommandLinePostConfigLoad(string[] parms)
		{
			for (int i = 0; i < parms.Length; i++)
			{
				switch(parms[i].ToLower())
				{
					case "-port":
						int port = Convert.ToInt32(parms[++i]);
						Netplay.serverPort = port;
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

		/*
		 * Hooks:
		 * 
		 */

		public static int AuthToken = -1;

		private void OnPostInit(EventArgs args)
		{
			SetConsoleTitle(false);
			if (!File.Exists(Path.Combine(SavePath, "auth.lck")) && !File.Exists(Path.Combine(SavePath, "authcode.txt")))
			{
				var r = new Random((int) DateTime.Now.ToBinary());
				AuthToken = r.Next(100000, 10000000);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("TShock Notice: To become SuperAdmin, join the game and type {0}auth {1}", Commands.Specifier, AuthToken);
				Console.WriteLine("This token will display until disabled by verification. ({0}auth-verify)", Commands.Specifier);
				Console.ForegroundColor = ConsoleColor.Gray;
				FileTools.CreateFile(Path.Combine(SavePath, "authcode.txt"));
				using (var tw = new StreamWriter(Path.Combine(SavePath, "authcode.txt")))
				{
					tw.WriteLine(AuthToken);
				}
			}
			else if (File.Exists(Path.Combine(SavePath, "authcode.txt")))
			{
				using (var tr = new StreamReader(Path.Combine(SavePath, "authcode.txt")))
				{
					AuthToken = Convert.ToInt32(tr.ReadLine());
				}
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(
					"TShock Notice: authcode.txt is still present, and the AuthToken located in that file will be used.");
				Console.WriteLine("To become superadmin, join the game and type {0}auth {1}", Commands.Specifier, AuthToken);
				Console.WriteLine("This token will display until disabled by verification. ({0}auth-verify)", Commands.Specifier);
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			else
			{
				AuthToken = 0;
			}
			
			Regions.Reload();
			Warps.ReloadWarps();

			Lighting.lightMode = 2;
			ComputeMaxStyles();
			FixChestStacks();
			
			UpdateManager = new UpdateManager();
		}

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

		private DateTime LastCheck = DateTime.UtcNow;
		private DateTime LastSave = DateTime.UtcNow;

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

		private void OnSecondUpdate()
		{
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
							player.Disable("Reached TileKill threshold.");
							TSPlayer.Server.RevertTiles(player.TilesDestroyed);
							player.TilesDestroyed.Clear();
						}
					}
					if (player.TileKillThreshold > 0)
					{
						player.TileKillThreshold = 0;
						//We don't want to revert the entire map in case of a disable.
						player.TilesDestroyed.Clear();
					}

					if (player.TilesCreated != null)
					{
						if (player.TilePlaceThreshold >= Config.TilePlaceThreshold)
						{
							player.Disable("Reached TilePlace threshold");
							TSPlayer.Server.RevertTiles(player.TilesCreated);
							player.TilesCreated.Clear();
						}
					}
					if (player.TilePlaceThreshold > 0)
					{
						player.TilePlaceThreshold = 0;
					}
					
					if (player.RecentFuse >0)
						player.RecentFuse--;

					if ((Main.ServerSideCharacter) && (player.TPlayer.SpawnX > 0) &&(player.sX != player.TPlayer.SpawnX))
					{
						player.sX=player.TPlayer.SpawnX;
						player.sY=player.TPlayer.SpawnY;
					}

					if ((Main.ServerSideCharacter) && (player.sX > 0) && (player.sY > 0) && (player.TPlayer.SpawnX < 0))
					{
						player.TPlayer.SpawnX = player.sX;
						player.TPlayer.SpawnY = player.sY;
					}

					if (player.RPPending >0)
					{
						if (player.RPPending == 1)
						{
								var pos = RememberedPos.GetLeavePos(player.Name, player.IP);
								player.Teleport(pos.X*16, pos.Y*16 );
								player.RPPending = 0;							
						}
						else
						{
							player.RPPending--;
						}
					}					
					
					if (player.TileLiquidThreshold >= Config.TileLiquidThreshold)
					{
						player.Disable("Reached TileLiquid threshold");
					}
					if (player.TileLiquidThreshold > 0)
					{
						player.TileLiquidThreshold = 0;
					}

					if (player.ProjectileThreshold >= Config.ProjectileThreshold)
					{
						player.Disable("Reached projectile threshold");
					}
					if (player.ProjectileThreshold > 0)
					{
						player.ProjectileThreshold = 0;
					}

					if (player.PaintThreshold >= Config.TilePaintThreshold)
					{
						player.Disable("Reached paint threshold");
					}
					if (player.PaintThreshold > 0)
					{
						player.PaintThreshold = 0;
					}

					if (player.RespawnTimer > 0 && --player.RespawnTimer == 0 && player.Difficulty != 2)
					{
						player.Spawn();
					}
					string check = "none";
					foreach (Item item in player.TPlayer.inventory)
					{
						if (!player.Group.HasPermission(Permissions.ignorestackhackdetection) && (item.stack > item.maxStack || item.stack < 0) &&
							item.type != 0)
						{
							check = "Remove item " + item.name + " (" + item.stack + ") exceeds max stack of " + item.maxStack;
							player.SendErrorMessage(check);
							break;
						}
					}
					player.IgnoreActionsForCheating = check;
					check = "none";
					//todo: pretty sure we check every place a players inventory can change, so do we really need to do this?
					foreach (Item item in player.TPlayer.armor)
					{
						if (Itembans.ItemIsBanned(item.name, player))
						{
							player.SetBuff(30, 120); //Bleeding
							player.SetBuff(36, 120); //Broken Armor
							check = "Remove armor/accessory " + item.name;
							
							player.SendErrorMessage("You are wearing banned equipment. {0}", check);
							break;
						}
					}
					player.IgnoreActionsForDisabledArmor = check;
					if (CheckIgnores(player))
					{
						player.Disable("check ignores failed in SecondUpdate()", false);
					}
					else if (Itembans.ItemIsBanned(player.TPlayer.inventory[player.TPlayer.selectedItem].name, player))
					{
						player.SetBuff(23, 120); //Cursed
					}
				}
			}
			SetConsoleTitle(false);
		}

		private void SetConsoleTitle(bool empty)
		{
		    Console.Title = string.Format("{0}{1}/{2} @ {3}:{4} (TShock for Terraria v{5})",
		                                  !string.IsNullOrWhiteSpace(Config.ServerName) ? Config.ServerName + " - " : "",
		                                  empty ? 0 : Utils.ActivePlayers(),
		                                  Config.MaxSlots, Netplay.serverListenIP, Netplay.serverPort, Version);
		}

		private void OnHardUpdate(HardmodeTileUpdateEventArgs args)
		{
			if (args.Handled)
				return;

			if (!Config.AllowCrimsonCreep && (args.Type == 0 || args.Type == 199 || args.Type == 200 || args.Type == 203
                		|| args.Type == 234))
                	{
				args.Handled = true;
				return;
			}

			if (!Config.AllowCorruptionCreep && (args.Type == 23 || args.Type == 25 || args.Type == 0 ||
				args.Type == 112 || args.Type == 32))
			{
				args.Handled = true;
				return;
			}

			if (!Config.AllowHallowCreep && (args.Type == 109 || args.Type == 117 || args.Type == 116 || args.Type == 115
                		|| args.Type == 164))
			{
				args.Handled = true;
			}
		}

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

		private void OnConnect(ConnectEventArgs args)
		{
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
								ts.Days, ts.Days == 1 ? "": "s", ts.Hours, ts.Hours == 1 ? "" : "s", ban.Reason));
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

		private void OnLeave(LeaveEventArgs args)
		{
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
					RememberedPos.InsertLeavePos(tsplr.Name, tsplr.IP, (int) (tsplr.X/16), (int) (tsplr.Y/16));
				}
			}
			
			// The last player will leave after this hook is executed.
			if (Utils.ActivePlayers() == 1)
			{
				if (Config.SaveWorldOnLastPlayerExit)
					SaveManager.Instance.SaveWorld();
				SetConsoleTitle(true);
			}
		}

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
				Utils.Kick(tsplr, "Crash attempt", true);
				args.Handled = true;
				return;
			}

			/*if (!Utils.ValidString(text))
			{
				e.Handled = true;
				return;
			}*/

			if ((args.Text.StartsWith(Config.CommandSpecifier) || args.Text.StartsWith(Config.CommandSilentSpecifier)) 
				&& !string.IsNullOrWhiteSpace(args.Text.Substring(1)))
			{
				try
				{
					args.Handled = Commands.HandleCommand(tsplr, args.Text);
				}
				catch (Exception ex)
				{
					Log.ConsoleError("An exeption occurred executing a command.");
					Log.Error(ex.ToString());
				}
			}
			else
			{
				if (!tsplr.Group.HasPermission(Permissions.canchat))
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
		/// When a server command is run.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="e"></param>
		private void ServerHooks_OnCommand(CommandEventArgs args)
		{
			if (args.Handled)
				return;

			// Damn you ThreadStatic and Redigit
			if (Main.rand == null)
			{
				Main.rand = new Random();
			}
			if (WorldGen.genRand == null)
			{
				WorldGen.genRand = new Random();
			}

			if (args.Command.StartsWith("playing") || args.Command.StartsWith("{0}playing".SFormat(Commands.Specifier)))
			{
				int count = 0;
				foreach (TSPlayer player in Players)
				{
					if (player != null && player.Active)
					{
						count++;
						TSPlayer.Server.SendInfoMessage("{0} ({1}) [{2}] <{3}>", player.Name, player.IP,
							player.Group.Name, player.UserAccountName);
					}
				}
				TSPlayer.Server.SendInfoMessage("{0} players connected.", count);
			}
			else if (args.Command == "autosave")
			{
				Main.autoSave = Config.AutoSave = !Config.AutoSave;
				Log.ConsoleInfo("AutoSave " + (Config.AutoSave ? "Enabled" : "Disabled"));
			}
			else if (args.Command.StartsWith(Commands.Specifier))
			{
				Commands.HandleCommand(TSPlayer.Server, args.Command);
			}
			else
			{
				Commands.HandleCommand(TSPlayer.Server, "/" + args.Command);
			}
			args.Handled = true;
		}

		private void OnGetData(GetDataEventArgs e)
		{
			if (e.Handled)
				return;
			
			PacketTypes type = e.MsgID;

			Debug.WriteLine("Recv: {0:X}: {2} ({1:XX})", e.Msg.whoAmI, (byte) type, type);

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

			if ((player.State < 10 || player.Dead) && (int) type > 12 && (int) type != 16 && (int) type != 42 && (int) type != 50 &&
				(int) type != 38 && (int) type != 21 && (int) type != 22)
			{
				e.Handled = true;
				return;
			}

			using (var data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length - 1))
			{
				// Exceptions are already handled
				e.Handled = GetDataHandlers.HandlerGetData(type, player, data);
			}
		}

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

			Utils.ShowFileToUser(player, "motd.txt");

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
						player.IgnoreActionsForInventory = "Server side characters is enabled! Please {0}register or {0}login to play!", Commands.Specifier);
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

		private void NpcHooks_OnStrikeNpc(NpcStrikeEventArgs e)
		{
			if (Config.InfiniteInvasion)
			{
				IncrementKills();
				if (Main.invasionSize < 10)
				{
					Main.invasionSize = 20000000;
				}
			}
		}

		private void OnProjectileSetDefaults(SetDefaultsEventArgs<Projectile, int> e)
		{
			//tombstone fix.
			if (e.Info == 43 || (e.Info >= 201 && e.Info <= 205))
				if (Config.DisableTombstones)
					e.Object.SetDefaults(0);
			if (e.Info == 75)
				if (Config.DisableClownBombs)
					e.Object.SetDefaults(0);
			if (e.Info == 109)
				if (Config.DisableSnowBalls)
					e.Object.SetDefaults(0);
		}

		/// <summary>
		/// Send bytes to client using packetbuffering if available
		/// </summary>
		/// <param name="client">socket to send to</param>
		/// <param name="bytes">bytes to send</param>
		/// <returns>False on exception</returns>
		public static bool SendBytes(ServerSock client, byte[] bytes)
		{
			if (PacketBuffer != null)
			{
				PacketBuffer.BufferBytes(client, bytes);
				return true;
			}

			return SendBytesBufferless(client, bytes);
		}

		/// <summary>
		/// Send bytes to a client ignoring the packet buffer
		/// </summary>
		/// <param name="client">socket to send to</param>
		/// <param name="bytes">bytes to send</param>
		/// <returns>False on exception</returns>
		public static bool SendBytesBufferless(ServerSock client, byte[] bytes)
		{
			try
			{
				if (client.tcpClient.Connected)
					client.networkStream.Write(bytes, 0, bytes.Length);
				return true;
			}
			catch (Exception ex)
			{
				Log.Warn("This is a normal exception");
				Log.Warn(ex.ToString());
			}
			return false;
		}

		private void NetHooks_SendData(SendDataEventArgs e)
		{
			if (e.MsgId == PacketTypes.Disconnect)
			{
				Action<ServerSock, string> senddisconnect = (sock, str) =>
																{
																	if (sock == null || !sock.active)
																		return;
																	sock.kill = true;
																	using (var ms = new MemoryStream())
																	{
																		new DisconnectMsg { Reason = str }.PackFull(ms);
																		SendBytesBufferless(sock, ms.ToArray());
																	}
																};

				if (e.remoteClient != -1)
				{
					senddisconnect(Netplay.serverSock[e.remoteClient], e.text);
				}
				else
				{
					for (int i = 0; i < Netplay.serverSock.Length; i++)
					{
						if (e.ignoreClient != -1 && e.ignoreClient == i)
							continue;

						senddisconnect(Netplay.serverSock[i], e.text);
					}
				}
				e.Handled = true;
				return;
			}
			else if (e.MsgId == PacketTypes.WorldInfo)
			{
				if (e.remoteClient == -1) return;
				var player = Players[e.remoteClient];
				if (player == null) return;
				using (var ms = new MemoryStream())
				{
					var msg = new WorldInfoMsg
					{
						Time = (int)Main.time,
						DayTime = Main.dayTime,
						MoonPhase = (byte)Main.moonPhase,
						BloodMoon = Main.bloodMoon,
						Eclipse = Main.eclipse,
						MaxTilesX = (short)Main.maxTilesX,
						MaxTilesY = (short)Main.maxTilesY,
						SpawnX = (short)Main.spawnTileX,
						SpawnY = (short)Main.spawnTileY,
						WorldSurface = (short)Main.worldSurface,
						RockLayer = (short)Main.rockLayer,
						//Sending a fake world id causes the client to not be able to find a stored spawnx/y.
						//This fixes the bed spawn point bug. With a fake world id it wont be able to find the bed spawn.
						WorldID = Main.worldID,
						MoonType = (byte)Main.moonType,
						TreeX0 = Main.treeX[0],
						TreeX1 = Main.treeX[1],
						TreeX2 = Main.treeX[2],
						TreeStyle0 = (byte)Main.treeStyle[0],
						TreeStyle1 = (byte)Main.treeStyle[1],
						TreeStyle2 = (byte)Main.treeStyle[2],
						TreeStyle3 = (byte)Main.treeStyle[3],
						CaveBackX0 = Main.caveBackX[0],
						CaveBackX1 = Main.caveBackX[1],
						CaveBackX2 = Main.caveBackX[2],
						CaveBackStyle0 = (byte)Main.caveBackStyle[0],
						CaveBackStyle1 = (byte)Main.caveBackStyle[1],
						CaveBackStyle2 = (byte)Main.caveBackStyle[2],
						CaveBackStyle3 = (byte)Main.caveBackStyle[3],
						SetBG0 = (byte)WorldGen.treeBG,
						SetBG1 = (byte)WorldGen.corruptBG,
						SetBG2 = (byte)WorldGen.jungleBG,
						SetBG3 = (byte)WorldGen.snowBG,
						SetBG4 = (byte)WorldGen.hallowBG,
						SetBG5 = (byte)WorldGen.crimsonBG,
						SetBG6 = (byte)WorldGen.desertBG,
						SetBG7 = (byte)WorldGen.oceanBG,
						IceBackStyle = (byte)Main.iceBackStyle,
						JungleBackStyle = (byte)Main.jungleBackStyle,
						HellBackStyle = (byte)Main.hellBackStyle,
						WindSpeed = Main.windSpeed,
						NumberOfClouds = (byte)Main.numClouds,
						BossFlags = (WorldGen.shadowOrbSmashed ? BossFlags.OrbSmashed : BossFlags.None) |
									(NPC.downedBoss1 ? BossFlags.DownedBoss1 : BossFlags.None) |
									(NPC.downedBoss2 ? BossFlags.DownedBoss2 : BossFlags.None) |
									(NPC.downedBoss3 ? BossFlags.DownedBoss3 : BossFlags.None) |
									(Main.hardMode ? BossFlags.HardMode : BossFlags.None) |
									(NPC.downedClown ? BossFlags.DownedClown : BossFlags.None) |
									(Main.ServerSideCharacter ? BossFlags.ServerSideCharacter : BossFlags.None) |
									(NPC.downedPlantBoss ? BossFlags.DownedPlantBoss : BossFlags.None),
						BossFlags2 = (NPC.downedMechBoss1 ? BossFlags2.DownedMechBoss1 : BossFlags2.None) |
									 (NPC.downedMechBoss2 ? BossFlags2.DownedMechBoss2 : BossFlags2.None) |
									 (NPC.downedMechBoss3 ? BossFlags2.DownedMechBoss3 : BossFlags2.None) |
									 (NPC.downedMechBossAny ? BossFlags2.DownedMechBossAny : BossFlags2.None) |
									 (Main.cloudBGActive >= 1f ? BossFlags2.CloudBg : BossFlags2.None) |
									 (WorldGen.crimson ? BossFlags2.Crimson : BossFlags2.None) |
									 (Main.pumpkinMoon ? BossFlags2.PumpkinMoon : BossFlags2.None) |
									 (Main.snowMoon ? BossFlags2.SnowMoon : BossFlags2.None) ,
						Rain = Main.maxRaining,
						WorldName = TShock.Config.UseServerName ? TShock.Config.ServerName : Main.worldName
					};
					msg.PackFull(ms);
					player.SendRawData(ms.ToArray());
				}
				e.Handled = true;
				return;
			}
			else if (e.MsgId == PacketTypes.PlayerHp)
			{
				if (Main.player[(byte)e.number].statLife <= 0)
				{
					e.Handled = true;
					return;
				}
			}
		}

		private void OnStartHardMode(HandledEventArgs e)
		{
			if (Config.DisableHardmode)
				e.Handled = true;
		}

	    /*
		 * Useful stuff:
		 * */

		public static void StartInvasion(int type)
		{
			Main.invasionType = type;
			if (Config.InfiniteInvasion)
			{
				Main.invasionSize = 20000000;
			}
			else
			{
				Main.invasionSize = 100 + (Config.InvasionMultiplier*Utils.ActivePlayers());
			}

			Main.invasionWarn = 0;
			if (new Random().Next(2) == 0)
			{
				Main.invasionX = 0.0;
			}
			else
			{
				Main.invasionX = Main.maxTilesX;
			}
		}

		private static int KillCount;

		public static void IncrementKills()
		{
			KillCount++;
			Random r = new Random();
			int random = r.Next(5);
			if (KillCount%100 == 0)
			{
				switch (random)
				{
					case 0:
						Utils.Broadcast(string.Format("You call that a lot? {0} goblins killed!", KillCount), Color.Green);
						break;
					case 1:
						Utils.Broadcast(string.Format("Fatality! {0} goblins killed!", KillCount), Color.Green);
						break;
					case 2:
						Utils.Broadcast(string.Format("Number of 'noobs' killed to date: {0}", KillCount), Color.Green);
						break;
					case 3:
						Utils.Broadcast(string.Format("Duke Nukem would be proud. {0} goblins killed.", KillCount), Color.Green);
						break;
					case 4:
						Utils.Broadcast(string.Format("You call that a lot? {0} goblins killed!", KillCount), Color.Green);
						break;
					case 5:
						Utils.Broadcast(string.Format("{0} copies of Call of Duty smashed.", KillCount), Color.Green);
						break;
				}
			}
		}

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

		public static bool CheckRangePermission(TSPlayer player, int x, int y, int range = 32)
		{
			if (Config.RangeChecks && ((Math.Abs(player.TileX - x) > range) || (Math.Abs(player.TileY - y) > range)))
			{
				return true;
			}
			return false;
		}

		public static bool CheckTilePermission(TSPlayer player, int tileX, int tileY, short tileType, GetDataHandlers.EditAction actionType)
		{
			if (!player.Group.HasPermission(Permissions.canbuild))
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

			if (!player.Group.HasPermission(Permissions.editregion) && !Regions.CanBuild(tileX, tileY, player) &&
				Regions.InArea(tileX, tileY))
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
				if (!player.Group.HasPermission(Permissions.antibuild))
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
				if (!player.Group.HasPermission(Permissions.editspawn))
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

		public static bool CheckTilePermission(TSPlayer player, int tileX, int tileY, bool paint = false)
		{
			if ((!paint && !player.Group.HasPermission(Permissions.canbuild)) ||
				(paint && !player.Group.HasPermission(Permissions.canpaint)))
			{
				if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.BPm) > 2000)
				{
					player.SendErrorMessage("You do not have permission to build!");
					player.BPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
				}
				return true;
			}

			if (!player.Group.HasPermission(Permissions.editregion) && !Regions.CanBuild(tileX, tileY, player) &&
				Regions.InArea(tileX, tileY))
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
				if (!player.Group.HasPermission(Permissions.antibuild))
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
				if (!player.Group.HasPermission(Permissions.editspawn))
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

		public static bool CheckSpawn(int x, int y)
		{
			Vector2 tile = new Vector2(x, y);
			Vector2 spawn = new Vector2(Main.spawnTileX, Main.spawnTileY);
			return Distance(spawn, tile) <= Config.SpawnProtectionRadius;
		}

		public static float Distance(Vector2 value1, Vector2 value2)
		{
			float num2 = value1.X - value2.X;
			float num = value1.Y - value2.Y;
			float num3 = (num2*num2) + (num*num);
			return (float) Math.Sqrt(num3);
		}

		public static bool HackedInventory(TSPlayer player)
		{
			bool check = false;

			Item[] inventory = player.TPlayer.inventory;
			Item[] armor = player.TPlayer.armor;
			Item[] dye = player.TPlayer.dye;
			for (int i = 0; i < NetItem.maxNetInventory; i++)
			{
				if (i < NetItem.maxNetInventory - (NetItem.armorSlots + NetItem.dyeSlots))
				{
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
				else if(i < (NetItem.maxNetInventory - (NetItem.armorSlots + NetItem.dyeSlots)))
				{
					Item item = new Item();
					var index = i - (NetItem.maxNetInventory - (NetItem.armorSlots + NetItem.dyeSlots));
					if (armor[index] != null && armor[index].netID != 0)
					{
						item.netDefaults(armor[index].netID);
						item.Prefix(armor[index].prefix);
						item.AffixName();
						if (armor[index].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove armor {0} ({1}) and then rejoin", item.name, armor[i - 48].stack),
								Color.Cyan);
						}
					}
				}
				else if (i < (NetItem.maxNetInventory - (NetItem.armorSlots + NetItem.dyeSlots)))
				{
					Item item = new Item();
					var index = i - (NetItem.maxNetInventory - NetItem.dyeSlots);
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
			}

			return check;
		}

		public static bool CheckIgnores(TSPlayer player)
		{
			return player.IgnoreActionsForInventory != "none" || player.IgnoreActionsForCheating != "none" || player.IgnoreActionsForDisabledArmor != "none" || player.IgnoreActionsForClearingTrashCan || !player.IsLoggedIn && Config.RequireLogin;
		}

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
				Netplay.serverPort = file.ServerPort;
			}

			if (file.MaxSlots > 235)
				file.MaxSlots = 235;
			Main.maxNetPlayers = file.MaxSlots + 20;
			Netplay.password = "";
			Netplay.spamCheck = false;

			RconHandler.Password = file.RconPassword;
			RconHandler.ListenPort = file.RconPort;

			Utils.HashAlgo = file.HashAlgorithm;
		}
	}
}
