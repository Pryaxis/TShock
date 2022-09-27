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
using Terraria.ID;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Localization;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

[Command("itemban")]
[CommandPermissions(Permissions.manageitem)]
[HelpText("Manages item bans.")]
class ItemBanCommands : CommandCallbacks<TSPlayer>
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
			"add <item> - Adds an item ban.",
			"allow <item> <group> - Allows a group to use an item.",
			"del <item> - Deletes an item ban.",
			"disallow <item> <group> - Disallows a group from using an item.",
			"list [page] - Lists all item bans."
		};

		PaginationTools.SendPage(Sender, pageNumber, lines,
			new PaginationTools.Settings
			{
				HeaderFormat = "Item Ban Sub-Commands ({0}/{1}):",
				FooterFormat = "Type {0}itemban help {{0}} for more sub-commands.".SFormat(TextOptions.CommandPrefix)
			}
		);
	}

	[SubCommand("add")]
	public void Add([AllowSpaces] string? itemName = null)
	{
		if (String.IsNullOrWhiteSpace(itemName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}itemban add <item name>", TextOptions.CommandPrefix);
			return;
		}

		List<Item> items = TShock.Utils.GetItemByIdOrName(itemName);
		if (items.Count == 0)
		{
			Sender.SendErrorMessage("Invalid item.");
		}
		else if (items.Count > 1)
		{
			Sender.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
		}
		else
		{
			// Yes this is required because of localization
			// User may have passed in localized name but itembans works on English names
			string englishNameForStorage = EnglishLanguage.GetItemNameById(items[0].type);
			TShock.ItemBans.DataModel.AddNewBan(englishNameForStorage);

			// It was decided in Telegram that we would continue to ban
			// projectiles based on whether or not their associated item was
			// banned. However, it was also decided that we'd change the way
			// this worked: in particular, we'd make it so that the item ban
			// system just adds things to the projectile ban system at the
			// command layer instead of inferring the state of projectile
			// bans based on the state of the item ban system.

			if (items[0].type == ItemID.DirtRod)
			{
				TShock.ProjectileBans.AddNewBan(ProjectileID.DirtBall);
			}

			if (items[0].type == ItemID.Sandgun)
			{
				TShock.ProjectileBans.AddNewBan(ProjectileID.SandBallGun);
				TShock.ProjectileBans.AddNewBan(ProjectileID.EbonsandBallGun);
				TShock.ProjectileBans.AddNewBan(ProjectileID.PearlSandBallGun);
			}

			// This returns the localized name to the player, not the item as it was stored.
			Sender.SendSuccessMessage("Banned " + items[0].Name + ".");
		}
	}

	[SubCommand("allow")]
	public void Allow(string? itemName = null, string? group = null)
	{
		if (String.IsNullOrWhiteSpace(itemName) || String.IsNullOrWhiteSpace(group))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}itemban allow <item name> <group name>", TextOptions.CommandPrefix);
			return;
		}

		List<Item> items = TShock.Utils.GetItemByIdOrName(itemName);
		if (items.Count == 0)
		{
			Sender.SendErrorMessage("Invalid item.");
		}
		else if (items.Count > 1)
		{
			Sender.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
		}
		else
		{
			if (!TShock.Groups.GroupExists(group))
			{
				Sender.SendErrorMessage("Invalid group.");
				return;
			}

			ItemBan ban = TShock.ItemBans.DataModel.GetItemBanByName(EnglishLanguage.GetItemNameById(items[0].type));
			if (ban == null)
			{
				Sender.SendErrorMessage("{0} is not banned.", items[0].Name);
				return;
			}
			if (!ban.AllowedGroups.Contains(group))
			{
				TShock.ItemBans.DataModel.AllowGroup(EnglishLanguage.GetItemNameById(items[0].type), group);
				Sender.SendSuccessMessage("{0} has been allowed to use {1}.", group, items[0].Name);
			}
			else
			{
				Sender.SendWarningMessage("{0} is already allowed to use {1}.", group, items[0].Name);
			}
		}
	}

	[SubCommand("del", "delete")]
	public void Delete([AllowSpaces] string? itemName = null)
	{
		if (String.IsNullOrWhiteSpace(itemName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}itemban del <item name>", TextOptions.CommandPrefix);
			return;
		}

		List<Item> items = TShock.Utils.GetItemByIdOrName(itemName);
		if (items.Count == 0)
		{
			Sender.SendErrorMessage("Invalid item.");
		}
		else if (items.Count > 1)
		{
			Sender.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
		}
		else
		{
			TShock.ItemBans.DataModel.RemoveBan(EnglishLanguage.GetItemNameById(items[0].type));
			Sender.SendSuccessMessage("Unbanned " + items[0].Name + ".");
		}
	}

	[SubCommand("disallow")]
	public void Disallow(string? itemName = null, string? group = null)
	{
		if (String.IsNullOrWhiteSpace(itemName) || String.IsNullOrWhiteSpace(group))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}itemban disallow <item name> <group name>", TextOptions.CommandPrefix);
			return;
		}

		List<Item> items = TShock.Utils.GetItemByIdOrName(itemName);
		if (items.Count == 0)
		{
			Sender.SendErrorMessage("Invalid item.");
		}
		else if (items.Count > 1)
		{
			Sender.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
		}
		else
		{
			if (!TShock.Groups.GroupExists(group))
			{
				Sender.SendErrorMessage("Invalid group.");
				return;
			}

			ItemBan ban = TShock.ItemBans.DataModel.GetItemBanByName(EnglishLanguage.GetItemNameById(items[0].type));
			if (ban == null)
			{
				Sender.SendErrorMessage("{0} is not banned.", items[0].Name);
				return;
			}
			if (ban.AllowedGroups.Contains(group))
			{
				TShock.ItemBans.DataModel.RemoveGroup(EnglishLanguage.GetItemNameById(items[0].type), group);
				Sender.SendSuccessMessage("{0} has been disallowed to use {1}.", group, items[0].Name);
			}
			else
			{
				Sender.SendWarningMessage("{0} is already disallowed to use {1}.", group, items[0].Name);
			}
		}
	}

	[SubCommand("list")]
	public void List([PageNumber] int pageNumber = 1)
	{
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out pageNumber))
		//	return;
		IEnumerable<string> itemNames = from itemBan in TShock.ItemBans.DataModel.ItemBans
										select itemBan.Name;
		PaginationTools.SendPage(Sender, pageNumber, PaginationTools.BuildLinesFromTerms(itemNames),
			new PaginationTools.Settings
			{
				HeaderFormat = "Item bans ({0}/{1}):",
				FooterFormat = "Type {0}itemban list {{0}} for more.".SFormat(TextOptions.CommandPrefix),
				NothingToDisplayString = "There are currently no banned items."
			});
	}
}
