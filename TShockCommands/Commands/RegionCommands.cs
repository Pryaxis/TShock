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
using System.Threading;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using TShockCommands.Annotations;
using Utils = TShockAPI.Utils;

namespace TShockCommands.Commands;

[FlagsArgument]
public class RegionInfoFlags
{
	[FlagParams("-d")]
	public bool DisplayBoundaries = false;

	[FlagParams("-p")]
	public int PageNumber = 1;
}

[Command("region")]
[HelpText("Manages regions.")]
[CommandPermissions(Permissions.manageregion)]
class RegionCommands : CommandCallbacks<TSPlayer>
{
	[SubCommand(SubCommandType.Default)]
	public void Default([PageNumber] int pageNumber = 1) => Help(pageNumber);

	[SubCommand("help")]
	public void Help([PageNumber] int pageNumber = 1)
	{
		//int pageNumber;
		//int pageParamIndex = 0;
		//if (args.Parameters.Count > 1)
		//	pageParamIndex = 1;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, pageParamIndex, Sender, out pageNumber))
		//	return;

		List<string> lines = new List<string> {
			"set <1/2> - Sets the temporary region points.",
			"clear - Clears the temporary region points.",
			"define <name> - Defines the region with the given name.",
			"delete <name> - Deletes the given region.",
			"name [-u][-z][-p] - Shows the name of the region at the given point.",
			"rename <region> <new name> - Renames the given region.",
			"list - Lists all regions.",
			"resize <region> <u/d/l/r> <amount> - Resizes a region.",
			"allow <user> <region> - Allows a user to a region.",
			"remove <user> <region> - Removes a user from a region.",
			"allowg <group> <region> - Allows a user group to a region.",
			"removeg <group> <region> - Removes a user group from a region.",
			"info <region> [-d] - Displays several information about the given region.",
			"protect <name> <true/false> - Sets whether the tiles inside the region are protected or not.",
			"z <name> <#> - Sets the z-order of the region.",
		};
		if (Sender.HasPermission(Permissions.tp))
			lines.Add("tp <region> - Teleports you to the given region's center.");

		PaginationTools.SendPage(
		  Sender, pageNumber, lines,
		  new PaginationTools.Settings
		  {
			  HeaderFormat = "Available Region Sub-Commands ({0}/{1}):",
			  FooterFormat = "Type {0}region {{0}} for more sub-commands.".SFormat(TextOptions.CommandPrefix)
		  }
		);
	}

	[SubCommand("name")]
	public void Name([AllowSpaces] string? name = null)
	{
		Sender.SendInfoMessage("Hit a block to get the name of the region");
		Sender.AwaitingName = true;
		Sender.AwaitingNameParameters = (name ?? "").Split(' ');
	}

