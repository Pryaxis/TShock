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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Terraria;
using Terraria.IO;
using TerrariaApi.Server;

namespace TShockAPI
{
	internal class SaveManager
	{
		private SaveManager() 
		{
		}

		public static SaveManager Instance { get; } = new SaveManager();

		/// <summary>
		/// SaveWorld event handler which notifies users that the server may lag
		/// </summary>
		public void OnSaveWorld(WorldSaveEventArgs args)
		{
			if (TShock.Config.AnnounceSave)
			{
				// Protect against internal errors causing save failures
				// These can be caused by an unexpected error such as a bad or out of date plugin
				try
				{
					TShock.Utils.Broadcast("Saving world. Momentary lag might result from this.", Color.Red);
				}
				catch (Exception ex)
				{
					TShock.Log.Error("World saved notification failed");
					TShock.Log.Error(ex.ToString());
				}
			}
		}

		/// <summary>
		/// Saves the map data
		/// </summary>
		/// <param name="wait">wait for the pending save to finish (default: true)</param>
		/// <param name="resetTime">reset the last save time counter (default: false)</param>
		/// <param name="direct">use the realsaveWorld method instead of saveWorld event (default: false)</param>
		public void SaveWorld(bool wait = true, bool resetTime = false, bool direct = false)
		{
			var task = Task.Run(() =>
			{
				try
				{
					if (direct)
					{
						OnSaveWorld(new WorldSaveEventArgs());
						WorldFile.saveWorldDirect(resetTime);
					}
					else
						WorldFile.saveWorld(resetTime);
					TShock.Utils.Broadcast("World saved.", Color.Yellow);
					TShock.Log.Info($"World saved at ({Main.worldPathName})");
				}
				catch (Exception ex)
				{
					TShock.Log.Error("World saved failed");
					TShock.Log.Error(ex.ToString());
				}
			});

			if (wait)
			{
				task.Wait();
			}
		}
	}
}