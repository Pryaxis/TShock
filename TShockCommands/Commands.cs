//@@ -1,6618 + 0,0 @@
//﻿/*
//TShock, a server mod for Terraria
//Copyright (C) 2011-2019 Pryaxis & TShock Contributors

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.
//*/

//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using Terraria;
//using Terraria.ID;
//using Terraria.Localization;
//using TShockAPI.DB;
//using TerrariaApi.Server;
//using TShockAPI.Hooks;
//using Terraria.GameContent.Events;
//using Microsoft.Xna.Framework;
//using TShockAPI.Localization;
//using System.Text.RegularExpressions;
//using Terraria.DataStructures;
//using Terraria.GameContent.Creative;

//namespace TShockAPI
//{

//	public class Command
//	{
//		/// <summary>
//		/// Gets or sets whether to allow non-players to use this command.
//		/// </summary>
//		public bool AllowServer { get; set; }
//		/// <summary>
//		/// Gets or sets whether to do logging of this command.
//		/// </summary>
//		public bool DoLog { get; set; }
//		/// <summary>
//		/// Gets or sets the help text of this command.
//		/// </summary>
//		public string HelpText { get; set; }
//		/// <summary>
//		/// Gets or sets an extended description of this command.
//		/// </summary>
//		public string[] HelpDesc { get; set; }
//		/// <summary>
//		/// Gets the name of the command.
//		/// </summary>
//		public string Name { get { return Names[0]; } }
//		/// <summary>
//		/// Gets the names of the command.
//		/// </summary>
//		public List<string> Names { get; protected set; }
//		/// <summary>
//		/// Gets the permissions of the command.
//		/// </summary>
//		public List<string> Permissions { get; protected set; }

//		private CommandDelegate commandDelegate;
//		public CommandDelegate CommandDelegate
//		{
//			get { return commandDelegate; }
//			set
//			{
//				if (value == null)
//					throw new ArgumentNullException();

//				commandDelegate = value;
//			}
//		}

//		public Command(List<string> permissions, CommandDelegate cmd, params string[] names)
//			: this(cmd, names)
//		{
//			Permissions = permissions;
//		}

//		public Command(string permissions, CommandDelegate cmd, params string[] names)
//			: this(cmd, names)
//		{
//			Permissions = new List<string> { permissions };
//		}

//		public Command(CommandDelegate cmd, params string[] names)
//		{
//			if (cmd == null)
//				throw new ArgumentNullException("cmd");
//			if (names == null || names.Length < 1)
//				throw new ArgumentException("names");

//			AllowServer = true;
//			CommandDelegate = cmd;
//			DoLog = true;
//			HelpText = "No help available.";
//			HelpDesc = null;
//			Names = new List<string>(names);
//			Permissions = new List<string>();
//		}

//		public bool Run(string msg, bool silent, TSPlayer ply, List<string> parms)
//		{
//			if (!CanRun(ply))
//				return false;

//			try
//			{
//				CommandDelegate(new CommandArgs(msg, silent, ply, parms));
//			}
//			catch (Exception e)
//			{
//				ply.SendErrorMessage("Command failed, check logs for more details.");
//				TShock.Log.Error(e.ToString());
//			}

//			return true;
//		}

//		public bool Run(string msg, TSPlayer ply, List<string> parms)
//		{
//			return Run(msg, false, ply, parms);
//		}

//		public bool HasAlias(string name)
//		{
//			return Names.Contains(name);
//		}

//		public bool CanRun(TSPlayer ply)
//		{
//			if (Permissions == null || Permissions.Count < 1)
//				return true;
//			foreach (var Permission in Permissions)
//			{
//				if (ply.HasPermission(Permission))
//					return true;
//			}
//			return false;
//		}
//	}

//	public static class Commands
//	{
//		public static List<Command> ChatCommands = new List<Command>();
//		public static ReadOnlyCollection<Command> TShockCommands = new ReadOnlyCollection<Command>(new List<Command>());

//		/// <summary>
//		/// The command specifier, defaults to "/"
//		/// </summary>
//		public static string Specifier
//		{
//			get { return string.IsNullOrWhiteSpace(TShock.Config.Settings.CommandSpecifier) ? "/" : TShock.Config.Settings.CommandSpecifier; }
//		}

//		/// <summary>
//		/// The silent command specifier, defaults to "."
//		/// </summary>
//		public static string SilentSpecifier
//		{
//			get { return string.IsNullOrWhiteSpace(TShock.Config.Settings.CommandSilentSpecifier) ? "." : TShock.Config.Settings.CommandSilentSpecifier; }
//		}

//		private delegate void AddChatCommand(string permission, CommandDelegate command, params string[] names);

//		public static void InitCommands()
//		{
//			List<Command> tshockCommands = new List<Command>(100);
//			Action<Command> add = (cmd) =>
//			{
//				tshockCommands.Add(cmd);
//				ChatCommands.Add(cmd);
//			};

//			add(new Command(SetupToken, "setup")
//			{
//				AllowServer = false,
//				HelpText = "Used to authenticate as superadmin when first setting up TShock."
//			});
//			add(new Command(Permissions.user, ManageUsers, "user")
//			{
//				DoLog = false,
//				HelpText = "Manages user accounts."
//			});

