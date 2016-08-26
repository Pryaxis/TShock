/*
TShock, a server mod for Terraria
Copyright (C) 2011-2014 Nyx Studios (fka. The TShock Team)

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
using System.Text;
using System.Net;
using System.Threading;
using System.IO;
using System.Web;
using TerrariaApi.Server;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TShockAPI
{
	public class StatTracker
	{
		[DllImport("kernel32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetPhysicallyInstalledSystemMemory(out long totalMemInKb);

		public string ProviderToken = "";
		public bool OptOut = false;

		private bool failed;
		private bool initialized;
		private long totalMem;
		private string serverId = "";

		public StatTracker()
		{
			
		}

		public void Initialize()
		{
			if (!initialized && !OptOut)
			{
				initialized = true;
				serverId = Guid.NewGuid().ToString(); // Gets reset every server restart
				// ThreadPool.QueueUserWorkItem(SendUpdate);
				Thread t = new Thread(() => {
					do {
						Thread.Sleep(1000 * 60 * 5);
						SendUpdate(null);
					} while(true);
				});
				t.IsBackground = true;
				t.Name = "TShock Stat Tracker Thread";
				t.Start();
			}
		}

		private void SendUpdate(object info)
		{
			JsonData data;

			if(ServerApi.RunningMono)
			{
				var pc = new PerformanceCounter("Mono Memory", "Total Physical Memory");
				totalMem = (pc.RawValue / 1024 / 1024 / 1024);
				data = new JsonData
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
			} else
			{
				GetPhysicallyInstalledSystemMemory(out totalMem);
				totalMem = (totalMem / 1024 / 1024); // Super hardcore maths to convert to Gb from Kb
				data = new JsonData
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

			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			var encoded = HttpUtility.UrlEncode(serialized);
			var uri = String.Format("http://stats.tshock.co/submit/{0}", encoded);
			var client = (HttpWebRequest)WebRequest.Create(uri);
			client.Timeout = 5000;
			try
			{
				using (var resp = TShock.Utils.GetResponseNoException(client))
				{
					if (resp.StatusCode != HttpStatusCode.OK)
					{
						throw new IOException("Server did not respond with an OK.");
					}

					failed = false;
				}
			}
			catch (Exception e)
			{
				if (!failed)
				{
					TShock.Log.ConsoleError("StatTracker Exception: {0}", e);
					failed = true;
				}
			}
		}
	}

	public struct JsonData
	{
		public int port;
		public int currentPlayers;
		public int maxPlayers;
		public long systemRam;
		public string version;
		public string terrariaVersion;
		public string providerId;
		public string serverId;
		public bool mono;
	}
}
