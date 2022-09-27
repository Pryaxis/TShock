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
using TShockAPI;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

[Command("warp")]
[HelpText("Teleports you to a warp point or manages warps.")]
[CommandPermissions(Permissions.warp)]
class WarpCommands : CommandCallbacks<TSPlayer>
{
	[SubCommand(SubCommandType.Default)]
	[CommandPermissions(Permissions.warp)]
	public void Warp([AllowSpaces] string warpName)
	{
		var warp = TShock.Warps.Find(warpName);
		if (warp != null)
		{
			if (Sender.Teleport(warp.Position.X * 16, warp.Position.Y * 16))
				Sender.SendSuccessMessage("Warped to " + warpName + ".");
		}
		else
		{
			Sender.SendErrorMessage("The specified warp was not found.");
		}
	}

	[SubCommand]
	public void Help()
	{
		bool hasManageWarpPermission = Sender.HasPermission(Permissions.managewarp);
		//if (args.Parameters.Count < 1)
		{
			if (hasManageWarpPermission)
			{
				Sender.SendInfoMessage("Invalid syntax! Proper syntax: {0}warp [command] [arguments]", TextOptions.CommandPrefix);
				Sender.SendInfoMessage("Commands: add, del, hide, list, send, [warpname]");
				Sender.SendInfoMessage("Arguments: add [warp name], del [warp name], list [page]");
				Sender.SendInfoMessage("Arguments: send [player] [warp name], hide [warp name] [Enable(true/false)]");
				Sender.SendInfoMessage("Examples: {0}warp add foobar, {0}warp hide foobar true, {0}warp foobar", TextOptions.CommandPrefix);
				return;
			}
			else
			{
				Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}warp [name] or {0}warp list <page>", TextOptions.CommandPrefix);
				return;
			}
		}
	}

	[SubCommand]
	public void List([PageNumber] int pageNumber = 1)
	{
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out pageNumber))
		//	return;
		IEnumerable<string> warpNames = from warp in TShock.Warps.Warps
										where !warp.IsPrivate
										select warp.Name;
		PaginationTools.SendPage(Sender, pageNumber, PaginationTools.BuildLinesFromTerms(warpNames),
			new PaginationTools.Settings
			{
				HeaderFormat = "Warps ({0}/{1}):",
				FooterFormat = "Type {0}warp list {{0}} for more.".SFormat(TextOptions.CommandPrefix),
				NothingToDisplayString = "There are currently no warps defined."
			});
	}

	[SubCommand]
	[CommandPermissions(Permissions.managewarp)]
	public void Add(string warpName)
	{
		if (!String.IsNullOrWhiteSpace(warpName))
		{
			if (warpName == "list" || warpName == "hide" || warpName == "del" || warpName == "add")
			{
				Sender.SendErrorMessage("Name reserved, use a different name.");
			}
			else if (TShock.Warps.Add(Sender.TileX, Sender.TileY, warpName))
			{
				Sender.SendSuccessMessage("Warp added: " + warpName);
			}
			else
			{
				Sender.SendErrorMessage("Warp " + warpName + " already exists.");
			}
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}warp add [name]", TextOptions.CommandPrefix);
	}

	[SubCommand("del")]
	[CommandPermissions(Permissions.managewarp)]
	public void Delete(string warpName)
	{
		if (!String.IsNullOrWhiteSpace(warpName))
		{
			if (TShock.Warps.Remove(warpName))
			{
				Sender.SendSuccessMessage("Warp deleted: " + warpName);
			}
			else
				Sender.SendErrorMessage("Could not find the specified warp.");
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}warp del [name]", TextOptions.CommandPrefix);
	}

	[SubCommand]
	[CommandPermissions(Permissions.managewarp)]
	public void Hide(string warpName, bool hide)
	{
		//if (args.Parameters.Count == 3)
		//{
		//string warpName = args.Parameters[1];
		//bool state = false;
		//if (Boolean.TryParse(args.Parameters[2], out state))
		//{
		if (TShock.Warps.Hide(warpName, hide))
		{
			if (hide)
				Sender.SendSuccessMessage("Warp " + warpName + " is now private.");
			else
				Sender.SendSuccessMessage("Warp " + warpName + " is now public.");
		}
		else
			Sender.SendErrorMessage("Could not find specified warp.");
		//}
		//else
		//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}warp hide [name] <true/false>", Specifier);
		//}
		//else
		//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}warp hide [name] <true/false>", Specifier);
	}

	[SubCommand]
	[CommandPermissions(Permissions.tpothers)]
	public void Send(TSPlayer target, string warpName)
	{
		if (target is null || String.IsNullOrWhiteSpace(warpName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}warp send [player] [warpname]", TextOptions.CommandPrefix);
			return;
		}

		//var foundplr = TSPlayer.FindByNameOrID(args.Parameters[1]);
		//if (foundplr.Count == 0)
		//{
		//	Sender.SendErrorMessage("Invalid player!");
		//	return;
		//}
		//else if (foundplr.Count > 1)
		//{
		//	Sender.SendMultipleMatchError(foundplr.Select(p => p.Name));
		//	return;
		//}

		//string warpName = args.Parameters[2];
		var warp = TShock.Warps.Find(warpName);
		var plr = target; // foundplr[0];
		if (warp != null)
		{
			if (plr.Teleport(warp.Position.X * 16, warp.Position.Y * 16))
			{
				plr.SendSuccessMessage(String.Format("{0} warped you to {1}.", Sender.Name, warpName));
				Sender.SendSuccessMessage(String.Format("You warped {0} to {1}.", plr.Name, warpName));
			}
		}
		else
		{
			Sender.SendErrorMessage("Specified warp not found.");
		}
	}
}
