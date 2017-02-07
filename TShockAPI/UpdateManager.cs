/*
TShock, a server mod for Terraria
Copyright (C) 2011-2016 Nyx Studios (fka. The TShock Team)

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
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.Xna.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace TShockAPI
{
	/// <summary>
	/// Responsible for checking for and notifying users about new updates to TShock
	/// </summary>
	public class UpdateManager
	{
		private string updateUrl = "http://update.tshock.co/latest/";
		private HttpClient _client = new HttpClient();

		/// <summary>
		/// Check once every X minutes.
		/// </summary>
		private int CheckXMinutes = 30;

		/// <summary>
		/// Creates a new instance of <see cref="UpdateManager"/> and starts the update thread
		/// </summary>
		public UpdateManager()
		{
			//5 second timeout
			_client.Timeout = new TimeSpan(0, 0, 5);

			Thread t = new Thread(async () => {
				do {
					await CheckForUpdatesAsync(null);	
				} while (true);
			});
			
			t.Name = "TShock Update Thread";
			t.IsBackground = true;
			
			t.Start();
		}

		private async Task CheckForUpdatesAsync(object state)
		{
			try
			{
				await UpdateCheckAsync(state);
				CheckXMinutes = 30;
			}
			catch (Exception ex)
			{
				// Skip this run and check more frequently...

				string msg = BuildExceptionString(ex);
				//Give the console a brief
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"UpdateManager warning: {msg}");
				Console.ForegroundColor = ConsoleColor.Gray;
				//And log the full exception
				TShock.Log.Warn($"UpdateManager warning: {ex.ToString()}");
				TShock.Log.ConsoleError("Retrying in 5 minutes.");
				CheckXMinutes = 5;
			}
			
			Thread.Sleep(CheckXMinutes * 60 * 1000);
		}

		/// <summary>
		/// Checks for updates to the TShock server
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public async Task UpdateCheckAsync(object o)
		{
			var updates = await ServerIsOutOfDateAsync();
			if (updates != null)
			{
				NotifyAdministrators(updates);
			}
		}

		/// <summary>
		/// Checks to see if the server is out of date.
		/// </summary>
		/// <returns></returns>
		private async Task<Dictionary<string, string>> ServerIsOutOfDateAsync()
		{
			var resp = await _client.GetAsync(updateUrl);
			if (resp.StatusCode != HttpStatusCode.OK)
			{
				string reason = resp.ReasonPhrase;
				if (string.IsNullOrWhiteSpace(reason))
				{
					reason = "none";
				}
				throw new WebException("Update server did not respond with an OK. "
					+ $"Server message: [error {resp.StatusCode}] {reason}");
			}

			string json = await resp.Content.ReadAsStringAsync();
			var update = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

			var version = new Version(update["version"]);
			if (TShock.VersionNum.CompareTo(version) < 0)
			{
				return update;
			}
			
			return null;
		}

		private void NotifyAdministrators(Dictionary<string, string> update)
		{
			var changes = update["changes"].Split(new[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
			NotifyAdministrator(TSPlayer.Server, changes);
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.Active && player.HasPermission(Permissions.maintenance))
				{
					NotifyAdministrator(player, changes);
				}
			}
		}

		private void NotifyAdministrator(TSPlayer player, string[] changes)
		{
			player.SendMessage("The server is out of date. Latest version: ", Color.Red);
			for (int j = 0; j < changes.Length; j++)
			{
				player.SendMessage(changes[j], Color.Red);
			}
		}

		/// <summary>
		/// Produces a string containing all exception messages in an exception chain.
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		private string BuildExceptionString(Exception ex)
		{
			string msg = ex.Message;
			Exception inner = ex.InnerException;
			while (inner != null)
			{
				msg += $"\r\n\t-> {inner.Message}";
				inner = inner.InnerException;
			}

			return msg;
		}
	}
}