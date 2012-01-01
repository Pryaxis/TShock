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
using MySql.Data.MySqlClient;
using Terraria;

namespace TShockAPI.DB
{
	public class WarpManager
	{
		private IDbConnection database;

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public WarpManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Warps",
			                         new SqlColumn("WarpName", MySqlDbType.VarChar, 50) {Primary = true},
			                         new SqlColumn("X", MySqlDbType.Int32),
			                         new SqlColumn("Y", MySqlDbType.Int32),
			                         new SqlColumn("WorldID", MySqlDbType.Text),
			                         new SqlColumn("Private", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureExists(table);
		}

		public bool AddWarp(int x, int y, string name, string worldid)
		{
			try
			{
				database.Query("INSERT INTO Warps (X, Y, WarpName, WorldID) VALUES (@0, @1, @2, @3);", x, y, name, worldid);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

		public bool RemoveWarp(string name)
		{
			try
			{
				database.Query("DELETE FROM Warps WHERE WarpName=@0 AND WorldID=@1", name, Main.worldID.ToString());
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return false;
		}

		public Warp FindWarp(string name)
		{
			try
			{
				using (
					var reader = database.QueryReader("SELECT * FROM Warps WHERE WarpName=@0 AND WorldID=@1", name,
					                                  Main.worldID.ToString()))
				{
					if (reader.Read())
					{
						try
						{
							return new Warp(new Vector2(reader.Get<int>("X"), reader.Get<int>("Y")), reader.Get<string>("WarpName"),
							                reader.Get<string>("WorldID"), reader.Get<string>("Private"));
						}
						catch
						{
							return new Warp(new Vector2(reader.Get<int>("X"), reader.Get<int>("Y")), reader.Get<string>("WarpName"),
							                reader.Get<string>("WorldID"), "0");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return new Warp();
		}

		/// <summary>
		/// Gets all the warps names from world
		/// </summary>
		/// <param name="worldid">World name to get warps from</param>
		/// <returns>List of warps with only their names</returns>
		public List<Warp> ListAllPublicWarps(string worldid)
		{
			var warps = new List<Warp>();
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Warps WHERE WorldID=@0", worldid))
				{
					while (reader.Read())
					{
						try
						{
							if (reader.Get<String>("Private") == "0" || reader.Get<String>("Private") == null)
								warps.Add(new Warp {WarpName = reader.Get<string>("WarpName")});
						}
						catch
						{
							warps.Add(new Warp {WarpName = reader.Get<string>("WarpName")});
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return warps;
		}

		/// <summary>
		/// Gets all the warps names from world
		/// </summary>
		/// <param name="worldid">World name to get warps from</param>
		/// <returns>List of warps with only their names</returns>
		public bool HideWarp(string warp, bool state)
		{
			try
			{
				string query = "UPDATE Warps SET Private=@0 WHERE WarpName=@1 AND WorldID=@2";

				database.Query(query, state ? "1" : "0", warp, Main.worldID.ToString());

				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return false;
			}
		}
	}

	public class Warp
	{
		public Vector2 WarpPos { get; set; }
		public string WarpName { get; set; }
		public string WorldWarpID { get; set; }
		public string Private { get; set; }

		public Warp(Vector2 warppos, string name, string worldid, string hidden)
		{
			WarpPos = warppos;
			WarpName = name;
			WorldWarpID = worldid;
			Private = hidden;
		}

		public Warp()
		{
			WarpPos = Vector2.Zero;
			WarpName = null;
			WorldWarpID = string.Empty;
			Private = "0";
		}
	}
}