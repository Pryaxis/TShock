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
using System.IO;
using Microsoft.Xna.Framework;
using System.Xml;
using Terraria;

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
                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS 'Warps' ('X' NUMERIC, 'Y' NUMERIC, 'WarpName' TEXT PRIMARY KEY, 'WorldID' TEXT);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText =
                       "CREATE TABLE IF NOT EXISTS Warps (X INT(11), Y INT(11), WarpName VARCHAR(255) PRIMARY, WorldID VARCHAR(255));";

                com.ExecuteNonQuery();

                String file = Path.Combine(TShock.SavePath, "warps.xml");
                String name = "";
                String world = "";
                int x1 = 0;
                int y1 = 0;
                if (File.Exists(file))
                {
                    XmlReader reader;
                    using (reader = XmlReader.Create(new StreamReader(file)))
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
                                        if (TShock.Config.StorageType.ToLower() == "sqlite")
                                            com.CommandText = "INSERT OR IGNORE INTO Warps VALUES (@tx, @ty,@name, @worldid);";
                                        else if (TShock.Config.StorageType.ToLower() == "mysql")
                                            com.CommandText = "INSERT IGNORE INTO Warps SET X=@tx, Y=@ty, WarpName=@name, WorldID=@worldid;";
                                        com.AddParameter("@tx", x1);
                                        com.AddParameter("@ty", y1);
                                        com.AddParameter("@name", name);
                                        com.AddParameter("@worldid", world);
                                        com.ExecuteNonQuery();
                                        com.Parameters.Clear();
                                    }
                                    break;
                            }
                        }
                        
                    }
                    reader.Close();
                    String path = Path.Combine(TShock.SavePath, "old_configs");
                    String file2 = Path.Combine(path, "warps.xml");
                    if (!Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    if (File.Exists(file2))
                        File.Delete(file2);
                    //File.Move(file, file2);
                }
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
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "DELETE FROM Warps WHERE WarpName=@name AND WorldID=@worldid";
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldid", Main.worldName);
                    com.ExecuteNonQuery();
                    return true;
                }
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
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Warps WHERE WarpName=@name AND WorldID=@worldid";
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldid", Main.worldName);
                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Warp(new Vector2(reader.Get<int>("X"), reader.Get<int>("Y")), reader.Get<string>("WarpName"), reader.Get<string>("WorldID"));                            
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
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

                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
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
