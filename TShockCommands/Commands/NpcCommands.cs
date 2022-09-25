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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

class NpcCommands : CommandCallbacks<TSPlayer>
{
	[Command("butcher")]
	[HelpText("Kills hostile NPCs or NPCs of a certain type.")]
	[CommandPermissions(Permissions.butcher)]
	public void Butcher(string? npcName = null)
	{
		var user = Sender;
		//if (args.Parameters.Count > 1)
		//{
		//	user.SendMessage("Butcher Syntax and Example", Color.White);
		//	user.SendMessage($"{"butcher".Color(Utils.BoldHighlight)} [{"NPC name".Color(Utils.RedHighlight)}|{"ID".Color(Utils.RedHighlight)}]", Color.White);
		//	user.SendMessage($"Example usage: {"butcher".Color(Utils.BoldHighlight)} {"pigron".Color(Utils.RedHighlight)}", Color.White);
		//	user.SendMessage("All alive NPCs (excluding town NPCs) on the server will be killed if you do not input a name or ID.", Color.White);
		//	user.SendMessage($"To get rid of NPCs without making them drop items, use the {"clear".Color(Utils.BoldHighlight)} command instead.", Color.White);
		//	//user.SendMessage($"To execute this command silently, use {SilentSpecifier.Color(Utils.GreenHighlight)} instead of {Specifier.Color(Utils.RedHighlight)}", Color.White);
		//	return;
		//}

		int npcId = 0;

		if (!String.IsNullOrWhiteSpace(npcName))
		{
			var npcs = TShock.Utils.GetNPCByIdOrName(npcName);
			if (npcs.Count == 0)
			{
				user.SendErrorMessage($"\"{npcName}\" is not a valid NPC.");
				return;
			}

			if (npcs.Count > 1)
			{
				user.SendMultipleMatchError(npcs.Select(n => $"{n.FullName}({n.type})"));
				return;
			}
			npcId = npcs[0].netID;
		}

		int kills = 0;
		for (int i = 0; i < Main.npc.Length; i++)
		{
			if (Main.npc[i].active && ((npcId == 0 && !Main.npc[i].townNPC && Main.npc[i].netID != NPCID.TargetDummy) || Main.npc[i].netID == npcId))
			{
				TSPlayer.Server.StrikeNPC(i, (int)(Main.npc[i].life + (Main.npc[i].defense * 0.6)), 0, 0);
				kills++;
			}
		}

		//if (args.Silent)
		//	user.SendSuccessMessage($"You butchered {kills} NPC{(kills > 1 ? "s" : "")}.");
		//else
		TSPlayer.All.SendInfoMessage($"{user.Name} butchered {kills} NPC{(kills > 1 ? "s" : "")}.");
	}

