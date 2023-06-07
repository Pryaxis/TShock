using System.Collections.Generic;
using System.IO;

using Terraria;
using Terraria.ID;

using TShockAPI.Net;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Provides processors for handling tile rect packets.
	/// This required many hours of reverse engineering work, and is kindly provided to TShock for free by @punchready.
	/// </summary>
	public sealed class SendTileRectHandler : IPacketHandler<GetDataHandlers.SendTileRectEventArgs>
	{
		/// <summary>
		/// Represents a tile rectangle sent through the packet.
		/// </summary>
		private sealed class TileRect
		{
			private readonly NetTile[,] _tiles;
			public readonly int X;
			public readonly int Y;
			public readonly int Width;
			public readonly int Height;

			/// <summary>
			/// Accesses the tiles contained in this rect.
			/// </summary>
			/// <param name="x">The X coordinate within the rect.</param>
			/// <param name="y">The Y coordinate within the rect.</param>
			/// <returns>The tile at the given position within the rect.</returns>
			public NetTile this[int x, int y] => _tiles[x, y];

			/// <summary>
			/// Constructs a new tile rect based on the given information.
			/// </summary>
			public TileRect(NetTile[,] tiles, int x, int y, int width, int height)
			{
				_tiles = tiles;
				X = x;
				Y = y;
				Width = width;
				Height = height;
			}

			/// <summary>
			/// Reads a tile rect from the given stream.
			/// </summary>
			/// <returns>The resulting tile rect.</returns>
			public static TileRect Read(MemoryStream stream, int tileX, int tileY, int width, int height)
			{
				NetTile[,] tiles = new NetTile[width, height];
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						tiles[x, y] = new NetTile();
						tiles[x, y].Unpack(stream); // explicit > implicit
					}
				}
				return new TileRect(tiles, tileX, tileY, width, height);
			}
		}

		/// <summary>
		/// Represents a common tile rect operation (Placement, State Change, Removal).
		/// </summary>
		private readonly struct TileRectMatch
		{
			private const short IGNORE_FRAME = -1;

			private enum MatchType
			{
				Placement,
				StateChange,
				Removal,
			}

			private readonly int Width;
			private readonly int Height;

			private readonly ushort TileType;
			private readonly short MaxFrameX;
			private readonly short MaxFrameY;
			private readonly short FrameXStep;
			private readonly short FrameYStep;

			private readonly MatchType Type;

			private TileRectMatch(MatchType type, int width, int height, ushort tileType, short maxFrameX, short maxFrameY, short frameXStep, short frameYStep)
			{
				Type = type;
				Width = width;
				Height = height;
				TileType = tileType;
				MaxFrameX = maxFrameX;
				MaxFrameY = maxFrameY;
				FrameXStep = frameXStep;
				FrameYStep = frameYStep;
			}

			/// <summary>
			/// Creates a new placement operation.
			/// </summary>
			/// <param name="width">The width of the placement.</param>
			/// <param name="height">The height of the placement.</param>
			/// <param name="tileType">The tile type of the placement.</param>
			/// <param name="maxFrameX">The maximum allowed frameX of the placement.</param>
			/// <param name="maxFrameY">The maximum allowed frameY of the placement.</param>
			/// <param name="frameXStep">The step size in which frameX changes for this placement, or <c>1</c> if any value is allowed.</param>
			/// <param name="frameYStep">The step size in which frameX changes for this placement, or <c>1</c> if any value is allowed.</param>
			/// <returns>The resulting operation match.</returns>
			public static TileRectMatch Placement(int width, int height, ushort tileType, short maxFrameX, short maxFrameY, short frameXStep, short frameYStep)
			{
				return new TileRectMatch(MatchType.Placement, width, height, tileType, maxFrameX, maxFrameY, frameXStep, frameYStep);
			}

			/// <summary>
			/// Creates a new state change operation.
			/// </summary>
			/// <param name="width">The width of the state change.</param>
			/// <param name="height">The height of the state change.</param>
			/// <param name="tileType">The target tile type of the state change.</param>
			/// <param name="maxFrameX">The maximum allowed frameX of the state change.</param>
			/// <param name="maxFrameY">The maximum allowed frameY of the state change.</param>
			/// <param name="frameXStep">The step size in which frameX changes for this placement, or <c>1</c> if any value is allowed.</param>
			/// <param name="frameYStep">The step size in which frameY changes for this placement, or <c>1</c> if any value is allowed.</param>
			/// <returns>The resulting operation match.</returns>
			public static TileRectMatch StateChange(int width, int height, ushort tileType, short maxFrameX, short maxFrameY, short frameXStep, short frameYStep)
			{
				return new TileRectMatch(MatchType.StateChange, width, height, tileType, maxFrameX, maxFrameY, frameXStep, frameYStep);
			}

			/// <summary>
			/// Creates a new state change operation which only changes frameX.
			/// </summary>
			/// <param name="width">The width of the state change.</param>
			/// <param name="height">The height of the state change.</param>
			/// <param name="tileType">The target tile type of the state change.</param>
			/// <param name="maxFrame">The maximum allowed frameX of the state change.</param>
			/// <param name="frameStep">The step size in which frameX changes for this placement, or <c>1</c> if any value is allowed.</param>
			/// <returns>The resulting operation match.</returns>
			public static TileRectMatch StateChangeX(int width, int height, ushort tileType, short maxFrame, short frameStep)
			{
				return new TileRectMatch(MatchType.StateChange, width, height, tileType, maxFrame, IGNORE_FRAME, frameStep, 0);
			}

			/// <summary>
			/// Creates a new state change operation which only changes frameY.
			/// </summary>
			/// <param name="width">The width of the state change.</param>
			/// <param name="height">The height of the state change.</param>
			/// <param name="tileType">The target tile type of the state change.</param>
			/// <param name="maxFrame">The maximum allowed frameY of the state change.</param>
			/// <param name="frameStep">The step size in which frameY changes for this placement, or <c>1</c> if any value is allowed.</param>
			/// <returns>The resulting operation match.</returns>
			public static TileRectMatch StateChangeY(int width, int height, ushort tileType, short maxFrame, short frameStep)
			{
				return new TileRectMatch(MatchType.StateChange, width, height, tileType, IGNORE_FRAME, maxFrame, 0, frameStep);
			}

			/// <summary>
			/// Creates a new removal operation.
			/// </summary>
			/// <param name="width">The width of the removal.</param>
			/// <param name="height">The height of the removal.</param>
			/// <param name="tileType">The target tile type of the removal.</param>
			/// <returns>The resulting operation match.</returns>
			public static TileRectMatch Removal(int width, int height, ushort tileType)
			{
				return new TileRectMatch(MatchType.Removal, width, height, tileType, 0, 0, 0, 0);
			}

			/// <summary>
			/// Determines whether the given tile rectangle matches this operation, and if so, applies it to the world.
			/// </summary>
			/// <param name="player">The player the operation originates from.</param>
			/// <param name="rect">The tile rectangle of the operation.</param>
			/// <returns><see langword="true"/>, if the rect matches this operation and the changes have been applied, otherwise <see langword="false"/>.</returns>
			public bool Matches(TSPlayer player, TileRect rect)
			{
				if (rect.Width != Width || rect.Height != Height)
				{
					return false;
				}

				for (int x = 0; x < rect.Width; x++)
				{
					for (int y = 0; y < rect.Height; y++)
					{
						NetTile tile = rect[x, y];
						if (Type is MatchType.Placement or MatchType.StateChange)
						{
							if (tile.Type != TileType)
							{
								return false;
							}
						}
						if (Type is MatchType.Placement or MatchType.StateChange)
						{
							if (MaxFrameX != IGNORE_FRAME)
							{
								if (tile.FrameX < 0 || tile.FrameX > MaxFrameX || tile.FrameX % FrameXStep != 0)
								{
									return false;
								}
							}
							if (MaxFrameY != IGNORE_FRAME)
							{
								if (tile.FrameY < 0 || tile.FrameY > MaxFrameY || tile.FrameY % FrameYStep != 0)
								{
									// this is the only tile type sent in a tile rect where the frame have a different pattern (56, 74, 92 instead of 54, 72, 90)
									if (!(TileType == TileID.LunarMonolith && tile.FrameY % FrameYStep == 2))
									{
										return false;
									}
								}
							}
						}
						if (Type == MatchType.Removal)
						{
							if (tile.Active)
							{
								return false;
							}
						}
					}
				}

				for (int x = rect.X; x < rect.X + rect.Width; x++)
				{
					for (int y = rect.Y; y < rect.Y + rect.Height; y++)
					{
						if (!player.HasBuildPermission(x, y))
						{
							// for simplicity, let's pretend that the edit was valid, but do not execute it
							return true;
						}
					}
				}

				switch (Type)
				{
					case MatchType.Placement:
						{
							return MatchPlacement(player, rect);
						}
					case MatchType.StateChange:
						{
							return MatchStateChange(player, rect);
						}
					case MatchType.Removal:
						{
							return MatchRemoval(player, rect);
						}
				}

				return false;
			}

			private bool MatchPlacement(TSPlayer player, TileRect rect)
			{
				for (int x = rect.X; x < rect.Y + rect.Width; x++)
				{
					for (int y = rect.Y; y < rect.Y + rect.Height; y++)
					{
						if (Main.tile[x, y].active()) // the client will kill tiles that auto break before placing the object
						{
							return false;
						}
					}
				}

				// let's hope tile types never go out of short range (they use ushort in terraria's code)
				if (TShock.TileBans.TileIsBanned((short)TileType, player))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}

				for (int x = 0; x < rect.Width; x++)
				{
					for (int y = 0; y < rect.Height; y++)
					{
						Main.tile[x + rect.X, y + rect.Y].active(active: true);
						Main.tile[x + rect.X, y + rect.Y].type = rect[x, y].Type;
						Main.tile[x + rect.X, y + rect.Y].frameX = rect[x, y].FrameX;
						Main.tile[x + rect.X, y + rect.Y].frameY = rect[x, y].FrameY;
					}
				}

				return true;
			}

			private bool MatchStateChange(TSPlayer player, TileRect rect)
			{
				for (int x = rect.X; x < rect.Y + rect.Width; x++)
				{
					for (int y = rect.Y; y < rect.Y + rect.Height; y++)
					{
						if (!Main.tile[x, y].active() || Main.tile[x, y].type != TileType)
						{
							return false;
						}
					}
				}

				for (int x = 0; x < rect.Width; x++)
				{
					for (int y = 0; y < rect.Height; y++)
					{
						if (MaxFrameX != IGNORE_FRAME)
						{
							Main.tile[x + rect.X, y + rect.Y].frameX = rect[x, y].FrameX;
						}
						if (MaxFrameY != IGNORE_FRAME)
						{
							Main.tile[x + rect.X, y + rect.Y].frameY = rect[x, y].FrameY;
						}
					}
				}

				return true;
			}

			private bool MatchRemoval(TSPlayer player, TileRect rect)
			{
				for (int x = rect.X; x < rect.Y + rect.Width; x++)
				{
					for (int y = rect.Y; y < rect.Y + rect.Height; y++)
					{
						if (!Main.tile[x, y].active() || Main.tile[x, y].type != TileType)
						{
							return false;
						}
					}
				}

				for (int x = 0; x < rect.Width; x++)
				{
					for (int y = 0; y < rect.Height; y++)
					{
						Main.tile[x + rect.X, y + rect.Y].active(active: false);
						Main.tile[x + rect.X, y + rect.Y].frameX = -1;
						Main.tile[x + rect.X, y + rect.Y].frameY = -1;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// Contains the complete list of valid tile rect operations the game currently performs.
		/// </summary>
		// The matches restrict the tile rects to only place one kind of tile, and only with the given maximum values and step sizes for frameX and frameY. This performs pretty much perfect checks on the data, allowing only valid placements.
		// For TileID.MinecartTrack, the data is taken from `Minecart._trackSwitchOptions`, allowing any framing value in this array (currently 0-36).
		// For TileID.Plants, it is taken from `ItemID.Sets.flowerPacketInfo[n].stylesOnPurity`, allowing every style multiplied by 18.
		// The other operations are based on code analysis and manual observation.
		private static readonly TileRectMatch[] Matches = new TileRectMatch[]
		{
			TileRectMatch.Placement(2, 3, TileID.TargetDummy, 54, 36, 18, 18),
			TileRectMatch.Placement(3, 4, TileID.TeleportationPylon, 468, 54, 18, 18),
			TileRectMatch.Placement(2, 3, TileID.DisplayDoll, 126, 36, 18, 18),
			TileRectMatch.Placement(2, 3, TileID.HatRack, 90, 54, 18, 18),
			TileRectMatch.Placement(2, 2, TileID.ItemFrame, 162, 18, 18, 18),
			TileRectMatch.Placement(3, 3, TileID.WeaponsRack2, 90, 36, 18, 18),
			TileRectMatch.Placement(1, 1, TileID.FoodPlatter, 18, 0, 18, 18),
			TileRectMatch.Placement(1, 1, TileID.LogicSensor, 108, 0, 18, 18),

			TileRectMatch.StateChangeY(3, 2, TileID.Campfire, 54, 18),
			TileRectMatch.StateChangeY(4, 3, TileID.Cannon, 468, 18),
			TileRectMatch.StateChangeY(2, 2, TileID.ArrowSign, 270, 18),
			TileRectMatch.StateChangeY(2, 2, TileID.PaintedArrowSign, 270, 18),

			TileRectMatch.StateChangeX(2, 2, TileID.MusicBoxes, 54, 18),

			TileRectMatch.StateChangeY(2, 3, TileID.LunarMonolith, 92, 18),
			TileRectMatch.StateChangeY(2, 3, TileID.BloodMoonMonolith, 90, 18),
			TileRectMatch.StateChangeY(2, 3, TileID.VoidMonolith, 90, 18),
			TileRectMatch.StateChangeY(2, 3, TileID.EchoMonolith, 90, 18),
			TileRectMatch.StateChangeY(2, 3, TileID.ShimmerMonolith, 144, 18),
			TileRectMatch.StateChangeY(2, 4, TileID.WaterFountain, 126, 18),

			TileRectMatch.StateChangeX(1, 1, TileID.Candles, 18, 18),
			TileRectMatch.StateChangeX(1, 1, TileID.PeaceCandle, 18, 18),
			TileRectMatch.StateChangeX(1, 1, TileID.WaterCandle, 18, 18),
			TileRectMatch.StateChangeX(1, 1, TileID.PlatinumCandle, 18, 18),
			TileRectMatch.StateChangeX(1, 1, TileID.ShadowCandle, 18, 18),

			TileRectMatch.StateChange(1, 1, TileID.Traps, 90, 90, 18, 18),

			TileRectMatch.StateChangeX(1, 1, TileID.WirePipe, 36, 18),
			TileRectMatch.StateChangeX(1, 1, TileID.ProjectilePressurePad, 66, 22),
			TileRectMatch.StateChangeX(1, 1, TileID.Plants, 792, 18),
			TileRectMatch.StateChangeX(1, 1, TileID.MinecartTrack, 36, 1),

			TileRectMatch.Removal(1, 2, TileID.Firework),
			TileRectMatch.Removal(1, 1, TileID.LandMine),
		};


		/// <summary>
		/// Handles a packet receive event.
		/// </summary>
		public void OnReceive(object sender, GetDataHandlers.SendTileRectEventArgs args)
		{
			// this permission bypasses all checks for direct access to the world
			if (args.Player.HasPermission(Permissions.allowclientsideworldedit))
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect accepted clientside world edit from {args.Player.Name}"));

				// use vanilla handling
				args.Handled = false;
				return;
			}

			// this handler handles the entire logic of this packet
			args.Handled = true;

			// as of 1.4 this is the biggest size the client will send in any case, determined by full code analysis
			// see default matches above and special cases below
			if (args.Width > 4 || args.Length > 4)
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect rejected from size from {args.Player.Name}"));

				// definitely invalid; do not send any correcting data
				return;
			}

			// player throttled?
			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect rejected from throttle from {args.Player.Name}"));

				// send correcting data
				args.Player.SendTileRect(args.TileX, args.TileY, args.Length, args.Width);
				return;
			}

			// player disabled?
			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect rejected from being disabled from {args.Player.Name}"));

				// send correcting data
				args.Player.SendTileRect(args.TileX, args.TileY, args.Length, args.Width);
				return;
			}

			// read the tile rectangle
			TileRect rect = TileRect.Read(args.Data, args.TileX, args.TileY, args.Width, args.Length);

			// check if the positioning is valid
			if (!IsRectPositionValid(args.Player, rect))
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect rejected from out of bounds / build permission from {args.Player.Name}"));

				// send nothing due to out of bounds
				return;
			}

			// a very special case, due to the clentaminator having a larger range than TSPlayer.IsInRange() allows
			if (MatchesConversionSpread(args.Player, rect))
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect reimplemented from {args.Player.Name}"));

				// send correcting data
				args.Player.SendTileRect(args.TileX, args.TileY, args.Length, args.Width);
				return;
			}

			// check if the distance is valid
			if (!IsRectDistanceValid(args.Player, rect))
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect rejected from out of range from {args.Player.Name}"));

				// send correcting data
				args.Player.SendTileRect(args.TileX, args.TileY, args.Length, args.Width);
				return;
			}

			// a very special case, due to the flower seed check otherwise hijacking this
			if (MatchesFlowerBoots(args.Player, rect))
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect reimplemented from {args.Player.Name}"));

				// send correcting data
				args.Player.SendTileRect(args.TileX, args.TileY, args.Length, args.Width);
				return;
			}

			// check if the rect matches any valid operation
			foreach (TileRectMatch match in Matches)
			{
				if (match.Matches(args.Player, rect))
				{
					TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect reimplemented from {args.Player.Name}"));

					// send correcting data
					args.Player.SendTileRect(args.TileX, args.TileY, args.Length, args.Width);
					return;
				}
			}

			// a few special cases
			if (MatchesGrassMow(args.Player, rect) || MatchesChristmasTree(args.Player, rect))
			{
				TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect reimplemented from {args.Player.Name}"));

				// send correcting data
				args.Player.SendTileRect(args.TileX, args.TileY, args.Length, args.Width);
				return;
			}

			TShock.Log.ConsoleDebug(GetString($"Bouncer / SendTileRect rejected from matches from {args.Player.Name}"));

			// send correcting data
			args.Player.SendTileRect(args.TileX, args.TileY, args.Length, args.Width);
			return;
		}

		/// <summary>
		/// Checks whether the tile rect is at a valid position for the given player.
		/// </summary>
		/// <param name="player">The player the operation originates from.</param>
		/// <param name="rect">The tile rectangle of the operation.</param>
		/// <returns><see langword="true"/>, if the rect at a valid position, otherwise <see langword="false"/>.</returns>
		private static bool IsRectPositionValid(TSPlayer player, TileRect rect)
		{
			for (int x = 0; x < rect.Width; x++)
			{
				for (int y = 0; y < rect.Height; y++)
				{
					int realX = rect.X + x;
					int realY = rect.Y + y;

					if (realX < 0 || realX >= Main.maxTilesX || realY < 0 || realY >= Main.maxTilesY)
					{
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Checks whether the tile rect is at a valid distance to the given player.
		/// </summary>
		/// <param name="player">The player the operation originates from.</param>
		/// <param name="rect">The tile rectangle of the operation.</param>
		/// <returns><see langword="true"/>, if the rect at a valid distance, otherwise <see langword="false"/>.</returns>
		private static bool IsRectDistanceValid(TSPlayer player, TileRect rect)
		{
			for (int x = 0; x < rect.Width; x++)
			{
				for (int y = 0; y < rect.Height; y++)
				{
					int realX = rect.X + x;
					int realY = rect.Y + y;

					if (!player.IsInRange(realX, realY))
					{
						return false;
					}
				}
			}

			return true;
		}


		/// <summary>
		/// Checks whether the tile rect is a valid conversion spread (Clentaminator, Powders, etc.).
		/// </summary>
		/// <param name="player">The player the operation originates from.</param>
		/// <param name="rect">The tile rectangle of the operation.</param>
		/// <returns><see langword="true"/>, if the rect matches a conversion spread operation, otherwise <see langword="false"/>.</returns>
		private static bool MatchesConversionSpread(TSPlayer player, TileRect rect)
		{
			if (rect.Width != 1 || rect.Height != 1)
			{
				return false;
			}

			ITile oldTile = Main.tile[rect.X, rect.Y];
			NetTile newTile = rect[0, 0];

			WorldGenMock.SimulateConversionChange(rect.X, rect.Y, out HashSet<ushort> validTiles, out HashSet<ushort> validWalls);

			if (newTile.Type != oldTile.type && validTiles.Contains(newTile.Type))
			{
				if (TShock.TileBans.TileIsBanned((short)newTile.Type, player))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}
				else if (!player.HasBuildPermission(rect.X, rect.Y))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}
				else
				{
					Main.tile[rect.X, rect.Y].type = newTile.Type;
					Main.tile[rect.X, rect.Y].frameX = newTile.FrameX;
					Main.tile[rect.X, rect.Y].frameY = newTile.FrameY;

					return true;
				}
			}

			if (newTile.Wall != oldTile.wall && validWalls.Contains(newTile.Wall))
			{
				// wallbans when?

				if (!player.HasBuildPermission(rect.X, rect.Y))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}
				else
				{
					Main.tile[rect.X, rect.Y].wall = newTile.Wall;

					return true;
				}
			}

			return false;
		}


		private static readonly Dictionary<ushort, HashSet<ushort>> PlantToGrassMap = new Dictionary<ushort, HashSet<ushort>>
		{
			{ TileID.Plants, new HashSet<ushort>()
			{
				TileID.Grass, TileID.GolfGrass
			} },
			{ TileID.HallowedPlants, new HashSet<ushort>()
			{
				TileID.HallowedGrass, TileID.GolfGrassHallowed
			} },
			{ TileID.HallowedPlants2, new HashSet<ushort>()
			{
				TileID.HallowedGrass, TileID.GolfGrassHallowed
			} },
			{ TileID.JunglePlants2, new HashSet<ushort>()
			{
				TileID.JungleGrass
			} },
			{ TileID.AshPlants, new HashSet<ushort>()
			{
				TileID.AshGrass
			} },
		};

		private static readonly Dictionary<ushort, HashSet<ushort>> GrassToStyleMap = new Dictionary<ushort, HashSet<ushort>>()
		{
			{ TileID.Plants, new HashSet<ushort>()
			{
				6, 7, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 24, 27, 30, 33, 36, 39, 42,
				22, 23, 25, 26, 28, 29, 31, 32, 34, 35, 37, 38, 40, 41, 43, 44,
			} },
			{ TileID.HallowedPlants, new HashSet<ushort>()
			{
				4, 6,
			} },
			{ TileID.HallowedPlants2, new HashSet<ushort>()
			{
				2, 3, 4, 6, 7,
			} },
			{ TileID.JunglePlants2, new HashSet<ushort>()
			{
				9, 10, 11, 12, 13, 14, 15, 16,
			} },
			{ TileID.AshPlants, new HashSet<ushort>()
			{
				6, 7, 8, 9, 10,
			} },
		};

		/// <summary>
		/// Checks whether the tile rect is a valid Flower Boots placement.
		/// </summary>
		/// <param name="player">The player the operation originates from.</param>
		/// <param name="rect">The tile rectangle of the operation.</param>
		/// <returns><see langword="true"/>, if the rect matches a Flower Boots placement, otherwise <see langword="false"/>.</returns>
		private static bool MatchesFlowerBoots(TSPlayer player, TileRect rect)
		{
			if (rect.Width != 1 || rect.Height != 1)
			{
				return false;
			}

			if (!player.TPlayer.flowerBoots)
			{
				return false;
			}

			ITile oldTile = Main.tile[rect.X, rect.Y];
			NetTile newTile = rect[0, 0];

			if (
				PlantToGrassMap.TryGetValue(newTile.Type, out HashSet<ushort> grassTiles) &&
				!oldTile.active() && grassTiles.Contains(Main.tile[rect.X, rect.Y + 1].type) &&
				GrassToStyleMap[newTile.Type].Contains((ushort)(newTile.FrameX / 18))
			)
			{
				if (TShock.TileBans.TileIsBanned((short)newTile.Type, player))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}

				if (!player.HasBuildPermission(rect.X, rect.Y))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}

				Main.tile[rect.X, rect.Y].active(active: true);
				Main.tile[rect.X, rect.Y].type = newTile.Type;
				Main.tile[rect.X, rect.Y].frameX = newTile.FrameX;
				Main.tile[rect.X, rect.Y].frameY = 0;

				return true;
			}

			return false;
		}


		private static readonly Dictionary<ushort, ushort> GrassToMowedMap = new Dictionary<ushort, ushort>
		{
			{ TileID.Grass, TileID.GolfGrass },
			{ TileID.HallowedGrass, TileID.GolfGrassHallowed },
		};

		/// <summary>
		/// Checks whether the tile rect is a valid grass mow.
		/// </summary>
		/// <param name="player">The player the operation originates from.</param>
		/// <param name="rect">The tile rectangle of the operation.</param>
		/// <returns><see langword="true"/>, if the rect matches a grass mowing operation, otherwise <see langword="false"/>.</returns>
		private static bool MatchesGrassMow(TSPlayer player, TileRect rect)
		{
			if (rect.Width != 1 || rect.Height != 1)
			{
				return false;
			}

			ITile oldTile = Main.tile[rect.X, rect.Y];
			NetTile newTile = rect[0, 0];

			if (GrassToMowedMap.TryGetValue(oldTile.type, out ushort mowed) && newTile.Type == mowed)
			{
				if (TShock.TileBans.TileIsBanned((short)newTile.Type, player))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}

				if (!player.HasBuildPermission(rect.X, rect.Y))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}

				Main.tile[rect.X, rect.Y].type = newTile.Type;
				if (!newTile.FrameImportant)
				{
					Main.tile[rect.X, rect.Y].frameX = -1;
					Main.tile[rect.X, rect.Y].frameY = -1;
				}

				// prevent a common crash when the game checks all vines in an unlimited horizontal length
				if (TileID.Sets.IsVine[Main.tile[rect.X, rect.Y + 1].type])
				{
					WorldGen.KillTile(rect.X, rect.Y + 1);
				}

				return true;
			}

			return false;
		}


		/// <summary>
		/// Checks whether the tile rect is a valid christmas tree modification.
		/// This also required significant reverse engineering effort.
		/// </summary>
		/// <param name="player">The player the operation originates from.</param>
		/// <param name="rect">The tile rectangle of the operation.</param>
		/// <returns><see langword="true"/>, if the rect matches a christmas tree operation, otherwise <see langword="false"/>.</returns>
		private static bool MatchesChristmasTree(TSPlayer player, TileRect rect)
		{
			if (rect.Width != 1 || rect.Height != 1)
			{
				return false;
			}

			ITile oldTile = Main.tile[rect.X, rect.Y];
			NetTile newTile = rect[0, 0];

			if (oldTile.type == TileID.ChristmasTree && newTile.Type == TileID.ChristmasTree)
			{
				if (newTile.FrameX != 10)
				{
					return false;
				}

				int obj_0 = (newTile.FrameY & 0b0000000000000111);
				int obj_1 = (newTile.FrameY & 0b0000000000111000) >> 3;
				int obj_2 = (newTile.FrameY & 0b0000001111000000) >> 6;
				int obj_3 = (newTile.FrameY & 0b0011110000000000) >> 10;
				int obj_x = (newTile.FrameY & 0b1100000000000000) >> 14;

				if (obj_x != 0)
				{
					return false;
				}

				if (obj_0 is < 0 or > 4 || obj_1 is < 0 or > 6 || obj_2 is < 0 or > 11 || obj_3 is < 0 or > 11)
				{
					return false;
				}

				if (!player.HasBuildPermission(rect.X, rect.Y))
				{
					// for simplicity, let's pretend that the edit was valid, but do not execute it
					return true;
				}

				Main.tile[rect.X, rect.Y].frameY = newTile.FrameY;

				return true;
			}

			return false;
		}
	}

	/// <summary>
	/// This helper class allows simulating a `WorldGen.Convert` call and retrieving all valid changes for a given tile.
	/// </summary>
	internal static class WorldGenMock
	{
		/// <summary>
		/// This is a mock tile which collects all possible changes the `WorldGen.Convert` code could make in its property setters.
		/// </summary>
		private sealed class MockTile
		{
			private readonly HashSet<ushort> _setTypes;
			private readonly HashSet<ushort> _setWalls;

			private ushort _type;
			private ushort _wall;

			public MockTile(ushort type, ushort wall, HashSet<ushort> setTypes, HashSet<ushort> setWalls)
			{
				_setTypes = setTypes;
				_setWalls = setWalls;
				_type = type;
				_wall = wall;
			}

#pragma warning disable IDE1006

			public ushort type
			{
				get => _type;
				set
				{
					_setTypes.Add(value);
					_type = value;
				}
			}

			public ushort wall
			{
				get => _wall;
				set
				{
					_setWalls.Add(value);
					_wall = value;
				}
			}

#pragma warning restore IDE1006
		}

		/// <summary>
		/// Simulates what would happen if `WorldGen.Convert` was called on the given coordinates and returns two sets with the possible tile type and wall types that the conversion could change the tile to.
		/// </summary>
		public static void SimulateConversionChange(int x, int y, out HashSet<ushort> validTiles, out HashSet<ushort> validWalls)
		{
			validTiles = new HashSet<ushort>();
			validWalls = new HashSet<ushort>();

			// all the conversion types used in the code, most apparent in Projectile ai 31
			foreach (int conversionType in new int[] { 0, 1, 2, 3, 4, 5, 6, 7 })
			{
				MockTile mock = new(Main.tile[x, y].type, Main.tile[x, y].wall, validTiles, validWalls);
				Convert(mock, x, y, conversionType);
			}
		}

		/* 
		 * This is a copy of the `WorldGen.Convert` method with the following precise changes:
		 *  - Added a `MockTile tile` parameter
		 *  - Changed the `i` and `j` parameters to `k` and `l`
		 *  - Removed the size parameter
		 *  - Removed the area loop and `Tile tile = Main.tile[k, l]` access in favor of using the tile parameter
		 *  - Removed all calls to `WorldGen.SquareWallFrame`, `NetMessage.SendTileSquare`, `WorldGen.TryKillingTreesAboveIfTheyWouldBecomeInvalid`
		 *  - Changed all `continue` statements to `break` statements
		 *  - Removed the ifs checking the bounds of the tile and wall types
		 *  - Removed branches that would call `WorldGen.KillTile`
		 *  - Changed branches depending on randomness to instead set the property to both values after one another
		 * 
		 * This overall leads to a method that can be called on a MockTile and real-world coordinates and will spit out the proper conversion changes into the MockTile.
		 */

		private static void Convert(MockTile tile, int k, int l, int conversionType)
		{
			int type = tile.type;
			int wall = tile.wall;
			switch (conversionType)
			{
				case 4:
					if (WallID.Sets.Conversion.Grass[wall] && wall != 81)
					{
						tile.wall = 81;
					}
					else if (WallID.Sets.Conversion.Stone[wall] && wall != 83)
					{
						tile.wall = 83;
					}
					else if (WallID.Sets.Conversion.HardenedSand[wall] && wall != 218)
					{
						tile.wall = 218;
					}
					else if (WallID.Sets.Conversion.Sandstone[wall] && wall != 221)
					{
						tile.wall = 221;
					}
					else if (WallID.Sets.Conversion.NewWall1[wall] && wall != 192)
					{
						tile.wall = 192;
					}
					else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 193)
					{
						tile.wall = 193;
					}
					else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 194)
					{
						tile.wall = 194;
					}
					else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 195)
					{
						tile.wall = 195;
					}
					if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type]) && type != 203)
					{
						tile.type = 203;
					}
					else if (TileID.Sets.Conversion.JungleGrass[type] && type != 662)
					{
						tile.type = 662;
					}
					else if (TileID.Sets.Conversion.Grass[type] && type != 199)
					{
						tile.type = 199;
					}
					else if (TileID.Sets.Conversion.Ice[type] && type != 200)
					{
						tile.type = 200;
					}
					else if (TileID.Sets.Conversion.Sand[type] && type != 234)
					{
						tile.type = 234;
					}
					else if (TileID.Sets.Conversion.HardenedSand[type] && type != 399)
					{
						tile.type = 399;
					}
					else if (TileID.Sets.Conversion.Sandstone[type] && type != 401)
					{
						tile.type = 401;
					}
					else if (TileID.Sets.Conversion.Thorn[type] && type != 352)
					{
						tile.type = 352;
					}
					break;
				case 2:
					if (WallID.Sets.Conversion.Grass[wall] && wall != 70)
					{
						tile.wall = 70;
					}
					else if (WallID.Sets.Conversion.Stone[wall] && wall != 28)
					{
						tile.wall = 28;
					}
					else if (WallID.Sets.Conversion.HardenedSand[wall] && wall != 219)
					{
						tile.wall = 219;
					}
					else if (WallID.Sets.Conversion.Sandstone[wall] && wall != 222)
					{
						tile.wall = 222;
					}
					else if (WallID.Sets.Conversion.NewWall1[wall] && wall != 200)
					{
						tile.wall = 200;
					}
					else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 201)
					{
						tile.wall = 201;
					}
					else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 202)
					{
						tile.wall = 202;
					}
					else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 203)
					{
						tile.wall = 203;
					}
					if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type]) && type != 117)
					{
						tile.type = 117;
					}
					else if (TileID.Sets.Conversion.GolfGrass[type] && type != 492)
					{
						tile.type = 492;
					}
					else if (TileID.Sets.Conversion.Grass[type] && type != 109 && type != 492)
					{
						tile.type = 109;
					}
					else if (TileID.Sets.Conversion.Ice[type] && type != 164)
					{
						tile.type = 164;
					}
					else if (TileID.Sets.Conversion.Sand[type] && type != 116)
					{
						tile.type = 116;
					}
					else if (TileID.Sets.Conversion.HardenedSand[type] && type != 402)
					{
						tile.type = 402;
					}
					else if (TileID.Sets.Conversion.Sandstone[type] && type != 403)
					{
						tile.type = 403;
					}
					if (type == 59 && (Main.tile[k - 1, l].type == 109 || Main.tile[k + 1, l].type == 109 || Main.tile[k, l - 1].type == 109 || Main.tile[k, l + 1].type == 109))
					{
						tile.type = 0;
					}
					break;
				case 1:
					if (WallID.Sets.Conversion.Grass[wall] && wall != 69)
					{
						tile.wall = 69;
					}
					else if (TileID.Sets.Conversion.JungleGrass[type] && type != 661)
					{
						tile.type = 661;
					}
					else if (WallID.Sets.Conversion.Stone[wall] && wall != 3)
					{
						tile.wall = 3;
					}
					else if (WallID.Sets.Conversion.HardenedSand[wall] && wall != 217)
					{
						tile.wall = 217;
					}
					else if (WallID.Sets.Conversion.Sandstone[wall] && wall != 220)
					{
						tile.wall = 220;
					}
					else if (WallID.Sets.Conversion.NewWall1[wall] && wall != 188)
					{
						tile.wall = 188;
					}
					else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 189)
					{
						tile.wall = 189;
					}
					else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 190)
					{
						tile.wall = 190;
					}
					else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 191)
					{
						tile.wall = 191;
					}
					if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type]) && type != 25)
					{
						tile.type = 25;
					}
					else if (TileID.Sets.Conversion.Grass[type] && type != 23)
					{
						tile.type = 23;
					}
					else if (TileID.Sets.Conversion.Ice[type] && type != 163)
					{
						tile.type = 163;
					}
					else if (TileID.Sets.Conversion.Sand[type] && type != 112)
					{
						tile.type = 112;
					}
					else if (TileID.Sets.Conversion.HardenedSand[type] && type != 398)
					{
						tile.type = 398;
					}
					else if (TileID.Sets.Conversion.Sandstone[type] && type != 400)
					{
						tile.type = 400;
					}
					else if (TileID.Sets.Conversion.Thorn[type] && type != 32)
					{
						tile.type = 32;
					}
					break;
				case 3:
					if (WallID.Sets.CanBeConvertedToGlowingMushroom[wall])
					{
						tile.wall = 80;
					}
					if (tile.type == 60)
					{
						tile.type = 70;
					}
					break;
				case 5:
					if ((WallID.Sets.Conversion.Stone[wall] || WallID.Sets.Conversion.NewWall1[wall] || WallID.Sets.Conversion.NewWall2[wall] || WallID.Sets.Conversion.NewWall3[wall] || WallID.Sets.Conversion.NewWall4[wall] || WallID.Sets.Conversion.Ice[wall] || WallID.Sets.Conversion.Sandstone[wall]) && wall != 187)
					{
						tile.wall = 187;
					}
					else if ((WallID.Sets.Conversion.HardenedSand[wall] || WallID.Sets.Conversion.Dirt[wall] || WallID.Sets.Conversion.Snow[wall]) && wall != 216)
					{
						tile.wall = 216;
					}
					if ((TileID.Sets.Conversion.Grass[type] || TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.Snow[type] || TileID.Sets.Conversion.Dirt[type]) && type != 53)
					{
						int num = 53;
						if (WorldGen.BlockBelowMakesSandConvertIntoHardenedSand(k, l))
						{
							num = 397;
						}
						tile.type = (ushort)num;
					}
					else if (TileID.Sets.Conversion.HardenedSand[type] && type != 397)
					{
						tile.type = 397;
					}
					else if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type] || TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Sandstone[type]) && type != 396)
					{
						tile.type = 396;
					}
					break;
				case 6:
					if ((WallID.Sets.Conversion.Stone[wall] || WallID.Sets.Conversion.NewWall1[wall] || WallID.Sets.Conversion.NewWall2[wall] || WallID.Sets.Conversion.NewWall3[wall] || WallID.Sets.Conversion.NewWall4[wall] || WallID.Sets.Conversion.Ice[wall] || WallID.Sets.Conversion.Sandstone[wall]) && wall != 71)
					{
						tile.wall = 71;
					}
					else if ((WallID.Sets.Conversion.HardenedSand[wall] || WallID.Sets.Conversion.Dirt[wall] || WallID.Sets.Conversion.Snow[wall]) && wall != 40)
					{
						tile.wall = 40;
					}
					if ((TileID.Sets.Conversion.Grass[type] || TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Snow[type] || TileID.Sets.Conversion.Dirt[type]) && type != 147)
					{
						tile.type = 147;
					}
					else if ((Main.tileMoss[type] || TileID.Sets.Conversion.Stone[type] || TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Sandstone[type]) && type != 161)
					{
						tile.type = 161;
					}
					break;
				case 7:
					if ((WallID.Sets.Conversion.Stone[wall] || WallID.Sets.Conversion.Ice[wall] || WallID.Sets.Conversion.Sandstone[wall]) && wall != 1)
					{
						tile.wall = 1;
					}
					else if ((WallID.Sets.Conversion.HardenedSand[wall] || WallID.Sets.Conversion.Snow[wall] || WallID.Sets.Conversion.Dirt[wall]) && wall != 2)
					{
						tile.wall = 2;
					}
					else if (WallID.Sets.Conversion.NewWall1[wall] && wall != 196)
					{
						tile.wall = 196;
					}
					else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 197)
					{
						tile.wall = 197;
					}
					else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 198)
					{
						tile.wall = 198;
					}
					else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 199)
					{
						tile.wall = 199;
					}
					if ((TileID.Sets.Conversion.Stone[type] || TileID.Sets.Conversion.Ice[type] || TileID.Sets.Conversion.Sandstone[type]) && type != 1)
					{
						tile.type = 1;
					}
					else if (TileID.Sets.Conversion.GolfGrass[type] && type != 477)
					{
						tile.type = 477;
					}
					else if (TileID.Sets.Conversion.Grass[type] && type != 2 && type != 477)
					{
						tile.type = 2;
					}
					else if ((TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.HardenedSand[type] || TileID.Sets.Conversion.Snow[type] || TileID.Sets.Conversion.Dirt[type]) && type != 0)
					{
						int num2 = 0;
						if (WorldGen.TileIsExposedToAir(k, l))
						{
							num2 = 2;
						}
						tile.type = (ushort)num2;
					}
					break;
			}
			if (tile.wall == 69 || tile.wall == 70 || tile.wall == 81)
			{
				if (l < Main.worldSurface)
				{
					tile.wall = 65;
					tile.wall = 63;
				}
				else
				{
					tile.wall = 64;
				}
			}
			else if (WallID.Sets.Conversion.Stone[wall] && wall != 1 && wall != 262 && wall != 274 && wall != 61 && wall != 185)
			{
				tile.wall = 1;
			}
			else if (WallID.Sets.Conversion.Stone[wall] && wall == 262)
			{
				tile.wall = 61;
			}
			else if (WallID.Sets.Conversion.Stone[wall] && wall == 274)
			{
				tile.wall = 185;
			}
			if (WallID.Sets.Conversion.NewWall1[wall] && wall != 212)
			{
				tile.wall = 212;
			}
			else if (WallID.Sets.Conversion.NewWall2[wall] && wall != 213)
			{
				tile.wall = 213;
			}
			else if (WallID.Sets.Conversion.NewWall3[wall] && wall != 214)
			{
				tile.wall = 214;
			}
			else if (WallID.Sets.Conversion.NewWall4[wall] && wall != 215)
			{
				tile.wall = 215;
			}
			else if (tile.wall == 80)
			{
				tile.wall = 15;
				tile.wall = 64;
			}
			else if (WallID.Sets.Conversion.HardenedSand[wall] && wall != 216)
			{
				tile.wall = 216;
			}
			else if (WallID.Sets.Conversion.Sandstone[wall] && wall != 187)
			{
				tile.wall = 187;
			}
			if (tile.type == 492)
			{
				tile.type = 477;
			}
			else if (TileID.Sets.Conversion.JungleGrass[type] && type != 60)
			{
				tile.type = 60;
			}
			else if (TileID.Sets.Conversion.Grass[type] && type != 2 && type != 477)
			{
				tile.type = 2;
			}
			else if (TileID.Sets.Conversion.Stone[type] && type != 1)
			{
				tile.type = 1;
			}
			else if (TileID.Sets.Conversion.Sand[type] && type != 53)
			{
				tile.type = 53;
			}
			else if (TileID.Sets.Conversion.HardenedSand[type] && type != 397)
			{
				tile.type = 397;
			}
			else if (TileID.Sets.Conversion.Sandstone[type] && type != 396)
			{
				tile.type = 396;
			}
			else if (TileID.Sets.Conversion.Ice[type] && type != 161)
			{
				tile.type = 161;
			}
			else if (TileID.Sets.Conversion.MushroomGrass[type])
			{
				tile.type = 60;
			}
		}
	}
}
