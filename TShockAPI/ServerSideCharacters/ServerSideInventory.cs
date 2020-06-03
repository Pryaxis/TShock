using System;
using System.Collections.Generic;
using Terraria;
using TShockAPI.Net;

namespace TShockAPI.ServerSideCharacters
{
	/// <summary>
	/// Contains a server side player's inventory
	/// </summary>
	public class ServerSideInventory
	{
		/// <summary>
		/// Array of all items in the player's inventory
		/// </summary>
		internal NetItem[] Items = new NetItem[NetItem.TotalSlots];

		/// <summary>
		/// Gets the item at the given index of the inventory
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public NetItem this[int index] { get => Items[index]; set => Items[index] = value; }

		/// <summary>
		/// Items in the player's main inventory slots
		/// </summary>
		public ArraySegment<NetItem> Inventory =>
			new ArraySegment<NetItem>(Items, NetItem.InventoryGroup.Start, NetItem.InventoryGroup.Count);

		/// <summary>
		/// Items in the player's coin slots
		/// </summary>
		public ArraySegment<NetItem> Coins =>
			new ArraySegment<NetItem>(Items, NetItem.CoinGroup.Start, NetItem.CoinGroup.Count);

		/// <summary>
		/// Items in the player's ammo slots
		/// </summary>
		public ArraySegment<NetItem> Ammo =>
			new ArraySegment<NetItem>(Items, NetItem.AmmoGroup.Start, NetItem.AmmoGroup.Count);

		/// <summary>
		/// Items in the player's held item slot
		/// </summary>
		public ArraySegment<NetItem> HeldItem =>
			new ArraySegment<NetItem>(Items, NetItem.HeldItemGroup.Start, NetItem.HeldItemGroup.Count);

		/// <summary>
		/// Armor equipped by the player
		/// </summary>
		public ArraySegment<NetItem> Armor =>
			new ArraySegment<NetItem>(Items, NetItem.ArmorGroup.Start, NetItem.ArmorGroup.Count);

		/// <summary>
		/// Accessories equipped by the player
		/// </summary>
		public ArraySegment<NetItem> Accessory =>
			new ArraySegment<NetItem>(Items, NetItem.AccessoryGroup.Start, NetItem.AccessoryGroup.Count);

		/// <summary>
		/// Vanity armor equipped by the player
		/// </summary>
		public ArraySegment<NetItem> ArmorVanity =>
			new ArraySegment<NetItem>(Items, NetItem.ArmorVanityGroup.Start, NetItem.ArmorVanityGroup.Count);

		/// <summary>
		/// Vanity accessories equipped by the player
		/// </summary>
		public ArraySegment<NetItem> AccessoryVanity =>
			new ArraySegment<NetItem>(Items, NetItem.AccessoryVanityGroup.Start, NetItem.AccessoryVanityGroup.Count);

		/// <summary>
		/// Armor dyes equipped by the player
		/// </summary>
		public ArraySegment<NetItem> ArmorDye =>
			new ArraySegment<NetItem>(Items, NetItem.ArmorDyeGroup.Start, NetItem.ArmorDyeGroup.Count);

		/// <summary>
		/// Vanity dyes equipped by the player
		/// </summary>
		public ArraySegment<NetItem> AccessoryDye =>
			new ArraySegment<NetItem>(Items, NetItem.AccessoryDyeGroup.Start, NetItem.AccessoryDyeGroup.Count);

		/// <summary>
		/// Miscellanious items equipped by the player. This is the Mount/Hook/etc section of the inventory
		/// </summary>
		public ArraySegment<NetItem> MiscEquips =>
			new ArraySegment<NetItem>(Items, NetItem.MiscEquipGroup.Start, NetItem.MiscEquipGroup.Count);

		/// <summary>
		/// Dyes for the miscellanious items sections equipped by the player
		/// </summary>
		public ArraySegment<NetItem> MiscEquipDyes =>
			new ArraySegment<NetItem>(Items, NetItem.MiscDyeGroup.Start, NetItem.MiscDyeGroup.Count);

		/// <summary>
		/// Items in the player's piggy bank
		/// </summary>
		public ArraySegment<NetItem> PiggyBank =>
			new ArraySegment<NetItem>(Items, NetItem.PiggyGroup.Start, NetItem.PiggyGroup.Count);

		/// <summary>
		/// Items in the player's safe
		/// </summary>
		public ArraySegment<NetItem> SafeBank =>
			new ArraySegment<NetItem>(Items, NetItem.SafeGroup.Start, NetItem.SafeGroup.Count);

		/// <summary>
		/// Items in the player's trash slot
		/// </summary>
		public ArraySegment<NetItem> TrashSlot =>
			new ArraySegment<NetItem>(Items, NetItem.TrashGroup.Start, NetItem.TrashGroup.Count);