//			#region Account Commands
//			add(new Command(Permissions.canlogin, AttemptLogin, "login")
//			{
//				AllowServer = false,
//				DoLog = false,
//				HelpText = "Logs you into an account."
//			});
//			add(new Command(Permissions.canlogout, Logout, "logout")
//			{
//				AllowServer = false,
//				DoLog = false,
//				HelpText = "Logs you out of your current account."
//			});
//			add(new Command(Permissions.canchangepassword, PasswordUser, "password")
//			{
//				AllowServer = false,
//				DoLog = false,
//				HelpText = "Changes your account's password."
//			});
//			add(new Command(Permissions.canregister, RegisterUser, "register")
//			{
//				AllowServer = false,
//				DoLog = false,
//				HelpText = "Registers you an account."
//			});
//			add(new Command(Permissions.checkaccountinfo, ViewAccountInfo, "accountinfo", "ai")
//			{
//				HelpText = "Shows information about a user."
//			});
//			#endregion
//			#region Admin Commands
//			add(new Command(Permissions.ban, Ban, "ban")
//			{
//				HelpText = "Manages player bans."
//			});
//			add(new Command(Permissions.broadcast, Broadcast, "broadcast", "bc", "say")
//			{
//				HelpText = "Broadcasts a message to everyone on the server."
//			});
//			add(new Command(Permissions.logs, DisplayLogs, "displaylogs")
//			{
//				HelpText = "Toggles whether you receive server logs."
//			});
//			add(new Command(Permissions.managegroup, Group, "group")
//			{
//				HelpText = "Manages groups."
//			});
//			add(new Command(Permissions.manageitem, ItemBan, "itemban")
//			{
//				HelpText = "Manages item bans."
//			});
//			add(new Command(Permissions.manageprojectile, ProjectileBan, "projban")
//			{
//				HelpText = "Manages projectile bans."
//			});
//			add(new Command(Permissions.managetile, TileBan, "tileban")
//			{
//				HelpText = "Manages tile bans."
//			});
//			add(new Command(Permissions.manageregion, Region, "region")
//			{
//				HelpText = "Manages regions."
//			});
//			add(new Command(Permissions.kick, Kick, "kick")
//			{
//				HelpText = "Removes a player from the server."
//			});
//			add(new Command(Permissions.mute, Mute, "mute", "unmute")
//			{
//				HelpText = "Prevents a player from talking."
//			});
//			add(new Command(Permissions.savessc, OverrideSSC, "overridessc", "ossc")
//			{
//				HelpText = "Overrides serverside characters for a player, temporarily."
//			});
//			add(new Command(Permissions.savessc, SaveSSC, "savessc")
//			{
//				HelpText = "Saves all serverside characters."
//			});
//			add(new Command(Permissions.uploaddata, UploadJoinData, "uploadssc")
//			{
//				HelpText = "Upload the account information when you joined the server as your Server Side Character data."
//			});
//			add(new Command(Permissions.settempgroup, TempGroup, "tempgroup")
//			{
//				HelpText = "Temporarily sets another player's group."
//			});
//			add(new Command(Permissions.su, SubstituteUser, "su")
//			{
//				HelpText = "Temporarily elevates you to Super Admin."
//			});
//			add(new Command(Permissions.su, SubstituteUserDo, "sudo")
//			{
//				HelpText = "Executes a command as the super admin."
//			});
//			add(new Command(Permissions.userinfo, GrabUserUserInfo, "userinfo", "ui")
//			{
//				HelpText = "Shows information about a player."
//			});
//			#endregion
//			#region Annoy Commands
//			add(new Command(Permissions.annoy, Annoy, "annoy")
//			{
//				HelpText = "Annoys a player for an amount of time."
//			});
//			add(new Command(Permissions.annoy, Rocket, "rocket")
//			{
//				HelpText = "Rockets a player upwards. Requires SSC."
//			});
//			add(new Command(Permissions.annoy, FireWork, "firework")
//			{
//				HelpText = "Spawns fireworks at a player."
//			});
//			#endregion
//			#region Configuration Commands
//			add(new Command(Permissions.maintenance, CheckUpdates, "checkupdates")
//			{
//				HelpText = "Checks for TShock updates."
//			});
//			add(new Command(Permissions.maintenance, Off, "off", "exit", "stop")
//			{
//				HelpText = "Shuts down the server while saving."
//			});
//			add(new Command(Permissions.maintenance, OffNoSave, "off-nosave", "exit-nosave", "stop-nosave")
//			{
//				HelpText = "Shuts down the server without saving."
//			});
//			add(new Command(Permissions.cfgreload, Reload, "reload")
//			{
//				HelpText = "Reloads the server configuration file."
//			});
//			add(new Command(Permissions.cfgpassword, ServerPassword, "serverpassword")
//			{
//				HelpText = "Changes the server password."
//			});
//			add(new Command(Permissions.maintenance, GetVersion, "version")
//			{
//				HelpText = "Shows the TShock version."
//			});
//			add(new Command(Permissions.whitelist, Whitelist, "whitelist")
//			{
//				HelpText = "Manages the server whitelist."
//			});
//			#endregion
//			#region Item Commands
//			add(new Command(Permissions.give, Give, "give", "g")
//			{
//				HelpText = "Gives another player an item."
//			});
//			add(new Command(Permissions.item, Item, "item", "i")
//			{
//				AllowServer = false,
//				HelpText = "Gives yourself an item."
//			});
//			#endregion
//			#region NPC Commands
//			add(new Command(Permissions.butcher, Butcher, "butcher")
//			{
//				HelpText = "Kills hostile NPCs or NPCs of a certain type."
//			});
//			add(new Command(Permissions.renamenpc, RenameNPC, "renamenpc")
//			{
//				HelpText = "Renames an NPC."
//			});
//			add(new Command(Permissions.maxspawns, MaxSpawns, "maxspawns")
//			{
//				HelpText = "Sets the maximum number of NPCs."
//			});
//			add(new Command(Permissions.spawnboss, SpawnBoss, "spawnboss", "sb")
//			{
//				AllowServer = false,
//				HelpText = "Spawns a number of bosses around you."
//			});
//			add(new Command(Permissions.spawnmob, SpawnMob, "spawnmob", "sm")
//			{
//				AllowServer = false,
//				HelpText = "Spawns a number of mobs around you."
//			});
//			add(new Command(Permissions.spawnrate, SpawnRate, "spawnrate")
//			{
//				HelpText = "Sets the spawn rate of NPCs."
//			});
//			add(new Command(Permissions.clearangler, ClearAnglerQuests, "clearangler")
//			{
//				HelpText = "Resets the list of users who have completed an angler quest that day."
//			});
//			#endregion
//			#region TP Commands
//			add(new Command(Permissions.home, Home, "home")
//			{
//				AllowServer = false,
//				HelpText = "Sends you to your spawn point."
//			});
//			add(new Command(Permissions.spawn, Spawn, "spawn")
//			{
//				AllowServer = false,
//				HelpText = "Sends you to the world's spawn point."
//			});
//			add(new Command(Permissions.tp, TP, "tp")
//			{
//				AllowServer = false,
//				HelpText = "Teleports a player to another player."
//			});
//			add(new Command(Permissions.tpothers, TPHere, "tphere")
//			{
//				AllowServer = false,
//				HelpText = "Teleports a player to yourself."
//			});
//			add(new Command(Permissions.tpnpc, TPNpc, "tpnpc")
//			{
//				AllowServer = false,
//				HelpText = "Teleports you to an npc."
//			});
//			add(new Command(Permissions.tppos, TPPos, "tppos")
//			{
//				AllowServer = false,
//				HelpText = "Teleports you to tile coordinates."
//			});
//			add(new Command(Permissions.getpos, GetPos, "pos")
//			{
//				AllowServer = false,
//				HelpText = "Returns the user's or specified user's current position."
//			});
//			add(new Command(Permissions.tpallow, TPAllow, "tpallow")
//			{
//				AllowServer = false,
//				HelpText = "Toggles whether other people can teleport you."
//			});
//			#endregion
//			#region World Commands
//			add(new Command(Permissions.toggleexpert, ChangeWorldMode, "worldmode", "gamemode")
//			{
//				HelpText = "Changes the world mode."
//			});
//			add(new Command(Permissions.antibuild, ToggleAntiBuild, "antibuild")
//			{
//				HelpText = "Toggles build protection."
//			});
//			add(new Command(Permissions.grow, Grow, "grow")
//			{
//				AllowServer = false,
//				HelpText = "Grows plants at your location."
//			});
//			add(new Command(Permissions.halloween, ForceHalloween, "forcehalloween")
//			{
//				HelpText = "Toggles halloween mode (goodie bags, pumpkins, etc)."
//			});
//			add(new Command(Permissions.xmas, ForceXmas, "forcexmas")
//			{
//				HelpText = "Toggles christmas mode (present spawning, santa, etc)."
//			});
//			add(new Command(Permissions.manageevents, ManageWorldEvent, "worldevent")
//			{
//				HelpText = "Enables starting and stopping various world events."
//			});
//			add(new Command(Permissions.hardmode, Hardmode, "hardmode")
//			{
//				HelpText = "Toggles the world's hardmode status."
//			});
//			add(new Command(Permissions.editspawn, ProtectSpawn, "protectspawn")
//			{
//				HelpText = "Toggles spawn protection."
//			});
//			add(new Command(Permissions.worldsave, Save, "save")
//			{
//				HelpText = "Saves the world file."
//			});
//			add(new Command(Permissions.worldspawn, SetSpawn, "setspawn")
//			{
//				AllowServer = false,
//				HelpText = "Sets the world's spawn point to your location."
//			});
//			add(new Command(Permissions.dungeonposition, SetDungeon, "setdungeon")
//			{
//				AllowServer = false,
//				HelpText = "Sets the dungeon's position to your location."
//			});
//			add(new Command(Permissions.worldsettle, Settle, "settle")
//			{
//				HelpText = "Forces all liquids to update immediately."
//			});
//			add(new Command(Permissions.time, Time, "time")
//			{
//				HelpText = "Sets the world time."
//			});
//			add(new Command(Permissions.wind, Wind, "wind")
//			{
//				HelpText = "Changes the wind speed."
//			});
//			add(new Command(Permissions.worldinfo, WorldInfo, "worldinfo")
//			{
//				HelpText = "Shows information about the current world."
//			});
//			#endregion
//			#region Other Commands
//			add(new Command(Permissions.buff, Buff, "buff")
//			{
//				AllowServer = false,
//				HelpText = "Gives yourself a buff or debuff for an amount of time. Putting -1 for time will set it to 415 days."
//			});
//			add(new Command(Permissions.clear, Clear, "clear")
//			{
//				HelpText = "Clears item drops or projectiles."
//			});
//			add(new Command(Permissions.buffplayer, GBuff, "gbuff", "buffplayer")
//			{
//				HelpText = "Gives another player a buff or debuff for an amount of time. Putting -1 for time will set it to 415 days."
//			});
//			add(new Command(Permissions.godmode, ToggleGodMode, "godmode", "god")
//			{
//				HelpText = "Toggles godmode on a player."
//			});
//			add(new Command(Permissions.heal, Heal, "heal")
//			{
//				HelpText = "Heals a player in HP and MP."
//			});
//			add(new Command(Permissions.kill, Kill, "kill", "slay")
//			{
//				HelpText = "Kills another player."
//			});
//			add(new Command(Permissions.cantalkinthird, ThirdPerson, "me")
//			{
//				HelpText = "Sends an action message to everyone."
//			});
//			add(new Command(Permissions.canpartychat, PartyChat, "party", "p")
//			{
//				AllowServer = false,
//				HelpText = "Sends a message to everyone on your team."
//			});
//			add(new Command(Permissions.whisper, Reply, "reply", "r")
//			{
//				HelpText = "Replies to a PM sent to you."
//			});
//			add(new Command(Rests.RestPermissions.restmanage, ManageRest, "rest")
//			{
//				HelpText = "Manages the REST API."
//			});
//			add(new Command(Permissions.slap, Slap, "slap")
//			{
//				HelpText = "Slaps a player, dealing damage."
//			});
//			add(new Command(Permissions.serverinfo, ServerInfo, "serverinfo")
//			{
//				HelpText = "Shows the server information."
//			});
//			add(new Command(Permissions.warp, Warp, "warp")
//			{
//				HelpText = "Teleports you to a warp point or manages warps."
//			});
//			add(new Command(Permissions.whisper, Whisper, "whisper", "w", "tell", "pm", "dm")
//			{
//				HelpText = "Sends a PM to a player."
//			});
//			add(new Command(Permissions.whisper, Wallow, "wallow", "wa")
//			{
//				AllowServer = false,
//				HelpText = "Toggles to either ignore or recieve whispers from other players."
//			});
//			add(new Command(Permissions.createdumps, CreateDumps, "dump-reference-data")
//			{
//				HelpText = "Creates a reference tables for Terraria data types and the TShock permission system in the server folder."
//			});
//			add(new Command(Permissions.synclocalarea, SyncLocalArea, "sync")
//			{
//				HelpText = "Sends all tiles from the server to the player to resync the client with the actual world state."
//			});
//			add(new Command(Permissions.respawn, Respawn, "respawn")
//			{
//				HelpText = "Respawn yourself or another player."
//			});
//			#endregion

//			add(new Command(Aliases, "aliases")
//			{
//				HelpText = "Shows a command's aliases."
//			});
//			add(new Command(Help, "help")
//			{
//				HelpText = "Lists commands or gives help on them."
//			});
//			add(new Command(Motd, "motd")
//			{
//				HelpText = "Shows the message of the day."
//			});
//			add(new Command(ListConnectedPlayers, "playing", "online", "who")
//			{
//				HelpText = "Shows the currently connected players."
//			});
//			add(new Command(Rules, "rules")
//			{
//				HelpText = "Shows the server's rules."
//			});

//			TShockCommands = new ReadOnlyCollection<Command>(tshockCommands);
//		}

//		public static bool HandleCommand(TSPlayer player, string text)
//		{
//			string cmdText = text.Remove(0, 1);
//			string cmdPrefix = text[0].ToString();
//			bool silent = false;

//			if (cmdPrefix == SilentSpecifier)
//				silent = true;

//			int index = -1;
//			for (int i = 0; i < cmdText.Length; i++)
//			{
//				if (IsWhiteSpace(cmdText[i]))
//				{
//					index = i;
//					break;
//				}
//			}
//			string cmdName;
//			if (index == 0) // Space after the command specifier should not be supported
//			{
//				player.SendErrorMessage("Invalid command entered. Type {0}help for a list of valid commands.", Specifier);
//				return true;
//			}
//			else if (index < 0)
//				cmdName = cmdText.ToLower();
//			else
//				cmdName = cmdText.Substring(0, index).ToLower();

//			List<string> args;
//			if (index < 0)
//				args = new List<string>();
//			else
//				args = ParseParameters(cmdText.Substring(index));

//			IEnumerable<Command> cmds = ChatCommands.FindAll(c => c.HasAlias(cmdName));

//			if (Hooks.PlayerHooks.OnPlayerCommand(player, cmdName, cmdText, args, ref cmds, cmdPrefix))
//				return true;

