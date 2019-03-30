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

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TShockAPI.ServerSideCharacters
{
	public class ServerSideConfig
	{
		[Description("Enable server side characters, This stops the client from saving character data! EXPERIMENTAL!!!!!")]
		public bool Enabled = false;

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

		/// <summary>
		/// Dumps all configuration options to a text file in Markdown format
		/// </summary>
		public static void DumpDescriptions()
		{
			var sb = new StringBuilder();
			var defaults = new ServerSideConfig();

			foreach (var field in defaults.GetType().GetFields().OrderBy(f => f.Name))
			{
				if (field.IsStatic)
					continue;

				var name = field.Name;
				var type = field.FieldType.Name;

				var descattr =
					field.GetCustomAttributes(false).FirstOrDefault(o => o is DescriptionAttribute) as DescriptionAttribute;
				var desc = descattr != null && !string.IsNullOrWhiteSpace(descattr.Description) ? descattr.Description : "None";

				var def = field.GetValue(defaults);

				sb.AppendLine("{0}  ".SFormat(name));
				sb.AppendLine("Type: {0}  ".SFormat(type));
				sb.AppendLine("Description: {0}  ".SFormat(desc));
				sb.AppendLine("Default: \"{0}\"  ".SFormat(def));
				sb.AppendLine();
			}

			File.WriteAllText("ServerSideConfigDescriptions.txt", sb.ToString());
		}
	}
}
