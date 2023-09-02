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

using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;
using Terraria.Localization;
using Terraria.GameContent.NetModules;
using Terraria.Net;
using Terraria.ID;
using System;

namespace TShockAPI
{
	public class PlayerData
	{
		public NetItem[] inventory = new NetItem[NetItem.MaxInventory];
		public int health = TShock.ServerSideCharacterConfig.Settings.StartingHealth;
		public int maxHealth = TShock.ServerSideCharacterConfig.Settings.StartingHealth;
		public int mana = TShock.ServerSideCharacterConfig.Settings.StartingMana;
		public int maxMana = TShock.ServerSideCharacterConfig.Settings.StartingMana;
		public bool exists;
		public int spawnX = -1;
		public int spawnY = -1;
		public int? extraSlot;
		public int? skinVariant;
		public int? hair;
		public byte hairDye;
		public Color? hairColor;
		public Color? pantsColor;
		public Color? shirtColor;
		public Color? underShirtColor;
		public Color? shoeColor;
		public Color? skinColor;
		public Color? eyeColor;
		public bool[] hideVisuals;
		public int questsCompleted;
		public int usingBiomeTorches;
		public int happyFunTorchTime;
		public int unlockedBiomeTorches;
		public int currentLoadoutIndex;
		public int ateArtisanBread;
		public int usedAegisCrystal;
		public int usedAegisFruit;
		public int usedArcaneCrystal;
		public int usedGalaxyPearl;
		public int usedGummyWorm;
		public int usedAmbrosia;
		public int unlockedSuperCart;
		public int enabledSuperCart;

		/// <summary>
		/// Sets the default values for the inventory.
		/// </summary>
		[Obsolete("The player argument is not used.")]
		public PlayerData(TSPlayer player) : this(true) { }

		/// <summary>
		/// Sets the default values for the inventory.
		/// </summary>
		/// <param name="includingStarterInventory">Is it necessary to load items from TShock's config</param>
		public PlayerData(bool includingStarterInventory = true)
		{
			for (int i = 0; i < NetItem.MaxInventory; i++)
				this.inventory[i] = new NetItem();

			if (includingStarterInventory)
				for (int i = 0; i < TShock.ServerSideCharacterConfig.Settings.StartingInventory.Count; i++)
				{
					var item = TShock.ServerSideCharacterConfig.Settings.StartingInventory[i];
					StoreSlot(i, item.NetId, item.PrefixId, item.Stack);
				}
		}

		/// <summary>
		/// Stores an item at the specific storage slot
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="netID"></param>
		/// <param name="prefix"></param>
		/// <param name="stack"></param>
		public void StoreSlot(int slot, int netID, byte prefix, int stack)
		{
			StoreSlot(slot, new NetItem(netID, stack, prefix));
		}

		/// <summary>
		/// Stores an item at the specific storage slot
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="item"></param>
		public void StoreSlot(int slot, NetItem item)
		{
			if (slot > (this.inventory.Length - 1) || slot < 0) //if the slot is out of range then dont save
			{
				return;
			}

			this.inventory[slot] = item;
		}

