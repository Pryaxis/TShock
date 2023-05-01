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

namespace TShockAPI
{
	/// <summary>
	/// Represents an item.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public struct NetItem
	{
		/// <summary>
		/// 40 - The number of slots in a piggy bank
		/// </summary>
		public static readonly int PiggySlots = 40;

		/// <summary>
		/// 40 - The number of slots in a safe
		/// </summary>
		public static readonly int SafeSlots = PiggySlots;

		/// <summary>
		/// 40 - The number of slots in a forge
		/// </summary>
		public static readonly int ForgeSlots = SafeSlots;

		/// <summary>
		/// 40 - The number of slots in a void vault
		/// </summary>
		public static readonly int VoidSlots = ForgeSlots;

		/// <summary>
		/// 59 - The size of the player's inventory (inventory, coins, ammo, held item)
		/// </summary>
		public static readonly int InventorySlots = 59;

		/// <summary>
		/// 20 - The number of armor slots.
		/// </summary>
		public static readonly int ArmorSlots = 20;

		/// <summary>
		/// 5 - The number of other equippable items
		/// </summary>
		public static readonly int MiscEquipSlots = 5;

		/// <summary>
		/// 10 - The number of dye slots.
		/// </summary>
		public static readonly int DyeSlots = 10;

		/// <summary>
		/// 5 - The number of other dye slots (for <see cref="MiscEquipSlots"/>)
		/// </summary>
		public static readonly int MiscDyeSlots = MiscEquipSlots;

		/// <summary>
		/// 1 - The number of trash can slots.
		/// </summary>
		public static readonly int TrashSlots = 1;

		/// <summary>
		/// The number of armor slots in a loadout.
		/// </summary>
		public static readonly int LoadoutArmorSlots = ArmorSlots;

		/// <summary>
		/// The number of dye slots in a loadout.
		/// </summary>
		public static readonly int LoadoutDyeSlots = DyeSlots;

		/// <summary>
		/// 180 - The inventory size (inventory, held item, armour, dies, coins, ammo, piggy, safe, and trash)
		/// </summary>
		public static readonly int MaxInventory = InventorySlots + ArmorSlots + DyeSlots + MiscEquipSlots + MiscDyeSlots + PiggySlots +
		                                          SafeSlots + ForgeSlots + VoidSlots + TrashSlots + (LoadoutArmorSlots * 3) +
		                                          (LoadoutDyeSlots * 3);

		public static readonly Tuple<int, int> InventoryIndex = new Tuple<int, int>(0, InventorySlots);
		public static readonly Tuple<int, int> ArmorIndex = new Tuple<int, int>(InventoryIndex.Item2, InventoryIndex.Item2 + ArmorSlots);
		public static readonly Tuple<int, int> DyeIndex = new Tuple<int, int>(ArmorIndex.Item2, ArmorIndex.Item2 + DyeSlots);
		public static readonly Tuple<int, int> MiscEquipIndex = new Tuple<int, int>(DyeIndex.Item2, DyeIndex.Item2 + MiscEquipSlots);
		public static readonly Tuple<int, int> MiscDyeIndex = new Tuple<int, int>(MiscEquipIndex.Item2, MiscEquipIndex.Item2 + MiscDyeSlots);
		public static readonly Tuple<int, int> PiggyIndex = new Tuple<int, int>(MiscDyeIndex.Item2, MiscDyeIndex.Item2 + PiggySlots);
		public static readonly Tuple<int, int> SafeIndex = new Tuple<int, int>(PiggyIndex.Item2, PiggyIndex.Item2 + SafeSlots);
		public static readonly Tuple<int, int> TrashIndex = new Tuple<int, int>(SafeIndex.Item2, SafeIndex.Item2 + TrashSlots);
		public static readonly Tuple<int, int> ForgeIndex = new Tuple<int, int>(TrashIndex.Item2, TrashIndex.Item2 + ForgeSlots);
		public static readonly Tuple<int, int> VoidIndex = new Tuple<int, int>(ForgeIndex.Item2, ForgeIndex.Item2 + VoidSlots);

		public static readonly Tuple<int, int> Loadout1Armor = new Tuple<int, int>(VoidIndex.Item2, VoidIndex.Item2 + LoadoutArmorSlots);
		public static readonly Tuple<int, int> Loadout1Dye = new Tuple<int, int>(Loadout1Armor.Item2, Loadout1Armor.Item2 + LoadoutDyeSlots);

		public static readonly Tuple<int, int> Loadout2Armor = new Tuple<int, int>(Loadout1Dye.Item2, Loadout1Dye.Item2 + LoadoutArmorSlots);
		public static readonly Tuple<int, int> Loadout2Dye = new Tuple<int, int>(Loadout2Armor.Item2, Loadout2Armor.Item2 + LoadoutDyeSlots);

		public static readonly Tuple<int, int> Loadout3Armor = new Tuple<int, int>(Loadout2Dye.Item2, Loadout2Dye.Item2 + LoadoutArmorSlots);
		public static readonly Tuple<int, int> Loadout3Dye = new Tuple<int, int>(Loadout3Armor.Item2, Loadout3Armor.Item2 + LoadoutDyeSlots);

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
		/// Gets the item copy.
		/// </summary>
		public Item TItem
		{
			get
			{
				Item item = new Item();
				item.netDefaults(NetId);
				item.stack = Stack;
				item.prefix = PrefixId;
				return item;
			}
		}

		/// <summary>
		/// Creates a new <see cref="NetItem"/>.
		/// </summary>
		/// <param name="netId">The net ID.</param>
		/// <param name="stack">The stack.</param>
		/// <param name="prefixId">The prefix ID.</param>
		public NetItem(int netId, int stack = 1, byte prefixId = 0)
		{
			_netId = netId;
			_stack = stack;
			_prefixId = prefixId;
		}

		/// <summary>
		/// Creates a new <see cref="NetItem"/>.
		/// </summary>
		/// <param name="item">The <see cref="Item"/></param>
		public NetItem(Terraria.Item item) : this(item.netID, item.stack, item.prefix)
		{
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
				: new NetItem(item);
		}
	}
}
