using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using System.IO;
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
