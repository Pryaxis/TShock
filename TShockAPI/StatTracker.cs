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
using System.Net;
using System.Threading;
using System.Web;
using TerrariaApi.Server;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Threading.Tasks;
using TShockAPI.Extensions;

namespace TShockAPI
{
	/// <summary>
	/// StatTracker class for tracking various server metrics
	/// </summary>
	public class StatTracker
	{
		/// <summary>
		/// Calls the GetPhysicallyInstalledSystemMemory Windows API function, returning the amount of RAM installed on the host PC
		/// </summary>
		/// <param name="totalMemInKb">The amount of memory (in kilobytes) present on the host PC</param>
		/// <returns></returns>
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemInKb);

		/// <summary>
		/// A provider token set on the command line. Used by the stats server to group providers
		/// </summary>
		public string ProviderToken = "";
		/// <summary>
		/// Whether or not to opt out of stat tracking
		/// </summary>
		public bool OptOut = false;

		private long totalMem = 0;
		private string serverId = "";
		private HttpClient _client;

		private const string TrackerUrl = "http://stats.tshock.co/submit/{0}";

		/// <summary>
		/// Creates a new instance of <see cref="StatTracker"/>
		/// </summary>
		public StatTracker()
		{
		}

		/// <summary>
		/// Starts the stat tracker
		/// </summary>
		public void Start()
		{
			if (OptOut)
			{
				//If opting out, return and do not start stat tracking
				return;
			}

			//HttpClient with a 5 second timeout
			_client = new HttpClient()
			{
				Timeout = new TimeSpan(0, 0, 5)
			};
			serverId = Guid.NewGuid().ToString();

			Thread t = new Thread(async () =>
			{
				do
				{
					//Wait 5 minutes
					await Task.Delay(1000 * 60 * 5);
					//Then update again
					await SendUpdateAsync();
				} while (true);
			})
			{
				Name = "TShock Stat Tracker Thread",
				IsBackground = true
			};
			t.Start();
		}

		private async Task SendUpdateAsync()
		{
			JsonData data = PrepareJsonData();

			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			var encoded = HttpUtility.UrlEncode(serialized);
			var uri = String.Format(TrackerUrl, encoded);

			try
			{
				HttpResponseMessage msg = await _client.GetAsync(uri);
				if (msg.StatusCode != HttpStatusCode.OK)
				{
					string reason = msg.ReasonPhrase;
					if (string.IsNullOrWhiteSpace(reason))
					{
						reason = "none";
					}
					throw new WebException("Stats server did not respond with an OK. "
						+ $"Server message: [error {msg.StatusCode}] {reason}");
				}
			}
			catch (Exception ex)
			{
				string msg = ex.BuildExceptionString();

				//Give the console a brief
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine($"StatTracker warning: {msg}");
				Console.ForegroundColor = ConsoleColor.Gray;

				//And log the full exception
				TShock.Log.Warn($"StatTracker warning: {ex.ToString()}");
				TShock.Log.ConsoleError("Retrying in 5 minutes.");
			}
		}

		private JsonData PrepareJsonData()
		{
			if (ServerApi.RunningMono)
			{
				if (totalMem == 0)
				{
					//Only do this if we haven't already cached the total physicaly memory
					var pc = new PerformanceCounter("Mono Memory", "Total Physical Memory");
					totalMem = (pc.RawValue / 1024 / 1024 / 1024);
				}

				return new JsonData
				{
					port = Terraria.Netplay.ListenPort,
					currentPlayers = TShock.Utils.ActivePlayers(),
					maxPlayers = TShock.Config.MaxSlots,
					systemRam = totalMem,
					version = TShock.VersionNum.ToString(),
					terrariaVersion = Terraria.Main.versionNumber2,
					providerId = ProviderToken,
					serverId = serverId,
					mono = true
				};
			}

			if (totalMem == 0)
			{
				//Again, only call this if it hasn't previously been cached
				GetPhysicallyInstalledSystemMemory(out totalMem);
				totalMem = (totalMem / 1024 / 1024); // Super hardcore maths to convert to Gb from Kb
			}

			return new JsonData
			{
				port = Terraria.Netplay.ListenPort,
				currentPlayers = TShock.Utils.ActivePlayers(),
				maxPlayers = TShock.Config.MaxSlots,
				systemRam = totalMem,
				version = TShock.VersionNum.ToString(),
				terrariaVersion = Terraria.Main.versionNumber2,
				providerId = ProviderToken,
				serverId = serverId,
				mono = false
			};
		}
	}

	/// <summary>
	/// Contains JSON-Serializable information about a server
	/// </summary>
	public struct JsonData
	{
		/// <summary>
		/// The port the server is running on
		/// </summary>
		public int port;
		/// <summary>
		/// The number of players currently on the server
		/// </summary>
		public int currentPlayers;
		/// <summary>
		/// The maximum number of player slots available on the server
		/// </summary>
		public int maxPlayers;
		/// <summary>
		/// The amount of RAM installed on the server's host PC
		/// </summary>
		public long systemRam;
		/// <summary>
		/// The TShock version being used by the server
		/// </summary>
		public string version;
		/// <summary>
		/// The Terraria version supported by the server
		/// </summary>
		public string terrariaVersion;
		/// <summary>
		/// The provider ID set for the server
		/// </summary>
		public string providerId;
		/// <summary>
		/// The server ID set for the server
		/// </summary>
		public string serverId;
		/// <summary>
		/// Whether or not the server is running with Mono
		/// </summary>
		public bool mono;
	}
}