		/// <summary>
		/// Items in the player's defender's forge
		/// </summary>
		public ArraySegment<NetItem> ForgeBank =>
			new ArraySegment<NetItem>(Items, NetItem.ForgeGroup.Start, NetItem.ForgeGroup.Count);

		/// <summary>
		/// Items in the player's void bank
		/// </summary>
		public ArraySegment<NetItem> VoidBank =>
			new ArraySegment<NetItem>(Items, NetItem.VoidGroup.Start, NetItem.VoidGroup.Count);

		/// <summary>
		/// Updates an inventory slot with a new item
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="item"></param>
		public void UpdateInventorySlot(int slot, Item item) => UpdateInventorySlot(slot, (NetItem)item);

		/// <summary>
		/// Updates an inventory slot with a new item
		/// </summary>
		/// <param name="slot"></param>
		/// <param name="item"></param>
		public void UpdateInventorySlot(int slot, NetItem item)
		{
			if (slot < 0 || slot > NetItem.TotalSlots)
			{
				return;
			}

			Items[slot] = item;
		}

		/// <summary>
		/// Creates a default inventory using the items listed in <see cref="ServerSideConfig.StartingInventory"/>
		/// </summary>
		/// <returns></returns>
		public static ServerSideInventory CreateDefault()
		{
			ServerSideInventory inventory = new ServerSideInventory();

			// First fill as much of the inventory as possible with items from the SSC Config
			for (int i = 0; i < TShock.ServerSideCharacterConfig.StartingInventory.Count; i++)
			{
				inventory.Items[i] = TShock.ServerSideCharacterConfig.StartingInventory[i];
			}

			// Then fill the rest with empty items
			for (int i = TShock.ServerSideCharacterConfig.StartingInventory.Count; i < NetItem.TotalSlots; i++)
			{
				inventory.Items[i] = new NetItem();
			}

			return inventory;
		}

		/// <summary>
		/// Creates an inventory from an enumerable of NetItems
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public static ServerSideInventory CreateFromNetItems(IEnumerable<NetItem> items)
		{
			ServerSideInventory inventory = new ServerSideInventory();

			int index = 0;
			foreach (NetItem item in items)
			{
				inventory.Items[index] = item;
				index++;
			}

			return inventory;
		}

		/// <summary>
		/// Creates an inventory using the given player's inventory
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static ServerSideInventory CreateFromPlayer(Player player)
		{
			ServerSideInventory inventory = new ServerSideInventory();
			
			for (int i = 0; i < NetItem.TotalSlots; i++)
			{
				if (i <= NetItem.FullInventoryGroup.End)
				{
					//0-58
					inventory[i] = (NetItem)player.inventory[i];
				}
				else if (i <= NetItem.FullEquipmentAndVanityGroup.End)
				{
					//59-78
					var index = i - NetItem.FullEquipmentAndVanityGroup.Start;
					inventory[i] = (NetItem)player.armor[index];
				}
				else if (i <= NetItem.FullArmorAndVanityDyeGroup.End)
				{
					//79-88
					var index = i - NetItem.FullArmorAndVanityDyeGroup.Start;
					inventory[i] = (NetItem)player.dye[index];
				}
				else if (i <= NetItem.MiscEquipGroup.End)
				{
					//89-93
					var index = i - NetItem.MiscEquipGroup.Start;
					inventory[i] = (NetItem)player.miscEquips[index];
				}
				else if (i <= NetItem.MiscDyeGroup.End)
				{
					//93-98
					var index = i - NetItem.MiscDyeGroup.Start;
					inventory[i] = (NetItem)player.miscDyes[index];
				}
				else if (i <= NetItem.PiggyGroup.End)
				{
					//99-138
					var index = i - NetItem.PiggyGroup.Start;
					inventory[i] = (NetItem)player.bank.item[index];
				}
				else if (i <= NetItem.SafeGroup.End)
				{
					//139-178
					var index = i - NetItem.SafeGroup.Start;
					inventory[i] = (NetItem)player.bank2.item[index];
				}
				else if (i <= NetItem.TrashGroup.End)
				{
					//179
					inventory[i] = (NetItem)player.trashItem;
				}
				else if (i <= NetItem.ForgeGroup.End)
				{
					//180-219
					var index = i - NetItem.ForgeGroup.Start;
					inventory[i] = (NetItem)player.bank3.item[index];
				}
				else
				{
					//220-259
					var index = i - NetItem.VoidGroup.Start;
					inventory[i] = (NetItem)player.bank4.item[index];
				}
			}

			return inventory;
		}
	}
}
