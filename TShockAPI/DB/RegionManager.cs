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
using System.Xml;
using System.IO;
using System.Data;
using TShockAPI.DB;
using Community.CsharpSqlite.SQLiteClient;
using Microsoft.Xna.Framework;
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

            using (var com = database.CreateCommand())
            {
                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS 'Regions' ('X1' NUMERIC, 'Y1' NUMERIC, 'height' NUMERIC, 'width' NUMERIC, 'RegionName' TEXT PRIMARY KEY, 'WorldID' TEXT, 'UserIds' TEXT, 'Protected' NUMERIC);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS Regions (X1 INT(11), Y1 INT(11), height INT(11), width INT(11), RegionName VARCHAR(255) PRIMARY, WorldID VARCHAR(255), UserIds VARCHAR(255), Protected INT(1));";

                com.ExecuteNonQuery();

                ImportOld();
            }
        }

        public void ImportOld()
        {
            String file = Path.Combine(TShock.SavePath, "regions.xml");
            if (!File.Exists(file))
                return;

            Region region;
            Rectangle rect;
            using (var sr = new StreamReader(file))
            {
                using (var reader = XmlReader.Create(sr))
                {
                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        if (reader.NodeType != XmlNodeType.Element || reader.Name != "ProtectedRegion")
                            continue;

                        region = new Region();
                        rect = new Rectangle();

                        bool endregion = false;
                        while (reader.Read() && !endregion)
                        {
                            if (reader.NodeType != XmlNodeType.Element)
                                continue;

                            string name = reader.Name;

                            while (reader.Read() && reader.NodeType != XmlNodeType.Text) ;

                            switch (name)
                            {
                                case "RegionName":
                                    region.Name = reader.Value;
                                    break;
                                case "Point1X":
                                    int.TryParse(reader.Value, out rect.X);
                                    break;
                                case "Point1Y":
                                    int.TryParse(reader.Value, out rect.Y);
                                    break;
                                case "Point2X":
                                    int.TryParse(reader.Value, out rect.Width);
                                    break;
                                case "Point2Y":
                                    int.TryParse(reader.Value, out rect.Height);
                                    break;
                                case "Protected":
                                    region.Protected = reader.Value.ToLower().Equals("true");
                                    break;
                                case "WorldName":
                                    region.WorldID = reader.Value;
                                    break;
                                case "AllowedUserCount":
                                    break;
                                case "IP":
                                    region.AllowedIDs.Add(reader.Value);
                                    break;
                                default:
                                    endregion = true;
                                    break;
                            }
                        }

                        region.Area = rect;
                        using (var com = database.CreateCommand())
                        {
                            if (TShock.Config.StorageType.ToLower() == "sqlite")
                                com.CommandText =
                                    "INSERT OR IGNORE INTO Regions VALUES (@tx, @ty, @height, @width, @name, @worldid, @userids, @protected);";
                            else if (TShock.Config.StorageType.ToLower() == "mysql")
                                com.CommandText =
                                    "INSERT IGNORE INTO Regions SET X1=@tx, Y1=@ty, height=@height, width=@width, RegionName=@name, WorldID=@world, UserIds=@userids, Protected=@protected;";
                            com.AddParameter("@tx", region.Area.X);
                            com.AddParameter("@ty", region.Area.Y);
                            com.AddParameter("@width", region.Area.Width);
                            com.AddParameter("@height", region.Area.Height);
                            com.AddParameter("@name", region.Name);
                            com.AddParameter("@worldid", region.WorldID);
                            //Todo: What should this be? We don't really have a way to go from ips to userids
                            com.AddParameter("@userids", ""/*string.Join(",", region.AllowedIDs)*/);
                            com.AddParameter("@protected", region.Protected);
                            int num = com.ExecuteNonQuery();
                        }
                    }
                }
            }
            String path = Path.Combine(TShock.SavePath, "old_configs");
            String file2 = Path.Combine(path, "regions.xml");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (File.Exists(file2))
                File.Delete(file2);
            File.Move(file, file2);

            ReloadAllRegions();
        }

        public void ReloadAllRegions()
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Regions WHERE WorldID=@worldid";
                    com.AddParameter("@worldid", Main.worldID.ToString());
                    using (var reader = com.ExecuteReader())
                    {
                        Regions.Clear();
                        while (reader.Read())
                        {
                            int X1 = reader.Get<int>("X1");
                            int Y1 = reader.Get<int>("Y1");
                            int height = reader.Get<int>("height");
                            int width = reader.Get<int>("width");
                            int Protected = reader.Get<int>("Protected");
                            string MergedIDs = DbExt.Get<string>(reader, "UserIds");
                            string name = DbExt.Get<string>(reader, "RegionName");

                            Region r = new Region(new Rectangle(X1, Y1, width, height), name, Protected != 0, Main.worldID.ToString());
                            r.AllowedIDs = MergedIDs.Split(',').ToList();
                            Regions.Add(r);
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public bool AddRegion(Region region)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText =
                        "INSERT INTO Regions VALUES (@tx, @ty, @height, @width, @name, @worldid, @userids, @protected);";
                    com.AddParameter("@tx", region.Area.X);
                    com.AddParameter("@ty", region.Area.Y);
                    com.AddParameter("@width", region.Area.Right);
                    com.AddParameter("@height", region.Area.Bottom);
                    com.AddParameter("@name", region.Name);
                    com.AddParameter("@worldid", region.WorldID);
                    com.AddParameter("@userids", string.Join(",", region.AllowedIDs));
                    com.AddParameter("@protected", region.Protected);
                    if (com.ExecuteNonQuery() > 0)
                    {
                        ReloadAllRegions();
                        return true;
                    }

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
        }

        public bool AddRegion(int tx, int ty, int width, int height, string regionname, string worldid)
        {
            return AddRegion(new Region { Area = new Rectangle(tx, ty, width, height), Name = regionname, WorldID = worldid });
        }

        public bool DeleteRegion(string name)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "DELETE FROM Regions WHERE RegionName=@name AND WorldID=@worldid";
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldid", Main.worldID.ToString());
                    com.ExecuteNonQuery();
                    ReloadAllRegions();
                    return true;
                }
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
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "UPDATE Regions SET Protected=@bool WHERE RegionName=@name AND WorldID=@worldid";
                    com.AddParameter("@name", name);
                    com.AddParameter("@bool", state ? 1 : 0);
                    com.AddParameter("@worldid", Main.worldID.ToString());
                    int q = com.ExecuteNonQuery();
                    ReloadAllRegions();
                    return (q > 0);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }

        public bool CanBuild(int x, int y, TSPlayer ply)
        {
            User user = TShock.Users.GetUserByName(ply.TPlayer.name);
            if (!ply.Group.HasPermission("canbuild"))
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
                    region.Protected)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> ListIDs(string MergedIDs)
        {
            List<string> SplitIDs = new List<string>();
            var sb = new StringBuilder();
            for (int i = 0; i < MergedIDs.Length; i++)
            {
                char c = MergedIDs[i];

                if (c != ',')
                {
                    sb.Append(c);
                }
                else if (sb.Length > 0)
                {
                    SplitIDs.Add(sb.ToString());
                    sb.Clear();
                }
            }
            return SplitIDs;
        }

        public bool AddNewUser(string regionName, String userName)
        {
            string MergedIDs = string.Empty;

            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Regions WHERE RegionName=@name AND WorldID=@worldid";
                    com.AddParameter("@name", regionName);
                    com.AddParameter("@worldid", Main.worldID.ToString());

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                            MergedIDs = reader.Get<string>("UserIds");
                    }

                    if (MergedIDs == string.Empty)
                        MergedIDs = userName;
                    else
                        MergedIDs = MergedIDs + "," + userName;
                    com.Parameters.Clear();
                    com.CommandText = "UPDATE Regions SET UserIds=@ids WHERE RegionName=@name AND WorldID=@worldid";
                    com.AddParameter("@ids", MergedIDs);
                    com.AddParameter("@name", regionName);
                    com.AddParameter("@worldid", Main.worldID.ToString());
                    if (com.ExecuteNonQuery() > 0)
                    {
                        ReloadAllRegions();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return false;
            }
        }

        public List<Region> ListAllRegions()
        {
            List<Region> Regions = new List<Region>();
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Regions";
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                            Regions.Add(new Region(new Rectangle(reader.Get<int>("X1"), reader.Get<int>("Y1"), reader.Get<int>("height"), reader.Get<int>("width")), reader.Get<string>("RegionName"), reader.Get<int>("Protected") != 0, reader.Get<string>("WorldID")));
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return Regions;
        }
    }

    public class Region
    {
        public Rectangle Area { get; set; }
        public string Name { get; set; }
        public bool Protected { get; set; }
        public string WorldID { get; set; }
        public List<string> AllowedIDs { get; set; }

        public Region(Rectangle region, string name, bool disablebuild, string worldid)
            : this()
        {
            Area = region;
            Name = name;
            Protected = disablebuild;
            WorldID = worldid;
        }

        public Region()
        {
            Area = Rectangle.Empty;
            Name = string.Empty;
            Protected = true;
            WorldID = string.Empty;
            AllowedIDs = new List<string>();
        }

        public bool InArea(Rectangle point)
        {
            return Area.Contains(point.X, point.Y);
        }

        public bool HasPermissionToBuildInRegion(TSPlayer ply)
        {
            if (!ply.IsLoggedIn)
            {
                ply.SendMessage("You must be logged in to take advantage of protected regions.", Color.Red);
                return false;
            }
            if (!Protected)
                return true;

            for (int i = 0; i < AllowedIDs.Count; i++)
            {
                if (AllowedIDs[i] == ply.UserAccountName)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
