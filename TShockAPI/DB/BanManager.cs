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
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
	public class BanManager
	{
		private IDbConnection database;

		public BanManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Bans",
			                         new SqlColumn("IP", MySqlDbType.String, 16) {Primary = true},
			                         new SqlColumn("Name", MySqlDbType.Text),
			                         new SqlColumn("Reason", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
								try{
			creator.EnsureExists(table);
								}
								catch (DllNotFoundException ex)
{
System.Console.WriteLine("Possible problem with your database - is Sqlite3.dll present?");
throw new Exception("Could not find a database library (probably Sqlite3.dll)");
}

		}

		public Ban GetBanByIp(string ip)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans WHERE IP=@0", ip))
				{
					if (reader.Read())
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("Reason"));
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return null;
		}

		public List<Ban> GetBans()
		{
			List<Ban> banlist = new List<Ban>();
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans"))
				{
					while (reader.Read())
					{
						banlist.Add(new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("Reason")));						
					}
					return banlist;
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				Console.WriteLine(ex.StackTrace);
			}
			return null;
		}

		public Ban GetBanByName(string name, bool casesensitive = true)
		{
			try
			{
				var namecol = casesensitive ? "Name" : "UPPER(Name)";
				if (!casesensitive)
					name = name.ToUpper();
				using (var reader = database.QueryReader("SELECT * FROM Bans WHERE " + namecol + "=@0", name))
				{
					if (reader.Read())
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("Reason"));
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

		public bool RemoveBan(string match, bool byName = false, bool casesensitive = true)
		{
			try
			{
				if (!byName)
					return database.Query("DELETE FROM Bans WHERE IP=@0", match) != 0;

				var namecol = casesensitive ? "Name" : "UPPER(Name)";
				return database.Query("DELETE FROM Bans WHERE " + namecol + "=@0", casesensitive ? match : match.ToUpper()) != 0;
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
