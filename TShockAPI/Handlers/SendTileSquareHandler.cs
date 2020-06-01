using OTAPI.Tile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.ID;
using Terraria.ObjectData;
using TShockAPI.Net;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Provides processors and handling for Tile Square packets
	/// </summary>
	public class SendTileSquareHandler
	{
		/// <summary>
		/// Maps grass-type blocks to flowers that can be grown on them with flower boots
		/// </summary>
		Dictionary<ushort, List<ushort>> _grassToPlantMap = new Dictionary<ushort, List<ushort>>
		{
			{ TileID.Grass, new List<ushort>            { TileID.Plants,         TileID.Plants2 } },
			{ TileID.HallowedGrass, new List<ushort>    { TileID.HallowedPlants, TileID.HallowedPlants2 } },
			{ TileID.JungleGrass, new List<ushort>      { TileID.JunglePlants,   TileID.JunglePlants2 } }
		};

		/// <summary>
		/// Item IDs that can spawn flowers while you walk
		/// </summary>
		List<int> _flowerBootItems = new List<int>
		{
			ItemID.FlowerBoots,
			ItemID.FairyBoots
		};

		Dictionary<int, int> _tileEntityIdToTileIdMap = new Dictionary<int, int>
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
		/// Syncs a single tile on the server with a tile from the tile square packet
		/// </summary>
		/// <param name="tile">The tile to update</param>
		/// <param name="newTile">The NetTile containing new tile properties</param>
		public static void UpdateTile(ITile tile, NetTile newTile)
		{
			tile.active(newTile.Active);

			if (newTile.Active && !newTile.Inactive)
			{
				tile.type = newTile.Type;
			}

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
		///  Determines if a Tile Square for flower-growing boots should be accepted or not
		/// </summary>
		/// <param name="realx">The tile x position of the tile square packet - this is where the flowers are intending to grow</param>
		/// <param name="realy">The tile y position of the tile square packet - this is where the flowers are intending to grow</param>
		/// <param name="newTile">The NetTile containing information about the flowers that are being grown</param>
		/// <param name="args">SendTileSquareEventArgs containing event information</param>
		internal void HandleFlowerBoots(int realx, int realy, NetTile newTile, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// We need to get the tile below the tile square to determine what grass types are allowed
			if (!WorldGen.InWorld(realx, realy + 1))
			{
				// If the tile below the tile square isn't valid, we return here and don't update the server tile state
				return;
			}

			ITile tile = Main.tile[realx, realy + 1];
			if (!_grassToPlantMap.TryGetValue(tile.type, out List<ushort> plantTiles) && !plantTiles.Contains(newTile.Type))
			{
				// If the tile below the tile square isn't a valid plant tile (eg grass) then we don't update the server tile state
				return;
			}

			UpdateTile(Main.tile[realx, realy], newTile);
		}

		/// <summary>
		/// Updates a single tile on the server if it is a valid conversion from one tile or wall type to another (eg stone -> corrupt stone)
		/// </summary>
		/// <param name="tile">The tile to update</param>
		/// <param name="newTile">The NetTile containing new tile properties</param>
		internal void HandleConversionSpreads(ITile tile, NetTile newTile)
		{
			// Update if the existing tile or wall is convertible and the new tile or wall is a valid conversion
			if (
				((TileID.Sets.Conversion.Stone[tile.type] || Main.tileMoss[tile.type]) && (TileID.Sets.Conversion.Stone[newTile.Type] || Main.tileMoss[newTile.Type])) ||
				((tile.type == 0 || tile.type == 59) && (newTile.Type == 0 || newTile.Type == 59)) ||
				TileID.Sets.Conversion.Grass[tile.type]			&& TileID.Sets.Conversion.Grass[newTile.Type] ||
				TileID.Sets.Conversion.Ice[tile.type]			&& TileID.Sets.Conversion.Ice[newTile.Type] ||
				TileID.Sets.Conversion.Sand[tile.type]			&& TileID.Sets.Conversion.Sand[newTile.Type] ||
				TileID.Sets.Conversion.Sandstone[tile.type]		&& TileID.Sets.Conversion.Sandstone[newTile.Type] ||
				TileID.Sets.Conversion.HardenedSand[tile.type]	&& TileID.Sets.Conversion.HardenedSand[newTile.Type] ||
				TileID.Sets.Conversion.Thorn[tile.type]			&& TileID.Sets.Conversion.Thorn[newTile.Type] ||
				TileID.Sets.Conversion.Moss[tile.type]			&& TileID.Sets.Conversion.Moss[newTile.Type] ||
				TileID.Sets.Conversion.MossBrick[tile.type]		&& TileID.Sets.Conversion.MossBrick[newTile.Type] ||
				WallID.Sets.Conversion.Stone[tile.wall]			&& WallID.Sets.Conversion.Stone[newTile.Wall] ||
				WallID.Sets.Conversion.Grass[tile.wall]			&& WallID.Sets.Conversion.Grass[newTile.Wall] ||
				WallID.Sets.Conversion.Sandstone[tile.wall]		&& WallID.Sets.Conversion.Sandstone[newTile.Wall] ||
				WallID.Sets.Conversion.HardenedSand[tile.wall]	&& WallID.Sets.Conversion.HardenedSand[newTile.Wall] ||
				WallID.Sets.Conversion.PureSand[tile.wall]		&& WallID.Sets.Conversion.PureSand[newTile.Wall] ||
				WallID.Sets.Conversion.NewWall1[tile.wall]		&& WallID.Sets.Conversion.NewWall1[newTile.Wall] ||
				WallID.Sets.Conversion.NewWall2[tile.wall]		&& WallID.Sets.Conversion.NewWall2[newTile.Wall] ||
				WallID.Sets.Conversion.NewWall3[tile.wall]		&& WallID.Sets.Conversion.NewWall3[newTile.Wall] ||
				WallID.Sets.Conversion.NewWall4[tile.wall]		&& WallID.Sets.Conversion.NewWall4[newTile.Wall]
			)
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare processing a conversion update - [{0}|{1}] -> [{2}|{3}]", tile.type, tile.wall, newTile.Type, newTile.Wall);
				UpdateTile(tile, newTile);
			}
		}

		/// <summary>
		/// Processes a single tile from the tile square packet
		/// </summary>
		/// <param name="realx">X position at the top left of the object</param>
		/// <param name="realy">Y position at the top left of the object</param>
		/// <param name="newTile">The NetTile containing new tile properties</param>
		/// <param name="squareSize">The size of the tile square being received</param>
		/// <param name="args">SendTileSquareEventArgs containing event information</param>
		internal void ProcessSingleTile(int realx, int realy, NetTile newTile, int squareSize, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// Some boots allow growing flowers on grass. This process sends a 1x1 tile square to grow the flowers
			// The square size must be 1 and the player must have an accessory that allows growing flowers in order for this square to be valid
			if (squareSize == 1 && args.Player.Accessories.Any(a => a != null && _flowerBootItems.Contains(a.type)))
			{
				HandleFlowerBoots(realx, realy, newTile, args);
				return;
			}

			ITile tile = Main.tile[realx, realy];

			if (tile.type == TileID.LandMine && !newTile.Active)
			{
				UpdateTile(tile, newTile);
			}

			if (tile.type == TileID.WirePipe)
			{
				UpdateTile(tile, newTile);
			}

			HandleConversionSpreads(Main.tile[realx, realy], newTile);

			// All other single tile updates should not be processed.
		}

		/// <summary>
		/// Processes a tile object consisting of multiple tiles from the tile square packet
		/// </summary>
		/// <param name="tileType">The tile type the object is comprised of</param>
		/// <param name="data">TileObjectData describing the tile object</param>
		/// <param name="newTiles">2D array of NetTile containing the new tiles properties</param>
		/// <param name="realx">X position at the top left of the object</param>
		/// <param name="realy">Y position at the top left of the object</param>
		/// <param name="args">SendTileSquareEventArgs containing event information</param>
		public void ProcessTileObject(int tileType, TileObjectData data, NetTile[,] newTiles, int realx, int realy, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// As long as the player has permission to build, we should allow a tile object to be placed
			// More in depth checks should take place in handlers for the Place Object (79), Update Tile Entity (86), and Place Tile Entity (87) packets
			if (!args.Player.HasBuildPermissionForTileObject(realx, realy, data.Width, data.Height))
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from no permission for tile object from {0}", args.Player.Name);
				return;
			}

			for (int x = 0; x < data.Width; x++)
			{
				for (int y = 0; y < data.Height; y++)
				{
					// Update all tiles in the tile object. These will be synced back to the player later
					UpdateTile(Main.tile[realx + x, realy + y], newTiles[x, y]);
				}
			}

			// Tile entities have special placements that we should let the game deal with
			if (_tileEntityIdToTileIdMap.ContainsKey(tileType))
			{
				TileEntity.PlaceEntityNet(realx, realy, _tileEntityIdToTileIdMap[tileType]);
			}
		}

		/// <summary>
		/// Invoked when a SendTileSquare packet is received
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceiveSendTileSquare(object sender, GetDataHandlers.SendTileSquareEventArgs args)
		{
			short size = args.Size;
			int tileX = args.TileX;
			int tileY = args.TileY;

			// By default, we'll handle everything
			args.Handled = true;

			if (args.Player.HasPermission(Permissions.allowclientsideworldedit))
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare accepted clientside world edit from {0}", args.Player.Name);
				args.Handled = false;
				return;
			}

			// 5x5 is the largest vanilla-sized tile square. Anything larger than this should not be seen in the vanilla game and should be rejected
			if (size > 5)
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from non-vanilla tilemod from {0}", args.Player.Name);
				return;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from throttle from {0}", args.Player.Name);
				args.Player.SendTileSquare(tileX, tileY, size);
				return;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from being disabled from {0}", args.Player.Name);
				args.Player.SendTileSquare(tileX, tileY, size);
				return;
			}

			bool[,] processed = new bool[size, size];
			NetTile[,] tiles = new NetTile[size, size];
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					tiles[x, y] = new NetTile(args.Data);
				}
			}

			Debug.VisualiseTileSetDiff(tileX, tileY, size, size, tiles);

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					// Do not process already processed tiles
					if (processed[x, y])
					{
						continue;
					}

					int realx = tileX + x;
					int realy = tileY + y;

					// Do not process tiles outside of the world boundaries
					if ((realx < 0 || realx >= Main.maxTilesX)
						|| (realy < 0 || realy > Main.maxTilesY))
					{
						processed[x, y] = true;
						continue;
					}

					// Do not process tiles that the player cannot update
					if (!args.Player.HasBuildPermission(realx, realy) ||
						!args.Player.IsInRange(realx, realy))
					{
						continue;
					}

					NetTile newTile = tiles[x, y];
					TileObjectData data;

					// If the new tile has an associated TileObjectData object, we take the tile and the surrounding tiles that make up the tile object
					// and process them as a tile object
					if (newTile.Type < TileObjectData._data.Count && (data = TileObjectData._data[newTile.Type]) != null)
					{
						NetTile[,] newTiles = new NetTile[data.Width, data.Height];
						for (int i = 0; i < data.Width; i++)
						{
							for (int j = 0; j < data.Height; j++)
							{
								newTiles[i, j] = tiles[x + i, y + j];
								processed[x + i, y + j] = true;
							}
						}

						ProcessTileObject(newTile.Type, data, newTiles, realx, realy, args);
						continue;
					}

					// If the new tile does not have an associated tile object, process it as an individual tile
					ProcessSingleTile(realx, realy, newTile, size, args);
					processed[x, y] = true;
				}
			}

			// Uncommenting this function will send the same tile square 10 blocks above you for visualisation. This will modify your world and overwrite existing blocks.
			// Use in test worlds only.
			//Debug.DisplayTileSetInGame(tileX, tileY - 10, size, size, tiles, args.Player);

			// If we are handling this event then we have updated the server's Main.tile state the way we want it.
			// At this point we should send our state back to the client so they remain in sync with the server
			if (args.Handled == true)
			{
				args.Player.SendTileSquare(tileX, tileY, size);
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare reimplemented from spaghetti from {0}", args.Player.Name);
			}
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
							Console.Write((type2.ToString()).PadLeft(3, pad) + " ");
						}
						Console.Write(" -> ");
						for (int x = 0; x < width; x++)
						{
							int realX = x + tileX;
							if (newTiles[x, y].Active)
							{
								ushort type = newTiles[x, y].Type;
								string type2 = type.ToString();
								Console.Write((type2.ToString()).PadLeft(3, pad) + " ");
							}
							else
							{
								ushort type = Main.tile[realX, realY].type;
								string type2 = type.ToString();
								Console.Write((type2.ToString()).PadLeft(3, pad) + " ");
							}
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
						UpdateTile(Main.tile[tileX + x, tileY + y], newTiles[x, y]);
					}
					//Add a line of dirt blocks at the bottom for safety
					UpdateTile(Main.tile[tileX + x, tileY + height], new NetTile { Active = true, Type = 0 });
				}

				player.SendTileSquare(tileX, tileY, Math.Max(width, height) + 1);
			}
		}
	}
}
