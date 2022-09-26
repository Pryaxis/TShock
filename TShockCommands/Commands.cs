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

//		private static void ServerInfo(CommandArgs args)
//		{
//			args.Player.SendInfoMessage("Memory usage: " + Process.GetCurrentProcess().WorkingSet64);
//			args.Player.SendInfoMessage("Allocated memory: " + Process.GetCurrentProcess().VirtualMemorySize64);
//			args.Player.SendInfoMessage("Total processor time: " + Process.GetCurrentProcess().TotalProcessorTime);
//			args.Player.SendInfoMessage("Operating system: " + Environment.OSVersion);
//			args.Player.SendInfoMessage("Proc count: " + Environment.ProcessorCount);
//			args.Player.SendInfoMessage("Machine name: " + Environment.MachineName);
//		}

//		private static void WorldInfo(CommandArgs args)
//		{
//			args.Player.SendInfoMessage("Information about the currently running world");
//			args.Player.SendInfoMessage("Name: " + (TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName));
//			args.Player.SendInfoMessage("Size: {0}x{1}", Main.maxTilesX, Main.maxTilesY);
//			args.Player.SendInfoMessage("ID: " + Main.worldID);
//			args.Player.SendInfoMessage("Seed: " + WorldGen.currentWorldSeed);
//			args.Player.SendInfoMessage("Mode: " + Main.GameMode);
//			args.Player.SendInfoMessage("Path: " + Main.worldPathName);
//		}

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

//		private static void ForceHalloween(CommandArgs args)
//		{
//			TShock.Config.Settings.ForceHalloween = !TShock.Config.Settings.ForceHalloween;
//			Main.checkHalloween();
//			if (args.Silent)
//				args.Player.SendInfoMessage("{0}abled halloween mode!", (TShock.Config.Settings.ForceHalloween ? "en" : "dis"));
//			else
//				TSPlayer.All.SendInfoMessage("{0} {1}abled halloween mode!", args.Player.Name, (TShock.Config.Settings.ForceHalloween ? "en" : "dis"));
//		}

//		private static void ForceXmas(CommandArgs args)
//		{
//			TShock.Config.Settings.ForceXmas = !TShock.Config.Settings.ForceXmas;
//			Main.checkXMas();
//			if (args.Silent)
//				args.Player.SendInfoMessage("{0}abled Christmas mode!", (TShock.Config.Settings.ForceXmas ? "en" : "dis"));
//			else
//				TSPlayer.All.SendInfoMessage("{0} {1}abled Christmas mode!", args.Player.Name, (TShock.Config.Settings.ForceXmas ? "en" : "dis"));
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

//		static readonly List<string> _validEvents = new List<string>()
//		{
//			"meteor",
//			"fullmoon",
//			"bloodmoon",
//			"eclipse",
//			"invasion",
//			"sandstorm",
//			"rain",
//			"lanternsnight"
//		};
//		static readonly List<string> _validInvasions = new List<string>()
//		{
//			"goblins",
//			"snowmen",
//			"pirates",
//			"pumpkinmoon",
//			"frostmoon",
//					  "martians"
//		};

//		private static void ManageWorldEvent(CommandArgs args)
//		{
//			if (args.Parameters.Count < 1)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}worldevent <event type>", Specifier);
//				args.Player.SendErrorMessage("Valid event types: {0}", String.Join(", ", _validEvents));
//				args.Player.SendErrorMessage("Valid invasion types if spawning an invasion: {0}", String.Join(", ", _validInvasions));
//				return;
//			}

//			var eventType = args.Parameters[0].ToLowerInvariant();

//			void FailedPermissionCheck()
//			{
//				args.Player.SendErrorMessage("You do not have permission to start the {0} event.", eventType);
//				return;
//			}

//			switch (eventType)
//			{
//				case "meteor":
//					if (!args.Player.HasPermission(Permissions.dropmeteor) && !args.Player.HasPermission(Permissions.managemeteorevent))
//					{
//						FailedPermissionCheck();
//						return;
//					}

//					DropMeteor(args);
//					return;

//				case "fullmoon":
//				case "full moon":
//					if (!args.Player.HasPermission(Permissions.fullmoon) && !args.Player.HasPermission(Permissions.managefullmoonevent))
//					{
//						FailedPermissionCheck();
//						return;
//					}
//					Fullmoon(args);
//					return;

//				case "bloodmoon":
//				case "blood moon":
//					if (!args.Player.HasPermission(Permissions.bloodmoon) && !args.Player.HasPermission(Permissions.managebloodmoonevent))
//					{
//						FailedPermissionCheck();
//						return;
//					}
//					Bloodmoon(args);
//					return;

//				case "eclipse":
//					if (!args.Player.HasPermission(Permissions.eclipse) && !args.Player.HasPermission(Permissions.manageeclipseevent))
//					{
//						FailedPermissionCheck();
//						return;
//					}
//					Eclipse(args);
//					return;

//				case "invade":
//				case "invasion":
//					if (!args.Player.HasPermission(Permissions.invade) && !args.Player.HasPermission(Permissions.manageinvasionevent))
//					{
//						FailedPermissionCheck();
//						return;
//					}
//					Invade(args);
//					return;

//				case "sandstorm":
//					if (!args.Player.HasPermission(Permissions.sandstorm) && !args.Player.HasPermission(Permissions.managesandstormevent))
//					{
//						FailedPermissionCheck();
//						return;
//					}
//					Sandstorm(args);
//					return;

//				case "rain":
//					if (!args.Player.HasPermission(Permissions.rain) && !args.Player.HasPermission(Permissions.managerainevent))
//					{
//						FailedPermissionCheck();
//						return;
//					}
//					Rain(args);
//					return;

//				case "lanternsnight":
//				case "lanterns":
//					if (!args.Player.HasPermission(Permissions.managelanternsnightevent))
//					{
//						FailedPermissionCheck();
//						return;
//					}
//					LanternsNight(args);
//					return;