//			if (cmds.Count() == 0)
//			{
//				if (player.AwaitingResponse.ContainsKey(cmdName))
//				{
//					Action<CommandArgs> call = player.AwaitingResponse[cmdName];
//					player.AwaitingResponse.Remove(cmdName);
//					call(new CommandArgs(cmdText, player, args));
//					return true;
//				}
//				player.SendErrorMessage("Invalid command entered. Type {0}help for a list of valid commands.", Specifier);
//				return true;
//			}
//			foreach (Command cmd in cmds)
//			{
//				if (!cmd.CanRun(player))
//				{
//					TShock.Utils.SendLogs(string.Format("{0} tried to execute {1}{2}.", player.Name, Specifier, cmdText), Color.PaleVioletRed, player);
//					player.SendErrorMessage("You do not have access to this command.");
//					if (player.HasPermission(Permissions.su))
//					{
//						player.SendInfoMessage("You can use '{0}sudo {0}{1}' to override this check.", Specifier, cmdText);
//					}
//				}
//				else if (!cmd.AllowServer && !player.RealPlayer)
//				{
//					player.SendErrorMessage("You must use this command in-game.");
//				}
//				else
//				{
//					if (cmd.DoLog)
//						TShock.Utils.SendLogs(string.Format("{0} executed: {1}{2}.", player.Name, silent ? SilentSpecifier : Specifier, cmdText), Color.PaleVioletRed, player);
//					cmd.Run(cmdText, silent, player, args);
//				}
//			}
//			return true;
//		}

//		/// <summary>
//		/// Parses a string of parameters into a list. Handles quotes.
//		/// </summary>
//		/// <param name="str"></param>
//		/// <returns></returns>
//		private static List<String> ParseParameters(string str)
//		{
//			var ret = new List<string>();
//			var sb = new StringBuilder();
//			bool instr = false;
//			for (int i = 0; i < str.Length; i++)
//			{
//				char c = str[i];

//				if (c == '\\' && ++i < str.Length)
//				{
//					if (str[i] != '"' && str[i] != ' ' && str[i] != '\\')
//						sb.Append('\\');
//					sb.Append(str[i]);
//				}
//				else if (c == '"')
//				{
//					instr = !instr;
//					if (!instr)
//					{
//						ret.Add(sb.ToString());
//						sb.Clear();
//					}
//					else if (sb.Length > 0)
//					{
//						ret.Add(sb.ToString());
//						sb.Clear();
//					}
//				}
//				else if (IsWhiteSpace(c) && !instr)
//				{
//					if (sb.Length > 0)
//					{
//						ret.Add(sb.ToString());
//						sb.Clear();
//					}
//				}
//				else
//					sb.Append(c);
//			}
//			if (sb.Length > 0)
//				ret.Add(sb.ToString());

//			return ret;
//		}

//		private static bool IsWhiteSpace(char c)
//		{
//			return c == ' ' || c == '\t' || c == '\n';
//		}

//		#region Account commands
 


//		#endregion

//		#region Stupid commands

//		#endregion

//		#region Player Management Commands

//		private static void GrabUserUserInfo(CommandArgs args)
//		{
//			if (args.Parameters.Count < 1)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}userinfo <player>", Specifier);
//				return;
//			}

//			var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
//			if (players.Count < 1)
//				args.Player.SendErrorMessage("Invalid player.");
//			else if (players.Count > 1)
//				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
//			else
//			{
//				var message = new StringBuilder();
//				message.Append("IP Address: ").Append(players[0].IP);
//				if (players[0].Account != null && players[0].IsLoggedIn)
//					message.Append(" | Logged in as: ").Append(players[0].Account.Name).Append(" | Group: ").Append(players[0].Group.Name);
//				args.Player.SendSuccessMessage(message.ToString());
//			}
//		}


//		private static void Kick(CommandArgs args)
//		{
//			if (args.Parameters.Count < 1)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}kick <player> [reason]", Specifier);
//				return;
//			}
//			if (args.Parameters[0].Length == 0)
//			{
//				args.Player.SendErrorMessage("Missing player name.");
//				return;
//			}

//			string plStr = args.Parameters[0];
//			var players = TSPlayer.FindByNameOrID(plStr);
//			if (players.Count == 0)
//			{
//				args.Player.SendErrorMessage("Invalid player!");
//			}
//			else if (players.Count > 1)
//			{
//				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
//			}
//			else
//			{
//				string reason = args.Parameters.Count > 1
//									? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1))
//									: "Misbehaviour.";
//				if (!players[0].Kick(reason, !args.Player.RealPlayer, false, args.Player.Name))
//				{
//					args.Player.SendErrorMessage("You can't kick another admin!");
//				}
//			}
//		}

//		private static void DisplayLogs(CommandArgs args)
//		{
//			args.Player.DisplayLogs = (!args.Player.DisplayLogs);
//			args.Player.SendSuccessMessage("You will " + (args.Player.DisplayLogs ? "now" : "no longer") + " receive logs.");
//		}

//		private static void SaveSSC(CommandArgs args)
//		{
//			if (Main.ServerSideCharacter)
//			{
//				args.Player.SendSuccessMessage("SSC has been saved.");
//				foreach (TSPlayer player in TShock.Players)
//				{
//					if (player != null && player.IsLoggedIn && !player.IsDisabledPendingTrashRemoval)
//					{
//						TShock.CharacterDB.InsertPlayerData(player, true);
//					}
//				}
//			}
//		}

//		private static void OverrideSSC(CommandArgs args)
//		{
//			if (!Main.ServerSideCharacter)
//			{
//				args.Player.SendErrorMessage("Server Side Characters is disabled.");
//				return;
//			}
//			if (args.Parameters.Count < 1)
//			{
//				args.Player.SendErrorMessage("Correct usage: {0}overridessc|{0}ossc <player name>", Specifier);
//				return;
//			}

//			string playerNameToMatch = string.Join(" ", args.Parameters);
//			var matchedPlayers = TSPlayer.FindByNameOrID(playerNameToMatch);
//			if (matchedPlayers.Count < 1)
//			{
//				args.Player.SendErrorMessage("No players matched \"{0}\".", playerNameToMatch);
//				return;
//			}
//			else if (matchedPlayers.Count > 1)
//			{
//				args.Player.SendMultipleMatchError(matchedPlayers.Select(p => p.Name));
//				return;
//			}

//			TSPlayer matchedPlayer = matchedPlayers[0];
//			if (matchedPlayer.IsLoggedIn)
//			{
//				args.Player.SendErrorMessage("Player \"{0}\" is already logged in.", matchedPlayer.Name);
//				return;
//			}
//			if (!matchedPlayer.LoginFailsBySsi)
//			{
//				args.Player.SendErrorMessage("Player \"{0}\" has to perform a /login attempt first.", matchedPlayer.Name);
//				return;
//			}
//			if (matchedPlayer.IsDisabledPendingTrashRemoval)
//			{
//				args.Player.SendErrorMessage("Player \"{0}\" has to reconnect first.", matchedPlayer.Name);
//				return;
//			}

//			TShock.CharacterDB.InsertPlayerData(matchedPlayer);
//			args.Player.SendSuccessMessage("SSC of player \"{0}\" has been overriden.", matchedPlayer.Name);
//		}

//		private static void UploadJoinData(CommandArgs args)
//		{
//			TSPlayer targetPlayer = args.Player;
//			if (args.Parameters.Count == 1 && args.Player.HasPermission(Permissions.uploadothersdata))
//			{
//				List<TSPlayer> players = TSPlayer.FindByNameOrID(args.Parameters[0]);
//				if (players.Count > 1)
//				{
//					args.Player.SendMultipleMatchError(players.Select(p => p.Name));
//					return;
//				}
//				else if (players.Count == 0)
//				{
//					args.Player.SendErrorMessage("No player was found matching'{0}'", args.Parameters[0]);
//					return;
//				}
//				else
//				{
//					targetPlayer = players[0];
//				}
//			}
//			else if (args.Parameters.Count == 1)
//			{
//				args.Player.SendErrorMessage("You do not have permission to upload another player's character data.");
//				return;
//			}
//			else if (args.Parameters.Count > 0)
//			{
//				args.Player.SendErrorMessage("Usage: /uploadssc [playername]");
//				return;
//			}
//			else if (args.Parameters.Count == 0 && args.Player is TSServerPlayer)
//			{
//				args.Player.SendErrorMessage("A console can not upload their player data.");
//				args.Player.SendErrorMessage("Usage: /uploadssc [playername]");
//				return;
//			}

//			if (targetPlayer.IsLoggedIn)
//			{
//				if (TShock.CharacterDB.InsertSpecificPlayerData(targetPlayer, targetPlayer.DataWhenJoined))
//				{
//					targetPlayer.DataWhenJoined.RestoreCharacter(targetPlayer);
//					targetPlayer.SendSuccessMessage("Your local character data has been uploaded to the server.");
//					args.Player.SendSuccessMessage("The player's character data was successfully uploaded.");
//				}
//				else
//				{
//					args.Player.SendErrorMessage("Failed to upload your character data, are you logged in to an account?");
//				}
//			}
//			else
//			{
//				args.Player.SendErrorMessage("The target player has not logged in yet.");
//			}
//		}

//		private static void TempGroup(CommandArgs args)
//		{
//			if (args.Parameters.Count < 2)
//			{
//				args.Player.SendInfoMessage("Invalid usage");
//				args.Player.SendInfoMessage("Usage: {0}tempgroup <username> <new group> [time]", Specifier);
//				return;
//			}

//			List<TSPlayer> ply = TSPlayer.FindByNameOrID(args.Parameters[0]);
//			if (ply.Count < 1)
//			{
//				args.Player.SendErrorMessage("Could not find player {0}.", args.Parameters[0]);
//				return;
//			}

//			if (ply.Count > 1)
//			{
//				args.Player.SendMultipleMatchError(ply.Select(p => p.Account.Name));
//			}

//			if (!TShock.Groups.GroupExists(args.Parameters[1]))
//			{
//				args.Player.SendErrorMessage("Could not find group {0}", args.Parameters[1]);
//				return;
//			}

