using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Handles client side exploits of LandGolfBallInCup packet.
	/// </summary>
	public class LandGolfBallInCupHandler : IPacketHandler<LandGolfBallInCupEventArgs>
	{
		/// <summary>
		/// List of golf ball projectile IDs.
		/// </summary>
		public static readonly List<int> GolfBallProjectileIDs = new List<int>()
		{
			ProjectileID.DirtGolfBall,
			ProjectileID.GolfBallDyedBlack,
			ProjectileID.GolfBallDyedBlue,
			ProjectileID.GolfBallDyedBrown,
			ProjectileID.GolfBallDyedCyan,
			ProjectileID.GolfBallDyedGreen,
			ProjectileID.GolfBallDyedLimeGreen,
			ProjectileID.GolfBallDyedOrange,
			ProjectileID.GolfBallDyedPink,
			ProjectileID.GolfBallDyedPurple,
			ProjectileID.GolfBallDyedRed,
			ProjectileID.GolfBallDyedSkyBlue,
			ProjectileID.GolfBallDyedTeal,
			ProjectileID.GolfBallDyedViolet,
			ProjectileID.GolfBallDyedYellow
		};

		/// <summary>
		/// List of golf club item IDs
		/// </summary>
		public static readonly List<int> GolfClubItemIDs = new List<int>()
		{
			ItemID.GolfClubChlorophyteDriver,
			ItemID.GolfClubDiamondWedge,
			ItemID.GolfClubShroomitePutter,
			ItemID.Fake_BambooChest,
			ItemID.GolfClubTitaniumIron,
			ItemID.GolfClubGoldWedge,
			ItemID.GolfClubLeadPutter,
			ItemID.GolfClubMythrilIron,
			ItemID.GolfClubWoodDriver,
			ItemID.GolfClubBronzeWedge,
			ItemID.GolfClubRustyPutter,
			ItemID.GolfClubStoneIron,
			ItemID.GolfClubPearlwoodDriver,
			ItemID.GolfClubIron,
			ItemID.GolfClubDriver,
			ItemID.GolfClubWedge,
			ItemID.GolfClubPutter
		};
		/// <summary>
		/// List of golf ball item IDs.
		/// </summary>
		public static readonly List<int> GolfBallItemIDs = new List<int>()
		{
			ItemID.GolfBall,
			ItemID.GolfBallDyedBlack,
			ItemID.GolfBallDyedBlue,
			ItemID.GolfBallDyedBrown,
			ItemID.GolfBallDyedCyan,
			ItemID.GolfBallDyedGreen,
			ItemID.GolfBallDyedLimeGreen,
			ItemID.GolfBallDyedOrange,
			ItemID.GolfBallDyedPink,
			ItemID.GolfBallDyedPurple,
			ItemID.GolfBallDyedRed,
			ItemID.GolfBallDyedSkyBlue,
			ItemID.GolfBallDyedTeal,
			ItemID.GolfBallDyedViolet
		};

		/// <summary>
		/// Invoked when a player lands a golf ball in a cup.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, LandGolfBallInCupEventArgs args)
		{
			if (args.PlayerIndex != args.Player.Index)
			{
				TShock.Log.ConsoleDebug($"LandGolfBallInCupHandler: Packet rejected for ID spoofing. Expected {args.Player.Index}, received {args.PlayerIndex} from {args.Player.Name}.");
				args.Handled = true;
				return;
			}

			if (args.TileX > Main.maxTilesX || args.TileX < 0
			   || args.TileY > Main.maxTilesY || args.TileY < 0)
			{
				TShock.Log.ConsoleDebug($"LandGolfBallInCupHandler: X and Y position is out of world bounds! - From {args.Player.Name}");
				args.Handled = true;
				return;
			}

			if (!Main.tile[args.TileX, args.TileY].active() && Main.tile[args.TileX, args.TileY].type != TileID.GolfHole)
			{
				TShock.Log.ConsoleDebug($"LandGolfBallInCupHandler: Tile at packet position X:{args.TileX} Y:{args.TileY} is not a golf hole! - From {args.Player.Name}");
				args.Handled = true;
				return;
			}

			if (!GolfBallProjectileIDs.Contains(args.ProjectileType))
			{
				TShock.Log.ConsoleDebug($"LandGolfBallInCupHandler: Invalid golf ball projectile ID {args.ProjectileType}! - From {args.Player.Name}");
				args.Handled = true;
				return;
			}

			var usedGolfBall = args.Player.RecentlyCreatedProjectiles.Any(e => GolfBallProjectileIDs.Contains(e.Type));
			var usedGolfClub = args.Player.RecentlyCreatedProjectiles.Any(e => e.Type == ProjectileID.GolfClubHelper);
			if (!usedGolfClub && !usedGolfBall)
			{
				TShock.Log.ConsoleDebug($"GolfPacketHandler: Player did not have create a golf club projectile the last 5 seconds! - From {args.Player.Name}");
				args.Handled = true;
				return;
			}

			if (!GolfClubItemIDs.Contains(args.Player.SelectedItem.type))
			{
				TShock.Log.ConsoleDebug($"LandGolfBallInCupHandler: Item selected is not a golf club! - From {args.Player.Name}");
				args.Handled = true;
				return;
			}
		}
	}
}
