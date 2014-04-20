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

namespace TShockAPI
{
	public class StatTracker
	{
		private bool failed;
		private bool initialized;
		public StatTracker()
		{
			
		}

		public void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				ThreadPool.QueueUserWorkItem(SendUpdate);
			}
		}

		private HttpWebResponse GetResponseNoException(HttpWebRequest req)
		{
			try
			{
				return (HttpWebResponse)req.GetResponse();
			}
			catch (WebException we)
			{
				var resp = we.Response as HttpWebResponse;
				if (resp == null)
					throw;
				return resp;
			}
		}

		private void SendUpdate(object info)
		{
			Thread.Sleep(1000*60*15);
			var data = new JsonData
			{
				port = Terraria.Netplay.serverPort,
				currentPlayers = TShock.Utils.ActivePlayers(),
				maxPlayers = TShock.Config.MaxSlots,
				systemRam = 0,
				systemCPUClock = 0,
				version = TShock.VersionNum.ToString(),
				terrariaVersion = Terraria.Main.versionNumber2,
				mono = Terraria.Main.runningMono
			};

			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			var encoded = HttpUtility.UrlEncode(serialized);
			var uri = String.Format("http://96.47.231.227:8000?data={0}", encoded);
			var client = (HttpWebRequest)WebRequest.Create(uri);
			client.Timeout = 5000;
			try
			{
				using (var resp = GetResponseNoException(client))
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
					Log.ConsoleError("StatTracker Exception: {0}", e);
					failed = true;
				}
			}

			ThreadPool.QueueUserWorkItem(SendUpdate);
		}
	}

	public struct JsonData
	{
		public int port;
		public int currentPlayers;
		public int maxPlayers;
		public int systemRam;
		public int systemCPUClock;
		public string version;
		public string terrariaVersion;
		public bool mono;
	}
}
