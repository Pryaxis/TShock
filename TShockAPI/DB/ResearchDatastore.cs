using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace TShockAPI.DB
{
	/// <summary>
	/// This class is used as the data interface for Journey mode research.
	/// This information is maintained such that SSC characters will be properly set up with
	/// the world's current research.
	/// </summary>
	public class ResearchDatastore
	{
		private IDbConnection database;

		/// <summary>
		/// In-memory cache of what items have been sacrificed.
		/// The first call to GetSacrificedItems will load this with data from the database.
		/// </summary>
		private Dictionary<int, int> _itemsSacrificed;

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.ResearchDatastore"/> class.
		/// </summary>
		/// <param name="db">A valid connection to the TShock database</param>
		public ResearchDatastore(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Research",
									new SqlColumn("WorldId", MySqlDbType.Int32),
									new SqlColumn("PlayerId", MySqlDbType.Int32),
									new SqlColumn("ItemId", MySqlDbType.Int32),
									new SqlColumn("AmountSacrificed", MySqlDbType.Int32),
									new SqlColumn("TimeSacrificed", MySqlDbType.DateTime)
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
				Console.WriteLine("Possible problem with your database - is Sqlite3.dll present?");
				throw new Exception("Could not find a database library (probably Sqlite3.dll)");
			}
		}

		/// <summary>
		/// This call will return the memory-cached list of items sacrificed.
		/// If the cache is not initialized, it will be initialized from the database.
		/// </summary>
		/// <returns></returns>
		public Dictionary<int, int> GetSacrificedItems()
		{
			if (_itemsSacrificed == null)
			{
				_itemsSacrificed = ReadFromDatabase();
			}

			return _itemsSacrificed;
		}

		/// <summary>
		/// This function will return a Dictionary&lt;ItemId, AmountSacrificed&gt; representing
		/// what the progress of research on items is for this world.
		/// </summary>
		/// <returns>A dictionary of ItemID keys and Amount Sacrificed values.</returns>
		private Dictionary<int, int> ReadFromDatabase()
		{
			Dictionary<int, int> sacrificedItems = new Dictionary<int, int>();

			var sql = @"select itemId, sum(AmountSacrificed) totalSacrificed
  from Research
	where WorldId = @0
      group by itemId";

			try { 
				using (var reader = database.QueryReader(sql, Main.worldID))
				{
					while (reader.Read())
					{
						var itemId = reader.Get<Int32>("itemId");
						var amount = reader.Get<Int32>("totalSacrificed");
						sacrificedItems[itemId] = amount;
					}
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return sacrificedItems;
		}

		/// <summary>
		/// This method will sacrifice an amount of an item for research.
		/// </summary>
		/// <param name="itemId">The net ItemId that is being researched.</param>
		/// <param name="amount">The amount of items being sacrificed.</param>
		/// <param name="player">The player who sacrificed the item for research.</param>
		/// <returns>The cumulative total sacrifices for this item.</returns>
		public int SacrificeItem(int itemId, int amount, TSPlayer player)
		{
			var itemsSacrificed = GetSacrificedItems();
			if (!(itemsSacrificed.ContainsKey(itemId)))
				itemsSacrificed[itemId] = 0;

			var sql = @"insert into Research (WorldId, PlayerId, ItemId, AmountSacrificed, TimeSacrificed) values (@0, @1, @2, @3, @4)";

			var result = 0;
			try
			{
				result = database.Query(sql, Main.worldID, player.Account.ID, itemId, amount, DateTime.Now);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}

			if (result == 1)
			{
				itemsSacrificed[itemId] += amount;
			}

			return itemsSacrificed[itemId];
		}
	}
}
