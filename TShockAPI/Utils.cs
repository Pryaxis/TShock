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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Terraria;
using TShockAPI.DB;
using BCrypt.Net;

namespace TShockAPI
{
	/// <summary>
	/// Utilities and other TShock core calls that don't fit anywhere else
	/// </summary>
	public class Utils
	{
		/// <summary>
		/// The lowest id for a prefix.
		/// </summary>
		private const int FirstItemPrefix = 1;

		/// <summary>
		/// The highest id for a prefix.
		/// </summary>
		private const int LastItemPrefix = 83;

		/// <summary>instance - an instance of the utils class</summary>
		private static readonly Utils instance = new Utils();

		/// <summary>Utils - Creates a utilities object.</summary>
		private Utils() {}

		/// <summary>Instance - An instance of the utils class.</summary>
		/// <value>value - the Utils instance</value>
		public static Utils Instance { get { return instance; } }

		/// <summary>Random - An instance of random for generating random data.</summary>
		[Obsolete("Please create your own random objects; this will be removed in the next version of TShock.")]
		public Random Random = new Random();

		/// <summary>
		/// Provides the real IP address from a RemoteEndPoint string that contains a port and an IP
		/// </summary>
		/// <param name="mess">A string IPv4 address in IP:PORT form.</param>
		/// <returns>A string IPv4 address.</returns>
		public string GetRealIP(string mess)
		{
			return mess.Split(':')[0];
		}

		/// <summary>
		/// Returns a list of current players on the server
		/// </summary>
		/// <param name="includeIDs">bool includeIDs - whether or not the string of each player name should include ID data</param>
		/// <returns>List of strings with names</returns>
		public List<string> GetPlayers(bool includeIDs)
		{
			var players = new List<string>();

			foreach (TSPlayer ply in TShock.Players)
			{
					if (ply != null && ply.Active)
					{
							if (includeIDs)
							{
									players.Add(ply.Name + " (IX: " + ply.Index + ", ID: " + ply.User.ID + ")");
							}
							else
							{
									players.Add(ply.Name);
							}
					}
			}

			return players;
		}

		/// <summary>
		/// Finds a player and gets IP as string
		/// </summary>
		/// <param name="playername">string playername</param>
		public string GetPlayerIP(string playername)
		{
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.Active)
				{
					if (playername.ToLower() == player.Name.ToLower())
					{
						return player.IP;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// It's a clamp function
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="value">Value to clamp</param>
		/// <param name="max">Maximum bounds of the clamp</param>
		/// <param name="min">Minimum bounds of the clamp</param>
		/// <returns></returns>
		public T Clamp<T>(T value, T max, T min)
			where T : IComparable<T>
		{
			T result = value;
			if (value.CompareTo(max) > 0)
				result = max;
			if (value.CompareTo(min) < 0)
				result = min;
			return result;
		}

		/// <summary>
		/// Saves the map data by calling the SaveManager and instructing it to save the world.
		/// </summary>
		public void SaveWorld()
		{
			SaveManager.Instance.SaveWorld();
		}

		/// <summary>Broadcast - Broadcasts a message to all players on the server, as well as the server console, and the logs.</summary>
		/// <param name="msg">msg - The message to send</param>
		/// <param name="red">red - The amount of red (0-255) in the color for supported destinations.</param>
		/// <param name="green">green - The amount of green (0-255) in the color for supported destinations.</param>
		/// <param name="blue">blue - The amount of blue (0-255) in the color for the supported destinations.</param>
		public void Broadcast(string msg, byte red, byte green, byte blue)
		{
			TSPlayer.All.SendMessage(msg, red, green, blue);
			TSPlayer.Server.SendMessage(msg, red, green, blue);
			TShock.Log.Info(string.Format("Broadcast: {0}", msg));
		}

		/// <summary>>Broadcast - Broadcasts a message to all players on the server, as well as the server console, and the logs.</summary>
		/// <param name="msg">msg - The message to send</param>
		/// <param name="color">color - The color object for supported destinations.</param>
		public void Broadcast(string msg, Color color)
		{
			Broadcast(msg, color.R, color.G, color.B);
		}

		/// <summary>
		/// Broadcasts a message from a Terraria playerplayer, not TShock
		/// </summary>
		/// <param name="ply">ply - the Terraria player index that will send the packet</param>
		/// <param name="msg">msg - The message to send</param>
		/// <param name="red">red - The amount of red (0-255) in the color for supported destinations.</param>
		/// <param name="green">green - The amount of green (0-255) in the color for supported destinations.</param>
		/// <param name="blue">blue - The amount of blue (0-255) in the color for the supported destinations.</param>
		public void Broadcast(int ply, string msg, byte red, byte green, byte blue)
		{
			TSPlayer.All.SendMessageFromPlayer(msg, red, green, blue, ply);
			TSPlayer.Server.SendMessage(Main.player[ply].name + ": " + msg, red, green, blue);
			TShock.Log.Info(string.Format("Broadcast: {0}", Main.player[ply].name + ": " + msg));
		}

		/// <summary>
		/// Sends message to all players with 'logs' permission.
		/// </summary>
		/// <param name="log">Message to send</param>
		/// <param name="color">Color of the message</param>
		/// <param name="excludedPlayer">The player to not send the message to.</param>
		public void SendLogs(string log, Color color, TSPlayer excludedPlayer = null)
		{
			TShock.Log.Info(log);
			TSPlayer.Server.SendMessage(log, color);
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player != excludedPlayer && player.Active && player.Group.HasPermission(Permissions.logs) && 
						player.DisplayLogs && TShock.Config.DisableSpewLogs == false)
					player.SendMessage(log, color);
			}
		}