	[SubCommand("set")]
	public void Set(int choice)
	{
		//int choice = 0;
		if (/*args.Parameters.Count == 2 &&
			int.TryParse(args.Parameters[1], out choice) &&*/
			choice >= 1 && choice <= 2)
		{
			Sender.SendInfoMessage("Hit a block to Set Point " + choice);
			Sender.AwaitingTempPoint = choice;
		}
		else
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: /region set <1/2>");
		}
	}

	[SubCommand("define")]
	public void Define([AllowSpaces] string regionName)
	{
		if (!String.IsNullOrWhiteSpace(regionName))
		{
			if (!Sender.TempPoints.Any(p => p == Point.Zero))
			{
				//string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
				var x = Math.Min(Sender.TempPoints[0].X, Sender.TempPoints[1].X);
				var y = Math.Min(Sender.TempPoints[0].Y, Sender.TempPoints[1].Y);
				var width = Math.Abs(Sender.TempPoints[0].X - Sender.TempPoints[1].X);
				var height = Math.Abs(Sender.TempPoints[0].Y - Sender.TempPoints[1].Y);

				if (TShock.Regions.AddRegion(x, y, width, height, regionName, Sender.Account.Name,
											 Main.worldID.ToString()))
				{
					Sender.TempPoints[0] = Point.Zero;
					Sender.TempPoints[1] = Point.Zero;
					Sender.SendInfoMessage("Set region " + regionName);
				}
				else
				{
					Sender.SendErrorMessage("Region " + regionName + " already exists");
				}
			}
			else
			{
				Sender.SendErrorMessage("Points not set up yet");
			}
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region define <name>", TextOptions.CommandPrefix);
	}

	[SubCommand("protect")]
	public void Protect(string regionName, bool protect)
	{
		if (TShock.Regions.SetRegionState(regionName, protect))
			Sender.SendInfoMessage($"{(protect ? "P" : "Unp")}rotected region {regionName}");
		else
			Sender.SendErrorMessage("Could not find specified region");

		//if (args.Parameters.Count == 3)
		//{
		//	string regionName = args.Parameters[1];
		//	if (args.Parameters[2].ToLower() == "true")
		//	{
		//		if (TShock.Regions.SetRegionState(regionName, true))
		//			Sender.SendInfoMessage("Protected region " + regionName);
		//		else
		//			Sender.SendErrorMessage("Could not find specified region");
		//	}
		//	else if (args.Parameters[2].ToLower() == "false")
		//	{
		//		if (TShock.Regions.SetRegionState(regionName, false))
		//			Sender.SendInfoMessage("Unprotected region " + regionName);
		//		else
		//			Sender.SendErrorMessage("Could not find specified region");
		//	}
		//	else
		//		Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region protect <name> <true/false>", TextOptions.CommandPrefix);
		//}
		//else
		//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: /region protect <name> <true/false>", TextOptions.CommandPrefix);
	}

	[SubCommand("delete")]
	public void Delete([AllowSpaces] string regionName)
	{
		if (!String.IsNullOrWhiteSpace(regionName))
		{
			//string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
			if (TShock.Regions.DeleteRegion(regionName))
			{
				Sender.SendInfoMessage("Deleted region \"{0}\".", regionName);
			}
			else
				Sender.SendErrorMessage("Could not find the specified region!");
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region delete <name>", TextOptions.CommandPrefix);
	}

	[SubCommand("clear")]
	public void Clear()
	{
		Sender.TempPoints[0] = Point.Zero;
		Sender.TempPoints[1] = Point.Zero;
		Sender.SendInfoMessage("Cleared temporary points.");
		Sender.AwaitingTempPoint = 0;
	}

	[SubCommand("allow")]
	public void Allow(string? playerName = null, string? regionName = null)
	{
		if (!String.IsNullOrWhiteSpace(playerName) && !String.IsNullOrWhiteSpace(regionName))
		{
			//string playerName = args.Parameters[1];
			//string regionName = "";

			//for (int i = 2; i < args.Parameters.Count; i++)
			//{
			//	if (regionName == "")
			//	{
			//		regionName = args.Parameters[2];
			//	}
			//	else
			//	{
			//		regionName = regionName + " " + args.Parameters[i];
			//	}
			//}
			if (TShock.UserAccounts.GetUserAccountByName(playerName) != null)
			{
				if (TShock.Regions.AddNewUser(regionName, playerName))
				{
					Sender.SendInfoMessage("Added user " + playerName + " to " + regionName);
				}
				else
					Sender.SendErrorMessage("Region " + regionName + " not found");
			}
			else
			{
				Sender.SendErrorMessage("Player " + playerName + " not found");
			}
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region allow <player name> <region>", TextOptions.CommandPrefix);
	}

	[SubCommand("remove")]
	public void Remove(string? playerName = null, string? regionName = null)
	{
		if (!String.IsNullOrWhiteSpace(playerName) && !String.IsNullOrWhiteSpace(regionName))
		{
			//string playerName = args.Parameters[1];
			//string regionName = "";

			//for (int i = 2; i < args.Parameters.Count; i++)
			//{
			//	if (regionName == "")
			//	{
			//		regionName = args.Parameters[2];
			//	}
			//	else
			//	{
			//		regionName = regionName + " " + args.Parameters[i];
			//	}
			//}
			if (TShock.UserAccounts.GetUserAccountByName(playerName) != null)
			{
				if (TShock.Regions.RemoveUser(regionName, playerName))
				{
					Sender.SendInfoMessage("Removed user " + playerName + " from " + regionName);
				}
				else
					Sender.SendErrorMessage("Region " + regionName + " not found");
			}
			else
			{
				Sender.SendErrorMessage("Player " + playerName + " not found");
			}
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region remove <name> <region>", TextOptions.CommandPrefix);
	}

	[SubCommand("allowg")]
	public void AllowGroup(string? group = null, string? regionName = null)
	{
		if (!String.IsNullOrWhiteSpace(group) && !String.IsNullOrWhiteSpace(regionName))
		{
			//string group = args.Parameters[1];
			//string regionName = "";

			//for (int i = 2; i < args.Parameters.Count; i++)
			//{
			//	if (regionName == "")
			//	{
			//		regionName = args.Parameters[2];
			//	}
			//	else
			//	{
			//		regionName = regionName + " " + args.Parameters[i];
			//	}
			//}
			if (TShock.Groups.GroupExists(group))
			{
				if (TShock.Regions.AllowGroup(regionName, group))
				{
					Sender.SendInfoMessage("Added group " + group + " to " + regionName);
				}
				else
					Sender.SendErrorMessage("Region " + regionName + " not found");
			}
			else
			{
				Sender.SendErrorMessage("Group " + group + " not found");
			}
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region allowg <group> <region>", TextOptions.CommandPrefix);
	}

	[SubCommand("removeg")]
	public void RemoveGroup(string? group = null, string? regionName = null)
	{
		if (!String.IsNullOrWhiteSpace(group) && !String.IsNullOrWhiteSpace(regionName))
		{
			//string group = args.Parameters[1];
			//string regionName = "";

			//for (int i = 2; i < args.Parameters.Count; i++)
			//{
			//	if (regionName == "")
			//	{
			//		regionName = args.Parameters[2];
			//	}
			//	else
			//	{
			//		regionName = regionName + " " + args.Parameters[i];
			//	}
			//}
			if (TShock.Groups.GroupExists(group))
			{
				if (TShock.Regions.RemoveGroup(regionName, group))
				{
					Sender.SendInfoMessage("Removed group " + group + " from " + regionName);
				}
				else
					Sender.SendErrorMessage("Region " + regionName + " not found");
			}
			else
			{
				Sender.SendErrorMessage("Group " + group + " not found");
			}
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region removeg <group> <region>", TextOptions.CommandPrefix);
	}

	[SubCommand("list")]
	public void List([PageNumber] int pageNumber = 1)
	{
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out pageNumber))
		//	return;

		IEnumerable<string> regionNames = from region in TShock.Regions.Regions
										  where region.WorldID == Main.worldID.ToString()
										  select region.Name;
		PaginationTools.SendPage(Sender, pageNumber, PaginationTools.BuildLinesFromTerms(regionNames),
			new PaginationTools.Settings
			{
				HeaderFormat = "Regions ({0}/{1}):",
				FooterFormat = "Type {0}region list {{0}} for more.".SFormat(TextOptions.CommandPrefix),
				NothingToDisplayString = "There are currently no regions defined."
			});
	}

	[SubCommand("info")]
	public void Info(string regionName, RegionInfoFlags flags)
	{
		if (String.IsNullOrWhiteSpace(regionName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region info <region> [-d] [-page n]", TextOptions.CommandPrefix);
			return;
		}

		//string regionName = args.Parameters[1];
		//bool displayBoundaries = args.Parameters.Skip(2).Any(
		//	p => p.Equals("-d", StringComparison.InvariantCultureIgnoreCase)
		//);

		Region region = TShock.Regions.GetRegionByName(regionName);
		if (region == null)
		{
			Sender.SendErrorMessage("Region \"{0}\" does not exist.", regionName);
			return;
		}

		//int pageNumberIndex = displayBoundaries ? 3 : 2;
		//int pageNumber;
		//if (!PaginationTools.TryParsePageNumber(args.Parameters, pageNumberIndex, Sender, out pageNumber))
		//	break;

		List<string> lines = new List<string>
		{
			string.Format("X: {0}; Y: {1}; W: {2}; H: {3}, Z: {4}", region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Z),
			string.Concat("Owner: ", region.Owner),
			string.Concat("Protected: ", region.DisableBuild.ToString()),
		};

		if (region.AllowedIDs.Count > 0)
		{
			IEnumerable<string> sharedUsersSelector = region.AllowedIDs.Select(userId =>
			{
				UserAccount account = TShock.UserAccounts.GetUserAccountByID(userId);
				if (account != null)
					return account.Name;

				return string.Concat("{ID: ", userId, "}");
			});
			List<string> extraLines = PaginationTools.BuildLinesFromTerms(sharedUsersSelector.Distinct());
			extraLines[0] = "Shared with: " + extraLines[0];
			lines.AddRange(extraLines);
		}
		else
		{
			lines.Add("Region is not shared with any users.");
		}

		if (region.AllowedGroups.Count > 0)
		{
			List<string> extraLines = PaginationTools.BuildLinesFromTerms(region.AllowedGroups.Distinct());
			extraLines[0] = "Shared with groups: " + extraLines[0];
			lines.AddRange(extraLines);
		}
		else
		{
			lines.Add("Region is not shared with any groups.");
		}

		PaginationTools.SendPage(
			Sender, flags.PageNumber, lines, new PaginationTools.Settings
			{
				HeaderFormat = string.Format("Information About Region \"{0}\" ({{0}}/{{1}}):", region.Name),
				FooterFormat = string.Format("Type {0}region info {1} -p {{0}} for more information.", TextOptions.CommandPrefix, regionName)
			}
		);

		if (flags.DisplayBoundaries)
		{
			Rectangle regionArea = region.Area;
			foreach (Point boundaryPoint in Utils.Instance.EnumerateRegionBoundaries(regionArea))
			{
				// Preferring dotted lines as those should easily be distinguishable from actual wires.
				if ((boundaryPoint.X + boundaryPoint.Y & 1) == 0)
				{
					// Could be improved by sending raw tile data to the client instead but not really
					// worth the effort as chances are very low that overwriting the wire for a few
					// nanoseconds will cause much trouble.
					ITile tile = Main.tile[boundaryPoint.X, boundaryPoint.Y];
					bool oldWireState = tile.wire();
					tile.wire(true);

					try
					{
						Sender.SendTileSquareCentered(boundaryPoint.X, boundaryPoint.Y, 1);
					}
					finally
					{
						tile.wire(oldWireState);
					}
				}
			}

			Timer boundaryHideTimer = null;
			boundaryHideTimer = new Timer((state) =>
			{
				foreach (Point boundaryPoint in Utils.Instance.EnumerateRegionBoundaries(regionArea))
					if ((boundaryPoint.X + boundaryPoint.Y & 1) == 0)
						Sender.SendTileSquareCentered(boundaryPoint.X, boundaryPoint.Y, 1);

				Debug.Assert(boundaryHideTimer != null);
				boundaryHideTimer.Dispose();
			},
				null, 5000, Timeout.Infinite
			);
		}
	}

	[SubCommand("z")]
	public void SetZ(string? regionName, int z)
	{
		if (!String.IsNullOrWhiteSpace(regionName))
		{
			//string regionName = args.Parameters[1];
			//int z = 0;
			//if (int.TryParse(args.Parameters[2], out z))
			{
				if (TShock.Regions.SetZ(regionName, z))
					Sender.SendInfoMessage("Region's z is now " + z);
				else
					Sender.SendErrorMessage("Could not find specified region");
			}
			//else
			//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region z <name> <#>", TextOptions.CommandPrefix);
		}
		else
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region z <name> <#>", TextOptions.CommandPrefix);
	}

	[SubCommand("expand", "resize")]
	public void Expand(string regionName, ERegionResizeDirection direction, int amount)
	{
		//if (args.Parameters.Count == 4)
		{
			//int direction;
			//switch (args.Parameters[2])
			//{
			//	case "u":
			//	case "up":
			//		{
			//			direction = 0;
			//			break;
			//		}
			//	case "r":
			//	case "right":
			//		{
			//			direction = 1;
			//			break;
			//		}
			//	case "d":
			//	case "down":
			//		{
			//			direction = 2;
			//			break;
			//		}
			//	case "l":
			//	case "left":
			//		{
			//			direction = 3;
			//			break;
			//		}
			//	default:
			//		{
			//			direction = -1;
			//			break;
			//		}
			//}
			//int addAmount;
			//int.TryParse(args.Parameters[3], out addAmount);
			if (TShock.Regions.ResizeRegion(regionName, amount, direction))
			{
				Sender.SendInfoMessage("Region Resized Successfully!");
				TShock.Regions.Reload();
			}
			else
				Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region resize <region> <u/d/l/r> <amount>", TextOptions.CommandPrefix);
		}
		//else
		//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region resize <region> <u/d/l/r> <amount>", TextOptions.CommandPrefix);
	}

	[SubCommand("rename")]
	public void Rename(string? regionName = null, string? newName = null)
	{
		if (String.IsNullOrWhiteSpace(regionName) || String.IsNullOrWhiteSpace(newName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region rename <region> <new name>", TextOptions.CommandPrefix);
		}
		else
		{
			//string oldName = args.Parameters[1];
			//string newName = args.Parameters[2];

			if (regionName == newName)
			{
				Sender.SendErrorMessage("Error: both names are the same.");
				return;
			}

			Region oldRegion = TShock.Regions.GetRegionByName(regionName);

			if (oldRegion == null)
			{
				Sender.SendErrorMessage("Invalid region \"{0}\".", regionName);
				return;
			}

			Region newRegion = TShock.Regions.GetRegionByName(newName);

			if (newRegion != null)
			{
				Sender.SendErrorMessage("Region \"{0}\" already exists.", newName);
				return;
			}

			if (TShock.Regions.RenameRegion(regionName, newName))
			{
				Sender.SendInfoMessage("Region renamed successfully!");
			}
			else
			{
				Sender.SendErrorMessage("Failed to rename the region.");
			}
		}
	}

	[SubCommand("tp")]
	[CommandPermissions(Permissions.tp)]
	public void Tp([AllowSpaces] string? regionName = null)
	{
		if (String.IsNullOrWhiteSpace(regionName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}region tp <region>.", TextOptions.CommandPrefix);
			return;
		}

		//string regionName = string.Join(" ", args.Parameters.Skip(1));
		Region region = TShock.Regions.GetRegionByName(regionName);
		if (region == null)
		{
			Sender.SendErrorMessage("Region \"{0}\" does not exist.", regionName);
			return;
		}

		Sender.Teleport(region.Area.Center.X * 16, region.Area.Center.Y * 16);
	}
}
