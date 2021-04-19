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
using System.Collections.ObjectModel;

namespace TShockAPI.DB
{
	/// <summary>
	/// Class that manages bans.
	/// </summary>
	public class BanManager
	{
		private IDbConnection database;

		private Dictionary<int, Ban> _bans;

		/// <summary>
		/// Readonly dictionary of Bans, keyed on ban ticket number.
		/// </summary>
		public ReadOnlyDictionary<int, Ban> Bans => new ReadOnlyDictionary<int, Ban>(_bans);

		/// <summary>
		/// Event invoked when a ban is checked for validity
		/// </summary>
		public static event EventHandler<BanEventArgs> OnBanValidate;
		/// <summary>
		/// Event invoked before a ban is added
		/// </summary>
		public static event EventHandler<BanPreAddEventArgs> OnBanPreAdd;
		/// <summary>
		/// Event invoked after a ban is added
		/// </summary>
		public static event EventHandler<BanEventArgs> OnBanPostAdd;

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.BanManager"/> class.
		/// </summary>
		/// <param name="db">A valid connection to the TShock database</param>
		public BanManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("PlayerBans",
									new SqlColumn("TicketNumber", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
									new SqlColumn("Identifier", MySqlDbType.Text),
									new SqlColumn("Reason", MySqlDbType.Text),
									new SqlColumn("BanningUser", MySqlDbType.Text),
									new SqlColumn("Date", MySqlDbType.Int64),
									new SqlColumn("Expiration", MySqlDbType.Int64)
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

			EnsureBansCollection();
			TryConvertBans();

			OnBanValidate += BanValidateCheck;
			OnBanPreAdd += BanAddedCheck;
		}

		/// <summary>
		/// Ensures the <see cref="_bans"/> collection is ready to use.
		/// </summary>
		private void EnsureBansCollection()
		{
			if (_bans == null)
			{
				_bans = RetrieveAllBans().ToDictionary(b => b.TicketNumber);
			}
		}

		/// <summary>
		/// Converts bans from the old ban system to the new.
		/// </summary>
		public void TryConvertBans()
		{
			int res;
			if (database.GetSqlType() == SqlType.Mysql)
			{
				res = database.QueryScalar<int>("SELECT COUNT(table_name) FROM information_schema.tables WHERE table_schema = @0 and table_name = 'Bans'", TShock.Config.Settings.MySqlDbName);
			}
			else
			{
				res = database.QueryScalar<int>("SELECT COUNT(name) FROM sqlite_master WHERE type='table' AND name = 'Bans'");
			}

			if (res != 0)
			{
				var bans = new List<BanPreAddEventArgs>();
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

						if (!DateTime.TryParse(date, out DateTime start))
						{
							start = DateTime.UtcNow;
						}

						if (!DateTime.TryParse(expiration, out DateTime end))
						{
							end = DateTime.MaxValue;
						}

						if (!string.IsNullOrWhiteSpace(ip))
						{
							bans.Add(new BanPreAddEventArgs
							{
								Identifier = $"{Identifier.IP}{ip}",
								Reason = reason,
								BanningUser = banningUser,
								BanDateTime = start,
								ExpirationDateTime = end
							});
						}

						if (!string.IsNullOrWhiteSpace(account))
						{
							bans.Add(new BanPreAddEventArgs
							{
								Identifier = $"{Identifier.Account}{account}",
								Reason = reason,
								BanningUser = banningUser,
								BanDateTime = start,
								ExpirationDateTime = end
							});
						}

						if (!string.IsNullOrWhiteSpace(uuid))
						{
							bans.Add(new BanPreAddEventArgs
							{
								Identifier = $"{Identifier.UUID}{uuid}",
								Reason = reason,
								BanningUser = banningUser,
								BanDateTime = start,
								ExpirationDateTime = end
							});
						}
					}
				}

				foreach (var ban in bans)
					InsertBan(ban);

				database.Query("DROP TABLE Bans");
			}
		}