//				default:
//					args.Player.SendErrorMessage("Invalid event type! Valid event types: {0}", String.Join(", ", _validEvents));
//					return;
//			}
//		}

//		private static void DropMeteor(CommandArgs args)
//		{
//			WorldGen.spawnMeteor = false;
//			WorldGen.dropMeteor();
//			if (args.Silent)
//			{
//				args.Player.SendInfoMessage("A meteor has been triggered.");
//			}
//			else
//			{
//				TSPlayer.All.SendInfoMessage("{0} triggered a meteor.", args.Player.Name);
//			}
//		}

//		private static void Fullmoon(CommandArgs args)
//		{
//			TSPlayer.Server.SetFullMoon();
//			if (args.Silent)
//			{
//				args.Player.SendInfoMessage("Started a full moon.");
//			}
//			else
//			{
//				TSPlayer.All.SendInfoMessage("{0} started a full moon.", args.Player.Name);
//			}
//		}

//		private static void Bloodmoon(CommandArgs args)
//		{
//			TSPlayer.Server.SetBloodMoon(!Main.bloodMoon);
//			if (args.Silent)
//			{
//				args.Player.SendInfoMessage("{0}ed a blood moon.", Main.bloodMoon ? "start" : "stopp");
//			}
//			else
//			{
//				TSPlayer.All.SendInfoMessage("{0} {1}ed a blood moon.", args.Player.Name, Main.bloodMoon ? "start" : "stopp");
//			}
//		}

//		private static void Eclipse(CommandArgs args)
//		{
//			TSPlayer.Server.SetEclipse(!Main.eclipse);
//			if (args.Silent)
//			{
//				args.Player.SendInfoMessage("{0}ed an eclipse.", Main.eclipse ? "start" : "stopp");
//			}
//			else
//			{
//				TSPlayer.All.SendInfoMessage("{0} {1}ed an eclipse.", args.Player.Name, Main.eclipse ? "start" : "stopp");
//			}
//		}

//		private static void Invade(CommandArgs args)
//		{
//			if (Main.invasionSize <= 0)
//			{
//				if (args.Parameters.Count < 2)
//				{
//					args.Player.SendErrorMessage("Invalid syntax! Proper syntax:  {0}worldevent invasion [invasion type] [invasion wave]", Specifier);
//					args.Player.SendErrorMessage("Valid invasion types: {0}", String.Join(", ", _validInvasions));
//					return;
//				}

//				int wave = 1;
//				switch (args.Parameters[1].ToLowerInvariant())
//				{
//					case "goblin":
//					case "goblins":
//						TSPlayer.All.SendInfoMessage("{0} has started a goblin army invasion.", args.Player.Name);
//						TShock.Utils.StartInvasion(1);
//						break;

//					case "snowman":
//					case "snowmen":
//						TSPlayer.All.SendInfoMessage("{0} has started a snow legion invasion.", args.Player.Name);
//						TShock.Utils.StartInvasion(2);
//						break;

//					case "pirate":
//					case "pirates":
//						TSPlayer.All.SendInfoMessage("{0} has started a pirate invasion.", args.Player.Name);
//						TShock.Utils.StartInvasion(3);
//						break;

//					case "pumpkin":
//					case "pumpkinmoon":
//						if (args.Parameters.Count > 2)
//						{
//							if (!int.TryParse(args.Parameters[2], out wave) || wave <= 0)
//							{
//								args.Player.SendErrorMessage("Invalid wave!");
//								break;
//							}
//						}

//						TSPlayer.Server.SetPumpkinMoon(true);
//						Main.bloodMoon = false;
//						NPC.waveKills = 0f;
//						NPC.waveNumber = wave;
//						TSPlayer.All.SendInfoMessage("{0} started the pumpkin moon at wave {1}!", args.Player.Name, wave);
//						break;

//					case "frost":
//					case "frostmoon":
//						if (args.Parameters.Count > 2)
//						{
//							if (!int.TryParse(args.Parameters[2], out wave) || wave <= 0)
//							{
//								args.Player.SendErrorMessage("Invalid wave!");
//								return;
//							}
//						}

//						TSPlayer.Server.SetFrostMoon(true);
//						Main.bloodMoon = false;
//						NPC.waveKills = 0f;
//						NPC.waveNumber = wave;
//						TSPlayer.All.SendInfoMessage("{0} started the frost moon at wave {1}!", args.Player.Name, wave);
//						break;

//					case "martian":
//					case "martians":
//						TSPlayer.All.SendInfoMessage("{0} has started a martian invasion.", args.Player.Name);
//						TShock.Utils.StartInvasion(4);
//						break;

//					default:
//						args.Player.SendErrorMessage("Invalid invasion type! Valid invasion types: {0}", String.Join(", ", _validInvasions));
//						break;
//				}
//			}
//			else if (DD2Event.Ongoing)
//			{
//				DD2Event.StopInvasion();
//				TSPlayer.All.SendInfoMessage("{0} has ended the Old One's Army event.", args.Player.Name);
//			}
//			else
//			{
//				TSPlayer.All.SendInfoMessage("{0} has ended the invasion.", args.Player.Name);
//				Main.invasionSize = 0;
//			}
//		}

//		private static void Sandstorm(CommandArgs args)
//		{
//			if (Terraria.GameContent.Events.Sandstorm.Happening)
//			{
//				Terraria.GameContent.Events.Sandstorm.StopSandstorm();
//				TSPlayer.All.SendInfoMessage("{0} stopped the sandstorm.", args.Player.Name);
//			}
//			else
//			{
//				Terraria.GameContent.Events.Sandstorm.StartSandstorm();
//				TSPlayer.All.SendInfoMessage("{0} started a sandstorm.", args.Player.Name);
//			}
//		}

