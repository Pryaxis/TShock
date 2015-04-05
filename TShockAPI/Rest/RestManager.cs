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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HttpServer;
using Rests;
using Terraria;
using TShockAPI.DB;

namespace TShockAPI
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class Permission : Attribute
	{
		public string Name { get; set; }

		public Permission(string name)
		{
			Name = name;
		}
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class RouteAttribute : Attribute
	{
		public string Route { get; set; }

		public RouteAttribute(string route)
		{
			Route = route;
		}
	}

	public class ParameterAttribute : Attribute
	{
		public string Name { get; set; }
		public bool Required { get; set; }
		public string Description { get; set; }
		public Type ArgumentType { get; set; }

		public ParameterAttribute(string name, bool req, string desc, Type type)
		{
			Name = name;
			Required = req;
			Description = desc;
			ArgumentType = type;
		}
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class Noun : ParameterAttribute
	{
		public Noun(string name, bool req, string desc, Type type) : base(name, req, desc, type) { }
	}

	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class Verb : ParameterAttribute
	{
		public Verb(string name, bool req, string desc, Type type) : base(name, req, desc, type) { }
	}

	[AttributeUsage(AttributeTargets.Method)]
	public class Token : Noun
	{
		public Token() : base("token", true, "The REST authentication token.", typeof(String)){}
	}

	public class RestManager
	{
		private Rest Rest;

		public RestManager(Rest rest)
		{
			Rest = rest;
		}

		public void RegisterRestfulCommands()
		{
			// Server Commands
			if (TShock.Config.EnableTokenEndpointAuthentication)
			{
				Rest.Register(new SecureRestCommand("/v2/server/status", ServerStatusV2));
				Rest.Register(new SecureRestCommand("/status", ServerStatus));
				Rest.Register(new SecureRestCommand("/v3/server/motd", ServerMotd));
				Rest.Register(new SecureRestCommand("/v3/server/rules", ServerRules));
			}
			else
			{
				Rest.Register(new RestCommand("/v2/server/status", (a) => this.ServerStatusV2(new RestRequestArgs(a.Verbs, a.Parameters, a.Request, SecureRest.TokenData.None))));
				Rest.Register(new RestCommand("/status", (a) => this.ServerStatus(new RestRequestArgs(a.Verbs, a.Parameters, a.Request, SecureRest.TokenData.None))));
				Rest.Register(new RestCommand("/v3/server/motd", (a) => this.ServerMotd(new RestRequestArgs(a.Verbs, a.Parameters, a.Request, SecureRest.TokenData.None))));
				Rest.Register(new RestCommand("/v3/server/rules", (a) => this.ServerRules(new RestRequestArgs(a.Verbs, a.Parameters, a.Request, SecureRest.TokenData.None))));
			}

			Rest.Register(new SecureRestCommand("/v2/server/broadcast", ServerBroadcast));
			Rest.Register(new SecureRestCommand("/v3/server/reload", ServerReload, RestPermissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/server/off", ServerOff, RestPermissions.restmaintenance));
			Rest.Register(new SecureRestCommand("/v3/server/restart", ServerRestart, RestPermissions.restmaintenance));
			Rest.Register(new SecureRestCommand("/v2/server/rawcmd", ServerCommand, RestPermissions.restrawcommand));
			Rest.Register(new SecureRestCommand("/v3/server/rawcmd", ServerCommandV3, RestPermissions.restrawcommand));
			Rest.Register(new SecureRestCommand("/tokentest", ServerTokenTest));

			// User Commands
			Rest.Register(new SecureRestCommand("/v2/users/activelist", UserActiveListV2, RestPermissions.restviewusers));
			Rest.Register(new SecureRestCommand("/v2/users/create", UserCreateV2, RestPermissions.restmanageusers) { DoLog = false });
			Rest.Register(new SecureRestCommand("/v2/users/list", UserListV2, RestPermissions.restviewusers));
			Rest.Register(new SecureRestCommand("/v2/users/read", UserInfoV2, RestPermissions.restviewusers));
			Rest.Register(new SecureRestCommand("/v2/users/destroy", UserDestroyV2, RestPermissions.restmanageusers));
			Rest.Register(new SecureRestCommand("/v2/users/update", UserUpdateV2, RestPermissions.restmanageusers) { DoLog = false });

			// Ban Commands
			Rest.Register(new SecureRestCommand("/bans/create", BanCreate, RestPermissions.restmanagebans));
			Rest.Register(new SecureRestCommand("/v2/bans/list", BanListV2, RestPermissions.restviewbans));
			Rest.Register(new SecureRestCommand("/v2/bans/read", BanInfoV2, RestPermissions.restviewbans));
			Rest.Register(new SecureRestCommand("/v2/bans/destroy", BanDestroyV2, RestPermissions.restmanagebans));

			// World Commands
			Rest.Register(new SecureRestCommand("/world/read", WorldRead));
			Rest.Register(new SecureRestCommand("/world/meteor", WorldMeteor, RestPermissions.restcauseevents));
			Rest.Register(new SecureRestCommand("/world/bloodmoon/{bool}", WorldBloodmoon, RestPermissions.restcauseevents));
			Rest.Register(new SecureRestCommand("/v2/world/save", WorldSave, RestPermissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/world/autosave/state/{bool}", WorldChangeSaveSettings, RestPermissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/world/butcher", WorldButcher, RestPermissions.restbutcher));

			// Player Commands
			Rest.Register(new SecureRestCommand("/lists/players", PlayerList));
			Rest.Register(new SecureRestCommand("/v2/players/list", PlayerListV2));
			Rest.Register(new SecureRestCommand("/v2/players/read", PlayerReadV2, RestPermissions.restuserinfo));
			Rest.Register(new SecureRestCommand("/v3/players/read", PlayerReadV3, RestPermissions.restuserinfo));
			Rest.Register(new SecureRestCommand("/v2/players/kick", PlayerKickV2, RestPermissions.restkick));
			Rest.Register(new SecureRestCommand("/v2/players/ban", PlayerBanV2, RestPermissions.restban, RestPermissions.restmanagebans));
			Rest.Register(new SecureRestCommand("/v2/players/kill", PlayerKill, RestPermissions.restkill));
			Rest.Register(new SecureRestCommand("/v2/players/mute", PlayerMute, RestPermissions.restmute));
			Rest.Register(new SecureRestCommand("/v2/players/unmute", PlayerUnMute, RestPermissions.restmute));

			// Group Commands
			Rest.Register(new SecureRestCommand("/v2/groups/list", GroupList, RestPermissions.restviewgroups));
			Rest.Register(new SecureRestCommand("/v2/groups/read", GroupInfo, RestPermissions.restviewgroups));
			Rest.Register(new SecureRestCommand("/v2/groups/destroy", GroupDestroy, RestPermissions.restmanagegroups));
			Rest.Register(new SecureRestCommand("/v2/groups/create", GroupCreate, RestPermissions.restmanagegroups));
			Rest.Register(new SecureRestCommand("/v2/groups/update", GroupUpdate, RestPermissions.restmanagegroups));
		}

		#region RestServerMethods

		[Description("Deprecated: Executes a remote command on the server, and returns the output of the command.")]
		[RouteAttribute("/v2/server/rawcmd")]
		[Permission(RestPermissions.restrawcommand)]
		[Noun("cmd", true, "The command and arguments to execute.", typeof(String))]
		[Token]
		private object ServerCommand(RestRequestArgs args)
		{
			if (string.IsNullOrWhiteSpace(args.Parameters["cmd"]))
				return RestMissingParam("cmd");

			Group restPlayerGroup;
			// TODO: Get rid of this when the old REST permission model is removed.
			if (TShock.Config.RestUseNewPermissionModel)
				restPlayerGroup = TShock.Groups.GetGroupByName(args.TokenData.UserGroupName);
			else
				restPlayerGroup = new SuperAdminGroup();

			TSRestPlayer tr = new TSRestPlayer(args.TokenData.Username, restPlayerGroup);
			Commands.HandleCommand(tr, args.Parameters["cmd"]);
			return RestResponse(string.Join("\n", tr.GetCommandOutput()));
		}

		[Description("Executes a remote command on the server, and returns the output of the command.")]
		[RouteAttribute("/v3/server/rawcmd")]
		[Permission(RestPermissions.restrawcommand)]
		[Noun("cmd", true, "The command and arguments to execute.", typeof(String))]
		[Token]
		private object ServerCommandV3(RestRequestArgs args)
		{
			if (string.IsNullOrWhiteSpace(args.Parameters["cmd"]))
				return RestMissingParam("cmd");

			Group restPlayerGroup;
			// TODO: Get rid of this when the old REST permission model is removed.
			if (TShock.Config.RestUseNewPermissionModel)
				restPlayerGroup = TShock.Groups.GetGroupByName(args.TokenData.UserGroupName);
			else
				restPlayerGroup = new SuperAdminGroup();

			TSRestPlayer tr = new TSRestPlayer(args.TokenData.Username, restPlayerGroup);
			Commands.HandleCommand(tr, args.Parameters["cmd"]);
			return new RestObject()
			{
				{"response", tr.GetCommandOutput()}
			};
		}

		[Description("Turn the server off.")]
		[Route("/v2/server/off")]
		[Permission(RestPermissions.restmaintenance)]
		[Noun("confirm", true, "Required to confirm that actually want to turn the server off.", typeof(bool))]
		[Noun("message", false, "The shutdown message.", typeof(String))]
		[Noun("nosave", false, "Shutdown without saving.", typeof(bool))]
		[Token]
		private object ServerOff(RestRequestArgs args)
		{
			if (!GetBool(args.Parameters["confirm"], false))
				return RestInvalidParam("confirm");

			// Inform players the server is shutting down
			var reason = string.IsNullOrWhiteSpace(args.Parameters["message"]) ? "Server is shutting down" : args.Parameters["message"];
			TShock.Utils.StopServer(!GetBool(args.Parameters["nosave"], false), reason);

			return RestResponse("The server is shutting down");
		}

		[Description("Attempt to restart the server.")]
		[Route("/v3/server/restart")]
		[Permission(RestPermissions.restmaintenance)]
		[Noun("confirm", true, "Confirm that you actually want to restart the server", typeof(bool))]
		[Noun("message", false, "The shutdown message.", typeof(String))]
		[Noun("nosave", false, "Shutdown without saving.", typeof(bool))]
		[Token]
		private object ServerRestart(RestRequestArgs args)
		{
			if (!GetBool(args.Parameters["confirm"], false))
				return RestInvalidParam("confirm");

			// Inform players the server is shutting down
			var reason = string.IsNullOrWhiteSpace(args.Parameters["message"]) ? "Server is restarting" : args.Parameters["message"];
			TShock.Utils.RestartServer(!GetBool(args.Parameters["nosave"], false), reason);

			return RestResponse("The server is shutting down and will attempt to restart");
		}

		[Description("Reload config files for the server.")]
		[Route("/v3/server/reload")]
		[Permission(RestPermissions.restcfg)]
		[Token]
		private object ServerReload(RestRequestArgs args)
		{
			TShock.Utils.Reload(new TSRestPlayer(args.TokenData.Username, TShock.Groups.GetGroupByName(args.TokenData.UserGroupName)));
			
			return RestResponse("Configuration, permissions, and regions reload complete. Some changes may require a server restart.");
		}

		[Description("Broadcast a server wide message.")]
		[Route("/v2/server/broadcast")]
		[Noun("msg", true, "The message to broadcast.", typeof(String))]
		[Token]
		private object ServerBroadcast(RestRequestArgs args)
		{
			var msg = args.Parameters["msg"];
			if (string.IsNullOrWhiteSpace(msg))
				return RestMissingParam("msg");
			TSPlayer.All.SendInfoMessage(msg);
			return RestResponse("The message was broadcasted successfully");
		}

		private object ServerMotd(RestRequestArgs args)
		{
			string motdFilePath = Path.Combine(TShock.SavePath, "motd.txt");
			if (!File.Exists(motdFilePath))
				return this.RestError("The motd.txt was not found.", "500");

			return new RestObject()
			{
				{"motd", File.ReadAllLines(motdFilePath)}
			};
		}

		private object ServerRules(RestRequestArgs args)
		{
			string rulesFilePath = Path.Combine(TShock.SavePath, "rules.txt");
			if (!File.Exists(rulesFilePath))
				return this.RestError("The rules.txt was not found.", "500");

			return new RestObject()
			{
				{"rules", File.ReadAllLines(rulesFilePath)}
			};
		}

		private object ServerStatus(RestRequestArgs args)
		{
			var activeplayers = Main.player.Where(p => null != p && p.active).ToList();
			return new RestObject()
			{
				{"name", TShock.Config.ServerName},
				{"port", Convert.ToString(Netplay.serverPort)},
				{"playercount", Convert.ToString(activeplayers.Count())},
				{"players", string.Join(", ", activeplayers.Select(p => p.name))},
			};
		}

		private object ServerStatusV2(RestRequestArgs args)
		{
			var ret = new RestObject()
			{
				{"name", TShock.Config.ServerName},
				{"serverversion", Main.versionNumber},
				{"tshockversion", TShock.VersionNum},
				{"port", TShock.Config.ServerPort},
				{"playercount", Main.player.Where(p => null != p && p.active).Count()},
				{"maxplayers", TShock.Config.MaxSlots},
				{"world", Main.worldName},
				{"uptime", (DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime).ToString(@"d'.'hh':'mm':'ss")},
				{"serverpassword", !string.IsNullOrEmpty(TShock.Config.ServerPassword)}
			};

			if (GetBool(args.Parameters["players"], false))
			{
				var players = new ArrayList();
				foreach (TSPlayer tsPlayer in TShock.Players.Where(p => null != p))
				{
					var p = PlayerFilter(tsPlayer, args.Parameters, ((args.TokenData.UserGroupName) != "" && TShock.Utils.GetGroup(args.TokenData.UserGroupName).HasPermission(RestPermissions.viewips)));
					if (null != p)
						players.Add(p);
				}
				ret.Add("players", players);
			}

			if (GetBool(args.Parameters["rules"], false))
			{
				var rules = new Dictionary<string,object>();
				rules.Add("AutoSave", TShock.Config.AutoSave);
				rules.Add("DisableBuild", TShock.Config.DisableBuild);
				rules.Add("DisableClownBombs", TShock.Config.DisableClownBombs);
				rules.Add("DisableDungeonGuardian", TShock.Config.DisableDungeonGuardian);
				rules.Add("DisableInvisPvP", TShock.Config.DisableInvisPvP);
				rules.Add("DisableSnowBalls", TShock.Config.DisableSnowBalls);
				rules.Add("DisableTombstones", TShock.Config.DisableTombstones);
				rules.Add("EnableWhitelist", TShock.Config.EnableWhitelist);
				rules.Add("HardcoreOnly", TShock.Config.HardcoreOnly);
				rules.Add("PvPMode", TShock.Config.PvPMode);
				rules.Add("SpawnProtection", TShock.Config.SpawnProtection);
				rules.Add("SpawnProtectionRadius", TShock.Config.SpawnProtectionRadius);
				rules.Add("ServerSideInventory", Main.ServerSideCharacter);

				ret.Add("rules", rules);
			}
			return ret;
		}

		[Description("Test if a token is still valid.")]
		[Route("/tokentest")]
		[Token]
		private object ServerTokenTest(RestRequestArgs args)
		{
			return new RestObject()
			{
				{"response", "Token is valid and was passed through correctly."},
				{"associateduser", args.TokenData.Username}
			};
		}

		#endregion

		#region RestUserMethods

		private object UserActiveListV2(RestRequestArgs args)
		{
			return new RestObject() { { "activeusers", string.Join("\t", TShock.Players.Where(p => null != p && null != p.UserAccountName && p.Active).Select(p => p.UserAccountName)) } };
		}

		private object UserListV2(RestRequestArgs args)
		{
			return new RestObject() { { "users", TShock.Users.GetUsers().Select(p => new Dictionary<string,object>(){
				{"name", p.Name},
				{"id", p.ID},
				{"group", p.Group},
			}) } };
		}

		private object UserCreateV2(RestRequestArgs args)
		{
			var username = args.Parameters["user"];
			if (string.IsNullOrWhiteSpace(username))
				return RestMissingParam("user");

			var group = args.Parameters["group"];
		    if (string.IsNullOrWhiteSpace(group))
		        group = TShock.Config.DefaultRegistrationGroupName;

			var password = args.Parameters["password"];
			if (string.IsNullOrWhiteSpace(password))
				return RestMissingParam("password");

			// NOTE: ip can be blank
			User user = new User(username, password, "", group, "", "", "");
			try
			{
				TShock.Users.AddUser(user);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("User was successfully created");
		}

		private object UserUpdateV2(RestRequestArgs args)
		{
			var ret = UserFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			var password = args.Parameters["password"];
			var group = args.Parameters["group"];
			if (string.IsNullOrWhiteSpace(group) && string.IsNullOrWhiteSpace(password))
				return RestMissingParam("group", "password");

			User user = (User)ret;
			var response = new RestObject();
			if (!string.IsNullOrWhiteSpace(password))
			{
				try
				{
					TShock.Users.SetUserPassword(user, password);
					response.Add("password-response", "Password updated successfully");
				}
				catch (Exception e)
				{
					return RestError("Failed to update user password (" + e.Message + ")");
				}
			}

			if (!string.IsNullOrWhiteSpace(group))
			{
				try
				{
					TShock.Users.SetUserGroup(user, group);
					response.Add("group-response", "Group updated successfully");
				}
				catch (Exception e)
				{
					return RestError("Failed to update user group (" + e.Message + ")");
				}
			}

			return response;
		}

		private object UserDestroyV2(RestRequestArgs args)
		{
			var ret = UserFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			try
			{
				TShock.Users.RemoveUser((User)ret);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("User deleted successfully");
		}

		private object UserInfoV2(RestRequestArgs args)
		{
			var ret = UserFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			User user = (User)ret;
			return new RestObject() { { "group", user.Group }, { "id", user.ID.ToString() }, { "name", user.Name } };
		}

		#endregion

		#region RestBanMethods

		private object BanCreate(RestRequestArgs args)
		{
			var ip = args.Parameters["ip"];
			var name = args.Parameters["name"];

			if (string.IsNullOrWhiteSpace(ip) && string.IsNullOrWhiteSpace(name))
				return RestMissingParam("ip", "name");

			try
			{
				TShock.Bans.AddBan(ip, name, "", args.Parameters["reason"], true, args.TokenData.Username);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}
			return RestResponse("Ban created successfully");
		}

		private object BanDestroyV2(RestRequestArgs args)
		{
			var ret = BanFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			try
			{
				Ban ban = (Ban)ret;
				switch (args.Parameters["type"])
				{
					case "ip":
						if (!TShock.Bans.RemoveBan(ban.IP, false, false, true))
							return RestResponse("Failed to delete ban (already deleted?)");
						break;
					case "name":
						if (!TShock.Bans.RemoveBan(ban.Name, true, GetBool(args.Parameters["caseinsensitive"], true)))
							return RestResponse("Failed to delete ban (already deleted?)");
						break;
					default:
						return RestError("Invalid Type: '" + args.Parameters["type"] + "'");
				}

			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("Ban deleted successfully");
		}

		private object BanInfoV2(RestRequestArgs args)
		{
			var ret = BanFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			Ban ban = (Ban)ret;
			return new RestObject() {
				{"name", null == ban.Name ? "" : ban.Name},
				{"ip", null == ban.IP ? "" : ban.IP},
				{"reason", null == ban.Reason ? "" : ban.Reason},
			};
		}

		private object BanListV2(RestRequestArgs args)
		{
			var banList = new ArrayList();
			foreach (var ban in TShock.Bans.GetBans())
			{
				banList.Add(
					new Dictionary<string, string>
					{
						{"name", null == ban.Name ? "" : ban.Name},
						{"ip", null == ban.IP ? "" : ban.IP},
						{"reason", null == ban.Reason ? "" : ban.Reason},
					}
				);
			}

			return new RestObject() { { "bans", banList } };
		}

		#endregion

		#region RestWorldMethods

		private object WorldChangeSaveSettings(RestRequestArgs args)
		{
			bool autoSave;
			if (!bool.TryParse(args.Verbs["bool"], out autoSave))
				return RestInvalidParam("state");
			TShock.Config.AutoSave = autoSave;

			return RestResponse("AutoSave has been set to " + autoSave);
		}

		private object WorldSave(RestRequestArgs args)
		{
			SaveManager.Instance.SaveWorld();

			return RestResponse("World saved");
		}

		private object WorldButcher(RestRequestArgs args)
		{
			bool killFriendly;
			if (!bool.TryParse(args.Parameters["killfriendly"], out killFriendly))
				return RestInvalidParam("killfriendly");

			int killcount = 0;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type != 0 && !Main.npc[i].townNPC && (!Main.npc[i].friendly || killFriendly))
				{
					TSPlayer.Server.StrikeNPC(i, 99999, 90f, 1);
					killcount++;
				}
			}

			return RestResponse(killcount + " NPCs have been killed");
		}

		private object WorldRead(RestRequestArgs args)
		{
			return new RestObject()
			{
				{"name", Main.worldName},
				{"size", Main.maxTilesX + "*" + Main.maxTilesY},
				{"time", Main.time},
				{"daytime", Main.dayTime},
				{"bloodmoon", Main.bloodMoon},
				{"invasionsize", Main.invasionSize}
			};
		}

		private object WorldMeteor(RestRequestArgs args)
		{
			if (null == WorldGen.genRand)
				WorldGen.genRand = new Random();
			WorldGen.dropMeteor();
			return RestResponse("Meteor has been spawned");
		}

		private object WorldBloodmoon(RestRequestArgs args)
		{
			bool bloodmoon;
			if (!bool.TryParse(args.Verbs["bool"], out bloodmoon))
				return RestInvalidParam("bloodmoon");
			Main.bloodMoon = bloodmoon;

			return RestResponse("Blood Moon has been set to " + bloodmoon);
		}

		#endregion

		#region RestPlayerMethods

		private object PlayerUnMute(RestRequestArgs args)
		{
			return PlayerSetMute(args.Parameters, false);
		}

		private object PlayerMute(RestRequestArgs args)
		{
			return PlayerSetMute(args.Parameters, true);
		}

		private object PlayerList(RestRequestArgs args)
		{
			var activeplayers = Main.player.Where(p => null != p && p.active).ToList();
			return new RestObject() { { "players", string.Join(", ", activeplayers.Select(p => p.name)) } };
		}

		private object PlayerListV2(RestRequestArgs args)
		{
			var playerList = new ArrayList();
			foreach (TSPlayer tsPlayer in TShock.Players.Where(p => null != p))
			{
				var p = PlayerFilter(tsPlayer, args.Parameters);
				if (null != p)
					playerList.Add(p);
			}
			return new RestObject() { { "players", playerList } };
		}

		private object PlayerReadV2(RestRequestArgs args)
		{
			var ret = PlayerFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			var activeItems = player.TPlayer.inventory.Where(p => p.active).ToList();
			return new RestObject()
			{
				{"nickname", player.Name},
				{"username", null == player.UserAccountName ? "" : player.UserAccountName},
				{"ip", player.IP},
				{"group", player.Group.Name},
				{"position", player.TileX + "," + player.TileY},
				{"inventory", string.Join(", ", activeItems.Select(p => (p.name + ":" + p.stack)))},
				{"buffs", string.Join(", ", player.TPlayer.buffType)}
			};
		}

		private object PlayerReadV3(RestRequestArgs args)
		{
			var ret = PlayerFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			var inventory = player.TPlayer.inventory.Where(p => p.active).ToList();
			var equipment = player.TPlayer.armor.Where(p => p.active).ToList();
			var dyes = player.TPlayer.dye.Where(p => p.active).ToList();
			return new RestObject()
			{
				{"nickname", player.Name},
				{"username", null == player.UserAccountName ? "" : player.UserAccountName},
				{"ip", player.IP},
				{"group", player.Group.Name},
				{"position", player.TileX + "," + player.TileY},
				{"inventory", string.Join(", ", inventory.Select(p => (p.name + ":" + p.stack)))},
				{"armor", string.Join(", ", equipment.Select(p => (p.netID + ":" + p.prefix)))},
				{"dyes", string.Join(", ", dyes.Select(p => (p.name)))},
				{"buffs", string.Join(", ", player.TPlayer.buffType)}
			};
		}

		private object PlayerKickV2(RestRequestArgs args)
		{
			var ret = PlayerFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			TShock.Utils.ForceKick(player, null == args.Parameters["reason"] ? "Kicked via web" : args.Parameters["reason"], false, true);
			return RestResponse("Player " + player.Name + " was kicked");
		}

		private object PlayerBanV2(RestRequestArgs args)
		{
			var ret = PlayerFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			var reason = null == args.Parameters["reason"] ? "Banned via web" : args.Parameters["reason"];
			TShock.Bans.AddBan(player.IP, player.Name, "", reason);
			TShock.Utils.ForceKick(player, reason, false, true);
			return RestResponse("Player " + player.Name + " was banned");
		}

		private object PlayerKill(RestRequestArgs args)
		{
			var ret = PlayerFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			player.DamagePlayer(999999);
			var from = string.IsNullOrWhiteSpace(args.Parameters["from"]) ? "Server Admin" : args.Parameters["from"];
			player.SendInfoMessage(string.Format("{0} just killed you!", from));
			return RestResponse("Player " + player.Name + " was killed");
		}

		#endregion

		#region RestGroupMethods

		private object GroupList(RestRequestArgs args)
		{
			var groups = new ArrayList();
			foreach (Group group in TShock.Groups)
			{
				groups.Add(new Dictionary<string, object> {{"name", group.Name}, {"parent", group.ParentName}, {"chatcolor", group.ChatColor}});
			}
			return new RestObject() { { "groups", groups } };
		}

		private object GroupInfo(RestRequestArgs args)
		{
			var ret = GroupFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			Group group = (Group)ret;
			return new RestObject() {
				{"name", group.Name},
				{"parent", group.ParentName},
				{"chatcolor", string.Format("{0},{1},{2}", group.R, group.G, group.B)},
				{"permissions", group.permissions},
				{"negatedpermissions", group.negatedpermissions},
				{"totalpermissions", group.TotalPermissions}
			};
		}

		private object GroupDestroy(RestRequestArgs args)
		{
			var ret = GroupFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			Group group = (Group)ret;
			try
			{
				TShock.Groups.DeleteGroup(group.Name, true);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("Group '" + group.Name + "' deleted successfully");
		}

		private object GroupCreate(RestRequestArgs args)
		{
			var name = args.Parameters["group"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("group");
			try
			{
				TShock.Groups.AddGroup(name, args.Parameters["parent"], args.Parameters["permissions"], args.Parameters["chatcolor"], true);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("Group '" + name + "' created successfully");
		}

		private object GroupUpdate(RestRequestArgs args)
		{
			var ret = GroupFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			Group group = (Group)ret;
			var parent = (null == args.Parameters["parent"]) ? group.ParentName : args.Parameters["parent"];
			var chatcolor = (null == args.Parameters["chatcolor"]) ? string.Format("{0}.{1}.{2}", group.R, group.G, group.B) : args.Parameters["chatcolor"];
			var permissions = (null == args.Parameters["permissions"]) ? group.Permissions : args.Parameters["permissions"];
			try
			{
				TShock.Groups.UpdateGroup(group.Name, parent, permissions, chatcolor, group.Suffix, group.Prefix);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("Group '" + group.Name + "' updated successfully");
		}

		#endregion

		#region Utility Methods

		public static void DumpDescriptions()
		{
			var sb = new StringBuilder();
			var rest = new RestManager(null);

			foreach (var method in rest.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).OrderBy(f => f.Name))
			{
				if (method.IsStatic)
					continue;

				var name = method.Name;

				var descattr =
					method.GetCustomAttributes(false).FirstOrDefault(o => o is DescriptionAttribute) as DescriptionAttribute;
				var routeattr =
					method.GetCustomAttributes(false).FirstOrDefault(o => o is RouteAttribute) as RouteAttribute;

				if (descattr != null && !string.IsNullOrWhiteSpace(descattr.Description) && routeattr != null && !string.IsNullOrWhiteSpace(routeattr.Route))
				{
					sb.AppendLine("{0}  ".SFormat(name));
					sb.AppendLine("Description: {0}  ".SFormat(descattr.Description));


					var verbs = method.GetCustomAttributes(false).Where(o => o is Verb);
					if (verbs.Count() > 0)
					{
						sb.AppendLine("Verbs:");
						foreach (Verb verb in verbs)
						{
							sb.AppendLine("\t{0} - {1}".SFormat(verb.Name, verb.Required ? "Required" : "Optional"));
						}
					}

					var nouns = method.GetCustomAttributes(false).Where(o => o is Noun);
					if (nouns.Count() > 0)
					{
						sb.AppendLine("Nouns:");
						foreach (Noun noun in nouns)
						{
							sb.AppendLine("\t{0} - {1}".SFormat(noun.Name, noun.Required ? "Required" : "Optional"));
						}
					}
					sb.AppendLine("Example Usage: {0}?{1}".SFormat(routeattr.Route,
						string.Join("&", nouns.Select(n => String.Format("{0}={0}", ((Noun) n).Name)))));
					sb.AppendLine();
				}
			}

			File.WriteAllText("RestDescriptions.txt", sb.ToString());
		}

		private RestObject RestError(string message, string status = "400")
		{
			return new RestObject(status) {Error = message};
		}

		private RestObject RestResponse(string message, string status = "200")
		{
			return new RestObject(status) {Response = message};
		}

		private RestObject RestMissingParam(string var)
		{
			return RestError("Missing or empty " + var + " parameter");
		}

		private RestObject RestMissingParam(params string[] vars)
		{
			return RestMissingParam(string.Join(", ", vars));
		}

		private RestObject RestInvalidParam(string var)
		{
			return RestError("Missing or invalid " + var + " parameter");
		}

		private bool GetBool(string val, bool def)
		{
			bool ret;
			return bool.TryParse(val, out ret) ? ret : def;
		}

		private object PlayerFind(IParameterCollection parameters)
		{
			string name = parameters["player"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("player");

			var found = TShock.Utils.FindPlayer(name);
			switch(found.Count)
			{
				case 1:
					return found[0];
				case 0:
					return RestError("Player " + name + " was not found");
				default:
					return RestError("Player " + name + " matches " + found.Count + " players");
			}
		}

		private object UserFind(IParameterCollection parameters)
		{
			string name = parameters["user"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("user");

			User user;
			string type = parameters["type"];
			try
			{
				switch (type)
				{
					case null:
					case "name":
						type = "name";
						user = TShock.Users.GetUserByName(name);
						break;
					case "id":
						user = TShock.Users.GetUserByID(Convert.ToInt32(name));
						break;
					default:
						return RestError("Invalid Type: '" + type + "'");
				}
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			if (null == user)
				return RestError(String.Format("User {0} '{1}' doesn't exist", type, name));

			return user;
		}

		private object BanFind(IParameterCollection parameters)
		{
			string name = parameters["ban"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("ban");

			string type = parameters["type"];
			if (string.IsNullOrWhiteSpace(type))
				return RestMissingParam("type");

			Ban ban;
			switch (type)
			{
				case "ip":
					ban = TShock.Bans.GetBanByIp(name);
					break;
				case "name":
					ban = TShock.Bans.GetBanByName(name, GetBool(parameters["caseinsensitive"], true));
					break;
				default:
					return RestError("Invalid Type: '" + type + "'");
			}

			if (null == ban)
				return RestError("Ban " + type + " '" + name + "' doesn't exist");

			return ban;
		}

		private object GroupFind(IParameterCollection parameters)
		{
			var name = parameters["group"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("group");

			var group = TShock.Groups.GetGroupByName(name);
			if (null == group)
				return RestError("Group '" + name + "' doesn't exist");

			return group;
		}

		private Dictionary<string, object> PlayerFilter(TSPlayer tsPlayer, IParameterCollection parameters, bool viewips = false)
		{
			var player = new Dictionary<string, object>
				{
					{"nickname", tsPlayer.Name},
					{"username", tsPlayer.UserAccountName ?? ""},
					{"group", tsPlayer.Group.Name},
					{"active", tsPlayer.Active},
					{"state", tsPlayer.State},
					{"team", tsPlayer.Team},
				};

			if (viewips)
			{
				player.Add("ip", tsPlayer.IP);
			}
			foreach (IParameter filter in parameters)
			{
				if (player.ContainsKey(filter.Name) && !player[filter.Name].Equals(filter.Value))
					return null;
			}
			return player;
		}

		private object PlayerSetMute(IParameterCollection parameters, bool mute)
		{
			var ret = PlayerFind(parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			player.mute = mute;
			var verb = mute ? "muted" : "unmuted";
			player.SendInfoMessage("You have been remotely " + verb);
			return RestResponse("Player " + player.Name + " was " + verb);
		}

		#endregion
	}
}
