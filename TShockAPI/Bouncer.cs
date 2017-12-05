/*
TShock, a server mod for Terraria
Copyright (C) 2011-2017 Nyx Studios (fka. The TShock Team)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using Terraria;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using TShockAPI;
using TerrariaApi.Server;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using TShockAPI.Net;

namespace TShockAPI
{
	/// <summary>Bouncer - The TShock anti-hack and build guardian system</summary>
	public class Bouncer
	{
		/// <summary>Bouncer - Constructor call initializes Bouncer & related functionality.</summary>
		/// <returns>A new Bouncer.</returns>
		public Bouncer(TerrariaPlugin pluginInstance)
		{
			// Setup hooks

			// SendTileSquareEventArgs args = new GetDataHandlers.SendTileSquareEventArgs();
			GetDataHandlers.SendTileSquare.Register(OnSendTileSquare);
		}

		/// <summary>OnSendTileSquare - The handler for SendTileSquare events in Bouncer</summary>
		/// <param name="sender">sender</param>
		/// <param name="args">args</param>
		internal void OnSendTileSquare(object sender, GetDataHandlers.SendTileSquareEventArgs args)
		{
			short size = args.Size;
			int tileX = args.TileX;
			int tileY = args.TileY;

			if (args.Player.HasPermission(Permissions.allowclientsideworldedit))
			{
				args.Handled = false;
				return;
			}

			if (size > 5)
			{
				args.Handled = true;
				return;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendTileSquare(tileX, tileY, size);
				args.Handled = true;
				return;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY, size);
				args.Handled = true;
				return;
			}

			try
			{
				var tiles = new NetTile[size, size];
				for (int x = 0; x < size; x++)
				{
					for (int y = 0; y < size; y++)
					{
						tiles[x, y] = new NetTile(args.Data);
					}
				}

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
						if (TShock.CheckTilePermission(args.Player, realx, realy) ||
							TShock.CheckRangePermission(args.Player, realx, realy))
						{
							continue;
						}

						// Fixes the Flower Boots not creating flowers issue
						if (size == 1 && args.Player.Accessories.Any(i => i.active && i.netID == ItemID.FlowerBoots))
						{
							if (Main.tile[realx, realy + 1].type == TileID.Grass && (newtile.Type == TileID.Plants || newtile.Type == TileID.Plants2))
							{
								args.Handled = false;
								return;
							}

							if (Main.tile[realx, realy + 1].type == TileID.HallowedGrass && (newtile.Type == TileID.HallowedPlants || newtile.Type == TileID.HallowedPlants2))
							{
								args.Handled = false;
								return;
							}

							if (Main.tile[realx, realy + 1].type == TileID.JungleGrass && newtile.Type == TileID.JunglePlants2)
							{
								args.Handled = false;
								return;
							}
						}

						// Junction Box
						if (tile.type == TileID.WirePipe)
						{
							args.Handled = false;
							return;
						}

						// Orientable tiles
						if (tile.type == newtile.Type && orientableTiles.Contains(tile.type))
						{
							Main.tile[realx, realy].frameX = newtile.FrameX;
							Main.tile[realx, realy].frameY = newtile.FrameY;
							changed = true;
						}
						// Landmine
						if (tile.type == TileID.LandMine && !newtile.Active)
						{
							Main.tile[realx, realy].active(false);
							changed = true;
						}
						// Sensors
						if(newtile.Type == TileID.LogicSensor && !Main.tile[realx, realy].active())
						{
							Main.tile[realx, realy].type = newtile.Type;
							Main.tile[realx, realy].frameX = newtile.FrameX;
							Main.tile[realx, realy].frameY = newtile.FrameY;
							Main.tile[realx, realy].active(true);
							changed = true;
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
							(!tile.active() && newtile.Active && (newtile.Type == TileID.TrapdoorOpen||newtile.Type == TileID.TrapdoorClosed)))
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
			}
			catch
			{
				args.Player.SendTileSquare(tileX, tileY, size);
			}
			args.Handled = true;
			return;
		}

		private static int[] orientableTiles = new int[]
		{
			TileID.Cannon,
			TileID.Chairs,
			TileID.Beds,
			TileID.Bathtubs,
			TileID.Statues,
			TileID.Mannequin,
			TileID.Traps,
			TileID.MusicBoxes,
			TileID.ChristmasTree,
			TileID.WaterFountain,
			TileID.Womannequin,
			TileID.MinecartTrack,
			TileID.WeaponsRack,
			TileID.ItemFrame,
			TileID.LunarMonolith,
			TileID.TargetDummy,
			TileID.Campfire
		};

	}
}