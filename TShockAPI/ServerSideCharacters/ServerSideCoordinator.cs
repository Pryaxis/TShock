using System;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.Net;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.Net;

namespace TShockAPI.ServerSideCharacters
{
	/// <summary>
	/// Provides and executes CRUD operations on server side player data
	/// </summary>
	public class ServerSideCoordinator
	{
		private DateTime _lastSave;

		/// <summary>
		/// Creates a new instance of the server side coordinator, hooking into {put hook names here}
		/// </summary>
		public ServerSideCoordinator()
		{
			AccountHooks.AccountDelete += OnAccountDelete;
			PlayerHooks.PlayerPostLogin += OnPlayerPostLogin;
			PlayerHooks.PlayerLogout += OnPlayerLogout;

			GetDataHandlers.PlayerConnect += OnPlayerConnect;
			GetDataHandlers.KillMe += OnPlayerDeath;
			GetDataHandlers.PlayerSlot += OnPlayerSlotChange;
			GetDataHandlers.PlayerHP += OnPlayerHpChange;
			GetDataHandlers.PlayerMana += OnPlayerManaChange;

			_lastSave = DateTime.UtcNow;
		}

		private void OnAccountDelete(AccountDeleteEventArgs e)
		{
			DeleteAccountData(e.Account);
		}

		private void OnPlayerPostLogin(PlayerPostLoginEventArgs e)
		{
			if (!Main.ServerSideCharacter)
			{
				return;
			}

			if (e.Player.HasPermission(Permissions.bypassssc))
			{
				// Do bypass stuff?
				return;
			}

			ServerSidePlayerData data;

			if (TShock.CharacterDB.HasPlayerData(e.Player.Account.ID))
			{
				// Retrieve existing data
				data = ReadPlayerData(e.Player);
			}
			else
			{
				// Create new data and write it to disk
				data = CreateNewPlayerData(e.Player, writeToDisk: true);
			}

			e.Player.SscData = data;
			ApplyServerSidePlayerData(e.Player);
		}

		private void OnPlayerLogout(PlayerLogoutEventArgs e)
		{
			if (!Main.ServerSideCharacter)
			{
				// Ignore non-SSC logouts
				return;
			}

			// If the player is logging out and isn't trying to cheat their SSC data, update their data
			if (!e.Player.IsDisabledPendingTrashRemoval && (!e.Player.Dead || e.Player.TPlayer.difficulty != 2))
			{
				SavePlayerData(e.Player);
			}
		}

		private void OnPlayerConnect(object sender, GetDataHandledEventArgs args)
		{
			// Save their character data from on join.
			// This can be set as their real player data by commands, or applied temporarily when they logout or use the overridessc command
			args.Player.SscDataWhenJoined = ServerSidePlayerData.CreateFromPlayer(args.Player.TPlayer);
		}

		private void OnPlayerDeath(object sender, GetDataHandlers.KillMeEventArgs args)
		{
			if (!Main.ServerSideCharacter || !args.Player.IsLoggedIn)
			{
				// Ignore non-SSC deaths and non-logged in deaths
				return;
			}

			if (args.Player.TPlayer.difficulty == PlayerDifficultyID.Hardcore)
			{
				// The player has died in hardcore, so delete their saved data
				DeletePlayerData(args.Player);
				TShock.Log.ConsoleDebug("GetDataHandlers / HandlePlayerKillMeV2 ssc delete {0} {1}", args.Player.Name, args.Player.TPlayer.difficulty);
				args.Player.SendErrorMessage("You have fallen in hardcore mode, and your items have been lost forever.");
				// Then create new data, write it to disk, and apply it
				args.Player.SscData = CreateNewPlayerData(args.Player, writeToDisk: true);
				ApplyServerSidePlayerData(args.Player);
			}
		}

		private void OnPlayerSlotChange(object sender, GetDataHandlers.PlayerSlotEventArgs args)
		{
			if (!Main.ServerSideCharacter)
			{
				return;
			}

			if (args.Player.IsLoggedIn)
			{
				args.Player.SscData.Inventory.UpdateInventorySlot(args.Slot, new NetItem(args.Type, args.Stack, args.Prefix));
			}
			else if (TShock.Config.DisableLoginBeforeJoin && args.Player.HasSentInventory && !args.Player.HasPermission(Permissions.bypassssc))
			{
				// The player might have moved an item to their trash can before they performed a single login attempt yet.
				args.Player.IsDisabledPendingTrashRemoval = true;
			}
		}