//			if (args.Parameters.Count > 2)
//			{
//				int time;
//				if (!TShock.Utils.TryParseTime(args.Parameters[2], out time))
//				{
//					args.Player.SendErrorMessage("Invalid time string! Proper format: _d_h_m_s, with at least one time specifier.");
//					args.Player.SendErrorMessage("For example, 1d and 10h-30m+2m are both valid time strings, but 2 is not.");
//					return;
//				}

//				ply[0].tempGroupTimer = new System.Timers.Timer(time * 1000);
//				ply[0].tempGroupTimer.Elapsed += ply[0].TempGroupTimerElapsed;
//				ply[0].tempGroupTimer.Start();
//			}

//			Group g = TShock.Groups.GetGroupByName(args.Parameters[1]);

//			ply[0].tempGroup = g;

//			if (args.Parameters.Count < 3)
//			{
//				args.Player.SendSuccessMessage(String.Format("You have changed {0}'s group to {1}", ply[0].Name, g.Name));
//				ply[0].SendSuccessMessage(String.Format("Your group has temporarily been changed to {0}", g.Name));
//			}
//			else
//			{
//				args.Player.SendSuccessMessage(String.Format("You have changed {0}'s group to {1} for {2}",
//					ply[0].Name, g.Name, args.Parameters[2]));
//				ply[0].SendSuccessMessage(String.Format("Your group has been changed to {0} for {1}",
//					g.Name, args.Parameters[2]));
//			}
//		}

//		private static void SubstituteUser(CommandArgs args)
//		{

//			if (args.Player.tempGroup != null)
//			{
//				args.Player.tempGroup = null;
//				args.Player.tempGroupTimer.Stop();
//				args.Player.SendSuccessMessage("Your previous permission set has been restored.");
//				return;
//			}
//			else
//			{
//				args.Player.tempGroup = new SuperAdminGroup();
//				args.Player.tempGroupTimer = new System.Timers.Timer(600 * 1000);
//				args.Player.tempGroupTimer.Elapsed += args.Player.TempGroupTimerElapsed;
//				args.Player.tempGroupTimer.Start();
//				args.Player.SendSuccessMessage("Your account has been elevated to Super Admin for 10 minutes.");
//				return;
//			}
//		}

//		#endregion Player Management Commands

//		#region Server Maintenence Commands

//		// Executes a command as a superuser if you have sudo rights.
//		private static void SubstituteUserDo(CommandArgs args)
//		{
//			if (args.Parameters.Count == 0)
//			{
//				args.Player.SendErrorMessage("Usage: /sudo [command].");
//				args.Player.SendErrorMessage("Example: /sudo /ban add Shank 2d Hacking.");
//				return;
//			}

//			string replacementCommand = String.Join(" ", args.Parameters.Select(p => p.Contains(" ") ? $"\"{p}\"" : p));
//			args.Player.tempGroup = new SuperAdminGroup();
//			HandleCommand(args.Player, replacementCommand);
//			args.Player.tempGroup = null;
//			return;
//		}

//		private static void Broadcast(CommandArgs args)
//		{
//			string message = string.Join(" ", args.Parameters);

//			TShock.Utils.Broadcast(
//				"(Server Broadcast) " + message,
//				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[0]), Convert.ToByte(TShock.Config.Settings.BroadcastRGB[1]),
//				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[2]));
//		}



//		#endregion Server Maintenence Commands

//		#region Cause Events and Spawn Monsters Commands




//		#endregion Cause Events and Spawn Monsters Commands

//		#region Group Management

//		private static void Group(CommandArgs args)
//		{
//			string subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();

//			switch (subCmd)
//			{
//				case "add":
//					#region Add group
//					{
//						if (args.Parameters.Count < 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group add <group name> [permissions]", Specifier);
//							return;
//						}

//						string groupName = args.Parameters[1];
//						args.Parameters.RemoveRange(0, 2);
//						string permissions = String.Join(",", args.Parameters);

//						try
//						{
//							TShock.Groups.AddGroup(groupName, null, permissions, TShockAPI.Group.defaultChatColor);
//							args.Player.SendSuccessMessage("The group was added successfully!");
//						}
//						catch (GroupExistsException)
//						{
//							args.Player.SendErrorMessage("That group already exists!");
//						}
//						catch (GroupManagerException ex)
//						{
//							args.Player.SendErrorMessage(ex.ToString());
//						}
//					}
//					#endregion
//					return;
//				case "addperm":
//					#region Add permissions
//					{
//						if (args.Parameters.Count < 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group addperm <group name> <permissions...>", Specifier);
//							return;
//						}

//						string groupName = args.Parameters[1];
//						args.Parameters.RemoveRange(0, 2);
//						if (groupName == "*")
//						{
//							foreach (Group g in TShock.Groups)
//							{
//								TShock.Groups.AddPermissions(g.Name, args.Parameters);
//							}
//							args.Player.SendSuccessMessage("Modified all groups.");
//							return;
//						}
//						try
//						{
//							string response = TShock.Groups.AddPermissions(groupName, args.Parameters);
//							if (response.Length > 0)
//							{
//								args.Player.SendSuccessMessage(response);
//							}
//							return;
//						}
//						catch (GroupManagerException ex)
//						{
//							args.Player.SendErrorMessage(ex.ToString());
//						}
//					}
//					#endregion
//					return;
//				case "help":
//					#region Help
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;

//						var lines = new List<string>
//						{
//							"add <name> <permissions...> - Adds a new group.",
//							"addperm <group> <permissions...> - Adds permissions to a group.",
//							"color <group> <rrr,ggg,bbb> - Changes a group's chat color.",
//							"rename <group> <new name> - Changes a group's name.",
//							"del <group> - Deletes a group.",
//							"delperm <group> <permissions...> - Removes permissions from a group.",
//							"list [page] - Lists groups.",
//							"listperm <group> [page] - Lists a group's permissions.",
//							"parent <group> <parent group> - Changes a group's parent group.",
//							"prefix <group> <prefix> - Changes a group's prefix.",
//							"suffix <group> <suffix> - Changes a group's suffix."
//						};

//						PaginationTools.SendPage(args.Player, pageNumber, lines,
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Group Sub-Commands ({0}/{1}):",
//								FooterFormat = "Type {0}group help {{0}} for more sub-commands.".SFormat(Specifier)
//							}
//						);
//					}
//					#endregion
//					return;
//				case "parent":
//					#region Parent
//					{
//						if (args.Parameters.Count < 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group parent <group name> [new parent group name]", Specifier);
//							return;
//						}

//						string groupName = args.Parameters[1];
//						Group group = TShock.Groups.GetGroupByName(groupName);
//						if (group == null)
//						{
//							args.Player.SendErrorMessage("No such group \"{0}\".", groupName);
//							return;
//						}

//						if (args.Parameters.Count > 2)
//						{
//							string newParentGroupName = string.Join(" ", args.Parameters.Skip(2));
//							if (!string.IsNullOrWhiteSpace(newParentGroupName) && !TShock.Groups.GroupExists(newParentGroupName))
//							{
//								args.Player.SendErrorMessage("No such group \"{0}\".", newParentGroupName);
//								return;
//							}

//							try
//							{
//								TShock.Groups.UpdateGroup(groupName, newParentGroupName, group.Permissions, group.ChatColor, group.Suffix, group.Prefix);

//								if (!string.IsNullOrWhiteSpace(newParentGroupName))
//									args.Player.SendSuccessMessage("Parent of group \"{0}\" set to \"{1}\".", groupName, newParentGroupName);
//								else
//									args.Player.SendSuccessMessage("Removed parent of group \"{0}\".", groupName);
//							}
//							catch (GroupManagerException ex)
//							{
//								args.Player.SendErrorMessage(ex.Message);
//							}
//						}
//						else
//						{
//							if (group.Parent != null)
//								args.Player.SendSuccessMessage("Parent of \"{0}\" is \"{1}\".", group.Name, group.Parent.Name);
//							else
//								args.Player.SendSuccessMessage("Group \"{0}\" has no parent.", group.Name);
//						}
//					}
//					#endregion
//					return;
//				case "suffix":
//					#region Suffix
//					{
//						if (args.Parameters.Count < 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group suffix <group name> [new suffix]", Specifier);
//							return;
//						}

//						string groupName = args.Parameters[1];
//						Group group = TShock.Groups.GetGroupByName(groupName);
//						if (group == null)
//						{
//							args.Player.SendErrorMessage("No such group \"{0}\".", groupName);
//							return;
//						}

//						if (args.Parameters.Count > 2)
//						{
//							string newSuffix = string.Join(" ", args.Parameters.Skip(2));

//							try
//							{
//								TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, group.ChatColor, newSuffix, group.Prefix);

//								if (!string.IsNullOrWhiteSpace(newSuffix))
//									args.Player.SendSuccessMessage("Suffix of group \"{0}\" set to \"{1}\".", groupName, newSuffix);
//								else
//									args.Player.SendSuccessMessage("Removed suffix of group \"{0}\".", groupName);
//							}
//							catch (GroupManagerException ex)
//							{
//								args.Player.SendErrorMessage(ex.Message);
//							}
//						}
//						else
//						{
//							if (!string.IsNullOrWhiteSpace(group.Suffix))
//								args.Player.SendSuccessMessage("Suffix of \"{0}\" is \"{1}\".", group.Name, group.Suffix);
//							else
//								args.Player.SendSuccessMessage("Group \"{0}\" has no suffix.", group.Name);
//						}
//					}
//					#endregion
//					return;
//				case "prefix":
//					#region Prefix
//					{
//						if (args.Parameters.Count < 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group prefix <group name> [new prefix]", Specifier);
//							return;
//						}

//						string groupName = args.Parameters[1];
//						Group group = TShock.Groups.GetGroupByName(groupName);
//						if (group == null)
//						{
//							args.Player.SendErrorMessage("No such group \"{0}\".", groupName);
//							return;
//						}

//						if (args.Parameters.Count > 2)
//						{
//							string newPrefix = string.Join(" ", args.Parameters.Skip(2));

//							try
//							{
//								TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, group.ChatColor, group.Suffix, newPrefix);

