﻿/*
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
									 new SqlColumn("questsCompleted", MySqlDbType.Int32),
									 new SqlColumn("usingBiomeTorches", MySqlDbType.Int32),
									 new SqlColumn("happyFunTorchTime", MySqlDbType.Int32),
									 new SqlColumn("unlockedBiomeTorches", MySqlDbType.Int32),
									 new SqlColumn("currentLoadoutIndex", MySqlDbType.Int32),
									 new SqlColumn("ateArtisanBread", MySqlDbType.Int32),
									 new SqlColumn("usedAegisCrystal", MySqlDbType.Int32),
									 new SqlColumn("usedAegisFruit", MySqlDbType.Int32),
									 new SqlColumn("usedArcaneCrystal", MySqlDbType.Int32),
									 new SqlColumn("usedGalaxyPearl", MySqlDbType.Int32),
									 new SqlColumn("usedGummyWorm", MySqlDbType.Int32),
									 new SqlColumn("usedAmbrosia", MySqlDbType.Int32),
									 new SqlColumn("unlockedSuperCart", MySqlDbType.Int32),
									 new SqlColumn("enabledSuperCart", MySqlDbType.Int32)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
		}

		public PlayerData GetPlayerData(TSPlayer player, int acctid)
		{
			PlayerData playerData = new PlayerData(false);

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
						List<NetItem> inventory = reader.Get<string>("Inventory").Split('~').Select(NetItem.Parse).ToList();
						if (inventory.Count < NetItem.MaxInventory)
						{
							//TODO: unhardcode this - stop using magic numbers and use NetItem numbers
							//Set new armour slots empty
							inventory.InsertRange(67, new NetItem[2]);
							//Set new vanity slots empty
							inventory.InsertRange(77, new NetItem[2]);
							//Set new dye slots empty
							inventory.InsertRange(87, new NetItem[2]);
							//Set the rest of the new slots empty
							inventory.AddRange(new NetItem[NetItem.MaxInventory - inventory.Count]);
						}
						playerData.inventory = inventory.ToArray();
						playerData.extraSlot = reader.Get<int>("extraSlot");
						playerData.spawnX = reader.Get<int>("spawnX");
						playerData.spawnY = reader.Get<int>("spawnY");
						playerData.skinVariant = reader.Get<int?>("skinVariant");
						playerData.hair = reader.Get<int?>("hair");
						playerData.hairDye = (byte)reader.Get<int>("hairDye");
						playerData.hairColor = TShock.Utils.DecodeColor(reader.Get<int?>("hairColor"));
						playerData.pantsColor = TShock.Utils.DecodeColor(reader.Get<int?>("pantsColor"));
						playerData.shirtColor = TShock.Utils.DecodeColor(reader.Get<int?>("shirtColor"));
						playerData.underShirtColor = TShock.Utils.DecodeColor(reader.Get<int?>("underShirtColor"));
						playerData.shoeColor = TShock.Utils.DecodeColor(reader.Get<int?>("shoeColor"));
						playerData.hideVisuals = TShock.Utils.DecodeBoolArray(reader.Get<int?>("hideVisuals"));
						playerData.skinColor = TShock.Utils.DecodeColor(reader.Get<int?>("skinColor"));
						playerData.eyeColor = TShock.Utils.DecodeColor(reader.Get<int?>("eyeColor"));
						playerData.questsCompleted = reader.Get<int>("questsCompleted");
						playerData.usingBiomeTorches = reader.Get<int>("usingBiomeTorches");
						playerData.happyFunTorchTime = reader.Get<int>("happyFunTorchTime");
						playerData.unlockedBiomeTorches = reader.Get<int>("unlockedBiomeTorches");
						playerData.currentLoadoutIndex = reader.Get<int>("currentLoadoutIndex");
						playerData.ateArtisanBread = reader.Get<int>("ateArtisanBread");
						playerData.usedAegisCrystal = reader.Get<int>("usedAegisCrystal");
						playerData.usedAegisFruit = reader.Get<int>("usedAegisFruit");
						playerData.usedArcaneCrystal = reader.Get<int>("usedArcaneCrystal");
						playerData.usedGalaxyPearl = reader.Get<int>("usedGalaxyPearl");
						playerData.usedGummyWorm = reader.Get<int>("usedGummyWorm");
						playerData.usedAmbrosia = reader.Get<int>("usedAmbrosia");
						playerData.unlockedSuperCart = reader.Get<int>("unlockedSuperCart");
						playerData.enabledSuperCart = reader.Get<int>("enabledSuperCart");
						return playerData;
					}
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}

			return playerData;
		}

		public bool SeedInitialData(UserAccount account)
		{
			var inventory = new StringBuilder();

			var items = new List<NetItem>(TShock.ServerSideCharacterConfig.Settings.StartingInventory);
			if (items.Count < NetItem.MaxInventory)
				items.AddRange(new NetItem[NetItem.MaxInventory - items.Count]);

			string initialItems = String.Join("~", items.Take(NetItem.MaxInventory));
			try
			{
				database.Query("INSERT INTO tsCharacter (Account, Health, MaxHealth, Mana, MaxMana, Inventory, spawnX, spawnY, questsCompleted) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8);",
							   account.ID,
							   TShock.ServerSideCharacterConfig.Settings.StartingHealth,
							   TShock.ServerSideCharacterConfig.Settings.StartingHealth,
							   TShock.ServerSideCharacterConfig.Settings.StartingMana,
							   TShock.ServerSideCharacterConfig.Settings.StartingMana,
							   initialItems,
							   -1,
							   -1,
							   0);
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}

			return false;
		}

		/// <summary>
		/// Inserts player data to the tsCharacter database table
		/// </summary>
		/// <param name="player">player to take data from</param>
		/// <returns>true if inserted successfully</returns>
		public bool InsertPlayerData(TSPlayer player, bool fromCommand = false)
		{
			PlayerData playerData = player.PlayerData;

			if (!player.IsLoggedIn)
				return false;

			if (player.State < 10)
				return false;

			if (player.HasPermission(Permissions.bypassssc) && !fromCommand)
			{
				TShock.Log.ConsoleInfo(GetParticularString("{0} is a player name", $"Skipping SSC save (due to tshock.ignore.ssc) for {player.Account.Name}"));
				return false;
			}

			if (!GetPlayerData(player, player.Account.ID).exists)
			{
				try
				{
					database.Query(
						"INSERT INTO tsCharacter (Account, Health, MaxHealth, Mana, MaxMana, Inventory, extraSlot, spawnX, spawnY, skinVariant, hair, hairDye, hairColor, pantsColor, shirtColor, underShirtColor, shoeColor, hideVisuals, skinColor, eyeColor, questsCompleted, usingBiomeTorches, happyFunTorchTime, unlockedBiomeTorches, currentLoadoutIndex,ateArtisanBread, usedAegisCrystal, usedAegisFruit, usedArcaneCrystal, usedGalaxyPearl, usedGummyWorm, usedAmbrosia, unlockedSuperCart, enabledSuperCart) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18, @19, @20, @21, @22, @23, @24, @25, @26, @27, @28, @29, @30, @31, @32, @33);",
						player.Account.ID, playerData.health, playerData.maxHealth, playerData.mana, playerData.maxMana, String.Join("~", playerData.inventory), playerData.extraSlot, player.TPlayer.SpawnX, player.TPlayer.SpawnY, player.TPlayer.skinVariant, player.TPlayer.hair, player.TPlayer.hairDye, TShock.Utils.EncodeColor(player.TPlayer.hairColor), TShock.Utils.EncodeColor(player.TPlayer.pantsColor),TShock.Utils.EncodeColor(player.TPlayer.shirtColor), TShock.Utils.EncodeColor(player.TPlayer.underShirtColor), TShock.Utils.EncodeColor(player.TPlayer.shoeColor), TShock.Utils.EncodeBoolArray(player.TPlayer.hideVisibleAccessory), TShock.Utils.EncodeColor(player.TPlayer.skinColor),TShock.Utils.EncodeColor(player.TPlayer.eyeColor), player.TPlayer.anglerQuestsFinished, player.TPlayer.UsingBiomeTorches ? 1 : 0, player.TPlayer.happyFunTorchTime ? 1 : 0, player.TPlayer.unlockedBiomeTorches ? 1 : 0, player.TPlayer.CurrentLoadoutIndex, player.TPlayer.ateArtisanBread ? 1 : 0, player.TPlayer.usedAegisCrystal ? 1 : 0, player.TPlayer.usedAegisFruit ? 1 : 0, player.TPlayer.usedArcaneCrystal ? 1 : 0, player.TPlayer.usedGalaxyPearl ? 1 : 0, player.TPlayer.usedGummyWorm ? 1 : 0, player.TPlayer.usedAmbrosia ? 1 : 0, player.TPlayer.unlockedSuperCart ? 1 : 0, player.TPlayer.enabledSuperCart ? 1 : 0);
					return true;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}
			else
			{
				try
				{
					database.Query(
						"UPDATE tsCharacter SET Health = @0, MaxHealth = @1, Mana = @2, MaxMana = @3, Inventory = @4, spawnX = @6, spawnY = @7, hair = @8, hairDye = @9, hairColor = @10, pantsColor = @11, shirtColor = @12, underShirtColor = @13, shoeColor = @14, hideVisuals = @15, skinColor = @16, eyeColor = @17, questsCompleted = @18, skinVariant = @19, extraSlot = @20, usingBiomeTorches = @21, happyFunTorchTime = @22, unlockedBiomeTorches = @23, currentLoadoutIndex = @24, ateArtisanBread = @25, usedAegisCrystal = @26, usedAegisFruit = @27, usedArcaneCrystal = @28, usedGalaxyPearl = @29, usedGummyWorm = @30, usedAmbrosia = @31, unlockedSuperCart = @32, enabledSuperCart = @33 WHERE Account = @5;",
						playerData.health, playerData.maxHealth, playerData.mana, playerData.maxMana, String.Join("~", playerData.inventory), player.Account.ID, player.TPlayer.SpawnX, player.TPlayer.SpawnY, player.TPlayer.hair, player.TPlayer.hairDye, TShock.Utils.EncodeColor(player.TPlayer.hairColor), TShock.Utils.EncodeColor(player.TPlayer.pantsColor), TShock.Utils.EncodeColor(player.TPlayer.shirtColor), TShock.Utils.EncodeColor(player.TPlayer.underShirtColor), TShock.Utils.EncodeColor(player.TPlayer.shoeColor), TShock.Utils.EncodeBoolArray(player.TPlayer.hideVisibleAccessory), TShock.Utils.EncodeColor(player.TPlayer.skinColor), TShock.Utils.EncodeColor(player.TPlayer.eyeColor), player.TPlayer.anglerQuestsFinished, player.TPlayer.skinVariant, player.TPlayer.extraAccessory ? 1 : 0, player.TPlayer.UsingBiomeTorches ? 1 : 0, player.TPlayer.happyFunTorchTime ? 1 : 0, player.TPlayer.unlockedBiomeTorches ? 1 : 0, player.TPlayer.CurrentLoadoutIndex, player.TPlayer.ateArtisanBread ? 1 : 0, player.TPlayer.usedAegisCrystal ? 1 : 0, player.TPlayer.usedAegisFruit ? 1 : 0, player.TPlayer.usedArcaneCrystal ? 1 : 0, player.TPlayer.usedGalaxyPearl ? 1 : 0, player.TPlayer.usedGummyWorm ? 1 : 0, player.TPlayer.usedAmbrosia ? 1 : 0, player.TPlayer.unlockedSuperCart ? 1 : 0, player.TPlayer.enabledSuperCart ? 1 : 0);
					return true;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}
			return false;
		}

		/// <summary>
		/// Removes a player's data from the tsCharacter database table
		/// </summary>
		/// <param name="userid">User ID of the player</param>
		/// <returns>true if removed successfully</returns>
		public bool RemovePlayer(int userid)
		{
			try
			{
				database.Query("DELETE FROM tsCharacter WHERE Account = @0;", userid);
				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}

			return false;
		}

		/// <summary>
		/// Inserts a specific PlayerData into the SSC table for a player.
		/// </summary>
		/// <param name="player">The player to store the data for.</param>
		/// <param name="data">The player data to store.</param>
		/// <returns>If the command succeeds.</returns>
		public bool InsertSpecificPlayerData(TSPlayer player, PlayerData data)
		{
			PlayerData playerData = data;

			if (!player.IsLoggedIn)
				return false;

			if (player.HasPermission(Permissions.bypassssc))
			{
				TShock.Log.ConsoleInfo(GetParticularString("{0} is a player name", $"Skipping SSC save (due to tshock.ignore.ssc) for {player.Account.Name}"));
				return true;
			}

			if (!GetPlayerData(player, player.Account.ID).exists)
			{
				try
				{
					database.Query(
						"INSERT INTO tsCharacter (Account, Health, MaxHealth, Mana, MaxMana, Inventory, extraSlot, spawnX, spawnY, skinVariant, hair, hairDye, hairColor, pantsColor, shirtColor, underShirtColor, shoeColor, hideVisuals, skinColor, eyeColor, questsCompleted, usingBiomeTorches, happyFunTorchTime, unlockedBiomeTorches, currentLoadoutIndex, ateArtisanBread, usedAegisCrystal, usedAegisFruit, usedArcaneCrystal, usedGalaxyPearl, usedGummyWorm, usedAmbrosia, unlockedSuperCart, enabledSuperCart) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11, @12, @13, @14, @15, @16, @17, @18, @19, @20, @21, @22, @23, @24, @25, @26, @27, @28, @29, @30, @31, @32, @33);",
						player.Account.ID,
						playerData.health,
						playerData.maxHealth,
						playerData.mana,
						playerData.maxMana,
						String.Join("~", playerData.inventory),
						playerData.extraSlot,
						playerData.spawnX,
						playerData.spawnX,
						playerData.skinVariant,
						playerData.hair,
						playerData.hairDye,
						TShock.Utils.EncodeColor(playerData.hairColor),
						TShock.Utils.EncodeColor(playerData.pantsColor),
						TShock.Utils.EncodeColor(playerData.shirtColor),
						TShock.Utils.EncodeColor(playerData.underShirtColor),
						TShock.Utils.EncodeColor(playerData.shoeColor),
						TShock.Utils.EncodeBoolArray(playerData.hideVisuals),
						TShock.Utils.EncodeColor(playerData.skinColor),
						TShock.Utils.EncodeColor(playerData.eyeColor),
						playerData.questsCompleted,
						playerData.usingBiomeTorches,
						playerData.happyFunTorchTime,
						playerData.unlockedBiomeTorches,
						playerData.currentLoadoutIndex,
						playerData.ateArtisanBread,
						playerData.usedAegisCrystal,
						playerData.usedAegisFruit,
						playerData.usedArcaneCrystal,
						playerData.usedGalaxyPearl,
						playerData.usedGummyWorm,
						playerData.usedAmbrosia,
						playerData.unlockedSuperCart,
						playerData.enabledSuperCart);
					return true;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}
			else
			{
				try
				{
					database.Query(
						"UPDATE tsCharacter SET Health = @0, MaxHealth = @1, Mana = @2, MaxMana = @3, Inventory = @4, spawnX = @6, spawnY = @7, hair = @8, hairDye = @9, hairColor = @10, pantsColor = @11, shirtColor = @12, underShirtColor = @13, shoeColor = @14, hideVisuals = @15, skinColor = @16, eyeColor = @17, questsCompleted = @18, skinVariant = @19, extraSlot = @20, usingBiomeTorches = @21, happyFunTorchTime = @22, unlockedBiomeTorches = @23, currentLoadoutIndex = @24, ateArtisanBread = @25, usedAegisCrystal = @26, usedAegisFruit = @27, usedArcaneCrystal = @28, usedGalaxyPearl = @29, usedGummyWorm = @30, usedAmbrosia = @31, unlockedSuperCart = @32, enabledSuperCart = @33 WHERE Account = @5;",
						playerData.health,
						playerData.maxHealth,
						playerData.mana,
						playerData.maxMana,
						String.Join("~", playerData.inventory),
						player.Account.ID,
						playerData.spawnX,
						playerData.spawnX,
						playerData.skinVariant,
						playerData.hair,
						playerData.hairDye,
						TShock.Utils.EncodeColor(playerData.hairColor),
						TShock.Utils.EncodeColor(playerData.pantsColor),
						TShock.Utils.EncodeColor(playerData.shirtColor),
						TShock.Utils.EncodeColor(playerData.underShirtColor),
						TShock.Utils.EncodeColor(playerData.shoeColor),
						TShock.Utils.EncodeBoolArray(playerData.hideVisuals),
						TShock.Utils.EncodeColor(playerData.skinColor),
						TShock.Utils.EncodeColor(playerData.eyeColor),
						playerData.questsCompleted,
						playerData.extraSlot ?? 0,
						playerData.usingBiomeTorches,
						playerData.happyFunTorchTime,
						playerData.unlockedBiomeTorches,
						playerData.currentLoadoutIndex,
						playerData.ateArtisanBread,
						playerData.usedAegisCrystal,
						playerData.usedAegisFruit,
						playerData.usedArcaneCrystal,
						playerData.usedGalaxyPearl,
						playerData.usedGummyWorm,
						playerData.usedAmbrosia,
						playerData.unlockedSuperCart,
						playerData.enabledSuperCart);
					return true;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}
			return false;
		}
	}
}
