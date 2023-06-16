﻿/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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
using Newtonsoft.Json;

namespace TShockAPI
{
	/// <summary>
	/// Describes the permission required to use an API route
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class Permission : Attribute
	{
		/// <summary>
		/// Name of the permission
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="Permission"/> with the given name
		/// </summary>
		/// <param name="name">Permission required</param>
		public Permission(string name)
		{
			Name = name;
		}
	}

	/// <summary>
	/// Describes the route of a REST API call
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class RouteAttribute : Attribute
	{
		/// <summary>
		/// The route used to call the API
		/// </summary>
		public string Route { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="RouteAttribute"/> with the given route
		/// </summary>
		/// <param name="route">Route used to call the API</param>
		public RouteAttribute(string route)
		{
			Route = route;
		}
	}

	/// <summary>
	/// Describes a parameter in a REST route
	/// </summary>
	public class ParameterAttribute : Attribute
	{
		/// <summary>
		/// The parameter's name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Whether the parameter is required or not
		/// </summary>
		public bool Required { get; set; }
		/// <summary>
		/// The parameter's description
		/// </summary>
		public string Description { get; set; }
		/// <summary>
		/// The parameter's System Type
		/// </summary>
		public Type ArgumentType { get; set; }

		/// <summary>
		/// Creates a new instance of <see cref="ParameterAttribute"/> with the given name, description, and type.
		/// A ParameterAttribute may be optional or required.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="req"></param>
		/// <param name="desc"></param>
		/// <param name="type"></param>
		public ParameterAttribute(string name, bool req, string desc, Type type)
		{
			Name = name;
			Required = req;
			Description = desc;
			ArgumentType = type;
		}
	}

	/// <summary>
	/// Describes a parameter in a REST route
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class Noun : ParameterAttribute
	{
		/// <summary>
		/// Creates a new instance of <see cref="Noun"/> with the given name, description, and type.
		/// Nouns may be optional or required. A required Noun is akin to a <see cref="Verb"/>
		/// </summary>
		/// <param name="name">Name of the noun</param>
		/// <param name="req">Whether the noun is required or not</param>
		/// <param name="desc">Decription of the noun</param>
		/// <param name="type">System Type of the noun</param>
		public Noun(string name, bool req, string desc, Type type) : base(name, req, desc, type) { }
	}

	/// <summary>
	/// Describes a parameter in a REST route
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class Verb : ParameterAttribute
	{
		/// <summary>
		/// Creates a new instance of <see cref="Verb"/> with the given name, description, and type.
		/// Verbs are required arguments.
		/// </summary>
		/// <param name="name">Name of the verb</param>
		/// <param name="desc">Description of the verb</param>
		/// <param name="type">System Type of the verb</param>
		public Verb(string name, string desc, Type type) : base(name, true, desc, type) { }
	}

	/// <summary>
	/// Describes a REST authentication token
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class Token : Noun
	{
		/// <summary>
		/// Creates a new instance of <see cref="Token"/>
		/// </summary>
		public Token() : base("token", true, GetString("The REST authentication token."), typeof(String)) { }
	}

	/// <summary>
	/// Manages a <see cref="Rests.Rest"/> instance
	/// </summary>
	public class RestManager
	{
		/// <summary>
		/// The RESTful API service that handles API requests
		/// </summary>
		private Rest Rest;

		/// <summary>
		/// Creates a new instance of <see cref="RestManager"/> using the provided <see cref="Rest"/> object
		/// </summary>
		/// <param name="rest"></param>
		public RestManager(Rest rest)
		{
			Rest = rest;
		}

		/// <summary>
		/// Registers default TShock REST commands
		/// </summary>
		public void RegisterRestfulCommands()
		{
			// Server Commands
			if (TShock.Config.Settings.EnableTokenEndpointAuthentication)
			{
				Rest.Register(new SecureRestCommand("/v2/server/status", ServerStatusV2));
				Rest.Register(new SecureRestCommand("/v3/server/motd", ServerMotd));
				Rest.Register(new SecureRestCommand("/v3/server/rules", ServerRules));
			}
			else
			{
				Rest.Register(new RestCommand("/v2/server/status", (a) => ServerStatusV2(new RestRequestArgs(a.Verbs, a.Parameters, a.Request, SecureRest.TokenData.None, a.Context))));
				Rest.Register(new RestCommand("/v3/server/motd", (a) => ServerMotd(new RestRequestArgs(a.Verbs, a.Parameters, a.Request, SecureRest.TokenData.None, a.Context))));
				Rest.Register(new RestCommand("/v3/server/rules", (a) => ServerRules(new RestRequestArgs(a.Verbs, a.Parameters, a.Request, SecureRest.TokenData.None, a.Context))));
			}

			Rest.RegisterRedirect("/status", "/v2/server/status");
			Rest.RegisterRedirect("/token/create", "/v2/token/create");

			//server commands
			Rest.RegisterRedirect("/server/motd", "/v3/server/motd");
			Rest.RegisterRedirect("/server/rules", "/v3/server/rules");
			Rest.RegisterRedirect("/server/broadcast", "/v2/server/broadcast");
			Rest.RegisterRedirect("/server/reload", "/v2/server/reload");
			Rest.RegisterRedirect("/server/off", "/v2/server/off");
			Rest.RegisterRedirect("/server/rawcmd", "/v3/server/rawcmd");

			//user commands
			Rest.RegisterRedirect("/users/activelist", "/v2/users/activelist");
			Rest.RegisterRedirect("/users/create", "/v2/users/create");
			Rest.RegisterRedirect("/users/list", "/v2/users/list");
			Rest.RegisterRedirect("/users/read", "/v2/users/read");
			Rest.RegisterRedirect("/users/destroy", "/v2/users/destroy");
			Rest.RegisterRedirect("/users/update", "/v2/users/update");

			//ban commands
			Rest.RegisterRedirect("/bans/create", "/v3/bans/create");
			Rest.RegisterRedirect("/bans/list", "/v3/bans/list");
			Rest.RegisterRedirect("/bans/read", "/v3/bans/read");
			Rest.RegisterRedirect("/bans/destroy", "/v3/bans/destroy");
			Rest.RegisterRedirect("/v2/bans/list", "/v3/bans/list");
			Rest.RegisterRedirect("/v2/bans/read", "/v3/bans/read");
			Rest.RegisterRedirect("/v2/bans/destroy", "/v3/bans/destroy");

			//world commands
			Rest.RegisterRedirect("/world/bloodmoon", "v3/world/bloodmoon");
			Rest.RegisterRedirect("/world/save", "/v2/world/save");
			Rest.RegisterRedirect("/world/autosave", "/v3/world/autosave");

			//player commands
			Rest.RegisterRedirect("/lists/players", "/lists/players", "/v2/players/list");
			Rest.RegisterRedirect("/players/list", "/v2/players/list");
			Rest.RegisterRedirect("/players/read", "/v3/players/read", "v4/players/read");
			Rest.RegisterRedirect("/players/kick", "/v2/players/kick");
			Rest.RegisterRedirect("/players/ban", "/v2/players/ban");
			Rest.RegisterRedirect("/players/kill", "/v2/players/kill");
			Rest.RegisterRedirect("/players/mute", "/v2/players/mute");
			Rest.RegisterRedirect("/players/unmute", "/v2/players/unmute");

			//group commands
			Rest.RegisterRedirect("/groups/list", "/v2/groups/list");
			Rest.RegisterRedirect("/groups/read", "/v2/groups/read");
			Rest.RegisterRedirect("/groups/destroy", "/v2/groups/destroy");
			Rest.RegisterRedirect("/groups/create", "/v2/groups/create");
			Rest.RegisterRedirect("/groups/update", "/v2/groups/update");


			Rest.Register(new SecureRestCommand("/v2/server/broadcast", ServerBroadcast, RestPermissions.restbroadcast));
			Rest.Register(new SecureRestCommand("/v3/server/reload", ServerReload, RestPermissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/server/off", ServerOff, RestPermissions.restmaintenance));
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
			Rest.Register(new SecureRestCommand("/v3/bans/create", BanCreateV3, RestPermissions.restban, RestPermissions.restmanagebans));
			Rest.Register(new SecureRestCommand("/v3/bans/list", BanListV3, RestPermissions.restviewbans));
			Rest.Register(new SecureRestCommand("/v3/bans/read", BanInfoV3, RestPermissions.restviewbans));
			Rest.Register(new SecureRestCommand("/v3/bans/destroy", BanDestroyV3, RestPermissions.restmanagebans));

			// World Commands
			Rest.Register(new SecureRestCommand("/world/read", WorldRead));
			Rest.Register(new SecureRestCommand("/world/meteor", WorldMeteor, RestPermissions.restcauseevents));
			Rest.Register(new SecureRestCommand("/world/bloodmoon/{bloodmoon}", WorldBloodmoon, RestPermissions.restcauseevents));
			Rest.Register(new SecureRestCommand("/v3/world/bloodmoon", WorldBloodmoonV3, RestPermissions.restcauseevents));
			Rest.Register(new SecureRestCommand("/v2/world/save", WorldSave, RestPermissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/world/autosave/state/{state}", WorldChangeSaveSettings, RestPermissions.restcfg));
			Rest.Register(new SecureRestCommand("/v3/world/autosave", WorldChangeSaveSettingsV3, RestPermissions.restcfg));
			Rest.Register(new SecureRestCommand("/v2/world/butcher", WorldButcher, RestPermissions.restbutcher));

			// Player Commands
			Rest.Register(new SecureRestCommand("/lists/players", PlayerList));
			Rest.Register(new SecureRestCommand("/v2/players/list", PlayerListV2));
			Rest.Register(new SecureRestCommand("/v3/players/read", PlayerReadV3, RestPermissions.restuserinfo));
			Rest.Register(new SecureRestCommand("/v4/players/read", PlayerReadV4, RestPermissions.restuserinfo));
			Rest.Register(new SecureRestCommand("/v2/players/kick", PlayerKickV2, RestPermissions.restkick));
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

		#region Rest Server Methods

		[Description("Executes a remote command on the server, and returns the output of the command.")]
		[RouteAttribute("/v3/server/rawcmd")]
		[Permission(RestPermissions.restrawcommand)]
		[Noun("cmd", true, "The command and arguments to execute.", typeof(String))]
		[Token]
		private object ServerCommandV3(RestRequestArgs args)
		{
			if (string.IsNullOrWhiteSpace(args.Parameters["cmd"]))
				return RestMissingParam("cmd");

			Group restPlayerGroup = TShock.Groups.GetGroupByName(args.TokenData.UserGroupName);

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

		[Description("Reload config files for the server.")]
		[Route("/v3/server/reload")]
		[Permission(RestPermissions.restcfg)]
		[Token]
		private object ServerReload(RestRequestArgs args)
		{
			TShock.Utils.Reload();
			Hooks.GeneralHooks.OnReloadEvent(new TSRestPlayer(args.TokenData.Username, TShock.Groups.GetGroupByName(args.TokenData.UserGroupName)));

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

		[Description("Returns the motd, if it exists.")]
		[Route("/v3/server/motd")]
		[Token]
		private object ServerMotd(RestRequestArgs args)
		{
			string motdFilePath = FileTools.MotdPath;
			if (!File.Exists(motdFilePath))
				return this.RestError("The motd.txt was not found.", "500");

			return new RestObject()
			{
				{"motd", File.ReadAllLines(motdFilePath)}
			};
		}

		[Description("Returns the rules, if they exist.")]
		[Route("/v3/server/rules")]
		[Token]
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

		[Description("Get a list of information about the current TShock server.")]
		[Route("/v2/server/status")]
		[Token]
		private object ServerStatusV2(RestRequestArgs args)
		{
			var ret = new RestObject()
			{
				{"name", TShock.Config.Settings.ServerName},
				{"serverversion", Main.versionNumber},
				{"tshockversion", TShock.VersionNum},
				{"port", TShock.Config.Settings.ServerPort},
				{"playercount", TShock.Utils.GetActivePlayerCount()},
				{"maxplayers", TShock.Config.Settings.MaxSlots},
				{"world", (TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName)},
				{"uptime", (DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime).ToString(@"d'.'hh':'mm':'ss")},
				{"serverpassword", !string.IsNullOrEmpty(TShock.Config.Settings.ServerPassword)}
			};

			if (GetBool(args.Parameters["players"], false))
			{
				var players = new ArrayList();
				foreach (TSPlayer tsPlayer in TShock.Players.Where(p => null != p))
				{
					var p = PlayerFilter(tsPlayer, args.Parameters, (!string.IsNullOrEmpty(args.TokenData.UserGroupName) && TShock.Groups.GetGroupByName(args.TokenData.UserGroupName).HasPermission(RestPermissions.viewips)));
					if (null != p)
						players.Add(p);
				}
				ret.Add("players", players);
			}

			if (GetBool(args.Parameters["rules"], false))
			{
				var rules = new Dictionary<string, object>();
				rules.Add("AutoSave", TShock.Config.Settings.AutoSave);
				rules.Add("DisableBuild", TShock.Config.Settings.DisableBuild);
				rules.Add("DisableClownBombs", TShock.Config.Settings.DisableClownBombs);
				rules.Add("DisableDungeonGuardian", TShock.Config.Settings.DisableDungeonGuardian);
				rules.Add("DisableInvisPvP", TShock.Config.Settings.DisableInvisPvP);
				rules.Add("DisableSnowBalls", TShock.Config.Settings.DisableSnowBalls);
				rules.Add("DisableTombstones", TShock.Config.Settings.DisableTombstones);
				rules.Add("EnableWhitelist", TShock.Config.Settings.EnableWhitelist);
				rules.Add("HardcoreOnly", TShock.Config.Settings.HardcoreOnly);
				rules.Add("PvPMode", TShock.Config.Settings.PvPMode);
				rules.Add("SpawnProtection", TShock.Config.Settings.SpawnProtection);
				rules.Add("SpawnProtectionRadius", TShock.Config.Settings.SpawnProtectionRadius);
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

		#region Rest User Methods

		[Description("Returns the list of user accounts that are currently in use on the server.")]
		[Route("/v2/users/activelist")]
		[Permission(RestPermissions.restviewusers)]
		[Token]
		private object UserActiveListV2(RestRequestArgs args)
		{
			return new RestObject() { { "activeusers", string.Join("\t", TShock.Players.Where(p => null != p && null != p.Account && p.Active).Select(p => p.Account.Name)) } };
		}

		[Description("Lists all user accounts in the TShock database.")]
		[Route("/v2/users/list")]
		[Permission(RestPermissions.restviewusers)]
		[Token]
		private object UserListV2(RestRequestArgs args)
		{
			return new RestObject() { { "users", TShock.UserAccounts.GetUserAccounts().Select(p => new Dictionary<string,object>(){
				{"name", p.Name},
				{"id", p.ID},
				{"group", p.Group},
			}) } };
		}

		[Description("Create a new TShock user account.")]
		[Route("/v2/users/create")]
		[Permission(RestPermissions.restmanageusers)]
		[Noun("user", true, "The user account name for the new account.", typeof(String))]
		[Noun("group", false, "The group the new account should be assigned.", typeof(String))]
		[Noun("password", true, "The password for the new account.", typeof(String))]
		[Token]
		private object UserCreateV2(RestRequestArgs args)
		{
			var username = args.Parameters["user"];
			if (string.IsNullOrWhiteSpace(username))
				return RestMissingParam("user");

			var group = args.Parameters["group"];
			if (string.IsNullOrWhiteSpace(group))
				group = TShock.Config.Settings.DefaultRegistrationGroupName;

			var password = args.Parameters["password"];
			if (string.IsNullOrWhiteSpace(password))
				return RestMissingParam("password");

			// NOTE: ip can be blank
			UserAccount account = new UserAccount(username, "", "", group, "", "", "");
			try
			{
				account.CreateBCryptHash(password);
				TShock.UserAccounts.AddUserAccount(account);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("User was successfully created");
		}

		[Description("Update a users information.")]
		[Route("/v2/users/update")]
		[Permission(RestPermissions.restmanageusers)]
		[Noun("user", true, "The search criteria (name or id of account to lookup).", typeof(String))]
		[Noun("type", true, "The search criteria type (name for name lookup, id for id lookup).", typeof(String))]
		[Noun("password", false, "The users new password, and at least this or group must be defined.", typeof(String))]
		[Noun("group", false, "The new group for the user, at least this or password must be defined.", typeof(String))]
		[Token]
		private object UserUpdateV2(RestRequestArgs args)
		{
			var ret = UserFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			var password = args.Parameters["password"];
			var group = args.Parameters["group"];
			if (string.IsNullOrWhiteSpace(group) && string.IsNullOrWhiteSpace(password))
				return RestMissingParam("group", "password");

			UserAccount account = (UserAccount)ret;
			var response = new RestObject();
			if (!string.IsNullOrWhiteSpace(password))
			{
				try
				{
					TShock.UserAccounts.SetUserAccountPassword(account, password);
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
					TShock.UserAccounts.SetUserGroup(account, group);
					response.Add("group-response", "Group updated successfully");
				}
				catch (Exception e)
				{
					return RestError("Failed to update user group (" + e.Message + ")");
				}
			}

			return response;
		}

		[Description("Destroy a TShock user account.")]
		[Route("/v2/users/destroy")]
		[Permission(RestPermissions.restmanageusers)]
		[Noun("user", true, "The search criteria (name or id of account to lookup).", typeof(String))]
		[Noun("type", true, "The search criteria type (name for name lookup, id for id lookup).", typeof(String))]
		[Token]
		private object UserDestroyV2(RestRequestArgs args)
		{
			var ret = UserFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			try
			{
				TShock.UserAccounts.RemoveUserAccount((UserAccount)ret);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse("User deleted successfully");
		}

		[Description("List detailed information for a user account.")]
		[Route("/v2/users/read")]
		[Permission(RestPermissions.restviewusers)]
		[Noun("user", true, "The search criteria (name or id of account to lookup).", typeof(String))]
		[Noun("type", true, "The search criteria type (name for name lookup, id for id lookup).", typeof(String))]
		[Token]
		private object UserInfoV2(RestRequestArgs args)
		{
			var ret = UserFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			UserAccount account = (UserAccount)ret;
			return new RestObject() { { "group", account.Group }, { "id", account.ID.ToString() }, { "name", account.Name } };
		}

		#endregion

		#region Rest Ban Methods

		[Description("Create a new ban entry.")]
		[Route("/v3/bans/create")]
		[Permission(RestPermissions.restmanagebans)]
		[Noun("identifier", true, "The identifier to ban.", typeof(String))]
		[Noun("reason", false, "The reason to assign to the ban.", typeof(String))]
		[Noun("start", false, "The datetime at which the ban should start.", typeof(String))]
		[Noun("end", false, "The datetime at which the ban should end.", typeof(String))]
		[Token]
		private object BanCreateV3(RestRequestArgs args)
		{
			string identifier = args.Parameters["identifier"];
			if (string.IsNullOrWhiteSpace(identifier))
				return RestMissingParam("identifier");

			string reason = args.Parameters["reason"];
			if (string.IsNullOrWhiteSpace(reason))
				reason = "Banned";

			if (!DateTime.TryParse(args.Parameters["start"], out DateTime startDate))
				startDate = DateTime.UtcNow;

			if (!DateTime.TryParse(args.Parameters["end"], out DateTime endDate))
				endDate = DateTime.MaxValue;

			AddBanResult banResult = TShock.Bans.InsertBan(identifier, reason, args.TokenData.Username, startDate, endDate);
			if (banResult.Ban != null)
			{
				TSPlayer player = null;
				if (identifier.StartsWith(Identifier.IP.Prefix))
				{
					player = TShock.Players.FirstOrDefault(p => p.IP == identifier.Substring(Identifier.IP.Prefix.Length));
				}
				else if (identifier.StartsWith(Identifier.Name.Prefix))
				{
					//Character names may not necessarily be unique, so kick all matches
					foreach (var ply in TShock.Players.Where(p => p.Name == identifier.Substring(Identifier.Name.Prefix.Length)))
					{
						ply.Kick(reason, true);
					}
				}
				else if (identifier.StartsWith(Identifier.Account.Prefix))
				{
					player = TShock.Players.FirstOrDefault(p => p.Account?.Name == identifier.Substring(Identifier.Account.Prefix.Length));
				}
				else if (identifier.StartsWith(Identifier.UUID.Prefix))
				{
					player = TShock.Players.FirstOrDefault(p => p.UUID == identifier.Substring(Identifier.UUID.Prefix.Length));
				}

				if (player != null)
				{
					player.Kick(reason, true);
				}

				return RestResponse(GetString($"Ban added. Ticket number: {banResult.Ban.TicketNumber}"));
			}

			return RestError(GetString($"Failed to add ban. {banResult.Message}"), status: "500");
		}

		[Description("Delete an existing ban entry.")]
		[Route("/v3/bans/destroy")]
		[Permission(RestPermissions.restmanagebans)]
		[Noun("ticketNumber", true, "The ticket number of the ban to delete.", typeof(String))]
		[Noun("fullDelete", false, "Whether or not to completely remove the ban from the system.", typeof(bool))]
		[Token]
		private object BanDestroyV3(RestRequestArgs args)
		{
			string id = args.Parameters["ticketNumber"];
			if (string.IsNullOrWhiteSpace(id))
				return RestMissingParam("ticketNumber");

			if (!int.TryParse(id, out int ticketNumber))
			{
				return RestInvalidParam("ticketNumber");
			}

			bool.TryParse(args.Parameters["fullDelete"], out bool fullDelete);

			if (TShock.Bans.RemoveBan(ticketNumber, fullDelete))
			{
				return RestResponse(GetString("Ban removed."));
			}

			return RestError(GetString("Failed to remove ban."), status: "500");
		}

		[Description("View the details of a specific ban.")]
		[Route("/v3/bans/read")]
		[Permission(RestPermissions.restviewbans)]
		[Noun("ticketNumber", true, "The ticket number to search for.", typeof(String))]
		[Token]
		private object BanInfoV3(RestRequestArgs args)
		{
			string id = args.Parameters["ticketNumber"];
			if (string.IsNullOrWhiteSpace(id))
				return RestMissingParam("ticketNumber");

			if (!int.TryParse(id, out int ticketNumber))
			{
				return RestInvalidParam("ticketNumber");
			}

			Ban ban = TShock.Bans.GetBanById(ticketNumber);

			if (ban == null)
			{
				return RestResponse(GetString("No matching bans found."));
			}

			return new RestObject
			{
				{ "ticket_number", ban.TicketNumber },
				{ "identifier", ban.Identifier },
				{ "reason", ban.Reason },
				{ "banning_user", ban.BanningUser },
				{ "start_date_ticks", ban.BanDateTime.Ticks },
				{ "end_date_ticks", ban.ExpirationDateTime.Ticks },
			};
		}

		[Description("View all bans in the TShock database.")]
		[Route("/v3/bans/list")]
		[Permission(RestPermissions.restviewbans)]
		[Token]
		private object BanListV3(RestRequestArgs args)
		{
			IEnumerable<Ban> bans = TShock.Bans.Bans.Select(kvp => kvp.Value);

			var banList = new ArrayList();
			foreach (var ban in bans)
			{
				banList.Add(
					new Dictionary<string, object>
					{
						{ "ticket_number", ban.TicketNumber },
						{ "identifier", ban.Identifier },
						{ "reason", ban.Reason },
						{ "banning_user", ban.BanningUser },
						{ "start_date_ticks", ban.BanDateTime.Ticks },
						{ "end_date_ticks", ban.ExpirationDateTime.Ticks },
					}
				);
			}

			return new RestObject
			{
				{ "bans", banList }
			};
		}

		#endregion

		#region Rest World Methods

		[Route("/v2/world/autosave/state/{state}")]
		[Permission(RestPermissions.restcfg)]
		[Verb("state", "The status for autosave.", typeof(bool))]
		[Token]
		private object WorldChangeSaveSettings(RestRequestArgs args)
		{
			bool autoSave;
			if (!bool.TryParse(args.Verbs["state"], out autoSave))
				return RestInvalidParam("state");
			TShock.Config.Settings.AutoSave = autoSave;

			var resp = RestResponse($"AutoSave has been set to {autoSave}");
			resp.Add("upgrade", "/v3/world/autosave");
			return resp;
		}

		[Route("/v3/world/autosave")]
		[Permission(RestPermissions.restcfg)]
		[Parameter("state", false, "The status for autosave.", typeof(bool))]
		[Token]
		private object WorldChangeSaveSettingsV3(RestRequestArgs args)
		{
			bool autoSave;
			if (!bool.TryParse(args.Parameters["state"], out autoSave))
			{
				if (TShock.Config.Settings.AutoSave)
				{
					return RestResponse(GetString($"Autosave is currently enabled"));
				}
				else
				{
					return RestResponse(GetString($"Autosave is currently disabled"));
				}
			}
			TShock.Config.Settings.AutoSave = autoSave;

			if (TShock.Config.Settings.AutoSave)
			{
				return RestResponse(GetString($"AutoSave has been enabled"));
			}
			else
			{
				return RestResponse(GetString($"AutoSave has been disabled"));
			}
		}

		[Description("Save the world.")]
		[Route("/v2/world/save")]
		[Permission(RestPermissions.restcfg)]
		[Token]
		private object WorldSave(RestRequestArgs args)
		{
			SaveManager.Instance.SaveWorld();

			return RestResponse("World saved");
		}

		[Description("Butcher npcs.")]
		[Route("/v2/world/butcher")]
		[Permission(RestPermissions.restbutcher)]
		[Noun("killfriendly", false, "Should friendly npcs be butchered.", typeof(bool))]
		[Token]
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

			return RestResponse(GetPluralString("{0} NPC has been killed.", "{0} NPCs have been killed.", killcount, killcount));
		}

		[Description("Get information regarding the world.")]
		[Route("/world/read")]
		[Token]
		private object WorldRead(RestRequestArgs args)
		{
			return new RestObject()
			{
				{"name", (TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName)},
				{"size", Main.maxTilesX + "*" + Main.maxTilesY},
				{"time", Main.time},
				{"daytime", Main.dayTime},
				{"bloodmoon", Main.bloodMoon},
				{"invasionsize", Main.invasionSize}
			};
		}

		[Description("Drops a meteor on the world.")]
		[Route("/world/meteor")]
		[Permission(RestPermissions.restcauseevents)]
		[Token]
		private object WorldMeteor(RestRequestArgs args)
		{
			WorldGen.spawnMeteor = false;
			WorldGen.dropMeteor();
			return RestResponse(GetString("Meteor has been spawned"));
		}

		[Description("Toggle the status of blood moon.")]
		[Route("/world/bloodmoon/{bloodmoon}")]
		[Permission(RestPermissions.restcauseevents)]
		[Verb("bloodmoon", "State of bloodmoon.", typeof(bool))]
		[Token]
		private object WorldBloodmoon(RestRequestArgs args)
		{
			bool bloodmoon;
			if (!bool.TryParse(args.Verbs["bloodmoon"], out bloodmoon))
				return RestInvalidParam("bloodmoon");
			Main.bloodMoon = bloodmoon;

			var resp = RestResponse(GetString($"Blood Moon has been set to {bloodmoon}"));
			resp.Add("upgrade", "/v3/world/bloodmoon");
			return resp;
		}

		[Description("Toggle the status of blood moon.")]
		[Route("/v3/world/bloodmoon")]
		[Permission(RestPermissions.restcauseevents)]
		[Parameter("state", false, "Sets the state of the bloodmoon.", typeof(bool))]
		[Token]
		private object WorldBloodmoonV3(RestRequestArgs args)
		{
			bool bloodmoon;
			if (!bool.TryParse(args.Verbs["state"], out bloodmoon))
			{
				return RestResponse(GetString($"Bloodmoon state: {(Main.bloodMoon ? "Enabled" : "Disabled")}"));
			}
			Main.bloodMoon = bloodmoon;

			if (Main.bloodMoon)
			{
				return RestResponse($"Blood Moon has been enabled");
			}
			else
			{
				return RestResponse($"Blood Moon has been disabled");
			}
		}

		#endregion

		#region Rest Player Methods

		[Description("Unmute a player.")]
		[Route("/v2/players/unmute")]
		[Permission(RestPermissions.restmute)]
		[Noun("player", true, "The player to mute.", typeof(String))]
		[Token]
		private object PlayerUnMute(RestRequestArgs args)
		{
			return PlayerSetMute(args.Parameters, false);
		}

		[Description("Mute a player.")]
		[Route("/v2/players/mute")]
		[Permission(RestPermissions.restmute)]
		[Noun("player", true, "The player to mute.", typeof(String))]
		[Token]
		private object PlayerMute(RestRequestArgs args)
		{
			return PlayerSetMute(args.Parameters, true);
		}

		[Description("List all player names that are currently on the server.")]
		[Route("/lists/players")]
		[Token]
		private object PlayerList(RestRequestArgs args)
		{
			var activeplayers = TShock.Players.Where(p => null != p && p.Active).Select(p => p.Name);
			return new RestObject() { { "players", string.Join(", ", activeplayers) } };
		}

		[Description("Fetches detailed user information on all connected users, and can be filtered by specifying a key value pair filter users where the key is a field and the value is a users field value.")]
		[Route("/v2/players/list")]
		[Token]
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

		[Description("Get information for a user.")]
		[Route("/v3/players/read")]
		[Permission(RestPermissions.restuserinfo)]
		[Noun("player", true, "The player to lookup", typeof(String))]
		[Token]
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
				{"username", player.Account?.Name},
				{"ip", player.IP},
				{"group", player.Group.Name},
				{"registered", player.Account?.Registered},
				{"muted", player.mute },
				{"position", player.TileX + "," + player.TileY},
				{"inventory", string.Join(", ", inventory.Select(p => (p.Name + ":" + p.stack)))},
				{"armor", string.Join(", ", equipment.Select(p => (p.netID + ":" + p.prefix)))},
				{"dyes", string.Join(", ", dyes.Select(p => (p.Name)))},
				{"buffs", string.Join(", ", player.TPlayer.buffType)}
			};
		}

		[Description("Get information for a user.")]
		[Route("/v4/players/read")]
		[Permission(RestPermissions.restuserinfo)]
		[Noun("player", true, "The player to lookup", typeof(String))]
		[Token]
		private object PlayerReadV4(RestRequestArgs args)
		{
			var ret = PlayerFind(args.Parameters);
			if (ret is RestObject)
			{
				return ret;
			}

			TSPlayer player = (TSPlayer)ret;

			object items = new
			{
				inventory = player.TPlayer.inventory.Where(i => i.active).Select(item => (NetItem)item),
				equipment = player.TPlayer.armor.Where(i => i.active).Select(item => (NetItem)item),
				dyes = player.TPlayer.dye.Where(i => i.active).Select(item => (NetItem)item),
				piggy = player.TPlayer.bank.item.Where(i => i.active).Select(item => (NetItem)item),
				safe = player.TPlayer.bank2.item.Where(i => i.active).Select(item => (NetItem)item),
				forge = player.TPlayer.bank3.item.Where(i => i.active).Select(item => (NetItem)item)
			};

			return new RestObject
			{
				{"nickname", player.Name},
				{"username", player.Account?.Name},
				{"ip", player.IP},
				{"group", player.Group.Name},
				{"registered", player.Account?.Registered},
				{"muted", player.mute },
				{"position", player.TileX + "," + player.TileY},
				{"items", items},
				{"buffs", string.Join(", ", player.TPlayer.buffType)}
			};
		}

		[Description("Kick a player off the server.")]
		[Route("/v2/players/kick")]
		[Permission(RestPermissions.restkick)]
		[Noun("player", true, "The player to kick.", typeof(String))]
		[Noun("reason", false, "The reason the player was kicked.", typeof(String))]
		[Token]
		private object PlayerKickV2(RestRequestArgs args)
		{
			var ret = PlayerFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			player.Kick(null == args.Parameters["reason"] ? GetString("Kicked via web") : args.Parameters["reason"], false, true, null, true);
			return RestResponse($"Player {player.Name} was kicked");
		}

		[Description("Kill a player.")]
		[Route("/v2/players/kill")]
		[Permission(RestPermissions.restkill)]
		[Noun("player", true, "The player to kick.", typeof(String))]
		[Noun("from", false, "Who killed the player.", typeof(String))]
		[Token]
		private object PlayerKill(RestRequestArgs args)
		{
			var ret = PlayerFind(args.Parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			player.DamagePlayer(999999);
			var from = string.IsNullOrWhiteSpace(args.Parameters["from"]) ? "Server Admin" : args.Parameters["from"];
			player.SendInfoMessage(GetString($"{from} just killed you!"));
			return RestResponse(GetString($"Player {player.Name} was killed"));
		}

		#endregion

		#region Rest Group Methods

		[Description("View all groups in the TShock database.")]
		[Route("/v2/groups/list")]
		[Permission(RestPermissions.restviewgroups)]
		[Token]
		private object GroupList(RestRequestArgs args)
		{
			var groups = new ArrayList();
			foreach (Group group in TShock.Groups)
			{
				groups.Add(new Dictionary<string, object> { { "name", group.Name }, { "parent", group.ParentName }, { "chatcolor", group.ChatColor } });
			}
			return new RestObject() { { "groups", groups } };
		}

		[Description("Display information of a group.")]
		[Route("/v2/groups/read")]
		[Permission(RestPermissions.restviewgroups)]
		[Noun("group", true, "The group name to get information on.", typeof(String))]
		[Token]
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

		[Description("Delete a group.")]
		[Route("/v2/groups/destroy")]
		[Permission(RestPermissions.restmanagegroups)]
		[Noun("group", true, "The group name to delete.", typeof(String))]
		[Token]
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

			return RestResponse(GetString($"Group {group.Name} deleted successfully"));
		}

		[Description("Create a new group.")]
		[Route("/v2/groups/create")]
		[Permission(RestPermissions.restmanagegroups)]
		[Noun("group", true, "The name of the new group.", typeof(String))]
		[Noun("parent", false, "The name of the parent group.", typeof(String))]
		[Noun("permissions", false, "A comma separated list of permissions for the new group.", typeof(String))]
		[Noun("chatcolor", false, "A r,g,b string representing the color for this groups chat.", typeof(String))]
		[Token]
		private object GroupCreate(RestRequestArgs args)
		{
			var name = args.Parameters["group"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("group");
			try
			{
				TShock.Groups.AddGroup(name, args.Parameters["parent"], args.Parameters["permissions"], args.Parameters["chatcolor"]);
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			return RestResponse(GetString($"Group {name} created successfully"));
		}

		[Route("/v2/groups/update")]
		[Permission(RestPermissions.restmanagegroups)]
		[Noun("group", true, "The name of the group to modify.", typeof(String))]
		[Noun("parent", false, "The name of the new parent for this group.", typeof(String))]
		[Noun("chatcolor", false, "The new chat color r,g,b.", typeof(String))]
		[Noun("permissions", false, "The new comma separated list of permissions.", typeof(String))]
		[Token]
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

			return RestResponse(GetString($"Group {group.Name} updated successfully"));
		}

		#endregion

		#region Utility Methods

		// TODO: figure out how to localise the route descriptions
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
					sb.AppendLine("## {0}".SFormat(name));
					sb.AppendLine("{0}".SFormat(descattr.Description));

					var permission = method.GetCustomAttributes(false).Where(o => o is Permission);
					if (permission.Count() > 0)
					{
						sb.AppendLine(GetString("* **Permissions**: `{0}`", String.Join(", ", permission.Select(p => ((Permission)p).Name))));
					}
					else
					{
						sb.AppendLine(GetString("No special permissions are required for this route."));
					}
					sb.AppendLine();
					var verbs = method.GetCustomAttributes(false).Where(o => o is Verb);
					if (verbs.Count() > 0)
					{
						sb.AppendLine(GetString("**Verbs**:"));
						foreach (Verb verb in verbs)
						{
							if (verb.Required)
								sb.AppendLine(GetString("* `{0}` (Required) `{1}` - {2}".SFormat(verb.Name, verb.ArgumentType.Name, verb.Description)));
							else
								sb.AppendLine(GetString("* `{0}` (Optional) `{1}` - {2}".SFormat(verb.Name, verb.ArgumentType.Name, verb.Description)));
						}
					}
					sb.AppendLine();
					var nouns = method.GetCustomAttributes(false).Where(o => o is Noun);
					if (nouns.Count() > 0)
					{
						sb.AppendLine(GetString("**Nouns**:"));
						foreach (Noun noun in nouns)
						{
							if (noun.Required)
								sb.AppendLine(GetString("* `{0}` (Required) `{1}` - {2}".SFormat(noun.Name, noun.ArgumentType.Name, noun.Description)));
							else
								sb.AppendLine(GetString("* `{0}` (Optional) `{1}` - {2}".SFormat(noun.Name, noun.ArgumentType.Name, noun.Description)));
						}
					}
					sb.AppendLine();
					sb.AppendLine(GetString("**Example Usage**: `{0}?{1}`", routeattr.Route,
						string.Join("&", nouns.Select(n => String.Format("{0}={0}", ((Noun)n).Name)))));
					sb.AppendLine();
				}
			}

			File.WriteAllText("docs/rest-fields.md", sb.ToString());
		}

		private RestObject RestError(string message, string status = "400")
		{
			return new RestObject(status) { Error = message };
		}

		private RestObject RestResponse(string message, string status = "200")
		{
			return new RestObject(status) { Response = message };
		}

		private RestObject RestMissingParam(string var)
		{
			return RestError(GetString($"Missing or empty {var} parameter"));
		}

		private RestObject RestMissingParam(params string[] vars)
		{
			return RestMissingParam(string.Join(", ", vars));
		}

		private RestObject RestInvalidParam(string var)
		{
			return RestError(GetString($"Missing or invalid {var} parameter"));
		}

		private bool GetBool(string val, bool def)
		{
			bool ret;
			return bool.TryParse(val, out ret) ? ret : def;
		}

		private object PlayerFind(EscapedParameterCollection parameters)
		{
			string name = parameters["player"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("player");

			var found = TSPlayer.FindByNameOrID(name);
			switch (found.Count)
			{
				case 1:
					return found[0];
				case 0:
					return RestError(GetString($"Player {name} was not found"));
				default:
					return RestError(GetPluralString($"Player {name} matches {found.Count} player", $"Player {name} matches {found.Count} players", found.Count));
			}
		}

		private object UserFind(EscapedParameterCollection parameters)
		{
			string name = parameters["user"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("user");

			UserAccount account;
			string type = parameters["type"];
			try
			{
				switch (type)
				{
					case null:
					case "name":
						type = "name";
						account = TShock.UserAccounts.GetUserAccountByName(name);
						break;
					case "id":
						account = TShock.UserAccounts.GetUserAccountByID(Convert.ToInt32(name));
						break;
					default:
						return RestError(GetString($"Invalid Type: '{type}'"));
				}
			}
			catch (Exception e)
			{
				return RestError(e.Message);
			}

			if (null == account)
				return RestError(GetString($"User {type} '{name}' doesn't exist"));

			return account;
		}

		private object GroupFind(EscapedParameterCollection parameters)
		{
			var name = parameters["group"];
			if (string.IsNullOrWhiteSpace(name))
				return RestMissingParam("group");

			var group = TShock.Groups.GetGroupByName(name);
			if (null == group)
				return RestError(GetString($"Group {name} doesn't exist"));

			return group;
		}

		private Dictionary<string, object> PlayerFilter(TSPlayer tsPlayer, EscapedParameterCollection parameters, bool viewips = false)
		{
			var player = new Dictionary<string, object>
				{
					{"nickname", tsPlayer.Name},
					{"username", tsPlayer.Account == null ? "" : tsPlayer.Account.Name},
					{"group", tsPlayer.Group.Name},
					{"active", tsPlayer.Active},
					{"state", tsPlayer.State},
					{"team", tsPlayer.Team},
				};

			if (viewips)
			{
				player.Add("ip", tsPlayer.IP);
			}
			foreach (EscapedParameter filter in parameters)
			{
				if (player.ContainsKey(filter.Name) && !player[filter.Name].Equals(filter.Value))
					return null;
			}
			return player;
		}

		private object PlayerSetMute(EscapedParameterCollection parameters, bool mute)
		{
			var ret = PlayerFind(parameters);
			if (ret is RestObject)
				return ret;

			TSPlayer player = (TSPlayer)ret;
			player.mute = mute;
			if (mute)
			{
				player.SendInfoMessage(GetString("You have been remotely muted"));
				return RestResponse(GetString($"Player {player.Name} has been muted"));
			}
			else
			{
				player.SendInfoMessage(GetString("You have been remotely unmmuted"));
				return RestResponse(GetString($"Player {player.Name} has been unmuted"));
			}
		}

		#endregion
	}
}
