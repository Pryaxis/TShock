/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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
using System.Net;
using System.Reflection;
using System.Threading;
using Hooks;
using MaxMind;
using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using Rests;
using Terraria;
using TShockAPI.DB;
using TShockAPI.Net;

namespace TShockAPI
{
	[APIVersion(1, 11)]
	public class TShock : TerrariaPlugin
	{
		private const string LogFormatDefault = "yyyyMMddHHmmss";
		private static string LogFormat = LogFormatDefault;
		private static bool LogClear = false;
		public static readonly Version VersionNum = Assembly.GetExecutingAssembly().GetName().Version;
		public static readonly string VersionCodename = "Squashing bugs, and adding suggestions";

		public static string SavePath = "tshock";

		public static TSPlayer[] Players = new TSPlayer[Main.maxPlayers];
		public static BanManager Bans;
		public static WarpManager Warps;
		public static RegionManager Regions;
		public static BackupManager Backups;
		public static GroupManager Groups;
		public static UserManager Users;
		public static ItemManager Itembans;
		public static RemeberedPosManager RememberedPos;
		public static InventoryManager InventoryDB;
		public static ConfigFile Config { get; set; }
		public static IDbConnection DB;
		public static bool OverridePort;
		public static PacketBufferer PacketBuffer;
		public static GeoIPCountry Geo;
		public static SecureRest RestApi;
		public static RestManager RestManager;
		public static Utils Utils = Utils.Instance;
		public static StatTracker StatTracker = new StatTracker();
		/// <summary>
		/// Used for implementing REST Tokens prior to the REST system starting up.
		/// </summary>
		public static Dictionary<string, string> RESTStartupTokens = new Dictionary<string, string>();

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

		public TShock(Main game)
			: base(game)
		{
			Config = new ConfigFile();
			Order = 0;
		}


		[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
		public override void Initialize()
		{
			HandleCommandLine(Environment.GetCommandLineArgs());

			if (!Directory.Exists(SavePath))
				Directory.CreateDirectory(SavePath);

			DateTime now = DateTime.Now;
			string logFilename;
			try
			{
				logFilename = Path.Combine(SavePath, now.ToString(LogFormat)+".log");
			}
			catch(Exception)
			{
				// Problem with the log format use the default
				logFilename = Path.Combine(SavePath, now.ToString(LogFormatDefault) + ".log");
			}
#if DEBUG
			Log.Initialize(logFilename, LogLevel.All, false);
#else
			Log.Initialize(logFilename, LogLevel.All & ~LogLevel.Debug, LogClear);
#endif
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			try
			{
				if (File.Exists(Path.Combine(SavePath, "tshock.pid")))
				{
					Log.ConsoleInfo(
						"TShock was improperly shut down. Please avoid this in the future, world corruption may result from this.");
					File.Delete(Path.Combine(SavePath, "tshock.pid"));
				}
				File.WriteAllText(Path.Combine(SavePath, "tshock.pid"), Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture));

				ConfigFile.ConfigRead += OnConfigRead;
				FileTools.SetupConfig();

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
				Users = new UserManager(DB);
				Groups = new GroupManager(DB);
				Regions = new RegionManager(DB);
				Itembans = new ItemManager(DB);
				RememberedPos = new RemeberedPosManager(DB);
				InventoryDB = new InventoryManager(DB);
				RestApi = new SecureRest(Netplay.serverListenIP, Config.RestApiPort);
				RestApi.Verify += RestApi_Verify;
				RestApi.Port = Config.RestApiPort;
				RestManager = new RestManager(RestApi);
				RestManager.RegisterRestfulCommands();

				var geoippath = Path.Combine(SavePath, "GeoIP.dat");
				if (Config.EnableGeoIP && File.Exists(geoippath))
					Geo = new GeoIPCountry(geoippath);

				Log.ConsoleInfo(string.Format("TerrariaShock Version {0} ({1}) now running.", Version, VersionCodename));

				GameHooks.PostInitialize += OnPostInit;
				GameHooks.Update += OnUpdate;
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

		private RestObject RestApi_Verify(string username, string password)
		{
			var userAccount = Users.GetUserByName(username);
			if (userAccount == null)
			{
				return new RestObject("401")
						{Error = "Invalid username/password combination provided. Please re-submit your query with a correct pair."};
			}

			if (Utils.HashPassword(password).ToUpper() != userAccount.Password.ToUpper())
			{
				return new RestObject("401")
						{Error = "Invalid username/password combination provided. Please re-submit your query with a correct pair."};
			}

			if (!Utils.GetGroup(userAccount.Group).HasPermission("api") && userAccount.Group != "superadmin")
			{
				return new RestObject("403")
						{
							Error =
								"Although your account was successfully found and identified, your account lacks the permission required to use the API. (api)"
						};
			}

			return new RestObject("200") {Response = "Successful login"}; //Maybe return some user info too?
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

				if (File.Exists(Path.Combine(SavePath, "tshock.pid")))
				{
					File.Delete(Path.Combine(SavePath, "tshock.pid"));
				}

				RestApi.Dispose();
				Log.Dispose();
			}
			base.Dispose(disposing);
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

					case "-dump":
						ConfigFile.DumpDescriptions();
						Permissions.DumpDescriptions();
						break;

					case "-logformat":
						LogFormat = parms[++i];
						break;

					case "-logclear":
						bool.TryParse(parms[++i], out LogClear);
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
						RESTStartupTokens.Add(token, "null");
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

			StatTracker.CheckIn();
			FixChestStacks();
		}

		private void FixChestStacks()
		{
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
			StatTracker.CheckIn();
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
							player.Disable("Reached TileKill threshold");
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
						player.Disable("Reached Projectile threshold");
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
							check = "Remove Item " + item.name + " (" + item.stack + ") exceeds max stack of " + item.maxStack;
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
							check = "Remove Armor/Accessory " + item.name;
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
			Console.Title = string.Format("{0} - {1}/{2} @ {3}:{4} (TerrariaShock v{5})", Config.ServerName, Utils.ActivePlayers(),
								  Config.MaxSlots, Netplay.serverListenIP, Config.ServerPort, Version);
		}

		private void OnConnect(int ply, HandledEventArgs handler)
		{
			var player = new TSPlayer(ply);
			if (Config.EnableDNSHostResolution)
			{
				player.Group = Users.GetGroupForIPExpensive(player.IP);
			}
			else
			{
				player.Group = Users.GetGroupForIP(player.IP);
			}

			if (Utils.ActivePlayers() + 1 > Config.MaxSlots + 20)
			{
				Utils.ForceKick(player, Config.ServerFullNoReservedReason);
				handler.Handled = true;
				return;
			}

			var ipban = Bans.GetBanByIp(player.IP);
			Ban ban = null;
			if (ipban != null && Config.EnableIPBans)
				ban = ipban;

			if (ban != null)
			{
				Utils.ForceKick(player, string.Format("You are banned: {0}", ban.Reason));
				handler.Handled = true;
				return;
			}

			if (!FileTools.OnWhitelist(player.IP))
			{
				Utils.ForceKick(player, "Not on whitelist.");
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
						Utils.ForceKick(player, "Proxies are not allowed");
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
				Utils.ForceKick(player, string.Format("You are banned: {0}", ban.Reason));
				handler.Handled = true;
				return;
			}
		}

		private void OnLeave(int ply)
		{

			var tsplr = Players[ply];
			Players[ply] = null;

			if (tsplr != null && tsplr.ReceivedInfo)
			{
				if (!tsplr.SilentKickInProgress)
				{
					Utils.Broadcast(tsplr.Name + " left", Color.Yellow);
				}
				Log.Info(string.Format("{0} left.", tsplr.Name));

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
			else if (!tsplr.mute)
			{
				Utils.Broadcast(
					String.Format(Config.ChatFormat, tsplr.Group.Name, tsplr.Group.Prefix, tsplr.Name, tsplr.Group.Suffix, text),
					tsplr.Group.R, tsplr.Group.G, tsplr.Group.B);
				e.Handled = true;
			}
			else if (tsplr.mute)
			{
				tsplr.SendMessage("You are muted!");
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
						TSPlayer.Server.SendMessage(string.Format("{0} ({1}) [{2}] <{3}>", player.Name, player.IP,
																  player.Group.Name, player.UserAccountName));
					}
				}
				TSPlayer.Server.SendMessage(string.Format("{0} players connected.", count));
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
				(int) type != 38 && (int) type != 5 && (int) type != 21)
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
				player.SendMessage("PvP is forced! Enable PvP else you can't move or do anything!", Color.Red);
			}

