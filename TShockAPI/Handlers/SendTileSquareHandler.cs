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
	public class SendTileSquareHandler
	{
		Dictionary<ushort, List<ushort>> _grassToPlantMap = new Dictionary<ushort, List<ushort>>
		{
			{ TileID.Grass, new List<ushort>            { TileID.Plants,         TileID.Plants2 } },
			{ TileID.HallowedGrass, new List<ushort>    { TileID.HallowedPlants, TileID.HallowedPlants2 } },
			{ TileID.JungleGrass, new List<ushort>      { TileID.JunglePlants,   TileID.JunglePlants2 } }
		};

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
		/// Updates a single tile
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="newTile"></param>
		public void UpdateTile(ITile tile, NetTile newTile)
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
		/// <param name="realx"></param>
		/// <param name="realy"></param>
		/// <param name="newTile"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		internal void HandleFlowerBoots(int realx, int realy, NetTile newTile, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// We need to get the tile below the tile square to determine what grass types are allowed
			if (!WorldGen.InWorld(realx, realy + 1))
			{
				return;
			}

			ITile tile = Main.tile[realx, realy + 1];
			if (!_grassToPlantMap.TryGetValue(tile.type, out List<ushort> plantTiles) && !plantTiles.Contains(newTile.Type))
			{
				return;
			}

			UpdateTile(Main.tile[realx, realy], newTile);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tile"></param>
		/// <param name="newTile"></param>
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

		internal void ProcessSingleTile(int realx, int realy, NetTile newTile, int squareSize, GetDataHandlers.SendTileSquareEventArgs args)
		{
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
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tileType"></param>
		/// <param name="data"></param>
		/// <param name="newTiles"></param>
		/// <param name="realx"></param>
		/// <param name="realy"></param>
		/// <param name="args"></param>
		public void ProcessTileObject(int tileType, TileObjectData data, NetTile[,] newTiles, int realx, int realy, GetDataHandlers.SendTileSquareEventArgs args)
		{
			// As long as the player has permission to build, we should allow a tile object to be placed
			// More in depth checks should take place in handlers for the Place Object (79), Update Tile Entity (86), and Place Tile Entity (87) packets
			if (!args.Player.HasBuildPermissionForTileObject(realx, realy, data.Width, data.Height))
			{
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare rejected from no permission for tile object from {0}", args.Player.Name);
				return;
			}

			if (tileType == TileID.ClosedDoor || tileType == TileID.OpenDoor)
			{
				ProcessDoor(realx, realy, newTiles, args);
				return;
			}

			for (int x = 0; x < data.Width; x++)
			{
				for (int y = 0; y < data.Height; y++)
				{
					// Update all tiles in the tile object
					UpdateTile(Main.tile[realx + x, realy + y], newTiles[x, y]);
				}
			}

			args.Handled = false;
		}

		private void ProcessDoor(int realx, int realy, NetTile[,] newTiles, GetDataHandlers.SendTileSquareEventArgs args)
		{
			//If we handle this then doors disappear... We only send back the tile square if args.Handled == true, so somewhere we're either breaking or not implementing the tile square properly
			///Calling UpdateTile on all tiles in newTiles doesn't help either.
			//Need to figure out what's differing from Vanilla in this regard
			args.Handled = false;
			TShock.Log.ConsoleDebug("Bouncer / SendTileSquare processing door from {0}", args.Player.Name);
		}

		static List<char> symbols = new List<char> { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y' };
		void VisualiseTileSquare(NetTile[,] tiles, int size)
		{
			Dictionary<int, char> tileToCharMap = new Dictionary<int, char>();
			int lastCharIndex = 0;

			StringBuilder sb = new StringBuilder("\n");

			for (int y = 0; y < size; y++)
			{
				for (int x = 0; x < size; x++)
				{
					NetTile tile = tiles[x, y];
					char symbol;
					if (tileToCharMap.ContainsKey(tile.Type))
					{
						symbol = tileToCharMap[tile.Type];
					}
					else
					{
						symbol = symbols[lastCharIndex++];
						tileToCharMap.Add(tile.Type, symbol);
					}

					sb.Append(symbol);
				}
				sb.Append("\n");
			}
			sb.Append("Key: " + String.Join(", ", tileToCharMap.Select(kvp => $"{kvp.Value} = {kvp.Key}")));

			TShock.Log.ConsoleDebug(sb.ToString());
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

			// From White:
			// IIRC it's because 5 means a 5x5 square which is normal for a tile square, and anything bigger is a non-vanilla tile modification attempt
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

			VisualiseTileSquare(tiles, size);

			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					TShock.Log.ConsoleDebug("Bouncer / SendTileSquare tile ({0}, {1})", x, y);

					// Do not handle already processed tiles
					if (processed[x, y])
					{
						TShock.Log.ConsoleDebug("Bouncer / SendTileSquare skipping processed tile from {0}", args.Player.Name);
						continue;
					}

					int realx = tileX + x;
					int realy = tileY + y;

					if ((realx < 0 || realx >= Main.maxTilesX)
						|| (realy < 0 || realy > Main.maxTilesY))
					{
						processed[x, y] = true;
						continue;
					}

					if (!args.Player.HasBuildPermission(realx, realy) ||
						!args.Player.IsInRange(realx, realy))
					{
						continue;
					}

					NetTile newTile = tiles[x, y];
					TileObjectData data;

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

						TShock.Log.ConsoleDebug("Bouncer / SendTileSquare processing tile object ({1}) from {0}", args.Player.Name, newTile.Type);
						ProcessTileObject(newTile.Type, data, newTiles, realx, realy, args);
						continue;
					}

					if (Main.tile[realx, realy].type == TileID.ClosedDoor || Main.tile[realx, realy].type == TileID.OpenDoor)
					{
						NetTile[,] newTiles = new NetTile[2, 3];
						for (int i = 0; i < 2; i++)
						{
							for (int j = 0; j < 3; j++)
							{
								newTiles[i, j] = tiles[x + i, y + j];
								processed[x + i, y + j] = true;
							}
						}

						ProcessDoor(realx, realy, newTiles, args);
						continue;
					}

					TShock.Log.ConsoleDebug("Bouncer / SendTileSquare processing single tile ({1}->{2}) from {0}", args.Player.Name, Main.tile[realx, realy].type, newTile.Type);
					ProcessSingleTile(realx, realy, newTile, size, args);
					processed[x, y] = true;
				}
			}

			if (args.Handled == true)
			{
				args.Player.SendTileSquare(tileX, tileY, size);
				TShock.Log.ConsoleDebug("Bouncer / SendTileSquare reimplemented from spaghetti from {0}", args.Player.Name);
			}
		}
	}

	/*
		bool changed = false;
		for (int x = 0; x < size; x++)
		{
			int realx = tileX + x;
			if (realx < 0 || realx >= Main.maxTilesX)
				continue;

			for (int y = 0; y < size; y++)
			{
				int realy = tileY + y;
				if (realy < 0 || realy >= Main.maxTilesY)
					continue;

				var tile = Main.tile[realx, realy];
				var newtile = tiles[x, y];

				// Junction Box
				if (tile.type == TileID.WirePipe)
				{
					args.Handled = false;
					return;
				}

				if (tile.active() && newtile.Active && tile.type != newtile.Type)
				{
					// Grass <-> Grass
					if ((TileID.Sets.Conversion.Grass[tile.type] && TileID.Sets.Conversion.Grass[newtile.Type]) ||
						// Dirt <-> Dirt
						((tile.type == 0 || tile.type == 59) &&
						(newtile.Type == 0 || newtile.Type == 59)) ||
						// Ice <-> Ice
						(TileID.Sets.Conversion.Ice[tile.type] && TileID.Sets.Conversion.Ice[newtile.Type]) ||
						// Stone <-> Stone
						((TileID.Sets.Conversion.Stone[tile.type] || Main.tileMoss[tile.type]) &&
						(TileID.Sets.Conversion.Stone[newtile.Type] || Main.tileMoss[newtile.Type])) ||
						// Sand <-> Sand
						(TileID.Sets.Conversion.Sand[tile.type] && TileID.Sets.Conversion.Sand[newtile.Type]) ||
						// Sandstone <-> Sandstone
						(TileID.Sets.Conversion.Sandstone[tile.type] && TileID.Sets.Conversion.Sandstone[newtile.Type]) ||
						// Hardened Sand <-> Hardened Sand
						(TileID.Sets.Conversion.HardenedSand[tile.type] && TileID.Sets.Conversion.HardenedSand[newtile.Type]))
					{
						Main.tile[realx, realy].type = newtile.Type;
						changed = true;
					}
				}

				// Stone wall <-> Stone wall
				if (((tile.wall == 1 || tile.wall == 3 || tile.wall == 28 || tile.wall == 83) &&
					(newtile.Wall == 1 || newtile.Wall == 3 || newtile.Wall == 28 || newtile.Wall == 83)) ||
					// Leaf wall <-> Leaf wall
					(((tile.wall >= 63 && tile.wall <= 70) || tile.wall == 81) &&
					((newtile.Wall >= 63 && newtile.Wall <= 70) || newtile.Wall == 81)))
				{
					Main.tile[realx, realy].wall = newtile.Wall;
					changed = true;
				}

				if ((tile.type == TileID.TrapdoorClosed && (newtile.Type == TileID.TrapdoorOpen || !newtile.Active)) ||
					(tile.type == TileID.TrapdoorOpen && (newtile.Type == TileID.TrapdoorClosed || !newtile.Active)) ||
					(!tile.active() && newtile.Active && (newtile.Type == TileID.TrapdoorOpen || newtile.Type == TileID.TrapdoorClosed)))
				{
					Main.tile[realx, realy].type = newtile.Type;
					Main.tile[realx, realy].frameX = newtile.FrameX;
					Main.tile[realx, realy].frameY = newtile.FrameY;
					Main.tile[realx, realy].active(newtile.Active);
					changed = true;
				}
			}
		}

		if (changed)
		{
			TSPlayer.All.SendTileSquare(tileX, tileY, size + 1);
			WorldGen.RangeFrame(tileX, tileY, tileX + size, tileY + size);
		}
		else
		{
			args.Player.SendTileSquare(tileX, tileY, size);
		}
	}*/
}
