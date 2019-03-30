/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors
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
using System.Linq;
using Microsoft.Xna.Framework;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace TShockAPI
{
	/// <summary>
	/// Represents TShock's Region subsystem. This subsystem is in charge of executing region related logic, such as
	/// setting temp points or invoking region events.
	/// </summary>
	internal sealed class RegionHandler : IDisposable
	{
		private readonly RegionManager _regionManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="RegionHandler"/> class with the specified <see cref="RegionManager"/> instance.
		/// </summary>
		/// <param name="regionManager">The <see cref="RegionManager"/> instance.</param>
		public RegionHandler(RegionManager regionManager)
		{
			_regionManager = regionManager;

			GetDataHandlers.GemLockToggle += OnGemLockToggle;
			GetDataHandlers.PlayerUpdate += OnPlayerUpdate;
			GetDataHandlers.TileEdit += OnTileEdit;
		}

		/// <summary>
		/// Disposes the region handler.
		/// </summary>
		public void Dispose()
		{
			GetDataHandlers.GemLockToggle -= OnGemLockToggle;
			GetDataHandlers.PlayerUpdate -= OnPlayerUpdate;
			GetDataHandlers.TileEdit -= OnTileEdit;
		}

		private void OnGemLockToggle(object sender, GetDataHandlers.GemLockToggleEventArgs e)
		{
			if (TShock.Config.RegionProtectGemLocks)
			{
				e.Handled = true;
			}
		}

		private void OnPlayerUpdate(object sender, GetDataHandlers.PlayerUpdateEventArgs e)
		{
			var player = e.Player;

			// Store the player's last known region and update the current based on known regions at their coordinates.
			var oldRegion = player.CurrentRegion;
			player.CurrentRegion = _regionManager.GetTopRegion(_regionManager.InAreaRegion(player.TileX, player.TileY));

			// Do not fire any hooks if the player has not left and/or entered a region.
			if (player.CurrentRegion == oldRegion)
			{
				return;
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

		private void OnTileEdit(object sender, GetDataHandlers.TileEditEventArgs e)
		{
			var player = e.Player;

			#region Region Information Display

			if (player.AwaitingName)
			{
				bool includeUnprotected = false;
				bool includeZIndexes = false;
				bool persistentMode = false;

				foreach (string nameParameter in player.AwaitingNameParameters)
				{
					// If this flag is passed the final output will include unprotected regions, i.e regions
					// that have the DisableBuild flag set to false
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

					if (e.Y < region.Area.Top || e.Y > region.Area.Bottom)
					{
						continue;
					}

					// Do not include the current region if it has not been protected and the includeUnprotected flag has not been set
					if (!region.DisableBuild && !includeUnprotected)
					{
						continue;
					}

					output.Add($"{region.Name} {(includeZIndexes ? $"(Z:{region.Z}" : string.Empty)}");
				}

				if (output.Count == 0)
				{
					player.SendInfoMessage(includeUnprotected
						? "There are no regions at this point."
						: "There are no regions at this point, or they are not protected.");
				}
				else
				{
					player.SendInfoMessage(includeUnprotected ? "Regions at this point: " : "Protected regions at this point: ");

					foreach (string line in PaginationTools.BuildLinesFromTerms(output))
					{
						player.SendMessage(line, Color.White);
					}
				}

				if (!persistentMode)
				{
					player.AwaitingName = false;
					player.AwaitingNameParameters = null;
				}

				// Revert all tile changes and handle the event
				player.SendTileSquare(e.X, e.Y, 4);
				e.Handled = true;
			}

			#endregion

			#region TempPoints Setup

			if (player.AwaitingTempPoint != 0)
			{
				// Set temp point coordinates to current tile coordinates
				player.TempPoints[player.AwaitingTempPoint - 1].X = e.X;
				player.TempPoints[player.AwaitingTempPoint - 1].Y = e.Y;
				player.SendInfoMessage($"Set temp point {player.AwaitingTempPoint}.");

				// Reset the awaiting temp point
				player.AwaitingTempPoint = 0;

				// Revert all tile changes and handle the event
				player.SendTileSquare(e.X, e.Y, 4);
				e.Handled = true;
			}

			#endregion
		}
	}
}
