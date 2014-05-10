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
using System.Data;
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
									 new SqlColumn("UUID", MySqlDbType.Text),
			                         new SqlColumn("Reason", MySqlDbType.Text),
                                     new SqlColumn("BanningUser", MySqlDbType.Text),
                                     new SqlColumn("Date", MySqlDbType.Text),
                                     new SqlColumn("Expiration", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
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

		/// <summary>
		/// Gets a ban by IP.
		/// </summary>
		/// <param name="ip">The IP.</param>
		/// <returns>The ban.</returns>
		public Ban GetBanByIp(string ip)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans WHERE IP=@0", ip))
				{
					if (reader.Read())
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return null;
		}

		/// <summary>
		/// Gets a list of bans.
		/// </summary>
		public List<Ban> GetBans()
		{
			List<Ban> banlist = new List<Ban>();
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans"))
				{
					while (reader.Read())
					{
						banlist.Add(new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration")));					
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

		/// <summary>
		/// Gets a ban by name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="casesensitive">Whether to check with case sensitivity.</param>
		/// <returns>The ban.</returns>
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
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return null;
		}

		/// <summary>
		/// Gets a ban by UUID.
		/// </summary>
		/// <param name="uuid">The UUID.</param>
		/// <returns>The ban.</returns>
		public Ban GetBanByUUID(string uuid)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans WHERE UUID=@0", uuid))
				{
					if (reader.Read())
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			return null;
		}

#if COMPAT_SIGS
		[Obsolete("This method is for signature compatibility for external code only")]
		public bool AddBan(string ip, string name, string reason)
		{
			return AddBan(ip, name, "", reason, false, "", "");
		}
#endif
		public bool AddBan(string ip, string name = "", string uuid = "", string reason = "", bool exceptions = false, string banner = "", string expiration = "")
		{
			try
			{
				return database.Query("INSERT INTO Bans (IP, Name, UUID, Reason, BanningUser, Date, Expiration) VALUES (@0, @1, @2, @3, @4, @5, @6);", ip, name, uuid, reason, banner, DateTime.UtcNow.ToString("s"), expiration) != 0;
			}
			catch (Exception ex)
			{
				if (exceptions)
					throw ex;
				Log.Error(ex.ToString());
			}
			return false;
		}

#if COMPAT_SIGS
		[Obsolete("This method is for signature compatibility for external code only")]
		public bool RemoveBan(string ip)
		{
			return RemoveBan(ip, false, true, false);
		}
#endif
		public bool RemoveBan(string match, bool byName = false, bool casesensitive = true, bool exceptions = false)
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
				if (exceptions)
					throw ex;
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

		public string UUID { get; set; }

		public string Reason { get; set; }

        public string BanningUser { get; set; }

        public string Date { get; set; }

        public string Expiration { get; set; }

		public Ban(string ip, string name, string uuid, string reason, string banner, string date, string exp)
		{
			IP = ip;
			Name = name;
			UUID = uuid;
			Reason = reason;
		    BanningUser = banner;
		    Date = date;
		    Expiration = exp;
		}

		public Ban()
		{
			IP = string.Empty;
			Name = string.Empty;
			UUID = string.Empty;
			Reason = string.Empty;
		    BanningUser = "";
		    Date = "";
		    Expiration = "";
		}
	}
}
