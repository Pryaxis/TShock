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

/**
 *  Note:
 *  To avoid reinventing the wheel, this file has been taken from the following link:
 *  https://github.com/ZakFahey/easy-commands-tshock/blob/master/EasyCommandsTShock/EasyCommandsTShock/TShockParsingRules.cs
 */

using EasyCommands;
using EasyCommands.Arguments;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using TShockCommands.Annotations;

namespace TShockCommands;

class ParsingRules : ParsingRules<TSPlayer>
{
	string FailMessage(string message, bool showProperSyntax = true)
	{
		string text = string.Format(message, ParameterName);
		if (showProperSyntax)
			text = text + "\n" + string.Format(TextOptions.ProperSyntax, ProperSyntax);

		return text;
	}

	[ParseRule]
	public TSPlayer ParsePlayer(string arg)
	{
		var players = TSPlayer.FindByNameOrID(arg);

		if (players.Count == 0)
			throw new CommandParsingException(FailMessage("Invalid player!", false));

		if (players.Count > 1)
			throw new CommandParsingException(FailMessage("More than one match found:\n" + string.Join(", ", players.Select(p => p.Name)), false));

		return players[0];
	}

	[ParseRule]
	public UserAccount ParseUser(string arg)
	{
		UserAccount? user = TShock.UserAccounts.GetUserAccountByName(arg);

		if (user is null)
			throw new CommandParsingException(FailMessage($"User {arg} does not exist.", false));

		return user;
	}

	[ParseRule]
	public Item ParseItem(string arg)
	{
		var items = TShock.Utils.GetItemByIdOrName(arg);
		if (items.Count == 0)
			throw new CommandParsingException(FailMessage("Invalid item.", false));

		if (items.Count > 1)
			throw new CommandParsingException(FailMessage("More than one match found:\n" + string.Join(", ", items.Select(i => i.Name)), false));

		if (items[0].type < 1 || items[0].type >= Main.maxItemTypes)
			throw new CommandParsingException(FailMessage("Invalid item type!"));

		return items[0];
	}

	[ParseRule]
	public Group ParseGroup(string arg)
	{
		var group = TShock.Groups.FirstOrDefault(x => x.Name == arg);
		if (group == null)
			throw new CommandParsingException(FailMessage($"Group {arg} does not exist.", false));

		return group;
	}

	[ParseRule]
	public NPC ParseNPC(string arg)
	{
		var npcs = TShock.Utils.GetNPCByIdOrName(arg);
		if (npcs.Count == 0)
			throw new CommandParsingException(FailMessage("Invalid mob type!", false));

		if (npcs.Count > 1)
			throw new CommandParsingException(FailMessage("More than one match found:\n" + string.Join(", ", npcs.Select(n => $"{n.FullName}({n.type})")), false));

		return npcs[0];
	}

	[ParseRule]
	public Region ParseRegion(string arg)
	{
		Region region = TShock.Regions.GetRegionByName(arg);
		int id;
		if (region == null && int.TryParse(arg, out id))
		{
			region = TShock.Regions.GetRegionByID(id);
		}
		if (region == null)
			throw new CommandParsingException(FailMessage($"Region \"{arg}\" does not exist."));

		return region;
	}

	[ParseRule]
	public Color ParseColor(string arg)
	{
		// First check by name
		Color color;
		if (ColorNames.GetColorFromName(arg, out color))
		{
			return color;
		}

		// Then check by rgb
		string[] rgb = arg.Split(',').Select(x => x.Trim()).ToArray();
		if (rgb.Length == 3)
		{
			try
			{
				return new Color(int.Parse(rgb[0]), int.Parse(rgb[1]), int.Parse(rgb[2]));
			}
			catch { }
		}

		// Then check by hex code
		if (arg.StartsWith("#") && arg.Length == 7)
		{
			uint hexNum = uint.Parse(arg.Substring(1), NumberStyles.HexNumber);
			return new Color(hexNum);
		}

		// Nothing works, so invalid argument
		Fail("{0} must be a valid color! You can use a color name, comma-separated RGB triplet, or hex value.");
		return new Color();
	}

	[ParseRule]
	public int ParseTeam(string arg, TeamColor attributeOverride)
	{
		return arg.ToLower() switch
		{
			"none" or "white" => 0,
			"red" => 1,
			"green" => 2,
			"blue" => 3,
			"yellow" => 4,
			"pink" => 5,
			_ => throw new CommandParsingException(FailMessage("Invalid syntax! {0} must be a team color!"))
		};
	}

	[ParseRule]
	public int ParseItemPrefix(string arg, ItemPrefix attributeOverride)
	{
		/* TODO: In TShock's implementation, there's this check:

		if (item.accessory && prefixIds.Contains(PrefixID.Quick))
			{
				prefixIds.Remove(PrefixID.Quick);
				prefixIds.Remove(PrefixID.Quick2);
				prefixIds.Add(PrefixID.Quick2);
			}
			else if (!item.accessory && prefixIds.Contains(PrefixID.Quick))
				prefixIds.Remove(PrefixID.Quick2);

		Is it necessary to remove this? And if so, how do you get the item context? */

		List<int> prefixes = TShock.Utils.GetPrefixByIdOrName(arg);
		if (prefixes.Count == 0)
			throw new CommandParsingException(FailMessage($"No prefix matched \"{arg}\".", false));
		if (prefixes.Count > 1)
			throw new CommandParsingException(FailMessage("More than one match found:\n" + string.Join(", ", prefixes), false));
		return prefixes[0];
	}

	[ParseRule]
	public int ParseBuff(string arg, Buff attributeOverride)
	{
		List<int> buffs = TShock.Utils.GetBuffByName(arg);
		if (buffs.Count == 0)
			throw new CommandParsingException(FailMessage("Invalid buff name!", false));
		else if (buffs.Count > 1)
			throw new CommandParsingException(FailMessage("More than one match found:\n" + string.Join(", ", buffs.Select(f => Lang.GetBuffName(f))), false));
		if (buffs[0] <= 0 || buffs[0] >= Main.maxBuffTypes)
			throw new CommandParsingException(FailMessage("Invalid buff ID!"));
		return buffs[0];
	}

	[ParseRule]
	public int ParsePageNumber(string arg, PageNumber attributeOverride)
	{
		int pageNumber = 1;
		if (!String.IsNullOrWhiteSpace(arg) // is the arg specified?
			&& (
				!int.TryParse(arg, out pageNumber) || pageNumber < 1
			)
		)
		{
			//throw new CommandParsingException(FailMessage(attributeOverride.ErrorMessage ?? $"\"{arg}\" is not a valid page number."));
			return -1;
		}

		return pageNumber;
	}

	[ParseRule]
	public int ParseDuration(string arg, Duration attributeOverride)
	{
		if (arg != ".") // allow . to be used to specify the default
		{
			if (TShock.Utils.TryParseTime(arg, out int seconds))
			{
				return seconds;
			}
		}
		return DateTime.MaxValue.Second;
	}
}