//		private static void Rain(CommandArgs args)
//		{
//			bool slime = false;
//			if (args.Parameters.Count > 1 && args.Parameters[1].ToLowerInvariant() == "slime")
//			{
//				slime = true;
//			}

//			if (!slime)
//			{
//				args.Player.SendInfoMessage("Use \"{0}worldevent rain slime\" to start slime rain!", Specifier);
//			}

//			if (slime && Main.raining) //Slime rain cannot be activated during normal rain
//			{
//				args.Player.SendErrorMessage("You should stop the current downpour before beginning a slimier one!");
//				return;
//			}

//			if (slime && Main.slimeRain) //Toggle slime rain off
//			{
//				Main.StopSlimeRain(false);
//				TSPlayer.All.SendData(PacketTypes.WorldInfo);
//				TSPlayer.All.SendInfoMessage("{0} ended the slimey downpour.", args.Player.Name);
//				return;
//			}

//			if (slime && !Main.slimeRain) //Toggle slime rain on
//			{
//				Main.StartSlimeRain(false);
//				TSPlayer.All.SendData(PacketTypes.WorldInfo);
//				TSPlayer.All.SendInfoMessage("{0} caused it to rain slime.", args.Player.Name);
//			}

//			if (Main.raining && !slime) //Toggle rain off
//			{
//				Main.StopRain();
//				TSPlayer.All.SendData(PacketTypes.WorldInfo);
//				TSPlayer.All.SendInfoMessage("{0} ended the downpour.", args.Player.Name);
//				return;
//			}

//			if (!Main.raining && !slime) //Toggle rain on
//			{
//				Main.StartRain();
//				TSPlayer.All.SendData(PacketTypes.WorldInfo);
//				TSPlayer.All.SendInfoMessage("{0} caused it to rain.", args.Player.Name);
//				return;
//			}
//		}

//		private static void LanternsNight(CommandArgs args)
//		{
//			LanternNight.ToggleManualLanterns();
//			string msg = $" st{(LanternNight.LanternsUp ? "art" : "opp")}ed a lantern night.";
//			if (args.Silent)
//			{
//				args.Player.SendInfoMessage("You" + msg);
//			}
//			else
//			{
//				TSPlayer.All.SendInfoMessage(args.Player.Name + msg);
//			}
//		}

//		static Dictionary<string, int> _worldModes = new Dictionary<string, int>
//		{
//			{ "normal",    0 },
//			{ "expert",    1 },
//			{ "master",    2 },
//			{ "journey",   3 },
//			{ "creative",  3 }
//		};

//		private static void ChangeWorldMode(CommandArgs args)
//		{
//			if (args.Parameters.Count < 1)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}worldmode <mode>", Specifier);
//				args.Player.SendErrorMessage("Valid modes: {0}", String.Join(", ", _worldModes.Keys));
//				return;
//			}

//			int mode;

//			if (int.TryParse(args.Parameters[0], out mode))
//			{
//				if (mode < 0 || mode > 3)
//				{
//					args.Player.SendErrorMessage("Invalid mode! Valid modes: {0}", String.Join(", ", _worldModes.Keys));
//					return;
//				}
//			}
//			else if (_worldModes.ContainsKey(args.Parameters[0].ToLowerInvariant()))
//			{
//				mode = _worldModes[args.Parameters[0].ToLowerInvariant()];
//			}
//			else
//			{
//				args.Player.SendErrorMessage("Invalid mode! Valid modes: {0}", String.Join(", ", _worldModes.Keys));
//				return;
//			}

//			Main.GameMode = mode;
//			args.Player.SendSuccessMessage("World mode set to {0}", _worldModes.Keys.ElementAt(mode));
//			TSPlayer.All.SendData(PacketTypes.WorldInfo);
//		}

//		private static void Hardmode(CommandArgs args)
//		{
//			if (Main.hardMode)
//			{
//				Main.hardMode = false;
//				TSPlayer.All.SendData(PacketTypes.WorldInfo);
//				args.Player.SendSuccessMessage("Hardmode is now off.");
//			}
//			else if (!TShock.Config.Settings.DisableHardmode)
//			{
//				WorldGen.StartHardmode();
//				args.Player.SendSuccessMessage("Hardmode is now on.");
//			}
//			else
//			{
//				args.Player.SendErrorMessage("Hardmode is disabled via config.");
//			}
//		}


//		#endregion Cause Events and Spawn Monsters Commands

//		#region Teleport Commands

//		private static void Home(CommandArgs args)
//		{
//			if (args.Player.Dead)
//			{
//				args.Player.SendErrorMessage("You are dead.");
//				return;
//			}
//			args.Player.Spawn(PlayerSpawnContext.RecallFromItem);
//			args.Player.SendSuccessMessage("Teleported to your spawnpoint.");
//		}

//		private static void Spawn(CommandArgs args)
//		{
//			if (args.Player.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48))
//				args.Player.SendSuccessMessage("Teleported to the map's spawnpoint.");
//		}

//		private static void TP(CommandArgs args)
//		{
//			if (args.Parameters.Count != 1 && args.Parameters.Count != 2)
//			{
//				if (args.Player.HasPermission(Permissions.tpothers))
//					args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tp <player> [player 2]", Specifier);
//				else
//					args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tp <player>", Specifier);
//				return;
//			}

//			if (args.Parameters.Count == 1)
//			{
//				var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
//				if (players.Count == 0)
//					args.Player.SendErrorMessage("Invalid player!");
//				else if (players.Count > 1)
//					args.Player.SendMultipleMatchError(players.Select(p => p.Name));
//				else
//				{
//					var target = players[0];
//					if (!target.TPAllow && !args.Player.HasPermission(Permissions.tpoverride))
//					{
//						args.Player.SendErrorMessage("{0} has disabled players from teleporting.", target.Name);
//						return;
//					}
//					if (args.Player.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
//					{
//						args.Player.SendSuccessMessage("Teleported to {0}.", target.Name);
//						if (!args.Player.HasPermission(Permissions.tpsilent))
//							target.SendInfoMessage("{0} teleported to you.", args.Player.Name);
//					}
//				}
//			}
//			else
//			{
//				if (!args.Player.HasPermission(Permissions.tpothers))
//				{
//					args.Player.SendErrorMessage("You do not have access to this command.");
//					return;
//				}

