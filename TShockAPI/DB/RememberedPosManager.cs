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
using System.Data;
using MySql.Data.MySqlClient;
using Terraria;
using Microsoft.Xna.Framework;

namespace TShockAPI.DB
{
	public class RememberedPosManager
	{
		public IDbConnection database;

		public RememberedPosManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("RememberedPos",
			                         new SqlColumn("Name", MySqlDbType.VarChar, 50) {Primary = true},
			                         new SqlColumn("IP", MySqlDbType.Text),
			                         new SqlColumn("X", MySqlDbType.Int32),
			                         new SqlColumn("Y", MySqlDbType.Int32),
			                         new SqlColumn("WorldID", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
		}

		public Vector2 CheckLeavePos(string name)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM RememberedPos WHERE Name=@0", name))
				{
					if (reader.Read())
					{
						int checkX=reader.Get<int>("X");
						int checkY=reader.Get<int>("Y");
						//fix leftover inconsistancies
						if (checkX==0)
						   checkX++;
						if (checkY==0)
						   checkY++;
						return new Vector2(checkX, checkY);
					}
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}

			return new Vector2();
		}



		public Vector2 GetLeavePos(string name, string IP)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM RememberedPos WHERE Name=@0 AND IP=@1 AND WorldID=@2", name, IP, Main.worldID.ToString()))
				{
					if (reader.Read())
					{
						return new Vector2(reader.Get<int>("X"), reader.Get<int>("Y"));
					}
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}

			return new Vector2();
		}

		public void InsertLeavePos(string name, string IP, int X, int Y)
		{
			if (CheckLeavePos(name) == Vector2.Zero)
			{
				try
				{
					if ((X != 0) && ( Y !=0)) //invalid pos!
					database.Query("INSERT INTO RememberedPos (Name, IP, X, Y, WorldID) VALUES (@0, @1, @2, @3, @4);", name, IP, X, Y , Main.worldID.ToString());
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}
			else
			{
				try
				{
					if ((X != 0) && ( Y !=0)) //invalid pos!
					database.Query("UPDATE RememberedPos SET X = @0, Y = @1, IP = @2, WorldID = @3 WHERE Name = @4;", X, Y, IP, Main.worldID.ToString(), name);
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}
		}
	}
}