//								if (!string.IsNullOrWhiteSpace(newPrefix))
//									args.Player.SendSuccessMessage("Prefix of group \"{0}\" set to \"{1}\".", groupName, newPrefix);
//								else
//									args.Player.SendSuccessMessage("Removed prefix of group \"{0}\".", groupName);
//							}
//							catch (GroupManagerException ex)
//							{
//								args.Player.SendErrorMessage(ex.Message);
//							}
//						}
//						else
//						{
//							if (!string.IsNullOrWhiteSpace(group.Prefix))
//								args.Player.SendSuccessMessage("Prefix of \"{0}\" is \"{1}\".", group.Name, group.Prefix);
//							else
//								args.Player.SendSuccessMessage("Group \"{0}\" has no prefix.", group.Name);
//						}
//					}
//					#endregion
//					return;
//				case "color":
//					#region Color
//					{
//						if (args.Parameters.Count < 2 || args.Parameters.Count > 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group color <group name> [new color(000,000,000)]", Specifier);
//							return;
//						}

//						string groupName = args.Parameters[1];
//						Group group = TShock.Groups.GetGroupByName(groupName);
//						if (group == null)
//						{
//							args.Player.SendErrorMessage("No such group \"{0}\".", groupName);
//							return;
//						}

//						if (args.Parameters.Count == 3)
//						{
//							string newColor = args.Parameters[2];

//							String[] parts = newColor.Split(',');
//							byte r;
//							byte g;
//							byte b;
//							if (parts.Length == 3 && byte.TryParse(parts[0], out r) && byte.TryParse(parts[1], out g) && byte.TryParse(parts[2], out b))
//							{
//								try
//								{
//									TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, newColor, group.Suffix, group.Prefix);

//									args.Player.SendSuccessMessage("Color of group \"{0}\" set to \"{1}\".", groupName, newColor);
//								}
//								catch (GroupManagerException ex)
//								{
//									args.Player.SendErrorMessage(ex.Message);
//								}
//							}
//							else
//							{
//								args.Player.SendErrorMessage("Invalid syntax for color, expected \"rrr,ggg,bbb\"");
//							}
//						}
//						else
//						{
//							args.Player.SendSuccessMessage("Color of \"{0}\" is \"{1}\".", group.Name, group.ChatColor);
//						}
//					}
//					#endregion
//					return;
//				case "rename":
//					#region Rename group
//					{
//						if (args.Parameters.Count != 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group rename <group> <new name>", Specifier);
//							return;
//						}

//						string group = args.Parameters[1];
//						string newName = args.Parameters[2];
//						try
//						{
//							string response = TShock.Groups.RenameGroup(group, newName);
//							args.Player.SendSuccessMessage(response);
//						}
//						catch (GroupManagerException ex)
//						{
//							args.Player.SendErrorMessage(ex.Message);
//						}
//					}
//					#endregion
//					return;
//				case "del":
//					#region Delete group
//					{
//						if (args.Parameters.Count != 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group del <group name>", Specifier);
//							return;
//						}

//						try
//						{
//							string response = TShock.Groups.DeleteGroup(args.Parameters[1], true);
//							if (response.Length > 0)
//							{
//								args.Player.SendSuccessMessage(response);
//							}
//						}
//						catch (GroupManagerException ex)
//						{
//							args.Player.SendErrorMessage(ex.Message);
//						}
//					}
//					#endregion
//					return;
//				case "delperm":
//					#region Delete permissions
//					{
//						if (args.Parameters.Count < 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group delperm <group name> <permissions...>", Specifier);
//							return;
//						}

//						string groupName = args.Parameters[1];
//						args.Parameters.RemoveRange(0, 2);
//						if (groupName == "*")
//						{
//							foreach (Group g in TShock.Groups)
//							{
//								TShock.Groups.DeletePermissions(g.Name, args.Parameters);
//							}
//							args.Player.SendSuccessMessage("Modified all groups.");
//							return;
//						}
//						try
//						{
//							string response = TShock.Groups.DeletePermissions(groupName, args.Parameters);
//							if (response.Length > 0)
//							{
//								args.Player.SendSuccessMessage(response);
//							}
//							return;
//						}
//						catch (GroupManagerException ex)
//						{
//							args.Player.SendErrorMessage(ex.Message);
//						}
//					}
//					#endregion
//					return;
//				case "list":
//					#region List groups
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;
//						var groupNames = from grp in TShock.Groups.groups
//										 select grp.Name;
//						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(groupNames),
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Groups ({0}/{1}):",
//								FooterFormat = "Type {0}group list {{0}} for more.".SFormat(Specifier)
//							});
//					}
//					#endregion
//					return;
//				case "listperm":
//					#region List permissions
//					{
//						if (args.Parameters.Count == 1)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}group listperm <group name> [page]", Specifier);
//							return;
//						}
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out pageNumber))
//							return;

//						if (!TShock.Groups.GroupExists(args.Parameters[1]))
//						{
//							args.Player.SendErrorMessage("Invalid group.");
//							return;
//						}
//						Group grp = TShock.Groups.GetGroupByName(args.Parameters[1]);
//						List<string> permissions = grp.TotalPermissions;

//						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(permissions),
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Permissions for " + grp.Name + " ({0}/{1}):",
//								FooterFormat = "Type {0}group listperm {1} {{0}} for more.".SFormat(Specifier, grp.Name),
//								NothingToDisplayString = "There are currently no permissions for " + grp.Name + "."
//							});
//					}
//					#endregion
//					return;
//				default:
//					args.Player.SendErrorMessage("Invalid subcommand! Type {0}group help for more information on valid commands.", Specifier);
//					return;
//			}
//		}
//		#endregion Group Management

//		#region Item Management

//		private static void ItemBan(CommandArgs args)
//		{
//			string subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();
//			switch (subCmd)
//			{
//				case "add":
//					#region Add item
//					{
//						if (args.Parameters.Count != 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}itemban add <item name>", Specifier);
//							return;
//						}

//						List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
//						if (items.Count == 0)
//						{
//							args.Player.SendErrorMessage("Invalid item.");
//						}
//						else if (items.Count > 1)
//						{
//							args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
//						}
//						else
//						{
//							// Yes this is required because of localization
//							// User may have passed in localized name but itembans works on English names
//							string englishNameForStorage = EnglishLanguage.GetItemNameById(items[0].type);
//							TShock.ItemBans.DataModel.AddNewBan(englishNameForStorage);

//							// It was decided in Telegram that we would continue to ban
//							// projectiles based on whether or not their associated item was
//							// banned. However, it was also decided that we'd change the way
//							// this worked: in particular, we'd make it so that the item ban
//							// system just adds things to the projectile ban system at the
//							// command layer instead of inferring the state of projectile
//							// bans based on the state of the item ban system.

//							if (items[0].type == ItemID.DirtRod)
//							{
//								TShock.ProjectileBans.AddNewBan(ProjectileID.DirtBall);
//							}

//							if (items[0].type == ItemID.Sandgun)
//							{
//								TShock.ProjectileBans.AddNewBan(ProjectileID.SandBallGun);
//								TShock.ProjectileBans.AddNewBan(ProjectileID.EbonsandBallGun);
//								TShock.ProjectileBans.AddNewBan(ProjectileID.PearlSandBallGun);
//							}

//							// This returns the localized name to the player, not the item as it was stored.
//							args.Player.SendSuccessMessage("Banned " + items[0].Name + ".");
//						}
//					}
//					#endregion
//					return;
//				case "allow":
//					#region Allow group to item
//					{
//						if (args.Parameters.Count != 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}itemban allow <item name> <group name>", Specifier);
//							return;
//						}

//						List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
//						if (items.Count == 0)
//						{
//							args.Player.SendErrorMessage("Invalid item.");
//						}
//						else if (items.Count > 1)
//						{
//							args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
//						}
//						else
//						{
//							if (!TShock.Groups.GroupExists(args.Parameters[2]))
//							{
//								args.Player.SendErrorMessage("Invalid group.");
//								return;
//							}

//							ItemBan ban = TShock.ItemBans.DataModel.GetItemBanByName(EnglishLanguage.GetItemNameById(items[0].type));
//							if (ban == null)
//							{
//								args.Player.SendErrorMessage("{0} is not banned.", items[0].Name);
//								return;
//							}
//							if (!ban.AllowedGroups.Contains(args.Parameters[2]))
//							{
//								TShock.ItemBans.DataModel.AllowGroup(EnglishLanguage.GetItemNameById(items[0].type), args.Parameters[2]);
//								args.Player.SendSuccessMessage("{0} has been allowed to use {1}.", args.Parameters[2], items[0].Name);
//							}
//							else
//							{
//								args.Player.SendWarningMessage("{0} is already allowed to use {1}.", args.Parameters[2], items[0].Name);
//							}
//						}
//					}
//					#endregion
//					return;
//				case "del":
//					#region Delete item
//					{
//						if (args.Parameters.Count != 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}itemban del <item name>", Specifier);
//							return;
//						}

//						List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
//						if (items.Count == 0)
//						{
//							args.Player.SendErrorMessage("Invalid item.");
//						}
//						else if (items.Count > 1)
//						{
//							args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
//						}
//						else
//						{
//							TShock.ItemBans.DataModel.RemoveBan(EnglishLanguage.GetItemNameById(items[0].type));
//							args.Player.SendSuccessMessage("Unbanned " + items[0].Name + ".");
//						}
//					}
//					#endregion
//					return;
//				case "disallow":
//					#region Disllow group from item
//					{
//						if (args.Parameters.Count != 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}itemban disallow <item name> <group name>", Specifier);
//							return;
//						}

//						List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
//						if (items.Count == 0)
//						{
//							args.Player.SendErrorMessage("Invalid item.");
//						}
//						else if (items.Count > 1)
//						{
//							args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
//						}
//						else
//						{
//							if (!TShock.Groups.GroupExists(args.Parameters[2]))
//							{
//								args.Player.SendErrorMessage("Invalid group.");
//								return;
//							}

//							ItemBan ban = TShock.ItemBans.DataModel.GetItemBanByName(EnglishLanguage.GetItemNameById(items[0].type));
//							if (ban == null)
//							{
//								args.Player.SendErrorMessage("{0} is not banned.", items[0].Name);
//								return;
//							}
//							if (ban.AllowedGroups.Contains(args.Parameters[2]))
//							{
//								TShock.ItemBans.DataModel.RemoveGroup(EnglishLanguage.GetItemNameById(items[0].type), args.Parameters[2]);
//								args.Player.SendSuccessMessage("{0} has been disallowed to use {1}.", args.Parameters[2], items[0].Name);
//							}
//							else
//							{
//								args.Player.SendWarningMessage("{0} is already disallowed to use {1}.", args.Parameters[2], items[0].Name);
//							}
//						}
//					}
//					#endregion
//					return;
//				case "help":
//					#region Help
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;

