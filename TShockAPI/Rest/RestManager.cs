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
			Rest.Register(new RestCommand("/status", Status) {RequiresToken = false});
			Rest.Register(new RestCommand("/tokentest", TokenTest) {RequiresToken = true});

			Rest.Register(new RestCommand("/v2/users/activelist", UserListV2) { RequiresToken = true });
			Rest.Register(new RestCommand("/v2/users/read", UserInfoV2) { RequiresToken = true });
			Rest.Register(new RestCommand("/v2/users/destroy", UserDestroyV2) { RequiresToken = true });
			Rest.Register(new RestCommand("/v2/users/update", UserUpdateV2) { RequiresToken = true });

			Rest.Register(new RestCommand("/bans/create", BanCreate) {RequiresToken = true});
			Rest.Register(new RestCommand("/v2/bans/read", BanInfoV2) { RequiresToken = true });
			Rest.Register(new RestCommand("/v2/bans/destroy", BanDestroyV2) { RequiresToken = true });

			Rest.Register(new RestCommand("/lists/players", PlayerList) {RequiresToken = true});

			Rest.Register(new RestCommand("/world/read", WorldRead) {RequiresToken = true});
			Rest.Register(new RestCommand("/world/meteor", WorldMeteor) {RequiresToken = true});
			Rest.Register(new RestCommand("/world/bloodmoon/{bool}", WorldBloodmoon) {RequiresToken = true});
			Rest.Register(new RestCommand("/v2/world/save", WorldSave) { RequiresToken = true});
			Rest.Register(new RestCommand("/v2/world/autosave/state/{bool}", ChangeWorldSaveSettings) { RequiresToken = true });
			Rest.Register(new RestCommand("/v2/world/butcher", Butcher) {RequiresToken = true});

			Rest.Register(new RestCommand("/v2/players/read", PlayerReadV2) { RequiresToken = true });
			Rest.Register(new RestCommand("/v2/players/kick", PlayerKickV2) { RequiresToken = true });
			Rest.Register(new RestCommand("/v2/players/ban", PlayerBanV2) { RequiresToken = true });
			Rest.Register(new RestCommand("/v2/players/kill", PlayerKill) {RequiresToken = true});
			Rest.Register(new RestCommand("/v2/players/mute", PlayerMute) {RequiresToken = true});
			Rest.Register(new RestCommand("/v2/players/unmute", PlayerUnMute) {RequiresToken = true});

			Rest.Register(new RestCommand("/v2/server/broadcast", Broadcast) { RequiresToken = true});
			Rest.Register(new RestCommand("/v2/server/off", Off) {RequiresToken = true});
			Rest.Register(new RestCommand("/v2/server/rawcmd", ServerCommand) {RequiresToken = true});
		}

		#region RestServerMethods

		private object ServerCommand(RestVerbs verbs, IParameterCollection parameters)
		{
			if (parameters["cmd"] != null && parameters["cmd"].Trim() != "")
			{
				TSRestPlayer tr = new TSRestPlayer();
				RestObject ro = new RestObject("200");
				Commands.HandleCommand(tr, parameters["cmd"]);
				foreach (string s in tr.GetCommandOutput())
				{
					ro.Add("response", s);
				}
				return ro;
			}
			RestObject fail = new RestObject("400");
			fail["response"] = "Missing or blank cmd parameter.";
			return fail;
		}

		private object Off(RestVerbs verbs, IParameterCollection parameters)
		{
			bool confirm;
			bool.TryParse(parameters["confirm"], out confirm);
			bool nosave;
			bool.TryParse(parameters["nosave"], out nosave);

			if (confirm == true)
			{
				if (!nosave)
					WorldGen.saveWorld();
				Netplay.disconnect = true;
				RestObject reply = new RestObject("200");
				reply["response"] = "The server is shutting down.";
				return reply;
			}
			RestObject fail = new RestObject("400");
			fail["response"] = "Invalid/missing confirm switch, and/or missing nosave switch.";
			return fail;
		}

		private object Broadcast(RestVerbs verbs, IParameterCollection parameters)
		{
			if (parameters["msg"] != null && parameters["msg"].Trim() != "")
			{
				TShock.Utils.Broadcast(parameters["msg"]);
				RestObject reply = new RestObject("200");
				reply["response"] = "The message was broadcasted successfully.";
				return reply;
			}
			RestObject fail = new RestObject("400");
			fail["response"] = "Broadcast failed.";
			return fail;
		}

		#endregion

		#region RestMethods

		private object TokenTest(RestVerbs verbs, IParameterCollection parameters)
		{
			return new Dictionary<string, string>
					{{"status", "200"}, {"response", "Token is valid and was passed through correctly."}};
		}

		private object Status(RestVerbs verbs, IParameterCollection parameters)
		{
			if (TShock.Config.EnableTokenEndpointAuthentication)
				return new RestObject("403") {Error = "Server settings require a token for this API call."};

			var activeplayers = Main.player.Where(p => p != null && p.active).ToList();
			string currentPlayers = string.Join(", ", activeplayers.Select(p => p.name));

			var ret = new RestObject("200");
			ret["name"] = TShock.Config.ServerNickname;
			ret["port"] = Convert.ToString(TShock.Config.ServerPort);
			ret["playercount"] = Convert.ToString(activeplayers.Count());
			ret["players"] = currentPlayers;

			return ret;
		}

		#endregion

		#region RestUserMethods

		private object UserListV2(RestVerbs verbs, IParameterCollection parameters)
		{
			string playerlist = "";
			foreach (var TSPlayer in TShock.Players)
			{
				playerlist += playerlist == "" ? TSPlayer.UserAccountName : "\t" + TSPlayer.UserAccountName;
			}
            var returnBlock = new Dictionary<string, string>();
            returnBlock.Add("status", "200");
            returnBlock.Add("activeusers", playerlist);
			return returnBlock;
		}

		private object UserUpdateV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, string>();
			var password = parameters["password"];
			var group = parameters["group"];

			if (group == null && password == null)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "No parameters were passed.");
				return returnBlock;
			}

			var user = TShock.Users.GetUserByName(parameters["user"]);
			if (user == null)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "The specefied user doesn't exist.");
				return returnBlock;
			}

			if (password != null)
			{
				TShock.Users.SetUserPassword(user, password);
				returnBlock.Add("password-response", "Password updated successfully.");
			}

			if (group != null)
			{
				TShock.Users.SetUserGroup(user, group);
				returnBlock.Add("group-response", "Group updated successfully.");
			}

			returnBlock.Add("status", "200");
			return returnBlock;
		}

		private object UserDestroyV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var user = TShock.Users.GetUserByName(parameters["user"]);
			if (user == null)
			{
				return new Dictionary<string, string> {{"status", "400"}, {"error", "The specified user account does not exist."}};
			}
			var returnBlock = new Dictionary<string, string>();
			try
			{
				TShock.Users.RemoveUser(user);
			}
			catch (Exception)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "The specified user was unable to be removed.");
				return returnBlock;
			}
			returnBlock.Add("status", "200");
			returnBlock.Add("response", "User deleted successfully.");
			return returnBlock;
		}

		private object UserInfoV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var user = TShock.Users.GetUserByName(parameters["user"]);
			if (user == null)
			{
				return new Dictionary<string, string> {{"status", "400"}, {"error", "The specified user account does not exist."}};
			}

			var returnBlock = new Dictionary<string, string>();
			returnBlock.Add("status", "200");
			returnBlock.Add("group", user.Group);
			returnBlock.Add("id", user.ID.ToString());
			return returnBlock;
		}

		#endregion

		#region RestBanMethods

		private object BanCreate(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, string>();
			var ip = parameters["ip"];
			var name = parameters["name"];
			var reason = parameters["reason"];

			if (ip == null && name == null)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Required parameters were missing from this API endpoint.");
				return returnBlock;
			}

			if (ip == null)
			{
				ip = "";
			}

			if (name == null)
			{
				name = "";
			}

			if (reason == null)
			{
				reason = "";
			}

			try
			{
				TShock.Bans.AddBan(ip, name, reason);
			}
			catch (Exception)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "The specified ban was unable to be created.");
				return returnBlock;
			}
			returnBlock.Add("status", "200");
			returnBlock.Add("response", "Ban created successfully.");
			return returnBlock;
		}

		private object BanDestroyV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, string>();

			var type = parameters["type"];
			if (type == null)
			{
				returnBlock.Add("Error", "Invalid Type");
				return returnBlock;
			}

			var ban = new Ban();
			if (type == "ip") ban = TShock.Bans.GetBanByIp(parameters["user"]);
			else if (type == "name") ban = TShock.Bans.GetBanByName(parameters["user"]);
			else
			{
				returnBlock.Add("Error", "Invalid Type");
				return returnBlock;
			}

			if (ban == null)
			{
				return new Dictionary<string, string> {{"status", "400"}, {"error", "The specified ban does not exist."}};
			}

			try
			{
				TShock.Bans.RemoveBan(ban.IP);
			}
			catch (Exception)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "The specified ban was unable to be removed.");
				return returnBlock;
			}
			returnBlock.Add("status", "200");
			returnBlock.Add("response", "Ban deleted successfully.");
			return returnBlock;
		}

		private object BanInfoV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, string>();

			var type = parameters["type"];
			if (type == null)
			{
				returnBlock.Add("Error", "Invalid Type");
				return returnBlock;
			}

			var ban = new Ban();
			if (type == "ip") ban = TShock.Bans.GetBanByIp(parameters["user"]);
			else if (type == "name") ban = TShock.Bans.GetBanByName(parameters["user"]);
			else
			{
				returnBlock.Add("Error", "Invalid Type");
				return returnBlock;
			}

			if (ban == null)
			{
				return new Dictionary<string, string> { { "status", "400" }, { "error", "The specified ban does not exist." } };
			}

			returnBlock.Add("status", "200");
			returnBlock.Add("name", ban.Name);
			returnBlock.Add("ip", ban.IP);
			returnBlock.Add("reason", ban.Reason);
			return returnBlock;
		}

		#endregion

		#region RestWorldMethods

		private object ChangeWorldSaveSettings(RestVerbs verbs, IParameterCollection parameters)
		{
			bool state;
			bool.TryParse(verbs["state"], out state);

				if (state == true)
				{
					TShock.Config.AutoSave = true;
				}
				else
				{
					TShock.Config.AutoSave = false;
				}

				RestObject rj = new RestObject("200");
				rj["response"] = "Value changed";
				rj["state"] = state;

				return rj;
		}

		private object WorldSave(RestVerbs verbs, IParameterCollection parameters)
		{
			TShock.Utils.SaveWorld();

			RestObject rj = new RestObject("200");
			rj["response"] = "World saved.";
			return rj;
		}

		private object Butcher(RestVerbs verbs, IParameterCollection parameters)
		{
			bool killFriendly;
			if (!bool.TryParse(parameters["killfriendly"], out killFriendly))
			{
				RestObject fail = new RestObject("400");
				fail["response"] = "The given value for killfriendly wasn't a boolean value.";
				return fail;
			}
			if (killFriendly)
			{
				killFriendly = !killFriendly;
			}

			int killcount = 0;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type != 0 && !Main.npc[i].townNPC && (!Main.npc[i].friendly || killFriendly))
				{
					TSPlayer.Server.StrikeNPC(i, 99999, 90f, 1);
					killcount++;
				}
			}

			RestObject rj = new RestObject("200");
			rj["response"] = killcount + " NPCs have been killed.";
			return rj;
		}

		private object WorldRead(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, object>();
			returnBlock.Add("status", "200");
			returnBlock.Add("name", Main.worldName);
			returnBlock.Add("size", Main.maxTilesX + "*" + Main.maxTilesY);
			returnBlock.Add("time", Main.time);
			returnBlock.Add("daytime", Main.dayTime);
			returnBlock.Add("bloodmoon", Main.bloodMoon);
			returnBlock.Add("invasionsize", Main.invasionSize);
			return returnBlock;
		}

		private object WorldMeteor(RestVerbs verbs, IParameterCollection parameters)
		{
			if (WorldGen.genRand == null)
				WorldGen.genRand = new Random();
			WorldGen.dropMeteor();
			var returnBlock = new Dictionary<string, string> {{"status", "200"}, {"response", "Meteor has been spawned."}};
			return returnBlock;
		}

		private object WorldBloodmoon(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, string>();
			var bloodmoonVerb = verbs["bool"];
			bool bloodmoon;
			if (bloodmoonVerb == null)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "No parameter was passed.");
				return returnBlock;
			}
			if (!bool.TryParse(bloodmoonVerb, out bloodmoon))
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Unable to parse parameter.");
				return returnBlock;
			}
			Main.bloodMoon = bloodmoon;
			returnBlock.Add("status", "200");
			returnBlock.Add("response", "Blood Moon has been set to " + bloodmoon);
			return returnBlock;
		}

		#endregion

		#region RestPlayerMethods

		private object PlayerUnMute(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, object>();
			var playerParam = parameters["player"];
			var found = TShock.Utils.FindPlayer(playerParam);
			var reason = parameters["reason"];
			if (found.Count == 0)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " was not found");
			}
			else if (found.Count > 1)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " matches " + playerParam.Count() + " players");
			}
			else if (found.Count == 1)
			{
				var player = found[0];
				player.mute = false;
				player.SendMessage("You have been remotely unmuted.");
				returnBlock.Add("status", "200");
				returnBlock.Add("response", "Player " + player.Name + " was muted.");
			}
			return returnBlock;
		}

		private object PlayerMute(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, object>();
			var playerParam = parameters["player"];
			var found = TShock.Utils.FindPlayer(playerParam);
			var reason = parameters["reason"];
			if (found.Count == 0)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " was not found");
			}
			else if (found.Count > 1)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " matches " + playerParam.Count() + " players");
			}
			else if (found.Count == 1)
			{
				var player = found[0];
				player.mute = true;
				player.SendMessage("You have been remotely muted.");
				returnBlock.Add("status", "200");
				returnBlock.Add("response", "Player " + player.Name + " was muted.");
			}
			return returnBlock;
		}

		private object PlayerList(RestVerbs verbs, IParameterCollection parameters)
		{
			var activeplayers = Main.player.Where(p => p != null && p.active).ToList();
			string currentPlayers = string.Join(", ", activeplayers.Select(p => p.name));
			var ret = new RestObject("200");
			ret["players"] = currentPlayers;
			return ret;
		}

		private object PlayerReadV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, object>();
			var playerParam = parameters["player"];
			var found = TShock.Utils.FindPlayer(playerParam);
			if (found.Count == 0)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " was not found");
			}
			else if (found.Count > 1)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " matches " + playerParam.Count() + " players");
			}
			else if (found.Count == 1)
			{
				var player = found[0];
				returnBlock.Add("status", "200");
				returnBlock.Add("nickname", player.Name);
				returnBlock.Add("username", player.UserAccountName == null ? "" : player.UserAccountName);
				returnBlock.Add("ip", player.IP);
				returnBlock.Add("group", player.Group.Name);
				returnBlock.Add("position", player.TileX + "," + player.TileY);
				var activeItems = player.TPlayer.inventory.Where(p => p.active).ToList();
				returnBlock.Add("inventory", string.Join(", ", activeItems.Select(p => (p.name + ":" + p.stack))));
				returnBlock.Add("buffs", string.Join(", ", player.TPlayer.buffType));
			}
			return returnBlock;
		}

		private object PlayerKickV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, object>();
			var playerParam = parameters["player"];
			var found = TShock.Utils.FindPlayer(playerParam);
			var reason = parameters["reason"];
			if (found.Count == 0)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " was not found");
			}
			else if (found.Count > 1)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " matches " + playerParam.Count() + " players");
			}
			else if (found.Count == 1)
			{
				var player = found[0];
				TShock.Utils.ForceKick(player, reason == null ? "Kicked via web" : reason);
				returnBlock.Add("status", "200");
				returnBlock.Add("response", "Player " + player.Name + " was kicked");
			}
			return returnBlock;
		}

		private object PlayerBanV2(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, object>();
			var playerParam = parameters["player"];
			var found = TShock.Utils.FindPlayer(playerParam);
			var reason = parameters["reason"];
			if (found.Count == 0)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " was not found");
			}
			else if (found.Count > 1)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " matches " + playerParam.Count() + " players");
			}
			else if (found.Count == 1)
			{
				var player = found[0];
				TShock.Bans.AddBan(player.IP, player.Name, reason == null ? "Banned via web" : reason);
				TShock.Utils.ForceKick(player, reason == null ? "Banned via web" : reason);
				returnBlock.Add("status", "200");
				returnBlock.Add("response", "Player " + player.Name + " was banned");
			}
			return returnBlock;
		}

		private object PlayerKill(RestVerbs verbs, IParameterCollection parameters)
		{
			var returnBlock = new Dictionary<string, object>();
			var playerParam = parameters["player"];
			var found = TShock.Utils.FindPlayer(playerParam);
			var from = verbs["from"];
			if (found.Count == 0)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " was not found");
			}
			else if (found.Count > 1)
			{
				returnBlock.Add("status", "400");
				returnBlock.Add("error", "Name " + playerParam + " matches " + playerParam.Count() + " players");
			}
			else if (found.Count == 1)
			{
				var player = found[0];
				player.DamagePlayer(999999);
				player.SendMessage(string.Format("{0} just killed you!", from));
				returnBlock.Add("status", "200");
				returnBlock.Add("response", "Player " + player.Name + " was killed.");
			}
			return returnBlock;
		}

		#endregion
	}
}