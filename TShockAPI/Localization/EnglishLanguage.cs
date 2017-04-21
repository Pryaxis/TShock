using System;
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
				if(!skip)
					LanguageManager.Instance.SetLanguage(GameCulture.English);

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
				if(!skip)
					LanguageManager.Instance.SetLanguage(culture);
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

			return string.Empty;
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

			return string.Empty;
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

			return string.Empty;
		}
	}
}