//						var lines = new List<string>
//						{
//							"add <item> - Adds an item ban.",
//							"allow <item> <group> - Allows a group to use an item.",
//							"del <item> - Deletes an item ban.",
//							"disallow <item> <group> - Disallows a group from using an item.",
//							"list [page] - Lists all item bans."
//						};

//						PaginationTools.SendPage(args.Player, pageNumber, lines,
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Item Ban Sub-Commands ({0}/{1}):",
//								FooterFormat = "Type {0}itemban help {{0}} for more sub-commands.".SFormat(Specifier)
//							}
//						);
//					}
//					#endregion
//					return;
//				case "list":
//					#region List items
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;
//						IEnumerable<string> itemNames = from itemBan in TShock.ItemBans.DataModel.ItemBans
//														select itemBan.Name;
//						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(itemNames),
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Item bans ({0}/{1}):",
//								FooterFormat = "Type {0}itemban list {{0}} for more.".SFormat(Specifier),
//								NothingToDisplayString = "There are currently no banned items."
//							});
//					}
//					#endregion
//					return;
//				default:
//					#region Default
//					{
//						args.Player.SendErrorMessage("Invalid subcommand! Type {0}itemban help for more information on valid subcommands.", Specifier);
//					}
//					#endregion
//					return;

//			}
//		}
//		#endregion Item Management

//		#region Projectile Management

//		private static void ProjectileBan(CommandArgs args)
//		{
//			string subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();
//			switch (subCmd)
//			{
//				case "add":
//					#region Add projectile
//					{
//						if (args.Parameters.Count != 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}projban add <proj id>", Specifier);
//							return;
//						}
//						short id;
//						if (Int16.TryParse(args.Parameters[1], out id) && id > 0 && id < Main.maxProjectileTypes)
//						{
//							TShock.ProjectileBans.AddNewBan(id);
//							args.Player.SendSuccessMessage("Banned projectile {0}.", id);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid projectile ID!");
//					}
//					#endregion
//					return;
//				case "allow":
//					#region Allow group to projectile
//					{
//						if (args.Parameters.Count != 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}projban allow <id> <group>", Specifier);
//							return;
//						}

//						short id;
//						if (Int16.TryParse(args.Parameters[1], out id) && id > 0 && id < Main.maxProjectileTypes)
//						{
//							if (!TShock.Groups.GroupExists(args.Parameters[2]))
//							{
//								args.Player.SendErrorMessage("Invalid group.");
//								return;
//							}

//							ProjectileBan ban = TShock.ProjectileBans.GetBanById(id);
//							if (ban == null)
//							{
//								args.Player.SendErrorMessage("Projectile {0} is not banned.", id);
//								return;
//							}
//							if (!ban.AllowedGroups.Contains(args.Parameters[2]))
//							{
//								TShock.ProjectileBans.AllowGroup(id, args.Parameters[2]);
//								args.Player.SendSuccessMessage("{0} has been allowed to use projectile {1}.", args.Parameters[2], id);
//							}
//							else
//								args.Player.SendWarningMessage("{0} is already allowed to use projectile {1}.", args.Parameters[2], id);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid projectile ID!");
//					}
//					#endregion
//					return;
//				case "del":
//					#region Delete projectile
//					{
//						if (args.Parameters.Count != 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}projban del <id>", Specifier);
//							return;
//						}

//						short id;
//						if (Int16.TryParse(args.Parameters[1], out id) && id > 0 && id < Main.maxProjectileTypes)
//						{
//							TShock.ProjectileBans.RemoveBan(id);
//							args.Player.SendSuccessMessage("Unbanned projectile {0}.", id);
//							return;
//						}
//						else
//							args.Player.SendErrorMessage("Invalid projectile ID!");
//					}
//					#endregion
//					return;
//				case "disallow":
//					#region Disallow group from projectile
//					{
//						if (args.Parameters.Count != 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}projban disallow <id> <group name>", Specifier);
//							return;
//						}

//						short id;
//						if (Int16.TryParse(args.Parameters[1], out id) && id > 0 && id < Main.maxProjectileTypes)
//						{
//							if (!TShock.Groups.GroupExists(args.Parameters[2]))
//							{
//								args.Player.SendErrorMessage("Invalid group.");
//								return;
//							}

//							ProjectileBan ban = TShock.ProjectileBans.GetBanById(id);
//							if (ban == null)
//							{
//								args.Player.SendErrorMessage("Projectile {0} is not banned.", id);
//								return;
//							}
//							if (ban.AllowedGroups.Contains(args.Parameters[2]))
//							{
//								TShock.ProjectileBans.RemoveGroup(id, args.Parameters[2]);
//								args.Player.SendSuccessMessage("{0} has been disallowed from using projectile {1}.", args.Parameters[2], id);
//								return;
//							}
//							else
//								args.Player.SendWarningMessage("{0} is already prevented from using projectile {1}.", args.Parameters[2], id);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid projectile ID!");
//					}
//					#endregion
//					return;
//				case "help":
//					#region Help
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;

//						var lines = new List<string>
//						{
//							"add <projectile ID> - Adds a projectile ban.",
//							"allow <projectile ID> <group> - Allows a group to use a projectile.",
//							"del <projectile ID> - Deletes an projectile ban.",
//							"disallow <projectile ID> <group> - Disallows a group from using a projectile.",
//							"list [page] - Lists all projectile bans."
//						};

//						PaginationTools.SendPage(args.Player, pageNumber, lines,
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Projectile Ban Sub-Commands ({0}/{1}):",
//								FooterFormat = "Type {0}projban help {{0}} for more sub-commands.".SFormat(Specifier)
//							}
//						);
//					}
//					#endregion
//					return;
//				case "list":
//					#region List projectiles
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;
//						IEnumerable<Int16> projectileIds = from projectileBan in TShock.ProjectileBans.ProjectileBans
//														   select projectileBan.ID;
//						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(projectileIds),
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Projectile bans ({0}/{1}):",
//								FooterFormat = "Type {0}projban list {{0}} for more.".SFormat(Specifier),
//								NothingToDisplayString = "There are currently no banned projectiles."
//							});
//					}
//					#endregion
//					return;
//				default:
//					#region Default
//					{
//						args.Player.SendErrorMessage("Invalid subcommand! Type {0}projban help for more information on valid subcommands.", Specifier);
//					}
//					#endregion
//					return;
//			}
//		}
//		#endregion Projectile Management

//		#region Tile Management
//		private static void TileBan(CommandArgs args)
//		{
//			string subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();
//			switch (subCmd)
//			{
//				case "add":
//					#region Add tile
//					{
//						if (args.Parameters.Count != 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tileban add <tile id>", Specifier);
//							return;
//						}
//						short id;
//						if (Int16.TryParse(args.Parameters[1], out id) && id >= 0 && id < Main.maxTileSets)
//						{
//							TShock.TileBans.AddNewBan(id);
//							args.Player.SendSuccessMessage("Banned tile {0}.", id);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid tile ID!");
//					}
//					#endregion
//					return;
//				case "allow":
//					#region Allow group to place tile
//					{
//						if (args.Parameters.Count != 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tileban allow <id> <group>", Specifier);
//							return;
//						}

//						short id;
//						if (Int16.TryParse(args.Parameters[1], out id) && id >= 0 && id < Main.maxTileSets)
//						{
//							if (!TShock.Groups.GroupExists(args.Parameters[2]))
//							{
//								args.Player.SendErrorMessage("Invalid group.");
//								return;
//							}

//							TileBan ban = TShock.TileBans.GetBanById(id);
//							if (ban == null)
//							{
//								args.Player.SendErrorMessage("Tile {0} is not banned.", id);
//								return;
//							}
//							if (!ban.AllowedGroups.Contains(args.Parameters[2]))
//							{
//								TShock.TileBans.AllowGroup(id, args.Parameters[2]);
//								args.Player.SendSuccessMessage("{0} has been allowed to place tile {1}.", args.Parameters[2], id);
//							}
//							else
//								args.Player.SendWarningMessage("{0} is already allowed to place tile {1}.", args.Parameters[2], id);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid tile ID!");
//					}
//					#endregion
//					return;
//				case "del":
//					#region Delete tile ban
//					{
//						if (args.Parameters.Count != 2)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tileban del <id>", Specifier);
//							return;
//						}

//						short id;
//						if (Int16.TryParse(args.Parameters[1], out id) && id >= 0 && id < Main.maxTileSets)
//						{
//							TShock.TileBans.RemoveBan(id);
//							args.Player.SendSuccessMessage("Unbanned tile {0}.", id);
//							return;
//						}
//						else
//							args.Player.SendErrorMessage("Invalid tile ID!");
//					}
//					#endregion
//					return;
//				case "disallow":
//					#region Disallow group from placing tile
//					{
//						if (args.Parameters.Count != 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tileban disallow <id> <group name>", Specifier);
//							return;
//						}

//						short id;
//						if (Int16.TryParse(args.Parameters[1], out id) && id >= 0 && id < Main.maxTileSets)
//						{
//							if (!TShock.Groups.GroupExists(args.Parameters[2]))
//							{
//								args.Player.SendErrorMessage("Invalid group.");
//								return;
//							}

//							TileBan ban = TShock.TileBans.GetBanById(id);
//							if (ban == null)
//							{
//								args.Player.SendErrorMessage("Tile {0} is not banned.", id);
//								return;
//							}
//							if (ban.AllowedGroups.Contains(args.Parameters[2]))
//							{
//								TShock.TileBans.RemoveGroup(id, args.Parameters[2]);
//								args.Player.SendSuccessMessage("{0} has been disallowed from placing tile {1}.", args.Parameters[2], id);
//								return;
//							}
//							else
//								args.Player.SendWarningMessage("{0} is already prevented from placing tile {1}.", args.Parameters[2], id);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid tile ID!");
//					}
//					#endregion
//					return;
//				case "help":
//					#region Help
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;

