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

[Command("projban")]
[CommandPermissions(Permissions.manageprojectile)]
[HelpText("Manages projectile bans.")]
class ProjectileBanCommands : CommandCallbacks<TSPlayer>
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
									"add <projectile ID> - Adds a projectile ban.",
									"allow <projectile ID> <group> - Allows a group to use a projectile.",
									"del <projectile ID> - Deletes an projectile ban.",
									"disallow <projectile ID> <group> - Disallows a group from using a projectile.",
									"list [page] - Lists all projectile bans."
								};

		PaginationTools.SendPage(Sender, pageNumber, lines,
			new PaginationTools.Settings
			{
				HeaderFormat = "Projectile Ban Sub-Commands ({0}/{1}):",
				FooterFormat = "Type {0}projban help {{0}} for more sub-commands.".SFormat(TextOptions.CommandPrefix)
			}
		);
	}

	[SubCommand("add")]
	public void Add(string? projectileId = null)
	{
		if (String.IsNullOrWhiteSpace(projectileId))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}projban add <proj id>", TextOptions.CommandPrefix);
			return;
		}
		short id;
		if (Int16.TryParse(projectileId, out id) && id > 0 && id < Main.maxProjectileTypes)
		{
			TShock.ProjectileBans.AddNewBan(id);
			Sender.SendSuccessMessage("Banned projectile {0}.", id);
		}
		else
			Sender.SendErrorMessage("Invalid projectile ID!");
	}

	[SubCommand("allow")]
	public void Allow(string? projectileId = null, string? group = null)
	{
		if (String.IsNullOrWhiteSpace(projectileId) || String.IsNullOrWhiteSpace(group))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}projban allow <id> <group>", TextOptions.CommandPrefix);
			return;
		}

		short id;
		if (Int16.TryParse(projectileId, out id) && id > 0 && id < Main.maxProjectileTypes)
		{
			if (!TShock.Groups.GroupExists(group))
			{
				Sender.SendErrorMessage("Invalid group.");
				return;
			}

			ProjectileBan ban = TShock.ProjectileBans.GetBanById(id);
			if (ban == null)
			{
				Sender.SendErrorMessage("Projectile {0} is not banned.", id);
				return;
			}
			if (!ban.AllowedGroups.Contains(group))
			{
				TShock.ProjectileBans.AllowGroup(id, group);
				Sender.SendSuccessMessage("{0} has been allowed to use projectile {1}.", group, id);
			}
			else
				Sender.SendWarningMessage("{0} is already allowed to use projectile {1}.", group, id);
		}
		else
			Sender.SendErrorMessage("Invalid projectile ID!");
	}

	[SubCommand("del")]
	public void Delete(string? projectileId = null)
	{
		if (String.IsNullOrWhiteSpace(projectileId))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}projban del <id>", TextOptions.CommandPrefix);
			return;
		}

		short id;
		if (Int16.TryParse(projectileId, out id) && id > 0 && id < Main.maxProjectileTypes)
		{
			TShock.ProjectileBans.RemoveBan(id);
			Sender.SendSuccessMessage("Unbanned projectile {0}.", id);
			return;
		}
		else
			Sender.SendErrorMessage("Invalid projectile ID!");
	}

	[SubCommand("disallow")]
	public void Disallow(string? projectileId = null, string? group = null)
	{
		if (String.IsNullOrWhiteSpace(projectileId) || String.IsNullOrWhiteSpace(group))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}projban disallow <id> <group name>", TextOptions.CommandPrefix);
			return;
		}

		short id;
		if (Int16.TryParse(projectileId, out id) && id > 0 && id < Main.maxProjectileTypes)
		{
			if (!TShock.Groups.GroupExists(group))
			{
				Sender.SendErrorMessage("Invalid group.");
				return;
			}

			ProjectileBan ban = TShock.ProjectileBans.GetBanById(id);
			if (ban == null)
			{
				Sender.SendErrorMessage("Projectile {0} is not banned.", id);
				return;
			}
			if (ban.AllowedGroups.Contains(group))
			{
				TShock.ProjectileBans.RemoveGroup(id, group);
				Sender.SendSuccessMessage("{0} has been disallowed from using projectile {1}.", group, id);
				return;
			}
			else
				Sender.SendWarningMessage("{0} is already prevented from using projectile {1}.", group, id);
		}
		else
			Sender.SendErrorMessage("Invalid projectile ID!");
	}

	[SubCommand("list")]
	public void List([PageNumber] int pageNumber = 1)
	{
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out pageNumber))
		//	return;
		IEnumerable<Int16> projectileIds = from projectileBan in TShock.ProjectileBans.ProjectileBans
										   select projectileBan.ID;
		PaginationTools.SendPage(Sender, pageNumber, PaginationTools.BuildLinesFromTerms(projectileIds),
			new PaginationTools.Settings
			{
				HeaderFormat = "Projectile bans ({0}/{1}):",
				FooterFormat = "Type {0}projban list {{0}} for more.".SFormat(TextOptions.CommandPrefix),
				NothingToDisplayString = "There are currently no banned projectiles."
			});
	}
}
