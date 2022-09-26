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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EasyCommands;
using EasyCommands.Commands;
using Microsoft.Xna.Framework;
using TShockAPI;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

class TShockCommands : CommandCallbacks<TSPlayer>
{
	[Command("setup")]
	[HelpText("Used to authenticate as superadmin when first setting up TShock.")]
	[AllowServer(false)]
	public void SetupToken(string? code = null)
	{
		if (TShock.SetupToken == 0)
		{
			Sender.SendWarningMessage("The initial setup system is disabled. This incident has been logged.");
			Sender.SendWarningMessage("If you are locked out of all admin accounts, ask for help on https://tshock.co/");
			TShock.Log.Warn("{0} attempted to use the initial setup system even though it's disabled.", Sender.IP);
			return;
		}

		// If the user account is already logged in, turn off the setup system
		if (Sender.IsLoggedIn && Sender.tempGroup == null)
		{
			Sender.SendSuccessMessage("Your new account has been verified, and the {0}setup system has been turned off.", TextOptions.CommandPrefix);
			Sender.SendSuccessMessage("Share your server, talk with admins, and chill on GitHub & Discord. -- https://tshock.co/");
			Sender.SendSuccessMessage("Thank you for using TShock for Terraria!");
			FileTools.CreateFile(Path.Combine(TShock.SavePath, "setup.lock"));
			File.Delete(Path.Combine(TShock.SavePath, "setup-code.txt"));
			TShock.SetupToken = 0;
			return;
		}

		if (String.IsNullOrWhiteSpace(code))
		{
			Sender.SendErrorMessage("You must provide a setup code!");
			return;
		}

		int givenCode;
		if (!Int32.TryParse(code, out givenCode) || givenCode != TShock.SetupToken)
		{
			Sender.SendErrorMessage("Incorrect setup code. This incident has been logged.");
			TShock.Log.Warn(Sender.IP + " attempted to use an incorrect setup code.");
			return;
		}

		if (Sender.Group.Name != "superadmin")
			Sender.tempGroup = new SuperAdminGroup();

		Sender.SendInfoMessage("Temporary system access has been given to you, so you can run one command.");
		Sender.SendWarningMessage("Please use the following to create a permanent account for you.");
		Sender.SendWarningMessage("{0}user add <username> <password> owner", TextOptions.CommandPrefix);
		Sender.SendInfoMessage("Creates: <username> with the password <password> as part of the owner group.");
		Sender.SendInfoMessage("Please use {0}login <username> <password> after this process.", TextOptions.CommandPrefix);
		Sender.SendWarningMessage("If you understand, please {0}login <username> <password> now, and then type {0}setup.", TextOptions.CommandPrefix);
		return;
	}

	[Command]
	[HelpText("Shows a command's aliases.")]
	public void Aliases([AllowSpaces] string? commandName = null)
	{
		if (String.IsNullOrWhiteSpace(commandName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}aliases <command or alias>", TextOptions.CommandPrefix);
			return;
		}

		//string givenCommandName = string.Join(" ", args.Parameters);
		//if (string.IsNullOrWhiteSpace(givenCommandName))
		//{
		//	Sender.SendErrorMessage("Please enter a proper command name or alias.");
		//	return;
		//}

		//string commandName;
		//if (givenCommandName[0] == Specifier[0])
		//	commandName = givenCommandName.Substring(1);
		//else
		//	commandName = givenCommandName;

		if (commandName.StartsWith(TextOptions.CommandPrefix))
			commandName = commandName.Substring(TextOptions.CommandPrefix.Length);

		var commands = this.CommandRepository.CommandList;

		bool didMatch = false;
		foreach (var matchingCommand in commands
			.Where(cmd =>
				cmd.Name.IndexOf(commandName, StringComparison.CurrentCultureIgnoreCase) != -1
				|| cmd.Aliases.Any(a => a.IndexOf(commandName, StringComparison.CurrentCultureIgnoreCase) != -1)
			)
			.Distinct()
		)
		{
			if (matchingCommand.Aliases.Length > 1)
				Sender.SendInfoMessage(
					"Aliases of {0}{1}: {0}{2}", TextOptions.CommandPrefix, matchingCommand.Name, string.Join(", {0}".SFormat(TextOptions.CommandPrefix), matchingCommand.Aliases));
			else
				Sender.SendInfoMessage("{0}{1} defines no aliases.", TextOptions.CommandPrefix, matchingCommand.Name);

			didMatch = true;
		}

		if (!didMatch)
			Sender.SendErrorMessage("No command or command alias matching \"{0}\" found.", commandName);
	}

