/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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
using System.Linq;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
	/// <summary>
	/// Class that manages bans.
	/// </summary>
	public class BanManager
	{
		private IDbConnection database;

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.BanManager"/> class.
		/// </summary>
		/// <param name="db">A valid connection to the TShock database</param>
		public BanManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Bans",
									new SqlColumn("IP", MySqlDbType.String, 16) { Primary = true },
									new SqlColumn("Name", MySqlDbType.Text),
									new SqlColumn("UUID", MySqlDbType.Text),
									new SqlColumn("Reason", MySqlDbType.Text),
									new SqlColumn("BanningUser", MySqlDbType.Text),
									new SqlColumn("Date", MySqlDbType.Text),
									new SqlColumn("Expiration", MySqlDbType.Text),
									new SqlColumn("AccountName", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
				db.GetSqlType() == SqlType.Sqlite
					? (IQueryBuilder)new SqliteQueryCreator()
					: new MysqlQueryCreator());
			try
			{
				creator.EnsureTableStructure(table);
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
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("AccountName"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}

		/// <summary>
		/// Gets a list of bans sorted by their addition date from newest to oldest
		/// </summary>
		public List<Ban> GetBans() => GetSortedBans(BanSortMethod.AddedNewestToOldest).ToList();

		/// <summary>
		/// Retrieves an enumerable of <see cref="Ban"/> objects, sorted using the provided sort method
		/// </summary>
		/// <param name="sortMethod"></param>
		/// <returns></returns>
		public IEnumerable<Ban> GetSortedBans(BanSortMethod sortMethod)
		{
			List<Ban> banlist = new List<Ban>();
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Bans"))
				{
					while (reader.Read())
					{
						banlist.Add(new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("AccountName"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration")));
					}

					banlist.Sort(new BanComparer(sortMethod));
					return banlist;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
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
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("AccountName"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}

		/// <summary>
		/// Gets a ban by account name (not player/character name).
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="casesensitive">Whether to check with case sensitivity.</param>
		/// <returns>The ban.</returns>
		public Ban GetBanByAccountName(string name, bool casesensitive = false)
		{
			try
			{
				var namecol = casesensitive ? "AccountName" : "UPPER(AccountName)";
				if (!casesensitive)
					name = name.ToUpper();
				using (var reader = database.QueryReader("SELECT * FROM Bans WHERE " + namecol + "=@0", name))
				{
					if (reader.Read())
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("AccountName"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
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
						return new Ban(reader.Get<string>("IP"), reader.Get<string>("Name"), reader.Get<string>("UUID"), reader.Get<string>("AccountName"), reader.Get<string>("Reason"), reader.Get<string>("BanningUser"), reader.Get<string>("Date"), reader.Get<string>("Expiration"));
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}

		/// <summary>
		/// Adds a ban.
		/// </summary>
		/// <returns><c>true</c>, if ban was added, <c>false</c> otherwise.</returns>
		/// <param name="ip">Ip.</param>
		/// <param name="name">Name.</param>
		/// <param name="uuid">UUID.</param>
		/// <param name="reason">Reason.</param>
		/// <param name="exceptions">If set to <c>true</c> enable throwing exceptions.</param>
		/// <param name="banner">Banner.</param>
		/// <param name="expiration">Expiration date.</param>
		public bool AddBan(string ip, string name = "", string uuid = "", string accountName = "", string reason = "", bool exceptions = false, string banner = "", string expiration = "")
		{
			try
			{
				/*
				* If the ban already exists, update its entry with the new date
				* and expiration details.
				*/
				if (GetBanByIp(ip) != null)
				{
					return database.Query("UPDATE Bans SET Date = @0, Expiration = @1 WHERE IP = @2", DateTime.UtcNow.ToString("s"), expiration, ip) == 1;
				}
				else
				{
					return database.Query("INSERT INTO Bans (IP, Name, UUID, Reason, BanningUser, Date, Expiration, AccountName) VALUES (@0, @1, @2, @3, @4, @5, @6, @7);", ip, name, uuid, reason, banner, DateTime.UtcNow.ToString("s"), expiration, accountName) != 0;
				}
			}
			catch (Exception ex)
			{
				if (exceptions)
					throw ex;
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Removes a ban.
		/// </summary>
		/// <returns><c>true</c>, if ban was removed, <c>false</c> otherwise.</returns>
		/// <param name="match">Match.</param>
		/// <param name="byName">If set to <c>true</c> by name.</param>
		/// <param name="casesensitive">If set to <c>true</c> casesensitive.</param>
		/// <param name="exceptions">If set to <c>true</c> exceptions.</param>
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
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Removes a ban by account name (not character/player name).
		/// </summary>
		/// <returns><c>true</c>, if ban was removed, <c>false</c> otherwise.</returns>
		/// <param name="match">Match.</param>
		/// <param name="casesensitive">If set to <c>true</c> casesensitive.</param>
		public bool RemoveBanByAccountName(string match, bool casesensitive = true)
		{
			try
			{
				var namecol = casesensitive ? "AccountName" : "UPPER(AccountName)";
				return database.Query("DELETE FROM Bans WHERE " + namecol + "=@0", casesensitive ? match : match.ToUpper()) != 0;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Clears bans.
		/// </summary>
		/// <returns><c>true</c>, if bans were cleared, <c>false</c> otherwise.</returns>
		public bool ClearBans()
		{
			try
			{
				return database.Query("DELETE FROM Bans") != 0;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>Removes a ban if it has expired.</summary>
		/// <param name="ban">The candidate ban to check.</param>
		/// <returns>If the ban has been removed.</returns>
		public bool RemoveBanIfExpired(Ban ban)
		{
			if (!string.IsNullOrWhiteSpace(ban.Expiration) && (ban.ExpirationDateTime != null) && (DateTime.UtcNow >= ban.ExpirationDateTime))
			{
				RemoveBan(ban.IP, false, false, false);
				return true;
			}

			return false;
		}
	}

	/// <summary>
	/// Enum containing sort options for ban retrieval
	/// </summary>
	public enum BanSortMethod
	{
		/// <summary>
		/// Bans will be sorted on expiration date, from soonest to latest
		/// </summary>
		ExpirationSoonestToLatest,
		/// <summary>
		/// Bans will be sorted on expiration date, from latest to soonest
		/// </summary>
		ExpirationLatestToSoonest,
		/// <summary>
		/// Bans will be sorted by the date they were added, from newest to oldest
		/// </summary>
		AddedNewestToOldest,
		/// <summary>
		/// Bans will be sorted by the date they were added, from oldest to newest
		/// </summary>
		AddedOldestToNewest
	}

	/// <summary>
	/// An <see cref="IComparer{Ban}"/> used for sorting an enumerable of bans
	/// </summary>
	public class BanComparer : IComparer<Ban>
	{
		private BanSortMethod _method;

		/// <summary>
		/// Generates a new <see cref="BanComparer"/> using the given <see cref="BanSortMethod"/>
		/// </summary>
		/// <param name="method"></param>
		public BanComparer(BanSortMethod method)
		{
			_method = method;
		}

		private int CompareDateTimes(DateTime? x, DateTime? y)
		{
			if (x == null)
			{
				if (y == null)
				{
					//If both bans have no BanDateTime they're considered equal
					return 0;
				}
				//If we're sorting by a newest to oldest method, a null value will come after the valid value.
				return _method == BanSortMethod.AddedNewestToOldest || _method == BanSortMethod.ExpirationSoonestToLatest ? 1 : -1;
			}

			if (y == null)
			{
				return _method == BanSortMethod.AddedNewestToOldest || _method == BanSortMethod.ExpirationSoonestToLatest ? -1 : 1;
			}

			//Newest to oldest sorting uses x compared to y. Oldest to newest uses y compared to x
			return _method == BanSortMethod.AddedNewestToOldest || _method == BanSortMethod.ExpirationSoonestToLatest ? x.Value.CompareTo(y.Value)
				: y.Value.CompareTo(x.Value);
		}

		/// <summary>
		/// Compares two ban objects
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>1 if x is less than y, 0 if x is equal to y, -1 if x is greater than y</returns>
		public int Compare(Ban x, Ban y)
		{
			if (x == null)
			{
				if (y == null)
				{
					return 0;
				}

				//If Ban y is null and Ban x is not, and we're sorting from newest to oldest, x goes before y. Else y goes before x
				return _method == BanSortMethod.AddedNewestToOldest || _method == BanSortMethod.ExpirationSoonestToLatest ? -1 : 1;
			}

			if (x == null)
			{
				if (y == null)
				{
					return 0;
				}

				//If Ban y is null and Ban x is not, and we're sorting from newest to oldest, x goes before y. Else y goes before x
				return _method == BanSortMethod.AddedNewestToOldest || _method == BanSortMethod.ExpirationSoonestToLatest ? -1 : 1;
			}

			switch (_method)
			{
				case BanSortMethod.AddedNewestToOldest:
				case BanSortMethod.AddedOldestToNewest:
					return CompareDateTimes(x.BanDateTime, y.BanDateTime);

				case BanSortMethod.ExpirationSoonestToLatest:
				case BanSortMethod.ExpirationLatestToSoonest:
					return CompareDateTimes(x.ExpirationDateTime, y.ExpirationDateTime);

				default:
					return 0;
			}
		}
	}

	/// <summary>
	/// Model class that represents a ban entry in the TShock database.
	/// </summary>
	public class Ban
	{
		/// <summary>
		/// Gets or sets the IP address of the ban entry.
		/// </summary>
		/// <value>The IP Address</value>
		public string IP { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Client UUID of the ban
		/// </summary>
		/// <value>The UUID</value>
		public string UUID { get; set; }

		/// <summary>
		/// Gets or sets the account name of the ban
		/// </summary>
		/// <value>The account name</value>
		public String AccountName { get; set; }

		/// <summary>
		/// Gets or sets the ban reason.
		/// </summary>
		/// <value>The ban reason.</value>
		public string Reason { get; set; }

		/// <summary>
		/// Gets or sets the name of the user who added this ban entry.
		/// </summary>
		/// <value>The banning user.</value>
		public string BanningUser { get; set; }

		/// <summary>
		/// Gets or sets the UTC date of when the ban is to take effect
		/// </summary>
		/// <value>The date, which must be in UTC</value>
		public string Date { get; set; }

		/// <summary>
		/// Gets the <see cref="System.DateTime"/> object representation of the <see cref="Date"/> string.
		/// </summary>
		public DateTime? BanDateTime { get; }

		/// <summary>
		/// Gets or sets the expiration date, in which the ban shall be lifted
		/// </summary>
		/// <value>The expiration.</value>
		public string Expiration { get; set; }

		/// <summary>
		/// Gets the <see cref="System.DateTime"/> object representation of the <see cref="Expiration"/> string.
		/// </summary>
		public DateTime? ExpirationDateTime { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.Ban"/> class.
		/// </summary>
		/// <param name="ip">Ip.</param>
		/// <param name="name">Name.</param>
		/// <param name="uuid">UUID.</param>
		/// <param name="reason">Reason.</param>
		/// <param name="banner">Banner.</param>
		/// <param name="date">UTC ban date.</param>
		/// <param name="exp">Expiration time</param>
		public Ban(string ip, string name, string uuid, string accountName, string reason, string banner, string date, string exp)
		{
			IP = ip;
			Name = name;
			UUID = uuid;
			AccountName = accountName;
			Reason = reason;
			BanningUser = banner;
			Date = date;
			Expiration = exp;

			DateTime d;
			DateTime e;
			if (DateTime.TryParse(Date, out d))
			{
				BanDateTime = d;
			}
			if (DateTime.TryParse(Expiration, out e))
			{
				ExpirationDateTime = e;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.Ban"/> class.
		/// </summary>
		public Ban()
		{
			IP = string.Empty;
			Name = string.Empty;
			UUID = string.Empty;
			AccountName = string.Empty;
			Reason = string.Empty;
			BanningUser = string.Empty;
			Date = string.Empty;
			Expiration = string.Empty;
		}
	}
}
