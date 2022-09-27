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
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

[Command("tileban")]
[CommandPermissions(Permissions.managetile)]
[HelpText("Manages tile bans.")]
class TileBanCommands : CommandCallbacks<TSPlayer>
{
	[SubCommand(SubCommandType.Default)]
	public void Default([PageNumber] int pageNumber = 1) => Help(pageNumber);

	[SubCommand("help")]
	public void Help([PageNumber] int pageNumber = 1)
	{
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out pageNumber))
		//	return;

		var lines = new List<string>
		{
			"add <tile ID> - Adds a tile ban.",
			"allow <tile ID> <group> - Allows a group to place a tile.",
			"del <tile ID> - Deletes a tile ban.",
			"disallow <tile ID> <group> - Disallows a group from place a tile.",
			"list [page] - Lists all tile bans."
		};

		PaginationTools.SendPage(Sender, pageNumber, lines,
			new PaginationTools.Settings
			{
				HeaderFormat = "Tile Ban Sub-Commands ({0}/{1}):",
				FooterFormat = "Type {0}tileban help {{0}} for more sub-commands.".SFormat(TextOptions.CommandPrefix)
			}
		);
	}

	[SubCommand("add")]
	public void Add(string? tileId = null)
	{
		if (String.IsNullOrWhiteSpace(tileId))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tileban add <tile id>", TextOptions.CommandPrefix);
			return;
		}
		short id;
		if (Int16.TryParse(tileId, out id) && id >= 0 && id < Main.maxTileSets)
		{
			TShock.TileBans.AddNewBan(id);
			Sender.SendSuccessMessage("Banned tile {0}.", id);
		}
		else
			Sender.SendErrorMessage("Invalid tile ID!");
	}

	[SubCommand("allow")]
	public void Allow(string? tileId = null, string? group = null)
	{
		if (String.IsNullOrWhiteSpace(tileId) || String.IsNullOrWhiteSpace(group))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tileban allow <id> <group>", TextOptions.CommandPrefix);
			return;
		}

		short id;
		if (Int16.TryParse(tileId, out id) && id >= 0 && id < Main.maxTileSets)
		{
			if (!TShock.Groups.GroupExists(group))
			{
				Sender.SendErrorMessage("Invalid group.");
				return;
			}

			TileBan ban = TShock.TileBans.GetBanById(id);
			if (ban == null)
			{
				Sender.SendErrorMessage("Tile {0} is not banned.", id);
				return;
			}
			if (!ban.AllowedGroups.Contains(group))
			{
				TShock.TileBans.AllowGroup(id, group);
				Sender.SendSuccessMessage("{0} has been allowed to place tile {1}.", group, id);
			}
			else
				Sender.SendWarningMessage("{0} is already allowed to place tile {1}.", group, id);
		}
		else
			Sender.SendErrorMessage("Invalid tile ID!");
	}

	[SubCommand("del", "delete")]
	public void Delete(string? tileId = null)
	{
		if (String.IsNullOrWhiteSpace(tileId))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tileban del <id>", TextOptions.CommandPrefix);
			return;
		}

		short id;
		if (Int16.TryParse(tileId, out id) && id >= 0 && id < Main.maxTileSets)
		{
			TShock.TileBans.RemoveBan(id);
			Sender.SendSuccessMessage("Unbanned tile {0}.", id);
			return;
		}
		else
			Sender.SendErrorMessage("Invalid tile ID!");
	}

	[SubCommand("disallow")]
	public void Disallow(string? tileId = null, string? group = null)
	{
		if (String.IsNullOrWhiteSpace(tileId) || String.IsNullOrWhiteSpace(group))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tileban disallow <id> <group name>", TextOptions.CommandPrefix);
			return;
		}

		short id;
		if (Int16.TryParse(tileId, out id) && id >= 0 && id < Main.maxTileSets)
		{
			if (!TShock.Groups.GroupExists(group))
			{
				Sender.SendErrorMessage("Invalid group.");
				return;
			}

			TileBan ban = TShock.TileBans.GetBanById(id);
			if (ban == null)
			{
				Sender.SendErrorMessage("Tile {0} is not banned.", id);
				return;
			}
			if (ban.AllowedGroups.Contains(group))
			{
				TShock.TileBans.RemoveGroup(id, group);
				Sender.SendSuccessMessage("{0} has been disallowed from placing tile {1}.", group, id);
				return;
			}
			else
				Sender.SendWarningMessage("{0} is already prevented from placing tile {1}.", group, id);
		}
		else
			Sender.SendErrorMessage("Invalid tile ID!");
	}

	[SubCommand("list")]
	public void List([PageNumber] int pageNumber = 1)
	{
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out pageNumber))
		//	return;
		IEnumerable<Int16> tileIds = from tileBan in TShock.TileBans.TileBans
									 select tileBan.ID;
		PaginationTools.SendPage(Sender, pageNumber, PaginationTools.BuildLinesFromTerms(tileIds),
			new PaginationTools.Settings
			{
				HeaderFormat = "Tile bans ({0}/{1}):",
				FooterFormat = "Type {0}tileban list {{0}} for more.".SFormat(TextOptions.CommandPrefix),
				NothingToDisplayString = "There are currently no banned tiles."
			});
	}
}