		internal bool CheckBan(TSPlayer player)
		{
			List<string> identifiers = new List<string>
			{
				$"{Identifier.UUID}{player.UUID}",
				$"{Identifier.Name}{player.Name}",
				$"{Identifier.IP}{player.IP}"
			};

			if (player.Account != null)
			{
				identifiers.Add($"{Identifier.Account}{player.Account.Name}");
			}

			Ban ban = TShock.Bans.Bans.FirstOrDefault(b => identifiers.Contains(b.Value.Identifier) && TShock.Bans.IsValidBan(b.Value, player)).Value;

			if (ban != null)
			{
				if (ban.ExpirationDateTime == DateTime.MaxValue)
				{
					player.Disconnect($"#{ban.TicketNumber} - You are banned: {ban.Reason}");
					return true;
				}

				TimeSpan ts = ban.ExpirationDateTime - DateTime.UtcNow;
				player.Disconnect($"#{ban.TicketNumber} - You are banned: {ban.Reason} ({ban.GetPrettyExpirationString()} remaining)");
				return true;
			}

			return false;
		}

		/// <summary>
		/// Determines whether or not a ban is valid
		/// </summary>
		/// <param name="ban"></param>
		/// <param name="player"></param>
		/// <returns></returns>
		public bool IsValidBan(Ban ban, TSPlayer player)
		{
			BanEventArgs args = new BanEventArgs
			{
				Ban = ban,
				Player = player
			};

			OnBanValidate?.Invoke(this, args);

			return args.Valid;
		}

		internal void BanValidateCheck(object sender, BanEventArgs args)
		{
			//Only perform validation if the event has not been cancelled before we got here
			if (args.Valid)
			{
				//We consider a ban to be valid if the start time is before now and the end time is after now
				args.Valid = (DateTime.UtcNow > args.Ban.BanDateTime && DateTime.UtcNow < args.Ban.ExpirationDateTime);
			}
		}

		internal void BanAddedCheck(object sender, BanPreAddEventArgs args)
		{
			//Only perform validation if the event has not been cancelled before we got here
			if (args.Valid)
			{
				//We consider a ban valid to add if no other *current* bans exist for the identifier provided.
				//E.g., if a previous ban has expired, a new ban is valid.
				//However, if a previous ban on the provided identifier is still in effect, a new ban is not valid
				args.Valid = !Bans.Any(b => b.Value.Identifier == args.Identifier && b.Value.ExpirationDateTime > DateTime.UtcNow);
				args.Message = args.Valid ? null : "a current ban for this identifier already exists.";
			}
		}

		/// <summary>
		/// Adds a new ban for the given identifier. Returns a Ban object if the ban was added, else null
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="reason"></param>
		/// <param name="banningUser"></param>
		/// <param name="fromDate"></param>
		/// <param name="toDate"></param>
		/// <returns></returns>
		public AddBanResult InsertBan(string identifier, string reason, string banningUser, DateTime fromDate, DateTime toDate)
		{
			BanPreAddEventArgs args = new BanPreAddEventArgs
			{
				Identifier = identifier,
				Reason = reason,
				BanningUser = banningUser,
				BanDateTime = fromDate,
				ExpirationDateTime = toDate
			};
			return InsertBan(args);
		}

		/// <summary>
		/// Adds a new ban for the given data. Returns a Ban object if the ban was added, else null
		/// </summary>
		/// <param name="args">A predefined instance of <see cref="BanPreAddEventArgs"/></param>
		/// <returns></returns>
		public AddBanResult InsertBan(BanPreAddEventArgs args)
		{
			OnBanPreAdd?.Invoke(this, args);

			if (!args.Valid)
			{
				string message = $"Ban was not valid: {(args.Message ?? "no further information provided.")}";
				return new AddBanResult { Message = message };
			}

			string query = "INSERT INTO PlayerBans (Identifier, Reason, BanningUser, Date, Expiration) VALUES (@0, @1, @2, @3, @4);";

			if (database.GetSqlType() == SqlType.Mysql)
			{
				query += "SELECT LAST_INSERT_ID();";
			}
			else
			{
				query += "SELECT CAST(last_insert_rowid() as INT);";
			}

			int ticketId = database.QueryScalar<int>(query, args.Identifier, args.Reason, args.BanningUser, args.BanDateTime.Ticks, args.ExpirationDateTime.Ticks);

			if (ticketId == 0)
			{
				return new AddBanResult { Message = "Database insert failed." };
			}

			Ban b = new Ban(ticketId, args.Identifier, args.Reason, args.BanningUser, args.BanDateTime, args.ExpirationDateTime);
			_bans.Add(ticketId, b);

			OnBanPostAdd?.Invoke(this, new BanEventArgs { Ban = b });

			return new AddBanResult { Ban = b };
		}

