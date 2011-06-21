using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;

namespace TShockAPI
{
    class WarpsManager
    {
        public static List<Warp> Warps = new List<Warp>();

        public static bool AddWarp(int x, int y, string name)
        {
            foreach (Warp nametest in Warps)
            {
                if (name.ToLower() == nametest.WarpName.ToLower())
                {
                    return false;
                }
            }
            Warps.Add(new Warp(new Vector2(x, y), name));
            return true;
        }

        public static bool DeleteWarp(string name)
        {
            foreach (Warp nametest in Warps)
            {
                if (name.ToLower() == nametest.WarpName.ToLower())
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
                if (name.ToLower() == nametest.WarpName.ToLower())
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

                using (XmlWriter settingsw = XmlWriter.Create("warps.xml", xmlWriterSettings))
                {
                    settingsw.WriteStartDocument();
                    settingsw.WriteStartElement("Warps");

                    foreach (Warp warp in Warps)
                    {
                        settingsw.WriteStartElement("Warp");
                        settingsw.WriteElementString("WarpName", warp.WarpName);
                        settingsw.WriteElementString("X", warp.WarpPos.X.ToString());
                        settingsw.WriteElementString("Y", warp.WarpPos.Y.ToString());
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

                using (XmlReader settingr = XmlReader.Create("warps.xml", xmlReaderSettings))
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
                                            string name;
                                            int x = 0;
                                            int y = 0;

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

                                            AddWarp(x, y, name);
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

        public Warp(Vector2 warppos, string name)
        {
            WarpPos = warppos;
            WarpName = name;
        }

        public Warp()
        {
            WarpPos = Vector2.Zero;
            WarpName = null;
        }
    }
}
