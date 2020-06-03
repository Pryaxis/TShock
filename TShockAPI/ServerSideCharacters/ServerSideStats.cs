using System;
using Terraria;

namespace TShockAPI.ServerSideCharacters
{
	/// <summary>
	/// Contains a server side player's stats
	/// </summary>
	public class ServerSideStats
	{
		/// <summary>
		/// The player's current health
		/// </summary>
		public int Health { get; set; }
		/// <summary>
		/// The player's maximum health
		/// </summary>
		public int MaxHealth { get; set; }
		/// <summary>
		/// The player's current mana
		/// </summary>
		public int Mana { get; set; }
		/// <summary>
		/// The player's maximum mana
		/// </summary>
		public int MaxMana { get; set; }
		/// <summary>
		/// Whether or not the player has unlocked extra slots
		/// </summary>
		public bool HasExtraSlot { get; set; }
		/// <summary>
		/// The number of angler quests completed by the player
		/// </summary>
		public int QuestsCompleted { get; set; }
		/// <summary>
		/// The amount of golf score accumulated by the player
		/// </summary>
		public int GolfScoreAccumulated { get; set; }

		/// <summary>
		/// Creates a default set of stats using the <see cref="ServerSideConfig"/> settings
		/// </summary>
		/// <returns></returns>
		public static ServerSideStats CreateDefault()
		{
			ServerSideStats stats = new ServerSideStats
			{
				Health = TShock.ServerSideCharacterConfig.StartingHealth,
				MaxHealth = TShock.ServerSideCharacterConfig.StartingHealth,
				Mana = TShock.ServerSideCharacterConfig.StartingMana,
				MaxMana = TShock.ServerSideCharacterConfig.StartingMana,
				HasExtraSlot = false,
				QuestsCompleted = 0,
				GolfScoreAccumulated = 0
			};

			return stats;
		}

		/// <summary>
		/// Creates a set of server side stats using the given player's stats
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static ServerSideStats CreateFromPlayer(Player player)
		{
			ServerSideStats stats = new ServerSideStats
			{
				Health = player.statLife,
				MaxHealth = player.statLifeMax,
				Mana = player.statMana,
				MaxMana = player.statManaMax,
				HasExtraSlot = player.extraAccessory,
				QuestsCompleted = player.anglerQuestsFinished,
				GolfScoreAccumulated = player.golferScoreAccumulated
			};

			return stats;
		}
	}
}