//				var players1 = TSPlayer.FindByNameOrID(args.Parameters[0]);
//				var players2 = TSPlayer.FindByNameOrID(args.Parameters[1]);

//				if (players2.Count == 0)
//					args.Player.SendErrorMessage("Invalid player!");
//				else if (players2.Count > 1)
//					args.Player.SendMultipleMatchError(players2.Select(p => p.Name));
//				else if (players1.Count == 0)
//				{
//					if (args.Parameters[0] == "*")
//					{
//						if (!args.Player.HasPermission(Permissions.tpallothers))
//						{
//							args.Player.SendErrorMessage("You do not have access to this command.");
//							return;
//						}

//						var target = players2[0];
//						foreach (var source in TShock.Players.Where(p => p != null && p != args.Player))
//						{
//							if (!target.TPAllow && !args.Player.HasPermission(Permissions.tpoverride))
//								continue;
//							if (source.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
//							{
//								if (args.Player != source)
//								{
//									if (args.Player.HasPermission(Permissions.tpsilent))
//										source.SendSuccessMessage("You were teleported to {0}.", target.Name);
//									else
//										source.SendSuccessMessage("{0} teleported you to {1}.", args.Player.Name, target.Name);
//								}
//								if (args.Player != target)
//								{
//									if (args.Player.HasPermission(Permissions.tpsilent))
//										target.SendInfoMessage("{0} was teleported to you.", source.Name);
//									if (!args.Player.HasPermission(Permissions.tpsilent))
//										target.SendInfoMessage("{0} teleported {1} to you.", args.Player.Name, source.Name);
//								}
//							}
//						}
//						args.Player.SendSuccessMessage("Teleported everyone to {0}.", target.Name);
//					}
//					else
//						args.Player.SendErrorMessage("Invalid player!");
//				}
//				else if (players1.Count > 1)
//					args.Player.SendMultipleMatchError(players1.Select(p => p.Name));
//				else
//				{
//					var source = players1[0];
//					if (!source.TPAllow && !args.Player.HasPermission(Permissions.tpoverride))
//					{
//						args.Player.SendErrorMessage("{0} has disabled players from teleporting.", source.Name);
//						return;
//					}
//					var target = players2[0];
//					if (!target.TPAllow && !args.Player.HasPermission(Permissions.tpoverride))
//					{
//						args.Player.SendErrorMessage("{0} has disabled players from teleporting.", target.Name);
//						return;
//					}
//					args.Player.SendSuccessMessage("Teleported {0} to {1}.", source.Name, target.Name);
//					if (source.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
//					{
//						if (args.Player != source)
//						{
//							if (args.Player.HasPermission(Permissions.tpsilent))
//								source.SendSuccessMessage("You were teleported to {0}.", target.Name);
//							else
//								source.SendSuccessMessage("{0} teleported you to {1}.", args.Player.Name, target.Name);
//						}
//						if (args.Player != target)
//						{
//							if (args.Player.HasPermission(Permissions.tpsilent))
//								target.SendInfoMessage("{0} was teleported to you.", source.Name);
//							if (!args.Player.HasPermission(Permissions.tpsilent))
//								target.SendInfoMessage("{0} teleported {1} to you.", args.Player.Name, source.Name);
//						}
//					}
//				}
//			}
//		}

//		private static void TPHere(CommandArgs args)
//		{
//			if (args.Parameters.Count < 1)
//			{
//				if (args.Player.HasPermission(Permissions.tpallothers))
//					args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tphere <player|*>", Specifier);
//				else
//					args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tphere <player>", Specifier);
//				return;
//			}

//			string playerName = String.Join(" ", args.Parameters);
//			var players = TSPlayer.FindByNameOrID(playerName);
//			if (players.Count == 0)
//			{
//				if (playerName == "*")
//				{
//					if (!args.Player.HasPermission(Permissions.tpallothers))
//					{
//						args.Player.SendErrorMessage("You do not have permission to use this command.");
//						return;
//					}
//					for (int i = 0; i < Main.maxPlayers; i++)
//					{
//						if (Main.player[i].active && (Main.player[i] != args.TPlayer))
//						{
//							if (TShock.Players[i].Teleport(args.TPlayer.position.X, args.TPlayer.position.Y))
//								TShock.Players[i].SendSuccessMessage(String.Format("You were teleported to {0}.", args.Player.Name));
//						}
//					}
//					args.Player.SendSuccessMessage("Teleported everyone to yourself.");
//				}
//				else
//					args.Player.SendErrorMessage("Invalid player!");
//			}
//			else if (players.Count > 1)
//				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
//			else
//			{
//				var plr = players[0];
//				if (plr.Teleport(args.TPlayer.position.X, args.TPlayer.position.Y))
//				{
//					plr.SendInfoMessage("You were teleported to {0}.", args.Player.Name);
//					args.Player.SendSuccessMessage("Teleported {0} to yourself.", plr.Name);
//				}
//			}
//		}

//		private static void TPNpc(CommandArgs args)
//		{
//			if (args.Parameters.Count < 1)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tpnpc <NPC>", Specifier);
//				return;
//			}

//			var npcStr = string.Join(" ", args.Parameters);
//			var matches = new List<NPC>();
//			foreach (var npc in Main.npc.Where(npc => npc.active))
//			{
//				var englishName = EnglishLanguage.GetNpcNameById(npc.netID);

