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
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Terraria;

namespace TShockAPI.Net
{
	/// <summary>
	/// Helper class to group inventory indexes.
	/// Contains the starting index of an inventory group, and the number of items in the group
	/// </summary>
	public class InventoryGrouping
	{
		/// <summary>
		/// The starting index of the inventory group
		/// </summary>
		public int Start;
		/// <summary>
		/// The number of items in the group
		/// </summary>
		public int Count;

		/// <summary>
		/// Creates a new grouping with the given start and count
		/// </summary>
		/// <param name="start"></param>
		/// <param name="count"></param>
		public InventoryGrouping(int start, int count)
		{
			Start = start;
			Count = count;
		}

		/// <summary>
		/// Creates a new grouping using the given previous grouping to determine the start of this grouping
		/// </summary>
		/// <param name="previous"></param>
		/// <param name="count"></param>
		public InventoryGrouping(InventoryGrouping previous, int count)
		{
			Start = previous.Start + previous.Count;
			Count = count;
		}
	}

	/// <summary>
	/// Represents an item.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public struct NetItem
	{
		/// <summary>
		/// The maximum number of slots in the main inventory. This does not include coins, ammo, or the held item
		/// </summary>
		public static readonly int InventorySlots = 50;
		/// <summary>
		/// The maximum number of coin slots
		/// </summary>
		public static readonly int CoinSlots = 4;
		/// <summary>
		/// The maximum number of ammo slots
		/// </summary>
		public static readonly int AmmoSlots = 4;
		/// <summary>
		/// The maximum number of held item slots
		/// </summary>
		public static readonly int HeldItemSlots = 1;

		/// <summary>
		/// The maximum number of armor slots
		/// </summary>
		public static readonly int ArmorSlots = 3;
		/// <summary>
		/// The maximum number of accessory slots
		/// </summary>
		public static readonly int AccessorySlots = 7;

		/// <summary>
		/// The maximum number of armor vanity slots
		/// </summary>
		public static readonly int ArmorVanitySlots = ArmorSlots;
		/// <summary>
		/// The maximum number of accessory vanity slots
		/// </summary>
		public static readonly int AccessoryVanitySlots = AccessorySlots;

		/// <summary>
		/// The maximum number of armor dye slots
		/// </summary>
		public static readonly int ArmorDyeSlots = ArmorVanitySlots;
		/// <summary>
		/// The maximum number of accessory dye slots
		/// </summary>
		public static readonly int AccessoryDyeSlots = AccessoryVanitySlots;

		/// <summary>
		/// The maximum number of misc equipment slots. This is the mount/hook/etc equipment
		/// </summary>
		public static readonly int MiscEquipSlots = 5;
		/// <summary>
		/// The maximum number of misc dye slots
		/// </summary>
		public static readonly int MiscDyeSlots = MiscEquipSlots;

		/// <summary>
		/// The maximum number of slots in a piggy bank
		/// </summary>
		public static readonly int PiggySlots = 40;

		/// <summary>
		/// The maximum number of slots in the trash can
		/// </summary>
		public static readonly int TrashSlots = 1;

		/// <summary>
		/// The maximum number of slots in a safe
		/// </summary>
		public static readonly int SafeSlots = PiggySlots;

		/// <summary>
		/// The maximum number of slots in a defender's forge
		/// </summary>
		public static readonly int ForgeSlots = SafeSlots;

		/// <summary>
		/// The maximum numbert of slots in a void bank
		/// </summary>
		public static readonly int VoidSlots = ForgeSlots;

		/// <summary>
		/// The total number of slots available across the entire inventory
		/// </summary>
		public static readonly int MaxInventory =
			InventorySlots + CoinSlots + AmmoSlots + HeldItemSlots +
			ArmorSlots + AccessorySlots +
			ArmorVanitySlots + AccessoryVanitySlots +
			ArmorDyeSlots + AccessoryDyeSlots +
			MiscEquipSlots + MiscDyeSlots +
			PiggySlots +
			TrashSlots +
			SafeSlots +
			ForgeSlots +
			VoidSlots;

		/// <summary>
		/// Groups the main inventory slots
		/// </summary>
		public static readonly InventoryGrouping MainInventoryGroup = new InventoryGrouping(0, InventorySlots);
		/// <summary>
		/// Groups the coin slots
		/// </summary>
		public static readonly InventoryGrouping CoinGroup = new InventoryGrouping(MainInventoryGroup, CoinSlots);
		/// <summary>
		/// Groups the ammo slots
		/// </summary>
		public static readonly InventoryGrouping AmmoGroup = new InventoryGrouping(CoinGroup, AmmoSlots);
		/// <summary>
		/// Groups the held item slot
		/// </summary>
		public static readonly InventoryGrouping HeldItemGroup = new InventoryGrouping(AmmoGroup, HeldItemSlots);

		/// <summary>
		/// Groups the armor slots
		/// </summary>
		public static readonly InventoryGrouping ArmorGroup = new InventoryGrouping(HeldItemGroup, ArmorSlots);
		/// <summary>
		/// Groups the accessory slots
		/// </summary>
		public static readonly InventoryGrouping AccessoryGroup = new InventoryGrouping(ArmorGroup, AccessorySlots);

		/// <summary>
		/// Groups the armor vanity slots
		/// </summary>
		public static readonly InventoryGrouping ArmorVanityGroup = new InventoryGrouping(AccessoryGroup, ArmorVanitySlots);
		/// <summary>
		/// Groups the accessory vanity slots
		/// </summary>
		public static readonly InventoryGrouping AccessoryVanityGroup = new InventoryGrouping(ArmorVanityGroup, AccessoryVanitySlots);

		/// <summary>
		/// Groups the armor dye slots
		/// </summary>
		public static readonly InventoryGrouping ArmorDyeGroup = new InventoryGrouping(AccessoryVanityGroup, ArmorDyeSlots);
		/// <summary>
		/// Groups the accessory dye slots
		/// </summary>
		public static readonly InventoryGrouping AccessoryDyeGroup = new InventoryGrouping(ArmorDyeGroup, AccessoryDyeSlots);

		/// <summary>
		/// Groups the misc equipment slots
		/// </summary>
		public static readonly InventoryGrouping MiscEquipGroup = new InventoryGrouping(AccessoryDyeGroup, MiscEquipSlots);
		/// <summary>
		/// Groups the misc dye slots
		/// </summary>
		public static readonly InventoryGrouping MiscDyeGroup = new InventoryGrouping(MiscEquipGroup, MiscDyeSlots);

		/// <summary>
		/// Groups the piggy bank slots
		/// </summary>
		public static readonly InventoryGrouping PiggyGroup = new InventoryGrouping(MiscDyeGroup, PiggySlots);

		/// <summary>
		/// Groups the trash item slot
		/// </summary>
		public static readonly InventoryGrouping TrashGroup = new InventoryGrouping(PiggyGroup, TrashSlots);

		/// <summary>
		/// Groups the safe item slots
		/// </summary>
		public static readonly InventoryGrouping SafeGroup = new InventoryGrouping(TrashGroup, SafeSlots);

		/// <summary>
		/// Groups the defender's forge item slots
		/// </summary>
		public static readonly InventoryGrouping ForgeGroup = new InventoryGrouping(SafeGroup, ForgeSlots);

		/// <summary>
		/// Groups the void bank item slots
		/// </summary>
		public static readonly InventoryGrouping VoidGroup = new InventoryGrouping(ForgeGroup, VoidSlots);


		[JsonProperty("netID")]
		private int _netId;
		[JsonProperty("prefix")]
		private byte _prefixId;
		[JsonProperty("stack")]
		private int _stack;

		/// <summary>
		/// Gets the net ID.
		/// </summary>
		public int NetId
		{
			get { return _netId; }
		}

		/// <summary>
		/// Gets the prefix.
		/// </summary>
		public byte PrefixId
		{
			get { return _prefixId; }
		}

		/// <summary>
		/// Gets the stack.
		/// </summary>
		public int Stack
		{
			get { return _stack; }
		}

		/// <summary>
		/// Creates a new <see cref="NetItem"/>.
		/// </summary>
		/// <param name="netId">The net ID.</param>
		/// <param name="stack">The stack.</param>
		/// <param name="prefixId">The prefix ID.</param>
		public NetItem(int netId, int stack, byte prefixId)
		{
			_netId = netId;
			_stack = stack;
			_prefixId = prefixId;
		}

		/// <summary>
		/// Converts the <see cref="NetItem"/> to a string.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0},{1},{2}", _netId, _stack, _prefixId);
		}

		/// <summary>
		/// Converts a string into a <see cref="NetItem"/>.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <exception cref="ArgumentNullException"/>
		/// <exception cref="FormatException"/>
		/// <returns></returns>
		public static NetItem Parse(string str)
		{
			if (str == null)
				throw new ArgumentNullException("str");

			string[] comp = str.Split(',');
			if (comp.Length != 3)
				throw new FormatException("String does not contain three sections.");

			int netId = Int32.Parse(comp[0]);
			int stack = Int32.Parse(comp[1]);
			byte prefixId = Byte.Parse(comp[2]);

			return new NetItem(netId, stack, prefixId);
		}

		/// <summary>
		/// Converts an <see cref="Item"/> into a <see cref="NetItem"/>.
		/// </summary>
		/// <param name="item">The <see cref="Item"/>.</param>
		/// <returns></returns>
		public static explicit operator NetItem(Item item)
		{
			return item == null
				? new NetItem()
				: new NetItem(item.netID, item.stack, item.prefix);
		}
	}
}
