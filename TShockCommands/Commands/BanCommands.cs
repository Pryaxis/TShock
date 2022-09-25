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
using System.Linq;
using TShockAPI;
using TShockAPI.DB;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

[FlagsArgument]
public class AddBanFlags
{
	[FlagParams("-e")]
	public bool ExactTarget = true;

	[FlagParams("-a")]
	public bool BanAccount = false;

	[FlagParams("-u")]
	public bool BanUuid = true;

	[FlagParams("-n")]
	public bool BanName = false;

	[FlagParams("-ip")]
	public bool BanIp = true;
}

[Command("ban")]
[HelpText("Manages player bans.")]
[CommandPermissions(Permissions.ban)]
class BanCommands : CommandCallbacks<TSPlayer>
{
	//Ban syntax:
	// ban add <target> [reason] [duration] [flags (default: -a -u -ip)]
	//						Duration is in the format 0d0h0m0s. Any part can be ignored. E.g., 1s is a valid ban time, as is 1d1s, etc. If no duration is specified, ban is permanent
	//						Valid flags: -a (ban account name), -u (ban UUID), -n (ban character name), -ip (ban IP address), -e (exact, ban the identifier provided as 'target')
	//						Unless -e is passed to the command, <target> is assumed to be a player or player index.
	// ban del <ban ID>
	//						Target is expected to be a ban Unique ID
	// ban list [page]
	//						Displays a paginated list of bans
	// ban details <ban ID>
	//						Target is expected to be a ban Unique ID
	// ban help [command]
	//						Provides extended help on specific ban commands

