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
        }

        /// <summary>
        /// Reloads the file if it was changed
        /// </summary>
        public void EnsureChanges()
        {
            if (new FileInfo(Path).LastWriteTime > LastLoad)
                LoadBans();
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
            if (!File.Exists(Path))
                return;

            Bans.Clear();
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