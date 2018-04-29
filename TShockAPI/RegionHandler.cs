/*
TShock, a server mod for Terraria
Copyright (C) 2011-2018 Pryaxis & TShock Contributors
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

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Xna.Framework;
using TerrariaApi.Server;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace TShockAPI
{
	/// <summary>
	/// Represents TShock's Region subsystem. This subsystem is in charge of executing region related logic, such as
	/// setting temp points.
	/// </summary>
	internal sealed class RegionHandler
	{
		private readonly RegionManager _regionManager;
		private DateTime _lastCheck = DateTime.Now;

		/// <summary>
		/// Initializes a new instance of the <see cref="RegionHandler"/> class with the specified TShock instance and database connection.
		/// </summary>
		/// <param name="plugin">The <see cref="TShock"/> instance.</param>
		/// <param name="connection">The database connection.</param>
		public RegionHandler(TShock plugin, IDbConnection connection)
		{
			_regionManager = new RegionManager(connection);

			GetDataHandlers.GemLockToggle += OnGemLockToggle;
			GetDataHandlers.TileEdit += OnTileEdit;
			ServerApi.Hooks.GameUpdate.Register(plugin, OnGameUpdate);
		}

		private void OnGameUpdate(EventArgs args)
		{
			// Do not perform checks unless enough time has passed since the last execution.
			if ((DateTime.Now - _lastCheck).TotalSeconds < 1)
			{
				return;
			}

			foreach (var player in TShock.Players.Where(p => p?.Active == true))
			{
				// Store the player's last known region and update the current based on known regions at their coordinates.
				var oldRegion = player.CurrentRegion;
				player.CurrentRegion = _regionManager.GetTopRegion(_regionManager.InAreaRegion(player.TileX, player.TileY));

				// Do not fire any hooks if the player has not left and/or entered a region.
				if (player.CurrentRegion == oldRegion)
				{
					continue;
				}

				// Ensure that the player has left a region before invoking the RegionLeft event
				if (oldRegion != null)
				{
					RegionHooks.OnRegionLeft(player, oldRegion);
				}

				// Ensure that the player has entered a valid region before invoking the RegionEntered event 
				if (player.CurrentRegion != null)
				{
					RegionHooks.OnRegionEntered(player, player.CurrentRegion);
				}
			}

			// Set last execution time to this moment so we know when to execute the above code block again
			_lastCheck = DateTime.Now;
		}

		private void OnGemLockToggle(object sender, GetDataHandlers.GemLockToggleEventArgs e)
		{
			if (TShock.Config.RegionProtectGemLocks)
			{
				e.Handled = true;
			}
		}

		private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs e)
		{
			#region Region Information Display

			if (e.Player.AwaitingName)
			{
				bool includeUnprotected = false;
				bool includeZIndexes = false;
				bool persistentMode = false;

				foreach (string nameParameter in e.Player.AwaitingNameParameters)
				{
					// If this flag is passed the final output will include unprotected regions, i.e regions
					// that do not have the DisableBuild flag set to false
					if (nameParameter.Equals("-u", StringComparison.InvariantCultureIgnoreCase))
					{
						includeUnprotected = true;
					}

					// If this flag is passed the final output will include a region's Z index
					if (nameParameter.Equals("-z", StringComparison.InvariantCultureIgnoreCase))
					{
						includeZIndexes = true;
					}

					// If this flag is passed the player will continue to receive region information upon editing tiles
					if (nameParameter.Equals("-p", StringComparison.InvariantCultureIgnoreCase))
					{
						persistentMode = true;
					}
				}

				var output = new List<string>();
				foreach (Region region in _regionManager.Regions.OrderBy(r => r.Z).Reverse())
				{
					// Ensure that the specified tile is region protected
					if (e.X < region.Area.Left || e.X > region.Area.Right)
					{
						continue;
					}

					if (e.Y < region.Area.Top || e.X > region.Area.Bottom)
					{
						continue;
					}

					// Do not include the current region if it has not been protected and the includeProtected flag has not been set
					if (!region.DisableBuild && !includeUnprotected)
					{
						continue;
					}

					output.Add($"{region.Name} {(includeZIndexes ? $"(Z:{region.Z}" : string.Empty)}");
				}

				if (output.Count == 0)
				{
					e.Player.SendInfoMessage(includeUnprotected
						? "There are no regions at this point."
						: "There are no regions at this point, or they are not protected.");
				}
				else
				{
					e.Player.SendInfoMessage(includeUnprotected ? "Regions at this point: " : "Protected regions at this point: ");

					foreach (string line in PaginationTools.BuildLinesFromTerms(output))
					{
						e.Player.SendMessage(line, Color.White);
					}
				}

				if (!persistentMode)
				{
					e.Player.AwaitingName = false;
					e.Player.AwaitingNameParameters = null;
				}

				// Revert all tile changes and handle the event
				e.Player.SendTileSquare(e.X, e.Y, 4);
				e.Handled = true;
			}

			#endregion

			#region TempPoints Setup

			if (e.Player.AwaitingTempPoint != 0)
			{
				// Set temp point coordinates to current tile coordinates
				e.Player.TempPoints[e.Player.AwaitingTempPoint - 1].X = e.X;
				e.Player.TempPoints[e.Player.AwaitingTempPoint - 1].Y = e.Y;
				e.Player.SendInfoMessage($"Set temp point {e.Player.AwaitingTempPoint}.");

				e.Player.AwaitingTempPoint = 0;

				// Revert all tile changes and handle the event
				e.Player.SendTileSquare(e.X, e.Y, 4);
				e.Handled = true;
			}

			#endregion
		}
	}
}