	[SubCommand]
	public void Help(string? cmd = null, [PageNumber] int pageNumber = 1)
	{
		if (string.IsNullOrWhiteSpace(cmd))
		{
			Sender.SendMessage("TShock Ban Help", Color.White);
			Sender.SendMessage("Available Ban commands:", Color.White);
			Sender.SendMessage($"ban {"add".Color(Utils.RedHighlight)} <Target> [Flags]", Color.White);
			Sender.SendMessage($"ban {"del".Color(Utils.RedHighlight)} <Ban ID>", Color.White);
			Sender.SendMessage($"ban {"list".Color(Utils.RedHighlight)}", Color.White);
			Sender.SendMessage($"ban {"details".Color(Utils.RedHighlight)} <Ban ID>", Color.White);
			Sender.SendMessage($"Quick usage: {"ban add".Color(Utils.BoldHighlight)} {Sender.Name.Color(Utils.RedHighlight)} \"Griefing\"", Color.White);
			Sender.SendMessage($"For more info, use {"ban help".Color(Utils.BoldHighlight)} {"command".Color(Utils.RedHighlight)} or {"ban help".Color(Utils.BoldHighlight)} {"examples".Color(Utils.RedHighlight)}", Color.White);
		}
		else
		{
			switch (cmd)
			{
				case "add":
					Sender.SendMessage("", Color.White);
					Sender.SendMessage("Ban Add Syntax", Color.White);
					Sender.SendMessage($"{"ban add".Color(Utils.BoldHighlight)} <{"Target".Color(Utils.RedHighlight)}> <{"Reason".Color(Utils.BoldHighlight)} | .> <{"Duration".Color(Utils.PinkHighlight)} | .> [{"Flags".Color(Utils.GreenHighlight)}]", Color.White);
					Sender.SendMessage($"- {"Duration".Color(Utils.PinkHighlight)}: uses the format {"0d0m0s".Color(Utils.PinkHighlight)} to determine the length of the ban.", Color.White);
					Sender.SendMessage($"   Eg a value of {"10d30m0s".Color(Utils.PinkHighlight)} would represent 10 days, 30 minutes, 0 seconds.", Color.White);
					Sender.SendMessage($"   If no duration is provided, the ban will be permanent.", Color.White);
					Sender.SendMessage($"- {"Flags".Color(Utils.GreenHighlight)}: -a (account name), -u (UUID), -n (character name), -ip (IP address), -e (exact, {"Target".Color(Utils.RedHighlight)} will be treated as identifier)", Color.White);
					Sender.SendMessage($"   Unless {"-e".Color(Utils.GreenHighlight)} is passed to the command, {"Target".Color(Utils.RedHighlight)} is assumed to be a player or player index", Color.White);
					Sender.SendMessage($"   If no {"Flags".Color(Utils.GreenHighlight)} are specified, the command uses {"-a -u -ip".Color(Utils.GreenHighlight)} by default.", Color.White);
					Sender.SendMessage($"Example usage: {"ban add".Color(Utils.BoldHighlight)} {Sender.Name.Color(Utils.RedHighlight)} {"\"Cheating\"".Color(Utils.BoldHighlight)} {"10d30m0s".Color(Utils.PinkHighlight)} {"-a -u -ip".Color(Utils.GreenHighlight)}", Color.White);
					break;

				case "del":
					Sender.SendMessage("", Color.White);
					Sender.SendMessage("Ban Del Syntax", Color.White);
					Sender.SendMessage($"{"ban del".Color(Utils.BoldHighlight)} <{"Ticket Number".Color(Utils.RedHighlight)}>", Color.White);
					Sender.SendMessage($"- {"Ticket Numbers".Color(Utils.RedHighlight)} are provided when you add a ban, and can also be viewed with the {"ban list".Color(Utils.BoldHighlight)} command.", Color.White);
					Sender.SendMessage($"Example usage: {"ban del".Color(Utils.BoldHighlight)} {"12345".Color(Utils.RedHighlight)}", Color.White);
					break;

				case "list":
					Sender.SendMessage("", Color.White);
					Sender.SendMessage("Ban List Syntax", Color.White);
					Sender.SendMessage($"{"ban list".Color(Utils.BoldHighlight)} [{"Page".Color(Utils.PinkHighlight)}]", Color.White);
					Sender.SendMessage("- Lists active bans. Color trends towards green as the ban approaches expiration", Color.White);
					Sender.SendMessage($"Example usage: {"ban list".Color(Utils.BoldHighlight)}", Color.White);
					break;

				case "details":
					Sender.SendMessage("", Color.White);
					Sender.SendMessage("Ban Details Syntax", Color.White);
					Sender.SendMessage($"{"ban details".Color(Utils.BoldHighlight)} <{"Ticket Number".Color(Utils.RedHighlight)}>", Color.White);
					Sender.SendMessage($"- {"Ticket Numbers".Color(Utils.RedHighlight)} are provided when you add a ban, and can be found with the {"ban list".Color(Utils.BoldHighlight)} command.", Color.White);
					Sender.SendMessage($"Example usage: {"ban details".Color(Utils.BoldHighlight)} {"12345".Color(Utils.RedHighlight)}", Color.White);
					break;

				case "identifiers":
					//if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, Sender, out int pageNumber))
					if (pageNumber < 1)
					{
						Sender.SendMessage($"Invalid page number. Page number must be numeric.", Color.White);
						return;
					}

					var idents = from ident in Identifier.Available
								 select $"{ident.Color(Utils.RedHighlight)} - {ident.Description}";

					Sender.SendMessage("", Color.White);
					PaginationTools.SendPage(Sender, pageNumber, idents.ToList(),
						new PaginationTools.Settings
						{
							HeaderFormat = "Available identifiers ({0}/{1}):",
							FooterFormat = "Type {0}ban help identifiers {{0}} for more.".SFormat(TextOptions.CommandPrefix),
							NothingToDisplayString = "There are currently no available identifiers.",
							HeaderTextColor = Color.White,
							LineTextColor = Color.White
						});
					break;

				case "examples":
					Sender.SendMessage("", Color.White);
					Sender.SendMessage("Ban Usage Examples", Color.White);
					Sender.SendMessage("- Ban an offline player by account name", Color.White);
					Sender.SendMessage($"   {TextOptions.CommandPrefix}{"ban add".Color(Utils.BoldHighlight)} \"{"acc:".Color(Utils.RedHighlight)}{Sender.Account.Color(Utils.RedHighlight)}\" {"\"Multiple accounts are not allowed\"".Color(Utils.BoldHighlight)} {"-e".Color(Utils.GreenHighlight)} (Permanently bans this account name)", Color.White);
					Sender.SendMessage("- Ban an offline player by IP address", Color.White);
					Sender.SendMessage($"   {TextOptions.CommandPrefix}{"ai".Color(Utils.BoldHighlight)} \"{Sender.Account.Color(Utils.RedHighlight)}\" (Find the IP associated with the offline target's account)", Color.White);
					Sender.SendMessage($"   {TextOptions.CommandPrefix}{"ban add".Color(Utils.BoldHighlight)} {"ip:".Color(Utils.RedHighlight)}{Sender.IP.Color(Utils.RedHighlight)} {"\"Griefing\"".Color(Utils.BoldHighlight)} {"-e".Color(Utils.GreenHighlight)} (Permanently bans this IP address)", Color.White);
					Sender.SendMessage($"- Ban an online player by index (Useful for hard to type names)", Color.White);
					Sender.SendMessage($"   {TextOptions.CommandPrefix}{"who".Color(Utils.BoldHighlight)} {"-i".Color(Utils.GreenHighlight)} (Find the player index for the target)", Color.White);
					Sender.SendMessage($"   {TextOptions.CommandPrefix}{"ban add".Color(Utils.BoldHighlight)} {"tsi:".Color(Utils.RedHighlight)}{Sender.Index.Color(Utils.RedHighlight)} {"\"Trolling\"".Color(Utils.BoldHighlight)} {"-a -u -ip".Color(Utils.GreenHighlight)} (Permanently bans the online player by Account, UUID, and IP)", Color.White);
					// Ban by account ID when?
					break;

				default:
					Sender.SendMessage($"Unknown ban command. Try {"ban help".Color(Utils.BoldHighlight)} {"add".Color(Utils.RedHighlight)}, {"del".Color(Utils.RedHighlight)}, {"list".Color(Utils.RedHighlight)}, {"details".Color(Utils.RedHighlight)}, {"identifiers".Color(Utils.RedHighlight)}, or {"examples".Color(Utils.RedHighlight)}.", Color.White); break;
			}
		}

	}

