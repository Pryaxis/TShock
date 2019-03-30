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

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;

namespace TShockAPI.Localization
{
	/// <summary>
	/// Provides a series of methods that give English texts
	/// </summary>
	public static class EnglishLanguage
	{
		private static readonly Dictionary<int, string> ItemNames = new Dictionary<int, string>();

		private static readonly Dictionary<int, string> NpcNames = new Dictionary<int, string>();

		private static readonly Dictionary<int, string> Prefixs = new Dictionary<int, string>();

		internal static void Initialize()
		{
			var culture = Language.ActiveCulture;

			var skip = culture == GameCulture.English;

			try
			{
				if (!skip)
				{
					LanguageManager.Instance.SetLanguage(GameCulture.English);
				}

				for (var i = -48; i < Main.maxItemTypes; i++)
				{
					ItemNames.Add(i, Lang.GetItemNameValue(i));
				}

				for (var i = -17; i < Main.maxNPCTypes; i++)
				{
					NpcNames.Add(i, Lang.GetNPCNameValue(i));
				}

				foreach (var field in typeof(Main).Assembly.GetType("Terraria.ID.PrefixID")
							.GetFields().Where(f => !f.Name.Equals("Count", StringComparison.Ordinal)))
				{
					Prefixs.Add((int) field.GetValue(null), field.Name);
				}
			}
			finally
			{
				if (!skip)
				{
					LanguageManager.Instance.SetLanguage(culture);
				}
			}
		}

		/// <summary>
		/// Get the english name of an item
		/// </summary>
		/// <param name="id">Id of the item</param>
		/// <returns>Item name in English</returns>
		public static string GetItemNameById(int id)
		{
			string itemName;
			if (ItemNames.TryGetValue(id, out itemName))
				return itemName;

			return null;
		}

		/// <summary>
		/// Get the english name of a npc
		/// </summary>
		/// <param name="id">Id of the npc</param>
		/// <returns>Npc name in English</returns>
		public static string GetNpcNameById(int id)
		{
			string npcName;
			if (NpcNames.TryGetValue(id, out npcName))
				return npcName;

			return null;
		}

		/// <summary>
		/// Get prefix in English
		/// </summary>
		/// <param name="id">Prefix Id</param>
		/// <returns>Prefix in English</returns>
		public static string GetPrefixById(int id)
		{
			string prefix;
			if (Prefixs.TryGetValue(id, out prefix))
				return prefix;

			return null;
		}
	}
}