	[Command("help")]
	[HelpText("Lists commands or gives help on them.")]
	public void Help(string? commandOrPage = null)
	{
		int pageNumber = 1;
		if (String.IsNullOrWhiteSpace(commandOrPage) || int.TryParse(commandOrPage, out pageNumber))
		{
			if (!String.IsNullOrWhiteSpace(commandOrPage)
				&& !PaginationTools.TryParsePageNumber(new(new[] { commandOrPage }), 0, Sender, out pageNumber))
			{
				return;
			}

			IEnumerable<string> cmdNames = (from cmd in CommandRepository.CommandList
										   where cmd.CanRun(Sender) && (cmd.Name != "setup" || TShock.SetupToken != 0)
										   select TextOptions.CommandPrefix + cmd.Name).Distinct();

			PaginationTools.SendPage(Sender, pageNumber, PaginationTools.BuildLinesFromTerms(cmdNames),
				new PaginationTools.Settings
				{
					HeaderFormat = "Commands ({0}/{1}):",
					FooterFormat = "Type {0}help {{0}} for more.".SFormat(TextOptions.CommandPrefix)
				});
		}
		else
		{
			//string commandName = args.Parameters[0].ToLower();
			if (commandOrPage.StartsWith(TextOptions.CommandPrefix))
			{
				commandOrPage = commandOrPage.Substring(TextOptions.CommandPrefix.Length);
			}

			var command = CommandRepository.CommandList.FirstOrDefault(c => c.Name == commandOrPage || c.Aliases.Contains(commandOrPage));
			if (command == null)
			{
				Sender.SendErrorMessage("Invalid command.");
				return;
			}
			if (!command.CanRun(Sender))
			{
				Sender.SendErrorMessage("You do not have access to this command.");
				return;
			}

			Sender.SendSuccessMessage("{0}{1} help: ", TextOptions.CommandPrefix, command.Name);
			//if (command.HelpDesc == null)
			//{
			Sender.SendInfoMessage(command.GetHelpText().Documentation);
			//	return;
			//}
			//foreach (string line in command.HelpDesc)
			//{
			//	Sender.SendInfoMessage(line);
			//}
		}
	}

	[Command("motd")]
	[HelpText("Shows the message of the day.")]
	public void Motd()
	{
		Sender.SendFileTextAsMessage(FileTools.MotdPath);
	}

	[Command("playing", "online", "who")]
	[HelpText("Shows the currently connected players.")]
	public void ListConnectedPlayers([AllowSpaces] string args = "")
	{
		var parameters = args.Split(' ');
		bool invalidUsage = (parameters.Length > 2);

		bool displayIdsRequested = false;
		int pageNumber = 1;
		if (!invalidUsage)
		{
			foreach (string parameter in parameters)
			{
				if (parameter.Equals("-i", StringComparison.InvariantCultureIgnoreCase))
				{
					displayIdsRequested = true;
					continue;
				}

				if (!int.TryParse(parameter, out pageNumber))
				{
					invalidUsage = true;
					break;
				}
			}
		}
		if (invalidUsage)
		{
			Sender.SendMessage($"List Online Players Syntax", Color.White);
			Sender.SendMessage($"{"playing".Color(Utils.BoldHighlight)} {"[-i]".Color(Utils.RedHighlight)} {"[page]".Color(Utils.GreenHighlight)}", Color.White);
			Sender.SendMessage($"Command aliases: {"playing".Color(Utils.GreenHighlight)}, {"online".Color(Utils.GreenHighlight)}, {"who".Color(Utils.GreenHighlight)}", Color.White);
			Sender.SendMessage($"Example usage: {"who".Color(Utils.BoldHighlight)} {"-i".Color(Utils.RedHighlight)}", Color.White);
			return;
		}
		if (displayIdsRequested && !Sender.HasPermission(Permissions.seeids))
		{
			Sender.SendErrorMessage("You do not have permission to see player IDs.");
			return;
		}

		if (TShock.Utils.GetActivePlayerCount() == 0)
		{
			Sender.SendMessage("There are currently no players online.", Color.White);
			return;
		}
		Sender.SendMessage($"Online Players ({TShock.Utils.GetActivePlayerCount().Color(Utils.GreenHighlight)}/{TShock.Config.Settings.MaxSlots})", Color.White);

		var players = new List<string>();

		foreach (TSPlayer ply in TShock.Players)
		{
			if (ply != null && ply.Active)
			{
				if (displayIdsRequested)
					players.Add($"{ply.Name} (Index: {ply.Index}{(ply.Account != null ? ", Account ID: " + ply.Account.ID : "")})");
				else
					players.Add(ply.Name);
			}
		}

		PaginationTools.SendPage(
			Sender, pageNumber, PaginationTools.BuildLinesFromTerms(players),
			new PaginationTools.Settings
			{
				IncludeHeader = false,
				FooterFormat = $"Type {TextOptions.CommandPrefix}who {(displayIdsRequested ? "-i" : string.Empty)}{TextOptions.CommandPrefix} for more."
			}
		);
	}

	[Command("rules")]
	//[CommandPermissions("")]
	[HelpText("Shows the server's rules.")]
	public void Rules()
	{
		Sender.SendFileTextAsMessage(FileTools.RulesPath);
	}
}
