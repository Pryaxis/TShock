using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using Newtonsoft.Json;

namespace TShockAPI.ServerSideCharacters
{
	public class ServerSideConfig
	{
		[Description("Enable server side characters, This stops the client from saving character data! EXPERIMENTAL!!!!!")]
		public bool Enabled;

		[Description("How often SSC should save, in minutes.")]
		public int ServerSideCharacterSave = 5;

		[Description("Time, in milliseconds, to disallow discarding items after logging in when ServerSideInventory is ON.")]
		public int LogonDiscardThreshold = 250;

		[Description("The starting default health for new SSC.")] 
		public int StartingHealth = 100;

		[Description("The starting default mana for new SSC.")] 
		public int StartingMana = 20;

		[Description("The starting default inventory for new SSC.")] 
		public List<NetItem> StartingInventory = new List<NetItem>();

		public static ServerSideConfig Read(string path)
		{
			using (var reader = new StreamReader(path))
			{
				string txt = reader.ReadToEnd();
				var config = JsonConvert.DeserializeObject<ServerSideConfig>(txt);
				return config;
			}
		}

		public void Write(string path)
		{
			using (var writer = new StreamWriter(path))
			{
				writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
			}
		}
	}
}
