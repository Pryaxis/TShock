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
		private bool failed = false;
		public StatTracker()
		{
			ThreadPool.QueueUserWorkItem(SendUpdate);
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
				systemRam = 2097152,
				systemCPUClock = 2516582,
				version = TShock.VersionNum.ToString(),
				terrariaVersion = Terraria.Main.versionNumber2		
			};

			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(data);
			var encoded = HttpUtility.UrlEncode(serialized);
			var uri = String.Format("http://96.47.231.227:8000?data={0}", encoded);
			var client = (HttpWebRequest)WebRequest.Create(uri);
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
	}
}
