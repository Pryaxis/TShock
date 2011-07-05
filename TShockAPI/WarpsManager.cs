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
                    "CREATE TABLE IF NOT EXISTS \"Warps\" (\"X\" INTEGER(11) NOT NULL  UNIQUE, \"Y\" INTEGER(11) NOT NULL  UNIQUE , \"WarpName\" VARCHAR(32) NOT NULL UNIQUE , \"WorldName\" VARCHAR(255) NOT NULL );";
                com.ExecuteNonQuery();
            }

            if(!File.Exists(FileTools.WarpsPath))
            {
                ReadWarps();
                File.Delete(FileTools.WarpsPath);
            }
        }



        public bool AddWarp(int x, int y, string name, string worldname)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "INSERT INTO Warps (X, Y, WarpName, WorldName) VALUES (@x, @y, @name, @worldname)";
                    com.AddParameter("@x", x);
                    com.AddParameter("@y", y);
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldname", worldname);
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
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldname", Main.worldName);
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
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@worldname", Main.worldName);
                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                            return new Warp(new Vector2(reader.Get<int>("X"),reader.Get<int>("Y")), reader.Get<string>("WarpName"), reader.Get<string>("WorldName"));
                    }
                }
            }
            catch (SqliteExecutionException ex)
            {
            }
            return new Warp();
        }

        public static void ReadWarps()
        {
            try
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.IgnoreWhitespace = true;

                using (XmlReader settingr = XmlReader.Create(FileTools.WarpsPath, xmlReaderSettings))
                {
                    while (settingr.Read())
                    {
                        if (settingr.IsStartElement())
                        {
                            switch (settingr.Name)
                            {
                                case "Warps":
                                    {
                                        break;
                                    }
                                case "Warp":
                                    {
                                        if (settingr.Read())
                                        {
                                            string name = string.Empty;
                                            int x = 0;
                                            int y = 0;
                                            string worldname = string.Empty;

                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                name = settingr.Value;
                                            else
                                                Log.Warn("Warp name is empty, This warp will not work");

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                Int32.TryParse(settingr.Value, out x);
                                            else
                                                Log.Warn("x for warp " + name + " is empty");

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                Int32.TryParse(settingr.Value, out y);
                                            else
                                                Log.Warn("y for warp " + name + " is empty");

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                worldname = settingr.Value;
                                            else
                                                Log.Warn("Worldname for warp " + name + " is empty");

                                            TShock.Warps.AddWarp(x, y, name, worldname);
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
                Log.Info("Read Warps");
            }
            catch
            {
                Log.Info("Could not read Warps");
            }
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
