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
using TShockAPI.Localization;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

class ItemCommands : CommandCallbacks<TSPlayer>
{
	[Command("give", "g")]
	[HelpText("Gives another player an item.")]
	[CommandPermissions(Permissions.give)]
	public void Give(string itemNameOrId, TSPlayer player, int itemAmount = 0, string? prefixidOrName = null)
	{
		if (itemNameOrId is null || player is null)
		{
			Sender.SendErrorMessage(
				"Invalid syntax! Proper syntax: {0}give <item type/id> <player> [item amount] [prefix id/name]", TextOptions.CommandPrefix);
			return;
		}
		if (itemNameOrId is null)
		{
			Sender.SendErrorMessage("Missing item name/id.");
			return;
		}
		if (player is null)
		{
			Sender.SendErrorMessage("Missing player name.");
			return;
		}
		//int itemAmount = 0;
		int prefix = 0;
		var items = TShock.Utils.GetItemByIdOrName(itemNameOrId);
		//args.Parameters.RemoveAt(0);
		//string plStr = args.Parameters[0];
		//args.Parameters.RemoveAt(0);
		//if (args.Parameters.Count == 1)
		//	int.TryParse(args.Parameters[0], out itemAmount);

		if (items.Count == 0)
		{
			Sender.SendErrorMessage("Invalid item type!");
		}
		else if (items.Count > 1)
		{
			Sender.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
		}
		else
		{
			var item = items[0];

			if (!String.IsNullOrWhiteSpace(prefixidOrName))
			{
				//int.TryParse(idOrName, out itemAmount);
				var prefixIds = TShock.Utils.GetPrefixByIdOrName(prefixidOrName);
				if (item.accessory && prefixIds.Contains(PrefixID.Quick))
				{
					prefixIds.Remove(PrefixID.Quick);
					prefixIds.Remove(PrefixID.Quick2);
					prefixIds.Add(PrefixID.Quick2);
				}
				else if (!item.accessory && prefixIds.Contains(PrefixID.Quick))
					prefixIds.Remove(PrefixID.Quick2);
				if (prefixIds.Count == 1)
					prefix = prefixIds[0];
			}

			if (item.type >= 1 && item.type < Terraria.Main.maxItemTypes)
			{
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
					var plr = player; // players[0];
					if (plr.InventorySlotAvailable || (item.type > 70 && item.type < 75) || item.ammo > 0 || item.type == 58 || item.type == 184)
					{
						if (itemAmount == 0 || itemAmount > item.maxStack)
							itemAmount = item.maxStack;
						if (plr.GiveItemCheck(item.type, EnglishLanguage.GetItemNameById(item.type), itemAmount, prefix))
						{
							Sender.SendSuccessMessage(string.Format("Gave {0} {1} {2}(s).", plr.Name, itemAmount, item.Name));
							plr.SendSuccessMessage(string.Format("{0} gave you {1} {2}(s).", Sender.Name, itemAmount, item.Name));
						}
						else
						{
							Sender.SendErrorMessage("You cannot spawn banned items.");
						}

					}
					else
					{
						Sender.SendErrorMessage("Player does not have free slots!");
					}
				}
			}
			else
			{
				Sender.SendErrorMessage("Invalid item type!");
			}
		}
	}

	//			add(new Command(Permissions., , )
	//			{
	//				AllowServer = false,
	//				HelpText = "."
	//			});


	[Command("item", "i")]
	[HelpText("Gives yourself an item.")]
	[CommandPermissions(Permissions.item)]
	[AllowServer(false)]
	public void Item(string itemNameOrId, int itemAmount = 0, string? prefixidOrName = null)
	{
		if (itemNameOrId is null)
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}item <item name/id> [item amount] [prefix id/name]", TextOptions.CommandPrefix);
			return;
		}

		//int amountParamIndex = -1;
		//int itemAmount = 0;
		//for (int i = 1; i < args.Parameters.Count; i++)
		//{
		//	if (int.TryParse(args.Parameters[i], out itemAmount))
		//	{
		//		amountParamIndex = i;
		//		break;
		//	}
		//}

		//string itemNameOrId;
		//if (amountParamIndex == -1)
		//	itemNameOrId = string.Join(" ", args.Parameters);
		//else
		//	itemNameOrId = string.Join(" ", args.Parameters.Take(amountParamIndex));

		Item item;
		List<Item> matchedItems = TShock.Utils.GetItemByIdOrName(itemNameOrId);
		if (matchedItems.Count == 0)
		{
			Sender.SendErrorMessage("Invalid item type!");
			return;
		}
		else if (matchedItems.Count > 1)
		{
			Sender.SendMultipleMatchError(matchedItems.Select(i => $"{i.Name}({i.netID})"));
			return;
		}
		else
		{
			item = matchedItems[0];
		}
		if (item.type < 1 && item.type >= Main.maxItemTypes)
		{
			Sender.SendErrorMessage("The item type {0} is invalid.", itemNameOrId);
			return;
		}

		int prefixId = 0;
		if (!String.IsNullOrWhiteSpace(prefixidOrName))
		{
			var prefixIds = TShock.Utils.GetPrefixByIdOrName(prefixidOrName);

			if (item.accessory && prefixIds.Contains(PrefixID.Quick))
			{
				prefixIds.Remove(PrefixID.Quick);
				prefixIds.Remove(PrefixID.Quick2);
				prefixIds.Add(PrefixID.Quick2);
			}
			else if (!item.accessory && prefixIds.Contains(PrefixID.Quick))
				prefixIds.Remove(PrefixID.Quick2);

			if (prefixIds.Count > 1)
			{
				Sender.SendMultipleMatchError(prefixIds.Select(p => p.ToString()));
				return;
			}
			else if (prefixIds.Count == 0)
			{
				Sender.SendErrorMessage("No prefix matched \"{0}\".", prefixidOrName);
				return;
			}
			else
			{
				prefixId = prefixIds[0];
			}
		}

		if (Sender.InventorySlotAvailable || (item.type > 70 && item.type < 75) || item.ammo > 0 || item.type == 58 || item.type == 184)
		{
			if (itemAmount == 0 || itemAmount > item.maxStack)
				itemAmount = item.maxStack;

			if (Sender.GiveItemCheck(item.type, EnglishLanguage.GetItemNameById(item.type), itemAmount, prefixId))
			{
				item.prefix = (byte)prefixId;
				Sender.SendSuccessMessage("Gave {0} {1}(s).", itemAmount, item.AffixName());
			}
			else
			{
				Sender.SendErrorMessage("You cannot spawn banned items.");
			}
		}
		else
		{
			Sender.SendErrorMessage("Your inventory seems full.");
		}
	}
}