//						var lines = new List<string>
//						{
//							"add <tile ID> - Adds a tile ban.",
//							"allow <tile ID> <group> - Allows a group to place a tile.",
//							"del <tile ID> - Deletes a tile ban.",
//							"disallow <tile ID> <group> - Disallows a group from place a tile.",
//							"list [page] - Lists all tile bans."
//						};

//						PaginationTools.SendPage(args.Player, pageNumber, lines,
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Tile Ban Sub-Commands ({0}/{1}):",
//								FooterFormat = "Type {0}tileban help {{0}} for more sub-commands.".SFormat(Specifier)
//							}
//						);
//					}
//					#endregion
//					return;
//				case "list":
//					#region List tile bans
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;
//						IEnumerable<Int16> tileIds = from tileBan in TShock.TileBans.TileBans
//													 select tileBan.ID;
//						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(tileIds),
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Tile bans ({0}/{1}):",
//								FooterFormat = "Type {0}tileban list {{0}} for more.".SFormat(Specifier),
//								NothingToDisplayString = "There are currently no banned tiles."
//							});
//					}
//					#endregion
//					return;
//				default:
//					#region Default
//					{
//						args.Player.SendErrorMessage("Invalid subcommand! Type {0}tileban help for more information on valid subcommands.", Specifier);
//					}
//					#endregion
//					return;
//			}
//		}
//		#endregion Tile Management

//		#region Server Config Commands


//		#endregion Server Config Commands

//		#region Time/PvpFun Commands

//		#endregion Time/PvpFun Commands

//		#region Region Commands

//		private static void Region(CommandArgs args)
//		{
//			string cmd = "help";
//			if (args.Parameters.Count > 0)
//			{
//				cmd = args.Parameters[0].ToLower();
//			}
//			switch (cmd)
//			{
//				case "name":
//					{
//						{
//							args.Player.SendInfoMessage("Hit a block to get the name of the region");
//							args.Player.AwaitingName = true;
//							args.Player.AwaitingNameParameters = args.Parameters.Skip(1).ToArray();
//						}
//						break;
//					}
//				case "set":
//					{
//						int choice = 0;
//						if (args.Parameters.Count == 2 &&
//							int.TryParse(args.Parameters[1], out choice) &&
//							choice >= 1 && choice <= 2)
//						{
//							args.Player.SendInfoMessage("Hit a block to Set Point " + choice);
//							args.Player.AwaitingTempPoint = choice;
//						}
//						else
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region set <1/2>");
//						}
//						break;
//					}
//				case "define":
//					{
//						if (args.Parameters.Count > 1)
//						{
//							if (!args.Player.TempPoints.Any(p => p == Point.Zero))
//							{
//								string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
//								var x = Math.Min(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X);
//								var y = Math.Min(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y);
//								var width = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X);
//								var height = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y);

//								if (TShock.Regions.AddRegion(x, y, width, height, regionName, args.Player.Account.Name,
//															 Main.worldID.ToString()))
//								{
//									args.Player.TempPoints[0] = Point.Zero;
//									args.Player.TempPoints[1] = Point.Zero;
//									args.Player.SendInfoMessage("Set region " + regionName);
//								}
//								else
//								{
//									args.Player.SendErrorMessage("Region " + regionName + " already exists");
//								}
//							}
//							else
//							{
//								args.Player.SendErrorMessage("Points not set up yet");
//							}
//						}
//						else
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region define <name>", Specifier);
//						break;
//					}
//				case "protect":
//					{
//						if (args.Parameters.Count == 3)
//						{
//							string regionName = args.Parameters[1];
//							if (args.Parameters[2].ToLower() == "true")
//							{
//								if (TShock.Regions.SetRegionState(regionName, true))
//									args.Player.SendInfoMessage("Protected region " + regionName);
//								else
//									args.Player.SendErrorMessage("Could not find specified region");
//							}
//							else if (args.Parameters[2].ToLower() == "false")
//							{
//								if (TShock.Regions.SetRegionState(regionName, false))
//									args.Player.SendInfoMessage("Unprotected region " + regionName);
//								else
//									args.Player.SendErrorMessage("Could not find specified region");
//							}
//							else
//								args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region protect <name> <true/false>", Specifier);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /region protect <name> <true/false>", Specifier);
//						break;
//					}
//				case "delete":
//					{
//						if (args.Parameters.Count > 1)
//						{
//							string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
//							if (TShock.Regions.DeleteRegion(regionName))
//							{
//								args.Player.SendInfoMessage("Deleted region \"{0}\".", regionName);
//							}
//							else
//								args.Player.SendErrorMessage("Could not find the specified region!");
//						}
//						else
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region delete <name>", Specifier);
//						break;
//					}
//				case "clear":
//					{
//						args.Player.TempPoints[0] = Point.Zero;
//						args.Player.TempPoints[1] = Point.Zero;
//						args.Player.SendInfoMessage("Cleared temporary points.");
//						args.Player.AwaitingTempPoint = 0;
//						break;
//					}
//				case "allow":
//					{
//						if (args.Parameters.Count > 2)
//						{
//							string playerName = args.Parameters[1];
//							string regionName = "";

//							for (int i = 2; i < args.Parameters.Count; i++)
//							{
//								if (regionName == "")
//								{
//									regionName = args.Parameters[2];
//								}
//								else
//								{
//									regionName = regionName + " " + args.Parameters[i];
//								}
//							}
//							if (TShock.UserAccounts.GetUserAccountByName(playerName) != null)
//							{
//								if (TShock.Regions.AddNewUser(regionName, playerName))
//								{
//									args.Player.SendInfoMessage("Added user " + playerName + " to " + regionName);
//								}
//								else
//									args.Player.SendErrorMessage("Region " + regionName + " not found");
//							}
//							else
//							{
//								args.Player.SendErrorMessage("Player " + playerName + " not found");
//							}
//						}
//						else
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region allow <name> <region>", Specifier);
//						break;
//					}
//				case "remove":
//					if (args.Parameters.Count > 2)
//					{
//						string playerName = args.Parameters[1];
//						string regionName = "";

//						for (int i = 2; i < args.Parameters.Count; i++)
//						{
//							if (regionName == "")
//							{
//								regionName = args.Parameters[2];
//							}
//							else
//							{
//								regionName = regionName + " " + args.Parameters[i];
//							}
//						}
//						if (TShock.UserAccounts.GetUserAccountByName(playerName) != null)
//						{
//							if (TShock.Regions.RemoveUser(regionName, playerName))
//							{
//								args.Player.SendInfoMessage("Removed user " + playerName + " from " + regionName);
//							}
//							else
//								args.Player.SendErrorMessage("Region " + regionName + " not found");
//						}
//						else
//						{
//							args.Player.SendErrorMessage("Player " + playerName + " not found");
//						}
//					}
//					else
//						args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region remove <name> <region>", Specifier);
//					break;
//				case "allowg":
//					{
//						if (args.Parameters.Count > 2)
//						{
//							string group = args.Parameters[1];
//							string regionName = "";

//							for (int i = 2; i < args.Parameters.Count; i++)
//							{
//								if (regionName == "")
//								{
//									regionName = args.Parameters[2];
//								}
//								else
//								{
//									regionName = regionName + " " + args.Parameters[i];
//								}
//							}
//							if (TShock.Groups.GroupExists(group))
//							{
//								if (TShock.Regions.AllowGroup(regionName, group))
//								{
//									args.Player.SendInfoMessage("Added group " + group + " to " + regionName);
//								}
//								else
//									args.Player.SendErrorMessage("Region " + regionName + " not found");
//							}
//							else
//							{
//								args.Player.SendErrorMessage("Group " + group + " not found");
//							}
//						}
//						else
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region allowg <group> <region>", Specifier);
//						break;
//					}
//				case "removeg":
//					if (args.Parameters.Count > 2)
//					{
//						string group = args.Parameters[1];
//						string regionName = "";

//						for (int i = 2; i < args.Parameters.Count; i++)
//						{
//							if (regionName == "")
//							{
//								regionName = args.Parameters[2];
//							}
//							else
//							{
//								regionName = regionName + " " + args.Parameters[i];
//							}
//						}
//						if (TShock.Groups.GroupExists(group))
//						{
//							if (TShock.Regions.RemoveGroup(regionName, group))
//							{
//								args.Player.SendInfoMessage("Removed group " + group + " from " + regionName);
//							}
//							else
//								args.Player.SendErrorMessage("Region " + regionName + " not found");
//						}
//						else
//						{
//							args.Player.SendErrorMessage("Group " + group + " not found");
//						}
//					}
//					else
//						args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region removeg <group> <region>", Specifier);
//					break;
//				case "list":
//					{
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
//							return;

//						IEnumerable<string> regionNames = from region in TShock.Regions.Regions
//														  where region.WorldID == Main.worldID.ToString()
//														  select region.Name;
//						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(regionNames),
//							new PaginationTools.Settings
//							{
//								HeaderFormat = "Regions ({0}/{1}):",
//								FooterFormat = "Type {0}region list {{0}} for more.".SFormat(Specifier),
//								NothingToDisplayString = "There are currently no regions defined."
//							});
//						break;
//					}
//				case "info":
//					{
//						if (args.Parameters.Count == 1 || args.Parameters.Count > 4)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region info <region> [-d] [page]", Specifier);
//							break;
//						}

//						string regionName = args.Parameters[1];
//						bool displayBoundaries = args.Parameters.Skip(2).Any(
//							p => p.Equals("-d", StringComparison.InvariantCultureIgnoreCase)
//						);

//						Region region = TShock.Regions.GetRegionByName(regionName);
//						if (region == null)
//						{
//							args.Player.SendErrorMessage("Region \"{0}\" does not exist.", regionName);
//							break;
//						}

//						int pageNumberIndex = displayBoundaries ? 3 : 2;
//						int pageNumber;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, pageNumberIndex, args.Player, out pageNumber))
//							break;

//						List<string> lines = new List<string>
//						{
//							string.Format("X: {0}; Y: {1}; W: {2}; H: {3}, Z: {4}", region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Z),
//							string.Concat("Owner: ", region.Owner),
//							string.Concat("Protected: ", region.DisableBuild.ToString()),
//						};

//						if (region.AllowedIDs.Count > 0)
//						{
//							IEnumerable<string> sharedUsersSelector = region.AllowedIDs.Select(userId =>
//							{
//								UserAccount account = TShock.UserAccounts.GetUserAccountByID(userId);
//								if (account != null)
//									return account.Name;