		/// <summary>
		/// Copies a characters data to this object
		/// </summary>
		/// <param name="player"></param>
		public void CopyCharacter(TSPlayer player)
		{
			this.health = player.TPlayer.statLife > 0 ? player.TPlayer.statLife : 1;
			this.maxHealth = player.TPlayer.statLifeMax;
			this.mana = player.TPlayer.statMana;
			this.maxMana = player.TPlayer.statManaMax;
			if (player.sX > 0 && player.sY > 0)
			{
				this.spawnX = player.sX;
				this.spawnY = player.sY;
			}
			else
			{
				this.spawnX = player.TPlayer.SpawnX;
				this.spawnY = player.TPlayer.SpawnY;
			}
			extraSlot = player.TPlayer.extraAccessory ? 1 : 0;
			this.skinVariant = player.TPlayer.skinVariant;
			this.hair = player.TPlayer.hair;
			this.hairDye = player.TPlayer.hairDye;
			this.hairColor = player.TPlayer.hairColor;
			this.pantsColor = player.TPlayer.pantsColor;
			this.shirtColor = player.TPlayer.shirtColor;
			this.underShirtColor = player.TPlayer.underShirtColor;
			this.shoeColor = player.TPlayer.shoeColor;
			this.hideVisuals = player.TPlayer.hideVisibleAccessory;
			this.skinColor = player.TPlayer.skinColor;
			this.eyeColor = player.TPlayer.eyeColor;
			this.questsCompleted = player.TPlayer.anglerQuestsFinished;
			this.usingBiomeTorches = player.TPlayer.UsingBiomeTorches ? 1 : 0;
			this.happyFunTorchTime = player.TPlayer.happyFunTorchTime ? 1 : 0;
			this.unlockedBiomeTorches = player.TPlayer.unlockedBiomeTorches ? 1 : 0;
			this.currentLoadoutIndex = player.TPlayer.CurrentLoadoutIndex;
			this.ateArtisanBread = player.TPlayer.ateArtisanBread ? 1 : 0;
			this.usedAegisCrystal = player.TPlayer.usedAegisCrystal ? 1 : 0;
			this.usedAegisFruit = player.TPlayer.usedAegisFruit ? 1 : 0;
			this.usedArcaneCrystal = player.TPlayer.usedArcaneCrystal ? 1 : 0;
			this.usedGalaxyPearl = player.TPlayer.usedGalaxyPearl ? 1 : 0;
			this.usedGummyWorm = player.TPlayer.usedGummyWorm ? 1 : 0;
			this.usedAmbrosia = player.TPlayer.usedAmbrosia ? 1 : 0;
			this.unlockedSuperCart = player.TPlayer.unlockedSuperCart ? 1 : 0;
			this.enabledSuperCart = player.TPlayer.enabledSuperCart ? 1 : 0;

			Item[] inventory = player.TPlayer.inventory;
			Item[] armor = player.TPlayer.armor;
			Item[] dye = player.TPlayer.dye;
			Item[] miscEqups = player.TPlayer.miscEquips;
			Item[] miscDyes = player.TPlayer.miscDyes;
			Item[] piggy = player.TPlayer.bank.item;
			Item[] safe = player.TPlayer.bank2.item;
			Item[] forge = player.TPlayer.bank3.item;
			Item[] voidVault = player.TPlayer.bank4.item;
			Item trash = player.TPlayer.trashItem;
			Item[] loadout1Armor = player.TPlayer.Loadouts[0].Armor;
			Item[] loadout1Dye = player.TPlayer.Loadouts[0].Dye;
			Item[] loadout2Armor = player.TPlayer.Loadouts[1].Armor;
			Item[] loadout2Dye = player.TPlayer.Loadouts[1].Dye;
			Item[] loadout3Armor = player.TPlayer.Loadouts[2].Armor;
			Item[] loadout3Dye = player.TPlayer.Loadouts[2].Dye;

			for (int i = 0; i < NetItem.MaxInventory; i++)
			{
				if (i < NetItem.InventoryIndex.Item2)
				{
					//0-58
					this.inventory[i] = (NetItem)inventory[i];
				}
				else if (i < NetItem.ArmorIndex.Item2)
				{
					//59-78
					var index = i - NetItem.ArmorIndex.Item1;
					this.inventory[i] = (NetItem)armor[index];
				}
				else if (i < NetItem.DyeIndex.Item2)
				{
					//79-88
					var index = i - NetItem.DyeIndex.Item1;
					this.inventory[i] = (NetItem)dye[index];
				}
				else if (i < NetItem.MiscEquipIndex.Item2)
				{
					//89-93
					var index = i - NetItem.MiscEquipIndex.Item1;
					this.inventory[i] = (NetItem)miscEqups[index];
				}
				else if (i < NetItem.MiscDyeIndex.Item2)
				{
					//93-98
					var index = i - NetItem.MiscDyeIndex.Item1;
					this.inventory[i] = (NetItem)miscDyes[index];
				}
				else if (i < NetItem.PiggyIndex.Item2)
				{
					//98-138
					var index = i - NetItem.PiggyIndex.Item1;
					this.inventory[i] = (NetItem)piggy[index];
				}
				else if (i < NetItem.SafeIndex.Item2)
				{
					//138-178
					var index = i - NetItem.SafeIndex.Item1;
					this.inventory[i] = (NetItem)safe[index];
				}
				else if (i < NetItem.TrashIndex.Item2)
				{
					//179-219
					this.inventory[i] = (NetItem)trash;
				}
				else if (i < NetItem.ForgeIndex.Item2)
				{
					//220
					var index = i - NetItem.ForgeIndex.Item1;
					this.inventory[i] = (NetItem)forge[index];
				}
				else if(i < NetItem.VoidIndex.Item2)
				{
					//220
					var index = i - NetItem.VoidIndex.Item1;
					this.inventory[i] = (NetItem)voidVault[index];
				}
				else if(i < NetItem.Loadout1Armor.Item2)
				{
					var index = i - NetItem.Loadout1Armor.Item1;
					this.inventory[i] = (NetItem)loadout1Armor[index];
				}
				else if(i < NetItem.Loadout1Dye.Item2)
				{
					var index = i - NetItem.Loadout1Dye.Item1;
					this.inventory[i] = (NetItem)loadout1Dye[index];
				}
				else if(i < NetItem.Loadout2Armor.Item2)
				{
					var index = i - NetItem.Loadout2Armor.Item1;
					this.inventory[i] = (NetItem)loadout2Armor[index];
				}
				else if(i < NetItem.Loadout2Dye.Item2)
				{
					var index = i - NetItem.Loadout2Dye.Item1;
					this.inventory[i] = (NetItem)loadout2Dye[index];
				}
				else if(i < NetItem.Loadout3Armor.Item2)
				{
					var index = i - NetItem.Loadout3Armor.Item1;
					this.inventory[i] = (NetItem)loadout3Armor[index];
				}
				else if(i < NetItem.Loadout3Dye.Item2)
				{
					var index = i - NetItem.Loadout3Dye.Item1;
					this.inventory[i] = (NetItem)loadout3Dye[index];
				}
			}
		}

