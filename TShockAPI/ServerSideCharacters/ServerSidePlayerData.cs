using Terraria;

namespace TShockAPI.ServerSideCharacters
{
	/// <summary>
	/// Contains information about the server side state of a player
	/// </summary>
	public class ServerSidePlayerData
	{
		/// <summary>
		/// Contains the server side player's stats
		/// </summary>
		public ServerSideStats Stats { get; set; }

		/// <summary>
		/// Contains the server side player's vanity values
		/// </summary>
		public ServerSideVanity Vanity { get; set; }

		/// <summary>
		/// Contains the server side player's inventory values
		/// </summary>
		public ServerSideInventory Inventory { get; set; }

		/// <summary>
		/// Contains the server side player's spawn information
		/// </summary>
		public ServerSideSpawn Spawn { get; set; }

		/// <summary>
		/// Creates a basic server side player using inventory and stat defaults from the config, and vanity from the player
		/// </summary>
		/// <param name="player"></param>
		public static ServerSidePlayerData CreateDefaultFromPlayer(Player player)
		{
			ServerSidePlayerData data = new ServerSidePlayerData
			{
				Stats = ServerSideStats.CreateDefault(),
				Inventory = ServerSideInventory.CreateDefault(),
				Vanity = ServerSideVanity.CreateFromPlayer(player),
				Spawn = ServerSideSpawn.CreateDefault()
			};

			return data;
		}

		/// <summary>
		/// Creates a set of server side player data using the given player's character data
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static ServerSidePlayerData CreateFromPlayer(Player player)
		{
			ServerSidePlayerData data = new ServerSidePlayerData
			{
				Stats = ServerSideStats.CreateFromPlayer(player),
				Inventory = ServerSideInventory.CreateFromPlayer(player),
				Vanity = ServerSideVanity.CreateFromPlayer(player)
			};

			return data;
		}
	}
}