//				if (string.Equals(npc.FullName, npcStr, StringComparison.InvariantCultureIgnoreCase) ||
//					string.Equals(englishName, npcStr, StringComparison.InvariantCultureIgnoreCase))
//				{
//					matches = new List<NPC> { npc };
//					break;
//				}
//				if (npc.FullName.ToLowerInvariant().StartsWith(npcStr.ToLowerInvariant()) ||
//					englishName?.StartsWith(npcStr, StringComparison.InvariantCultureIgnoreCase) == true)
//					matches.Add(npc);
//			}

//			if (matches.Count > 1)
//			{
//				args.Player.SendMultipleMatchError(matches.Select(n => $"{n.FullName}({n.whoAmI})"));
//				return;
//			}
//			if (matches.Count == 0)
//			{
//				args.Player.SendErrorMessage("Invalid NPC!");
//				return;
//			}

//			var target = matches[0];
//			args.Player.Teleport(target.position.X, target.position.Y);
//			args.Player.SendSuccessMessage("Teleported to the '{0}'.", target.FullName);
//		}

//		private static void GetPos(CommandArgs args)
//		{
//			var player = args.Player.Name;
//			if (args.Parameters.Count > 0)
//			{
//				player = String.Join(" ", args.Parameters);
//			}

//			var players = TSPlayer.FindByNameOrID(player);
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
//				args.Player.SendSuccessMessage("Location of {0} is ({1}, {2}).", players[0].Name, players[0].TileX, players[0].TileY);
//			}
//		}

//		private static void TPPos(CommandArgs args)
//		{
//			if (args.Parameters.Count != 2)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}tppos <tile x> <tile y>", Specifier);
//				return;
//			}

//			int x, y;
//			if (!int.TryParse(args.Parameters[0], out x) || !int.TryParse(args.Parameters[1], out y))
//			{
//				args.Player.SendErrorMessage("Invalid tile positions!");
//				return;
//			}
//			x = Math.Max(0, x);
//			y = Math.Max(0, y);
//			x = Math.Min(x, Main.maxTilesX - 1);
//			y = Math.Min(y, Main.maxTilesY - 1);

//			args.Player.Teleport(16 * x, 16 * y);
//			args.Player.SendSuccessMessage("Teleported to {0}, {1}!", x, y);
//		}

//		private static void TPAllow(CommandArgs args)
//		{
//			if (!args.Player.TPAllow)
//				args.Player.SendSuccessMessage("You have removed your teleportation protection.");
//			if (args.Player.TPAllow)
//				args.Player.SendSuccessMessage("You have enabled teleportation protection.");
//			args.Player.TPAllow = !args.Player.TPAllow;
//		}


//		#endregion Teleport Commands

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

//		private static void SetSpawn(CommandArgs args)
//		{
//			Main.spawnTileX = args.Player.TileX + 1;
//			Main.spawnTileY = args.Player.TileY + 3;
//			SaveManager.Instance.SaveWorld(false);
//			args.Player.SendSuccessMessage("Spawn has now been set at your location.");
//		}

//		private static void SetDungeon(CommandArgs args)
//		{
//			Main.dungeonX = args.Player.TileX + 1;
//			Main.dungeonY = args.Player.TileY + 3;
//			SaveManager.Instance.SaveWorld(false);
//			args.Player.SendSuccessMessage("The dungeon's position has now been set at your location.");
//		}

//		private static void Save(CommandArgs args)
//		{
//			SaveManager.Instance.SaveWorld(false);
//			foreach (TSPlayer tsply in TShock.Players.Where(tsply => tsply != null))
//			{
//				tsply.SaveServerCharacter();
//			}
//		}

//		private static void Settle(CommandArgs args)
//		{
//			if (Liquid.panicMode)
//			{
//				args.Player.SendWarningMessage("Liquids are already settling!");
//				return;
//			}
//			Liquid.StartPanic();
//			args.Player.SendInfoMessage("Settling liquids.");
//		}

//		#endregion Server Config Commands

//		#region Time/PvpFun Commands

//		private static void Time(CommandArgs args)
//		{
//			if (args.Parameters.Count == 0)
//			{
//				double time = Main.time / 3600.0;
//				time += 4.5;
//				if (!Main.dayTime)
//					time += 15.0;
//				time = time % 24.0;
//				args.Player.SendInfoMessage("The current time is {0}:{1:D2}.", (int)Math.Floor(time), (int)Math.Floor((time % 1.0) * 60.0));
//				return;
//			}

//			switch (args.Parameters[0].ToLower())
//			{
//				case "day":
//					TSPlayer.Server.SetTime(true, 0.0);
//					TSPlayer.All.SendInfoMessage("{0} set the time to 4:30.", args.Player.Name);
//					break;
//				case "night":
//					TSPlayer.Server.SetTime(false, 0.0);
//					TSPlayer.All.SendInfoMessage("{0} set the time to 19:30.", args.Player.Name);
//					break;
//				case "noon":
//					TSPlayer.Server.SetTime(true, 27000.0);
//					TSPlayer.All.SendInfoMessage("{0} set the time to 12:00.", args.Player.Name);
//					break;
//				case "midnight":
//					TSPlayer.Server.SetTime(false, 16200.0);
//					TSPlayer.All.SendInfoMessage("{0} set the time to 0:00.", args.Player.Name);
//					break;
//				default:
//					string[] array = args.Parameters[0].Split(':');
//					if (array.Length != 2)
//					{
//						args.Player.SendErrorMessage("Invalid time string! Proper format: hh:mm, in 24-hour time.");
//						return;
//					}

//					int hours;
//					int minutes;
//					if (!int.TryParse(array[0], out hours) || hours < 0 || hours > 23
//						|| !int.TryParse(array[1], out minutes) || minutes < 0 || minutes > 59)
//					{
//						args.Player.SendErrorMessage("Invalid time string! Proper format: hh:mm, in 24-hour time.");
//						return;
//					}