		/// <summary>
		/// Attempts to remove a ban. Returns true if the ban was removed or expired. False if the ban could not be removed or expired
		/// </summary>
		/// <param name="ticketNumber">The ticket number of the ban to change</param>
		/// <param name="fullDelete">If true, deletes the ban from the database. If false, marks the expiration time as now, rendering the ban expired. Defaults to false</param>
		/// <returns></returns>
		public bool RemoveBan(int ticketNumber, bool fullDelete = false)
		{
			int rowsModified;
			if (fullDelete)
			{
				rowsModified = database.Query("DELETE FROM PlayerBans WHERE TicketNumber=@0", ticketNumber);
				_bans.Remove(ticketNumber);
			}
			else
			{
				rowsModified = database.Query("UPDATE PlayerBans SET Expiration=@0 WHERE TicketNumber=@1", DateTime.UtcNow.Ticks, ticketNumber);
				_bans[ticketNumber].ExpirationDateTime = DateTime.UtcNow;
			}

			return rowsModified > 0;
		}

		/// <summary>
		/// Retrieves a single ban from a ban's ticket number
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Ban GetBanById(int id)
		{
			if (Bans.ContainsKey(id))
			{
				return Bans[id];
			}

			using (var reader = database.QueryReader("SELECT * FROM PlayerBans WHERE TicketNumber=@0", id))
			{
				if (reader.Read())
				{
					var ticketNumber = reader.Get<int>("TicketNumber");
					var identifier = reader.Get<string>("Identifier");
					var reason = reader.Get<string>("Reason");
					var banningUser = reader.Get<string>("BanningUser");
					var date = reader.Get<long>("Date");
					var expiration = reader.Get<long>("Expiration");

					return new Ban(ticketNumber, identifier, reason, banningUser, date, expiration);
				}
			}

			return null;
		}

		/// <summary>
		/// Retrieves an enumerable of all bans for a given identifier
		/// </summary>
		/// <param name="identifier">Identifier to search with</param>
		/// <param name="currentOnly">Whether or not to exclude expired bans</param>
		/// <returns></returns>
		public IEnumerable<Ban> RetrieveBansByIdentifier(string identifier, bool currentOnly = true)
		{
			string query = "SELECT * FROM PlayerBans WHERE Identifier=@0";
			if (currentOnly)
			{
				query += $" AND Expiration > {DateTime.UtcNow.Ticks}";
			}

			using (var reader = database.QueryReader(query, identifier))
			{
				while (reader.Read())
				{
					var ticketNumber = reader.Get<int>("TicketNumber");
					var ident = reader.Get<string>("Identifier");
					var reason = reader.Get<string>("Reason");
					var banningUser = reader.Get<string>("BanningUser");
					var date = reader.Get<long>("Date");
					var expiration = reader.Get<long>("Expiration");

					yield return new Ban(ticketNumber, ident, reason, banningUser, date, expiration);
				}
			}
		}

