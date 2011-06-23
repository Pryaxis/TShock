using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria;
using Microsoft.Xna.Framework;
using System.Xml;

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
