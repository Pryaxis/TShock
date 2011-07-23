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

                String file = Path.Combine(TShock.SavePath, "regions.xml");
                String name = "";
                String world = "";
                int x1 = 0;
                int x2 = 0;
                int y1 = 0;
                int y2 = 0;
                int prot = 0;
                int users = 0;
                String[] ips;
                String ipstr = "";
                int updates = 0;
                if (File.Exists(file))
                {
                    XmlReader reader;
                    reader = XmlReader.Create(new StreamReader(file));
                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "ProtectedRegion":
                                        name = "";
                                        world = "";
                                        x1 = 0;
                                        x2 = 0;
                                        y1 = 0;
                                        y2 = 0;
                                        prot = 0;
                                        users = 0;
                                        ips = null;
                                        ipstr = "";
                                        break;
                                    case "RegionName":
                                        while (reader.NodeType != XmlNodeType.Text)
                                            reader.Read();
                                        name = reader.Value;
                                        break;
                                    case "Point1X":
                                        while (reader.NodeType != XmlNodeType.Text)
                                            reader.Read();
                                        int.TryParse(reader.Value, out x1);
                                        break;
                                    case "Point1Y":
                                        while (reader.NodeType != XmlNodeType.Text)
                                            reader.Read();
                                        int.TryParse(reader.Value, out y1);
                                        break;
                                    case "Point2X":
                                        while (reader.NodeType != XmlNodeType.Text)
                                            reader.Read();
                                        int.TryParse(reader.Value, out x2);
                                        break;
                                    case "Point2Y":
                                        while (reader.NodeType != XmlNodeType.Text)
                                            reader.Read();
                                        int.TryParse(reader.Value, out y2);
                                        break;
                                    case "Protected":
                                        while (reader.NodeType != XmlNodeType.Text)
                                            reader.Read();
                                        if (reader.Value.Equals("True"))
                                        {
                                            prot = 0;
                                        }
                                        else
                                        {
                                            prot = 1;
                                        }
                                        break;
                                    case "WorldName":
                                        while (reader.NodeType != XmlNodeType.Text)
                                            reader.Read();
                                        world = reader.Value;
                                        break;
                                    case "AllowedUserCount":
                                        while (reader.NodeType != XmlNodeType.Text)
                                            reader.Read();
                                        int.TryParse(reader.Value, out users);
                                        if (users > 0)
                                        {
                                            ips = new String[users];
                                            for (int i = 0; i < users; i++)
                                            {
                                                do
                                                    reader.Read();
                                                while (reader.NodeType != XmlNodeType.Text);
                                                ips[i] = reader.Value;
                                            }
                                            ipstr = "";
                                            for (int i = 0; i < ips.Length; i++)
                                            {
                                                try
                                                {
                                                    if (ipstr != "")
                                                        ipstr += ",";
                                                    ipstr += TShock.Users.GetUserID(ips[i]);
                                                }
                                                catch (Exception)
                                                {
                                                    Log.Error("An IP address failed to import. It wasn't a user in the new user system.");
                                                }

                                            }

                                        }
                                        if (TShock.Config.StorageType.ToLower() == "sqlite")
                                            com.CommandText = "INSERT OR IGNORE INTO Regions VALUES (@tx, @ty, @height, @width, @name, @worldid, @userids, @protected);";
                                        else if (TShock.Config.StorageType.ToLower() == "mysql")
                                            com.CommandText = "INSERT IGNORE INTO Regions SET X1=@tx, Y1=@ty, height=@height, width=@width, RegionName=@name, WorldID=@world, UserIds=@userids, Protected=@protected;";
                                        com.AddParameter("@tx", x1);
                                        com.AddParameter("@ty", y1);
                                        com.AddParameter("@width", x2);
                                        com.AddParameter("@height", y2);
                                        com.AddParameter("@name", name);
                                        com.AddParameter("@worldid", world);
                                        com.AddParameter("@userids", ipstr);
                                        com.AddParameter("@protected", prot);
                                        updates += com.ExecuteNonQuery();
                                        com.Parameters.Clear();
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

                                break;
                        }
                    }
                    reader.Close();
                    reader = null;
                    String path = Path.Combine(TShock.SavePath, "old_configs");
                    String file2 = Path.Combine(path, "regions.xml");
                    if (!Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    if (File.Exists(file2))
                        File.Delete(file2);
                    //File.Move(file, file2);
                }
                if (updates > 0)
                    ReloadAllRegions();
            }
        }

        public void ConvertDB()
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "UPDATE Regions SET WorldID=@worldid";
                    com.AddParameter("@worldid", Main.worldID.ToString());
                    com.ExecuteNonQuery();
                    com.Parameters.Clear();
                    com.CommandText = "UPDATE Regions SET UserIds=@UserIds";
                    com.AddParameter("@UserIds", "");
                    com.ExecuteNonQuery();
                    ReloadAllRegions();
                }
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
                            System.Console.WriteLine(MergedIDs);
                            string[] SplitIDs = MergedIDs.Split(',');

                            Region r = new Region(new Rectangle(X1, Y1, width, height), name, Protected, Main.worldID.ToString());
                            r.RegionAllowedIDs = new int[SplitIDs.Length];
                            try
                            {
                                for (int i = 0; i < SplitIDs.Length; i++)
                                {
                                    if (SplitIDs.Length == 1 && SplitIDs[0].Equals(""))
                                    {
                                        break;
                                    }
                                    //System.Console.WriteLine(SplitIDs[i]);
                                    r.RegionAllowedIDs[i] = Convert.ToInt32(SplitIDs[i]);
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
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public bool AddRegion(int tx, int ty, int width, int height, string regionname, string worldid)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText =
                        "INSERT INTO Regions VALUES (@tx, @ty, @height, @width, @name, @worldid, @userids, @protected);";
                    com.AddParameter("@tx", tx);
                    com.AddParameter("@ty", ty);
                    com.AddParameter("@width", width);
                    com.AddParameter("@height", height);
                    com.AddParameter("@name", regionname.ToLower());
                    com.AddParameter("@worldid", worldid);
                    com.AddParameter("@userids", "");
                    com.AddParameter("@protected", 1);
                    if (com.ExecuteNonQuery() > 0)
                    {
                        Regions.Add(new Region(new Rectangle(tx, ty, width, height), regionname, 0, worldid));
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
                if (x >= region.RegionArea.Left && x <= region.RegionArea.Right &&
                    y >= region.RegionArea.Top && y <= region.RegionArea.Bottom &&
                    region.DisableBuild == 1)
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


            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Regions WHERE RegionName=@name AND WorldID=@worldid";
                    com.AddParameter("@name", regionName);
                    com.AddParameter("@worldid", Main.worldID.ToString());
                    string MergedIDs = string.Empty;
                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                            MergedIDs = reader.Get<string>("UserIds");
                    }

                    if (MergedIDs == string.Empty)
                        MergedIDs = Convert.ToString(TShock.Users.GetUserID(userName));
                    else
                        MergedIDs = MergedIDs + "," + Convert.ToString(TShock.Users.GetUserID(userName));
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
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT RegionName FROM Regions WHERE WorldID=@worldid";
                    com.AddParameter("@worldid", worldid);
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                            regions.Add(new Region { RegionName = reader.Get<string>("RegionName") });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return regions;
        }
    }

    public class Region
    {
        public Rectangle RegionArea { get; set; }
        public string RegionName { get; set; }
        public int DisableBuild { get; set; }
        public string RegionWorldID { get; set; }
        public int[] RegionAllowedIDs { get; set; }

        public Region(Rectangle region, string name, int disablebuild, string RegionWorldIDz)
        {
            RegionArea = region;
            RegionName = name;
            DisableBuild = disablebuild;
            RegionWorldID = RegionWorldIDz;
        }

        public Region()
        {
            RegionArea = Rectangle.Empty;
            RegionName = string.Empty;
            DisableBuild = 1;
            RegionWorldID = string.Empty;
        }

        public bool InArea(Rectangle point)
        {
            if (RegionArea.Contains(point.X, point.Y))
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
            if (DisableBuild == 0)
            {
                return true;
            }

            for (int i = 0; i < RegionAllowedIDs.Length; i++)
            {
                if (RegionAllowedIDs[i] == ply.UserID)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
