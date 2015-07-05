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
		/// The size of the player's inventory (inventory, coins, ammo, trash)
		/// </summary>
		public static readonly int InventorySlots = 59;

		/// <summary>
		/// The number of armor slots.
		/// </summary>
		public static readonly int ArmorSlots = 20;

		/// <summary>
		/// The number of other equippable items
		/// </summary>
		public static readonly int MiscEquipSlots = 5;

		/// <summary>
		/// The number of dye slots.
		/// </summary>
		public static readonly int DyeSlots = 10;

		/// <summary>
		/// The number of other dye slots (for <see cref="MiscEquipSlots"/>)
		/// </summary>
		public static readonly int MiscDyeSlots = 5;

		/// <summary>
		/// The inventory size (including armour, dies, coins, ammo, and trash)
		/// </summary>
		public static readonly int MaxInventory = 99;

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