//					decimal time = hours + (minutes / 60.0m);
//					time -= 4.50m;
//					if (time < 0.00m)
//						time += 24.00m;

//					if (time >= 15.00m)
//					{
//						TSPlayer.Server.SetTime(false, (double)((time - 15.00m) * 3600.0m));
//					}
//					else
//					{
//						TSPlayer.Server.SetTime(true, (double)(time * 3600.0m));
//					}
//					TSPlayer.All.SendInfoMessage("{0} set the time to {1}:{2:D2}.", args.Player.Name, hours, minutes);
//					break;
//			}
//		}


//		private static void Wind(CommandArgs args)
//		{
//			if (args.Parameters.Count != 1)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}wind <speed>", Specifier);
//				return;
//			}

//			int speed;
//			if (!int.TryParse(args.Parameters[0], out speed) || speed * 100 < 0)
//			{
//				args.Player.SendErrorMessage("Invalid wind speed!");
//				return;
//			}

//			Main.windSpeedCurrent = speed;
//			Main.windSpeedTarget = speed;
//			TSPlayer.All.SendData(PacketTypes.WorldInfo);
//			TSPlayer.All.SendInfoMessage("{0} changed the wind speed to {1}.", args.Player.Name, speed);
//		}

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

//		private static void ToggleAntiBuild(CommandArgs args)
//		{
//			TShock.Config.Settings.DisableBuild = !TShock.Config.Settings.DisableBuild;
//			TSPlayer.All.SendSuccessMessage(string.Format("Anti-build is now {0}.", (TShock.Config.Settings.DisableBuild ? "on" : "off")));
//		}

//		private static void ProtectSpawn(CommandArgs args)
//		{
//			TShock.Config.Settings.SpawnProtection = !TShock.Config.Settings.SpawnProtection;
//			TSPlayer.All.SendSuccessMessage(string.Format("Spawn is now {0}.", (TShock.Config.Settings.SpawnProtection ? "protected" : "open")));
//		}

//		#endregion World Protection Commands

//		#region General Commands

//		private static void Help(CommandArgs args)
//		{
//			if (args.Parameters.Count > 1)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}help <command/page>", Specifier);
//				return;
//			}

//			int pageNumber;
//			if (args.Parameters.Count == 0 || int.TryParse(args.Parameters[0], out pageNumber))
//			{
//				if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out pageNumber))
//				{
//					return;
//				}

//				IEnumerable<string> cmdNames = from cmd in ChatCommands
//											   where cmd.CanRun(args.Player) && (cmd.Name != "setup" || TShock.SetupToken != 0)
//											   select Specifier + cmd.Name;

//				PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(cmdNames),
//					new PaginationTools.Settings
//					{
//						HeaderFormat = "Commands ({0}/{1}):",
//						FooterFormat = "Type {0}help {{0}} for more.".SFormat(Specifier)
//					});
//			}
//			else
//			{
//				string commandName = args.Parameters[0].ToLower();
//				if (commandName.StartsWith(Specifier))
//				{
//					commandName = commandName.Substring(1);
//				}

//				Command command = ChatCommands.Find(c => c.Names.Contains(commandName));
//				if (command == null)
//				{
//					args.Player.SendErrorMessage("Invalid command.");
//					return;
//				}
//				if (!command.CanRun(args.Player))
//				{
//					args.Player.SendErrorMessage("You do not have access to this command.");
//					return;
//				}

//				args.Player.SendSuccessMessage("{0}{1} help: ", Specifier, command.Name);
//				if (command.HelpDesc == null)
//				{
//					args.Player.SendInfoMessage(command.HelpText);
//					return;
//				}
//				foreach (string line in command.HelpDesc)
//				{
//					args.Player.SendInfoMessage(line);
//				}
//			}
//		}


//		private static void ListConnectedPlayers(CommandArgs args)
//		{
//			bool invalidUsage = (args.Parameters.Count > 2);

//			bool displayIdsRequested = false;
//			int pageNumber = 1;
//			if (!invalidUsage)
//			{
//				foreach (string parameter in args.Parameters)
//				{
//					if (parameter.Equals("-i", StringComparison.InvariantCultureIgnoreCase))
//					{
//						displayIdsRequested = true;
//						continue;
//					}

//					if (!int.TryParse(parameter, out pageNumber))
//					{
//						invalidUsage = true;
//						break;
//					}
//				}
//			}
//			if (invalidUsage)
//			{
//				args.Player.SendMessage($"List Online Players Syntax", Color.White);
//				args.Player.SendMessage($"{"playing".Color(Utils.BoldHighlight)} {"[-i]".Color(Utils.RedHighlight)} {"[page]".Color(Utils.GreenHighlight)}", Color.White);
//				args.Player.SendMessage($"Command aliases: {"playing".Color(Utils.GreenHighlight)}, {"online".Color(Utils.GreenHighlight)}, {"who".Color(Utils.GreenHighlight)}", Color.White);
//				args.Player.SendMessage($"Example usage: {"who".Color(Utils.BoldHighlight)} {"-i".Color(Utils.RedHighlight)}", Color.White);
//				return;
//			}
//			if (displayIdsRequested && !args.Player.HasPermission(Permissions.seeids))
//			{
//				args.Player.SendErrorMessage("You do not have permission to see player IDs.");
//				return;
//			}

//			if (TShock.Utils.GetActivePlayerCount() == 0)
//			{
//				args.Player.SendMessage("There are currently no players online.", Color.White);
//				return;
//			}
//			args.Player.SendMessage($"Online Players ({TShock.Utils.GetActivePlayerCount().Color(Utils.GreenHighlight)}/{TShock.Config.Settings.MaxSlots})", Color.White);

//			var players = new List<string>();

//			foreach (TSPlayer ply in TShock.Players)
//			{
//				if (ply != null && ply.Active)
//				{
//					if (displayIdsRequested)
//						players.Add($"{ply.Name} (Index: {ply.Index}{(ply.Account != null ? ", Account ID: " + ply.Account.ID : "")})");
//					else
//						players.Add(ply.Name);
//				}
//			}

