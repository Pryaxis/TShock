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
using System.IO;
using System.Text;

namespace TShockAPI
{
    public class BanManager
    {
        private DateTime LastLoad;
        private string Path;

        /// <summary>
        /// IP - Name - Reason
        /// </summary>
        private List<Ban> Bans = new List<Ban>();

        public BanManager(string path)
        {
            Path = path;
        }

        public Ban GetBanByIp(string ip)
        {
            EnsureChanges();
            foreach (var ban in Bans)
            {
                if (ban.IP.Equals(ip))
                    return ban;
            }
            return null;
        }

        public Ban GetBanByName(string name, bool casesensitive = true)
        {
            EnsureChanges();
            foreach (var ban in Bans)
            {
                if (ban.Name.Equals(name,
                                    casesensitive
                                        ? StringComparison.Ordinal
                                        : StringComparison.InvariantCultureIgnoreCase))
                    return ban;
            }
            return null;
        }

        public void AddBan(string ip, string name = "", string reason = "")
        {
            if (GetBanByIp(ip) != null)
                return;
            Bans.Add(new Ban(ip, name, reason));
            SaveBans();
        }

        public void RemoveBan(Ban ban)
        {
            Bans.Remove(ban);
            SaveBans();
        }

        /// <summary>
        /// Reloads the file if it was changed
        /// </summary>
        public void EnsureChanges()
        {
            if (File.Exists(Path))
            {
                if (new FileInfo(Path).LastWriteTime > LastLoad)
                    LoadBans();
            }
            else
            {
                Bans.Clear();
            }
        }

        /// <summary>
        /// Removes | from the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string MakeSafe(string str)
        {
            return str.Replace("|", " ");
        }

        public void LoadBans()
        {
            Bans.Clear();

            if (!File.Exists(Path))
                return;

            LastLoad = new FileInfo(Path).LastWriteTime;

            foreach (var line in File.ReadAllLines(Path))
            {
                var bansp = line.Split('|');
                if (bansp.Length != 3)
                    continue;
                Bans.Add(new Ban(bansp[0], bansp[1], bansp[2]));
            }
        }

        public void SaveBans()
        {
            var output = new StringBuilder();
            foreach (var ban in Bans)
            {
                output.AppendFormat("{0}|{1}|{2}\r\n", MakeSafe(ban.IP), MakeSafe(ban.Name), MakeSafe(ban.Reason));
            }

            File.WriteAllText(Path, output.ToString());
        }
    }

    public class Ban
    {
        public string IP { get; set; }

        public string Name { get; set; }

        public string Reason { get; set; }

        public Ban(string ip, string name, string reason)
        {
            IP = ip;
            Name = name;
            Reason = reason;
        }

        public Ban()
        {
            IP = string.Empty;
            Name = string.Empty;
            Reason = string.Empty;
        }
    }
}