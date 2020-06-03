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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using Terraria;
using TShockAPI.Net;
using TShockAPI.ServerSideCharacters;

namespace TShockAPI.DB
{
	public class CharacterManager
	{
		public IDbConnection database;

		public CharacterManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("tsCharacter",
									 new SqlColumn("Account", MySqlDbType.Int32) { Primary = true },
									 new SqlColumn("Health", MySqlDbType.Int32),
									 new SqlColumn("MaxHealth", MySqlDbType.Int32),
									 new SqlColumn("Mana", MySqlDbType.Int32),
									 new SqlColumn("MaxMana", MySqlDbType.Int32),
									 new SqlColumn("Inventory", MySqlDbType.Text),
									 new SqlColumn("extraSlot", MySqlDbType.Int32),
									 new SqlColumn("spawnX", MySqlDbType.Int32),
									 new SqlColumn("spawnY", MySqlDbType.Int32),
									 new SqlColumn("skinVariant", MySqlDbType.Int32),
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
												  ? (IQueryBuilder)new SqliteQueryCreator()
												  : new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
		}

		/// <summary>
		/// Determines if the given account ID has saved SSC data or not
		/// </summary>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public bool HasPlayerData(int accountId)
		{
			using (var reader = database.QueryReader("SELECT 1 FROM tsCharacter WHERE Account=@0", accountId))
			{
				return reader.Read();
			}
		}

		/// <summary>
		/// Creates the given server side player data in the database for the given account ID
		/// </summary>
		/// <param name="data"></param>
		/// <param name="accountId"></param>
		public void CreatePlayerData(ServerSidePlayerData data, int accountId)
		{
			database.Query(
				"INSERT INTO tsCharacter (Account, Health, MaxHealth, Mana, MaxMana, Inventory, extraSlot, spawnX, spawnY," +
				"skinVariant, hair, hairDye, hairColor, pantsColor, shirtColor, underShirtColor, shoeColor, skinColor, eyeColor, hideVisuals, questsCompleted) " +
				"VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18, @19, @20)",
				accountId, data.Stats.Health, data.Stats.MaxHealth, data.Stats.Mana, data.Stats.MaxMana, String.Join("~", data.Inventory.Items),
				data.Stats.HasExtraSlot ? 1 : 0, data.Spawn.TileX, data.Spawn.TileY, data.Vanity.SkinVariant, data.Vanity.Hair, data.Vanity.HairDye,
				TShock.Utils.EncodeColor(data.Vanity.HairColor), TShock.Utils.EncodeColor(data.Vanity.PantsColor), TShock.Utils.EncodeColor(data.Vanity.ShirtColor),
				TShock.Utils.EncodeColor(data.Vanity.UnderShirtColor), TShock.Utils.EncodeColor(data.Vanity.ShoeColor), TShock.Utils.EncodeColor(data.Vanity.SkinColor),
				TShock.Utils.EncodeColor(data.Vanity.EyeColor), TShock.Utils.EncodeBoolArray(data.Vanity.HideVisuals), data.Stats.QuestsCompleted
			);
		}

		/// <summary>
		/// Reads the server side player data from the database for the given account ID
		/// </summary>
		/// <param name="accountId"></param>
		/// <returns></returns>
		public ServerSidePlayerData ReadPlayerData(int accountId)
		{
			ServerSidePlayerData data = new ServerSidePlayerData();

			using (var reader = database.QueryReader("SELECT * FROM tsCharacter WHERE Account=@0", accountId))
			{
				if (reader.Read())
				{
					data.Stats = ReadStats(reader);
					data.Inventory = ReadInventory(reader);
					data.Spawn = ReadSpawn(reader);
					data.Vanity = ReadVanity(reader);
				}
			}

			return data;
		}

		internal ServerSideStats ReadStats(QueryResult reader)
		{
			ServerSideStats stats = new ServerSideStats
			{
				Health = reader.Get<int>("Health"),
				Mana = reader.Get<int>("Mana"),
				MaxHealth = reader.Get<int>("MaxHealth"),
				MaxMana = reader.Get<int>("MaxMana"),
				HasExtraSlot = reader.Get<int>("extraSlot") == 1,
				QuestsCompleted = reader.Get<int>("questsCompleted")
			};

			return stats;
		}

