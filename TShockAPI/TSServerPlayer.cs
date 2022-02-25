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

using Microsoft.Xna.Framework;
using OTAPI.Tile;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Utilities;
using TShockAPI;
using TShockAPI.DB;
using Terraria.Localization;
using System.Linq;
using Terraria.DataStructures;

namespace TShockAPI
{
	public class TSServerPlayer : TSPlayer
	{
		public static string AccountName = "ServerConsole";

		public TSServerPlayer()
			: base("Server")
		{
			Group = new SuperAdminGroup();
			Account = new UserAccount { Name = AccountName };
		}

		public override void SendErrorMessage(string msg)
		{
			SendConsoleMessage(msg, 255, 0, 0);
		}

		public override void SendInfoMessage(string msg)
		{
			SendConsoleMessage(msg, 255, 250, 170);
		}

		public override void SendSuccessMessage(string msg)
		{
			SendConsoleMessage(msg, 0, 255, 0);
		}

		public override void SendWarningMessage(string msg)
		{
			SendConsoleMessage(msg, 139, 0, 0);
		}

		public override void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		public override void SendMessage(string msg, byte red, byte green, byte blue)
		{
			SendConsoleMessage(msg, red, green, blue);
		}

		public void SendConsoleMessage(string msg, byte red, byte green, byte blue)
		{
			var snippets = Terraria.UI.Chat.ChatManager.ParseMessage(msg, new Color(red, green, blue));

			foreach (var snippet in snippets)
			{
				if (snippet.Color != null)
				{
					Console.ForegroundColor = PickNearbyConsoleColor(snippet.Color);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Gray;
				}

				Console.Write(snippet.Text);
			}
			Console.WriteLine();
			Console.ResetColor();
		}

		public void SetFullMoon()
		{
			Main.dayTime = false;
			Main.moonPhase = 0;
			Main.time = 0.0;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetBloodMoon(bool bloodMoon)
		{
			if (bloodMoon)
			{
				Main.dayTime = false;
				Main.bloodMoon = true;
				Main.time = 0.0;
			}
			else
				Main.bloodMoon = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetFrostMoon(bool snowMoon)
		{
			if (snowMoon)
			{
				Main.dayTime = false;
				Main.snowMoon = true;
				Main.time = 0.0;
			}
			else
				Main.snowMoon = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetPumpkinMoon(bool pumpkinMoon)
		{
			if (pumpkinMoon)
			{
				Main.dayTime = false;
				Main.pumpkinMoon = true;
				Main.time = 0.0;
			}
			else
				Main.pumpkinMoon = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetEclipse(bool eclipse)
		{
			if (eclipse)
			{
				Main.dayTime = Main.eclipse = true;
				Main.time = 0.0;
			}
			else
				Main.eclipse = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetTime(bool dayTime, double time)
		{
			Main.dayTime = dayTime;
			Main.time = time;
			TSPlayer.All.SendData(PacketTypes.TimeSet, "", dayTime ? 1 : 0, (int)time, Main.sunModY, Main.moonModY);
		}

		public void SpawnNPC(int type, string name, int amount, int startTileX, int startTileY, int tileXRange = 100,
			int tileYRange = 50)
		{
			for (int i = 0; i < amount; i++)
			{
				int spawnTileX;
				int spawnTileY;
				TShock.Utils.GetRandomClearTileWithInRange(startTileX, startTileY, tileXRange, tileYRange, out spawnTileX,
															 out spawnTileY);
				NPC.NewNPC(new EntitySource_DebugCommand(), spawnTileX * 16, spawnTileY * 16, type);
			}
		}

		public void StrikeNPC(int npcid, int damage, float knockBack, int hitDirection)
		{
			// Main.rand is thread static.
			if (Main.rand == null)
				Main.rand = new UnifiedRandom();

			Main.npc[npcid].StrikeNPC(damage, knockBack, hitDirection);
			NetMessage.SendData((int)PacketTypes.NpcStrike, -1, -1, NetworkText.Empty, npcid, damage, knockBack, hitDirection);
		}

		public void RevertTiles(Dictionary<Vector2, ITile> tiles)
		{
			// Update Main.Tile first so that when tile square is sent it is correct
			foreach (KeyValuePair<Vector2, ITile> entry in tiles)
			{
				Main.tile[(int)entry.Key.X, (int)entry.Key.Y] = entry.Value;
			}
			// Send all players updated tile squares
			foreach (Vector2 coords in tiles.Keys)
			{
				All.SendTileSquareCentered((int)coords.X, (int)coords.Y, 3);
			}
		}


		private readonly Dictionary<Color, ConsoleColor> _consoleColorMap = new Dictionary<Color, ConsoleColor>
		{
			{ Color.Red,					ConsoleColor.Red },
			{ Color.Green,					ConsoleColor.Green },
			{ Color.Blue,					ConsoleColor.Cyan },
			{ new Color(255, 250, 170),		ConsoleColor.Yellow },
			{ new Color(170, 170, 255),     ConsoleColor.Cyan },
			{ new Color(255, 170, 255),		ConsoleColor.Magenta },
			{ new Color(170, 255, 170),		ConsoleColor.Green },
			{ new Color(255, 170, 170),     ConsoleColor.Red },
			{ new Color(139, 0, 0),			ConsoleColor.DarkRed }, // This is the console warning color
			{ Color.PaleVioletRed,          ConsoleColor.Magenta }, // This is the command logging color
			{ Color.White,					ConsoleColor.White }
		};

		private ConsoleColor PickNearbyConsoleColor(Color color)
		{
			//Grabs an integer difference between two colors in euclidean space
			int ColorDiff(Color c1, Color c2)
			{
				return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
									   + (c1.G - c2.G) * (c1.G - c2.G)
									   + (c1.B - c2.B) * (c1.B - c2.B));
			}

			var diffs = _consoleColorMap.Select(kvp => ColorDiff(kvp.Key, color));
			int index = 0;
			int min = int.MaxValue;

			for (int i = 0; i < _consoleColorMap.Count; i++)
			{
				if (diffs.ElementAt(i) < min)
				{
					index = i;
					min = diffs.ElementAt(i);
				}
			}

			return _consoleColorMap.Values.ElementAt(index);
		}
	}
}