		/// <summary>
		/// Retrieves an enumerable of bans for a given set of identifiers
		/// </summary>
		/// <param name="currentOnly">Whether or not to exclude expired bans</param>
		/// <param name="identifiers"></param>
		/// <returns></returns>
		public IEnumerable<Ban> GetBansByIdentifiers(bool currentOnly = true, params string[] identifiers)
		{
			//Generate a sequence of '@0, @1, @2, ... etc'
			var parameters = string.Join(", ", Enumerable.Range(0, identifiers.Count()).Select(p => $"@{p}"));

			string query = $"SELECT * FROM PlayerBans WHERE Identifier IN ({parameters})";
			if (currentOnly)
			{
				query += $" AND Expiration > {DateTime.UtcNow.Ticks}";
			}

			using (var reader = database.QueryReader(query, identifiers))
			{
				while (reader.Read())
				{
					var ticketNumber = reader.Get<int>("TicketNumber");
					var identifier = reader.Get<string>("Identifier");
					var reason = reader.Get<string>("Reason");
					var banningUser = reader.Get<string>("BanningUser");
					var date = reader.Get<long>("Date");
					var expiration = reader.Get<long>("Expiration");

					yield return new Ban(ticketNumber, identifier, reason, banningUser, date, expiration);
				}
			}
		}

		/// <summary>
		/// Retrieves a list of bans from the database, sorted by their addition date from newest to oldest
		/// </summary>
		public IEnumerable<Ban> RetrieveAllBans() => RetrieveAllBansSorted(BanSortMethod.AddedNewestToOldest);

		/// <summary>
		/// Retrieves an enumerable of <see cref="Ban"/>s from the database, sorted using the provided sort method
		/// </summary>
		/// <param name="sortMethod"></param>
		/// <returns></returns>
		public IEnumerable<Ban> RetrieveAllBansSorted(BanSortMethod sortMethod)
		{
			List<Ban> banlist = new List<Ban>();
			try
			{
				var orderBy = SortToOrderByMap[sortMethod];
				using (var reader = database.QueryReader($"SELECT * FROM PlayerBans ORDER BY {orderBy}"))
				{
					while (reader.Read())
					{
						var ticketNumber = reader.Get<int>("TicketNumber");
						var identifier = reader.Get<string>("Identifier");
						var reason = reader.Get<string>("Reason");
						var banningUser = reader.Get<string>("BanningUser");
						var date = reader.Get<long>("Date");
						var expiration = reader.Get<long>("Expiration");

						var ban = new Ban(ticketNumber, identifier, reason, banningUser, date, expiration);
						banlist.Add(ban);
					}
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				Console.WriteLine(ex.StackTrace);
			}

			return banlist;
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
		AddedOldestToNewest,
		/// <summary>
		/// Bans will be sorted by their ticket number
		/// </summary>
		TicketNumber
	}

	/// <summary>
	/// Result of an attempt to add a ban
	/// </summary>
	public class AddBanResult
	{
		/// <summary>
		/// Message generated from the attempt
		/// </summary>
		public string Message { get; set; }
		/// <summary>
		/// Ban object generated from the attempt, or null if the attempt failed
		/// </summary>
		public Ban Ban { get; set; }
	}

	/// <summary>
	/// Event args used for completed bans
	/// </summary>
	public class BanEventArgs : EventArgs
	{
		/// <summary>
		/// Complete ban object
		/// </summary>
		public Ban Ban { get; set; }

		/// <summary>
		/// Player ban is being applied to
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// Whether or not the operation should be considered to be valid
		/// </summary>
		public bool Valid { get; set; } = true;
	}

	/// <summary>
	/// Event args used for ban data prior to a ban being formalized
	/// </summary>
	public class BanPreAddEventArgs : EventArgs
	{
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
		public DateTime BanDateTime { get; set; }

		/// <summary>
		/// DateTime at which the ban will end
		/// </summary>
		public DateTime ExpirationDateTime { get; set; }

		/// <summary>
		/// Whether or not the operation should be considered to be valid
		/// </summary>
		public bool Valid { get; set; } = true;

		/// <summary>
		/// Optional message to explain why the event was invalidated, if it was
		/// </summary>
		public string Message { get; set; }
	}

	/// <summary>
	/// Describes an identifier used by the ban system
	/// </summary>
	public class Identifier
	{
		/// <summary>
		/// Identifiers currently registered
		/// </summary>
		public static List<Identifier> Available = new List<Identifier>();

		/// <summary>
		/// The prefix of the identifier. E.g, 'ip:'
		/// </summary>
		public string Prefix { get; }
		/// <summary>
		/// Short description of the identifier and its basic usage
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// IP identifier
		/// </summary>
		public static Identifier IP = Register("ip:", $"An identifier for an IP Address in octet format. Eg., '{"127.0.0.1".Color(Utils.RedHighlight)}'.");
		/// <summary>
		/// UUID identifier
		/// </summary>
		public static Identifier UUID = Register("uuid:", "An identifier for a UUID.");
		/// <summary>
		/// Player name identifier
		/// </summary>
		public static Identifier Name = Register("name:", "An identifier for a character name.");
		/// <summary>
		/// User account identifier
		/// </summary>
		public static Identifier Account = Register("acc:", "An identifier for a TShock User Account name.");

		private Identifier(string prefix, string description)
		{
			Prefix = prefix;
			Description = description;
		}

		/// <summary>
		/// Returns the identifier's prefix
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Prefix;
		}

		/// <summary>
		/// Registers a new identifier with the given prefix and description
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="description"></param>
		public static Identifier Register(string prefix, string description)
		{
			var ident = new Identifier(prefix, description);
			Available.Add(ident);

			return ident;
		}
	}

