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
using System.Drawing;
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
                new SqlColumn("RegionName", MySqlDbType.VarChar, 50) { Primary = true },
                new SqlColumn("WorldID", MySqlDbType.Text),
                new SqlColumn("UserIds", MySqlDbType.Text),
                new SqlColumn("Protected", MySqlDbType.Int32)
            );
            var creator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            creator.EnsureExists(table);

            ImportOld();
            ReloadAllRegions();

        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public void ImportOld()
        {
            String file = Path.Combine(TShock.SavePath, "regions.xml");
            if (!File.Exists(file))
                return;

            Region region;
            Rectangle rect;

            using (var reader = XmlReader.Create(new StreamReader(file), new XmlReaderSettings { CloseInput = true }))
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

                        int t = 0;

                        switch (name)
                        {
                            case "RegionName":
                                region.Name = reader.Value;
                                break;
                            case "Point1X":
                                int.TryParse(reader.Value, out t);
                                rect.X = t;
                                break;
                            case "Point1Y":
                                int.TryParse(reader.Value, out t);
                                rect.Y = t;
                                break;
                            case "Point2X":
                                int.TryParse(reader.Value, out t);
                                rect.Width = t;
                                break;
                            case "Point2Y":
                                int.TryParse(reader.Value, out t);
                                rect.Height = t;
                                break;
                            case "Protected":
                                region.DisableBuild = reader.Value.ToLower().Equals("true");
                                break;
                            case "WorldName":
                                region.WorldID = reader.Value;
                                break;
                            case "AllowedUserCount":
                                break;
                            case "IP":
                                region.AllowedIDs.Add(int.Parse(reader.Value));
                                break;
                            default:
                                endregion = true;
                                break;
                        }
                    }

                    region.Area = rect;
                    string query = (TShock.Config.StorageType.ToLower() == "sqlite") ?
                        "INSERT OR IGNORE INTO Regions VALUES (@0, @1, @2, @3, @4, @5, @6, @7);" :
                        "INSERT IGNORE INTO Regions SET X1=@0, Y1=@1, height=@2, width=@3, RegionName=@4, WorldID=@5, UserIds=@6, Protected=@7;";
                    database.Query(query, region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Name, region.WorldID, "", region.DisableBuild);

                    //Todo: What should this be? We don't really have a way to go from ips to userids
                    /*string.Join(",", region.AllowedIDs)*/

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

        public void ConvertDB()
        {
            try
            {
                database.Query("UPDATE Regions SET WorldID=@0, UserIds=''", Main.worldID.ToString());
                ReloadAllRegions();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
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

                        string[] splitids = mergedids.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                        Region r = new Region(new Rectangle(X1, Y1, width, height), name, Protected != 0, Main.worldID.ToString());

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

                    Region r = new Region(new Rectangle(X1, Y1, width, height), name, Protected != 0, Main.worldID.ToString());
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

        public bool AddRegion(int tx, int ty, int width, int height, string regionname, string worldid)
        {
            if (GetRegionByName(regionname) != null)
            {
                return false;
            }
            try
            {
                database.Query("INSERT INTO Regions (X1, Y1, width, height, RegionName, WorldID, UserIds, Protected) VALUES (@0, @1, @2, @3, @4, @5, @6, @7);",
                    tx, ty, width, height, regionname, worldid, "", 1);
                Regions.Add(new Region(new Rectangle(tx, ty, width, height), regionname, true, worldid));
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
                database.Query("UPDATE Regions SET Protected=@0 WHERE RegionName=@1 AND WorldID=@2", state ? 1 : 0, name, Main.worldID.ToString());
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
            return MergedIDs.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
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
                using (var reader = database.QueryReader("SELECT * FROM Regions WHERE RegionName=@0 AND WorldID=@1", regionName, Main.worldID.ToString()))
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
                        regions.Add(new Region { Name = reader.Get<string>("RegionName") });
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
    }

    public class Region
    {
        public Rectangle Area { get; set; }
        public string Name { get; set; }
        public bool DisableBuild { get; set; }
        public string WorldID { get; set; }
        public List<int> AllowedIDs { get; set; }

        public Region(Rectangle region, string name, bool disablebuild, string RegionWorldIDz)
            : this()
        {
            Area = region;
            Name = name;
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

            for (int i = 0; i < AllowedIDs.Count; i++)
            {
                if (AllowedIDs[i] == ply.UserID)
                {
                    return true;
                }
            }
            return false;
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
    }
}
