/*
TShock, a server mod for Terraria
Copyright (C) 2011-2016 Nyx Studios (fka. The TShock Team)

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

﻿using System;
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
		/// 180 - The inventory size (inventory, held item, armour, dies, coins, ammo, piggy, safe, and trash)
		/// </summary>
		public static readonly int MaxInventory = InventorySlots + ArmorSlots + DyeSlots + MiscEquipSlots + MiscDyeSlots + PiggySlots + SafeSlots + 1;

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
