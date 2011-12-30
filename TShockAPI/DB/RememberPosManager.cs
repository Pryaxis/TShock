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
using System.Data;
using MySql.Data.MySqlClient;
using Terraria;

namespace TShockAPI.DB
{
    public class RemeberedPosManager
    {
        public IDbConnection database;

        public RemeberedPosManager(IDbConnection db)
        {
            database = db;

            var table = new SqlTable("RememberedPos",
                new SqlColumn("Name", MySqlDbType.VarChar, 50) { Primary = true },
                new SqlColumn("IP", MySqlDbType.Text),
                new SqlColumn("X", MySqlDbType.Int32),
                new SqlColumn("Y", MySqlDbType.Int32),
                new SqlColumn("WorldID", MySqlDbType.Text)
            );
            var creator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            creator.EnsureExists(table);
        }

        public Vector2 GetLeavePos(string name, string IP)
        {
            try
            {
                using (var reader = database.QueryReader("SELECT * FROM RememberedPos WHERE Name=@0 AND IP=@1", name, IP))
                {
                    if (reader.Read())
                    {
                        return new Vector2(reader.Get<int>("X"), reader.Get<int>("Y"));
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }

            return new Vector2();
        }

        public void InsertLeavePos(string name, string IP, int X, int Y)
        {
            if (GetLeavePos(name, IP) == Vector2.Zero)
            {
                try
                {
                    database.Query("INSERT INTO RememberedPos (Name, IP, X, Y, WorldID) VALUES (@0, @1, @2, @3, @4);", name, IP, X, Y + 3, Main.worldID.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
            else
            {
                try
                {
                    database.Query("UPDATE RememberedPos SET X = @0, Y = @1 WHERE Name = @2 AND IP = @3 AND WorldID = @4;", X, Y + 3, name, IP, Main.worldID.ToString());
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }
    }
}
