using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace TShockAPI
{
    public class RegionManager
    {
        public static List<Region> Regions = new List<Region>();

        public static bool AddRegion(int tx, int ty, int width, int height, string name, bool state)
        {
            foreach (Region nametest in Regions)
            {
                if (name.ToLower() == nametest.RegionName.ToLower())
                {
                    return false;
                }
            }
            Regions.Add(new Region(new Rectangle(tx, ty, width, height), name, true));
            return true;
        }

        public static bool DeleteRegion(string name)
        {
            foreach (Region nametest in Regions)
            {
                if (name.ToLower() == nametest.RegionName.ToLower())
                {
                    Regions.Remove(nametest);
                    WriteSettings();
                    return true;
                }
            }
            return false;
        }

        public static bool SetRegionState(string name, bool state)
        {
            foreach (Region nametest in Regions)
            {
                if (name.ToLower() == nametest.RegionName.ToLower())
                {
                    nametest.DisableBuild = state;
                    WriteSettings();
                    return true;
                }
            }
            return false;
        }

        public static bool InProtectedArea(int X, int Y)
        {
            foreach(Region region in Regions)
            {
                if (X >= region.RegionArea.Left && X <= region.RegionArea.Right && Y >= region.RegionArea.Top && Y <= region.RegionArea.Bottom && region.DisableBuild)
                {
                    return true;
                }
            }
            return false;
        }

        public static void WriteSettings()
        {
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineChars = Environment.NewLine;

                using (XmlWriter settingsw = XmlWriter.Create("regions.xml", xmlWriterSettings))
                {
                    settingsw.WriteStartDocument();
                    settingsw.WriteStartElement("Regions");

                    foreach (Region region in Regions)
                    {
                        settingsw.WriteStartElement("ProtectedRegion");
                        settingsw.WriteElementString("RegionName", region.RegionName);
                        settingsw.WriteElementString("Point1X", region.RegionArea.X.ToString());
                        settingsw.WriteElementString("Point1Y", region.RegionArea.Y.ToString());
                        settingsw.WriteElementString("Point2X", region.RegionArea.Width.ToString());
                        settingsw.WriteElementString("Point2Y", region.RegionArea.Height.ToString());
                        settingsw.WriteElementString("Protected", region.DisableBuild.ToString());
                        settingsw.WriteEndElement();
                    }

                    settingsw.WriteEndElement();
                    settingsw.WriteEndDocument();
                }
                Log.Info("Wrote Regions");
            }
            catch
            {
                Log.Info("Could not write Regions");
            }
        }

        public static void ReadAllSettings()
        {
            try
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.IgnoreWhitespace = true;

                using (XmlReader settingr = XmlReader.Create("regions.xml", xmlReaderSettings))
                {
                    while (settingr.Read())
                    {
                        if (settingr.IsStartElement())
                        {
                            switch (settingr.Name)
                            {                                    
                                case "Regions":
                                    {
                                        break;
                                    }                                
                                case "ProtectedRegion":
                                    {
                                        if (settingr.Read())
                                        {
                                            string name;
                                            int x = 0;
                                            int y = 0;
                                            int width = 0;
                                            int height = 0;
                                            bool state = true;

                                            settingr.Read();
                                            name = settingr.Value;

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                Int32.TryParse(settingr.Value, out x);
                                            else
                                                Console.WriteLine("Could not parse x");

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                Int32.TryParse(settingr.Value, out y);
                                            else
                                                Console.WriteLine("Could not parse y");

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                Int32.TryParse(settingr.Value, out width);
                                            else
                                                Console.WriteLine("Could not parse width");

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                Int32.TryParse(settingr.Value, out height);
                                            else
                                                Console.WriteLine("Could not parse height");

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                bool.TryParse(settingr.Value, out state);
                                            else
                                                Console.WriteLine("Could not parse state");

                                            AddRegion(x, y, width, height, name, state);
                                        }
                                        break;
                                    }                                    
                            }
                        }
                    }
                }
                Log.Info("Read Regions");
            }
            catch
            {                
                Log.Info("Could not read Regions");
                WriteSettings();
            }
        }
    }

    public class Region
    {
        public Rectangle RegionArea { get; set; }
        public string RegionName { get; set; }
        public bool DisableBuild { get; set; }

        public Region(Rectangle region, string name, bool disablebuild)
        {
            RegionArea = region;
            RegionName = name;
            DisableBuild = disablebuild;
        }

        public Region()
        {
            RegionArea = Rectangle.Empty;
            RegionName = string.Empty;
            DisableBuild = true;
        }
    }
}
