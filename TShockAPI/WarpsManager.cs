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
using System.Linq;
using System.Text;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;
using Microsoft.Xna.Framework;
using System.Xml;
using Terraria;

namespace TShockAPI
{
    public class WarpManager
    {
        private IDbConnection database;

        public static List<Warp> Warps = new List<Warp>();

        public WarpManager(IDbConnection db)
        {
            database = db;

            using (var com = database.CreateCommand())
            {
                com.CommandText =
                    "CREATE TABLE IF NOT EXISTS \"Warps\" (\"X\" VARCHAR(4) NOT NULL  UNIQUE, \"Y\" VARCHAR(4) NOT NULL  UNIQUE , \"WarpName\" VARCHAR(32) NOT NULL , \"WorldName\" VARCHAR(255) NOT NULL );";
                com.ExecuteNonQuery();
            }
        }

        static IDbDataParameter AddParameter(IDbCommand command, string name, object data)
        {
            var parm = command.CreateParameter();
            parm.ParameterName = name;
            parm.Value = data;
            command.Parameters.Add(parm);
            return parm;
        }

        public bool AddWarp(int x, int y, string name, string worldname)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "INSERT INTO Warps (X, Y, WarpName, WorldName) VALUES (@x, @y, @name, @worldname)";
                    AddParameter(com, "@x", x);
                    AddParameter(com, "@y", y);
                    AddParameter(com, "@name", name.ToLower());
                    AddParameter(com, "@worldname", worldname);
                    com.ExecuteNonQuery();
                }
                return true;
            }
            catch (SqliteExecutionException ex)
            {
            }
            return false;
        }

        public bool RemoveWarp(string name)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "DELETE FROM Warps WHERE WarpName=@name AND WorldName=@worldname";
                    AddParameter(com, "@name", name.ToLower());
                    AddParameter(com, "@worldname", Main.worldName);
                    com.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqliteExecutionException ex)
            {
            }
            return false;
        }

        public Warp FindWarp(string name)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Warps WHERE WarpName=@name AND WorldName=@worldname";
                    AddParameter(com, "@name", name.ToLower());
                    AddParameter(com, "@worldname", Main.worldName);
                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                            return new Warp(new Vector2(Int32.Parse((string)reader["X"]),Int32.Parse((string)reader["Y"])), (string)reader["WarpName"], (string)reader["WorldName"]);
                    }
                }
            }
            catch (SqliteExecutionException ex)
            {
            }
            return new Warp();
        }
    }

    public class Warp
    {
        public Vector2 WarpPos { get; set; }
        public string WarpName { get; set; }
        public string WorldWarpName { get; set; }

        public Warp(Vector2 warppos, string name, string worldname)
        {
            WarpPos = warppos;
            WarpName = name;
            WorldWarpName = worldname;
        }

        public Warp()
        {
            WarpPos = Vector2.Zero;
            WarpName = null;
            WorldWarpName = string.Empty;
        }
    }
}