	AddBanResult DoBan(string ident, string reason, DateTime expiration)
	{
		AddBanResult banResult = TShock.Bans.InsertBan(ident, reason, Sender.Account.Name, DateTime.UtcNow, expiration);
		if (banResult.Ban != null)
		{
			Sender.SendSuccessMessage($"Ban added. Ticket Number {banResult.Ban.TicketNumber.Color(Utils.GreenHighlight)} was created for identifier {ident.Color(Utils.WhiteHighlight)}.");
		}
		else
		{
			Sender.SendWarningMessage($"Failed to add ban for identifier: {ident.Color(Utils.WhiteHighlight)}");
			Sender.SendWarningMessage($"Reason: {banResult.Message}");
		}

		return banResult;
	}

	[SubCommand]
	public void Add(string target, string reason, [Duration] int duration, AddBanFlags abf)
	{
		//if (!args.Parameters.TryGetValue(1, out string target))
		if (String.IsNullOrWhiteSpace(target))
		{
			Sender.SendMessage($"Invalid Ban Add syntax. Refer to {"ban help add".Color(Utils.BoldHighlight)} for details on how to use the {"ban add".Color(Utils.BoldHighlight)} command", Color.White);
			return;
		}

		//bool exactTarget = args.Parameters.Any(p => p == "-e");
		//bool banAccount = args.Parameters.Any(p => p == "-a");
		//bool banUuid = args.Parameters.Any(p => p == "-u");
		//bool banName = args.Parameters.Any(p => p == "-n");
		//bool banIp = args.Parameters.Any(p => p == "-ip");

		//List<string> flags = new List<string>() { "-e", "-a", "-u", "-n", "-ip" };

		if (reason == ".") reason = null;
		reason ??= "Banned.";
		//string duration = null;
		//DateTime expiration = DateTime.MaxValue;
		//var expiration = duration ?? DateTime.MaxValue;
		var expiration = DateTime.UtcNow.AddSeconds(duration);

		//This is hacky. We want flag values to be independent of order so we must force the consecutive ordering of the 'reason' and 'duration' parameters,
		//while still allowing them to be placed arbitrarily in the parameter list.
		//As an example, the following parameter lists (and more) should all be acceptable:
		//-u "reason" -a duration -ip
		//"reason" duration -u -a -ip
		//-u -a -ip "reason" duration
		//-u -a -ip
		//for (int i = 2; i < args.Parameters.Count; i++)
		//{
		//	var param = args.Parameters[i];
		//	if (!flags.Contains(param))
		//	{
		//		reason = param;
		//		break;
		//	}
		//}
		//for (int i = 3; i < args.Parameters.Count; i++)
		//{
		//	var param = args.Parameters[i];
		//	if (!flags.Contains(param))
		//	{
		//		duration = param;
		//		break;
		//	}
		//}

		//if (TShock.Utils.TryParseTime(duration, out int seconds))
		//{
		//	expiration = DateTime.UtcNow.AddSeconds(seconds);
		//}

		//If no flags were specified, default to account, uuid, and IP
		if (!abf.ExactTarget && !abf.BanAccount && !abf.BanUuid && !abf.BanName && !abf.BanIp)
		{
			abf.BanAccount = abf.BanUuid = abf.BanIp = true;

			if (TShock.Config.Settings.DisableDefaultIPBan)
			{
				abf.BanIp = false;
			}
		}

		reason ??= "Banned";

		if (abf.ExactTarget)
		{
			DoBan(target, reason, expiration);
			return;
		}

		var players = TSPlayer.FindByNameOrID(target);

		if (players.Count > 1)
		{
			Sender.SendMultipleMatchError(players.Select(p => p.Name));
			return;
		}

		if (players.Count < 1)
		{
			Sender.SendErrorMessage("Could not find the target specified. Check that you have the correct spelling.");
			return;
		}

		var player = players[0];
		AddBanResult banResult = null;

		if (abf.BanAccount)
		{
			if (player.Account != null)
			{
				banResult = DoBan($"{Identifier.Account}{player.Account.Name}", reason, expiration);
			}
		}

		if (abf.BanUuid)
		{
			banResult = DoBan($"{Identifier.UUID}{player.UUID}", reason, expiration);
		}

		if (abf.BanName)
		{
			banResult = DoBan($"{Identifier.Name}{player.Name}", reason, expiration);
		}

		if (abf.BanIp)
		{
			banResult = DoBan($"{Identifier.IP}{player.IP}", reason, expiration);
		}

		if (banResult?.Ban != null)
		{
			player.Disconnect($"#{banResult.Ban.TicketNumber} - You have been banned: {banResult.Ban.Reason}.");
		}
	}