		private void OnPlayerHpChange(object sender, GetDataHandlers.PlayerHPEventArgs args)
		{
			if (args.Player.IsLoggedIn)
			{
				args.Player.TPlayer.statLife = args.Current;
				args.Player.TPlayer.statLifeMax = args.Max;

				args.Player.SscData.Stats.Health = args.Current;
				args.Player.SscData.Stats.MaxHealth = args.Max;
			}
		}

		private void OnPlayerManaChange(object sender, GetDataHandlers.PlayerManaEventArgs args)
		{
			if (args.Player.IsLoggedIn)
			{
				args.Player.TPlayer.statMana = args.Current;
				args.Player.TPlayer.statManaMax = args.Max;

				args.Player.SscData.Stats.Mana = args.Current;
				args.Player.SscData.Stats.MaxMana = args.Max;
			}
		}

		/// <summary>
		/// Invoked approximately once per second
		/// </summary>
		internal void OnSecondUpdate()
		{
			int saveInterval = TShock.ServerSideCharacterConfig.ServerSideCharacterSave * 60; // converting minutes to seconds
			if ((DateTime.UtcNow - _lastSave).TotalSeconds >= saveInterval)
			{
				SaveAllPlayersData();

				_lastSave = DateTime.UtcNow;
			}
		}

		/// <summary>
		/// Creates new server side player data for the given player and optionally writes it to disk
		/// </summary>
		/// <param name="player"></param>
		/// <param name="writeToDisk"></param>
		public static ServerSidePlayerData CreateNewPlayerData(TSPlayer player, bool writeToDisk)
		{
			var data = ServerSidePlayerData.CreateDefaultFromPlayer(player.TPlayer);

			if (writeToDisk)
			{
				TShock.CharacterDB.InsertPlayerData(data, player.Account.ID);
			}

			return data;
		}

		/// <summary>
		/// Reads a player's server side player data from disk
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static ServerSidePlayerData ReadPlayerData(TSPlayer player)
		{
			return TShock.CharacterDB.ReadPlayerData(player.Account.ID);
		}

		/// <summary>
		/// Writes a player's current player data to disk
		/// </summary>
		/// <param name="player"></param>
		public static void SavePlayerData(TSPlayer player)
		{
			TShock.CharacterDB.UpdatePlayerData(player.SscData, player.Account.ID);
		}