			if (!player.IsLoggedIn)
			{
				if (Config.ServerSideInventory)
				{
					player.SendMessage(
						player.IgnoreActionsForInventory = "Server Side Inventory is enabled! Please /register or /login to play!",
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
						Utils.Broadcast(string.Format("You call that a lot? {0} goblins killed!", KillCount));
						break;
					case 1:
						Utils.Broadcast(string.Format("Fatality! {0} goblins killed!", KillCount));
						break;
					case 2:
						Utils.Broadcast(string.Format("Number of 'noobs' killed to date: {0}", KillCount));
						break;
					case 3:
						Utils.Broadcast(string.Format("Duke Nukem would be proud. {0} goblins killed.", KillCount));
						break;
					case 4:
						Utils.Broadcast(string.Format("You call that a lot? {0} goblins killed!", KillCount));
						break;
					case 5:
						Utils.Broadcast(string.Format("{0} copies of Call of Duty smashed.", KillCount));
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
                		    if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.RPm) > 2000){
                        player.SendMessage("Region protected from changes.", Color.Red);
			player.RPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

}
                return true;
            }
            if (Config.DisableBuild)
            {
                if (!player.Group.HasPermission(Permissions.editspawn))
                {
 		    if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.WPm) > 2000){
                        player.SendMessage("World protected from changes.", Color.Red);
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
                        player.SendMessage("Spawn protected from changes.", Color.Red);
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


		    if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.RPm) > 2000){
                        player.SendMessage("Region protected from changes.", Color.Red);
						player.RPm=DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
						}
				return true;
			}
			
			if (Config.DisableBuild)
			{
				if (!player.Group.HasPermission(Permissions.editspawn))
				{
				if (((DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - player.WPm) > 2000){
                        player.SendMessage("World protected from changes.", Color.Red);
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
                        player.SendMessage("Spawn protected from changes.", Color.Red);
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

		public static bool HackedHealth(TSPlayer player)
		{
			return (player.TPlayer.statManaMax > 400) ||
				   (player.TPlayer.statMana > 400) ||
				   (player.TPlayer.statLifeMax > 400) ||
				   (player.TPlayer.statLife > 400);
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
				player.SendMessage("Error: Your max health exceeded (" + playerData.maxHealth + ") which is stored on server",
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
								"Your item (" + item.name + ") (" + inventory[i].stack + ") needs to have it's stack decreased to (" +
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
								"Your armor (" + item.name + ") (" + inventory[i].stack + ") needs to have it's stack decreased to (" +
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
		}
	}
}
