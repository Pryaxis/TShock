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
using System.Data;
using System.IO;

namespace TShockAPI.DB
{
    public class BanManager
    {
        private IDbConnection database;

        public BanManager(IDbConnection db)
        {
            database = db;

            string query;
            if (TShock.Config.StorageType.ToLower() == "sqlite")
                query =
                    "CREATE TABLE IF NOT EXISTS 'Bans' ('IP' TEXT PRIMARY KEY, 'Name' TEXT, 'Reason' TEXT);";
            else
                query =
                    "CREATE TABLE IF NOT EXISTS  Bans (IP VARCHAR(255) PRIMARY, Name VARCHAR(255), Reason VARCHAR(255));";

            db.Query(query);

            String file = Path.Combine(TShock.SavePath, "bans.txt");
            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] info = line.Split('|');
                        if (TShock.Config.StorageType.ToLower() == "sqlite")
                            query = "INSERT OR IGNORE INTO Bans (IP, Name, Reason) VALUES (@0, @1, @2);";
                        else
                            query = "INSERT IGNORE INTO Bans SET IP=@0, Name=@1, Reason=@2;";
                        db.Query(query, info[0].Trim(), info[1].Trim(), info[2].Trim());
                    }
                }
                String path = Path.Combine(TShock.SavePath, "old_configs");
                String file2 = Path.Combine(path, "bans.txt");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (File.Exists(file2))
                    File.Delete(file2);
                File.Move(file, file2);
            }
        }

        public Ban GetBanByIp(string ip)
        {
            try
            {
                using (var reader = database.QueryReader("SELECT * FROM Bans WHERE IP=@0", ip))
                {
                    if (reader.Read())
                        return new Ban((string)reader["IP"], (string)reader["Name"], (string)reader["Reason"]);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return null;
        }

        public Ban GetBanByName(string name, bool casesensitive = true)
        {
            if (!TShock.Config.EnableBanOnUsernames)
            {
                return null;
            }
            try
            {
                var namecol = casesensitive ? "Name" : "UPPER(Name)";
                if (!casesensitive)
                    name = name.ToUpper();
                using (var reader = database.QueryReader("SELECT * FROM Bans WHERE " + namecol + "=@0", name))
                {
                    if (reader.Read())
                        return new Ban((string)reader["IP"], (string)reader["Name"], (string)reader["Reason"]);

                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return null;
        }

        public bool AddBan(string ip, string name = "", string reason = "")
        {
            try
            {
                return database.Query("INSERT INTO Bans (IP, Name, Reason) VALUES (@0, @1, @2);", ip, name, reason) != 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
        }

        public bool RemoveBan(string ip)
        {
            try
            {
                return database.Query("DELETE FROM Bans WHERE IP=@ip", ip) != 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
        }
        public bool ClearBans()
        {
            try
            {
                return database.Query("DELETE FROM Bans") != 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
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