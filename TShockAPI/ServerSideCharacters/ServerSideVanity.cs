using Microsoft.Xna.Framework;
using Terraria;

namespace TShockAPI.ServerSideCharacters
{
	/// <summary>
	/// Contains a server side player's vanity display
	/// </summary>
	public class ServerSideVanity
	{
		/// <summary>
		/// Player's skin variant
		/// </summary>
		public int SkinVariant { get; set; }
		/// <summary>
		/// Player's hair type
		/// </summary>
		public int Hair { get; set; }
		/// <summary>
		/// Hair dye applied to the player's hair
		/// </summary>
		public byte HairDye { get; set; }
		/// <summary>
		/// Player's hair color
		/// </summary>
		public Color? HairColor { get; set; }
		/// <summary>
		/// Player's pants color
		/// </summary>
		public Color? PantsColor { get; set; }
		/// <summary>
		/// Player's shirt color
		/// </summary>
		public Color? ShirtColor { get; set; }
		/// <summary>
		/// Player's undershirt color
		/// </summary>
		public Color? UnderShirtColor { get; set; }
		/// <summary>
		/// Player's shoe color
		/// </summary>
		public Color? ShoeColor { get; set; }
		/// <summary>
		/// Player's skin color
		/// </summary>
		public Color? SkinColor { get; set; }
		/// <summary>
		/// Player's eye color
		/// </summary>
		public Color? EyeColor { get; set; }
		/// <summary>
		/// Player's hidden accessories
		/// </summary>
		public bool[] HideVisuals { get; set; }

		/// <summary>
		/// Creates server side vanity for the given player
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static ServerSideVanity CreateFrom(Player player)
		{
			ServerSideVanity vanity = new ServerSideVanity
			{
				Hair = player.hair,
				HairDye = player.hairDye,
				HairColor = player.hairColor,
				PantsColor = player.pantsColor,
				ShirtColor = player.shirtColor,
				UnderShirtColor = player.underShirtColor,
				ShoeColor = player.shoeColor,
				SkinColor = player.skinColor,
				EyeColor = player.eyeColor,
				HideVisuals = player.hideVisibleAccessory
			};

			return vanity;
		}
	}
}
