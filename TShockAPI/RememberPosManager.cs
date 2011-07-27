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
using System.Xml;
using Microsoft.Xna.Framework;

namespace TShockAPI
{
    class RemeberedPosManager
    {
        public static List<RemeberedPos> RemeberedPosistions = new List<RemeberedPos>();

        public static void LoadPos()
        {
            try
            {
                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
                xmlReaderSettings.IgnoreWhitespace = true;

                using (XmlReader settingr = XmlReader.Create(FileTools.RememberedPosPath, xmlReaderSettings))
                {
                    while (settingr.Read())
                    {
                        if (settingr.IsStartElement())
                        {
                            switch (settingr.Name)
                            {
                                case "Positions":
                                    {
                                        break;
                                    }
                                case "Player":
                                    {
                                        if (settingr.Read())
                                        {
                                            string IP = null;
                                            float X = 0;
                                            float Y = 0;

                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                IP = settingr.Value;
                                            else
                                                Log.Warn("IP is empty");


                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                float.TryParse(settingr.Value, out X);
                                            else
                                                Log.Warn("X for IP " + IP + " is empty");

                                            settingr.Read();
                                            settingr.Read();
                                            settingr.Read();
                                            if (settingr.Value != "" || settingr.Value != null)
                                                float.TryParse(settingr.Value, out Y);
                                            else
                                                Log.Warn("Y for IP " + IP + " is empty");

                                            if (X != 0 && Y != 0)
                                                RemeberedPosistions.Add(new RemeberedPos(IP, new Vector2(X, Y)));
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                }
                Log.Info("Read Remembered Positions");
            }
            catch
            {
                Log.Warn("Could not read Remembered Positions");
                WriteSettings();
            }
        }

        public static void WriteSettings()
        {
            try
            {
                XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.NewLineChars = Environment.NewLine;

                using (XmlWriter settingsw = XmlWriter.Create(FileTools.RememberedPosPath, xmlWriterSettings))
                {
                    settingsw.WriteStartDocument();
                    settingsw.WriteStartElement("Positions");

                    foreach (RemeberedPos player in RemeberedPosistions)
                    {
                        settingsw.WriteStartElement("Player");
                        settingsw.WriteElementString("IP", player.IP);
                        settingsw.WriteElementString("X", player.Pos.X.ToString());
                        settingsw.WriteElementString("Y", player.Pos.Y.ToString());
                        settingsw.WriteEndElement();
                    }

                    settingsw.WriteEndElement();
                    settingsw.WriteEndDocument();
                }
                Log.Info("Wrote Remembered Positions");
            }
            catch
            {
                Log.Warn("Could not write Remembered Positions");
            }
        }
    }


    public class RemeberedPos
    {
        public string IP { get; set; }
        public Vector2 Pos { get; set; }

        public RemeberedPos(string ip, Vector2 pos)
        {
            IP = ip;
            Pos = pos;
        }

        public RemeberedPos()
        {
            IP = string.Empty;
            Pos = Vector2.Zero;
        }
    }
}