//			PaginationTools.SendPage(
//				args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(players),
//				new PaginationTools.Settings
//				{
//					IncludeHeader = false,
//					FooterFormat = $"Type {Specifier}who {(displayIdsRequested ? "-i" : string.Empty)}{Specifier} for more."
//				}
//			);
//		}

//		private static void SetupToken(CommandArgs args)
//		{
//			if (TShock.SetupToken == 0)
//			{
//				args.Player.SendWarningMessage("The initial setup system is disabled. This incident has been logged.");
//				args.Player.SendWarningMessage("If you are locked out of all admin accounts, ask for help on https://tshock.co/");
//				TShock.Log.Warn("{0} attempted to use the initial setup system even though it's disabled.", args.Player.IP);
//				return;
//			}

//			// If the user account is already logged in, turn off the setup system
//			if (args.Player.IsLoggedIn && args.Player.tempGroup == null)
//			{
//				args.Player.SendSuccessMessage("Your new account has been verified, and the {0}setup system has been turned off.", Specifier);
//				args.Player.SendSuccessMessage("Share your server, talk with admins, and chill on GitHub & Discord. -- https://tshock.co/");
//				args.Player.SendSuccessMessage("Thank you for using TShock for Terraria!");
//				FileTools.CreateFile(Path.Combine(TShock.SavePath, "setup.lock"));
//				File.Delete(Path.Combine(TShock.SavePath, "setup-code.txt"));
//				TShock.SetupToken = 0;
//				return;
//			}

//			if (args.Parameters.Count == 0)
//			{
//				args.Player.SendErrorMessage("You must provide a setup code!");
//				return;
//			}

//			int givenCode;
//			if (!Int32.TryParse(args.Parameters[0], out givenCode) || givenCode != TShock.SetupToken)
//			{
//				args.Player.SendErrorMessage("Incorrect setup code. This incident has been logged.");
//				TShock.Log.Warn(args.Player.IP + " attempted to use an incorrect setup code.");
//				return;
//			}

//			if (args.Player.Group.Name != "superadmin")
//				args.Player.tempGroup = new SuperAdminGroup();

//			args.Player.SendInfoMessage("Temporary system access has been given to you, so you can run one command.");
//			args.Player.SendWarningMessage("Please use the following to create a permanent account for you.");
//			args.Player.SendWarningMessage("{0}user add <username> <password> owner", Specifier);
//			args.Player.SendInfoMessage("Creates: <username> with the password <password> as part of the owner group.");
//			args.Player.SendInfoMessage("Please use {0}login <username> <password> after this process.", Specifier);
//			args.Player.SendWarningMessage("If you understand, please {0}login <username> <password> now, and then type {0}setup.", Specifier);
//			return;
//		}



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

//		private static void Motd(CommandArgs args)
//		{
//			args.Player.SendFileTextAsMessage(FileTools.MotdPath);
//		}

//		private static void Rules(CommandArgs args)
//		{
//			args.Player.SendFileTextAsMessage(FileTools.RulesPath);
//		}




//		private static void Aliases(CommandArgs args)
//		{
//			if (args.Parameters.Count < 1)
//			{
//				args.Player.SendErrorMessage("Invalid syntax! Proper syntax: {0}aliases <command or alias>", Specifier);
//				return;
//			}

//			string givenCommandName = string.Join(" ", args.Parameters);
//			if (string.IsNullOrWhiteSpace(givenCommandName))
//			{
//				args.Player.SendErrorMessage("Please enter a proper command name or alias.");
//				return;
//			}

//			string commandName;
//			if (givenCommandName[0] == Specifier[0])
//				commandName = givenCommandName.Substring(1);
//			else
//				commandName = givenCommandName;

//			bool didMatch = false;
//			foreach (Command matchingCommand in ChatCommands.Where(cmd => cmd.Names.IndexOf(commandName) != -1))
//			{
//				if (matchingCommand.Names.Count > 1)
//					args.Player.SendInfoMessage(
//						"Aliases of {0}{1}: {0}{2}", Specifier, matchingCommand.Name, string.Join(", {0}".SFormat(Specifier), matchingCommand.Names.Skip(1)));
//				else
//					args.Player.SendInfoMessage("{0}{1} defines no aliases.", Specifier, matchingCommand.Name);

//				didMatch = true;
//			}

//			if (!didMatch)
//				args.Player.SendErrorMessage("No command or command alias matching \"{0}\" found.", givenCommandName);
//		}


//		#endregion General Commands

//		#region Game Commands




//		public static void Grow(CommandArgs args)
//		{
//			bool canGrowEvil = args.Player.HasPermission(Permissions.growevil);
//			string subcmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();

//			var name = "Fail";
//			var x = args.Player.TileX;
//			var y = args.Player.TileY + 3;

//			if (!TShock.Regions.CanBuild(x, y, args.Player))
//			{
//				args.Player.SendErrorMessage("You're not allowed to change tiles here!");
//				return;
//			}

//			switch (subcmd)
//			{
//				case "help":
//					{
//						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
//							return;

//						var lines = new List<string>
//					{
//						"- Default trees :",
//						"     'basic', 'sakura', 'willow', 'boreal', 'mahogany', 'ebonwood', 'shadewood', 'pearlwood'.",
//						"- Palm trees :",
//						"     'palm', 'corruptpalm', 'crimsonpalm', 'hallowpalm'.",
//						"- Gem trees :",
//						"     'topaz', 'amethyst', 'sapphire', 'emerald', 'ruby', 'diamond', 'amber'.",
//						"- Misc :",
//						"     'cactus', 'herb', 'mushroom'."
//					};

