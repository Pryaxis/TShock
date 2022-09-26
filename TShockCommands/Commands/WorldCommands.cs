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
using EasyCommands;
using EasyCommands.Commands;
using TShockAPI;
using Terraria;
using TShockCommands.Annotations;
using System;
using Terraria.ID;
using System.Collections.Generic;
using System.Linq;

namespace TShockCommands.Commands;

class WorldCommands : CommandCallbacks<TSPlayer>
{
	static Dictionary<string, int> _worldModes = new Dictionary<string, int>
	{
		{ "normal",    0 },
		{ "expert",    1 },
		{ "master",    2 },
		{ "journey",   3 },
		{ "creative",  3 }
	};

	[Command("worldmode", "gamemode")]
	[CommandPermissions(Permissions.toggleexpert)]
	[HelpText("Changes the world mode.")]
	public void ChangeWorldMode(string? newmode = null)
	{
		if (String.IsNullOrWhiteSpace(newmode))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}worldmode <mode>", TextOptions.CommandPrefix);
			Sender.SendErrorMessage("Valid modes: {0}", String.Join(", ", _worldModes.Keys));
			return;
		}

		int mode;

		if (int.TryParse(newmode, out mode))
		{
			if (mode < 0 || mode > 3)
			{
				Sender.SendErrorMessage("Invalid mode! Valid modes: {0}", String.Join(", ", _worldModes.Keys));
				return;
			}
		}
		else if (_worldModes.ContainsKey(newmode.ToLowerInvariant()))
		{
			mode = _worldModes[newmode.ToLowerInvariant()];
		}
		else
		{
			Sender.SendErrorMessage("Invalid mode! Valid modes: {0}", String.Join(", ", _worldModes.Keys));
			return;
		}

		Main.GameMode = mode;
		Sender.SendSuccessMessage("World mode set to {0}", _worldModes.Keys.ElementAt(mode));
		TSPlayer.All.SendData(PacketTypes.WorldInfo);
	}

	[Command("antibuild")]
	[CommandPermissions(Permissions.antibuild)]
	[HelpText("Toggles build protection.")]
	public void ToggleAntiBuild()
	{
		TShock.Config.Settings.DisableBuild = !TShock.Config.Settings.DisableBuild;
		TSPlayer.All.SendSuccessMessage(string.Format("Anti-build is now {0}.", (TShock.Config.Settings.DisableBuild ? "on" : "off")));
	}

	[Command]
	[CommandPermissions(Permissions.grow)]
	[HelpText("Grows plants at your location.")]
	[AllowServer(false)]
	public void Grow(string? type = null, [PageNumber] int pageNumber = 1)
	{
		bool canGrowEvil = Sender.HasPermission(Permissions.growevil);
		//string subcmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();
		var subcmd = !String.IsNullOrWhiteSpace(type) ? type : "help";

		var name = "Fail";
		var x = Sender.TileX;
		var y = Sender.TileY + 3;

		if (!TShock.Regions.CanBuild(x, y, Sender))
		{
			Sender.SendErrorMessage("You're not allowed to change tiles here!");
			return;
		}

		switch (subcmd)
		{
			case "help":
				{
					//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out int pageNumber))
					//	return;

					var lines = new List<string>
						{
							"- Default trees :",
							"     'basic', 'sakura', 'willow', 'boreal', 'mahogany', 'ebonwood', 'shadewood', 'pearlwood'.",
							"- Palm trees :",
							"     'palm', 'corruptpalm', 'crimsonpalm', 'hallowpalm'.",
							"- Gem trees :",
							"     'topaz', 'amethyst', 'sapphire', 'emerald', 'ruby', 'diamond', 'amber'.",
							"- Misc :",
							"     'cactus', 'herb', 'mushroom'."
						};

					PaginationTools.SendPage(Sender, pageNumber, lines,
						new PaginationTools.Settings
						{
							HeaderFormat = "Trees types & misc available to use. ({0}/{1}):",
							FooterFormat = "Type {0}grow help {{0}} for more sub-commands.".SFormat(TextOptions.CommandPrefix)
						}
					);
				}
				return;

				bool rejectCannotGrowEvil()
				{
					if (!canGrowEvil)
					{
						Sender.SendErrorMessage("You do not have permission to grow this tree type");
						return false;
					}

					return true;
				}

				bool prepareAreaForGrow(ushort groundType = TileID.Grass, bool evil = false)
				{
					if (evil && !rejectCannotGrowEvil())
						return false;

					for (var i = x - 2; i < x + 3; i++)
					{
						Main.tile[i, y].active(true);
						Main.tile[i, y].type = groundType;
						Main.tile[i, y].wall = WallID.None;
					}
					Main.tile[x, y - 1].wall = WallID.None;

					return true;
				}

				bool growTree(ushort groundType, string fancyName, bool evil = false)
				{
					if (!prepareAreaForGrow(groundType, evil))
						return false;
					WorldGen.GrowTree(x, y);
					name = fancyName;

					return true;
				}

				bool growTreeByType(ushort groundType, string fancyName, ushort typeToPrepare = 2, bool evil = false)
				{
					if (!prepareAreaForGrow(typeToPrepare, evil))
						return false;
					WorldGen.TryGrowingTreeByType(groundType, x, y);
					name = fancyName;

					return true;
				}

				bool growPalmTree(ushort sandType, ushort supportingType, string properName, bool evil = false)
				{
					if (evil && !rejectCannotGrowEvil())
						return false;

					for (int i = x - 2; i < x + 3; i++)
					{
						Main.tile[i, y].active(true);
						Main.tile[i, y].type = sandType;
						Main.tile[i, y].wall = WallID.None;
					}
					for (int i = x - 2; i < x + 3; i++)
					{
						Main.tile[i, y + 1].active(true);
						Main.tile[i, y + 1].type = supportingType;
						Main.tile[i, y + 1].wall = WallID.None;
					}

					Main.tile[x, y - 1].wall = WallID.None;
					WorldGen.GrowPalmTree(x, y);

					name = properName;

					return true;
				}

			case "basic":
				growTree(TileID.Grass, "Basic Tree");
				break;

			case "boreal":
				growTree(TileID.SnowBlock, "Boreal Tree");
				break;

			case "mahogany":
				growTree(TileID.JungleGrass, "Rich Mahogany");
				break;

			case "sakura":
				growTreeByType(TileID.VanityTreeSakura, "Sakura Tree");
				break;

			case "willow":
				growTreeByType(TileID.VanityTreeYellowWillow, "Willow Tree");
				break;

			case "shadewood":
				if (!growTree(TileID.CrimsonGrass, "Shadewood Tree", true))
					return;
				break;

			case "ebonwood":
				if (!growTree(TileID.CorruptGrass, "Ebonwood Tree", true))
					return;
				break;

			case "pearlwood":
				if (!growTree(TileID.HallowedGrass, "Pearlwood Tree", true))
					return;
				break;

			case "palm":
				growPalmTree(TileID.Sand, TileID.HardenedSand, "Desert Palm");
				break;

			case "hallowpalm":
				if (!growPalmTree(TileID.Pearlsand, TileID.HallowHardenedSand, "Hallow Palm", true))
					return;
				break;

			case "crimsonpalm":
				if (!growPalmTree(TileID.Crimsand, TileID.CrimsonHardenedSand, "Crimson Palm", true))
					return;
				break;

			case "corruptpalm":
				if (!growPalmTree(TileID.Ebonsand, TileID.CorruptHardenedSand, "Corruption Palm", true))
					return;
				break;

			case "topaz":
				growTreeByType(TileID.TreeTopaz, "Topaz Gemtree", 1);
				break;

			case "amethyst":
				growTreeByType(TileID.TreeAmethyst, "Amethyst Gemtree", 1);
				break;

			case "sapphire":
				growTreeByType(TileID.TreeSapphire, "Sapphire Gemtree", 1);
				break;

			case "emerald":
				growTreeByType(TileID.TreeEmerald, "Emerald Gemtree", 1);
				break;

			case "ruby":
				growTreeByType(TileID.TreeRuby, "Ruby Gemtree", 1);
				break;

			case "diamond":
				growTreeByType(TileID.TreeDiamond, "Diamond Gemtree", 1);
				break;

			case "amber":
				growTreeByType(TileID.TreeAmber, "Amber Gemtree", 1);
				break;

			case "cactus":
				Main.tile[x, y].type = TileID.Sand;
				WorldGen.GrowCactus(x, y);
				name = "Cactus";
				break;

			case "herb":
				Main.tile[x, y].active(true);
				Main.tile[x, y].frameX = 36;
				Main.tile[x, y].type = TileID.MatureHerbs;
				WorldGen.GrowAlch(x, y);
				name = "Herb";
				break;

			case "mushroom":
				prepareAreaForGrow(TileID.MushroomGrass);
				WorldGen.GrowShroom(x, y);
				name = "Glowing Mushroom Tree";
				break;

			default:
				Sender.SendErrorMessage("Unknown plant!");
				return;
		}
		if (!String.IsNullOrWhiteSpace(type))
		{
			Sender.SendTileSquareCentered(x - 2, y - 20, 25);
			Sender.SendSuccessMessage("Tried to grow a " + name + ".");
		}
	}

	[Command]
	[CommandPermissions(Permissions.halloween)]
	[HelpText("Toggles halloween mode (goodie bags, pumpkins, etc).")]
	public void ForceHalloween()
	{
		TShock.Config.Settings.ForceHalloween = !TShock.Config.Settings.ForceHalloween;
		Main.checkHalloween();
		//if (args.Silent)
		//	Sender.SendInfoMessage("{0}abled halloween mode!", (TShock.Config.Settings.ForceHalloween ? "en" : "dis"));
		//else
		TSPlayer.All.SendInfoMessage("{0} {1}abled halloween mode!", Sender.Name, (TShock.Config.Settings.ForceHalloween ? "en" : "dis"));
	}

	//			add(new Command(Permissions., ForceXmas, "forcexmas")
	//			{
	//				HelpText = "."
	//			});
	[Command]
	[CommandPermissions(Permissions.xmas)]
	[HelpText("Toggles christmas mode (present spawning, santa, etc).")]
	public void ForceXmas()
	{
		TShock.Config.Settings.ForceXmas = !TShock.Config.Settings.ForceXmas;
		Main.checkXMas();
		//if (args.Silent)
		//	Sender.SendInfoMessage("{0}abled Christmas mode!", (TShock.Config.Settings.ForceXmas ? "en" : "dis"));
		//else
		TSPlayer.All.SendInfoMessage("{0} {1}abled Christmas mode!", Sender.Name, (TShock.Config.Settings.ForceXmas ? "en" : "dis"));
	}

	[Command]
	[CommandPermissions(Permissions.hardmode)]
	[HelpText("Toggles the world's hardmode status.")]
	public void Hardmode()
	{
		if (Main.hardMode)
		{
			Main.hardMode = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
			Sender.SendSuccessMessage("Hardmode is now off.");
		}
		else if (!TShock.Config.Settings.DisableHardmode)
		{
			WorldGen.StartHardmode();
			Sender.SendSuccessMessage("Hardmode is now on.");
		}
		else
		{
			Sender.SendErrorMessage("Hardmode is disabled via config.");
		}
	}

	[Command]
	[CommandPermissions(Permissions.editspawn)]
	[HelpText("Toggles spawn protection.")]
	public void ProtectSpawn()
	{
		TShock.Config.Settings.SpawnProtection = !TShock.Config.Settings.SpawnProtection;
		TSPlayer.All.SendSuccessMessage(string.Format("Spawn is now {0}.", (TShock.Config.Settings.SpawnProtection ? "protected" : "open")));
	}

	[Command]
	[CommandPermissions(Permissions.worldsave)]
	[HelpText("Saves the world file.")]
	public void Save()
	{
		SaveManager.Instance.SaveWorld(false);
		foreach (TSPlayer tsply in TShock.Players.Where(tsply => tsply != null))
		{
			tsply.SaveServerCharacter();
		}
	}

	[Command]
	[CommandPermissions(Permissions.worldspawn)]
	[HelpText("Sets the world's spawn point to your location.")]
	[AllowServer(false)]
	public void SetSpawn()
	{
		Main.spawnTileX = Sender.TileX + 1;
		Main.spawnTileY = Sender.TileY + 3;
		SaveManager.Instance.SaveWorld(false);
		Sender.SendSuccessMessage("Spawn has now been set at your location.");
	}

	[Command]
	[CommandPermissions(Permissions.dungeonposition)]
	[HelpText("Sets the dungeon's position to your location.")]
	[AllowServer(false)]
	public void SetDungeon()
	{
		Main.dungeonX = Sender.TileX + 1;
		Main.dungeonY = Sender.TileY + 3;
		SaveManager.Instance.SaveWorld(false);
		Sender.SendSuccessMessage("The dungeon's position has now been set at your location.");
	}

	[Command]
	[CommandPermissions(Permissions.worldsettle)]
	[HelpText("Forces all liquids to update immediately.")]
	public void Settle()
	{
		if (Liquid.panicMode)
		{
			Sender.SendWarningMessage("Liquids are already settling!");
			return;
		}
		Liquid.StartPanic();
		Sender.SendInfoMessage("Settling liquids.");
	}

	[Command]
	[CommandPermissions(Permissions.time)]
	[HelpText("Sets the world time.")]
	public void Time(string? newtime = null)
	{
		if (String.IsNullOrWhiteSpace(newtime))
		{
			double time = Main.time / 3600.0;
			time += 4.5;
			if (!Main.dayTime)
				time += 15.0;
			time = time % 24.0;
			Sender.SendInfoMessage("The current time is {0}:{1:D2}.", (int)Math.Floor(time), (int)Math.Floor((time % 1.0) * 60.0));
			return;
		}

		switch (newtime)
		{
			case "day":
				TSPlayer.Server.SetTime(true, 0.0);
				TSPlayer.All.SendInfoMessage("{0} set the time to 4:30.", Sender.Name);
				break;
			case "night":
				TSPlayer.Server.SetTime(false, 0.0);
				TSPlayer.All.SendInfoMessage("{0} set the time to 19:30.", Sender.Name);
				break;
			case "noon":
				TSPlayer.Server.SetTime(true, 27000.0);
				TSPlayer.All.SendInfoMessage("{0} set the time to 12:00.", Sender.Name);
				break;
			case "midnight":
				TSPlayer.Server.SetTime(false, 16200.0);
				TSPlayer.All.SendInfoMessage("{0} set the time to 0:00.", Sender.Name);
				break;
			default:
				string[] array = newtime.Split(':');
				if (array.Length != 2)
				{
					Sender.SendErrorMessage("Invalid time string! Proper format: hh:mm, in 24-hour time.");
					return;
				}

				int hours;
				int minutes;
				if (!int.TryParse(array[0], out hours) || hours < 0 || hours > 23
					|| !int.TryParse(array[1], out minutes) || minutes < 0 || minutes > 59)
				{
					Sender.SendErrorMessage("Invalid time string! Proper format: hh:mm, in 24-hour time.");
					return;
				}

				decimal time = hours + (minutes / 60.0m);
				time -= 4.50m;
				if (time < 0.00m)
					time += 24.00m;

				if (time >= 15.00m)
				{
					TSPlayer.Server.SetTime(false, (double)((time - 15.00m) * 3600.0m));
				}
				else
				{
					TSPlayer.Server.SetTime(true, (double)(time * 3600.0m));
				}
				TSPlayer.All.SendInfoMessage("{0} set the time to {1}:{2:D2}.", Sender.Name, hours, minutes);
				break;
		}
	}

	[Command]
	[CommandPermissions(Permissions.wind)]
	[HelpText("Changes the wind speed.")]
	public void Wind(int speed)
	{
		//if (args.Parameters.Count != 1)
		//{
		//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}wind <speed>", TextOptions.CommandPrefix);
		//	return;
		//}

		//int speed;
		if (/*!int.TryParse(args.Parameters[0], out speed) ||*/ speed * 100 < 0)
		{
			Sender.SendErrorMessage("Invalid wind speed!");
			return;
		}

		Main.windSpeedCurrent = speed;
		Main.windSpeedTarget = speed;
		TSPlayer.All.SendData(PacketTypes.WorldInfo);
		TSPlayer.All.SendInfoMessage("{0} changed the wind speed to {1}.", Sender.Name, speed);
	}
}
