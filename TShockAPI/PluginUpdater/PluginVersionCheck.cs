/*
TShock, a server mod for Terraria
Copyright (C) 2011-2013 Nyx Studios (fka. The TShock Team)

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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using JsonLoader;
using Newtonsoft.Json;

namespace TShockAPI.PluginUpdater
{
    public class PluginVersionCheck
    {
        public delegate void PluginUpdateD(UpdateArgs e);
        public static event PluginUpdateD PluginUpdate;
        public static void OnPluginUpdate(UpdateArgs args)
        {
            if (PluginUpdate == null)
            {
                return;
            }

            PluginUpdate(args);
        }

        public static void CheckPlugin(object p)
        {
            TerrariaPlugin plugin = (TerrariaPlugin)p;
            UpdateArgs args = new UpdateArgs {Plugin = plugin, Success = true, Error = ""};
            List<string> files = new List<string>();

            try
            {
                if (!String.IsNullOrEmpty(plugin.UpdateURL))
                {
                    var request = HttpWebRequest.Create(plugin.UpdateURL);
                    VersionInfo vi;

                    request.Timeout = 5000;
                    using (var response = request.GetResponse())
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            vi = JsonConvert.DeserializeObject<VersionInfo>(reader.ReadToEnd());
                        }
                    }

                    System.Version v = System.Version.Parse((vi.version.ToString()));

                    if (!v.Equals(plugin.Version))
                    {
                        DownloadPackage pkg;
                        request = HttpWebRequest.Create(vi.url);

                        request.Timeout = 5000;
                        using (var response = request.GetResponse())
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                pkg = JsonConvert.DeserializeObject<DownloadPackage>(reader.ReadToEnd());
                            }
                        }

                        foreach (PluginFile f in pkg.files)
                        {
                            using (WebClient Client = new WebClient())
                            {
                                string dir = Path.Combine(TShock.SavePath, "UpdatedPlugins");
                                if (!Directory.Exists(dir))
                                    Directory.CreateDirectory(dir);

                                Client.DownloadFile(f.url,
                                                    Path.Combine(dir, f.destination));

                                files.Add(Path.Combine(dir, f.destination));
                            }                            
                        }
                    }
                    else
                    {
                        args.Error = "Plugin is up to date.";
                    }
                }
                else
                {
                    args.Error = "Plugin has no updater recorded.";
                }
            }
            catch(Exception e)
            {
                args.Success = false;
                args.Error = e.Message;
                if(files.Count > 0)
                {
                    foreach(string s in files)
                    {
                        File.Delete(s);
                    }
                }
            }

            OnPluginUpdate(args);
        }
    }

    public class UpdateArgs
    {
        public TerrariaPlugin Plugin { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }
    }
}
