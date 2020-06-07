using OTAPI.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ObjectData;
using TShockAPI.Net;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Provides processors for handling Tile Square packets
	/// </summary>
	public class SendTileSquareHandler : IPacketHandler<GetDataHandlers.SendTileSquareEventArgs>
	{
		/// <summary>
		/// Maps grass-type blocks to flowers that can be grown on them with flower boots
		/// </summary>
		public static Dictionary<ushort, List<ushort>> GrassToPlantMap = new Dictionary<ushort, List<ushort>>
		{
			{ TileID.Grass, new List<ushort>            { TileID.Plants,         TileID.Plants2 } },
			{ TileID.HallowedGrass, new List<ushort>    { TileID.HallowedPlants, TileID.HallowedPlants2 } },
			{ TileID.JungleGrass, new List<ushort>      { TileID.JunglePlants,   TileID.JunglePlants2 } }
		};

		/// <summary>
		/// Item IDs that can spawn flowers while you walk
		/// </summary>
		public static List<int> FlowerBootItems = new List<int>
		{
			ItemID.FlowerBoots,
			ItemID.FairyBoots
		};

		/// <summary>
		/// Maps TileIDs to Tile Entity IDs.
		/// Note: <see cref="Terraria.ID.TileEntityID"/> is empty at the time of writing, but entities are dynamically assigned their ID at initialize time
		/// which is why we can use the _myEntityId field on each entity type
		/// </summary>
		public static Dictionary<int, int> TileEntityIdToTileIdMap = new Dictionary<int, int>
		{
			{ TileID.TargetDummy,        TETrainingDummy._myEntityID },
			{ TileID.ItemFrame,          TEItemFrame._myEntityID },
			{ TileID.LogicSensor,        TELogicSensor._myEntityID },
			{ TileID.DisplayDoll,        TEDisplayDoll._myEntityID },
			{ TileID.WeaponsRack2,       TEWeaponsRack._myEntityID },
			{ TileID.HatRack,            TEHatRack._myEntityID },
			{ TileID.FoodPlatter,        TEFoodPlatter._myEntityID },
			{ TileID.TeleportationPylon, TETeleportationPylon._myEntityID }
		};

		/// <summary>
		/// Invoked when a SendTileSquare packet is received
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// By default, we'll handle everything
			args.Handled = true;

			if (ShouldSkipProcessing(args))
			{
				return;
			}

			bool[,] processed = new bool[args.Size, args.Size];
			NetTile[,] tiles = ReadNetTilesFromStream(args.Data, args.Size);

			Debug.VisualiseTileSetDiff(args.TileX, args.TileY, args.Size, args.Size, tiles);

			IterateTileSquare(tiles, processed, args);

			// Uncommenting this function will send the same tile square 10 blocks above you for visualisation. This will modify your world and overwrite existing blocks.
			// Use in test worlds only.
			//Debug.DisplayTileSetInGame(tileX, tileY - 10, size, size, tiles, args.Player);

			// If we are handling this event then we have updated the server's Main.tile state the way we want it.
			// At this point we should send our state back to the client so they remain in sync with the server
			if (args.Handled == true)
			{
				args.Player.SendTileSquare(args.TileX, args.TileY, args.Size);
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare reimplemented from carbonara from {0}", args.Player.Name);
			}
		}

		/// <summary>
		/// Iterates over each tile in the tile square and performs processing on individual tiles or multi-tile Tile Objects
		/// </summary>
		/// <param name="tiles"></param>
		/// <param name="processed"></param>
		/// <param name="args"></param>
		internal void IterateTileSquare(NetTile[,] tiles, bool[,] processed, GetDataHandlers.SendTileSquareEventArgs args)
		{
			short size = args.Size;
			int tileX = args.TileX;
			int tileY = args.TileY;

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					// Do not process already processed tiles
					if (processed[x, y])
					{
						continue;
					}

					int realX = tileX + x;
					int realY = tileY + y;

					// Do not process tiles outside of the world boundaries
					if ((realX < 0 || realX >= Main.maxTilesX)
						|| (realY < 0 || realY > Main.maxTilesY))
					{
						processed[x, y] = true;
						continue;
					}

					// Do not process tiles that the player cannot update
					if (!args.Player.HasBuildPermission(realX, realY) ||
						!args.Player.IsInRange(realX, realY))
					{
						processed[x, y] = true;
						continue;
					}

					NetTile newTile = tiles[x, y];
					TileObjectData data;

					// If the new tile has an associated TileObjectData object, we take the tile and the surrounding tiles that make up the tile object
					// and process them as a tile object
					if (newTile.Type < TileObjectData._data.Count && TileObjectData._data[newTile.Type] != null)
					{
						data = TileObjectData._data[newTile.Type];
						NetTile[,] newTiles;
						int width = data.Width;
						int height = data.Height;
						int offsetY = 0;

						if (newTile.Type == TileID.TrapdoorClosed)
						{
							// Trapdoors can modify a 2x3 space. When it closes it will have leftover tiles either on top or bottom.
							// If we don't update these tiles, the trapdoor gets confused and disappears.
							// So we capture all 6 possible tiles and offset ourselves 1 tile above the closed trapdoor to capture the entire 2x3 area
							width = 2;
							height = 3;
							offsetY = -1;
						}

						// Ensure the tile object fits inside the square before processing it
						if (!DoesTileObjectFitInTileSquare(x, y, width, height, size, offsetY, processed))
						{
							continue;
						}

						newTiles = new NetTile[width, height];

						for (int i = 0; i < width; i++)
						{
							for (int j = 0; j < height; j++)
							{
								newTiles[i, j] = tiles[x + i, y + j + offsetY];
								processed[x + i, y + j + offsetY] = true;
							}
						}
						ProcessTileObject(newTile.Type, realX, realY + offsetY, width, height, newTiles, args);
						continue;
					}

					// If the new tile does not have an associated tile object, process it as an individual tile
					ProcessSingleTile(realX, realY, newTile, size, args);
					processed[x, y] = true;
				}
			}
		}

		/// <summary>
		/// Processes a tile object consisting of multiple tiles from the tile square packet
		/// </summary>
		/// <param name="tileType">The tile type the object is comprised of</param>
		/// <param name="newTiles">2D array of NetTile containing the new tiles properties</param>
		/// <param name="realX">X position at the top left of the object</param>
		/// <param name="realY">Y position at the top left of the object</param>
		/// <param name="width">Width of the tile object</param>
		/// <param name="height">Height of the tile object</param>
		/// <param name="args">SendTileSquareEventArgs containing event information</param>
		internal void ProcessTileObject(int tileType, int realX, int realY, int width, int height, NetTile[,] newTiles, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// As long as the player has permission to build, we should allow a tile object to be placed
			// More in depth checks should take place in handlers for the Place Object (79), Update Tile Entity (86), and Place Tile Entity (87) packets
			if (!args.Player.HasBuildPermissionForTileObject(realX, realY, width, height))
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from no permission for tile object from {0}", args.Player.Name);
				return;
			}
			
			if (TShock.TileBans.TileIsBanned((short)tileType))
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected for banned tile");
				return;
			}

			// Update all tiles in the tile object. These will be sent back to the player later
			UpdateMultipleServerTileStates(realX, realY, width, height, newTiles);

			// Tile entities have special placements that we should let the game deal with
			if (TileEntityIdToTileIdMap.ContainsKey(tileType))
			{
				TileEntity.PlaceEntityNet(realX, realY, TileEntityIdToTileIdMap[tileType]);
			}
		}

		/// <summary>
		/// Processes a single tile from the tile square packet
		/// </summary>
		/// <param name="realX">X position at the top left of the object</param>
		/// <param name="realY">Y position at the top left of the object</param>
		/// <param name="newTile">The NetTile containing new tile properties</param>
		/// <param name="squareSize">The size of the tile square being received</param>
		/// <param name="args">SendTileSquareEventArgs containing event information</param>
		internal void ProcessSingleTile(int realX, int realY, NetTile newTile, int squareSize, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// Some boots allow growing flowers on grass. This process sends a 1x1 tile square to grow the flowers
			// The square size must be 1 and the player must have an accessory that allows growing flowers in order for this square to be valid
			if (squareSize == 1 && args.Player.Accessories.Any(a => a != null && FlowerBootItems.Contains(a.type)))
			{
				ProcessFlowerBoots(realX, realY, newTile, args);
				return;
			}

			ITile tile = Main.tile[realX, realY];

			if (tile.type == TileID.LandMine && !newTile.Active)
			{
				UpdateServerTileState(tile, newTile);
			}

			if (tile.type == TileID.WirePipe)
			{
				UpdateServerTileState(tile, newTile);
			}

			ProcessConversionSpreads(Main.tile[realX, realY], newTile);

			// All other single tile updates should not be processed.
		}

		/// <summary>
		/// Applies changes to a tile if a tile square for flower-growing boots is valid
		/// </summary>
		/// <param name="realX">The tile x position of the tile square packet - this is where the flowers are intending to grow</param>
		/// <param name="realY">The tile y position of the tile square packet - this is where the flowers are intending to grow</param>
		/// <param name="newTile">The NetTile containing information about the flowers that are being grown</param>
		/// <param name="args">SendTileSquareEventArgs containing event information</param>
		internal void ProcessFlowerBoots(int realX, int realY, NetTile newTile, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// We need to get the tile below the tile square to determine what grass types are allowed
			if (!WorldGen.InWorld(realX, realY + 1))
			{
				// If the tile below the tile square isn't valid, we return here and don't update the server tile state
				return;
			}

			ITile tile = Main.tile[realX, realY + 1];
			if (!GrassToPlantMap.TryGetValue(tile.type, out List<ushort> plantTiles) && !plantTiles.Contains(newTile.Type))
			{
				// If the tile below the tile square isn't a valid plant tile (eg grass) then we don't update the server tile state
				return;
			}

			UpdateServerTileState(Main.tile[realX, realY], newTile);
		}

		/// <summary>
		/// Updates a single tile on the server if it is a valid conversion from one tile or wall type to another (eg stone -> corrupt stone)
		/// </summary>
		/// <param name="tile">The tile to update</param>
		/// <param name="newTile">The NetTile containing new tile properties</param>
		internal void ProcessConversionSpreads(ITile tile, NetTile newTile)
		{
			// Update if the existing tile or wall is convertible and the new tile or wall is a valid conversion
			if (
				((TileID.Sets.Conversion.Stone[tile.type] || Main.tileMoss[tile.type]) && (TileID.Sets.Conversion.Stone[newTile.Type] || Main.tileMoss[newTile.Type])) ||
				((tile.type == 0 || tile.type == 59) && (newTile.Type == 0 || newTile.Type == 59)) ||
				TileID.Sets.Conversion.Grass[tile.type] && TileID.Sets.Conversion.Grass[newTile.Type] ||
				TileID.Sets.Conversion.Ice[tile.type] && TileID.Sets.Conversion.Ice[newTile.Type] ||
				TileID.Sets.Conversion.Sand[tile.type] && TileID.Sets.Conversion.Sand[newTile.Type] ||
				TileID.Sets.Conversion.Sandstone[tile.type] && TileID.Sets.Conversion.Sandstone[newTile.Type] ||
				TileID.Sets.Conversion.HardenedSand[tile.type] && TileID.Sets.Conversion.HardenedSand[newTile.Type] ||
				TileID.Sets.Conversion.Thorn[tile.type] && TileID.Sets.Conversion.Thorn[newTile.Type] ||
				TileID.Sets.Conversion.Moss[tile.type] && TileID.Sets.Conversion.Moss[newTile.Type] ||
				TileID.Sets.Conversion.MossBrick[tile.type] && TileID.Sets.Conversion.MossBrick[newTile.Type] ||
				WallID.Sets.Conversion.Stone[tile.wall] && WallID.Sets.Conversion.Stone[newTile.Wall] ||
				WallID.Sets.Conversion.Grass[tile.wall] && WallID.Sets.Conversion.Grass[newTile.Wall] ||
				WallID.Sets.Conversion.Sandstone[tile.wall] && WallID.Sets.Conversion.Sandstone[newTile.Wall] ||
				WallID.Sets.Conversion.HardenedSand[tile.wall] && WallID.Sets.Conversion.HardenedSand[newTile.Wall] ||
				WallID.Sets.Conversion.PureSand[tile.wall] && WallID.Sets.Conversion.PureSand[newTile.Wall] ||
				WallID.Sets.Conversion.NewWall1[tile.wall] && WallID.Sets.Conversion.NewWall1[newTile.Wall] ||
				WallID.Sets.Conversion.NewWall2[tile.wall] && WallID.Sets.Conversion.NewWall2[newTile.Wall] ||
				WallID.Sets.Conversion.NewWall3[tile.wall] && WallID.Sets.Conversion.NewWall3[newTile.Wall] ||
				WallID.Sets.Conversion.NewWall4[tile.wall] && WallID.Sets.Conversion.NewWall4[newTile.Wall]
			)
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare processing a conversion update - [{0}|{1}] -> [{2}|{3}]", tile.type, tile.wall, newTile.Type, newTile.Wall);
				UpdateServerTileState(tile, newTile);
			}
		}

		/// <summary>
		/// Updates a single tile's world state with a change from the tile square packet
		/// </summary>
		/// <param name="tile">The tile to update</param>
		/// <param name="newTile">The NetTile containing the change</param>
		public static void UpdateServerTileState(ITile tile, NetTile newTile)
		{
			tile.active(newTile.Active);
			tile.type = newTile.Type;

			if (newTile.FrameImportant)
			{
				tile.frameX = newTile.FrameX;
				tile.frameY = newTile.FrameY;
			}

			if (newTile.HasWall)
			{
				tile.wall = newTile.Wall;
			}

			if (newTile.HasLiquid)
			{
				tile.liquid = newTile.Liquid;
				tile.liquidType(newTile.LiquidType);
			}

			tile.wire(newTile.Wire);
			tile.wire2(newTile.Wire2);
			tile.wire3(newTile.Wire3);
			tile.wire4(newTile.Wire4);

			tile.halfBrick(newTile.IsHalf);

			if (newTile.HasColor)
			{
				tile.color(newTile.TileColor);
			}

			if (newTile.HasWallColor)
			{
				tile.wallColor(newTile.WallColor);
			}

			byte slope = 0;
			if (newTile.Slope)
			{
				slope += 1;
			}
			if (newTile.Slope2)
			{
				slope += 2;
			}
			if (newTile.Slope3)
			{
				slope += 4;
			}

			tile.slope(slope);

			TShock.Log.ConsoleDebug("Bouncer / SendTileSquare updated a tile from type {0} to {1}", tile.type, newTile.Type);
		}

		/// <summary>
		/// Performs <see cref="UpdateServerTileState(ITile, NetTile)"/> on multiple tiles
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="newTiles"></param>
		public static void UpdateMultipleServerTileStates(int x, int y, int width, int height, NetTile[,] newTiles)
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					UpdateServerTileState(Main.tile[x + i, y + j], newTiles[i, j]);
				}
			}
		}

		/// <summary>
		/// Reads a set of NetTiles from a memory stream
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		static NetTile[,] ReadNetTilesFromStream(System.IO.MemoryStream stream, short size)
		{
			NetTile[,] tiles = new NetTile[size, size];
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					tiles[x, y] = new NetTile(stream);
				}
			}

			return tiles;
		}

		/// <summary>
		/// Determines whether or not the tile square should be immediately accepted or rejected
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		static bool ShouldSkipProcessing(GetDataHandlers.SendTileSquareEventArgs args)
		{
			if (args.Player.HasPermission(Permissions.allowclientsideworldedit))
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare accepted clientside world edit from {0}", args.Player.Name);
				args.Handled = false;
				return true;
			}

			// 7x7 is the largest vanilla-sized tile square (used for lamp posts). Anything larger than this should not be sent by the vanilla game and should be rejected
			if (args.Size > 7)
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from non-vanilla tilemod from {0}", args.Player.Name);
				return true;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from throttle from {0}", args.Player.Name);
				args.Player.SendTileSquare(args.TileX, args.TileY, args.Size);
				return true;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from being disabled from {0}", args.Player.Name);
				args.Player.SendTileSquare(args.TileX, args.TileY, args.Size);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Checks if a tile object fits inside the dimensions of a tile square
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="size"></param>
		/// <param name="offsetY"></param>
		/// <param name="processed"></param>
		/// <returns></returns>
		static bool DoesTileObjectFitInTileSquare(int x, int y, int width, int height, int size, int offsetY, bool[,] processed)
		{
			// If the starting y position of this tile object is at (x, 0) and the y offset is negative, we'll be accessing tiles outside the square
			if (y + offsetY < 0)
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquareHandler - rejected tile object because object dimensions fall outside the tile square (negative y value)");
				return false;
			}

			if (x + width >= size || y + height + offsetY >= size)
			{
				// This is ugly, but we want to mark all these tiles as processed so that we're not hitting this check multiple times for one dodgy tile object
				for (int i = x; i < size; i++)
				{
					for (int j = Math.Max(0, y + offsetY); j < size; j++) // This is also ugly. Using Math.Max to make sure y + offsetY >= 0
					{
						processed[i, j] = true;
					}
				}

				TShock.Log.ConsoleDebug("Bouncer / SendTileSquareHandler - rejected tile object because object dimensions fall outside the tile square (excessive size)");
				return false;
			}

			return true;
		}

		class Debug
		{
			/// <summary>
			/// Displays the difference in IDs between existing tiles and a set of NetTiles to the console
			/// </summary>
			/// <param name="tileX">X position at the top left of the square</param>
			/// <param name="tileY">Y position at the top left of the square</param>
			/// <param name="width">Width of the NetTile set</param>
			/// <param name="height">Height of the NetTile set</param>
			/// <param name="newTiles">New tiles to be visualised</param>
			public static void VisualiseTileSetDiff(int tileX, int tileY, int width, int height, NetTile[,] newTiles)
			{
				if (TShock.Config.DebugLogs)
				{
					char pad = '0';
					for (int y = 0; y < height; y++)
					{
						int realY = y + tileY;
						for (int x = 0; x < width; x++)
						{
							int realX = x + tileX;
							ushort type = Main.tile[realX, realY].type;
							string type2 = type.ToString();
							Console.Write((type2.ToString()).PadLeft(3, pad) + (Main.tile[realX, realY].active() ? "a" : "-") + " ");
						}
						Console.Write(" -> ");
						for (int x = 0; x < width; x++)
						{
							int realX = x + tileX;
							ushort type = newTiles[x, y].Type;
							string type2 = type.ToString();
							Console.Write((type2.ToString()).PadLeft(3, pad) + (newTiles[x, y].Active ? "a" : "-") + " ");
						}
						Console.Write("\n");
					}
				}
			}

			/// <summary>
			/// Sends a tile square at the given (tileX, tileY) coordinate, using the given set of NetTiles information to update the tile square
			/// </summary>
			/// <param name="tileX">X position at the top left of the square</param>
			/// <param name="tileY">Y position at the top left of the square</param>
			/// <param name="width">Width of the NetTile set</param>
			/// <param name="height">Height of the NetTile set</param>
			/// <param name="newTiles">New tiles to place in the square</param>
			/// <param name="player">Player to send the debug display to</param>
			public static void DisplayTileSetInGame(int tileX, int tileY, int width, int height, NetTile[,] newTiles, TSPlayer player)
			{
				for (int x = 0; x < width; x++)
				{
					for (int y = 0; y < height; y++)
					{
						UpdateServerTileState(Main.tile[tileX + x, tileY + y], newTiles[x, y]);
					}
					//Add a line of dirt blocks at the bottom for safety
					UpdateServerTileState(Main.tile[tileX + x, tileY + height], new NetTile { Active = true, Type = 0 });
				}

				player.SendTileSquare(tileX, tileY, Math.Max(width, height) + 1);
			}
		}
	}
}