		/// <summary>
		/// Restores a player's character to the state stored in the database
		/// </summary>
		/// <param name="player"></param>
		public void RestoreCharacter(TSPlayer player)
		{
			// Start ignoring SSC-related packets! This is critical so that we don't send or receive dirty data!
			player.IgnoreSSCPackets = true;

			player.TPlayer.statLife = this.health;
			player.TPlayer.statLifeMax = this.maxHealth;
			player.TPlayer.statMana = this.maxMana;
			player.TPlayer.statManaMax = this.maxMana;
			player.TPlayer.SpawnX = this.spawnX;
			player.TPlayer.SpawnY = this.spawnY;
			player.sX = this.spawnX;
			player.sY = this.spawnY;
			player.TPlayer.hairDye = this.hairDye;
			player.TPlayer.anglerQuestsFinished = this.questsCompleted;
			player.TPlayer.UsingBiomeTorches = this.usingBiomeTorches == 1;
			player.TPlayer.happyFunTorchTime = this.happyFunTorchTime == 1;
			player.TPlayer.unlockedBiomeTorches = this.unlockedBiomeTorches == 1;
			player.TPlayer.CurrentLoadoutIndex = this.currentLoadoutIndex;
			player.TPlayer.ateArtisanBread = this.ateArtisanBread == 1;
			player.TPlayer.usedAegisCrystal = this.usedAegisCrystal == 1;
			player.TPlayer.usedAegisFruit = this.usedAegisFruit == 1;
			player.TPlayer.usedArcaneCrystal = this.usedArcaneCrystal == 1;
			player.TPlayer.usedGalaxyPearl = this.usedGalaxyPearl == 1;
			player.TPlayer.usedGummyWorm = this.usedGummyWorm == 1;
			player.TPlayer.usedAmbrosia = this.usedAmbrosia == 1;
			player.TPlayer.unlockedSuperCart = this.unlockedSuperCart == 1;
			player.TPlayer.enabledSuperCart = this.enabledSuperCart == 1;

			if (extraSlot != null)
				player.TPlayer.extraAccessory = extraSlot.Value == 1 ? true : false;
			if (this.skinVariant != null)
				player.TPlayer.skinVariant = this.skinVariant.Value;
			if (this.hair != null)
				player.TPlayer.hair = this.hair.Value;
			if (this.hairColor != null)
				player.TPlayer.hairColor = this.hairColor.Value;
			if (this.pantsColor != null)
				player.TPlayer.pantsColor = this.pantsColor.Value;
			if (this.shirtColor != null)
				player.TPlayer.shirtColor = this.shirtColor.Value;
			if (this.underShirtColor != null)
				player.TPlayer.underShirtColor = this.underShirtColor.Value;
			if (this.shoeColor != null)
				player.TPlayer.shoeColor = this.shoeColor.Value;
			if (this.skinColor != null)
				player.TPlayer.skinColor = this.skinColor.Value;
			if (this.eyeColor != null)
				player.TPlayer.eyeColor = this.eyeColor.Value;

			if (this.hideVisuals != null)
				player.TPlayer.hideVisibleAccessory = this.hideVisuals;
			else
				player.TPlayer.hideVisibleAccessory = new bool[player.TPlayer.hideVisibleAccessory.Length];

			for (int i = 0; i < NetItem.MaxInventory; i++)
			{
				if (i < NetItem.InventoryIndex.Item2)
				{
					//0-58
					player.TPlayer.inventory[i].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.inventory[i].netID != 0)
					{
						player.TPlayer.inventory[i].stack = this.inventory[i].Stack;
						player.TPlayer.inventory[i].prefix = this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.ArmorIndex.Item2)
				{
					//59-78
					var index = i - NetItem.ArmorIndex.Item1;
					player.TPlayer.armor[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.armor[index].netID != 0)
					{
						player.TPlayer.armor[index].stack = this.inventory[i].Stack;
						player.TPlayer.armor[index].prefix = (byte)this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.DyeIndex.Item2)
				{
					//79-88
					var index = i - NetItem.DyeIndex.Item1;
					player.TPlayer.dye[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.dye[index].netID != 0)
					{
						player.TPlayer.dye[index].stack = this.inventory[i].Stack;
						player.TPlayer.dye[index].prefix = (byte)this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.MiscEquipIndex.Item2)
				{
					//89-93
					var index = i - NetItem.MiscEquipIndex.Item1;
					player.TPlayer.miscEquips[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.miscEquips[index].netID != 0)
					{
						player.TPlayer.miscEquips[index].stack = this.inventory[i].Stack;
						player.TPlayer.miscEquips[index].prefix = (byte)this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.MiscDyeIndex.Item2)
				{
					//93-98
					var index = i - NetItem.MiscDyeIndex.Item1;
					player.TPlayer.miscDyes[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.miscDyes[index].netID != 0)
					{
						player.TPlayer.miscDyes[index].stack = this.inventory[i].Stack;
						player.TPlayer.miscDyes[index].prefix = (byte)this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.PiggyIndex.Item2)
				{
					//98-138
					var index = i - NetItem.PiggyIndex.Item1;
					player.TPlayer.bank.item[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.bank.item[index].netID != 0)
					{
						player.TPlayer.bank.item[index].stack = this.inventory[i].Stack;
						player.TPlayer.bank.item[index].prefix = (byte)this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.SafeIndex.Item2)
				{
					//138-178
					var index = i - NetItem.SafeIndex.Item1;
					player.TPlayer.bank2.item[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.bank2.item[index].netID != 0)
					{
						player.TPlayer.bank2.item[index].stack = this.inventory[i].Stack;
						player.TPlayer.bank2.item[index].prefix = (byte)this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.TrashIndex.Item2)
				{
					//179-219
					var index = i - NetItem.TrashIndex.Item1;
					player.TPlayer.trashItem.netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.trashItem.netID != 0)
					{
						player.TPlayer.trashItem.stack = this.inventory[i].Stack;
						player.TPlayer.trashItem.prefix = (byte)this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.ForgeIndex.Item2)
				{
					//220
					var index = i - NetItem.ForgeIndex.Item1;
					player.TPlayer.bank3.item[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.bank3.item[index].netID != 0)
					{
						player.TPlayer.bank3.item[index].stack = this.inventory[i].Stack;
						player.TPlayer.bank3.item[index].Prefix((byte)this.inventory[i].PrefixId);
					}
				}
				else if (i < NetItem.VoidIndex.Item2)
				{
					//260
					var index = i - NetItem.VoidIndex.Item1;
					player.TPlayer.bank4.item[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.bank4.item[index].netID != 0)
					{
						player.TPlayer.bank4.item[index].stack = this.inventory[i].Stack;
						player.TPlayer.bank4.item[index].Prefix((byte)this.inventory[i].PrefixId);
					}
				}
				else if (i < NetItem.Loadout1Armor.Item2)
				{
					var index = i - NetItem.Loadout1Armor.Item1;
					player.TPlayer.Loadouts[0].Armor[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.Loadouts[0].Armor[index].netID != 0)
					{
						player.TPlayer.Loadouts[0].Armor[index].stack = this.inventory[i].Stack;
						player.TPlayer.Loadouts[0].Armor[index].Prefix((byte)this.inventory[i].PrefixId);
					}
				}
				else if (i < NetItem.Loadout1Dye.Item2)
				{
					var index = i - NetItem.Loadout1Dye.Item1;
					player.TPlayer.Loadouts[0].Dye[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.Loadouts[0].Dye[index].netID != 0)
					{
						player.TPlayer.Loadouts[0].Dye[index].stack = this.inventory[i].Stack;
						player.TPlayer.Loadouts[0].Dye[index].Prefix((byte)this.inventory[i].PrefixId);
					}
				}
				else if (i < NetItem.Loadout2Armor.Item2)
				{
					var index = i - NetItem.Loadout2Armor.Item1;
					player.TPlayer.Loadouts[1].Armor[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.Loadouts[1].Armor[index].netID != 0)
					{
						player.TPlayer.Loadouts[1].Armor[index].stack = this.inventory[i].Stack;
						player.TPlayer.Loadouts[1].Armor[index].Prefix((byte)this.inventory[i].PrefixId);
					}
				}
				else if (i < NetItem.Loadout2Dye.Item2)
				{
					var index = i - NetItem.Loadout2Dye.Item1;
					player.TPlayer.Loadouts[1].Dye[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.Loadouts[1].Dye[index].netID != 0)
					{
						player.TPlayer.Loadouts[1].Dye[index].stack = this.inventory[i].Stack;
						player.TPlayer.Loadouts[1].Dye[index].Prefix((byte)this.inventory[i].PrefixId);
					}
				}
				else if (i < NetItem.Loadout3Armor.Item2)
				{
					var index = i - NetItem.Loadout3Armor.Item1;
					player.TPlayer.Loadouts[2].Armor[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.Loadouts[2].Armor[index].netID != 0)
					{
						player.TPlayer.Loadouts[2].Armor[index].stack = this.inventory[i].Stack;
						player.TPlayer.Loadouts[2].Armor[index].Prefix((byte)this.inventory[i].PrefixId);
					}
				}
				else if (i < NetItem.Loadout3Dye.Item2)
				{
					var index = i - NetItem.Loadout3Dye.Item1;
					player.TPlayer.Loadouts[2].Dye[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.Loadouts[2].Dye[index].netID != 0)
					{
						player.TPlayer.Loadouts[2].Dye[index].stack = this.inventory[i].Stack;
						player.TPlayer.Loadouts[2].Dye[index].Prefix((byte)this.inventory[i].PrefixId);
					}
				}
			}

			// Just like in MessageBuffer when the client receives a ContinueConnecting, let's sync the CurrentLoadoutIndex _before_ any of
			// the items.
			// This is sent to everyone BUT this player, and then ONLY this player. When using UUID login, it is too soon for the server to
			// broadcast packets to this client.
			NetMessage.SendData((int)PacketTypes.SyncLoadout, remoteClient: player.Index, number: player.Index, number2: player.TPlayer.CurrentLoadoutIndex);
			NetMessage.SendData((int)PacketTypes.SyncLoadout, ignoreClient: player.Index, number: player.Index, number2: player.TPlayer.CurrentLoadoutIndex);

			float slot = 0f;
			for (int k = 0; k < NetItem.InventorySlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].inventory[k].Name), player.Index, slot, (float)Main.player[player.Index].inventory[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.ArmorSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].armor[k].Name), player.Index, slot, (float)Main.player[player.Index].armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.DyeSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].dye[k].Name), player.Index, slot, (float)Main.player[player.Index].dye[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.MiscEquipSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].miscEquips[k].Name), player.Index, slot, (float)Main.player[player.Index].miscEquips[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.MiscDyeSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].miscDyes[k].Name), player.Index, slot, (float)Main.player[player.Index].miscDyes[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.PiggySlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank.item[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.SafeSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank2.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank2.item[k].prefix);
				slot++;
			}
			NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].trashItem.Name), player.Index, slot++, (float)Main.player[player.Index].trashItem.prefix);
			for (int k = 0; k < NetItem.ForgeSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank3.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank3.item[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.VoidSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].bank4.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank4.item[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[0].Armor[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[0].Armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[0].Dye[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[0].Dye[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[1].Armor[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[1].Armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[1].Dye[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[1].Dye[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[2].Armor[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[2].Armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
			{
				NetMessage.SendData(5, -1, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[1].Dye[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[2].Dye[k].prefix);
				slot++;
			}


			NetMessage.SendData(4, -1, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(42, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(16, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);

			slot = 0f;
			for (int k = 0; k < NetItem.InventorySlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].inventory[k].Name), player.Index, slot, (float)Main.player[player.Index].inventory[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.ArmorSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].armor[k].Name), player.Index, slot, (float)Main.player[player.Index].armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.DyeSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].dye[k].Name), player.Index, slot, (float)Main.player[player.Index].dye[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.MiscEquipSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].miscEquips[k].Name), player.Index, slot, (float)Main.player[player.Index].miscEquips[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.MiscDyeSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].miscDyes[k].Name), player.Index, slot, (float)Main.player[player.Index].miscDyes[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.PiggySlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank.item[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.SafeSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank2.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank2.item[k].prefix);
				slot++;
			}
			NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].trashItem.Name), player.Index, slot++, (float)Main.player[player.Index].trashItem.prefix);
			for (int k = 0; k < NetItem.ForgeSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank3.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank3.item[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.VoidSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].bank4.item[k].Name), player.Index, slot, (float)Main.player[player.Index].bank4.item[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[0].Armor[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[0].Armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[0].Dye[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[0].Dye[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[1].Armor[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[1].Armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[1].Dye[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[1].Dye[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutArmorSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[2].Armor[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[2].Armor[k].prefix);
				slot++;
			}
			for (int k = 0; k < NetItem.LoadoutDyeSlots; k++)
			{
				NetMessage.SendData(5, player.Index, -1, NetworkText.FromLiteral(Main.player[player.Index].Loadouts[2].Dye[k].Name), player.Index, slot, (float)Main.player[player.Index].Loadouts[2].Dye[k].prefix);
				slot++;
			}



			NetMessage.SendData(4, player.Index, -1, NetworkText.FromLiteral(player.Name), player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(42, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(16, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);

			for (int k = 0; k < Player.maxBuffs; k++)
			{
				player.TPlayer.buffType[k] = 0;
			}

			/*
			 * The following packets are sent twice because the server will not send a packet to a client
			 * if they have not spawned yet if the remoteclient is -1
			 * This is for when players login via uuid or serverpassword instead of via
			 * the login command.
			 */
			NetMessage.SendData(50, -1, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(50, player.Index, -1, NetworkText.Empty, player.Index, 0f, 0f, 0f, 0);

			NetMessage.SendData(76, player.Index, -1, NetworkText.Empty, player.Index);
			NetMessage.SendData(76, -1, -1, NetworkText.Empty, player.Index);

			NetMessage.SendData(39, player.Index, -1, NetworkText.Empty, 400);

			if (Main.GameModeInfo.IsJourneyMode)
			{
				var sacrificedItems = TShock.ResearchDatastore.GetSacrificedItems();
				for(int i = 0; i < ItemID.Count; i++)
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