	/// <summary>
	/// Model class that represents a ban entry in the TShock database.
	/// </summary>
	public class Ban
	{
		/// <summary>
		/// A unique ID assigned to this ban
		/// </summary>
		public int TicketNumber { get; set; }

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
		public DateTime BanDateTime { get; set; }

		/// <summary>
		/// DateTime at which the ban will end
		/// </summary>
		public DateTime ExpirationDateTime { get; set; }

		/// <summary>
		/// Returns a string in the format dd:mm:hh:ss indicating the time until the ban expires.
		/// If the ban is not set to expire (ExpirationDateTime == DateTime.MaxValue), returns the string 'Never'
		/// </summary>
		/// <returns></returns>
		public string GetPrettyExpirationString()
		{
			if (ExpirationDateTime == DateTime.MaxValue)
			{
				return "Never";
			}

			TimeSpan ts = (ExpirationDateTime - DateTime.UtcNow).Duration(); // Use duration to avoid pesky negatives for expired bans
			return $"{ts.Days:00}:{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
		}

		/// <summary>
		/// Returns a string in the format dd:mm:hh:ss indicating the time elapsed since the ban was added.
		/// </summary>
		/// <returns></returns>
		public string GetPrettyTimeSinceBanString()
		{
			TimeSpan ts = (DateTime.UtcNow - BanDateTime).Duration();
			return $"{ts.Days:00}:{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.Ban"/> class.
		/// </summary>
		/// <param name="ticketNumber">Unique ID assigned to the ban</param>
		/// <param name="identifier">Identifier to apply the ban to</param>
		/// <param name="reason">Reason for the ban</param>
		/// <param name="banningUser">Account name that executed the ban</param>
		/// <param name="start">System ticks at which the ban began</param>
		/// <param name="end">System ticks at which the ban will end</param>
		public Ban(int ticketNumber, string identifier, string reason, string banningUser, long start, long end)
			: this(ticketNumber, identifier, reason, banningUser, new DateTime(start), new DateTime(end))
		{
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.Ban"/> class.
		/// </summary>
		/// <param name="ticketNumber">Unique ID assigned to the ban</param>
		/// <param name="identifier">Identifier to apply the ban to</param>
		/// <param name="reason">Reason for the ban</param>
		/// <param name="banningUser">Account name that executed the ban</param>
		/// <param name="start">DateTime at which the ban will start</param>
		/// <param name="end">DateTime at which the ban will end</param>
		public Ban(int ticketNumber, string identifier, string reason, string banningUser, DateTime start, DateTime end)
		{
			TicketNumber = ticketNumber;
			Identifier = identifier;
			Reason = reason;
			BanningUser = banningUser;
			BanDateTime = start;
			ExpirationDateTime = end;
		}
	}
}
