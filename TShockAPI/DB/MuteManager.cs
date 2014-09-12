/*
TShock, a server mod for Terraria
Copyright (C) 2011-2014 Nyx Studios (fka. The TShock Team)

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
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
    public class MuteManager
    {
        private IDbConnection database;

        public MuteManager(IDbConnection db)
        {
            database = db;

            var table = new SqlTable("MuteBans",
                                     new SqlColumn("Account", MySqlDbType.Text) { Primary = true },
                                     new SqlColumn("IP", MySqlDbType.String, 16),
                                     new SqlColumn("MuteDate", MySqlDbType.Text),
                                     new SqlColumn("UnmuteDate", MySqlDbType.Text),
                                     new SqlColumn("MutedBy", MySqlDbType.Text),
                                     new SqlColumn("Reason", MySqlDbType.Text)
                );
            var creator = new SqlTableCreator(db,
                                              db.GetSqlType() == SqlType.Sqlite
                                                ? (IQueryBuilder)new SqliteQueryCreator()
                                                : new MysqlQueryCreator());
            try
            {
                creator.EnsureExists(table);
            }
            catch (DllNotFoundException)
            {
                System.Console.WriteLine("Possible problem with your database - is Sqlite3.dll present?");
                throw new Exception("Could not find a database library (probably Sqlite3.dll)");
            }
        }

        public bool AddMute(string name = "", string ip = "", string unmutedate = "", string mutedby = "", string reason = "", bool exceptions = false)
        {
            try
            {
                return database.Query("INSERT INTO MuteBans (Account, IP, MuteDate, UnmuteDate, MutedBy, Reason) VALUES (@0, @1, @2, @3, @4, @5);", name, ip, DateTime.UtcNow.ToString("s"), unmutedate, mutedby, reason) != 0;
            }
            catch (Exception ex)
            {
                if (exceptions)
                    throw ex;
                Log.Error(ex.ToString());
            }
            return false;
        }

        public bool RemoveMute(string match, bool byName = false, bool casesensitive = true, bool exceptions = false)
        {
            try
            {
                if (!byName)
                    return database.Query("DELETE FROM MuteBans WHERE IP=@0", match) != 0;

                var namecol = casesensitive ? "Account" : "UPPER(Account)";
                return database.Query("DELETE FROM MuteBans WHERE " + namecol + "=@0", casesensitive ? match : match.ToUpper()) != 0;
            }
            catch (Exception ex)
            {
                if (exceptions)
                    throw ex;
                Log.Error(ex.ToString());
            }
            return false;
        }

        public Mute GetMuteByAccount(string name, bool casesensitive = true)
        {
            try
            {
                var namecol = casesensitive ? "Account" : "UPPER(Account)";
                if (!casesensitive)
                    name = name.ToUpper();
                using (var reader = database.QueryReader("SELECT * FROM MuteBans WHERE " + namecol + "=@0", name))
                {
                    if (reader.Read())
                        return new Mute(reader.Get<string>("Account"), reader.Get<string>("IP"), reader.Get<string>("MuteDate"), reader.Get<string>("UnmuteDate"), reader.Get<string>("MutedBy"), reader.Get<string>("Reason"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return null;
        }

        public Mute GetMuteByIp(string ip)
        {
            try
            {
                using (var reader = database.QueryReader("SELECT * FROM MuteBans WHERE IP=@0", ip))
                {
                    if (reader.Read())
                        return new Mute(reader.Get<string>("Account"), reader.Get<string>("IP"), reader.Get<string>("MuteDate"), reader.Get<string>("UnmuteDate"), reader.Get<string>("MutedBy"), reader.Get<string>("Reason"));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return null;
        }
    }



    public class Mute
	{
        public string Account { get; set; }
        public string IP { get; set; }
		public string MuteDate { get; set; }
		public string UnmuteDate { get; set; }
		public string MutedBy { get; set; }
        public string Reason { get; set; }

		public Mute(string account, string ip, string mutedate, string unmutedate, string mutedby, string reason)
		{
            Account = account;
            IP = ip;
			MuteDate = mutedate;
			UnmuteDate = unmutedate;
			MutedBy = mutedby;
		    Reason = reason;
		}

		public Mute()
		{
			Account = string.Empty;
            IP = "";
			MuteDate = "";
			UnmuteDate = "";
			MutedBy = "";
		    Reason = "";
		}
    }
}
