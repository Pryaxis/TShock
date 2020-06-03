using System;

namespace TShockAPI.ServerSideCharacters
{
	/// <summary>
	/// Contains details about a server side player's spawn location
	/// </summary>
	public class ServerSideSpawn
	{
		/// <summary>
		/// The tile coordinate x position the player will spawn at
		/// </summary>
		public int TileX { get; set; }

		/// <summary>
		/// The tile coordinate y position the player will spawn at
		/// </summary>
		public int TileY { get; set; }

		/// <summary>
		/// Creates a default spawn point that spawns the player at the world's spawn point
		/// </summary>
		/// <returns></returns>
		public static ServerSideSpawn CreateDefault()
		{
			ServerSideSpawn spawn = new ServerSideSpawn
			{
				TileX = -1,
				TileY = -1
			};

			return spawn;
		}
	}
}