		internal ServerSideInventory ReadInventory(QueryResult reader)
		{
			IEnumerable<NetItem> items = reader.Get<string>("Inventory").Split('~').Select(NetItem.Parse);
			return ServerSideInventory.FromNetItems(items);
		}

		internal ServerSideSpawn ReadSpawn(QueryResult reader)
		{
			ServerSideSpawn spawn = new ServerSideSpawn
			{
				TileX = reader.Get<int>("spawnX"),
				TileY = reader.Get<int>("spawnY")
			};

			return spawn;
		}

		internal ServerSideVanity ReadVanity(QueryResult reader)
		{
			ServerSideVanity vanity = new ServerSideVanity
			{
				SkinVariant = reader.Get<int>("skinVariant"),
				Hair = reader.Get<int>("hair"),
				HairDye = (byte)reader.Get<int>("hairDye"),
				HairColor = TShock.Utils.DecodeColor(reader.Get<int?>("hairColor")),
				PantsColor = TShock.Utils.DecodeColor(reader.Get<int?>("pantsColor")),
				ShirtColor = TShock.Utils.DecodeColor(reader.Get<int?>("shirtColor")),
				UnderShirtColor = TShock.Utils.DecodeColor(reader.Get<int?>("underShirtColor")),
				ShoeColor = TShock.Utils.DecodeColor(reader.Get<int?>("shoeColor")),
				SkinColor = TShock.Utils.DecodeColor(reader.Get<int?>("skinColor")),
				EyeColor = TShock.Utils.DecodeColor(reader.Get<int?>("eyeColor")),
				HideVisuals = TShock.Utils.DecodeBoolArray(reader.Get<int?>("hideVisuals"))
			};

			return vanity;
		}

		/// <summary>
		/// Updates the given server side player data in the database for the given account ID
		/// </summary>
		/// <param name="data"></param>
		/// <param name="accountId"></param>
		public void UpdatePlayerData(ServerSidePlayerData data, int accountId)
		{
			StringBuilder sb = new StringBuilder("UPDATE tsCharacter SET ")
				.Append("Health = @0, MaxHealth = @1, Mana = @2, MaxMana = @3, extraSlot = @4, questsCompleted = @5,")
				.Append("spawnX = @6, spawnY = @7,")
				.Append("skinVariant = @8, hair = @9, hairDye = @10," +
						"hairColor = @11, pantsColor = @12, shirtColor = @13," +
						"underShirtColor = @14, shoeColor = @15, skinColor = @16," +
						"eyeColor = @17, hideVisuals = @18,")
				.Append("inventory = @19")
				.Append("WHERE Account = @20");

			database.Query(
				sb.ToString(),
				data.Stats.Health, data.Stats.MaxHealth, data.Stats.Mana, data.Stats.MaxMana, data.Stats.HasExtraSlot ? 1 : 0, data.Stats.QuestsCompleted,
				data.Spawn.TileX, data.Spawn.TileY,
				data.Vanity.SkinVariant, data.Vanity.Hair, data.Vanity.HairDye,
				TShock.Utils.EncodeColor(data.Vanity.HairColor), TShock.Utils.EncodeColor(data.Vanity.PantsColor), TShock.Utils.EncodeColor(data.Vanity.ShirtColor),
				TShock.Utils.EncodeColor(data.Vanity.UnderShirtColor), TShock.Utils.EncodeColor(data.Vanity.ShoeColor), TShock.Utils.EncodeColor(data.Vanity.SkinColor),
				TShock.Utils.EncodeColor(data.Vanity.EyeColor), TShock.Utils.EncodeBoolArray(data.Vanity.HideVisuals),
				String.Join("~", data.Inventory.Items),
				accountId
			);
		}

		/// <summary>
		/// Deletes the server side player data associated with the given account ID
		/// </summary>
		/// <param name="accountId">Account ID of the player</param>
		/// <returns>true if removed successfully</returns>
		public void DeletePlayerData(int accountId)
		{
			database.Query("DELETE FROM tsCharacter WHERE Account = @0;", accountId);
		}
	}
}
