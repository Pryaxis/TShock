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
using TShockAPI.DB;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

[Command("group")]
[HelpText("Manages groups.")]
[CommandPermissions(Permissions.managegroup)]
class GroupCommands : CommandCallbacks<TSPlayer>
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
			"add <name> <permissions...> - Adds a new group.",
			"addperm <group> <permissions...> - Adds permissions to a group.",
			"color <group> <rrr,ggg,bbb> - Changes a group's chat color.",
			"rename <group> <new name> - Changes a group's name.",
			"del <group> - Deletes a group.",
			"delperm <group> <permissions...> - Removes permissions from a group.",
			"list [page] - Lists groups.",
			"listperm <group> [page] - Lists a group's permissions.",
			"parent <group> <parent group> - Changes a group's parent group.",
			"prefix <group> <prefix> - Changes a group's prefix.",
			"suffix <group> <suffix> - Changes a group's suffix."
		};

		PaginationTools.SendPage(Sender, pageNumber, lines,
			new PaginationTools.Settings
			{
				HeaderFormat = "Group Sub-Commands ({0}/{1}):",
				FooterFormat = "Type {0}group help {{0}} for more sub-commands.".SFormat(TextOptions.CommandPrefix)
			}
		);
	}

	[SubCommand("add")]
	public void Add(string groupName, [AllowSpaces] string permissions)
	{
		if (String.IsNullOrWhiteSpace(groupName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group add <group name> [permissions csv]", TextOptions.CommandPrefix);
			return;
		}

		//string groupName = args.Parameters[1];
		//args.Parameters.RemoveRange(0, 2);
		//string permissions = String.Join(",", args.Parameters);
		var nodes = String.Join(",", permissions.Split(' '));

		try
		{
			TShock.Groups.AddGroup(groupName, null, nodes, TShockAPI.Group.defaultChatColor);
			Sender.SendSuccessMessage("The group was added successfully!");
		}
		catch (GroupExistsException)
		{
			Sender.SendErrorMessage("That group already exists!");
		}
		catch (GroupManagerException ex)
		{
			Sender.SendErrorMessage(ex.ToString());
		}
	}

	[SubCommand("addperm")]
	public void AddPerm(string groupName, [AllowSpaces] string permissions)
	{
		if (String.IsNullOrWhiteSpace(groupName) || String.IsNullOrWhiteSpace(permissions))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group addperm <group name> <permissions...>", TextOptions.CommandPrefix);
			return;
		}

		var permlist = permissions.Split(' ').ToList();

		//string groupName = args.Parameters[1];
		//args.Parameters.RemoveRange(0, 2);
		if (groupName == "*")
		{
			foreach (Group g in TShock.Groups)
			{
				TShock.Groups.AddPermissions(g.Name, permlist);
			}
			Sender.SendSuccessMessage("Modified all groups.");
			return;
		}
		try
		{
			string response = TShock.Groups.AddPermissions(groupName, permlist);
			if (response.Length > 0)
			{
				Sender.SendSuccessMessage(response);
			}
			return;
		}
		catch (GroupManagerException ex)
		{
			Sender.SendErrorMessage(ex.ToString());
		}
	}

	[SubCommand("parent")]
	public void Parent(string groupName, [AllowSpaces] string? parent = null)
	{
		if (String.IsNullOrWhiteSpace(groupName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group parent <group name> [new parent group name]", TextOptions.CommandPrefix);
			return;
		}

		//string groupName = args.Parameters[1];
		Group group = TShock.Groups.GetGroupByName(groupName);
		if (group == null)
		{
			Sender.SendErrorMessage("No such group \"{0}\".", groupName);
			return;
		}

		if (!String.IsNullOrWhiteSpace(parent))
		{
			string newParentGroupName = parent; // string.Join(" ", args.Parameters.Skip(2));
			if (!string.IsNullOrWhiteSpace(newParentGroupName) && !TShock.Groups.GroupExists(newParentGroupName))
			{
				Sender.SendErrorMessage("No such group \"{0}\".", newParentGroupName);
				return;
			}

			try
			{
				TShock.Groups.UpdateGroup(groupName, newParentGroupName, group.Permissions, group.ChatColor, group.Suffix, group.Prefix);

				if (!string.IsNullOrWhiteSpace(newParentGroupName))
					Sender.SendSuccessMessage("Parent of group \"{0}\" set to \"{1}\".", groupName, newParentGroupName);
				else
					Sender.SendSuccessMessage("Removed parent of group \"{0}\".", groupName);
			}
			catch (GroupManagerException ex)
			{
				Sender.SendErrorMessage(ex.Message);
			}
		}
		else
		{
			if (group.Parent != null)
				Sender.SendSuccessMessage("Parent of \"{0}\" is \"{1}\".", group.Name, group.Parent.Name);
			else
				Sender.SendSuccessMessage("Group \"{0}\" has no parent.", group.Name);
		}
	}

	[SubCommand("suffix")]
	public void Suffix(string groupName, [AllowSpaces] string? suffix = null)
	{
		if (String.IsNullOrWhiteSpace(groupName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group suffix <group name> [new suffix]", TextOptions.CommandPrefix);
			return;
		}

		//string groupName = args.Parameters[1];
		Group group = TShock.Groups.GetGroupByName(groupName);
		if (group == null)
		{
			Sender.SendErrorMessage("No such group \"{0}\".", groupName);
			return;
		}

		if (!String.IsNullOrWhiteSpace(suffix))
		{
			string newSuffix = suffix; // string.Join(" ", args.Parameters.Skip(2));

			try
			{
				TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, group.ChatColor, newSuffix, group.Prefix);

				if (!string.IsNullOrWhiteSpace(newSuffix))
					Sender.SendSuccessMessage("Suffix of group \"{0}\" set to \"{1}\".", groupName, newSuffix);
				else
					Sender.SendSuccessMessage("Removed suffix of group \"{0}\".", groupName);
			}
			catch (GroupManagerException ex)
			{
				Sender.SendErrorMessage(ex.Message);
			}
		}
		else
		{
			if (!string.IsNullOrWhiteSpace(group.Suffix))
				Sender.SendSuccessMessage("Suffix of \"{0}\" is \"{1}\".", group.Name, group.Suffix);
			else
				Sender.SendSuccessMessage("Group \"{0}\" has no suffix.", group.Name);
		}
	}

	[SubCommand("prefix")]
	public void Prefix(string groupName, [AllowSpaces] string? prefix = null)
	{
		if (String.IsNullOrWhiteSpace(groupName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group prefix <group name> [new prefix]", TextOptions.CommandPrefix);
			return;
		}

		//string groupName = args.Parameters[1];
		Group group = TShock.Groups.GetGroupByName(groupName);
		if (group == null)
		{
			Sender.SendErrorMessage("No such group \"{0}\".", groupName);
			return;
		}

		if (!String.IsNullOrWhiteSpace(prefix))
		{
			string newPrefix = prefix; // string.Join(" ", args.Parameters.Skip(2));

			try
			{
				TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, group.ChatColor, group.Suffix, newPrefix);

				if (!string.IsNullOrWhiteSpace(newPrefix))
					Sender.SendSuccessMessage("Prefix of group \"{0}\" set to \"{1}\".", groupName, newPrefix);
				else
					Sender.SendSuccessMessage("Removed prefix of group \"{0}\".", groupName);
			}
			catch (GroupManagerException ex)
			{
				Sender.SendErrorMessage(ex.Message);
			}
		}
		else
		{
			if (!string.IsNullOrWhiteSpace(group.Prefix))
				Sender.SendSuccessMessage("Prefix of \"{0}\" is \"{1}\".", group.Name, group.Prefix);
			else
				Sender.SendSuccessMessage("Group \"{0}\" has no prefix.", group.Name);
		}
	}

	[SubCommand("color", "colour")]
	public void Color(string groupName, [AllowSpaces] string? color = null)
	{
		if (String.IsNullOrWhiteSpace(groupName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group color <group name> [new color(000,000,000)]", TextOptions.CommandPrefix);
			return;
		}

		//string groupName = args.Parameters[1];
		Group group = TShock.Groups.GetGroupByName(groupName);
		if (group == null)
		{
			Sender.SendErrorMessage("No such group \"{0}\".", groupName);
			return;
		}

		if (!String.IsNullOrWhiteSpace(color))
		{
			string newColor = color; // args.Parameters[2];

			String[] parts = newColor.Split(',');
			byte r;
			byte g;
			byte b;
			if (parts.Length == 3 && byte.TryParse(parts[0], out r) && byte.TryParse(parts[1], out g) && byte.TryParse(parts[2], out b))
			{
				try
				{
					TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, newColor, group.Suffix, group.Prefix);

					Sender.SendSuccessMessage("Color of group \"{0}\" set to \"{1}\".", groupName, newColor);
				}
				catch (GroupManagerException ex)
				{
					Sender.SendErrorMessage(ex.Message);
				}
			}
			else
			{
				Sender.SendErrorMessage("Invalid syntax for color, expected \"rrr,ggg,bbb\"");
			}
		}
		else
		{
			Sender.SendSuccessMessage("Color of \"{0}\" is \"{1}\".", group.Name, group.ChatColor);
		}
	}

	[SubCommand("rename")]
	public void Rename(string? groupName = null, string? newName = null)
	{
		if (String.IsNullOrWhiteSpace(groupName) || String.IsNullOrWhiteSpace(newName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group rename <group> <new name>", TextOptions.CommandPrefix);
			return;
		}

		//string group = args.Parameters[1];
		//string newName = args.Parameters[2];
		try
		{
			string response = TShock.Groups.RenameGroup(groupName, newName);
			Sender.SendSuccessMessage(response);
		}
		catch (GroupManagerException ex)
		{
			Sender.SendErrorMessage(ex.Message);
		}
	}

	[SubCommand("del", "delete")]
	public void Delete(string? groupName = null)
	{
		if (String.IsNullOrWhiteSpace(groupName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group del <group name>", TextOptions.CommandPrefix);
			return;
		}

		try
		{
			string response = TShock.Groups.DeleteGroup(groupName, true);
			if (response.Length > 0)
			{
				Sender.SendSuccessMessage(response);
			}
		}
		catch (GroupManagerException ex)
		{
			Sender.SendErrorMessage(ex.Message);
		}
	}

	[SubCommand("delperm", "deleteperm")]
	public void DeletePerm(string groupName, [AllowSpaces] string permissions)
	{
		if (String.IsNullOrWhiteSpace(groupName) || String.IsNullOrWhiteSpace(permissions))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group delperm <group name> <permissions...>", TextOptions.CommandPrefix);
			return;
		}

		var permlist = permissions.Split(' ').ToList();
		//string groupName = args.Parameters[1];
		//args.Parameters.RemoveRange(0, 2);
		if (groupName == "*")
		{
			foreach (Group g in TShock.Groups)
			{
				TShock.Groups.DeletePermissions(g.Name, permlist);
			}
			Sender.SendSuccessMessage("Modified all groups.");
			return;
		}
		try
		{
			string response = TShock.Groups.DeletePermissions(groupName, permlist);
			if (response.Length > 0)
			{
				Sender.SendSuccessMessage(response);
			}
			return;
		}
		catch (GroupManagerException ex)
		{
			Sender.SendErrorMessage(ex.Message);
		}
	}

	[SubCommand("list")]
	public void List([PageNumber] int pageNumber = 1)
	{
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out pageNumber))
		//	return;
		var groupNames = from grp in TShock.Groups.groups
						 select grp.Name;
		PaginationTools.SendPage(Sender, pageNumber, PaginationTools.BuildLinesFromTerms(groupNames),
			new PaginationTools.Settings
			{
				HeaderFormat = "Groups ({0}/{1}):",
				FooterFormat = "Type {0}group list {{0}} for more.".SFormat(TextOptions.CommandPrefix)
			});
	}

	[SubCommand("listperm")]
	public void ListPerm(string? groupName = null, [PageNumber] int pageNumber = 1)
	{
		if (String.IsNullOrWhiteSpace(groupName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}group listperm <group name> [page]", TextOptions.CommandPrefix);
			return;
		}
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, Sender, out pageNumber))
		//	return;

		if (!TShock.Groups.GroupExists(groupName))
		{
			Sender.SendErrorMessage("Invalid group.");
			return;
		}
		Group grp = TShock.Groups.GetGroupByName(groupName);
		List<string> permissions = grp.TotalPermissions;

		PaginationTools.SendPage(Sender, pageNumber, PaginationTools.BuildLinesFromTerms(permissions),
			new PaginationTools.Settings
			{
				HeaderFormat = "Permissions for " + grp.Name + " ({0}/{1}):",
				FooterFormat = "Type {0}group listperm {1} {{0}} for more.".SFormat(TextOptions.CommandPrefix, grp.Name),
				NothingToDisplayString = "There are currently no permissions for " + grp.Name + "."
			});
	}
}
