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
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using Terraria;
using Microsoft.Xna.Framework;

namespace TShockAPI.DB
{
	/// <summary>
	/// Represents the Region database manager.
	/// </summary>
	public class RegionManager
	{
		/// <summary>
		/// The list of regions.
		/// </summary>
		public List<Region> Regions = new List<Region>();

		private IDbConnection database;

		internal RegionManager(IDbConnection db)
		{
			database = db;
			var table = new SqlTable("Regions",
									 new SqlColumn("Id", MySqlDbType.Int32) {Primary = true, AutoIncrement = true},
									 new SqlColumn("X1", MySqlDbType.Int32),
									 new SqlColumn("Y1", MySqlDbType.Int32),
									 new SqlColumn("width", MySqlDbType.Int32),
									 new SqlColumn("height", MySqlDbType.Int32),
									 new SqlColumn("RegionName", MySqlDbType.VarChar, 50) {Unique = true},
									 new SqlColumn("WorldID", MySqlDbType.VarChar, 50) { Unique = true },
									 new SqlColumn("UserIds", MySqlDbType.Text),
									 new SqlColumn("Protected", MySqlDbType.Int32),
									 new SqlColumn("Groups", MySqlDbType.Text),
									 new SqlColumn("Owner", MySqlDbType.VarChar, 50),
									 new SqlColumn("Z", MySqlDbType.Int32){ DefaultValue = "0" }
				);
			var creator = new SqlTableCreator(db,
											  db.GetSqlType() == SqlType.Sqlite
											  	? (IQueryBuilder) new SqliteQueryCreator()
											  	: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
		}

		/// <summary>
		/// Reloads all regions.
		/// </summary>
		public void Reload()
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Regions WHERE WorldID=@0", Main.worldID.ToString()))
				{
					Regions.Clear();
					while (reader.Read())
					{
						int id = reader.Get<int>("Id");
						int X1 = reader.Get<int>("X1");
						int Y1 = reader.Get<int>("Y1");
						int height = reader.Get<int>("height");
						int width = reader.Get<int>("width");
						int Protected = reader.Get<int>("Protected");
						string mergedids = reader.Get<string>("UserIds");
						string name = reader.Get<string>("RegionName");
						string owner = reader.Get<string>("Owner");
						string groups = reader.Get<string>("Groups");
						int z = reader.Get<int>("Z");

						string[] splitids = mergedids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

						Region r = new Region(id, new Rectangle(X1, Y1, width, height), name, owner, Protected != 0, Main.worldID.ToString(), z);
						r.SetAllowedGroups(groups);
						try
						{
							for (int i = 0; i < splitids.Length; i++)
							{
								int userid;

								if (Int32.TryParse(splitids[i], out userid)) // if unparsable, it's not an int, so silently skip
									r.AllowedIDs.Add(userid);
								else
									TShock.Log.Warn("One of your UserIDs is not a usable integer: " + splitids[i]);
							}
						}
						catch (Exception e)
						{
							TShock.Log.Error("Your database contains invalid UserIDs (they should be ints).");
							TShock.Log.Error("A lot of things will fail because of this. You must manually delete and re-create the allowed field.");
							TShock.Log.Error(e.ToString());
							TShock.Log.Error(e.StackTrace);
						}

						Regions.Add(r);
					}
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
		}

		/// <summary>
		/// Adds a region to the database.
		/// </summary>
		/// <param name="tx">TileX of the top left corner.</param>
		/// <param name="ty">TileY of the top left corner.</param>
		/// <param name="width">Width of the region in tiles.</param>
		/// <param name="height">Height of the region in tiles.</param>
		/// <param name="regionname">The name of the region.</param>
		/// <param name="owner">The User Account Name of the person who created this region.</param>
		/// <param name="worldid">The world id that this region is in.</param>
		/// <param name="z">The Z index of the region.</param>
		/// <returns>Whether the region was created and added successfully.</returns>
		public bool AddRegion(int tx, int ty, int width, int height, string regionname, string owner, string worldid, int z = 0)
		{
			if (GetRegionByName(regionname) != null)
			{
				return false;
			}
			try
			{
				database.Query(
					"INSERT INTO Regions (X1, Y1, width, height, RegionName, WorldID, UserIds, Protected, Groups, Owner, Z) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10);",
					tx, ty, width, height, regionname, worldid, "", 1, "", owner, z);
				int id;
				using (QueryResult res = database.QueryReader("SELECT Id FROM Regions WHERE RegionName = @0 AND WorldID = @1", regionname, worldid))
				{
					if (res.Read())
					{
						id = res.Get<int>("Id");
					}
					else
					{
						return false;
					}
				}
				Region region = new Region(id, new Rectangle(tx, ty, width, height), regionname, owner, true, worldid, z);
				Regions.Add(region);
				Hooks.RegionHooks.OnRegionCreated(region);
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Delets the region from this world with a given ID.
		/// </summary>
		/// <param name="id">The ID of the region to delete.</param>
		/// <returns>Whether the region was successfully deleted.</returns>
		public bool DeleteRegion(int id)
		{
			try
			{
				database.Query("DELETE FROM Regions WHERE Id=@0 AND WorldID=@1", id, Main.worldID.ToString());
				var worldid = Main.worldID.ToString();
				var region = Regions.FirstOrDefault(r => r.ID == id && r.WorldID == worldid);
				Regions.RemoveAll(r => r.ID == id && r.WorldID == worldid);
				Hooks.RegionHooks.OnRegionDeleted(region);
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Deletes the region from this world with a given name.
		/// </summary>
		/// <param name="name">The name of the region to delete.</param>
		/// <returns>Whether the region was successfully deleted.</returns>
		public bool DeleteRegion(string name)
		{
			try
			{
				database.Query("DELETE FROM Regions WHERE RegionName=@0 AND WorldID=@1", name, Main.worldID.ToString());
				var worldid = Main.worldID.ToString();
				var region = Regions.FirstOrDefault(r => r.Name == name && r.WorldID == worldid);
				Regions.RemoveAll(r => r.Name == name && r.WorldID == worldid);
				Hooks.RegionHooks.OnRegionDeleted(region);
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Sets the protected state of the region with a given ID.
		/// </summary>
		/// <param name="id">The ID of the region to change.</param>
		/// <param name="state">New protected state of the region.</param>
		/// <returns>Whether the region's state was successfully changed.</returns>
		public bool SetRegionState(int id, bool state)
		{
			try
			{
				database.Query("UPDATE Regions SET Protected = @0 WHERE Id = @1 AND WorldID = @2", state ? 1 : 0, id,
							   Main.worldID.ToString());
				var region = GetRegionByID(id);
				if (region != null)
				{
					region.DisableBuild = state;
				}
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}
		}

		/// <summary>
		/// Sets the protected state of the region with a given name.
		/// </summary>
		/// <param name="name">The name of the region to change.</param>
		/// <param name="state">New protected state of the region.</param>
		/// <returns>Whether the region's state was successfully changed.</returns>
		public bool SetRegionState(string name, bool state)
		{
			try
			{
				database.Query("UPDATE Regions SET Protected=@0 WHERE RegionName=@1 AND WorldID=@2", state ? 1 : 0, name,
							   Main.worldID.ToString());
				var region = GetRegionByName(name);
				if (region != null)
					region.DisableBuild = state;
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}
		}

		/// <summary>
		/// Checks if a given player can build in a region at the given (x, y) coordinate
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="ply">Player to check permissions with</param>
		/// <returns>Whether the player can build at the given (x, y) coordinate</returns>
		public bool CanBuild(int x, int y, TSPlayer ply)
		{
			if (!ply.HasPermission(Permissions.canbuild))
			{
				return false;
			}
			Region top = null;

			foreach (Region region in Regions.ToList())
			{
				if (region.InArea(x, y))
				{
					if (top == null || region.Z > top.Z)
						top = region;	
				}
			}
			return top == null || top.HasPermissionToBuildInRegion(ply);
		}

		/// <summary>
		/// Checks if any regions exist at the given (x, y) coordinate
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>Whether any regions exist at the given (x, y) coordinate</returns>
		public bool InArea(int x, int y)
		{
			return Regions.Any(r => r.InArea(x, y));
		}

		/// <summary>
		/// Checks if any regions exist at the given (x, y) coordinate
		/// and returns an IEnumerable containing their names
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>The names of any regions that exist at the given (x, y) coordinate</returns>
		public IEnumerable<string> InAreaRegionName(int x, int y)
		{
			return Regions.Where(r => r.InArea(x, y)).Select(r => r.Name);
		}

		/// <summary>
		/// Checks if any regions exist at the given (x, y) coordinate 
		/// and returns an IEnumerable containing their IDs
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>The IDs of any regions that exist at the given (x, y) coordinate</returns>
		public IEnumerable<int> InAreaRegionID(int x, int y)
		{
			return Regions.Where(r => r.InArea(x, y)).Select(r => r.ID);
		}

		/// <summary>
		/// Checks if any regions exist at the given (x, y) coordinate
		/// and returns an IEnumerable containing their <see cref="Region"/> objects
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>The <see cref="Region"/> objects of any regions that exist at the given (x, y) coordinate</returns>
		public IEnumerable<Region> InAreaRegion(int x, int y)
		{
			return Regions.Where(r => r.InArea(x, y));
		}

		/// <summary>
		/// Changes the size of a given region
		/// </summary>
		/// <param name="regionName">Name of the region to resize</param>
		/// <param name="addAmount">Amount to resize</param>
		/// <param name="direction">Direction to resize in:
		/// 0 = resize height and Y.
		/// 1 = resize width.
		/// 2 = resize height.
		/// 3 = resize width and X.</param>
		/// <returns></returns>
		public bool ResizeRegion(string regionName, int addAmount, int direction)
		{
			//0 = up
			//1 = right
			//2 = down
			//3 = left
			int X = 0;
			int Y = 0;
			int height = 0;
			int width = 0;

			try
			{
				using (var reader = database.QueryReader("SELECT X1, Y1, height, width FROM Regions WHERE RegionName=@0 AND WorldID=@1",
													  regionName, Main.worldID.ToString()))
				{
					if (reader.Read())
					{
						X = reader.Get<int>("X1");
						width = reader.Get<int>("width");
						Y = reader.Get<int>("Y1");
						height = reader.Get<int>("height");
					}
				}
				switch (direction)
				{
					case 0:
						Y -= addAmount;
						height += addAmount;
						break;
					case 1:
						width += addAmount;
						break;
					case 2:
						height += addAmount;
						break;
					case 3:
						X -= addAmount;
						width += addAmount;
						break;
					default:
						return false;
				}
				
				foreach (var region in Regions.Where(r => r.Name == regionName))
					region.Area = new Rectangle(X, Y, width, height);
				int q = database.Query("UPDATE Regions SET X1 = @0, Y1 = @1, width = @2, height = @3 WHERE RegionName = @4 AND WorldID=@5", X, Y, width,
						height, regionName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}
		
		/// <summary>
		/// Renames a region
		/// </summary>
		/// <param name="oldName">Name of the region to rename</param>
		/// <param name="newName">New name of the region</param>
		/// <returns>true if renamed successfully, false otherwise</returns>
		public bool RenameRegion(string oldName, string newName)
		{
			Region region = null;
			string worldID = Main.worldID.ToString();

			bool result = false;

			try
			{
				int q = database.Query("UPDATE Regions SET RegionName = @0 WHERE RegionName = @1 AND WorldID = @2",
																					newName, oldName, worldID);

				if (q > 0)
				{
					region = Regions.First(r => r.Name == oldName && r.WorldID == worldID);
					region.Name = newName;
					Hooks.RegionHooks.OnRegionRenamed(region, oldName, newName);
					result = true;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}

			return result;
		}
		
		/// <summary>
		/// Removes an allowed user from a region
		/// </summary>
		/// <param name="regionName">Name of the region to modify</param>
		/// <param name="userName">Username to remove</param>
		/// <returns>true if removed successfully</returns>
		public bool RemoveUser(string regionName, string userName)
		{
			Region r = GetRegionByName(regionName);
			if (r != null)
			{
				if (!r.RemoveID(TShock.UserAccounts.GetUserAccountID(userName)))
				{
					return false;
				}

				string ids = string.Join(",", r.AllowedIDs);
				return database.Query("UPDATE Regions SET UserIds=@0 WHERE RegionName=@1 AND WorldID=@2", ids,
									   regionName, Main.worldID.ToString()) > 0;
			}

			return false;
		}

		/// <summary>
		/// Adds a user to a region's allowed user list
		/// </summary>
		/// <param name="regionName">Name of the region to modify</param>
		/// <param name="userName">Username to add</param>
		/// <returns>true if added successfully</returns>
		public bool AddNewUser(string regionName, string userName)
		{
			try
			{
				string mergedIDs = string.Empty;
				using (
					var reader = database.QueryReader("SELECT UserIds FROM Regions WHERE RegionName=@0 AND WorldID=@1", regionName,
													  Main.worldID.ToString()))
				{
					if (reader.Read())
						mergedIDs = reader.Get<string>("UserIds");
				}

				string userIdToAdd = Convert.ToString(TShock.UserAccounts.GetUserAccountID(userName));
				string[] ids = mergedIDs.Split(',');
				// Is the user already allowed to the region?
				if (ids.Contains(userIdToAdd))
					return true;

				if (string.IsNullOrEmpty(mergedIDs))
					mergedIDs = userIdToAdd;
				else
					mergedIDs = string.Concat(mergedIDs, ",", userIdToAdd);

				int q = database.Query("UPDATE Regions SET UserIds=@0 WHERE RegionName=@1 AND WorldID=@2", mergedIDs,
									   regionName, Main.worldID.ToString());
				foreach (var r in Regions)
				{
					if (r.Name == regionName && r.WorldID == Main.worldID.ToString())
						r.SetAllowedIDs(mergedIDs);
				}
				return q != 0;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Sets the position of a region.
		/// </summary>
		/// <param name="regionName">The region name.</param>
		/// <param name="x">The X position.</param>
		/// <param name="y">The Y position.</param>
		/// <param name="height">The height.</param>
		/// <param name="width">The width.</param>
		/// <returns>Whether the operation succeeded.</returns>
		public bool PositionRegion(string regionName, int x, int y, int width, int height)
		{
			try
			{
				Region region = Regions.First(r => String.Equals(regionName, r.Name, StringComparison.OrdinalIgnoreCase));
				region.Area = new Rectangle(x, y, width, height);

				if (database.Query("UPDATE Regions SET X1 = @0, Y1 = @1, width = @2, height = @3 WHERE RegionName = @4 AND WorldID = @5",
					x, y, width, height, regionName, Main.worldID.ToString()) > 0)
					return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Gets all the regions names from world
		/// </summary>
		/// <param name="worldid">World name to get regions from</param>
		/// <returns>List of regions with only their names</returns>
		public List<Region> ListAllRegions(string worldid)
		{
			var regions = new List<Region>();
			try
			{
				using (var reader = database.QueryReader("SELECT RegionName FROM Regions WHERE WorldID=@0", worldid))
				{
					while (reader.Read())
						regions.Add(new Region {Name = reader.Get<string>("RegionName")});
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return regions;
		}

		/// <summary>
		/// Returns a region with the given name
		/// </summary>
		/// <param name="name">Region name</param>
		/// <returns>The region with the given name, or null if not found</returns>
		public Region GetRegionByName(String name)
		{
			return Regions.FirstOrDefault(r => r.Name.Equals(name) && r.WorldID == Main.worldID.ToString());
		}

		/// <summary>
		/// Returns a region with the given ID
		/// </summary>
		/// <param name="id">Region ID</param>
		/// <returns>The region with the given ID, or null if not found</returns>
		public Region GetRegionByID(int id)
		{
			return Regions.FirstOrDefault(r => r.ID == id && r.WorldID == Main.worldID.ToString());
		}

		/// <summary>
		/// Changes the owner of the region with the given name
		/// </summary>
		/// <param name="regionName">Region name</param>
		/// <param name="newOwner">New owner's username</param>
		/// <returns>Whether the change was successfull</returns>
		public bool ChangeOwner(string regionName, string newOwner)
		{
			var region = GetRegionByName(regionName);
			if (region != null)
			{
				region.Owner = newOwner;
				int q = database.Query("UPDATE Regions SET Owner=@0 WHERE RegionName=@1 AND WorldID=@2", newOwner,
									   regionName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Allows a group to use a region
		/// </summary>
		/// <param name="regionName">Region name</param>
		/// <param name="groupName">Group's name</param>
		/// <returns>Whether the change was successfull</returns>
		public bool AllowGroup(string regionName, string groupName)
		{
			string mergedGroups = "";
			using (
				var reader = database.QueryReader("SELECT Groups FROM Regions WHERE RegionName=@0 AND WorldID=@1", regionName,
												  Main.worldID.ToString()))
			{
				if (reader.Read())
					mergedGroups = reader.Get<string>("Groups");
			}

			string[] groups = mergedGroups.Split(',');
			// Is the group already allowed to the region?
			if (groups.Contains(groupName))
				return true;

			if (mergedGroups != "")
				mergedGroups += ",";
			mergedGroups += groupName;

			int q = database.Query("UPDATE Regions SET Groups=@0 WHERE RegionName=@1 AND WorldID=@2", mergedGroups,
								   regionName, Main.worldID.ToString());

			Region r = GetRegionByName(regionName);
			if (r != null)
			{
				r.SetAllowedGroups(mergedGroups);
			}
			else
			{
				return false;
			}

			return q > 0;
		}

		/// <summary>
		/// Removes a group's access to a region
		/// </summary>
		/// <param name="regionName">Region name</param>
		/// <param name="group">Group name</param>
		/// <returns>Whether the change was successfull</returns>
		public bool RemoveGroup(string regionName, string group)
		{
			Region r = GetRegionByName(regionName);
			if (r != null)
			{
				r.RemoveGroup(group);
				string groups = string.Join(",", r.AllowedGroups);
				int q = database.Query("UPDATE Regions SET Groups=@0 WHERE RegionName=@1 AND WorldID=@2", groups,
									   regionName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the <see cref="Region"/> with the highest Z index of the given list
		/// </summary>
		/// <param name="regions">List of Regions to compare</param>
		/// <returns></returns>
		public Region GetTopRegion(IEnumerable<Region> regions)
		{
			Region ret = null;
			foreach (Region r in regions)
			{
				if (ret == null)
					ret = r;
				else
				{
					if (r.Z > ret.Z)
						ret = r;
				}
			}
			return ret;
		}

		/// <summary>
		/// Sets the Z index of a given region
		/// </summary>
		/// <param name="name">Region name</param>
		/// <param name="z">New Z index</param>
		/// <returns>Whether the change was successfull</returns>
		public bool SetZ(string name, int z)
		{
			try
			{
				database.Query("UPDATE Regions SET Z=@0 WHERE RegionName=@1 AND WorldID=@2", z, name,
							   Main.worldID.ToString());

				var region = GetRegionByName(name);
				if (region != null)
					region.Z = z;
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}
		}
	}

	public class Region
	{
		public int ID { get; set; }
		public Rectangle Area { get; set; }
		public string Name { get; set; }
		public string Owner { get; set; }
		public bool DisableBuild { get; set; }
		public string WorldID { get; set; }
		public List<int> AllowedIDs { get; set; }
		public List<string> AllowedGroups { get; set; }
		public int Z { get; set; }

		public Region(int id, Rectangle region, string name, string owner, bool disablebuild, string RegionWorldIDz, int z)
			: this()
		{
			ID = id;
			Area = region;
			Name = name;
			Owner = owner;
			DisableBuild = disablebuild;
			WorldID = RegionWorldIDz;
			Z = z;
		}

		public Region()
		{
			Area = Rectangle.Empty;
			Name = string.Empty;
			DisableBuild = true;
			WorldID = string.Empty;
			AllowedIDs = new List<int>();
			AllowedGroups = new List<string>();
			Z = 0;
		}

		/// <summary>
		/// Checks if a given point is in the region's area
		/// </summary>
		/// <param name="point">Point to check</param>
		/// <returns>Whether the point exists in the region's area</returns>
		public bool InArea(Rectangle point)
		{
			return InArea(point.X, point.Y);
		}

		/// <summary>
		/// Checks if a given (x, y) coordinate is in the region's area
		/// </summary>
		/// <param name="x">X coordinate to check</param>
		/// <param name="y">Y coordinate to check</param>
		/// <returns>Whether the coordinate exists in the region's area</returns>
		public bool InArea(int x, int y) //overloaded with x,y
		{
			/*
			DO NOT CHANGE TO Area.Contains(x, y)!
			Area.Contains does not account for the right and bottom 'border' of the rectangle,
			which results in regions being trimmed.
			*/
			return x >= Area.X && x <= Area.X + Area.Width && y >= Area.Y && y <= Area.Y + Area.Height;
		}
		
		/// <summary>
		/// Checks if a given player has permission to build in the region
		/// </summary>
		/// <param name="ply">Player to check permissions with</param>
		/// <returns>Whether the player has permission</returns>
		public bool HasPermissionToBuildInRegion(TSPlayer ply)
		{
			if (!DisableBuild)
			{
				return true;
			}
			if (!ply.IsLoggedIn)
			{
				if (!ply.HasBeenNaggedAboutLoggingIn)
				{
					ply.SendMessage("You must be logged in to take advantage of protected regions.", Color.Red);
					ply.HasBeenNaggedAboutLoggingIn = true;
				}
				return false;
			}

			return ply.HasPermission(Permissions.editregion) || AllowedIDs.Contains(ply.Account.ID) || AllowedGroups.Contains(ply.Group.Name) || Owner == ply.Account.Name;
		}

		/// <summary>
		/// Sets the user IDs which are allowed to use the region
		/// </summary>
		/// <param name="ids">String of IDs to set</param>
		public void SetAllowedIDs(String ids)
		{
			String[] idArr = ids.Split(',');
			List<int> idList = new List<int>();

			foreach (String id in idArr)
			{
				int i = 0;
				if (int.TryParse(id, out i) && i != 0)
				{
					idList.Add(i);
				}
			}
			AllowedIDs = idList;
		}

		/// <summary>
		/// Sets the group names which are allowed to use the region
		/// </summary>
		/// <param name="groups">String of group names to set</param>
		public void SetAllowedGroups(String groups)
		{
			// prevent null pointer exceptions
			if (!string.IsNullOrEmpty(groups))
			{
				List<String> groupList = groups.Split(',').ToList();

				for (int i = 0; i < groupList.Count; i++)
				{
					groupList[i] = groupList[i].Trim();
				}

				AllowedGroups = groupList;
			}
		}

		/// <summary>
		/// Removes a user's access to the region
		/// </summary>
		/// <param name="id">User ID to remove</param>
		/// <returns>true if the user was found and removed from the region's allowed users</returns>
		public bool RemoveID(int id)
		{
			return AllowedIDs.Remove(id);
		}

		/// <summary>
		/// Removes a group's access to the region
		/// </summary>
		/// <param name="groupName">Group name to remove</param>
		/// <returns></returns>
		public bool RemoveGroup(string groupName)
		{
			return AllowedGroups.Remove(groupName);
		}
	}
}
