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
			"Welcome to [c/ffff00:%map%] on [c/7ddff8:T][c/81dbf6:S][c/86d7f4:h][c/8ad3f3:o][c/8ecef1:c][c/93caef:k] for [c/55d284:T][c/62d27a:e][c/6fd16f:r][c/7cd165:r][c/89d15a:a][c/95d150:r][c/a4d145:i][c/b1d03b:a].\n[c/FFFFFF:Online players (%onlineplayers%/%serverslots%):] [c/FFFF00:%players%]\nType [c/55D284:%specifier%][c/62D27A:h][c/6FD16F:e][c/7CD165:l][c/89D15A:p] for a list of commands.\n";
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
		/// <param name="cfg"></param>
		/// <param name="requiredUpgrade"></param>
		/// <returns></returns>
		internal static JObject AttemptConfigUpgrade(JObject cfg, out bool requiredUpgrade)
		{
			requiredUpgrade = false;

			if (cfg.SelectToken("Settings") == null)
			{
				JObject newCfg = new JObject
				{
					{ "Settings", cfg }
				};
				cfg = newCfg;
				requiredUpgrade = true;
			}

			return cfg;
		}

		internal static TSettings LoadConfigAndCheckForChanges<TSettings>(string json, out bool writeConfig) where TSettings : new()
		{
			//If an empty file is attempting to be loaded as a config, instead use an empty json object. Otherwise Newtonsoft throws an exception here
			if (string.IsNullOrWhiteSpace(json))
			{
				json = "{}";
			}

			return LoadConfigAndCheckForChanges<TSettings>(JObject.Parse(json), out writeConfig);
		}

		/// <summary>
		/// Parses a JObject into a TSettings object, also emitting a bool indicating if the JObject was incomplete
		/// </summary>
		/// <typeparam name="TSettings">The type of the config file object</typeparam>
		/// <param name="jObject">The json object to parse</param>
		/// <param name="writeConfig">Whether the config needs to be written to disk again</param>
		/// <returns>The config object</returns>
		internal static TSettings LoadConfigAndCheckForChanges<TSettings>(JObject jObject, out bool writeConfig) where TSettings : new()
		{
			JObject cfg = AttemptConfigUpgrade(jObject, out bool requiredUpgrade);

			var configFields = new HashSet<string>(typeof(TSettings).GetFields()
				.Where(field => !field.IsStatic)
				.Select(field => field.Name));

			var jsonFields = new HashSet<string>(cfg.SelectToken("Settings")
				.Children()
				.Select(field => field as JProperty)
				.Where(field => field != null)
				.Select(field => field.Name));

			bool missingFields = !configFields.SetEquals(jsonFields);


			//If the config file had to be upgraded or the fields in the given TSettings don't match the config, we'll want the config to be rewritten with the correct data
			writeConfig = requiredUpgrade || missingFields;

			return cfg.SelectToken("Settings").ToObject<TSettings>();
		}
	}
}
