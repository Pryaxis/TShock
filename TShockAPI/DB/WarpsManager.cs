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
using System.IO;
using Community.CsharpSqlite.SQLiteClient;
using Microsoft.Xna.Framework;
using System.Xml;
using Terraria;
using TShockAPI.DB;

namespace TShockAPI.DB
{
    public class WarpManager
    {
        private IDbConnection database;

        public WarpManager(IDbConnection db)
        {
            database = db;

            using (var com = database.CreateCommand())
            {
                com.CommandText =
                    "CREATE TABLE IF NOT EXISTS 'Warps' ('X' NUMERIC, 'Y' NUMERIC, 'WarpName' TEXT UNIQUE, 'WorldID' TEXT);";
                com.ExecuteNonQuery();
            }
        }

        public bool AddWarp(int x, int y, string name, string worldid)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "INSERT INTO Warps (X, Y, WarpName, WorldID) VALUES (@x, @y, @name, @worldid);";
                    com.AddParameter("@x", x);
                    com.AddParameter("@y", y);
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldid", worldid);
                    com.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqliteExecutionException ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
        }

        public bool RemoveWarp(string name)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "DELETE FROM Warps WHERE WarpName=@name AND WorldID=@worldid";
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldid", Main.worldID.ToString());
                    com.ExecuteNonQuery();
                    return true;
                }
            }
            catch (SqliteExecutionException ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
        }

        public Warp FindWarp(string name)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Warps WHERE WarpName=@name AND WorldID=@worldid";
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldid", Main.worldID.ToString());
                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                            return new Warp(new Vector2(reader.Get<int>("X"), reader.Get<int>("Y")), reader.Get<string>("WarpName"), reader.Get<string>("WorldID"));
                    }
                }
            }
            catch (SqliteExecutionException ex)
            {
                Log.Error(ex.ToString());
            }
            return new Warp();
        }

        public List<Warp> ListAllWarps()
        {
            List<Warp> Warps = new List<Warp>();
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Warps";
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                            Warps.Add(new Warp(new Vector2(reader.Get<int>("X"), reader.Get<int>("Y")), reader.Get<string>("WarpName"), reader.Get<string>("WorldID")));
                    }
                }
            }
            catch (SqliteExecutionException ex)
            {
                Log.Error(ex.ToString());
            }
            return Warps;
        }
    }

    public class Warp
    {
        public Vector2 WarpPos { get; set; }
        public string WarpName { get; set; }
        public string WorldWarpID { get; set; }

        public Warp(Vector2 warppos, string name, string worldname)
        {
            WarpPos = warppos;
            WarpName = name;
            WorldWarpID = worldname;
        }

        public Warp()
        {
            WarpPos = Vector2.Zero;
            WarpName = null;
            WorldWarpID = string.Empty;
        }
    }
}