//								return string.Concat("{ID: ", userId, "}");
//							});
//							List<string> extraLines = PaginationTools.BuildLinesFromTerms(sharedUsersSelector.Distinct());
//							extraLines[0] = "Shared with: " + extraLines[0];
//							lines.AddRange(extraLines);
//						}
//						else
//						{
//							lines.Add("Region is not shared with any users.");
//						}

//						if (region.AllowedGroups.Count > 0)
//						{
//							List<string> extraLines = PaginationTools.BuildLinesFromTerms(region.AllowedGroups.Distinct());
//							extraLines[0] = "Shared with groups: " + extraLines[0];
//							lines.AddRange(extraLines);
//						}
//						else
//						{
//							lines.Add("Region is not shared with any groups.");
//						}

//						PaginationTools.SendPage(
//							args.Player, pageNumber, lines, new PaginationTools.Settings
//							{
//								HeaderFormat = string.Format("Information About Region \"{0}\" ({{0}}/{{1}}):", region.Name),
//								FooterFormat = string.Format("Type {0}region info {1} {{0}} for more information.", Specifier, regionName)
//							}
//						);

//						if (displayBoundaries)
//						{
//							Rectangle regionArea = region.Area;
//							foreach (Point boundaryPoint in Utils.Instance.EnumerateRegionBoundaries(regionArea))
//							{
//								// Preferring dotted lines as those should easily be distinguishable from actual wires.
//								if ((boundaryPoint.X + boundaryPoint.Y & 1) == 0)
//								{
//									// Could be improved by sending raw tile data to the client instead but not really
//									// worth the effort as chances are very low that overwriting the wire for a few
//									// nanoseconds will cause much trouble.
//									ITile tile = Main.tile[boundaryPoint.X, boundaryPoint.Y];
//									bool oldWireState = tile.wire();
//									tile.wire(true);

//									try
//									{
//										args.Player.SendTileSquareCentered(boundaryPoint.X, boundaryPoint.Y, 1);
//									}
//									finally
//									{
//										tile.wire(oldWireState);
//									}
//								}
//							}

//							Timer boundaryHideTimer = null;
//							boundaryHideTimer = new Timer((state) =>
//							{
//								foreach (Point boundaryPoint in Utils.Instance.EnumerateRegionBoundaries(regionArea))
//									if ((boundaryPoint.X + boundaryPoint.Y & 1) == 0)
//										args.Player.SendTileSquareCentered(boundaryPoint.X, boundaryPoint.Y, 1);

//								Debug.Assert(boundaryHideTimer != null);
//								boundaryHideTimer.Dispose();
//							},
//								null, 5000, Timeout.Infinite
//							);
//						}

//						break;
//					}
//				case "z":
//					{
//						if (args.Parameters.Count == 3)
//						{
//							string regionName = args.Parameters[1];
//							int z = 0;
//							if (int.TryParse(args.Parameters[2], out z))
//							{
//								if (TShock.Regions.SetZ(regionName, z))
//									args.Player.SendInfoMessage("Region's z is now " + z);
//								else
//									args.Player.SendErrorMessage("Could not find specified region");
//							}
//							else
//								args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region z <name> <#>", Specifier);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region z <name> <#>", Specifier);
//						break;
//					}
//				case "resize":
//				case "expand":
//					{
//						if (args.Parameters.Count == 4)
//						{
//							int direction;
//							switch (args.Parameters[2])
//							{
//								case "u":
//								case "up":
//									{
//										direction = 0;
//										break;
//									}
//								case "r":
//								case "right":
//									{
//										direction = 1;
//										break;
//									}
//								case "d":
//								case "down":
//									{
//										direction = 2;
//										break;
//									}
//								case "l":
//								case "left":
//									{
//										direction = 3;
//										break;
//									}
//								default:
//									{
//										direction = -1;
//										break;
//									}
//							}
//							int addAmount;
//							int.TryParse(args.Parameters[3], out addAmount);
//							if (TShock.Regions.ResizeRegion(args.Parameters[1], addAmount, direction))
//							{
//								args.Player.SendInfoMessage("Region Resized Successfully!");
//								TShock.Regions.Reload();
//							}
//							else
//								args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region resize <region> <u/d/l/r> <amount>", Specifier);
//						}
//						else
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region resize <region> <u/d/l/r> <amount>", Specifier);
//						break;
//					}
//				case "rename":
//					{
//						if (args.Parameters.Count != 3)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region rename <region> <new name>", Specifier);
//							break;
//						}
//						else
//						{
//							string oldName = args.Parameters[1];
//							string newName = args.Parameters[2];

//							if (oldName == newName)
//							{
//								args.Player.SendErrorMessage("Error: both names are the same.");
//								break;
//							}

//							Region oldRegion = TShock.Regions.GetRegionByName(oldName);

//							if (oldRegion == null)
//							{
//								args.Player.SendErrorMessage("Invalid region \"{0}\".", oldName);
//								break;
//							}

//							Region newRegion = TShock.Regions.GetRegionByName(newName);

//							if (newRegion != null)
//							{
//								args.Player.SendErrorMessage("Region \"{0}\" already exists.", newName);
//								break;
//							}

//							if (TShock.Regions.RenameRegion(oldName, newName))
//							{
//								args.Player.SendInfoMessage("Region renamed successfully!");
//							}
//							else
//							{
//								args.Player.SendErrorMessage("Failed to rename the region.");
//							}
//						}
//						break;
//					}
//				case "tp":
//					{
//						if (!args.Player.HasPermission(Permissions.tp))
//						{
//							args.Player.SendErrorMessage("You do not have permission to teleport.");
//							break;
//						}
//						if (args.Parameters.Count <= 1)
//						{
//							args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}region tp <region>.", Specifier);
//							break;
//						}

//						string regionName = string.Join(" ", args.Parameters.Skip(1));
//						Region region = TShock.Regions.GetRegionByName(regionName);
//						if (region == null)
//						{
//							args.Player.SendErrorMessage("Region \"{0}\" does not exist.", regionName);
//							break;
//						}

//						args.Player.Teleport(region.Area.Center.X * 16, region.Area.Center.Y * 16);
//						break;
//					}
//				case "help":
//				default:
//					{
//						int pageNumber;
//						int pageParamIndex = 0;
//						if (args.Parameters.Count > 1)
//							pageParamIndex = 1;
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, pageParamIndex, args.Player, out pageNumber))
//							return;

//						List<string> lines = new List<string> {
//						  "set <1/2> - Sets the temporary region points.",
//						  "clear - Clears the temporary region points.",
//						  "define <name> - Defines the region with the given name.",
//						  "delete <name> - Deletes the given region.",
//						  "name [-u][-z][-p] - Shows the name of the region at the given point.",
//						  "rename <region> <new name> - Renames the given region.",
//						  "list - Lists all regions.",
//						  "resize <region> <u/d/l/r> <amount> - Resizes a region.",
//						  "allow <user> <region> - Allows a user to a region.",
//						  "remove <user> <region> - Removes a user from a region.",
//						  "allowg <group> <region> - Allows a user group to a region.",
//						  "removeg <group> <region> - Removes a user group from a region.",
//						  "info <region> [-d] - Displays several information about the given region.",
//						  "protect <name> <true/false> - Sets whether the tiles inside the region are protected or not.",
//						  "z <name> <#> - Sets the z-order of the region.",
//						};
//						if (args.Player.HasPermission(Permissions.tp))
//							lines.Add("tp <region> - Teleports you to the given region's center.");

//						PaginationTools.SendPage(
//						  args.Player, pageNumber, lines,
//						  new PaginationTools.Settings
//						  {
//							  HeaderFormat = "Available Region Sub-Commands ({0}/{1}):",
//							  FooterFormat = "Type {0}region {{0}} for more sub-commands.".SFormat(Specifier)
//						  }
//						);
//						break;
//					}
//			}
//		}

//		#endregion Region Commands

//		#region World Protection Commands


//		#endregion World Protection Commands

//		#region General Commands





//		private static void Mute(CommandArgs args)
//		{
//			if (args.Parameters.Count < 1)
//			{
//				args.Player.SendMessage("Mute Syntax", Color.White);
//				args.Player.SendMessage($"{"mute".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}> [{"reason".Color(Utils.GreenHighlight)}]", Color.White);
//				args.Player.SendMessage($"Example usage: {"mute".Color(Utils.BoldHighlight)} \"{args.Player.Name.Color(Utils.RedHighlight)}\" \"{"No swearing on my Christian server".Color(Utils.GreenHighlight)}\"", Color.White);
//				args.Player.SendMessage($"To mute a player without broadcasting to chat, use the command with {SilentSpecifier.Color(Utils.GreenHighlight)} instead of {Specifier.Color(Utils.RedHighlight)}", Color.White);
//				return;
//			}

//			var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
//			if (players.Count == 0)
//			{
//				args.Player.SendErrorMessage($"Could not find any players named \"{args.Parameters[0]}\"");
//			}
//			else if (players.Count > 1)
//			{
//				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
//			}
//			else if (players[0].HasPermission(Permissions.mute))
//			{
//				args.Player.SendErrorMessage($"You do not have permission to mute {players[0].Name}");
//			}
//			else if (players[0].mute)
//			{
//				var plr = players[0];
//				plr.mute = false;
//				if (args.Silent)
//					args.Player.SendSuccessMessage($"You have unmuted {plr.Name}.");
//				else
//					TSPlayer.All.SendInfoMessage($"{args.Player.Name} has unmuted {plr.Name}.");
//			}
//			else
//			{
//				string reason = "No reason specified.";
//				if (args.Parameters.Count > 1)
//					reason = String.Join(" ", args.Parameters.ToArray(), 1, args.Parameters.Count - 1);
//				var plr = players[0];
//				plr.mute = true;
//				if (args.Silent)
//					args.Player.SendSuccessMessage($"You have muted {plr.Name} for {reason}");
//				else
//					TSPlayer.All.SendInfoMessage($"{args.Player.Name} has muted {plr.Name} for {reason}.");
//			}
//		}






//		#endregion General Commands

//		#region Game Commands





//		#endregion Game Commands
//	}
//}
