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
using System.Data;
using System.IO;
using System.Text;
using Community.CsharpSqlite.SQLiteClient;

namespace TShockAPI
{
    public class BanManager
    {
        private DateTime LastLoad;
        private IDbConnection database;

        /// <summary>
        /// IP - Name - Reason
        /// </summary>
        private List<Ban> Bans = new List<Ban>();

        public BanManager(IDbConnection db)
        {
            database = db;

            using (var com = database.CreateCommand())
            {
                com.CommandText =
                    "CREATE TABLE IF NOT EXISTS \"Bans\" (\"IP\" VARCHAR(15) NOT NULL  UNIQUE , \"Name\" VARCHAR(32) NOT NULL , \"Reason\" VARCHAR(255) NOT NULL );";
                com.ExecuteNonQuery();
            }
        }

        public Ban GetBanByIp(string ip)
        {
            using (var com = database.CreateCommand())
            {
                com.CommandText = "SELECT * FROM Bans WHERE IP=@ip";
                AddParameter(com, "@ip", ip);
                using (var reader = com.ExecuteReader())
                {
                    if (reader.Read())
                        return new Ban((string)reader["IP"], (string)reader["Name"], (string)reader["Reason"]);
                }
            }
            return null;
        }
        IDbDataParameter AddParameter(IDbCommand command, string name, object data)
        {
            var parm = command.CreateParameter();
            parm.ParameterName = name;
            parm.Value = data;
            command.Parameters.Add(parm);
            return parm;
        }


        public Ban GetBanByName(string name, bool casesensitive = true)
        {
            using (var com = database.CreateCommand())
            {
                var namecol = casesensitive ? "Name" : "UPPER(Name)";
                if (!casesensitive)
                    name = name.ToUpper();
                com.CommandText = "SELECT *, COUNT(*) FROM Bans WHERE " + namecol + "=@name LIMIT 5";
                AddParameter(com, "@name", name);
                using (var reader = com.ExecuteReader())
                {
                    if (reader.Read())
                        return new Ban((string)reader["IP"], (string)reader["Name"], (string)reader["Reason"]);
                }
            }
            return null;
        }

        public void AddBan(string ip, string name = "", string reason = "")
        {
            using (var com = database.CreateCommand())
            {
                com.CommandText = "INSERT INTO Bans (IP, Name, Reason) VALUES (@ip, @name, @reason)";
                AddParameter(com, "@ip", ip);
                AddParameter(com, "@name", name);
                AddParameter(com, "@reason", reason);
                com.ExecuteNonQuery();
            }
        }

        public void RemoveBan(Ban ban)
        {

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