	[SubCommand("del")]
	public void Delete(string? target = null)
	{
		//if (!args.Parameters.TryGetValue(1, out string target))
		if (String.IsNullOrWhiteSpace(target))
		{
			Sender.SendMessage($"Invalid Ban Del syntax. Refer to {"ban help del".Color(Utils.BoldHighlight)} for details on how to use the {"ban del".Color(Utils.BoldHighlight)} command", Color.White);
			return;
		}

		if (!int.TryParse(target, out int banId))
		{
			Sender.SendMessage($"Invalid Ticket Number. Refer to {"ban help del".Color(Utils.BoldHighlight)} for details on how to use the {"ban del".Color(Utils.BoldHighlight)} command", Color.White);
			return;
		}

		if (TShock.Bans.RemoveBan(banId))
		{
			TShock.Log.ConsoleInfo($"Ban {banId} has been revoked by {Sender.Account.Name}.");
			Sender.SendSuccessMessage($"Ban {banId.Color(Utils.GreenHighlight)} has now been marked as expired.");
		}
		else
		{
			Sender.SendErrorMessage("Failed to remove ban.");
		}
	}

	[SubCommand]
	public void List([PageNumber] int pageNumber = 1)
	{
		string PickColorForBan(Ban ban)
		{
			double hoursRemaining = (ban.ExpirationDateTime - DateTime.UtcNow).TotalHours;
			double hoursTotal = (ban.ExpirationDateTime - ban.BanDateTime).TotalHours;
			double percentRemaining = TShock.Utils.Clamp(hoursRemaining / hoursTotal, 100, 0);

			int red = TShock.Utils.Clamp((int)(255 * 2.0f * percentRemaining), 255, 0);
			int green = TShock.Utils.Clamp((int)(255 * (2.0f * (1 - percentRemaining))), 255, 0);

			return $"{red:X2}{green:X2}{0:X2}";
		}

		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out int pageNumber))
		if (pageNumber < 1)
		{
			Sender.SendMessage($"Invalid Ban List syntax. Refer to {"ban help list".Color(Utils.BoldHighlight)} for details on how to use the {"ban list".Color(Utils.BoldHighlight)} command", Color.White);
			return;
		}

		var bans = from ban in TShock.Bans.Bans
				   where ban.Value.ExpirationDateTime > DateTime.UtcNow
				   orderby ban.Value.ExpirationDateTime ascending
				   select $"[{ban.Key.Color(Utils.GreenHighlight)}] {ban.Value.Identifier.Color(PickColorForBan(ban.Value))}";

		PaginationTools.SendPage(Sender, pageNumber, bans.ToList(),
			new PaginationTools.Settings
			{
				HeaderFormat = "Bans ({0}/{1}):",
				FooterFormat = "Type {0}ban list {{0}} for more.".SFormat(TextOptions.CommandPrefix),
				NothingToDisplayString = "There are currently no active bans."
			});
	}

	[SubCommand]
	public void Details(string target)
	{
		//if (!args.Parameters.TryGetValue(1, out string target))
		if (String.IsNullOrWhiteSpace(target))
		{
			Sender.SendMessage($"Invalid Ban Details syntax. Refer to {"ban help details".Color(Utils.BoldHighlight)} for details on how to use the {"ban details".Color(Utils.BoldHighlight)} command", Color.White);
			return;
		}

		if (!int.TryParse(target, out int banId))
		{
			Sender.SendMessage($"Invalid Ticket Number. Refer to {"ban help details".Color(Utils.BoldHighlight)} for details on how to use the {"ban details".Color(Utils.BoldHighlight)} command", Color.White);
			return;
		}

		Ban ban = TShock.Bans.GetBanById(banId);

		if (ban == null)
		{
			Sender.SendErrorMessage("No bans found matching the provided ticket number");
			return;
		}

		DisplayBanDetails(ban);
	}

	void DisplayBanDetails(Ban ban)
	{
		Sender.SendMessage($"{"Ban Details".Color(Utils.BoldHighlight)} - Ticket Number: {ban.TicketNumber.Color(Utils.GreenHighlight)}", Color.White);
		Sender.SendMessage($"{"Identifier:".Color(Utils.BoldHighlight)} {ban.Identifier}", Color.White);
		Sender.SendMessage($"{"Reason:".Color(Utils.BoldHighlight)} {ban.Reason}", Color.White);
		Sender.SendMessage($"{"Banned by:".Color(Utils.BoldHighlight)} {ban.BanningUser.Color(Utils.GreenHighlight)} on {ban.BanDateTime.ToString("yyyy/MM/dd").Color(Utils.RedHighlight)} ({ban.GetPrettyTimeSinceBanString().Color(Utils.YellowHighlight)} ago)", Color.White);
		if (ban.ExpirationDateTime < DateTime.UtcNow)
		{
			Sender.SendMessage($"{"Ban expired:".Color(Utils.BoldHighlight)} {ban.ExpirationDateTime.ToString("yyyy/MM/dd").Color(Utils.RedHighlight)} ({ban.GetPrettyExpirationString().Color(Utils.YellowHighlight)} ago)", Color.White);
		}
		else
		{
			string remaining;
			if (ban.ExpirationDateTime == DateTime.MaxValue)
			{
				remaining = "Never".Color(Utils.YellowHighlight);
			}
			else
			{
				remaining = $"{ban.GetPrettyExpirationString().Color(Utils.YellowHighlight)} remaining";
			}

			Sender.SendMessage($"{"Ban expires:".Color(Utils.BoldHighlight)} {ban.ExpirationDateTime.ToString("yyyy/MM/dd").Color(Utils.RedHighlight)} ({remaining})", Color.White);
		}
	}
}