		/// <summary>
		/// Gets the number of active players on the server.
		/// </summary>
		/// <returns>The number of active players on the server.</returns>
		public int ActivePlayers()
		{
			return Main.player.Where(p => null != p && p.active).Count();
		}

		/// <summary>
		/// Finds a TSPlayer based on name or ID
		/// </summary>
		/// <param name="plr">Player name or ID</param>
		/// <returns>A list of matching players</returns>
		public List<TSPlayer> FindPlayer(string plr)
		{
			var found = new List<TSPlayer>();
			// Avoid errors caused by null search
			if (plr == null)
				return found;

			byte plrID;
			if (byte.TryParse(plr, out plrID) && plrID < Main.maxPlayers)
			{
				TSPlayer player = TShock.Players[plrID];
				if (player != null && player.Active)
				{
					return new List<TSPlayer> { player };
				}
			}

			string plrLower = plr.ToLower();
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null)
				{
					// Must be an EXACT match
					if (player.Name == plr)
						return new List<TSPlayer> { player };
					if (player.Name.ToLower().StartsWith(plrLower))
						found.Add(player);
				}
			}
			return found;
		}

		/// <summary>
		/// Gets a random clear tile in range
		/// </summary>
		/// <param name="startTileX">Bound X</param>
		/// <param name="startTileY">Bound Y</param>
		/// <param name="tileXRange">Range on the X axis</param>
		/// <param name="tileYRange">Range on the Y axis</param>
		/// <param name="tileX">X location</param>
		/// <param name="tileY">Y location</param>
		public void GetRandomClearTileWithInRange(int startTileX, int startTileY, int tileXRange, int tileYRange,
																							out int tileX, out int tileY)
		{
			int j = 0;
			do
			{
				if (j == 100)
				{
					tileX = startTileX;
					tileY = startTileY;
					break;
				}
				Random r = new Random();
				tileX = startTileX + r.Next(tileXRange*-1, tileXRange);
				tileY = startTileY + r.Next(tileYRange*-1, tileYRange);
				j++;
			} while (TilePlacementValid(tileX, tileY) && TileSolid(tileX, tileY));
		}

		/// <summary>
		/// Determines if a tile is valid.
		/// </summary>
		/// <param name="tileX">Location X</param>
		/// <param name="tileY">Location Y</param>
		/// <returns>If the tile is valid</returns>
		public bool TilePlacementValid(int tileX, int tileY)
		{
			return tileX >= 0 && tileX < Main.maxTilesX && tileY >= 0 && tileY < Main.maxTilesY;
		}

		/// <summary>
		/// Checks if the tile is solid.
		/// </summary>
		/// <param name="tileX">Location X</param>
		/// <param name="tileY">Location Y</param>
		/// <returns>The tile's solidity.</returns>
		public bool TileSolid(int tileX, int tileY)
		{
			return TilePlacementValid(tileX, tileY) && Main.tile[tileX, tileY] != null &&
				Main.tile[tileX, tileY].active() && Main.tileSolid[Main.tile[tileX, tileY].type] &&
				!Main.tile[tileX, tileY].inActive() && !Main.tile[tileX, tileY].halfBrick() && Main.tile[tileX, tileY].slope() == 0;
		}

		/// <summary>
		/// Gets a list of items by ID or name
		/// </summary>
		/// <param name="idOrName">Item ID or name</param>
		/// <returns>List of Items</returns>
		public List<Item> GetItemByIdOrName(string idOrName)
		{
			int type = -1;
			if (int.TryParse(idOrName, out type))
			{
				if (type >= Main.maxItemTypes)
					return new List<Item>();
				return new List<Item> {GetItemById(type)};
			}
			return GetItemByName(idOrName);
		}

		/// <summary>
		/// Gets an item by ID
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>Item</returns>
		public Item GetItemById(int id)
		{
			Item item = new Item();
			item.netDefaults(id);
			return item;
		}

		/// <summary>
		/// Gets items by name
		/// </summary>
		/// <param name="name">name</param>
		/// <returns>List of Items</returns>
		public List<Item> GetItemByName(string name)
		{
			var found = new List<Item>();
			Item item = new Item();
			string nameLower = name.ToLower();
			for (int i = -48; i < Main.maxItemTypes; i++)
			{
				item.netDefaults(i);
				if (item.name.ToLower() == nameLower)
					return new List<Item> {item};
				if (item.name.ToLower().StartsWith(nameLower))
					found.Add((Item)item.Clone());
			}
			return found;
		}

		/// <summary>
		/// Gets an NPC by ID or Name
		/// </summary>
		/// <param name="idOrName"></param>
		/// <returns>List of NPCs</returns>
		public List<NPC> GetNPCByIdOrName(string idOrName)
		{
			int type = -1;
			if (int.TryParse(idOrName, out type))
			{
				if (type >= Main.maxNPCTypes)
					return new List<NPC>();
				return new List<NPC> { GetNPCById(type) };
			}
			return GetNPCByName(idOrName);
		}

		/// <summary>
		/// Gets an NPC by ID
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>NPC</returns>
		public NPC GetNPCById(int id)
		{
			NPC npc = new NPC();
			npc.netDefaults(id);
			return npc;
		}

		/// <summary>
		/// Gets a NPC by name
		/// </summary>
		/// <param name="name">Name</param>
		/// <returns>List of matching NPCs</returns>
		public List<NPC> GetNPCByName(string name)
		{
			var found = new List<NPC>();
			NPC npc = new NPC();
			string nameLower = name.ToLower();
			for (int i = -17; i < Main.maxNPCTypes; i++)
			{
				npc.netDefaults(i);
				if (npc.name.ToLower() == nameLower)
					return new List<NPC> { npc };
				if (npc.name.ToLower().StartsWith(nameLower))
					found.Add((NPC)npc.Clone());
			}
			return found;
		}

		/// <summary>
		/// Gets a buff name by id
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>name</returns>
		public string GetBuffName(int id)
		{
			return (id > 0 && id < Main.maxBuffTypes) ? Main.buffName[id] : "null";
		}

		/// <summary>
		/// Gets the description of a buff
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>description</returns>
		public string GetBuffDescription(int id)
		{
			return (id > 0 && id < Main.maxBuffTypes) ? Main.buffTip[id] : "null";
		}

		/// <summary>
		/// Gets a list of buffs by name
		/// </summary>
		/// <param name="name">name</param>
		/// <returns>Matching list of buff ids</returns>
		public List<int> GetBuffByName(string name)
		{
			string nameLower = name.ToLower();
			string buffname;
			for (int i = 1; i < Main.maxBuffTypes; i++)
			{
				buffname = Main.buffName[i];
				if (!String.IsNullOrWhiteSpace(buffname) && buffname.ToLower() == nameLower)
					return new List<int> {i};
			}
			var found = new List<int>();
			for (int i = 1; i < Main.maxBuffTypes; i++)
			{
				buffname = Main.buffName[i];
				if (!String.IsNullOrWhiteSpace(buffname) && buffname.ToLower().StartsWith(nameLower))
					found.Add(i);
			}
			return found;
		}

		/// <summary>
		/// Gets a prefix based on its id
		/// </summary>
		/// <param name="id">ID</param>
		/// <returns>Prefix name</returns>
		public string GetPrefixById(int id)
		{
			return id < FirstItemPrefix || id > LastItemPrefix ? "" : Lang.prefix[id] ?? "";
		}

		/// <summary>
		/// Gets a list of prefixes by name
		/// </summary>
		/// <param name="name">Name</param>
		/// <returns>List of prefix IDs</returns>
		public List<int> GetPrefixByName(string name)
		{
			Item item = new Item();
			item.SetDefaults(0);
			string lowerName = name.ToLower();
			var found = new List<int>();
			for (int i = FirstItemPrefix; i <= LastItemPrefix; i++)
			{
				item.prefix = (byte)i;
				string prefixName = item.AffixName().Trim().ToLower();
				if (prefixName == lowerName)
					return new List<int>() { i };
				else if (prefixName.StartsWith(lowerName)) // Partial match
					found.Add(i);
			}
			return found;
		}
				
				/// <summary>
		/// Gets a prefix by ID or name
		/// </summary>
		/// <param name="idOrName">ID or name</param>
		/// <returns>List of prefix IDs</returns>
		public List<int> GetPrefixByIdOrName(string idOrName)
		{
			int type = -1;
			if (int.TryParse(idOrName, out type) && type >= FirstItemPrefix && type <= LastItemPrefix)
			{
				return new List<int> {type};
			}
			return GetPrefixByName(idOrName);
		}

		/// <summary>
		/// Kicks all player from the server without checking for immunetokick permission.
		/// </summary>
		/// <param name="reason">string reason</param>
		public void ForceKickAll(string reason)
		{
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.Active)
				{
					ForceKick(player, reason, false, true);
				}
			}
		}

		/// <summary>
		/// Stops the server after kicking all players with a reason message, and optionally saving the world
		/// </summary>
		/// <param name="save">bool perform a world save before stop (default: true)</param>
		/// <param name="reason">string reason (default: "Server shutting down!")</param>
		public void StopServer(bool save = true, string reason = "Server shutting down!")
		{
			ForceKickAll(reason);
			if (save)
				SaveManager.Instance.SaveWorld();

			// Save takes a while so kick again
			ForceKickAll(reason);

			// Broadcast so console can see we are shutting down as well
			TShock.Utils.Broadcast(reason, Color.Red);

			// Disconnect after kick as that signifies server is exiting and could cause a race
			Netplay.disconnect = true;
		}

		/// <summary>
		/// Stops the server after kicking all players with a reason message, and optionally saving the world then attempts to 
		/// restart it.
		/// </summary>
		/// <param name="save">bool perform a world save before stop (default: true)</param>
		/// <param name="reason">string reason (default: "Server shutting down!")</param>
		public void RestartServer(bool save = true, string reason = "Server shutting down!")
		{
			if (Main.ServerSideCharacter)
				foreach (TSPlayer player in TShock.Players)
					if (player != null && player.IsLoggedIn && !player.IgnoreActionsForClearingTrashCan)
						TShock.CharacterDB.InsertPlayerData(player);

			StopServer(true, reason);
			System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
			Environment.Exit(0);
		}

		/// <summary>
		/// Reloads all configuration settings, groups, regions and raises the reload event.
		/// </summary>
		public void Reload(TSPlayer player)
		{
			FileTools.SetupConfig();
			TShock.HandleCommandLinePostConfigLoad(Environment.GetCommandLineArgs());
			TShock.Groups.LoadPermisions();
			TShock.Regions.Reload();
			TShock.Itembans.UpdateItemBans();
			Hooks.GeneralHooks.OnReloadEvent(player);
		}

		/// <summary>
		/// Kicks a player from the server without checking for immunetokick permission.
		/// </summary>
		/// <param name="player">TSPlayer player</param>
		/// <param name="reason">string reason</param>
		/// <param name="silent">bool silent (default: false)</param>
		/// <param name="saveSSI">bool saveSSI (default: false)</param>
		public void ForceKick(TSPlayer player, string reason, bool silent = false, bool saveSSI = false)
		{
			Kick(player, reason, true, silent, null, saveSSI);
		}

		/// <summary>
		/// Kicks a player from the server..
		/// </summary>
		/// <param name="player">TSPlayer player</param>
		/// <param name="reason">string reason</param>
		/// <param name="force">bool force (default: false)</param>
		/// <param name="silent">bool silent (default: false)</param>
		/// <param name="adminUserName">string adminUserName (default: null)</param>
		/// <param name="saveSSI">bool saveSSI (default: false)</param>
		public bool Kick(TSPlayer player, string reason, bool force = false, bool silent = false, string adminUserName = null, bool saveSSI = false)
		{
			if (!player.ConnectionAlive)
				return true;
			if (force || !player.Group.HasPermission(Permissions.immunetokick))
			{
				string playerName = player.Name;
				player.SilentKickInProgress = silent;
				if (player.IsLoggedIn && saveSSI)
					player.SaveServerCharacter();
				player.Disconnect(string.Format("Kicked: {0}", reason));
				TShock.Log.ConsoleInfo(string.Format("Kicked {0} for : '{1}'", playerName, reason));
				string verb = force ? "force " : "";
				if (!silent)
				{
					if (string.IsNullOrWhiteSpace(adminUserName))
						Broadcast(string.Format("{0} was {1}kicked for '{2}'", playerName, verb, reason.ToLower()), Color.Green);
					else
						Broadcast(string.Format("{0} {1}kicked {2} for '{3}'", adminUserName, verb, playerName, reason.ToLower()), Color.Green);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Bans and kicks a player from the server.
		/// </summary>
		/// <param name="player">TSPlayer player</param>
		/// <param name="reason">string reason</param>
		/// <param name="force">bool force (default: false)</param>
		/// <param name="adminUserName">string adminUserName (default: null)</param>
		public bool Ban(TSPlayer player, string reason, bool force = false, string adminUserName = null)
		{
			if (!player.ConnectionAlive)
				return true;
			if (force || !player.Group.HasPermission(Permissions.immunetoban))
			{
				string ip = player.IP;
				string uuid = player.UUID;
				string playerName = player.Name;
				TShock.Bans.AddBan(ip, playerName, uuid, reason, false, adminUserName);
				player.Disconnect(string.Format("Banned: {0}", reason));
				string verb = force ? "force " : "";
				if (string.IsNullOrWhiteSpace(adminUserName))
					TSPlayer.All.SendInfoMessage("{0} was {1}banned for '{2}'.", playerName, verb, reason);
				else
					TSPlayer.All.SendInfoMessage("{0} {1}banned {2} for '{3}'.", adminUserName, verb, playerName, reason);
				return true;
			}
			return false;
		}

		/// <summary>HasBanExpired - Returns whether or not a ban has expired or not.</summary>
		/// <param name="ban">ban - The ban object to check.</param>
		/// <param name="byName">byName - Defines whether or not the ban should be checked by name.</param>
		/// <returns>bool - True if the ban has expired.</returns>
		public bool HasBanExpired(Ban ban, bool byName = false)
		{
					DateTime exp;
					bool expirationExists = DateTime.TryParse(ban.Expiration, out exp);

					if (!string.IsNullOrWhiteSpace(ban.Expiration) && (expirationExists) &&
							(DateTime.UtcNow >= exp))
					{
							if (byName)
							{
									TShock.Bans.RemoveBan(ban.Name, true, true, false);
							}
							else
							{
									TShock.Bans.RemoveBan(ban.IP, false, false, false);
							}
							
							return true;
					}

				return false;
		}

		/// <summary>
		/// Shows a file to the user.
		/// </summary>
		/// <param name="player">TSPlayer player</param>
		/// <param name="file">string filename reletave to savedir</param>
		public void ShowFileToUser(TSPlayer player, string file)
		{
			string foo = "";
			using (var tr = new StreamReader(Path.Combine(TShock.SavePath, file)))
			{
				while ((foo = tr.ReadLine()) != null)
				{
					if (string.IsNullOrWhiteSpace(foo))
					{
						continue;
					}

					foo = foo.Replace("%map%", (TShock.Config.UseServerName ? TShock.Config.ServerName : Main.worldName));
					foo = foo.Replace("%players%", String.Join(",", GetPlayers(false)));
					Regex reg = new Regex("%\\s*(?<r>\\d{1,3})\\s*,\\s*(?<g>\\d{1,3})\\s*,\\s*(?<b>\\d{1,3})\\s*%");
					var matches = reg.Matches(foo);
					Color c = Color.White;
					foreach (Match match in matches)
					{
						byte r, g, b;
						if (byte.TryParse(match.Groups["r"].Value, out r) &&
							byte.TryParse(match.Groups["g"].Value, out g) &&
							byte.TryParse(match.Groups["b"].Value, out b))
						{
							c = new Color(r, g, b);
						}
						foo = foo.Remove(match.Index, match.Length);
					}
					player.SendMessage(foo, c);
				}
			}
		}

		/// <summary>
		/// Returns a Group from the name of the group
		/// </summary>
		/// <param name="groupName">string groupName</param>
		public Group GetGroup(string groupName)
		{
			//first attempt on cached groups
			for (int i = 0; i < TShock.Groups.groups.Count; i++)
			{
				if (TShock.Groups.groups[i].Name.Equals(groupName))
				{
					return TShock.Groups.groups[i];
				}
			}
				return Group.DefaultGroup;
		}

		/// <summary>
		/// Returns an IPv4 address from a DNS query
		/// </summary>
		/// <param name="hostname">string ip</param>
		public string GetIPv4Address(string hostname)
		{
			try
			{
				//Get the ipv4 address from GetHostAddresses, if an ip is passed it will return that ip
				var ip = Dns.GetHostAddresses(hostname).FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork);
				//if the dns query was successful then return it, otherwise return an empty string
				return ip != null ? ip.ToString() : "";
			}
			catch (SocketException)
			{
			}
			return "";
		}

		/// <summary>
		/// Sends the player an error message stating that more than one match was found
		/// appending a csv list of the matches.
		/// </summary>
		/// <param name="ply">Player to send the message to</param>
		/// <param name="matches">An enumerable list with the matches</param>
		public void SendMultipleMatchError(TSPlayer ply, IEnumerable<object> matches)
		{
			ply.SendErrorMessage("More than one match found: {0}", string.Join(",", matches));
			ply.SendErrorMessage("Use \"my query\" for items with spaces");
		}

		/// <summary>
		/// Default hashing algorithm.
		/// </summary>
		[Obsolete("This is no longer necessary, please use TShock.Config.HashAlgorithm instead if you really need it (but use User.VerifyPassword(password)) for verifying passwords.")]
		public string HashAlgo = "sha512";

		/// <summary>
		/// A dictionary of hashing algortihms and an implementation object.
		/// </summary>
		[Obsolete("This is no longer necessary, after switching to User.VerifyPassword(password) instead.")]
		public readonly Dictionary<string, Func<HashAlgorithm>> HashTypes = new Dictionary<string, Func<HashAlgorithm>>
			{
					{"sha512", () => new SHA512Managed()},
					{"sha256", () => new SHA256Managed()},
					{"md5", () => new MD5Cng()},
					{"sha512-xp", () => SHA512.Create()},
					{"sha256-xp", () => SHA256.Create()},
					{"md5-xp", () => MD5.Create()},
			};

		/// <summary>
		/// Returns a Sha256 string for a given string
		/// </summary>
		/// <param name="bytes">bytes to hash</param>
		/// <returns>string sha256</returns>
		[Obsolete("Please use User.VerifyPassword(password) instead. Warning: This will upgrade passwords to BCrypt. Already converted passwords will not hash correctly using this method.")]
		public string HashPassword(byte[] bytes)
		{
			if (bytes == null)
				throw new NullReferenceException("bytes");
			Func<HashAlgorithm> func;
			if (!HashTypes.TryGetValue(HashAlgo.ToLower(), out func))
				throw new NotSupportedException("Hashing algorithm {0} is not supported".SFormat(HashAlgo.ToLower()));

			using (var hash = func())
			{
				var ret = hash.ComputeHash(bytes);
				return ret.Aggregate("", (s, b) => s + b.ToString("X2"));
			}
		}

		/// <summary>
		/// Returns a Sha256 string for a given string
		/// </summary>
		/// <param name="password">string to hash</param>
		/// <returns>string sha256</returns>
		[Obsolete("Please use User.VerifyPassword(password) instead. Warning: This will upgrade passwords to BCrypt. Already converted passwords will not hash correctly using this method.")]
		public string HashPassword(string password)
		{
			if (string.IsNullOrEmpty(password) || password == "non-existant password")
				return "non-existant password";
			return HashPassword(Encoding.UTF8.GetBytes(password));
		}

		/// <summary>
		/// Checks if the string contains any unprintable characters
		/// </summary>
		/// <param name="str">String to check</param>
		/// <returns>True if the string only contains printable characters</returns>
		[Obsolete("ValidString is being removed as it serves no purpose to TShock at this time.")]
		public bool ValidString(string str)
		{
			foreach (var c in str)
			{
				if (c < 0x20 || c > 0xA9)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Checks if world has hit the max number of chests
		/// </summary>
		/// <returns>True if the entire chest array is used</returns>
		public bool MaxChests()
		{
			for (int i = 0; i < Main.chest.Length; i++)
			{
				if (Main.chest[i] == null)
					return false;
			}
			return true;
		}

		/// <summary>
		/// Attempts to parse a string as a timespan (_d_m_h_s).
		/// </summary>
		/// <param name="str">The time string.</param>
		/// <param name="seconds">The seconds.</param>
		/// <returns>Whether the string was parsed successfully.</returns>
		public bool TryParseTime(string str, out int seconds)
		{
			seconds = 0;

			var sb = new StringBuilder(3);
			for (int i = 0; i < str.Length; i++)
			{
				if (Char.IsDigit(str[i]) || (str[i] == '-' || str[i] == '+'))
					sb.Append(str[i]);
				else
				{
					int num;
					if (!int.TryParse(sb.ToString(), out num))
						return false;
					
					sb.Clear();
					switch (str[i])
					{
						case 's':
							seconds += num;
							break;
						case 'm':
							seconds += num * 60;
							break;
						case 'h':
							seconds += num * 60 * 60;
							break;
						case 'd':
							seconds += num * 60 * 60 * 24;
							break;
						default:
							return false;
					}
				}
			}
			if (sb.Length != 0)
				return false;
			return true;
		}

		/// <summary>
		/// Searches for a projectile by identity and owner
		/// </summary>
		/// <param name="identity">identity</param>
		/// <param name="owner">owner</param>
		/// <returns>projectile ID</returns>
		public int SearchProjectile(short identity, int owner)
		{
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				if (Main.projectile[i].identity == identity && Main.projectile[i].owner == owner)
					return i;
			}
			return 1000;
		}

		/// <summary>
		/// Sanitizes input strings
		/// </summary>
		/// <param name="str">string</param>
		/// <returns>sanitized string</returns>
		[Obsolete("SanitizeString is being removed from TShock as it currently serves no purpose.")]
		public string SanitizeString(string str)
		{
			var returnstr = str.ToCharArray();
			for (int i = 0; i < str.Length; i++)
			{
				if (!ValidString(str[i].ToString()))
					returnstr[i] = ' ';
			}
			return new string(returnstr);
		}

		/// <summary>
		/// Enumerates boundary points of the given region's rectangle.
		/// </summary>
		/// <param name="regionArea">The region's area to enumerate through.</param>
		/// <returns>The enumerated boundary points.</returns>
		public IEnumerable<Point> EnumerateRegionBoundaries(Rectangle regionArea)
		{
			for (int x = 0; x < regionArea.Width + 1; x++)
			{
				yield return new Point(regionArea.Left + x, regionArea.Top);
				yield return new Point(regionArea.Left + x, regionArea.Bottom);
			}

			for (int y = 1; y < regionArea.Height; y++)
			{
				yield return new Point(regionArea.Left, regionArea.Top + y);
				yield return new Point(regionArea.Right, regionArea.Top + y);
			}
		}

		/// <summary>EncodeColor - Encodes a color as an int.</summary>
		/// <param name="color">color - The color to encode</param>
		/// <returns>int? - The encoded color</returns>
		public int? EncodeColor(Color? color)
		{
			if (color == null)
				return null;

			return BitConverter.ToInt32(new[] { color.Value.R, color.Value.G, color.Value.B, color.Value.A }, 0);
		}

		/// <summary>DecodeColor - Decodes a color encoded by the EncodeColor function.</summary>
		/// <param name="encodedColor">encodedColor - The encoded color</param>
		/// <returns>Color? - The decoded color</returns>
		public Color? DecodeColor(int? encodedColor)
		{
			if (encodedColor == null)
				return null;

			byte[] data = BitConverter.GetBytes(encodedColor.Value);
			return new Color(data[0], data[1], data[2], data[3]);
		}

		/// <summary>
		/// Encodes a Boolean Array as an int.
		/// </summary>
		/// <param name="bools">The boolean array to encode.</param>
		/// <returns>The encoded int.</returns>
		public int? EncodeBoolArray(bool[] bools)
		{
			if (bools == null)
				return null;

			int result = 0;
			for (int i = 0; i < bools.Length; i++)
				if (bools[i])
					result |= (1 << i);

			return result;
		}

		/// <summary>
		/// Decodes a Boolean Array from an int.
		/// </summary>
		/// <param name="encodedbools">The encoded Boolean Array.</param>
		/// <returns>The resulting Boolean Array.</returns>
		public bool[] DecodeBoolArray(int? encodedbools)
		{
			if (encodedbools == null)
				return null;

			bool[] result = new bool[10];
			for (int i = 0; i < result.Length; i++)
				result[i] = (encodedbools & 1 << i) != 0;

			return result;
		}

		/// <summary>EncodeBitsByte - Encodes a BitsByte as a byte.</summary>
		/// <param name="bitsByte">bitsByte - The BitsByte object</param>
		/// <returns>byte? - The converted byte</returns>
		public byte? EncodeBitsByte(BitsByte? bitsByte)
		{
			if (bitsByte == null)
				return null;

			byte result = 0;
			for (int i = 0; i < 8; i++)
				if (bitsByte.Value[i])
					result |= (byte)(1 << i);

			return result;
		}

		/// <summary>DecodeBitsByte - Decodes a bitsbyte from an int.</summary>
		/// <param name="encodedBitsByte">encodedBitsByte - The encoded bitsbyte object.</param>
		/// <returns>BitsByte? - The decoded bitsbyte object</returns>
		public BitsByte? DecodeBitsByte(int? encodedBitsByte)
		{
			if (encodedBitsByte == null)
				return null;

			BitsByte result = new BitsByte();
			for (int i = 0; i < 8; i++)
				result[i] = (encodedBitsByte & 1 << i) != 0;

			return result;
		}

		/// <summary>GetResponseNoException - Gets a web response without generating an exception.</summary>
		/// <param name="req">req - The request to send.</param>
		/// <returns>HttpWebResponse - The response object.</returns>
		public HttpWebResponse GetResponseNoException(HttpWebRequest req)
		{
			try
			{
				return (HttpWebResponse)req.GetResponse();
			}
			catch (WebException we)
			{
				var resp = we.Response as HttpWebResponse;
				if (resp == null)
					throw;
				return resp;
			}
		}

		/// <summary>
		/// Colors the given text by correctly applying the color chat tag.
		/// </summary>
		/// <param name="text">The text to color.</param>
		/// <param name="color">The color to apply.</param>
		/// <returns>The <paramref name="text"/>, surrounded by the color tag with the appropriated hex code.</returns>
		public string ColorTag(string text, Color color)
		{
			return String.Format("[c/{0}:{1}]", color.Hex3(), text);
		}

		/// <summary>
		/// Converts an item into its text representation using the item chat tag.
		/// </summary>
		/// <param name="item">The item to convert.</param>
		/// <returns>The <paramref name="item"/> NetID surrounded by the item tag with proper stack/prefix data.</returns>
		public string ItemTag(Item item)
		{
			int netID = item.netID;
			int stack = item.stack;
			int prefix = item.prefix;
			string options = stack > 1 ? "/s" + stack : prefix != 0 ? "/p" + prefix : "";
			return String.Format("[i{0}:{1}]", options, netID);
		}
	}
}
