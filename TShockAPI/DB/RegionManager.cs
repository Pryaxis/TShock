/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Xml;
using MySql.Data.MySqlClient;
using Terraria;

namespace TShockAPI.DB
{
	public class RegionManager
	{
		public List<Region> Regions = new List<Region>();

		private IDbConnection database;

		public RegionManager(IDbConnection db)
		{
			database = db;
			var table = new SqlTable("Regions",
			                         new SqlColumn("X1", MySqlDbType.Int32),
			                         new SqlColumn("Y1", MySqlDbType.Int32),
			                         new SqlColumn("width", MySqlDbType.Int32),
			                         new SqlColumn("height", MySqlDbType.Int32),
			                         new SqlColumn("RegionName", MySqlDbType.VarChar, 50) {Primary = true},
			                         new SqlColumn("WorldID", MySqlDbType.Text),
			                         new SqlColumn("UserIds", MySqlDbType.Text),
			                         new SqlColumn("Protected", MySqlDbType.Int32),
			                         new SqlColumn("Groups", MySqlDbType.Text),
			                         new SqlColumn("Owner", MySqlDbType.VarChar, 50)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureExists(table);

			ReloadAllRegions();
		}

		public void ReloadAllRegions()
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Regions WHERE WorldID=@0", Main.worldID.ToString()))
				{
					Regions.Clear();
					while (reader.Read())
					{
						int X1 = reader.Get<int>("X1");
						int Y1 = reader.Get<int>("Y1");
						int height = reader.Get<int>("height");
						int width = reader.Get<int>("width");
						int Protected = reader.Get<int>("Protected");
						string mergedids = reader.Get<string>("UserIds");
						string name = reader.Get<string>("RegionName");
						string owner = reader.Get<string>("Owner");
						string groups = reader.Get<string>("Groups");

						string[] splitids = mergedids.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

						Region r = new Region(new Rectangle(X1, Y1, width, height), name, owner, Protected != 0, Main.worldID.ToString());
						r.SetAllowedGroups(groups);
						try
						{
							for (int i = 0; i < splitids.Length; i++)
							{
								int id;

								if (Int32.TryParse(splitids[i], out id)) // if unparsable, it's not an int, so silently skip
									r.AllowedIDs.Add(id);
								else
									Log.Warn("One of your UserIDs is not a usable integer: " + splitids[i]);
							}
						}
						catch (Exception e)
						{
							Log.Error("Your database contains invalid UserIDs (they should be ints).");
							Log.Error("A lot of things will fail because of this. You must manually delete and re-create the allowed field.");
							Log.Error(e.ToString());
							Log.Error(e.StackTrace);
						}

						Regions.Add(r);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
		}

		public void ReloadForUnitTest(String n)
		{
			using (var reader = database.QueryReader("SELECT * FROM Regions WHERE WorldID=@0", n))
			{
				Regions.Clear();
				while (reader.Read())
				{
					int X1 = reader.Get<int>("X1");
					int Y1 = reader.Get<int>("Y1");
					int height = reader.Get<int>("height");
					int width = reader.Get<int>("width");
					int Protected = reader.Get<int>("Protected");
					string MergedIDs = reader.Get<string>("UserIds");
					string name = reader.Get<string>("RegionName");
					string[] SplitIDs = MergedIDs.Split(',');
					string owner = reader.Get<string>("Owner");
					string groups = reader.Get<string>("Groups");

					Region r = new Region(new Rectangle(X1, Y1, width, height), name, owner, Protected != 0, Main.worldID.ToString());
					r.SetAllowedGroups(groups);
					try
					{
						for (int i = 0; i < SplitIDs.Length; i++)
						{
							int id;

							if (Int32.TryParse(SplitIDs[i], out id)) // if unparsable, it's not an int, so silently skip
								r.AllowedIDs.Add(id);
							else if (SplitIDs[i] == "") // Split gotcha, can return an empty string with certain conditions
								// but we only want to let the user know if it's really a nonparsable integer.
								Log.Warn("UnitTest: One of your UserIDs is not a usable integer: " + SplitIDs[i]);
						}
					}
					catch (Exception e)
					{
						Log.Error("Your database contains invalid UserIDs (they should be ints).");
						Log.Error("A lot of things will fail because of this. You must manually delete and re-create the allowed field.");
						Log.Error(e.Message);
						Log.Error(e.StackTrace);
					}

					Regions.Add(r);
				}
			}
		}

		public bool AddRegion(int tx, int ty, int width, int height, string regionname, string owner, string worldid)
		{
			if (GetRegionByName(regionname) != null)
			{
				return false;
			}
			try
			{
				database.Query(
					"INSERT INTO Regions (X1, Y1, width, height, RegionName, WorldID, UserIds, Protected, Groups, Owner) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9);",
					tx, ty, width, height, regionname, worldid, "", 1, "", owner);
				Regions.Add(new Region(new Rectangle(tx, ty, width, height), regionname, owner, true, worldid));
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

		public bool DeleteRegion(string name)
		{
			try
			{
				database.Query("DELETE FROM Regions WHERE RegionName=@0 AND WorldID=@1", name, Main.worldID.ToString());
				var worldid = Main.worldID.ToString();
				Regions.RemoveAll(r => r.Name == name && r.WorldID == worldid);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

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
				Log.Error(ex.ToString());
				return false;
			}
		}

		public bool SetRegionStateTest(string name, string world, bool state)
		{
			try
			{
				database.Query("UPDATE Regions SET Protected=@0 WHERE RegionName=@1 AND WorldID=@2", state ? 1 : 0, name, world);
				var region = GetRegionByName(name);
				if (region != null)
					region.DisableBuild = state;
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
		}

		public bool CanBuild(int x, int y, TSPlayer ply)
		{
			if (!ply.Group.HasPermission(Permissions.canbuild))
			{
				return false;
			}
			for (int i = 0; i < Regions.Count; i++)
			{
				if (Regions[i].InArea(new Rectangle(x, y, 0, 0)) && !Regions[i].HasPermissionToBuildInRegion(ply))
				{
					return false;
				}
			}
			return true;
		}

		public bool InArea(int x, int y)
		{
			foreach (Region region in Regions)
			{
				if (x >= region.Area.Left && x <= region.Area.Right &&
				    y >= region.Area.Top && y <= region.Area.Bottom &&
				    region.DisableBuild)
				{
					return true;
				}
			}
			return false;
		}

		public string InAreaRegionName(int x, int y)
		{
			foreach (Region region in Regions)
			{
				if (x >= region.Area.Left && x <= region.Area.Right &&
				    y >= region.Area.Top && y <= region.Area.Bottom &&
				    region.DisableBuild)
				{
					return region.Name;
				}
			}
			return null;
		}

		public static List<string> ListIDs(string MergedIDs)
		{
			return MergedIDs.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).ToList();
		}

		public bool resizeRegion(string regionName, int addAmount, int direction)
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
				using (
					var reader = database.QueryReader("SELECT X1, Y1, height, width FROM Regions WHERE RegionName=@0 AND WorldID=@1",
					                                  regionName, Main.worldID.ToString()))
				{
					if (reader.Read())
						X = reader.Get<int>("X1");
					width = reader.Get<int>("width");
					Y = reader.Get<int>("Y1");
					height = reader.Get<int>("height");
				}
				if (!(direction == 0))
				{
					if (!(direction == 1))
					{
						if (!(direction == 2))
						{
							if (!(direction == 3))
							{
								return false;
							}
							else
							{
								X -= addAmount;
								width += addAmount;
							}
						}
						else
						{
							height += addAmount;
						}
					}
					else
					{
						width += addAmount;
					}
				}
				else
				{
					Y -= addAmount;
					height += addAmount;
				}
				int q =
					database.Query(
						"UPDATE Regions SET X1 = @0, Y1 = @1, width = @2, height = @3 WHERE RegionName = @4 AND WorldID=@5", X, Y, width,
						height, regionName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

		public bool RemoveUser(string regionName, string userName)
		{
			Region r = GetRegionByName(regionName);
			if (r != null)
			{
				r.RemoveID(TShock.Users.GetUserID(userName));
				string ids = string.Join(",", r.AllowedIDs);
				int q = database.Query("UPDATE Regions SET UserIds=@0 WHERE RegionName=@1 AND WorldID=@2", ids,
				                       regionName, Main.worldID.ToString());
				if (q > 0)
					return true;
			}
			return false;
		}

		public bool AddNewUser(string regionName, String userName)
		{
			try
			{
				string MergedIDs = string.Empty;
				using (
					var reader = database.QueryReader("SELECT * FROM Regions WHERE RegionName=@0 AND WorldID=@1", regionName,
					                                  Main.worldID.ToString()))
				{
					if (reader.Read())
						MergedIDs = reader.Get<string>("UserIds");
				}

				if (string.IsNullOrEmpty(MergedIDs))
					MergedIDs = Convert.ToString(TShock.Users.GetUserID(userName));
				else
					MergedIDs = MergedIDs + "," + Convert.ToString(TShock.Users.GetUserID(userName));

				int q = database.Query("UPDATE Regions SET UserIds=@0 WHERE RegionName=@1 AND WorldID=@2", MergedIDs,
				                       regionName, Main.worldID.ToString());
				foreach (var r in Regions)
				{
					if (r.Name == regionName && r.WorldID == Main.worldID.ToString())
						r.setAllowedIDs(MergedIDs);
				}
				return q != 0;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
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
				Log.Error(ex.ToString());
			}
			return regions;
		}

		public Region GetRegionByName(String name)
		{
			return Regions.FirstOrDefault(r => r.Name.Equals(name) && r.WorldID == Main.worldID.ToString());
		}

		public Region ZacksGetRegionByName(String name)
		{
			foreach (Region r in Regions)
			{
				if (r.Name.Equals(name))
					return r;
			}
			return null;
		}

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

		public bool AllowGroup(string regionName, string groups)
		{
			string groupsNew = "";
			using (
				var reader = database.QueryReader("SELECT * FROM Regions WHERE RegionName=@0 AND WorldID=@1", regionName,
				                                  Main.worldID.ToString()))
			{
				if (reader.Read())
					groupsNew = reader.Get<string>("Groups");
			}
			if (groupsNew != "")
				groupsNew += ",";
			groupsNew += groups;

			int q = database.Query("UPDATE Regions SET Groups=@0 WHERE RegionName=@1 AND WorldID=@2", groupsNew,
			                       regionName, Main.worldID.ToString());

			Region r = GetRegionByName(regionName);
			if (r != null)
			{
				r.SetAllowedGroups(groupsNew);
			}
			else
			{
				return false;
			}

			return q > 0;
		}

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
	}

	public class Region
	{
		public Rectangle Area { get; set; }
		public string Name { get; set; }
		public string Owner { get; set; }
		public bool DisableBuild { get; set; }
		public string WorldID { get; set; }
		public List<int> AllowedIDs { get; set; }
		public List<string> AllowedGroups { get; set; }

		public Region(Rectangle region, string name, string owner, bool disablebuild, string RegionWorldIDz)
			: this()
		{
			Area = region;
			Name = name;
			Owner = owner;
			DisableBuild = disablebuild;
			WorldID = RegionWorldIDz;
		}

		public Region()
		{
			Area = Rectangle.Empty;
			Name = string.Empty;
			DisableBuild = true;
			WorldID = string.Empty;
			AllowedIDs = new List<int>();
			AllowedGroups = new List<string>();
		}

		public bool InArea(Rectangle point)
		{
			if (Area.Contains(point.X, point.Y))
			{
				return true;
			}
			return false;
		}

		public bool HasPermissionToBuildInRegion(TSPlayer ply)
		{
			if (!ply.IsLoggedIn)
			{
				if (!ply.HasBeenNaggedAboutLoggingIn)
				{
					ply.SendMessage("You must be logged in to take advantage of protected regions.", Color.Red);
					ply.HasBeenNaggedAboutLoggingIn = true;
				}
				return false;
			}
			if (!DisableBuild)
			{
				return true;
			}

			return AllowedIDs.Contains(ply.UserID) || AllowedGroups.Contains(ply.Group.Name) || Owner == ply.UserAccountName ||
			       ply.Group.HasPermission("manageregion");
		}

		public void setAllowedIDs(String ids)
		{
			String[] id_arr = ids.Split(',');
			List<int> id_list = new List<int>();
			foreach (String id in id_arr)
			{
				int i = 0;
				int.TryParse(id, out i);
				if (i != 0)
					id_list.Add(i);
			}
			AllowedIDs = id_list;
		}

		public void SetAllowedGroups(String groups)
		{
			// prevent null pointer exceptions
			if (!string.IsNullOrEmpty(groups))
			{
				List<String> groupArr = groups.Split(',').ToList();

				for (int i = 0; i < groupArr.Count; i++)
					groupArr[i] = groupArr[i].Trim();

				AllowedGroups = groupArr;
			}
		}

		public void RemoveID(int id)
		{
			var index = -1;
			for (int i = 0; i < AllowedIDs.Count; i++)
			{
				if (AllowedIDs[i] == id)
				{
					index = i;
					break;
				}
			}
			AllowedIDs.RemoveAt(index);
		}

		public bool RemoveGroup(string groupName)
		{
			return AllowedGroups.Remove(groupName);
		}
	}
}