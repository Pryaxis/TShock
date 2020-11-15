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
		/// Event invoked when a ban check occurs
		/// </summary>
		public static event EventHandler<BanEventArgs> OnBanCheck;
		/// <summary>
		/// Event invoked when a ban is added
		/// </summary>
		public static event EventHandler<BanEventArgs> OnBanAdded;

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.BanManager"/> class.
		/// </summary>
		/// <param name="db">A valid connection to the TShock database</param>
		public BanManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("PlayerBans",
									new SqlColumn("Identifier", MySqlDbType.Text) { Primary = true, Unique = true },
									new SqlColumn("Reason", MySqlDbType.Text),
									new SqlColumn("BanningUser", MySqlDbType.Text),
									new SqlColumn("Date", MySqlDbType.Text),
									new SqlColumn("Expiration", MySqlDbType.Text)
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

			OnBanCheck += CheckBanValid;
		}

		/// <summary>
		/// Converts bans from the old ban system to the new.
		/// </summary>
		public void ConvertBans()
		{
			using (var reader = database.QueryReader("SELECT * FROM Bans"))
			{
				while (reader.Read())
				{
					var ip = reader.Get<string>("IP");
					var uuid = reader.Get<string>("UUID");
					var account = reader.Get<string>("AccountName");
					var reason = reader.Get<string>("Reason");
					var banningUser = reader.Get<string>("BanningUser");
					var date = reader.Get<string>("Date");
					var expiration = reader.Get<string>("Expiration");

					if (!string.IsNullOrWhiteSpace(ip))
					{
						InsertBan($"{Ban.Identifiers.IP}{ip}", reason, banningUser, date, expiration);
					}

					if (!string.IsNullOrWhiteSpace(account))
					{
						InsertBan($"{Ban.Identifiers.Account}{account}", reason, banningUser, date, expiration);
					}

					if (!string.IsNullOrWhiteSpace(uuid))
					{
						InsertBan($"{Ban.Identifiers.UUID}{uuid}", reason, banningUser, date, expiration);
					}
				}
			}
		}

		/// <summary>
		/// Determines whether or not a ban is valid
		/// </summary>
		/// <param name="ban"></param>
		/// <returns></returns>
		public bool IsValidBan(Ban ban)
		{
			BanEventArgs args = new BanEventArgs { Ban = ban };
			OnBanCheck?.Invoke(this, args);

			return args.Valid;
		}

		internal void CheckBanValid(object sender, BanEventArgs args)
		{
			//We consider a ban to be valid if the start time is before now and the end time is after now
			args.Valid = args.Ban.BanDateTime < DateTime.UtcNow && args.Ban.ExpirationDateTime > DateTime.UtcNow;
		}

		/// <summary>
		/// Adds a new ban for the given identifier. If the addition succeeds, returns a ban object with the ban details. Else returns null
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="reason"></param>
		/// <param name="banningUser"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <returns></returns>
		public Ban InsertBan(string identifier, string reason, string banningUser, DateTime fromDate, DateTime toDate)
			=> InsertBan(identifier, reason, banningUser, fromDate.ToString("s"), toDate.ToString("s"));

		/// <summary>
		/// Adds a new ban for the given identifier. If the addition succeeds, returns a ban object with the ban details. Else returns null
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="reason"></param>
		/// <param name="banningUser"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <returns></returns>
		public Ban InsertBan(string identifier, string reason, string banningUser, string fromDate, string toDate)
		{
			Ban b = new Ban(identifier, reason, banningUser, fromDate, toDate);

			BanEventArgs args = new BanEventArgs { Ban = b };

			OnBanAdded?.Invoke(this, args);

			if (!args.Valid)
			{
				return null;
			}

			int rowsModified = database.Query("INSERT OR IGNORE INTO PlayerBans (Identifier, Reason, BanningUser, Date, Expiration) VALUES (@0, @1, @2, @3, @4);", identifier, reason, banningUser, fromDate, toDate);
			//Return the ban if the query actually inserted the row. If the given identifier already exists, the INSERT is ignored and 0 rows are modified.
			return rowsModified != 0 ? b : null;
		}

		/// <summary>
		/// Attempts to remove a ban. Returns true if the ban was removed or expired. False if the ban could not be removed or expired
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="fullDelete">If true, deletes the ban from the database. If false, marks the expiration time as now, rendering the ban expired. Defaults to false</param>
		/// <returns></returns>
		public bool RemoveBan(string identifier, bool fullDelete = false)
		{
			int rowsModified;
			if (fullDelete)
			{
				rowsModified = database.Query("DELETE FROM PlayerBans WHERE Identifier=@0", identifier);
			}
			else
			{
				rowsModified = database.Query("UPDATE PlayerBans SET Expiration=@0 WHERE Identifier=@1", DateTime.UtcNow.ToString("s"), identifier);
			}

			return rowsModified > 0;
		}

		/// <summary>
		/// Retrieves a ban for a given identifier, or null if no matches are found
		/// </summary>
		/// <param name="identifier"></param>
		/// <returns></returns>
		public Ban GetBanByIdentifier(string identifier)
		{
			using (var reader = database.QueryReader("SELECT * FROM PlayerBans WHERE Identifier=@0", identifier))
			{
				if (reader.Read())
				{
					var id = reader.Get<string>("Identifier");
					var reason = reader.Get<string>("Reason");
					var banningUser = reader.Get<string>("BanningUser");
					var date = reader.Get<string>("Date");
					var expiration = reader.Get<string>("Expiration");

					return new Ban(id, reason, banningUser, date, expiration);
				}
			}

			return null;
		}

		/// <summary>
		/// Retrieves an enumerable of bans for a given set of identifiers
		/// </summary>
		/// <param name="identifiers"></param>
		/// <returns></returns>
		public IEnumerable<Ban> GetBansByIdentifiers(params string[] identifiers)
		{
			//Generate a sequence of '@0, @1, @2, ... etc'
			var parameters = string.Join(", ", Enumerable.Range(0, identifiers.Count()).Select(p => $"@{p}"));

			using (var reader = database.QueryReader($"SELECT * FROM PlayerBans WHERE Identifier IN ({parameters})", identifiers))
			{
				while (reader.Read())
				{
					var id = reader.Get<string>("Identifier");
					var reason = reader.Get<string>("Reason");
					var banningUser = reader.Get<string>("BanningUser");
					var date = reader.Get<string>("Date");
					var expiration = reader.Get<string>("Expiration");

					yield return new Ban(id, reason, banningUser, date, expiration);
				}
			}
		}

		/// <summary>
		/// Gets a list of bans sorted by their addition date from newest to oldest
		/// </summary>
		public List<Ban> GetAllBans() => GetAllBansSorted(BanSortMethod.AddedNewestToOldest).ToList();

		/// <summary>
		/// Retrieves an enumerable of <see cref="Ban"/> objects, sorted using the provided sort method
		/// </summary>
		/// <param name="sortMethod"></param>
		/// <returns></returns>
		public IEnumerable<Ban> GetAllBansSorted(BanSortMethod sortMethod)
		{
			List<Ban> banlist = new List<Ban>();
			try
			{
				var orderBy = SortToOrderByMap[sortMethod];
				using (var reader = database.QueryReader($"SELECT * FROM PlayerBans ORDER BY {orderBy}"))
				{
					while (reader.Read())
					{
						var identifier = reader.Get<string>("Identifier");
						var reason = reader.Get<string>("Reason");
						var banningUser = reader.Get<string>("BanningUser");
						var date = reader.Get<string>("Date");
						var expiration = reader.Get<string>("Expiration");

						var ban = new Ban(identifier, reason, banningUser, date, expiration);
						banlist.Add(ban);
					}
				}
				return banlist;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				Console.WriteLine(ex.StackTrace);
			}
			return null;
		}

		/// <summary>
		/// Removes all bans from the database
		/// </summary>
		/// <returns><c>true</c>, if bans were cleared, <c>false</c> otherwise.</returns>
		public bool ClearBans()
		{
			try
			{
				return database.Query("DELETE FROM PlayerBans") != 0;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		internal Dictionary<BanSortMethod, string> SortToOrderByMap = new Dictionary<BanSortMethod, string>
		{
			{ BanSortMethod.AddedNewestToOldest, "Date DESC" },
			{ BanSortMethod.AddedOldestToNewest, "Date ASC" },
			{ BanSortMethod.ExpirationSoonestToLatest, "Expiration ASC" },
			{ BanSortMethod.ExpirationLatestToSoonest, "Expiration DESC" }
		};
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
	/// Event args used when a ban check is invoked, or a new ban is added
	/// </summary>
	public class BanEventArgs : EventArgs
	{
		/// <summary>
		/// The ban being checked or added
		/// </summary>
		public Ban Ban { get; set; }
		/// <summary>
		/// Whether or not the operation is valid
		/// </summary>
		public bool Valid { get; set; } = true;
	}

	/// <summary>
	/// Model class that represents a ban entry in the TShock database.
	/// </summary>
	public class Ban
	{
		/// <summary>
		/// Contains constants for different identifier types known to TShock
		/// </summary>
		public class Identifiers
		{
			/// <summary>
			/// IP identifier prefix constant
			/// </summary>
			public const string IP = "ip:";
			/// <summary>
			/// UUID identifier prefix constant
			/// </summary>
			public const string UUID = "uuid:";
			/// <summary>
			/// Player name identifier prefix constant
			/// </summary>
			public const string Name = "name:";
			/// <summary>
			/// User account identifier prefix constant
			/// </summary>
			public const string Account = "acc:";
		}

		/// <summary>
		/// An identifiable piece of information to ban
		/// </summary>
		public string Identifier { get; set; }

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
		/// DateTime from which the ban will take effect
		/// </summary>
		public DateTime BanDateTime { get; }

		/// <summary>
		/// DateTime at which the ban will end
		/// </summary>
		public DateTime ExpirationDateTime { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.Ban"/> class.
		/// </summary>
		/// <param name="identifier">Identifier to apply the ban to</param>
		/// <param name="reason">Reason for the ban</param>
		/// <param name="banningUser">Account name that executed the ban</param>
		/// <param name="start">Ban start datetime</param>
		/// <param name="end">Ban end datetime</param>
		public Ban(string identifier, string reason, string banningUser, string start, string end)
		{
			Identifier = identifier;
			Reason = reason;
			BanningUser = banningUser;

			if (DateTime.TryParse(start, out DateTime d))
			{
				BanDateTime = d;
			}
			else
			{
				BanDateTime = DateTime.UtcNow;
			}

			if (DateTime.TryParse(end, out DateTime e))
			{
				ExpirationDateTime = e;
			}
			else
			{
				ExpirationDateTime = DateTime.MaxValue;
			}
		}
	}
}