		/// <summary>
		/// Writes all connected players current player data to disk
		/// </summary>
		public static void SaveAllPlayersData()
		{
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.IsLoggedIn && !player.IsDisabledPendingTrashRemoval)
				{
					SavePlayerData(player);
				}
			}
		}

		/// <summary>
		/// Deletes a player's server side player data from disk
		/// </summary>
		/// <param name="player"></param>
		public static void DeletePlayerData(TSPlayer player) => DeleteAccountData(player.Account);

		/// <summary>
		/// Deletes an account's server side player data from disk
		/// </summary>
		/// <param name="account"></param>
		public static void DeleteAccountData(UserAccount account)
		{
			TShock.CharacterDB.DeletePlayerData(account.ID);
		}

		/// <summary>
		/// Applies a set of server side player data, syncing it from the server to the client
		/// </summary>
		/// <param name="player"></param>
		/// <param name="temporaryData"></param>
		public static void ApplyServerSidePlayerData(TSPlayer player, ServerSidePlayerData temporaryData = null)
		{
			ServerSidePlayerData data = temporaryData ?? player.SscData;

			// Start ignoring SSC-related packets! This is critical so that we don't send or receive dirty data!
			player.IgnoreSSCPackets = true;

			ApplyStatData(player.TPlayer, data.Stats);
			ApplySpawnData(player.TPlayer, data.Spawn);
			ApplyVanityData(player.TPlayer, data.Vanity);
			ApplyInventoryData(player.TPlayer, data.Inventory);

			SyncData(player, data);
		}

		static void ApplyStatData(Player player, ServerSideStats stats)
		{
			player.statLife = stats.Health;
			player.statLifeMax = stats.MaxHealth;
			player.statMana = stats.Mana;
			player.statManaMax = stats.MaxMana;
			player.extraAccessory = stats.HasExtraSlot;
			player.anglerQuestsFinished = stats.QuestsCompleted;
			player.golferScoreAccumulated = stats.GolfScoreAccumulated;
		}

		static void ApplySpawnData(Player player, ServerSideSpawn spawn)
		{
			player.SpawnX = spawn.TileX;
			player.SpawnY = spawn.TileY;
		}

		static void ApplyVanityData(Player player, ServerSideVanity vanity)
		{
			player.hairDye = vanity.HairDye;
			player.skinVariant = vanity.SkinVariant;
			player.hair = vanity.Hair;
			player.hairColor = vanity.HairColor;
			player.pantsColor = vanity.PantsColor;
			player.shirtColor = vanity.ShirtColor;
			player.underShirtColor = vanity.UnderShirtColor;
			player.shoeColor = vanity.ShoeColor;
			player.skinColor = vanity.SkinColor;
			player.eyeColor = vanity.EyeColor;
			player.hideVisibleAccessory = vanity.HideVisuals;
		}

		static void ApplyInventoryData(Player player, ServerSideInventory inventory)
		{
			for (int i = 0; i < NetItem.TotalSlots; i++)
			{
				if (i <= NetItem.FullInventoryGroup.End)
				{
					//0-58
					player.inventory[i].netDefaults(inventory[i].NetId);

					if (player.inventory[i].netID != 0)
					{	
						player.inventory[i].stack = inventory[i].Stack;
						player.inventory[i].Prefix(inventory[i].PrefixId);
					}
				}
				else if (i <= NetItem.FullEquipmentAndVanityGroup.End)
				{
					//59-78
					var index = i - NetItem.FullEquipmentAndVanityGroup.Start;
					player.armor[index].netDefaults(inventory[i].NetId);

					if (player.armor[index].netID != 0)
					{	
						player.armor[index].stack = inventory[i].Stack;
						player.armor[index].Prefix(inventory[i].PrefixId);
					}
				}
				else if (i <= NetItem.FullArmorAndVanityDyeGroup.End)
				{
					//79-88
					var index = i - NetItem.FullArmorAndVanityDyeGroup.Start;
					player.dye[index].netDefaults(inventory[i].NetId);

					if (player.dye[index].netID != 0)
					{	
						player.dye[index].stack = inventory[i].Stack;
						player.dye[index].Prefix(inventory[i].PrefixId);
					}
				}
				else if (i <= NetItem.MiscEquipGroup.End)
				{
					//89-93
					var index = i - NetItem.MiscEquipGroup.Start;
					player.miscEquips[index].netDefaults(inventory[i].NetId);

					if (player.miscEquips[index].netID != 0)
					{
						player.miscEquips[index].stack = inventory[i].Stack;
						player.miscEquips[index].Prefix(inventory[i].PrefixId);
					}
				}
				else if (i <= NetItem.MiscDyeGroup.End)
				{
					//93-98
					var index = i - NetItem.MiscDyeGroup.Start;
					player.miscDyes[index].netDefaults(inventory[i].NetId);

					if (player.miscDyes[index].netID != 0)
					{
						player.miscDyes[index].stack = inventory[i].Stack;
						player.miscDyes[index].Prefix(inventory[i].PrefixId);
					}
				}
				else if (i <= NetItem.PiggyGroup.End)
				{
					//99-138
					var index = i - NetItem.PiggyGroup.Start;
					player.bank.item[index].netDefaults(inventory[i].NetId);

					if (player.bank.item[index].netID != 0)
					{
						player.bank.item[index].stack = inventory[i].Stack;
						player.bank.item[index].Prefix(inventory[i].PrefixId);
					}
				}
				else if (i <= NetItem.SafeGroup.End)
				{
					//139-178
					var index = i - NetItem.SafeGroup.Start;
					player.bank2.item[index].netDefaults(inventory[i].NetId);

					if (player.bank2.item[index].netID != 0)
					{
						player.bank2.item[index].stack = inventory[i].Stack;
						player.bank2.item[index].Prefix(inventory[i].PrefixId);
					}
				}
				else if (i <= NetItem.TrashGroup.End)
				{
					//179
					player.trashItem.netDefaults(inventory[i].NetId);

					if (player.trashItem.netID != 0)
					{
						player.trashItem.stack = inventory[i].Stack;
						player.trashItem.Prefix(inventory[i].PrefixId);
					}
				}
				else if (i <= NetItem.ForgeGroup.End)
				{
					//180-219
					var index = i - NetItem.ForgeGroup.Start;
					player.bank3.item[index].netDefaults(inventory[i].NetId);

					if (player.bank3.item[index].netID != 0)
					{
						player.bank3.item[index].stack = inventory[i].Stack;
						player.bank3.item[index].Prefix(inventory[i].PrefixId);
					}
				}
				else
				{
					//220-259
					var index = i - NetItem.VoidGroup.Start;
					player.bank4.item[index].netDefaults(inventory[i].NetId);

					if (player.bank4.item[index].netID != 0)
					{
						player.bank4.item[index].stack = inventory[i].Stack;
						player.bank4.item[index].Prefix(inventory[i].PrefixId);
					}
				}
			}
		}

		private static void SyncData(TSPlayer player, ServerSidePlayerData data)
		{
			// Broadcast a player slot packet for each item in the server side inventory to everyone on the server.
			for (int slot = 0; slot < NetItem.TotalSlots; slot++)
			{
				NetItem item = data.Inventory[slot];
				NetMessage.SendData(
					(int)PacketTypes.PlayerSlot,
					-1,
					-1,
					NetworkText.FromLiteral(Lang.GetItemNameValue(item.NetId)),
					player.Index,
					slot,
					item.PrefixId
				);
			}

			// Broadcast appearance and stats to the server
			NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerMana, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerHp, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);

			// Send a player slot packet for each item in the server side inventory to the player joining
			// This is required so that the player actually receives the inventory they're meant to
			for (int slot = 0; slot < NetItem.TotalSlots; slot++)
			{
				NetItem item = data.Inventory[slot];
				NetMessage.SendData(
					(int)PacketTypes.PlayerSlot,
					player.Index,
					-1,
					NetworkText.FromLiteral(Lang.GetItemNameValue(item.NetId)),
					player.Index,
					slot,
					item.PrefixId
				);
			}

			// Sync appearance and stats with the player
			NetMessage.SendData((int)PacketTypes.PlayerInfo, player.Index, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerMana, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerHp, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);


			// Reset player buffs
			for (int k = 0; k < 22; k++)
			{
				player.TPlayer.buffType[k] = 0;
			}

			/*
			 * The following packets are sent twice because the server will not send a packet to a client
			 * if they have not spawned yet if the remoteclient is -1
			 * This is for when players login via uuid or serverpassword instead of via
			 * the login command.
			 */
			NetMessage.SendData((int)PacketTypes.PlayerBuff, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData((int)PacketTypes.PlayerBuff, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);

			NetMessage.SendData((int)PacketTypes.NumberOfAnglerQuestsCompleted, player.Index, -1, NetworkText.Empty, player.Index);
			NetMessage.SendData((int)PacketTypes.NumberOfAnglerQuestsCompleted, -1, -1, NetworkText.Empty, player.Index);

			NetMessage.SendData((int)PacketTypes.RemoveItemOwner, player.Index, -1, NetworkText.Empty, 400);

			// Sync journey research with the player
			if (Main.GameModeInfo.IsJourneyMode)
			{
				var sacrificedItems = TShock.ResearchDatastore.GetSacrificedItems();
				for (int i = 0; i < ItemID.Count; i++)
				{
					var amount = 0;
					if (sacrificedItems.ContainsKey(i))
					{
						amount = sacrificedItems[i];
					}

					var response = NetCreativeUnlocksModule.SerializeItemSacrifice(i, amount);
					NetManager.Instance.SendToClient(response, player.Index);
				}
			}
		}
	}
}
