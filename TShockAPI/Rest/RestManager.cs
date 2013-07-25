/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

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
using System.Linq;
using HttpServer;
using Rests;
using Terraria;
using TShockAPI.DB;

namespace TShockAPI
{
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
			Rest.Register(new SecureRestCommand("/v2/server/broadcast", ServerBroadcast));
			Rest.Register(new SecureRestCommand("/v2/server/off", ServerOff, Permissions.restmaintenance));
			Rest.Register(new SecureRestCommand("/v2/server/restart", ServerRestart, Permissions.restmaintenance));
			Rest.Register(new SecureRestCommand("/v2/server/reload", ServerReload, Permissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/server/rawcmd", ServerCommand, Permissions.restrawcommand));
			Rest.Register(new SecureRestCommand("/v3/server/rawcmd", ServerCommandV3, Permissions.restrawcommand));
			Rest.Register(new SecureRestCommand("/tokentest", ServerTokenTest));

			if (TShock.Config.EnableTokenEndpointAuthentication)
			{
				Rest.Register(new SecureRestCommand("/v2/server/status", ServerStatusV2));
				Rest.Register(new SecureRestCommand("/status", ServerStatus));
			}
			else
			{
				Rest.Register(new RestCommand("/v2/server/status", (a, b) => this.ServerStatusV2(a, b, SecureRest.TokenData.None)));
				Rest.Register(new RestCommand("/status", (a, b) => this.ServerStatusV2(a, b, SecureRest.TokenData.None)));
			}

			// User Commands
			Rest.Register(new SecureRestCommand("/v2/users/activelist", UserActiveListV2, Permissions.restviewusers));
			Rest.Register(new SecureRestCommand("/v2/users/create", UserCreateV2, Permissions.restmanageusers) { DoLog = false });
			Rest.Register(new SecureRestCommand("/v2/users/list", UserListV2, Permissions.restviewusers));
			Rest.Register(new SecureRestCommand("/v2/users/read", UserInfoV2, Permissions.restviewusers));
			Rest.Register(new SecureRestCommand("/v2/users/destroy", UserDestroyV2, Permissions.restmanageusers));
			Rest.Register(new SecureRestCommand("/v2/users/update", UserUpdateV2, Permissions.restmanageusers) { DoLog = false });

			// Ban Commands
			Rest.Register(new SecureRestCommand("/bans/create", BanCreate, Permissions.restmanagebans));
			Rest.Register(new SecureRestCommand("/v2/bans/list", BanListV2, Permissions.restviewbans));
			Rest.Register(new SecureRestCommand("/v2/bans/read", BanInfoV2, Permissions.restviewbans));
			Rest.Register(new SecureRestCommand("/v2/bans/destroy", BanDestroyV2, Permissions.restmanagebans));

			// World Commands
			Rest.Register(new SecureRestCommand("/world/read", WorldRead));
			Rest.Register(new SecureRestCommand("/world/meteor", WorldMeteor, Permissions.restcauseevents));
			Rest.Register(new SecureRestCommand("/world/bloodmoon/{bool}", WorldBloodmoon, Permissions.restcauseevents));
			Rest.Register(new SecureRestCommand("/v2/world/save", WorldSave, Permissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/world/autosave/state/{bool}", WorldChangeSaveSettings, Permissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/world/butcher", WorldButcher, Permissions.restbutcher));

			// Player Commands
			Rest.Register(new SecureRestCommand("/lists/players", PlayerList));
			Rest.Register(new SecureRestCommand("/v2/players/list", PlayerListV2));
			Rest.Register(new SecureRestCommand("/v2/players/read", PlayerReadV2, Permissions.restuserinfo));
			Rest.Register(new SecureRestCommand("/v2/players/kick", PlayerKickV2, Permissions.restkick));
			Rest.Register(new SecureRestCommand("/v2/players/ban", PlayerBanV2, Permissions.restban, Permissions.restmanagebans));
			Rest.Register(new SecureRestCommand("/v2/players/kill", PlayerKill, Permissions.restkill));
			Rest.Register(new SecureRestCommand("/v2/players/mute", PlayerMute, Permissions.restmute));
			Rest.Register(new SecureRestCommand("/v2/players/unmute", PlayerUnMute, Permissions.restmute));

			// Group Commands
			Rest.Register(new SecureRestCommand("/v2/groups/list", GroupList, Permissions.restviewgroups));
			Rest.Register(new SecureRestCommand("/v2/groups/read", GroupInfo, Permissions.restviewgroups));
			Rest.Register(new SecureRestCommand("/v2/groups/destroy", GroupDestroy, Permissions.restmanagegroups));
			Rest.Register(new SecureRestCommand("/v2/groups/create", GroupCreate, Permissions.restmanagegroups));
			Rest.Register(new SecureRestCommand("/v2/groups/update", GroupUpdate, Permissions.restmanagegroups));
		}

		#region RestServerMethods

		private object ServerCommand(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			if (string.IsNullOrWhiteSpace(parameters["cmd"]))
				return RestMissingParam("cmd");

			TSRestPlayer tr = new TSRestPlayer(tokenData.Username, tokenData.UserGroup);
			Commands.HandleCommand(tr, parameters["cmd"]);
			return RestResponse(string.Join("\n", tr.GetCommandOutput()));
		}

		private object ServerCommandV3(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			if (string.IsNullOrWhiteSpace(parameters["cmd"]))
				return RestMissingParam("cmd");

			TSRestPlayer tr = new TSRestPlayer(tokenData.Username, tokenData.UserGroup);
			Commands.HandleCommand(tr, parameters["cmd"]);
			return new Dictionary<string,object>
			{
				{"status", "200"},
				{"response", tr.GetCommandOutput()}
			};
		}

		private object ServerOff(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			if (!GetBool(parameters["confirm"], false))
				return RestInvalidParam("confirm");

			// Inform players the server is shutting down
			var reason = string.IsNullOrWhiteSpace(parameters["message"]) ? "Server is shutting down" : parameters["message"];
			TShock.Utils.StopServer(!GetBool(parameters["nosave"], false), reason);

			return RestResponse("The server is shutting down");
		}

		private object ServerRestart(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			if (!GetBool(parameters["confirm"], false))
				return RestInvalidParam("confirm");

			// Inform players the server is shutting down
			var reason = string.IsNullOrWhiteSpace(parameters["message"]) ? "Server is shutting down" : parameters["message"];
			TShock.Utils.RestartServer(!GetBool(parameters["nosave"], false), reason);

			return RestResponse("The server is shutting down and will attempt to restart");
		}

		private object ServerReload(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			try {
				TShock.Utils.Reload(new TSRestPlayer(tokenData.Username, tokenData.UserGroup));
			} catch (Exception ex) {
				return RestError("Exception was thrown during the reload: " + ex);
			}
			
			return RestResponse("Configuration, permissions, and regions reload complete. Some changes may require a server restart.");
		}

		private object ServerBroadcast(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var msg = parameters["msg"];
			if (string.IsNullOrWhiteSpace(msg))
				return RestMissingParam("msg");
			TShock.Utils.Broadcast(msg);
			return RestResponse("The message was broadcasted successfully");
		}

		private object ServerStatus(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
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

		private object ServerStatusV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = new RestObject()
			{
				{"name", TShock.Config.ServerName},
				{"port", TShock.Config.ServerPort},
				{"playercount", Main.player.Where(p => null != p && p.active).Count()},
				{"maxplayers", TShock.Config.MaxSlots},
				{"world", Main.worldName},
				{"uptime", (DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime).ToString(@"d'.'hh':'mm':'ss")},
				{"serverpassword", !string.IsNullOrEmpty(TShock.Config.ServerPassword)}
			};

			if (GetBool(parameters["players"], false))
			{
				var players = new ArrayList();
				foreach (TSPlayer tsPlayer in TShock.Players.Where(p => null != p))
				{
					var p = PlayerFilter(tsPlayer, parameters);
					if (null != p)
						players.Add(p);
				}
				ret.Add("players", players);
			}

			if (GetBool(parameters["rules"], false))
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
				rules.Add("ServerSideInventory", TShock.Config.ServerSideInventory);

				ret.Add("rules", rules);
			}
			return ret;
		}

