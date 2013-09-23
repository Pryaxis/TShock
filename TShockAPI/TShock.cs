/*
TShock, a server mod for Terraria
Copyright (C) 2011-2013 Nyx Studios (fka. The TShock Team)

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
using Hooks;
using MaxMind;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Rests;
using Terraria;
using TShockAPI.DB;
using TShockAPI.Net;

namespace TShockAPI
{
	[APIVersion(1, 13)]
	public class TShock : TerrariaPlugin
	{
		public static readonly Version VersionNum = Assembly.GetExecutingAssembly().GetName().Version;
		public static readonly string VersionCodename = "Welcome to the future.";

		public static string SavePath = "tshock";
		private const string LogFormatDefault = "yyyy-MM-dd_HH-mm-ss";
		private static string LogFormat = LogFormatDefault;
		private const string LogPathDefault = "tshock";
		private static string LogPath = LogPathDefault;
		private static bool LogClear = false;

		public static TSPlayer[] Players = new TSPlayer[Main.maxPlayers];
		public static BanManager Bans;
		public static WarpManager Warps;
        public static RegionManager Regions;
		public static BackupManager Backups;
		public static GroupManager Groups;
		public static UserManager Users;
		public static ItemManager Itembans;
		public static RememberedPosManager RememberedPos;
		public static InventoryManager InventoryDB;
		public static ConfigFile Config { get; set; }
		public static IDbConnection DB;
		public static bool OverridePort;
		public static PacketBufferer PacketBuffer;
		public static GeoIPCountry Geo;
		public static SecureRest RestApi;
		public static RestManager RestManager;
		public static Utils Utils = Utils.Instance;
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
			Order = 0;
		}


		[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		public override void Initialize()
		{
			try
			{
				HandleCommandLine(Environment.GetCommandLineArgs());

				if (Version.Major >= 4)
					getTShockAscii();

				if (!Directory.Exists(SavePath))
					Directory.CreateDirectory(SavePath);

				ConfigFile.ConfigRead += OnConfigRead;
				FileTools.SetupConfig();

				DateTime now = DateTime.Now;
				string logFilename;
				string logPathSetupWarning = null;
				// Log path was not already set by the command line parameter?
				if (LogPath == LogPathDefault)
					LogPath = Config.LogPath;
				try
				{
					logFilename = Path.Combine(LogPath, now.ToString(LogFormat)+".log");
					if (!Directory.Exists(LogPath))
						Directory.CreateDirectory(LogPath);
				}
				catch(Exception ex)
				{
					logPathSetupWarning = "Could not apply the given log path / log format, defaults will be used. Exception details:\n" + ex;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(logPathSetupWarning);
					Console.ForegroundColor = ConsoleColor.Gray;
					// Problem with the log path or format use the default
					logFilename = Path.Combine(LogPathDefault, now.ToString(LogFormatDefault) + ".log");
				}
#if DEBUG
				Log.Initialize(logFilename, LogLevel.All, false);
#else
				Log.Initialize(logFilename, LogLevel.All & ~LogLevel.Debug, LogClear);
#endif
				if (logPathSetupWarning != null)
					Log.Warn(logPathSetupWarning);

				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			}
			catch(Exception ex)
			{
				// Will be handled by the server api and written to its crashlog.txt.
				throw new Exception("Fatal TShock initialization exception. See inner exception for details.", ex);
			}

			// Further exceptions are written to TShock's log from now on.
			try
			{
				if (File.Exists(Path.Combine(SavePath, "tshock.pid")))
				{
					Log.ConsoleInfo(
						"TShock was improperly shut down. Please use the exit command in the future to prevent this.");
					File.Delete(Path.Combine(SavePath, "tshock.pid"));
				}
				File.WriteAllText(Path.Combine(SavePath, "tshock.pid"), Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));

				HandleCommandLinePostConfigLoad(Environment.GetCommandLineArgs());

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
						Log.Error(ex.ToString());
						throw new Exception("MySql not setup correctly");
					}
				}
				else
				{
					throw new Exception("Invalid storage type");
				}

				Backups = new BackupManager(Path.Combine(SavePath, "backups"));
				Backups.KeepFor = Config.BackupKeepFor;
				Backups.Interval = Config.BackupInterval;
				Bans = new BanManager(DB);
				Warps = new WarpManager(DB);
                Regions = new RegionManager(DB);
				Users = new UserManager(DB);
				Groups = new GroupManager(DB);
				Itembans = new ItemManager(DB);
				RememberedPos = new RememberedPosManager(DB);
				InventoryDB = new InventoryManager(DB);
				RestApi = new SecureRest(Netplay.serverListenIP, Config.RestApiPort);
				RestApi.Port = Config.RestApiPort;
				RestManager = new RestManager(RestApi);
				RestManager.RegisterRestfulCommands();

				var geoippath = Path.Combine(SavePath, "GeoIP.dat");
				if (Config.EnableGeoIP && File.Exists(geoippath))
					Geo = new GeoIPCountry(geoippath);

				Log.ConsoleInfo(string.Format("|> Version {0} ({1}) now running.", Version, VersionCodename));

				GameHooks.PostInitialize += OnPostInit;
				GameHooks.Update += OnUpdate;
                GameHooks.HardUpdate += OnHardUpdate;
                GameHooks.StatueSpawn += OnStatueSpawn;
				ServerHooks.Connect += OnConnect;
				ServerHooks.Join += OnJoin;
				ServerHooks.Leave += OnLeave;
				ServerHooks.Chat += OnChat;
				ServerHooks.Command += ServerHooks_OnCommand;
				NetHooks.GetData += OnGetData;
				NetHooks.SendData += NetHooks_SendData;
				NetHooks.GreetPlayer += OnGreetPlayer;
				NpcHooks.StrikeNpc += NpcHooks_OnStrikeNpc;
			    NpcHooks.SetDefaultsInt += OnNpcSetDefaults;
				ProjectileHooks.SetDefaults += OnProjectileSetDefaults;
				WorldHooks.StartHardMode += OnStartHardMode;
				WorldHooks.SaveWorld += SaveManager.Instance.OnSaveWorld;
			    WorldHooks.ChristmasCheck += OnXmasCheck;
                NetHooks.NameCollision += NetHooks_NameCollision;
			    TShockAPI.Hooks.PlayerHooks.PlayerPostLogin += OnPlayerLogin;

				GetDataHandlers.InitGetDataHandler();
				Commands.InitCommands();
				//RconHandler.StartThread();

				if (Config.RestApiEnabled)
					RestApi.Start();

				if (Config.BufferPackets)
					PacketBuffer = new PacketBufferer();

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

				GameHooks.PostInitialize -= OnPostInit;
				GameHooks.Update -= OnUpdate;
			    GameHooks.HardUpdate -= OnHardUpdate;
			    GameHooks.StatueSpawn -= OnStatueSpawn;
                ServerHooks.Connect -= OnConnect;
				ServerHooks.Join -= OnJoin;
				ServerHooks.Leave -= OnLeave;
				ServerHooks.Chat -= OnChat;
				ServerHooks.Command -= ServerHooks_OnCommand;
				NetHooks.GetData -= OnGetData;
				NetHooks.SendData -= NetHooks_SendData;
				NetHooks.GreetPlayer -= OnGreetPlayer;
				NpcHooks.StrikeNpc -= NpcHooks_OnStrikeNpc;
                NpcHooks.SetDefaultsInt -= OnNpcSetDefaults;
				ProjectileHooks.SetDefaults -= OnProjectileSetDefaults;
                WorldHooks.StartHardMode -= OnStartHardMode;
				WorldHooks.SaveWorld -= SaveManager.Instance.OnSaveWorld;
                WorldHooks.ChristmasCheck -= OnXmasCheck;
                NetHooks.NameCollision -= NetHooks_NameCollision;
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

        private void NetHooks_NameCollision(int who, string name, HandledEventArgs e)
        {
            string ip = TShock.Utils.GetRealIP(Netplay.serverSock[who].tcpClient.Client.RemoteEndPoint.ToString());
            foreach (TSPlayer ply in TShock.Players)
            {
                if (ply == null)
                {
                    continue;
                }
                if (ply.Name == name && ply.Index != who)
                {
                    if (ply.IP == ip)
                    {
                        if (ply.State < 2)
                        {
                            Utils.ForceKick(ply, "Name collision and this client has no world data.", true, false);
                            e.Handled = true;
                            return;
                        }
                        else
                        {
                            e.Handled = false;
                            return;
                        }
                    }
                }
            }
            e.Handled = false;
            return;
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

		private void OnPostInit()
		{
			SetConsoleTitle();
			if (!File.Exists(Path.Combine(SavePath, "auth.lck")) && !File.Exists(Path.Combine(SavePath, "authcode.txt")))
			{
				var r = new Random((int) DateTime.Now.ToBinary());
				AuthToken = r.Next(100000, 10000000);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("TShock Notice: To become SuperAdmin, join the game and type /auth " + AuthToken);
				Console.WriteLine("This token will display until disabled by verification. (/auth-verify)");
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
				Console.WriteLine("To become superadmin, join the game and type /auth " + AuthToken);
				Console.WriteLine("This token will display until disabled by verification. (/auth-verify)");
				Console.ForegroundColor = ConsoleColor.Gray;
			}
			else
			{
				AuthToken = 0;
			}

            Regions.ReloadAllRegions();

			Lighting.lightMode = 2;
			FixChestStacks();

            
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

		private void OnUpdate()
		{
			UpdateManager.UpdateProcedureCheck();
			if (Backups.IsBackupTime)
				Backups.Backup();
			//call these every second, not every update
			if ((DateTime.UtcNow - LastCheck).TotalSeconds >= 1)
			{
				OnSecondUpdate();
				LastCheck = DateTime.UtcNow;
			}

			if ((DateTime.UtcNow - LastSave).TotalMinutes >= Config.ServerSideInventorySave)
			{
				foreach (TSPlayer player in Players)
				{
					// prevent null point exceptions
					if (player != null && player.IsLoggedIn && !player.IgnoreActionsForClearingTrashCan)
					{

						InventoryDB.InsertPlayerData(player);
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
			int count = 0;
			foreach (TSPlayer player in Players)
			{
				if (player != null && player.Active)
				{
					count++;
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
							player.Disable("Reached TilePlace threshold.");
							TSPlayer.Server.RevertTiles(player.TilesCreated);
							player.TilesCreated.Clear();
						}
					}
					if (player.TilePlaceThreshold > 0)
					{
						player.TilePlaceThreshold = 0;
					}
					if (player.TileLiquidThreshold >= Config.TileLiquidThreshold)
					{
						player.Disable("Reached TileLiquid threshold.");
					}
					if (player.TileLiquidThreshold > 0)
					{
						player.TileLiquidThreshold = 0;
					}
					if (player.ProjectileThreshold >= Config.ProjectileThreshold)
					{
						player.Disable("Reached projectile threshold.");
					}
					if (player.ProjectileThreshold > 0)
					{
						player.ProjectileThreshold = 0;
					}
					if (player.Dead && (DateTime.Now - player.LastDeath).Seconds >= 3 && player.Difficulty != 2)
					{
						player.Spawn();
					}
					string check = "none";
					foreach (Item item in player.TPlayer.inventory)
					{
						if (!player.Group.HasPermission(Permissions.ignorestackhackdetection) && item.stack > item.maxStack &&
							item.type != 0)
						{
							check = "Remove item " + item.name + " (" + item.stack + ") exceeds max stack of " + item.maxStack;
						}
					}
					player.IgnoreActionsForCheating = check;
					check = "none";
					foreach (Item item in player.TPlayer.armor)
					{
						if (!player.Group.HasPermission(Permissions.usebanneditem) && Itembans.ItemIsBanned(item.name, player))
						{
							player.SetBuff(30, 120); //Bleeding
							player.SetBuff(36, 120); //Broken Armor
							check = "Remove armor/accessory " + item.name;
						}
					}
					player.IgnoreActionsForDisabledArmor = check;
					if (CheckIgnores(player))
					{
						player.SetBuff(33, 120); //Weak
						player.SetBuff(32, 120); //Slow
						player.SetBuff(23, 120); //Cursed
					}
					else if (!player.Group.HasPermission(Permissions.usebanneditem) &&
							 Itembans.ItemIsBanned(player.TPlayer.inventory[player.TPlayer.selectedItem].name, player))
					{
						player.SetBuff(23, 120); //Cursed
					}
				}
			}
			SetConsoleTitle();
		}

		private void SetConsoleTitle()
		{
		    Console.Title = string.Format("{0}{1}/{2} @ {3}:{4} (TerrariaShock v{5})",
		                                  !string.IsNullOrWhiteSpace(Config.ServerName) ? Config.ServerName + " - " : "",
		                                  Utils.ActivePlayers(),
		                                  Config.MaxSlots, Netplay.serverListenIP, Netplay.serverPort, Version);
		}

        private void OnHardUpdate( HardUpdateEventArgs args )
        {
            if (args.Handled)
                return;

            if (!Config.AllowCorruptionCreep && ( args.Type == 23 || args.Type == 25 || args.Type == 0 ||
                args.Type == 112 || args.Type == 23 || args.Type == 32 ) )
            {
                args.Handled = true;
                return;
            }

            if (!Config.AllowHallowCreep && (args.Type == 109 || args.Type == 117 || args.Type == 116 ) )
            {
                args.Handled = true;
            }
        }

        private void OnStatueSpawn( StatueSpawnEventArgs args )
        {
            if( args.Within200 < Config.StatueSpawn200 && args.Within600 < Config.StatueSpawn600 && args.WorldWide < Config.StatueSpawnWorld )
            {
                args.Handled = true;
            }
            else
            {
                args.Handled = false;
            }
        }

		private void OnConnect(int ply, HandledEventArgs handler)
		{
			var player = new TSPlayer(ply);

			if (Utils.ActivePlayers() + 1 > Config.MaxSlots + Config.ReservedSlots)
			{
				Utils.ForceKick(player, Config.ServerFullNoReservedReason, true, false);
				handler.Handled = true;
				return;
			}

			var ipban = Bans.GetBanByIp(player.IP);
			Ban ban = null;
			if (ipban != null && Config.EnableIPBans)
				ban = ipban;

			if (ban != null)
			{
				if (!Utils.HasBanExpired(ban))
			    {
			        DateTime exp;
			        string duration = DateTime.TryParse(ban.Expiration, out exp) ? String.Format("until {0}", exp.ToString("G")) : "forever";
			        Utils.ForceKick(player, string.Format("You are banned {0}: {1}", duration, ban.Reason), true, false);
			        handler.Handled = true;
			        return;
			    }
			}

			if (!FileTools.OnWhitelist(player.IP))
			{
				Utils.ForceKick(player, Config.WhitelistKickReason, true, false);
				handler.Handled = true;
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
						handler.Handled = true;
						return;
					}
				}
			}
		    Players[ply] = player;
		}

		private void OnJoin(int ply, HandledEventArgs handler)
		{
			var player = Players[ply];
			if (player == null)
			{
				handler.Handled = true;
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

			if (ban != null)
			{
			    if (!Utils.HasBanExpired(ban))
			    {
			        DateTime exp;
			        string duration = DateTime.TryParse(ban.Expiration, out exp) ? String.Format("until {0}", exp.ToString("G")) : "forever";
			        Utils.ForceKick(player, string.Format("You are banned {0}: {1}", duration, ban.Reason), true, false);
			        handler.Handled = true;
			    }
			}            
		}

		private void OnLeave(int ply)
		{

			var tsplr = Players[ply];
			Players[ply] = null;

			if (tsplr != null && tsplr.ReceivedInfo)
			{
				if (!tsplr.SilentKickInProgress && tsplr.State >= 3)
				{
					Utils.Broadcast(tsplr.Name + " left", Color.Yellow);
				}
				Log.Info(string.Format("{0} disconnected.", tsplr.Name));

				if (tsplr.IsLoggedIn && !tsplr.IgnoreActionsForClearingTrashCan)
				{
					tsplr.PlayerData.CopyInventory(tsplr);
					InventoryDB.InsertPlayerData(tsplr);
				}

				if ((Config.RememberLeavePos) &&(!tsplr.LoginHarassed))
				{
					RememberedPos.InsertLeavePos(tsplr.Name, tsplr.IP, (int) (tsplr.X/16), (int) (tsplr.Y/16));
				}
			}
		}

		private void OnChat(messageBuffer msg, int ply, string text, HandledEventArgs e)
		{
			if (e.Handled)
				return;

			var tsplr = Players[msg.whoAmI];
			if (tsplr == null)
			{
				e.Handled = true;
				return;
			}

			/*if (!Utils.ValidString(text))
			{
				e.Handled = true;
				return;
			}*/

			if (text.StartsWith("/"))
			{
				try
				{
					e.Handled = Commands.HandleCommand(tsplr, text);
				}
				catch (Exception ex)
				{
					Log.ConsoleError("Command exception");
					Log.Error(ex.ToString());
				}
			}
			else if (!tsplr.mute && !TShock.Config.EnableChatAboveHeads)
			{
				Utils.Broadcast(
					String.Format(Config.ChatFormat, tsplr.Group.Name, tsplr.Group.Prefix, tsplr.Name, tsplr.Group.Suffix, text),
					tsplr.Group.R, tsplr.Group.G, tsplr.Group.B);
				e.Handled = true;
			} else if (!tsplr.mute && TShock.Config.EnableChatAboveHeads)
			{
			    Utils.Broadcast(ply, String.Format(Config.ChatAboveHeadsFormat, tsplr.Group.Name, tsplr.Group.Prefix, tsplr.Name, tsplr.Group.Suffix, text), tsplr.Group.R, tsplr.Group.G, tsplr.Group.B);
			    e.Handled = true;
			}
			else if (tsplr.mute)
			{
				tsplr.SendErrorMessage("You are muted!");
				e.Handled = true;
			}
		}

		/// <summary>
		/// When a server command is run.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="e"></param>
		private void ServerHooks_OnCommand(string text, HandledEventArgs e)
		{
			if (e.Handled)
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

			if (text.StartsWith("playing") || text.StartsWith("/playing"))
			{
				int count = 0;
				foreach (TSPlayer player in Players)
				{
					if (player != null && player.Active)
					{
						count++;
						TSPlayer.Server.SendInfoMessage(string.Format("{0} ({1}) [{2}] <{3}>", player.Name, player.IP,
																  player.Group.Name, player.UserAccountName));
					}
				}
				TSPlayer.Server.SendInfoMessage(string.Format("{0} players connected.", count));
			}
			else if (text == "autosave")
			{
				Main.autoSave = Config.AutoSave = !Config.AutoSave;
				Log.ConsoleInfo("AutoSave " + (Config.AutoSave ? "Enabled" : "Disabled"));
			}
			else if (text.StartsWith("/"))
			{
				Commands.HandleCommand(TSPlayer.Server, text);
			}
			else
			{
				Commands.HandleCommand(TSPlayer.Server, "/" + text);
			}
			e.Handled = true;
		}

		private void OnGetData(GetDataEventArgs e)
		{
			if (e.Handled)
				return;

			PacketTypes type = e.MsgID;

			Debug.WriteLine("Recv: {0:X}: {2} ({1:XX})", e.Msg.whoAmI, (byte) type, type);

			var player = Players[e.Msg.whoAmI];
			if (player == null)
			{
				e.Handled = true;
				return;
			}

			if (!player.ConnectionAlive)
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
				(int) type != 38 && (int) type != 21)
			{
				e.Handled = true;
				return;
			}

			using (var data = new MemoryStream(e.Msg.readBuffer, e.Index, e.Length))
			{
				try
				{
					if (GetDataHandlers.HandlerGetData(type, player, data))
						e.Handled = true;
				}
				catch (Exception ex)
				{
					Log.Error(ex.ToString());
				}
			}
		}

		private void OnGreetPlayer(int who, HandledEventArgs e)
		{
			var player = Players[who];
			if (player == null)
			{
				e.Handled = true;
				return;
			}
			player.LoginMS= DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			
			Utils.ShowFileToUser(player, "motd.txt");

			if (Config.PvPMode == "always" && !player.TPlayer.hostile)
			{
				player.SendMessage("PvP is forced! Enable PvP else you can't do anything!", Color.Red);
			}

			if (!player.IsLoggedIn)
			{
				if (Config.ServerSideInventory)
				{
					player.SendMessage(
						player.IgnoreActionsForInventory = "Server side inventory is enabled! Please /register or /login to play!",
						Color.Red);
						player.LoginHarassed = true;
				}
				else if (Config.RequireLogin)
				{
					player.SendMessage("Please /register or /login to play!", Color.Red);
					player.LoginHarassed = true;
				}
			}

			if (player.Group.HasPermission(Permissions.causeevents) && Config.InfiniteInvasion)
			{
				StartInvasion();
			}

			player.LastNetPosition = new Vector2(Main.spawnTileX*16f, Main.spawnTileY*16f);

			if (Config.RememberLeavePos)
			{
			if (RememberedPos.GetLeavePos(player.Name, player.IP) != Vector2.Zero){
				var pos = RememberedPos.GetLeavePos(player.Name, player.IP);

				player.Teleport((int) pos.X, (int) pos.Y + 3);
			}}

			e.Handled = true;
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
			if (e.Info == 43)
				if (Config.DisableTombstones)
					e.Object.SetDefaults(0);
			if (e.Info == 75)
				if (Config.DisableClownBombs)
					e.Object.SetDefaults(0);
			if (e.Info == 109)
				if (Config.DisableSnowBalls)
					e.Object.SetDefaults(0);
		}

		private void OnNpcSetDefaults(SetDefaultsEventArgs<NPC, int> e)
		{
			if (Itembans.ItemIsBanned(e.Object.name, null))
			{
				e.Object.SetDefaults(0);
			}
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
			if (e.MsgID == PacketTypes.Disconnect)
			{
				Action<ServerSock, string> senddisconnect = (sock, str) =>
																{
																	if (sock == null || !sock.active)
																		return;
																	sock.kill = true;
																	using (var ms = new MemoryStream())
																	{
																		new DisconnectMsg {Reason = str}.PackFull(ms);
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
			}
            if (e.MsgID == PacketTypes.WorldInfo)
            {
                if (e.remoteClient == -1) return;
                var player = Players[e.remoteClient];
                if (player == null) return;
                if (Config.UseServerName)
                {
                    using (var ms = new MemoryStream())
                    {
                        var msg = new WorldInfoMsg
                        {
                            Time = (int)Main.time,
                            DayTime = Main.dayTime,
                            MoonPhase = (byte)Main.moonPhase,
                            BloodMoon = Main.bloodMoon,
                            MaxTilesX = Main.maxTilesX,
                            MaxTilesY = Main.maxTilesY,
                            SpawnX = Main.spawnTileX,
                            SpawnY = Main.spawnTileY,
                            WorldSurface = (int)Main.worldSurface,
                            RockLayer = (int)Main.rockLayer,
                            WorldID = Main.worldID,
                            WorldFlags =
                                (WorldGen.shadowOrbSmashed ? WorldInfoFlag.OrbSmashed : WorldInfoFlag.None) |
                                (NPC.downedBoss1 ? WorldInfoFlag.DownedBoss1 : WorldInfoFlag.None) |
                                (NPC.downedBoss2 ? WorldInfoFlag.DownedBoss2 : WorldInfoFlag.None) |
                                (NPC.downedBoss3 ? WorldInfoFlag.DownedBoss3 : WorldInfoFlag.None) |
                                (Main.hardMode ? WorldInfoFlag.HardMode : WorldInfoFlag.None) |
                                (NPC.downedClown ? WorldInfoFlag.DownedClown : WorldInfoFlag.None),
                            WorldName = Config.ServerName
                        };
                        msg.PackFull(ms);
                        player.SendRawData(ms.ToArray());
                    }
                    e.Handled = true;
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

		public static void StartInvasion()
		{
			Main.invasionType = 1;
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

			if (type == 17 && !player.Group.HasPermission(Permissions.usebanneditem) && Itembans.ItemIsBanned("Dirt Rod", player))
				//Dirt Rod Projectile
			{
				return true;
			}

			if ((type == 42 || type == 65 || type == 68) && !player.Group.HasPermission(Permissions.usebanneditem) &&
				Itembans.ItemIsBanned("Sandgun", player)) //Sandgun Projectiles
			{
				return true;
			}

			Projectile proj = new Projectile();
			proj.SetDefaults(type);

			if (!player.Group.HasPermission(Permissions.usebanneditem) && Itembans.ItemIsBanned(proj.name, player))
			{
				return true;
			}

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

        public static bool CheckTilePermission( TSPlayer player, int tileX, int tileY, byte tileType, byte actionType )
        {
            if (!player.Group.HasPermission(Permissions.canbuild))
            {
				if (TShock.Config.AllowIce && actionType != 1)
				{

					foreach (Point p in player.IceTiles)
					{
						if (p.X == tileX && p.Y == tileY && (Main.tile[p.X, p.Y].type == 0 || Main.tile[p.X, p.Y].type == 127))
						{
							player.IceTiles.Remove(p);
							return false;
						}
					}

    		        if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.BPm) > 2000){
					    player.SendMessage("You do not have permission to build!", Color.Red);
			            player.BPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                    }

					return true;
				}

				if (TShock.Config.AllowIce && actionType == 1 && tileType == 127)
				{
					player.IceTiles.Add(new Point(tileX, tileY));
					return false;
				}
				
		        if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.BPm) > 2000){
					player.SendMessage("You do not have permission to build!", Color.Red);
			        player.BPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                }   

				return true;

            }

            if (!player.Group.HasPermission(Permissions.editspawn) && !Regions.CanBuild(tileX, tileY, player) &&
                Regions.InArea(tileX, tileY))
            {
                if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.RPm) > 2000)
                {
                    player.SendMessage("This region is protected from changes.", Color.Red);
                    player.RPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                }
                return true;
            }

            if (Config.DisableBuild)
            {
                if (!player.Group.HasPermission(Permissions.editspawn))
                {
 		    if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.WPm) > 2000){
                        player.SendMessage("The world is protected from changes.", Color.Red);
			player.WPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

}
                    return true;
                }
            }
            if (Config.SpawnProtection)
            {
                if (!player.Group.HasPermission(Permissions.editspawn))
                {
                    var flag = CheckSpawn(tileX, tileY);
                    if (flag)
                    {		
					if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.SPm) > 2000){
                        player.SendMessage("Spawn is protected from changes.", Color.Red);
						player.SPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
						}
                    return true;
                    }
                }
            }
            return false;
        }

		public static bool CheckTilePermission(TSPlayer player, int tileX, int tileY)
		{
			if (!player.Group.HasPermission(Permissions.canbuild))
			{

		    if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.BPm) > 2000){
					player.SendMessage("You do not have permission to build!", Color.Red);
					player.BPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
					}
				return true;
			}

            if (!player.Group.HasPermission(Permissions.editspawn) && !Regions.CanBuild(tileX, tileY, player) &&
                    Regions.InArea(tileX, tileY))
            {


                if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.RPm) > 2000)
                {
                    player.SendMessage("This region is protected from changes.", Color.Red);
                    player.RPm = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                }
                return true;
            }

			if (Config.DisableBuild)
			{
				if (!player.Group.HasPermission(Permissions.editspawn))
				{
				if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.WPm) > 2000){
                        player.SendMessage("The world is protected from changes.", Color.Red);
						player.WPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
						}
					return true;
				}
			}
			if (Config.SpawnProtection)
			{
				if (!player.Group.HasPermission(Permissions.editspawn))
				{
					var flag = CheckSpawn(tileX, tileY);
					if (flag)
					{
					if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.SPm) > 1000){
                        player.SendMessage("Spawn is protected from changes.", Color.Red);
						player.SPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

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

		public static bool HackedStats(TSPlayer player)
		{
            return (player.TPlayer.statManaMax > TShock.Config.MaxMana) ||
                   (player.TPlayer.statMana > TShock.Config.MaxMana) ||
                   (player.TPlayer.statLifeMax > TShock.Config.MaxHealth) ||
                   (player.TPlayer.statLife > TShock.Config.MaxHealth);
		}

		public static bool HackedInventory(TSPlayer player)
		{
			bool check = false;

			Item[] inventory = player.TPlayer.inventory;
			Item[] armor = player.TPlayer.armor;
			for (int i = 0; i < NetItem.maxNetInventory; i++)
			{
				if (i < 49)
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
				else
				{
					Item item = new Item();
					if (armor[i - 48] != null && armor[i - 48].netID != 0)
					{
						item.netDefaults(armor[i - 48].netID);
						item.Prefix(armor[i - 48].prefix);
						item.AffixName();
						if (armor[i - 48].stack > item.maxStack)
						{
							check = true;
							player.SendMessage(
								String.Format("Stack cheat detected. Remove armor {0} ({1}) and then rejoin", item.name, armor[i - 48].stack),
								Color.Cyan);
						}
					}
				}
			}

			return check;
		}

		public static bool CheckInventory(TSPlayer player)
		{
			PlayerData playerData = player.PlayerData;
			bool check = true;

			if (player.TPlayer.statLifeMax > playerData.maxHealth)
			{
				player.SendMessage("Error: Your max health exceeded (" + playerData.maxHealth + ") which is stored on server.",
								   Color.Cyan);
				check = false;
			}

			Item[] inventory = player.TPlayer.inventory;
			Item[] armor = player.TPlayer.armor;
			for (int i = 0; i < NetItem.maxNetInventory; i++)
			{
				if (i < 49)
				{
					Item item = new Item();
					Item serverItem = new Item();
					if (inventory[i] != null && inventory[i].netID != 0)
					{
						if (playerData.inventory[i].netID != inventory[i].netID)
						{
							item.netDefaults(inventory[i].netID);
							item.Prefix(inventory[i].prefix);
							item.AffixName();
							player.SendMessage(player.IgnoreActionsForInventory = "Your item (" + item.name + ") needs to be deleted.",
											   Color.Cyan);
							check = false;
						}
						else if (playerData.inventory[i].prefix != inventory[i].prefix)
						{
							item.netDefaults(inventory[i].netID);
							item.Prefix(inventory[i].prefix);
							item.AffixName();
							player.SendMessage(player.IgnoreActionsForInventory = "Your item (" + item.name + ") needs to be deleted.",
											   Color.Cyan);
							check = false;
						}
						else if (inventory[i].stack > playerData.inventory[i].stack)
						{
							item.netDefaults(inventory[i].netID);
							item.Prefix(inventory[i].prefix);
							item.AffixName();
							player.SendMessage(
								player.IgnoreActionsForInventory =
								"Your item (" + item.name + ") (" + inventory[i].stack + ") needs to have its stack size decreased to (" +
								playerData.inventory[i].stack + ").", Color.Cyan);
							check = false;
						}
					}
				}
				else
				{
					Item item = new Item();
					Item serverItem = new Item();
					if (armor[i - 48] != null && armor[i - 48].netID != 0)
					{
						if (playerData.inventory[i].netID != armor[i - 48].netID)
						{
							item.netDefaults(armor[i - 48].netID);
							item.Prefix(armor[i - 48].prefix);
							item.AffixName();
							player.SendMessage(player.IgnoreActionsForInventory = "Your armor (" + item.name + ") needs to be deleted.",
											   Color.Cyan);
							check = false;
						}
						else if (playerData.inventory[i].prefix != armor[i - 48].prefix)
						{
							item.netDefaults(armor[i - 48].netID);
							item.Prefix(armor[i - 48].prefix);
							item.AffixName();
							player.SendMessage(player.IgnoreActionsForInventory = "Your armor (" + item.name + ") needs to be deleted.",
											   Color.Cyan);
							check = false;
						}
						else if (armor[i - 48].stack > playerData.inventory[i].stack)
						{
							item.netDefaults(armor[i - 48].netID);
							item.Prefix(armor[i - 48].prefix);
							item.AffixName();
							player.SendMessage(
								player.IgnoreActionsForInventory =
								"Your armor (" + item.name + ") (" + inventory[i].stack + ") needs to have its stack size decreased to (" +
								playerData.inventory[i].stack + ").", Color.Cyan);
							check = false;
						}
					}
				}
			}

			return check;
		}

		public static bool CheckIgnores(TSPlayer player)
		{
			bool check = false;
			if (Config.PvPMode == "always" && !player.TPlayer.hostile)
				check = true;
			if (player.IgnoreActionsForInventory != "none")
				check = true;
			if (player.IgnoreActionsForCheating != "none")
				check = true;
			if (player.IgnoreActionsForDisabledArmor != "none")
				check = true;
			if (player.IgnoreActionsForClearingTrashCan)
				check = true;
			if (!player.IsLoggedIn && Config.RequireLogin)
				check = true;
			return check;
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

            file.ServerName = file.ServerNickname;
		}
	}
}
