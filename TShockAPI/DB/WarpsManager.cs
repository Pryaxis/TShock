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
using System.Xml;
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
                new SqlColumn("WarpName", MySqlDbType.VarChar, 50) { Primary = true },
                new SqlColumn("X", MySqlDbType.Int32),
                new SqlColumn("Y", MySqlDbType.Int32),
                new SqlColumn("WorldID", MySqlDbType.Text),
                new SqlColumn("Private", MySqlDbType.Text)
            );
            var creator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            creator.EnsureExists(table);

            String file = Path.Combine(TShock.SavePath, "warps.xml");
            String name = "";
            String world = "";
            int x1 = 0;
            int y1 = 0;
            if (!File.Exists(file))
                return;

            using (var reader = XmlReader.Create(new StreamReader(file), new XmlReaderSettings { CloseInput = true }))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "Warp":
                                    name = "";
                                    world = "";
                                    x1 = 0;
                                    y1 = 0;
                                    break;
                                case "WarpName":
                                    while (reader.NodeType != XmlNodeType.Text)
                                        reader.Read();
                                    name = reader.Value;
                                    break;
                                case "X":
                                    while (reader.NodeType != XmlNodeType.Text)
                                        reader.Read();
                                    int.TryParse(reader.Value, out x1);
                                    break;
                                case "Y":
                                    while (reader.NodeType != XmlNodeType.Text)
                                        reader.Read();
                                    int.TryParse(reader.Value, out y1);
                                    break;
                                case "WorldName":
                                    while (reader.NodeType != XmlNodeType.Text)
                                        reader.Read();
                                    world = reader.Value;
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:

                            break;
                        case XmlNodeType.XmlDeclaration:
                        case XmlNodeType.ProcessingInstruction:
                            break;
                        case XmlNodeType.Comment:
                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name.Equals("Warp"))
                            {
                                string query = (TShock.Config.StorageType.ToLower() == "sqlite")
                                                   ? "INSERT OR IGNORE INTO Warps VALUES (@0, @1,@2, @3);"
                                                   : "INSERT IGNORE INTO Warps SET X=@0, Y=@1, WarpName=@2, WorldID=@3;";
                                database.Query(query, x1, y1, name, world);
                            }
                            break;
                    }
                }

            }

            String path = Path.Combine(TShock.SavePath, "old_configs");
            String file2 = Path.Combine(path, "warps.xml");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (File.Exists(file2))
                File.Delete(file2);
            File.Move(file, file2);
        }

        public void ConvertDB()
        {
            try
            {
                database.Query("UPDATE Warps SET WorldID=@0", Main.worldID.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
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
                using (var reader = database.QueryReader("SELECT * FROM Warps WHERE WarpName=@0 AND WorldID=@1", name, Main.worldID.ToString()))
                {
                    if (reader.Read())
                    {
                        try
                        {
                            return new Warp(new Vector2(reader.Get<int>("X"), reader.Get<int>("Y")), reader.Get<string>("WarpName"), reader.Get<string>("WorldID"), reader.Get<string>("Private"));
                        }
                        catch
                        {
                            return new Warp(new Vector2(reader.Get<int>("X"), reader.Get<int>("Y")), reader.Get<string>("WarpName"), reader.Get<string>("WorldID"), "0");
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
                                warps.Add(new Warp { WarpName = reader.Get<string>("WarpName") });
                        }
                        catch
                        {
                            warps.Add(new Warp { WarpName = reader.Get<string>("WarpName") });
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
