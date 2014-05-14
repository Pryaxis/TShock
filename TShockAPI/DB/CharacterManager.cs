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
using System.Data;
using MySql.Data.MySqlClient;
using Terraria;

namespace TShockAPI.DB
{
	public class CharacterManager
	{
		public IDbConnection database;

		public CharacterManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("tsCharacter",
			                         new SqlColumn("Account", MySqlDbType.Int32) {Primary = true},
									 new SqlColumn("Health", MySqlDbType.Int32),
			                         new SqlColumn("MaxHealth", MySqlDbType.Int32),
									 new SqlColumn("Mana", MySqlDbType.Int32),
			                         new SqlColumn("MaxMana", MySqlDbType.Int32),
			                         new SqlColumn("Inventory", MySqlDbType.Text),
									 new SqlColumn("spawnX", MySqlDbType.Int32),
									 new SqlColumn("spawnY", MySqlDbType.Int32),
									 new SqlColumn("hair", MySqlDbType.Int32),
									 new SqlColumn("hairDye", MySqlDbType.Int32),
									 new SqlColumn("hairColor", MySqlDbType.Int32),
									 new SqlColumn("pantsColor", MySqlDbType.Int32),
									 new SqlColumn("shirtColor", MySqlDbType.Int32),
									 new SqlColumn("underShirtColor", MySqlDbType.Int32),
									 new SqlColumn("shoeColor", MySqlDbType.Int32),
									 new SqlColumn("hideVisuals", MySqlDbType.Int32),
									 new SqlColumn("skinColor", MySqlDbType.Int32),
									 new SqlColumn("eyeColor", MySqlDbType.Int32),
									 new SqlColumn("questsCompleted", MySqlDbType.Int32)
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
				using (var reader = database.QueryReader("SELECT * FROM tsCharacter WHERE Account=@0", acctid))
				{
					if (reader.Read())
					{
						playerData.exists = true;
						playerData.health = reader.Get<int>("Health");
						playerData.maxHealth = reader.Get<int>("MaxHealth");
						playerData.mana = reader.Get<int>("Mana");
						playerData.maxMana = reader.Get<int>("MaxMana");
						playerData.inventory = NetItem.Parse(reader.Get<string>("Inventory"));
						playerData.spawnX = reader.Get<int>("spawnX");
						playerData.spawnY = reader.Get<int>("spawnY");
						playerData.hair = reader.Get<int?>("hair");
						playerData.hairDye = (byte)reader.Get<int>("hairDye");
						playerData.hairColor = TShock.Utils.DecodeColor(reader.Get<int?>("hairColor"));
						playerData.pantsColor = TShock.Utils.DecodeColor(reader.Get<int?>("pantsColor"));
						playerData.shirtColor = TShock.Utils.DecodeColor(reader.Get<int?>("shirtColor"));
						playerData.underShirtColor = TShock.Utils.DecodeColor(reader.Get<int?>("underShirtColor"));
						playerData.shoeColor = TShock.Utils.DecodeColor(reader.Get<int?>("shoeColor"));
						playerData.hideVisuals = TShock.Utils.DecodeBitsByte(reader.Get<int?>("hideVisuals"));
						playerData.skinColor = TShock.Utils.DecodeColor(reader.Get<int?>("skinColor"));
						playerData.eyeColor = TShock.Utils.DecodeColor(reader.Get<int?>("eyeColor"));
						playerData.questsCompleted = reader.Get<int>("questsCompleted");
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

		public bool SeedInitialData(User user)
		{
			string initialItems = "-15,1,0~-13,1,0~-16,1,45~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0~0,0,0";
			try
			{
				database.Query("INSERT INTO tsCharacter (Account, Health, MaxHealth, Mana, MaxMana, Inventory, spawnX, spawnY, completedQuests) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8);", user.ID,
							   100, 100, 20, 20, initialItems, -1, -1, 0);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return false;
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
					database.Query(
						"INSERT INTO tsCharacter (Account, Health, MaxHealth, Mana, MaxMana, Inventory, spawnX, spawnY, hair, hairDye, hairColor, pantsColor, shirtColor, underShirtColor, shoeColor, hideVisuals, skinColor, eyeColor, questsCompleted) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16);", 
						player.UserID, playerData.health, playerData.maxHealth, playerData.mana, playerData.maxMana, NetItem.ToString(playerData.inventory), player.TPlayer.SpawnX, player.TPlayer.SpawnY, player.TPlayer.hair, player.TPlayer.hairDye, TShock.Utils.EncodeColor(player.TPlayer.hairColor), TShock.Utils.EncodeColor(player.TPlayer.pantsColor), TShock.Utils.EncodeColor(player.TPlayer.shirtColor), TShock.Utils.EncodeColor(player.TPlayer.underShirtColor), TShock.Utils.EncodeColor(player.TPlayer.shoeColor), TShock.Utils.EncodeBitsByte(player.TPlayer.hideVisual), TShock.Utils.EncodeColor(player.TPlayer.skinColor), TShock.Utils.EncodeColor(player.TPlayer.eyeColor), player.TPlayer.anglerQuestsFinished);
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
					database.Query(
						"UPDATE tsCharacter SET Health = @0, MaxHealth = @1, Mana = @2, MaxMana = @3, Inventory = @4, spawnX = @6, spawnY = @7, hair = @8, hairDye = @9, hairColor = @10, pantsColor = @11, shirtColor = @12, underShirtColor = @13, shoeColor = @14, hideVisuals = @15, skinColor = @16, eyeColor = @17, questsCompleted = @18 WHERE Account = @5;",
						playerData.health, playerData.maxHealth, playerData.mana, playerData.maxMana, NetItem.ToString(playerData.inventory), player.UserID, player.TPlayer.SpawnX, player.TPlayer.SpawnY, player.TPlayer.hair, player.TPlayer.hairDye, TShock.Utils.EncodeColor(player.TPlayer.hairColor), TShock.Utils.EncodeColor(player.TPlayer.pantsColor), TShock.Utils.EncodeColor(player.TPlayer.shirtColor), TShock.Utils.EncodeColor(player.TPlayer.underShirtColor), TShock.Utils.EncodeColor(player.TPlayer.shoeColor), TShock.Utils.EncodeBitsByte(player.TPlayer.hideVisual), TShock.Utils.EncodeColor(player.TPlayer.skinColor), TShock.Utils.EncodeColor(player.TPlayer.eyeColor), player.TPlayer.anglerQuestsFinished);
					return true;
				}
				catch (Exception ex)
				{
					Log.Error(ex.ToString());
				}
			}
			return false;
		}

		public bool RemovePlayer(int userid)
		{
			try
			{
				database.Query("DELETE FROM tsCharacter WHERE Account = @0;", userid);
				return true;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}

			return false;
		}
	}
}