		private object ServerTokenTest(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			return RestResponse("Token is valid and was passed through correctly");
		}

		#endregion

		#region RestUserMethods

		private object UserActiveListV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			return new RestObject() { { "activeusers", string.Join("\t", TShock.Players.Where(p => null != p && null != p.UserAccountName && p.Active).Select(p => p.UserAccountName)) } };
		}

		private object UserListV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			return new RestObject() { { "users", TShock.Users.GetUsers().Select(p => new Dictionary<string,object>(){
				{"name", p.Name},
				{"id", p.ID},
				{"group", p.Group},
				{"ip", p.Address},
			}) } };
		}

		private object UserCreateV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var username = parameters["user"];
			if (string.IsNullOrWhiteSpace(username))
				return RestMissingParam("user");

			var group = parameters["group"];
			if (string.IsNullOrWhiteSpace(group))
				return RestMissingParam("group");

			var password = parameters["password"];
			if (string.IsNullOrWhiteSpace(password))
				return RestMissingParam("password");

			// NOTE: ip can be blank
			User user = new User(parameters["ip"], username, password, group);
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

		private object UserUpdateV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = UserFind(parameters);
			if (ret is RestObject)
				return ret;

			var password = parameters["password"];
			var group = parameters["group"];
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

		private object UserDestroyV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = UserFind(parameters);
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

		private object UserInfoV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = UserFind(parameters);
			if (ret is RestObject)
				return ret;

			User user = (User)ret;
			return new RestObject() { { "group", user.Group }, { "id", user.ID.ToString() }, { "name", user.Name } };
		}

		#endregion

		#region RestBanMethods

		private object BanCreate(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ip = parameters["ip"];
			var name = parameters["name"];

			if (string.IsNullOrWhiteSpace(ip) && string.IsNullOrWhiteSpace(name))
				return RestMissingParam("ip", "name");

			try
			{
				TShock.Bans.AddBan(ip, name, parameters["reason"], true);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}
			return RestResponse("Ban created successfully");
		}

		private object BanDestroyV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = BanFind(parameters);
			if (ret is RestObject)
				return ret;

			try
			{
				Ban ban = (Ban)ret;
				switch (parameters["type"])
				{
					case "ip":
						if (!TShock.Bans.RemoveBan(ban.IP, false, false, true))
							return RestResponse("Failed to delete ban (already deleted?)");
						break;
					case "name":
						if (!TShock.Bans.RemoveBan(ban.Name, true, GetBool(parameters["caseinsensitive"], true)))
							return RestResponse("Failed to delete ban (already deleted?)");
						break;
					default:
						return RestError("Invalid Type: '" + parameters["type"] + "'");
				}

			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("Ban deleted successfully");
		}

		private object BanInfoV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = BanFind(parameters);
			if (ret is RestObject)
				return ret;

			Ban ban = (Ban)ret;
			return new RestObject() {
				{"name", null == ban.Name ? "" : ban.Name},
				{"ip", null == ban.IP ? "" : ban.IP},
				{"reason", null == ban.Reason ? "" : ban.Reason},
			};
		}

		private object BanListV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
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

		private object WorldChangeSaveSettings(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			bool autoSave;
			if (!bool.TryParse(verbs["bool"], out autoSave))
				return RestInvalidParam("state");
			TShock.Config.AutoSave = autoSave;

			return RestResponse("AutoSave has been set to " + autoSave);
		}

		private object WorldSave(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			SaveManager.Instance.SaveWorld();

			return RestResponse("World saved");
		}

		private object WorldButcher(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			bool killFriendly;
			if (!bool.TryParse(parameters["killfriendly"], out killFriendly))
				return RestInvalidParam("killfriendly");

			if (killFriendly)
				killFriendly = !killFriendly;

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

		private object WorldRead(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
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

		private object WorldMeteor(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			if (null == WorldGen.genRand)
				WorldGen.genRand = new Random();
			WorldGen.dropMeteor();
			return RestResponse("Meteor has been spawned");
		}

		private object WorldBloodmoon(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			bool bloodmoon;
			if (!bool.TryParse(verbs["bool"], out bloodmoon))
				return RestInvalidParam("bloodmoon");
			Main.bloodMoon = bloodmoon;

			return RestResponse("Blood Moon has been set to " + bloodmoon);
		}

		#endregion

		#region RestPlayerMethods

		private object PlayerUnMute(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			return PlayerSetMute(parameters, false);
		}

		private object PlayerMute(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			return PlayerSetMute(parameters, true);
		}

		private object PlayerList(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var activeplayers = Main.player.Where(p => null != p && p.active).ToList();
			return new RestObject() { { "players", string.Join(", ", activeplayers.Select(p => p.name)) } };
		}

		private object PlayerListV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var playerList = new ArrayList();
			foreach (TSPlayer tsPlayer in TShock.Players.Where(p => null != p))
			{
				var p = PlayerFilter(tsPlayer, parameters);
				if (null != p)
					playerList.Add(p);
			}
			return new RestObject() { { "players", playerList } };
		}

		private object PlayerReadV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = PlayerFind(parameters);
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

		private object PlayerKickV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = PlayerFind(parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			TShock.Utils.ForceKick(player, null == parameters["reason"] ? "Kicked via web" : parameters["reason"], false, true);
			return RestResponse("Player " + player.Name + " was kicked");
		}

		private object PlayerBanV2(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = PlayerFind(parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			var reason = null == parameters["reason"] ? "Banned via web" : parameters["reason"];
			TShock.Bans.AddBan(player.IP, player.Name, reason);
			TShock.Utils.ForceKick(player, reason, false, true);
			return RestResponse("Player " + player.Name + " was banned");
		}

		private object PlayerKill(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = PlayerFind(parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			player.DamagePlayer(999999);
			var from = string.IsNullOrWhiteSpace(parameters["from"]) ? "Server Admin" : parameters["from"];
			player.SendMessage(string.Format("{0} just killed you!", from));
			return RestResponse("Player " + player.Name + " was killed");
		}

		#endregion

		#region RestGroupMethods

		private object GroupList(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var groups = new ArrayList();
			foreach (Group group in TShock.Groups)
			{
				groups.Add(new Dictionary<string, object> {{"name", group.Name}, {"parent", group.ParentName}, {"chatcolor", group.ChatColor}});
			}
			return new RestObject() { { "groups", groups } };
		}

		private object GroupInfo(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = GroupFind(parameters);
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

		private object GroupDestroy(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = GroupFind(parameters);
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

		private object GroupCreate(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var name = parameters["group"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("group");
			try
			{
				TShock.Groups.AddGroup(name, parameters["parent"], parameters["permissions"], parameters["chatcolor"], true);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("Group '" + name + "' created successfully");
		}

		private object GroupUpdate(RestVerbs verbs, IParameterCollection parameters, SecureRest.TokenData tokenData)
		{
			var ret = GroupFind(parameters);
			if (ret is RestObject)
				return ret;

			Group group = (Group)ret;
			var parent = (null == parameters["parent"]) ? group.ParentName : parameters["parent"];
			var chatcolor = (null == parameters["chatcolor"]) ? string.Format("{0}.{1}.{2}", group.R, group.G, group.B) : parameters["chatcolor"];
			var permissions = (null == parameters["permissions"]) ? group.Permissions : parameters["permissions"];
			try
			{
				TShock.Groups.UpdateGroup(group.Name, parent, permissions, chatcolor);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("Group '" + group.Name + "' updated successfully");
		}

		#endregion

		#region Utility Methods

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
					case "ip":
						user = TShock.Users.GetUserByIP(name);

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

		private Dictionary<string, object> PlayerFilter(TSPlayer tsPlayer, IParameterCollection parameters)
		{
			var player = new Dictionary<string, object>
				{
					{"nickname", tsPlayer.Name},
					{"username", null == tsPlayer.UserAccountName ? "" : tsPlayer.UserAccountName},
					{"ip", tsPlayer.IP},
					{"group", tsPlayer.Group.Name},
					{"active", tsPlayer.Active},
					{"state", tsPlayer.State},
					{"team", tsPlayer.Team},
				};
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
			player.SendMessage("You have been remotely " + verb);
			return RestResponse("Player " + player.Name + " was " + verb);
		}

		#endregion
	}
}
