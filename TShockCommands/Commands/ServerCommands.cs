/*
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
using EasyCommands;
using EasyCommands.Commands;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using TShockAPI;
using TShockCommands.Annotations;
using Utils = TShockAPI.Utils;

namespace TShockCommands.Commands;

class ServerCommands : CommandCallbacks<TSPlayer>
{
	[Command("serverinfo")]
	[CommandPermissions(Permissions.serverinfo)]
	[HelpText("Shows the server information.")]
	public void ServerInfo()
	{
		Sender.SendInfoMessage("Memory usage: " + Process.GetCurrentProcess().WorkingSet64);
		Sender.SendInfoMessage("Allocated memory: " + Process.GetCurrentProcess().VirtualMemorySize64);
		Sender.SendInfoMessage("Total processor time: " + Process.GetCurrentProcess().TotalProcessorTime);
		Sender.SendInfoMessage("Operating system: " + Environment.OSVersion);
		Sender.SendInfoMessage("Proc count: " + Environment.ProcessorCount);
		Sender.SendInfoMessage("Machine name: " + Environment.MachineName);
	}

	[Command("worldinfo")]
	[CommandPermissions(Permissions.worldinfo)]
	[HelpText("Shows information about the current world.")]
	public void WorldInfo()
	{
		Sender.SendInfoMessage("Information about the currently running world");
		Sender.SendInfoMessage("Name: " + (TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName));
		Sender.SendInfoMessage("Size: {0}x{1}", Main.maxTilesX, Main.maxTilesY);
		Sender.SendInfoMessage("ID: " + Main.worldID);
		Sender.SendInfoMessage("Seed: " + WorldGen.currentWorldSeed);
		Sender.SendInfoMessage("Mode: " + Main.GameMode);
		Sender.SendInfoMessage("Path: " + Main.worldPathName);
	}

	[Command("kick")]
	[CommandPermissions(Permissions.kick)]
	[HelpText("Removes a player from the server.")]
	public void Kick(TSPlayer player, [AllowSpaces] string? reason = null)
	{
		if (player is null)
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}kick <player> [reason]", TextOptions.CommandPrefix);
			return;
		}
		//if (args.Parameters[0].Length == 0)
		//{
		//	Sender.SendErrorMessage("Missing player name.");
		//	return;
		//}

		//string plStr = args.Parameters[0];
		//var players = TSPlayer.FindByNameOrID(plStr);
		//if (players.Count == 0)
		//{
		//	Sender.SendErrorMessage("Invalid player!");
		//}
		//else if (players.Count > 1)
		//{
		//	Sender.SendMultipleMatchError(players.Select(p => p.Name));
		//}
		//else
		{
			//string reason = args.Parameters.Count > 1
			//					? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1))
			//					: "Misbehaviour.";
			reason ??= "Misbehaviour.";
			if (!player.Kick(reason, !Sender.RealPlayer, false, Sender.Name))
			{
				Sender.SendErrorMessage("You can't kick another admin!");
			}
		}
	}

	[Command("mute", "unmute")]
	[CommandPermissions(Permissions.mute)]
	[HelpText("Prevents a player from talking.")]
	public void Mute(TSPlayer player, [AllowSpaces] string? reason = null)
	{
		if (player is null)
		{
			Sender.SendMessage("Mute Syntax", Color.White);
			Sender.SendMessage($"{"mute".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}> [{"reason".Color(Utils.GreenHighlight)}]", Color.White);
			Sender.SendMessage($"Example usage: {"mute".Color(Utils.BoldHighlight)} \"{Sender.Name.Color(Utils.RedHighlight)}\" \"{"No swearing on my Christian server".Color(Utils.GreenHighlight)}\"", Color.White);
			//Sender.SendMessage($"To mute a player without broadcasting to chat, use the command with {SilentTextOptions.CommandPrefix.Color(Utils.GreenHighlight)} instead of {TextOptions.CommandPrefix.Color(Utils.RedHighlight)}", Color.White);
			return;
		}

		//var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
		//if (players.Count == 0)
		//{
		//	Sender.SendErrorMessage($"Could not find any players named \"{args.Parameters[0]}\"");
		//}
		//else if (players.Count > 1)
		//{
		//	Sender.SendMultipleMatchError(players.Select(p => p.Name));
		//}
		//else if (players[0].HasPermission(Permissions.mute))
		//{
		//	Sender.SendErrorMessage($"You do not have permission to mute {players[0].Name}");
		//}
		//else

		if (player.mute)
		{
			//var plr = players[0];
			player.mute = false;
			//if (args.Silent)
			//	Sender.SendSuccessMessage($"You have unmuted {plr.Name}.");
			//else
			TSPlayer.All.SendInfoMessage($"{Sender.Name} has unmuted {player.Name}.");
		}
		else
		{
			//string reason = "No reason specified.";
			//if (args.Parameters.Count > 1)
			//	reason = String.Join(" ", args.Parameters.ToArray(), 1, args.Parameters.Count - 1);
			reason ??= "No reason specified.";
			//var plr = players[0];
			player.mute = true;
			//if (args.Silent)
			//	Sender.SendSuccessMessage($"You have muted {plr.Name} for {reason}");
			//else
			TSPlayer.All.SendInfoMessage($"{Sender.Name} has muted {player.Name} for {reason}.");
		}
	}

	[Command("displaylogs")]
	[CommandPermissions(Permissions.logs)]
	[HelpText("Toggles whether you receive server logs.")]
	public void DisplayLogs()
	{
		Sender.DisplayLogs = (!Sender.DisplayLogs);
		Sender.SendSuccessMessage("You will " + (Sender.DisplayLogs ? "now" : "no longer") + " receive logs.");
	}

	[Command("savessc")]
	[CommandPermissions(Permissions.savessc)]
	[HelpText("Saves all serverside characters.")]
	public void SaveSSC()
	{
		if (Main.ServerSideCharacter)
		{
			Sender.SendSuccessMessage("SSC has been saved.");
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.IsLoggedIn && !player.IsDisabledPendingTrashRemoval)
				{
					TShock.CharacterDB.InsertPlayerData(player, true);
				}
			}
		}
	}

	[Command("overridessc", "ossc")]
	[CommandPermissions(Permissions.savessc)]
	[HelpText("Overrides serverside characters for a player, temporarily.")]
	public void OverrideSSC([AllowSpaces] string playerName)
	{
		if (!Main.ServerSideCharacter)
		{
			Sender.SendErrorMessage("Server Side Characters is disabled.");
			return;
		}
		if (String.IsNullOrWhiteSpace(playerName))
		{
			Sender.SendErrorMessage("Correct usage: {0}overridessc|{0}ossc <player name>", TextOptions.CommandPrefix);
			return;
		}

		//string playerNameToMatch = string.Join(" ", args.Parameters);
		var matchedPlayers = TSPlayer.FindByNameOrID(playerName);
		if (matchedPlayers.Count < 1)
		{
			Sender.SendErrorMessage("No players matched \"{0}\".", playerName);
			return;
		}
		else if (matchedPlayers.Count > 1)
		{
			Sender.SendMultipleMatchError(matchedPlayers.Select(p => p.Name));
			return;
		}

		TSPlayer matchedPlayer = matchedPlayers[0];
		if (matchedPlayer.IsLoggedIn)
		{
			Sender.SendErrorMessage("Player \"{0}\" is already logged in.", matchedPlayer.Name);
			return;
		}
		if (!matchedPlayer.LoginFailsBySsi)
		{
			Sender.SendErrorMessage("Player \"{0}\" has to perform a /login attempt first.", matchedPlayer.Name);
			return;
		}
		if (matchedPlayer.IsDisabledPendingTrashRemoval)
		{
			Sender.SendErrorMessage("Player \"{0}\" has to reconnect first.", matchedPlayer.Name);
			return;
		}

		TShock.CharacterDB.InsertPlayerData(matchedPlayer);
		Sender.SendSuccessMessage("SSC of player \"{0}\" has been overriden.", matchedPlayer.Name);
	}

	[Command("uploadssc")]
	[CommandPermissions(Permissions.uploaddata)]
	[HelpText("Upload the account information when you joined the server as your Server Side Character data.")]
	public void UploadJoinData(string? playerName = null)
	{
		TSPlayer targetPlayer = Sender;
		if (!String.IsNullOrWhiteSpace(playerName) && Sender.HasPermission(Permissions.uploadothersdata))
		{
			List<TSPlayer> players = TSPlayer.FindByNameOrID(playerName);
			if (players.Count > 1)
			{
				Sender.SendMultipleMatchError(players.Select(p => p.Name));
				return;
			}
			else if (players.Count == 0)
			{
				Sender.SendErrorMessage("No player was found matching'{0}'", playerName);
				return;
			}
			else
			{
				targetPlayer = players[0];
			}
		}
		else if (!String.IsNullOrWhiteSpace(playerName))
		{
			Sender.SendErrorMessage("You do not have permission to upload another player's character data.");
			return;
		}
		//else if (args.Parameters.Count > 0)
		//{
		//	Sender.SendErrorMessage("Usage: /uploadssc [playername]");
		//	return;
		//}
		else if (String.IsNullOrWhiteSpace(playerName) && Sender is TSServerPlayer)
		{
			Sender.SendErrorMessage("A console can not upload their player data.");
			Sender.SendErrorMessage("Usage: /uploadssc [playername]");
			return;
		}

		if (targetPlayer.IsLoggedIn)
		{
			if (TShock.CharacterDB.InsertSpecificPlayerData(targetPlayer, targetPlayer.DataWhenJoined))
			{
				targetPlayer.DataWhenJoined.RestoreCharacter(targetPlayer);
				targetPlayer.SendSuccessMessage("Your local character data has been uploaded to the server.");
				Sender.SendSuccessMessage("The player's character data was successfully uploaded.");
			}
			else
			{
				Sender.SendErrorMessage("Failed to upload your character data, are you logged in to an account?");
			}
		}
		else
		{
			Sender.SendErrorMessage("The target player has not logged in yet.");
		}
	}

	[Command("tempgroup")]
	[CommandPermissions(Permissions.settempgroup)]
	[HelpText("Temporarily sets another player's group.")]
	public void TempGroup(string username, string newgroup, int time = 0)
	{
		if (String.IsNullOrWhiteSpace(username) || String.IsNullOrWhiteSpace(newgroup))
		{
			Sender.SendInfoMessage("Invalid usage");
			Sender.SendInfoMessage("Usage: {0}tempgroup <username> <new group> [time]", TextOptions.CommandPrefix);
			return;
		}

		List<TSPlayer> ply = TSPlayer.FindByNameOrID(username);
		if (ply.Count < 1)
		{
			Sender.SendErrorMessage("Could not find player {0}.", username);
			return;
		}

		if (ply.Count > 1)
		{
			Sender.SendMultipleMatchError(ply.Select(p => p.Account.Name));
		}

		if (!TShock.Groups.GroupExists(newgroup))
		{
			Sender.SendErrorMessage("Could not find group {0}", newgroup);
			return;
		}

		if (time > 0)
		{
			//int time;
			//if (!TShock.Utils.TryParseTime(args.Parameters[2], out time))
			//{
			//	Sender.SendErrorMessage("Invalid time string! Proper format: _d_h_m_s, with at least one time TextOptions.CommandPrefix.");
			//	Sender.SendErrorMessage("For example, 1d and 10h-30m+2m are both valid time strings, but 2 is not.");
			//	return;
			//}

			ply[0].tempGroupTimer = new System.Timers.Timer(time * 1000);
			ply[0].tempGroupTimer.Elapsed += ply[0].TempGroupTimerElapsed;
			ply[0].tempGroupTimer.Start();
		}

		Group g = TShock.Groups.GetGroupByName(newgroup);

		ply[0].tempGroup = g;

		if (time <= 0)
		{
			Sender.SendSuccessMessage(String.Format("You have changed {0}'s group to {1}", ply[0].Name, g.Name));
			ply[0].SendSuccessMessage(String.Format("Your group has temporarily been changed to {0}", g.Name));
		}
		else
		{
			Sender.SendSuccessMessage(String.Format("You have changed {0}'s group to {1} for {2}",
				ply[0].Name, g.Name, time));
			ply[0].SendSuccessMessage(String.Format("Your group has been changed to {0} for {1}",
				g.Name, time));
		}
	}

	[Command("su")]
	[CommandPermissions(Permissions.su)]
	[HelpText("Temporarily elevates you to Super Admin.")]
	public void SubstituteUser()
	{
		if (Sender.tempGroup != null)
		{
			Sender.tempGroup = null;
			Sender.tempGroupTimer.Stop();
			Sender.SendSuccessMessage("Your previous permission set has been restored.");
			return;
		}
		else
		{
			Sender.tempGroup = new SuperAdminGroup();
			Sender.tempGroupTimer = new System.Timers.Timer(600 * 1000);
			Sender.tempGroupTimer.Elapsed += Sender.TempGroupTimerElapsed;
			Sender.tempGroupTimer.Start();
			Sender.SendSuccessMessage("Your account has been elevated to Super Admin for 10 minutes.");
			return;
		}
	}

	//// Executes a command as a superuser if you have sudo rights.
	//[Command("sudo")]
	//[CommandPermissions(Permissions.su)]
	//[HelpText("Executes a command as the super admin.")]
	//public void SubstituteUserDo([AllowSpaces] string? command = null)
	//{
	//	if (String.IsNullOrWhiteSpace(command))
	//	{
	//		Sender.SendErrorMessage("Usage: /sudo [command].");
	//		Sender.SendErrorMessage("Example: /sudo /ban add Shank 2d Hacking.");
	//		return;
	//	}

	//	//string replacementCommand = String.Join(" ", args.Parameters.Select(p => p.Contains(" ") ? $"\"{p}\"" : p));
	//	Sender.tempGroup = new SuperAdminGroup();
	//	HandleCommand(Sender, replacementCommand);
	//	Sender.tempGroup = null;
	//	return;
	//}

	[Command("broadcast", "bc", "say")]
	[CommandPermissions(Permissions.broadcast)]
	[HelpText("Broadcasts a message to everyone on the server.")]
	public void Broadcast([AllowSpaces] string message)
	{
		//string message = string.Join(" ", args.Parameters);
		TShock.Utils.Broadcast(
			"(Server Broadcast) " + message,
			Convert.ToByte(TShock.Config.Settings.BroadcastRGB[0]), Convert.ToByte(TShock.Config.Settings.BroadcastRGB[1]),
			Convert.ToByte(TShock.Config.Settings.BroadcastRGB[2]));
	}
}
