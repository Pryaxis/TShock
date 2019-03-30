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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MySql.Data.MySqlClient;
using Terraria;
using Microsoft.Xna.Framework;

namespace TShockAPI.DB
{
	public class WarpManager
	{
		private IDbConnection database;
		/// <summary>
		/// The list of warps.
		/// </summary>
		public List<Warp> Warps = new List<Warp>();

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		internal WarpManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Warps",
			                         new SqlColumn("Id", MySqlDbType.Int32){Primary = true, AutoIncrement = true},
									 new SqlColumn("WarpName", MySqlDbType.VarChar, 50) {Unique = true},
			                         new SqlColumn("X", MySqlDbType.Int32),
			                         new SqlColumn("Y", MySqlDbType.Int32),
									 new SqlColumn("WorldID", MySqlDbType.VarChar, 50) { Unique = true },
			                         new SqlColumn("Private", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
		}

		/// <summary>
		/// Adds a warp.
		/// </summary>
		/// <param name="x">The X position.</param>
		/// <param name="y">The Y position.</param>
		/// <param name="name">The name.</param>
		/// <returns>Whether the operation succeeded.</returns>
		public bool Add(int x, int y, string name)
		{
			try
			{
				if (database.Query("INSERT INTO Warps (X, Y, WarpName, WorldID) VALUES (@0, @1, @2, @3);",
					x, y, name, Main.worldID.ToString()) > 0)
				{
					Warps.Add(new Warp(new Point(x, y), name));
					return true;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Reloads all warps.
		/// </summary>
		public void ReloadWarps()
		{
			Warps.Clear();

			using (var reader = database.QueryReader("SELECT * FROM Warps WHERE WorldID = @0",
				Main.worldID.ToString()))
			{
				while (reader.Read())
				{
					Warps.Add(new Warp(
						new Point(reader.Get<int>("X"), reader.Get<int>("Y")),
						reader.Get<string>("WarpName"),
						(reader.Get<string>("Private") ?? "0") != "0"));
				}
			}
		}

		/// <summary>
		/// Removes a warp.
		/// </summary>
		/// <param name="warpName">The warp name.</param>
		/// <returns>Whether the operation succeeded.</returns>
		public bool Remove(string warpName)
		{
			try
			{
				if (database.Query("DELETE FROM Warps WHERE WarpName = @0 AND WorldID = @1",
					warpName, Main.worldID.ToString()) > 0)
				{
					Warps.RemoveAll(w => String.Equals(w.Name, warpName, StringComparison.OrdinalIgnoreCase));
					return true;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Finds the warp with the given name.
		/// </summary>
		/// <param name="warpName">The name.</param>
		/// <returns>The warp, if it exists, or else null.</returns>
		public Warp Find(string warpName)
		{
			return Warps.FirstOrDefault(w => String.Equals(w.Name, warpName, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Sets the position of a warp.
		/// </summary>
		/// <param name="warpName">The warp name.</param>
		/// <param name="x">The X position.</param>
		/// <param name="y">The Y position.</param>
		/// <returns>Whether the operation suceeded.</returns>
		public bool Position(string warpName, int x, int y)
		{
			try
			{
				if (database.Query("UPDATE Warps SET X = @0, Y = @1 WHERE WarpName = @2 AND WorldID = @3",
					x, y, warpName, Main.worldID.ToString()) > 0)
				{
					Warps.Find(w => String.Equals(w.Name, warpName, StringComparison.OrdinalIgnoreCase)).Position = new Point(x, y);
					return true;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Sets the hidden state of a warp.
		/// </summary>
		/// <param name="warpName">The warp name.</param>
		/// <param name="state">The state.</param>
		/// <returns>Whether the operation suceeded.</returns>
		public bool Hide(string warpName, bool state)
		{
			try
			{
				if (database.Query("UPDATE Warps SET Private = @0 WHERE WarpName = @1 AND WorldID = @2",
					state ? "1" : "0", warpName, Main.worldID.ToString()) > 0)
				{
					Warps.Find(w => String.Equals(w.Name, warpName, StringComparison.OrdinalIgnoreCase)).IsPrivate = state;
					return true;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}
	}

	/// <summary>
	/// Represents a warp.
	/// </summary>
	public class Warp
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the warp's privacy state.
		/// </summary>
		public bool IsPrivate { get; set; }
		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		public Point Position { get; set; }

		public Warp(Point position, string name, bool isPrivate = false)
		{
			Name = name;
			Position = position;
			IsPrivate = isPrivate;
		}

		/// <summary>Creates a warp with a default coordinate of zero, an empty name, public.</summary>
		public Warp()
		{
			Position = Point.Zero;
			Name = "";
			IsPrivate = false;
		}
	}
}