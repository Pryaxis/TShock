/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace TShockAPI
{
	public class UpdateManager
	{
		private string updateUrl = "https://github.com/NyxStudios/TShock/blob/general-devel/tshock_update.json?raw=true";

		/// <summary>
		/// Check once every X minutes.
		/// </summary>
		private readonly int CheckXMinutes = 30;

		public UpdateManager()
		{
			ThreadPool.QueueUserWorkItem(CheckForUpdates);
		}

		private void CheckForUpdates(object state)
		{
			try
			{
				UpdateCheck(state);
			}
			catch (Exception)
			{
				//swallow the exception
				return;
			}
			
			Thread.Sleep(CheckXMinutes * 60 * 1000);
			ThreadPool.QueueUserWorkItem(CheckForUpdates);
		}

		public void UpdateCheck(object o)
		{
			var updates = ServerIsOutOfDate();
			if (updates != null)
			{
				NotifyAdministrators(updates);
			}
		}

		/// <summary>
		/// Checks to see if the server is out of date.
		/// </summary>
		/// <returns></returns>
		private Dictionary<string, string> ServerIsOutOfDate()
		{
			var client = (HttpWebRequest)WebRequest.Create(updateUrl);
			client.Timeout = 5000;
			try
			{
				using (var resp = TShock.Utils.GetResponseNoException(client))
				{
					if (resp.StatusCode != HttpStatusCode.OK)
					{
						throw new IOException("Server did not respond with an OK.");
					}

					using(var reader = new StreamReader(resp.GetResponseStream()))
					{
						string updatejson = reader.ReadToEnd();
						var update = JsonConvert.DeserializeObject<Dictionary<string, string>>(updatejson);
						var version = new Version(update["version"]);
						if (TShock.VersionNum.CompareTo(version) < 0)
							return update;
					}
				}
			}
			catch (Exception e)
			{
				Log.ConsoleError("UpdateManager Exception: {0}", e);
				throw e;
			}

			return null;
		}

		private void NotifyAdministrators(Dictionary<string, string> update)
		{
			var changes = update["changes"].Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
			NotifyAdministrator(TSPlayer.Server, changes);
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.Active && player.Group.HasPermission(Permissions.maintenance))
				{
					NotifyAdministrator(player, changes);
				}
			}
		}

		private void NotifyAdministrator(TSPlayer player, string[] changes)
		{
			player.SendMessage("The server is out of date.", Color.Red);
			for (int j = 0; j < changes.Length; j++)
			{
				player.SendMessage(changes[j], Color.Red);
			}
		}
	}
}