	[Command("renamenpc")]
	[HelpText("Renames an NPC.")]
	[CommandPermissions(Permissions.renamenpc)]
	public void RenameNPC(string? npcIdOrName = null, string? newname = null)
	{
		if (string.IsNullOrWhiteSpace(npcIdOrName) || string.IsNullOrWhiteSpace(newname))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}renameNPC <guide, nurse, etc.> <newname>", TextOptions.CommandPrefix);
			return;
		}
		int npcId = 0;
		//if (args.Parameters.Count == 2)
		{
			List<NPC> npcs = TShock.Utils.GetNPCByIdOrName(npcIdOrName);
			if (npcs.Count == 0)
			{
				Sender.SendErrorMessage("Invalid mob type!");
				return;
			}
			else if (npcs.Count > 1)
			{
				Sender.SendMultipleMatchError(npcs.Select(n => $"{n.FullName}({n.type})"));
				return;
			}
			else if (newname.Length > 200)
			{
				Sender.SendErrorMessage("New name is too large!");
				return;
			}
			else
			{
				npcId = npcs[0].netID;
			}
		}
		int done = 0;
		string? before = null;
		for (int i = 0; i < Main.npc.Length; i++)
		{
			if (Main.npc[i].active && ((npcId == 0 && !Main.npc[i].townNPC) || (Main.npc[i].netID == npcId && Main.npc[i].townNPC)))
			{
				before = Main.npc[i].GivenName;
				Main.npc[i].GivenName = newname;
				NetMessage.SendData(56, -1, -1, NetworkText.FromLiteral(newname), i, 0f, 0f, 0f, 0);
				done++;
			}
		}
		if (done > 0)
		{
			TSPlayer.All.SendInfoMessage($"{Sender.Name} renamed the {before ?? "<null>"} to {newname}.");
		}
		else
		{
			Sender.SendErrorMessage("Could not rename {0}!", npcIdOrName);
		}
	}

	[Command("maxspawns")]
	[HelpText("Sets the maximum number of NPCs.")]
	[CommandPermissions(Permissions.maxspawns)]
	public void MaxSpawns(string? value = null)
	{
		if (String.IsNullOrWhiteSpace(value))
		{
			Sender.SendInfoMessage("Current maximum spawns: {0}", TShock.Config.Settings.DefaultMaximumSpawns);
			return;
		}

		if (String.Equals(value, "default", StringComparison.CurrentCultureIgnoreCase))
		{
			TShock.Config.Settings.DefaultMaximumSpawns = NPC.defaultMaxSpawns = 5;
			//if (args.Silent)
			//{
			//	Sender.SendInfoMessage("Changed the maximum spawns to 5.");
			//}
			//else
			{
				TSPlayer.All.SendInfoMessage("{0} changed the maximum spawns to 5.", Sender.Name);
			}
			return;
		}

		int maxSpawns = -1;
		if (!int.TryParse(value, out maxSpawns) || maxSpawns < 0 || maxSpawns > Main.maxNPCs)
		{
			Sender.SendWarningMessage("Invalid maximum spawns!  Acceptable range is {0} to {1}", 0, Main.maxNPCs);
			return;
		}

		TShock.Config.Settings.DefaultMaximumSpawns = NPC.defaultMaxSpawns = maxSpawns;
		//if (args.Silent)
		//{
		//	Sender.SendInfoMessage("Changed the maximum spawns to {0}.", maxSpawns);
		//}
		//else
		{
			TSPlayer.All.SendInfoMessage("{0} changed the maximum spawns to {1}.", Sender.Name, maxSpawns);
		}
	}

	[Command("spawnboss", "sb")]
	[HelpText("Spawns a number of bosses around you.")]
	[CommandPermissions(Permissions.give)]
	[AllowServer(false)]
	public void SpawnBoss(string bossType, int amount = 1)
	{
		//if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
		//{
		//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}spawnboss <boss type> [amount]", Specifier);
		//	return;
		//}

		//int amount = 1;
		if (amount <= 0) //args.Parameters.Count == 2 && (!int.TryParse(args.Parameters[1], out amount) || amount <= 0))
		{
			Sender.SendErrorMessage("Invalid boss amount!");
			return;
		}

		string message = "{0} spawned {1} {2} time(s)";
		string spawnName;
		NPC npc = new NPC();
		switch (bossType.ToLower())
		{
			case "*":
			case "all":
				int[] npcIds = { 4, 13, 35, 50, 125, 126, 127, 134, 222, 245, 262, 266, 370, 398, 439, 636, 657 };
				TSPlayer.Server.SetTime(false, 0.0);
				foreach (int i in npcIds)
				{
					npc.SetDefaults(i);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				}
				spawnName = "all bosses";
				break;

			case "brain":
			case "brain of cthulhu":
			case "boc":
				npc.SetDefaults(266);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Brain of Cthulhu";
				break;

			case "destroyer":
				npc.SetDefaults(134);
				TSPlayer.Server.SetTime(false, 0.0);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Destroyer";
				break;
			case "duke":
			case "duke fishron":
			case "fishron":
				npc.SetDefaults(370);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "Duke Fishron";
				break;
			case "eater":
			case "eater of worlds":
			case "eow":
				npc.SetDefaults(13);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Eater of Worlds";
				break;
			case "eye":
			case "eye of cthulhu":
			case "eoc":
				npc.SetDefaults(4);
				TSPlayer.Server.SetTime(false, 0.0);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Eye of Cthulhu";
				break;
			case "golem":
				npc.SetDefaults(245);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Golem";
				break;
			case "king":
			case "king slime":
			case "ks":
				npc.SetDefaults(50);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the King Slime";
				break;
			case "plantera":
				npc.SetDefaults(262);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "Plantera";
				break;
			case "prime":
			case "skeletron prime":
				npc.SetDefaults(127);
				TSPlayer.Server.SetTime(false, 0.0);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "Skeletron Prime";
				break;
			case "queen bee":
			case "qb":
				npc.SetDefaults(222);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Queen Bee";
				break;
			case "skeletron":
				npc.SetDefaults(35);
				TSPlayer.Server.SetTime(false, 0.0);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "Skeletron";
				break;
			case "twins":
				TSPlayer.Server.SetTime(false, 0.0);
				npc.SetDefaults(125);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				npc.SetDefaults(126);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Twins";
				break;
			case "wof":
			case "wall of flesh":
				if (Main.wofNPCIndex != -1)
				{
					Sender.SendErrorMessage("There is already a Wall of Flesh!");
					return;
				}
				if (Sender.Y / 16f < Main.maxTilesY - 205)
				{
					Sender.SendErrorMessage("You must spawn the Wall of Flesh in hell!");
					return;
				}
				NPC.SpawnWOF(new Vector2(Sender.X, Sender.Y));
				spawnName = "the Wall of Flesh";
				break;
			case "moon":
			case "moon lord":
			case "ml":
				npc.SetDefaults(398);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Moon Lord";
				break;
			case "empress":
			case "empress of light":
			case "eol":
				npc.SetDefaults(636);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Empress of Light";
				break;
			case "queen slime":
			case "qs":
				npc.SetDefaults(657);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Queen Slime";
				break;
			case "lunatic":
			case "lunatic cultist":
			case "cultist":
			case "lc":
				npc.SetDefaults(439);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Lunatic Cultist";
				break;
			case "betsy":
				npc.SetDefaults(551);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "Betsy";
				break;
			case "flying dutchman":
			case "flying":
			case "dutchman":
				npc.SetDefaults(491);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Flying Dutchman";
				break;
			case "mourning wood":
				npc.SetDefaults(325);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "Mourning Wood";
				break;
			case "pumpking":
				npc.SetDefaults(327);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Pumpking";
				break;
			case "everscream":
				npc.SetDefaults(344);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "Everscream";
				break;
			case "santa-nk1":
			case "santa":
				npc.SetDefaults(346);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "Santa-NK1";
				break;
			case "ice queen":
				npc.SetDefaults(345);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "the Ice Queen";
				break;
			case "martian saucer":
				npc.SetDefaults(392);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "a Martian Saucer";
				break;
			case "solar pillar":
				npc.SetDefaults(517);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "a Solar Pillar";
				break;
			case "nebula pillar":
				npc.SetDefaults(507);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "a Nebula Pillar";
				break;
			case "vortex pillar":
				npc.SetDefaults(422);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "a Vortex Pillar";
				break;
			case "stardust pillar":
				npc.SetDefaults(493);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "a Stardust Pillar";
				break;
			case "deerclops":
				npc.SetDefaults(668);
				TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, Sender.TileX, Sender.TileY);
				spawnName = "a Deerclops";
				break;
			default:
				Sender.SendErrorMessage("Invalid boss type!");
				return;
		}

		//if (args.Silent)
		//{
		//	//"You spawned <spawn name> <x> time(s)"
		//	Sender.SendSuccessMessage(message, "You", spawnName, amount);
		//}
		//else
		{
			//"<player> spawned <spawn name> <x> time(s)"
			TSPlayer.All.SendSuccessMessage(message, Sender.Name, spawnName, amount);
		}
	}

	[Command("spawnmob", "sm")]
	[HelpText("Spawns a number of mobs around you.")]
	[CommandPermissions(Permissions.spawnmob)]
	[AllowServer(false)]
	public void SpawnMob(string modType, int amount = 1)
	{
		//if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
		//{
		//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}spawnmob <mob type> [amount]", Specifier);
		//	return;
		//}
		if (modType.Trim().Length == 0)
		{
			Sender.SendErrorMessage("Invalid mob type!");
			return;
		}

		//int amount = 1;
		if (amount <= 0) //args.Parameters.Count == 2 && !int.TryParse(args.Parameters[1], out amount))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}spawnmob <mob type> [amount]", TextOptions.CommandPrefix);
			return;
		}

		amount = Math.Min(amount, Main.maxNPCs);

		var npcs = TShock.Utils.GetNPCByIdOrName(modType);
		if (npcs.Count == 0)
		{
			Sender.SendErrorMessage("Invalid mob type!");
		}
		else if (npcs.Count > 1)
		{
			Sender.SendMultipleMatchError(npcs.Select(n => $"{n.FullName}({n.type})"));
		}
		else
		{
			var npc = npcs[0];
			if (npc.type >= 1 && npc.type < Main.maxNPCTypes && npc.type != 113)
			{
				TSPlayer.Server.SpawnNPC(npc.netID, npc.FullName, amount, Sender.TileX, Sender.TileY, 50, 20);
				//if (args.Silent)
				//{
				//	Sender.SendSuccessMessage("Spawned {0} {1} time(s).", npc.FullName, amount);
				//}
				//else
				{
					TSPlayer.All.SendSuccessMessage("{0} has spawned {1} {2} time(s).", Sender.Name, npc.FullName, amount);
				}
			}
			else if (npc.type == 113)
			{
				if (Main.wofNPCIndex != -1 || (Sender.Y / 16f < (Main.maxTilesY - 205)))
				{
					Sender.SendErrorMessage("Can't spawn Wall of Flesh!");
					return;
				}
				NPC.SpawnWOF(new Vector2(Sender.X, Sender.Y));
				//if (args.Silent)
				//{
				//	Sender.SendSuccessMessage("Spawned Wall of Flesh!");
				//}
				//else
				{
					TSPlayer.All.SendSuccessMessage("{0} has spawned a Wall of Flesh!", Sender.Name);
				}
			}
			else
			{
				Sender.SendErrorMessage("Invalid mob type!");
			}
		}
	}

	[Command("spawnrate")]
	[HelpText("Sets the spawn rate of NPCs.")]
	[CommandPermissions(Permissions.spawnrate)]
	public void SpawnRate(string? newrate = null)
	{
		if (String.IsNullOrWhiteSpace(newrate))
		{
			Sender.SendInfoMessage("Current spawn rate: {0}", TShock.Config.Settings.DefaultSpawnRate);
			return;
		}

		if (String.Equals(newrate, "default", StringComparison.CurrentCultureIgnoreCase))
		{
			TShock.Config.Settings.DefaultSpawnRate = NPC.defaultSpawnRate = 600;
			//if (args.Silent)
			//{
			//	Sender.SendInfoMessage("Changed the spawn rate to 600.");
			//}
			//else
			{
				TSPlayer.All.SendInfoMessage("{0} changed the spawn rate to 600.", Sender.Name);
			}
			return;
		}

		int spawnRate = -1;
		if (!int.TryParse(newrate, out spawnRate) || spawnRate < 0)
		{
			Sender.SendWarningMessage("Invalid spawn rate!");
			return;
		}
		TShock.Config.Settings.DefaultSpawnRate = NPC.defaultSpawnRate = spawnRate;
		//if (args.Silent)
		//{
		//	Sender.SendInfoMessage("Changed the spawn rate to {0}.", spawnRate);
		//}
		//else
		{
			TSPlayer.All.SendInfoMessage("{0} changed the spawn rate to {1}.", Sender.Name, spawnRate);
		}
	}

	[Command("clearangler")]
	[HelpText("Resets the list of users who have completed an angler quest that day.")]
	[CommandPermissions(Permissions.clearangler)]
	public void ClearAnglerQuests(string? playerName = null)
	{
		if (!String.IsNullOrWhiteSpace(playerName))
		{
			var result = Main.anglerWhoFinishedToday.RemoveAll(s => s.ToLower().Equals(playerName.ToLower()));
			if (result > 0)
			{
				Sender.SendSuccessMessage("Removed {0} players from the angler quest completion list for today.", result);
				foreach (TSPlayer ply in TShock.Players.Where(p => p != null && p.Active && p.TPlayer.name.ToLower().Equals(playerName.ToLower())))
				{
					//this will always tell the client that they have not done the quest today.
					ply.SendData((PacketTypes)74, "");
				}
			}
			else
				Sender.SendErrorMessage("Failed to find any users by that name on the list.");

		}
		else
		{
			Main.anglerWhoFinishedToday.Clear();
			NetMessage.SendAnglerQuest(-1);
			Sender.SendSuccessMessage("Cleared all users from the angler quest completion list for today.");
		}
	}
}
