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
using Microsoft.Xna.Framework;
using System.Xml;
using Terraria;

namespace TShockAPI
{
    class WarpsManager
    {
        public static List<Warp> Warps = new List<Warp>();

        public static bool AddWarp(int x, int y, string name, string worldname)
        {
            foreach (Warp nametest in Warps)
            {
                if (name.ToLower() == nametest.WarpName.ToLower() && worldname == nametest.WorldWarpName)
                {
                    return false;
                }
            }
            Warps.Add(new Warp(new Vector2(x, y), name, worldname));
            return true;
        }

        public static bool DeleteWarp(string name)
        {
            foreach (Warp nametest in Warps)
            {
                if (name.ToLower() == nametest.WarpName.ToLower() && nametest.WorldWarpName == Main.worldName)
                {
                    Warps.Remove(nametest);
                    WriteSettings();
                    return true;
                }
            }
            return false;
        }

        public static Vector2 FindWarp(string name)
        {
            foreach (Warp nametest in Warps)
            {
                if (name.ToLower() == nametest.WarpName.ToLower() && nametest.WorldWarpName == Main.worldName)
                {
                    return nametest.WarpPos;
                }
            }
            return Vector2.Zero;
        }

        public static void WriteSettings()
        {
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineChars = Environment.NewLine;

                using (XmlWriter settingsw = XmlWriter.Create(FileTools.WarpsPath, xmlWriterSettings))
                {
                    settingsw.WriteStartDocument();
                    settingsw.WriteStartElement("Warps");

                    foreach (Warp warp in Warps)
                    {
                        settingsw.WriteStartElement("Warp");
                        settingsw.WriteElementString("WarpName", warp.WarpName);
                        settingsw.WriteElementString("X", warp.WarpPos.X.ToString());
                        settingsw.WriteElementString("Y", warp.WarpPos.Y.ToString());
                        settingsw.WriteElementString("WorldName", warp.WorldWarpName);
                        settingsw.WriteEndElement();
                    }

                    settingsw.WriteEndElement();
                    settingsw.WriteEndDocument();
                }
                Log.Info("Wrote Warps");
            }
            catch
            {
                Log.Info("Could not write Warps");
            }
        }

        public static void ReadAllSettings()
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

                                            AddWarp(x, y, name, worldname);
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
                WriteSettings();
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
