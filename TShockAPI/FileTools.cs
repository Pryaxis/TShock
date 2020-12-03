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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TShockAPI
{
	public class FileTools
	{
		private const string MotdFormat =
			"This is [c/FF0000:%map%] on [c/00FFFF:TShock for Terraria].\n[c/00FF00:Current players:] [c/FFFF00:%players%]\nType [c/FF0000:/help] for a list of commands.\n";
		/// <summary>
		/// Path to the file containing the rules.
		/// </summary>
		internal static string RulesPath
		{
			get { return Path.Combine(TShock.SavePath, "rules.txt"); }
		}

		/// <summary>
		/// Path to the file containing the message of the day.
		/// </summary>
		internal static string MotdPath
		{
			get { return Path.Combine(TShock.SavePath, "motd.txt"); }
		}

		/// <summary>
		/// Path to the file containing the whitelist.
		/// </summary>
		internal static string WhitelistPath
		{
			get { return Path.Combine(TShock.SavePath, "whitelist.txt"); }
		}

		/// <summary>
		/// Path to the file containing the config.
		/// </summary>
		internal static string ConfigPath
		{
			get { return Path.Combine(TShock.SavePath, "config.json"); }
		}

		/// <summary>
		/// Path to the file containing the config.
		/// </summary>
		internal static string ServerSideCharacterConfigPath
		{
			get { return Path.Combine(TShock.SavePath, "sscconfig.json"); }
		}

		/// <summary>
		/// Creates an empty file at the given path.
		/// </summary>
		/// <param name="file">The path to the file.</param>
		public static void CreateFile(string file)
		{
			File.Create(file).Close();
		}

		/// <summary>
		/// Creates a file if the files doesn't already exist.
		/// </summary>
		/// <param name="file">The path to the files</param>
		/// <param name="data">The data to write to the file.</param>
		public static void CreateIfNot(string file, string data = "")
		{
			if (!File.Exists(file))
			{
				File.WriteAllText(file, data);
			}
		}

		/// <summary>
		/// Sets up the configuration file for all variables, and creates any missing files.
		/// </summary>
		public static void SetupConfig()
		{
			if (!Directory.Exists(TShock.SavePath))
			{
				Directory.CreateDirectory(TShock.SavePath);
			}

			CreateIfNot(RulesPath, "Respect the admins!\nDon't use TNT!");
			CreateIfNot(MotdPath, MotdFormat);
						
			CreateIfNot(WhitelistPath);
			bool writeConfig = true; // Default to true if the file doesn't exist
			if (File.Exists(ConfigPath))
			{
				TShock.Config.Read(ConfigPath, out writeConfig);
			}
			if (writeConfig)
			{
				// Add all the missing config properties in the json file
				TShock.Config.Write(ConfigPath);
			}

			bool writeSSCConfig = true; // Default to true if the file doesn't exist
			if (File.Exists(ServerSideCharacterConfigPath))
			{
				TShock.ServerSideCharacterConfig.Read(ServerSideCharacterConfigPath, out writeSSCConfig);
			}
			if (writeSSCConfig)
			{
				// Add all the missing config properties in the json file
				TShock.ServerSideCharacterConfig = new Configuration.ServerSideConfig
				{
					Settings = { StartingInventory =
						new List<NetItem>
						{
							new NetItem(-15, 1, 0),
							new NetItem(-13, 1, 0),
							new NetItem(-16, 1, 0)
						}
					}
				};
				TShock.ServerSideCharacterConfig.Write(ServerSideCharacterConfigPath);
			}
		}

		/// <summary>
		/// Tells if a user is on the whitelist
		/// </summary>
		/// <param name="ip">string ip of the user</param>
		/// <returns>true/false</returns>
		public static bool OnWhitelist(string ip)
		{
			if (!TShock.Config.Settings.EnableWhitelist)
			{
				return true;
			}
			CreateIfNot(WhitelistPath, "127.0.0.1");
			using (var tr = new StreamReader(WhitelistPath))
			{
				string whitelist = tr.ReadToEnd();
				ip = TShock.Utils.GetRealIP(ip);
				bool contains = whitelist.Contains(ip);
				if (!contains)
				{
					foreach (var line in whitelist.Split(Environment.NewLine.ToCharArray()))
					{
						if (string.IsNullOrWhiteSpace(line))
							continue;
						contains = TShock.Utils.GetIPv4AddressFromHostname(line).Equals(ip);
						if (contains)
							return true;
					}
					return false;
				}
				return true;
			}
		}
		
		/// <summary>
		/// Looks for a 'Settings' token in the json object. If one is not found, returns a new json object with all tokens of the previous object added
		/// as children to a root 'Settings' token
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		internal static JObject AttemptConfigUpgrade(string json)
		{
			JObject cfg = JObject.Parse(json);

			if (cfg.SelectToken("Settings") == null)
			{
				JObject newCfg = new JObject
				{
					{ "Settings", cfg }
				};
				cfg = newCfg;
			}

			return cfg;
		}

		internal static TSettings LoadConfigAndCheckForMissingFields<TSettings>(string json, out bool anyMissingFields)
		{
			return LoadConfigAndCheckForMissingFields<TSettings>(JObject.Parse(json), out anyMissingFields);
		}

		/// <summary>
		/// Parses a JObject into a TSettings object, also emitting a bool indicating if any of the TSetting's fields were missing from the JObject
		/// </summary>
		/// <typeparam name="TSettings">The type of the config file object</typeparam>
		/// <param name="jObject">The json object to parse</param>
		/// <param name="anyMissingFields">Whether any fields are missing from the config</param>
		/// <returns>The config object</returns>
		internal static TSettings LoadConfigAndCheckForMissingFields<TSettings>(JObject jObject, out bool anyMissingFields)
		{
			anyMissingFields = false;
			var configFields = new HashSet<string>(typeof(Configuration.ConfigFile<TSettings>).GetFields()
				.Where(field => !field.IsStatic)
				.Select(field => field.Name));
			var jsonFields = new HashSet<string>(jObject
				.Children()
				.Select(field => field as JProperty)
				.Where(field => field != null)
				.Select(field => field.Name));
			anyMissingFields = !configFields.SetEquals(jsonFields);

			return jObject.SelectToken("Settings").ToObject<TSettings>();
		}
	}
}