//						PaginationTools.SendPage(args.Player, pageNumber, lines,
//								new PaginationTools.Settings
//								{
//									HeaderFormat = "Trees types & misc available to use. ({0}/{1}):",
//									FooterFormat = "Type {0}grow help {{0}} for more sub-commands.".SFormat(Commands.Specifier)
//								}
//							);
//					}
//					break;

//					bool rejectCannotGrowEvil()
//					{
//						if (!canGrowEvil)
//						{
//							args.Player.SendErrorMessage("You do not have permission to grow this tree type");
//							return false;
//						}

//						return true;
//					}

//					bool prepareAreaForGrow(ushort groundType = TileID.Grass, bool evil = false)
//					{
//						if (evil && !rejectCannotGrowEvil())
//							return false;

//						for (var i = x - 2; i < x + 3; i++)
//						{
//							Main.tile[i, y].active(true);
//							Main.tile[i, y].type = groundType;
//							Main.tile[i, y].wall = WallID.None;
//						}
//						Main.tile[x, y - 1].wall = WallID.None;

//						return true;
//					}

//					bool growTree(ushort groundType, string fancyName, bool evil = false)
//					{
//						if (!prepareAreaForGrow(groundType, evil))
//							return false;
//						WorldGen.GrowTree(x, y);
//						name = fancyName;

//						return true;
//					}

//					bool growTreeByType(ushort groundType, string fancyName, ushort typeToPrepare = 2, bool evil = false)
//					{
//						if (!prepareAreaForGrow(typeToPrepare, evil))
//							return false;
//						WorldGen.TryGrowingTreeByType(groundType, x, y);
//						name = fancyName;

//						return true;
//					}

//					bool growPalmTree(ushort sandType, ushort supportingType, string properName, bool evil = false)
//					{
//						if (evil && !rejectCannotGrowEvil())
//							return false;

//						for (int i = x - 2; i < x + 3; i++)
//						{
//							Main.tile[i, y].active(true);
//							Main.tile[i, y].type = sandType;
//							Main.tile[i, y].wall = WallID.None;
//						}
//						for (int i = x - 2; i < x + 3; i++)
//						{
//							Main.tile[i, y + 1].active(true);
//							Main.tile[i, y + 1].type = supportingType;
//							Main.tile[i, y + 1].wall = WallID.None;
//						}

//						Main.tile[x, y - 1].wall = WallID.None;
//						WorldGen.GrowPalmTree(x, y);

//						name = properName;

//						return true;
//					}

//				case "basic":
//					growTree(TileID.Grass, "Basic Tree");
//					break;

//				case "boreal":
//					growTree(TileID.SnowBlock, "Boreal Tree");
//					break;

//				case "mahogany":
//					growTree(TileID.JungleGrass, "Rich Mahogany");
//					break;

//				case "sakura":
//					growTreeByType(TileID.VanityTreeSakura, "Sakura Tree");
//					break;

//				case "willow":
//					growTreeByType(TileID.VanityTreeYellowWillow, "Willow Tree");
//					break;

//				case "shadewood":
//					if (!growTree(TileID.CrimsonGrass, "Shadewood Tree", true))
//						return;
//					break;

//				case "ebonwood":
//					if (!growTree(TileID.CorruptGrass, "Ebonwood Tree", true))
//						return;
//					break;

//				case "pearlwood":
//					if (!growTree(TileID.HallowedGrass, "Pearlwood Tree", true))
//						return;
//					break;

//				case "palm":
//					growPalmTree(TileID.Sand, TileID.HardenedSand, "Desert Palm");
//					break;

//				case "hallowpalm":
//					if (!growPalmTree(TileID.Pearlsand, TileID.HallowHardenedSand, "Hallow Palm", true))
//						return;
//					break;

//				case "crimsonpalm":
//					if (!growPalmTree(TileID.Crimsand, TileID.CrimsonHardenedSand, "Crimson Palm", true))
//						return;
//					break;

//				case "corruptpalm":
//					if (!growPalmTree(TileID.Ebonsand, TileID.CorruptHardenedSand, "Corruption Palm", true))
//						return;
//					break;

//				case "topaz":
//					growTreeByType(TileID.TreeTopaz, "Topaz Gemtree", 1);
//					break;

//				case "amethyst":
//					growTreeByType(TileID.TreeAmethyst, "Amethyst Gemtree", 1);
//					break;

//				case "sapphire":
//					growTreeByType(TileID.TreeSapphire, "Sapphire Gemtree", 1);
//					break;

//				case "emerald":
//					growTreeByType(TileID.TreeEmerald, "Emerald Gemtree", 1);
//					break;

//				case "ruby":
//					growTreeByType(TileID.TreeRuby, "Ruby Gemtree", 1);
//					break;

//				case "diamond":
//					growTreeByType(TileID.TreeDiamond, "Diamond Gemtree", 1);
//					break;

//				case "amber":
//					growTreeByType(TileID.TreeAmber, "Amber Gemtree", 1);
//					break;

//				case "cactus":
//					Main.tile[x, y].type = TileID.Sand;
//					WorldGen.GrowCactus(x, y);
//					name = "Cactus";
//					break;

//				case "herb":
//					Main.tile[x, y].active(true);
//					Main.tile[x, y].frameX = 36;
//					Main.tile[x, y].type = TileID.MatureHerbs;
//					WorldGen.GrowAlch(x, y);
//					name = "Herb";
//					break;

//				case "mushroom":
//					prepareAreaForGrow(TileID.MushroomGrass);
//					WorldGen.GrowShroom(x, y);
//					name = "Glowing Mushroom Tree";
//					break;

//				default:
//					args.Player.SendErrorMessage("Unknown plant!");
//					return;
//			}
//			if (args.Parameters.Count == 1)
//			{
//				args.Player.SendTileSquareCentered(x - 2, y - 20, 25);
//				args.Player.SendSuccessMessage("Tried to grow a " + name + ".");
//			}
//		}


//		#endregion Game Commands
//	}
//}
