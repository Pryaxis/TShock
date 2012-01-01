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
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
	public class InventoryManager
	{
		public IDbConnection database;

		public InventoryManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Inventory",
			                         new SqlColumn("Account", MySqlDbType.Int32) {Primary = true},
			                         new SqlColumn("MaxHealth", MySqlDbType.Int32),
			                         new SqlColumn("MaxMana", MySqlDbType.Int32),
			                         new SqlColumn("Inventory", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureExists(table);
		}

		public PlayerData GetPlayerData(TSPlayer player, int acctid)
		{
			PlayerData playerData = new PlayerData(player);

			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Inventory WHERE Account=@0", acctid))
				{
					if (reader.Read())
					{
						playerData.exists = true;
						playerData.maxHealth = reader.Get<int>("MaxHealth");
						playerData.inventory = NetItem.Parse(reader.Get<string>("Inventory"));
						return playerData;
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return playerData;
		}

		public bool InsertPlayerData(TSPlayer player)
		{
			PlayerData playerData = player.PlayerData;

			if (!player.IsLoggedIn)
				return false;

			if (!GetPlayerData(player, player.UserID).exists)
			{
				try
				{
					database.Query("INSERT INTO Inventory (Account, MaxHealth, Inventory) VALUES (@0, @1, @2);", player.UserID,
					               playerData.maxHealth, NetItem.ToString(playerData.inventory));
					return true;
				}
				catch (Exception ex)
				{
					Log.Error(ex.ToString());
				}
			}
			else
			{
				try
				{
					database.Query("UPDATE Inventory SET MaxHealth = @0, Inventory = @1 WHERE Account = @2;", playerData.maxHealth,
					               NetItem.ToString(playerData.inventory), player.UserID);
					return true;
				}
				catch (Exception ex)
				{
					Log.Error(ex.ToString());
				}
			}
			return false;
		}
	}
}