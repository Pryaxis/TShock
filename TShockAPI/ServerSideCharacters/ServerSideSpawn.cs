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
	}
}
