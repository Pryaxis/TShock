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
using System.Linq;
using EasyCommands;
using EasyCommands.Commands;
using Microsoft.Xna.Framework;
using TShockAPI;
using TShockCommands.Annotations;
using Terraria;
using Utils = TShockAPI.Utils;
using Terraria.GameContent.Creative;
using System;
using System.Collections.Generic;
using TShockAPI.Modules;

namespace TShockCommands.Commands;

class OtherCommands : CommandCallbacks<TSPlayer>
{
	[Command("buff")]
	[CommandPermissions(Permissions.buff)]
	[HelpText("Gives yourself a buff or debuff for an amount of time. Putting -1 for time will set it to 415 days.")]
	[AllowServer(false)]
	public void Buff(string? buffNameOrId = null, int time = 60)
	{
		// buff <"buff name|ID"> [duration]
		var user = Sender;
		if (string.IsNullOrWhiteSpace(buffNameOrId))
		{
			user.SendMessage("Buff Syntax and Example", Color.White);
			user.SendMessage($"{"buff".Color(Utils.BoldHighlight)} <\"{"buff name".Color(Utils.RedHighlight)}|{"ID".Color(Utils.RedHighlight)}\"> [{"duration".Color(Utils.GreenHighlight)}]", Color.White);
			user.SendMessage($"Example usage: {"buff".Color(Utils.BoldHighlight)} \"{"obsidian skin".Color(Utils.RedHighlight)}\" {"-1".Color(Utils.GreenHighlight)}", Color.White);
			user.SendMessage($"If you don't specify the duration, it will default to {"60".Color(Utils.GreenHighlight)} seconds.", Color.White);
			user.SendMessage($"If you put {"-1".Color(Utils.GreenHighlight)} as the duration, it will use the max possible time of 415 days.", Color.White);
			return;
		}

		int id = 0;
		//int time = 60;
		var timeLimit = (int.MaxValue / 60) - 1;

		if (!int.TryParse(buffNameOrId, out id))
		{
			var found = TShock.Utils.GetBuffByName(buffNameOrId);

			if (found.Count == 0)
			{
				user.SendErrorMessage($"Unable to find any buffs named \"{buffNameOrId}\"");
				return;
			}

			if (found.Count > 1)
			{
				user.SendMultipleMatchError(found.Select(f => Lang.GetBuffName(f)));
				return;
			}
			id = found[0];
		}

		//if (args.Parameters.Count == 2)
		//	int.TryParse(args.Parameters[1], out time);

		if (id > 0 && id < Main.maxBuffTypes)
		{
			// Max possible buff duration as of Terraria 1.4.2.3 is 35791393 seconds (415 days).
			if (time < 0 || time > timeLimit)
				time = timeLimit;
			user.SetBuff(id, time * 60);
			user.SendSuccessMessage($"You buffed yourself with {TShock.Utils.GetBuffName(id)} ({TShock.Utils.GetBuffDescription(id)}) for {time} seconds.");
		}
		else
			user.SendErrorMessage($"\"{id}\" is not a valid buff ID!");
	}

	[Command("clear")]
	[CommandPermissions(Permissions.clear)]
	[HelpText("Clears item drops or projectiles.")]
	public void Clear(string? type = null, int radius = 50)
	{
		var user = Sender;
		var everyone = TSPlayer.All;
		//int radius = 50;

		if (string.IsNullOrWhiteSpace(type))
		{
			user.SendMessage("Clear Syntax", Color.White);
			user.SendMessage($"{"clear".Color(Utils.BoldHighlight)} <{"item".Color(Utils.GreenHighlight)}|{"npc".Color(Utils.RedHighlight)}|{"projectile".Color(Utils.YellowHighlight)}> [{"radius".Color(Utils.PinkHighlight)}]", Color.White);
			user.SendMessage($"Example usage: {"clear".Color(Utils.BoldHighlight)} {"i".Color(Utils.RedHighlight)} {"10000".Color(Utils.GreenHighlight)}", Color.White); user.SendMessage($"Example usage: {"clear".Color(Utils.BoldHighlight)} {"item".Color(Utils.RedHighlight)} {"10000".Color(Utils.GreenHighlight)}", Color.White);
			user.SendMessage($"If you do not specify a radius, it will use a default radius of {radius} around your character.", Color.White);
			//user.SendMessage($"You can use {SilentSpecifier.Color(Utils.GreenHighlight)} instead of {Specifier.Color(Utils.RedHighlight)} to execute this command silently.", Color.White);
			return;
		}

		if (radius <= 0)
		{
			user.SendErrorMessage($"\"{radius}\" is not a valid radius.");
			return;
		}

		switch (type.ToLower())
		{
			case "item":
			case "items":
			case "i":
				{
					int cleared = 0;
					for (int i = 0; i < Main.maxItems; i++)
					{
						float dX = Main.item[i].position.X - user.X;
						float dY = Main.item[i].position.Y - user.Y;

						if (Main.item[i].active && dX * dX + dY * dY <= radius * radius * 256f)
						{
							Main.item[i].active = false;
							everyone.SendData(PacketTypes.ItemDrop, "", i);
							cleared++;
						}
					}
					//if (args.Silent)
					//	user.SendSuccessMessage($"You deleted {cleared} item{(cleared > 1 ? "s" : "")} within a radius of {radius}.");
					//else
					everyone.SendInfoMessage($"{user.Name} deleted {cleared} item{(cleared > 1 ? "s" : "")} within a radius of {radius}.");
				}
				break;
			case "npc":
			case "npcs":
			case "n":
				{
					int cleared = 0;
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						float dX = Main.npc[i].position.X - user.X;
						float dY = Main.npc[i].position.Y - user.Y;

						if (Main.npc[i].active && dX * dX + dY * dY <= radius * radius * 256f)
						{
							Main.npc[i].active = false;
							Main.npc[i].type = 0;
							everyone.SendData(PacketTypes.NpcUpdate, "", i);
							cleared++;
						}
					}
					//if (args.Silent)
					//	user.SendSuccessMessage($"You deleted {cleared} NPC{(cleared > 1 ? "s" : "")} within a radius of {radius}.");
					//else
					everyone.SendInfoMessage($"{user.Name} deleted {cleared} NPC{(cleared > 1 ? "s" : "")} within a radius of {radius}.");
				}
				break;
			case "proj":
			case "projectile":
			case "projectiles":
			case "p":
				{
					int cleared = 0;
					for (int i = 0; i < Main.maxProjectiles; i++)
					{
						float dX = Main.projectile[i].position.X - user.X;
						float dY = Main.projectile[i].position.Y - user.Y;

						if (Main.projectile[i].active && dX * dX + dY * dY <= radius * radius * 256f)
						{
							Main.projectile[i].active = false;
							Main.projectile[i].type = 0;
							everyone.SendData(PacketTypes.ProjectileNew, "", i);
							cleared++;
						}
					}
					//if (args.Silent)
					//	user.SendSuccessMessage($"You deleted {cleared} projectile{(cleared > 1 ? "s" : "")} within a radius of {radius}.");
					//else
					everyone.SendInfoMessage($"{user.Name} deleted {cleared} projectile{(cleared > 1 ? "s" : "")} within a radius of {radius}");
				}
				break;
			default:
				user.SendErrorMessage($"\"{type}\" is not a valid clear option.");
				break;
		}
	}

	[Command("gbuff", "buffplayer")]
	[CommandPermissions(Permissions.buffplayer)]
	[HelpText("Gives another player a buff or debuff for an amount of time. Putting -1 for time will set it to 415 days.")]
	public void GBuff(string? player = null, string? buffname = null, int time = 60)
	{
		var user = Sender;
		if (string.IsNullOrWhiteSpace(player))
		{
			user.SendMessage("Give Buff Syntax and Example", Color.White);
			user.SendMessage($"{"gbuff".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}> <{"buff name".Color(Utils.PinkHighlight)}|{"ID".Color(Utils.PinkHighlight)}> [{"seconds".Color(Utils.GreenHighlight)}]", Color.White);
			user.SendMessage($"Example usage: {"gbuff".Color(Utils.BoldHighlight)} {user.Name.Color(Utils.RedHighlight)} {"regen".Color(Utils.PinkHighlight)} {"-1".Color(Utils.GreenHighlight)}", Color.White);
			//user.SendMessage($"To buff a player without them knowing, use {SilentSpecifier.Color(Utils.RedHighlight)} instead of {Specifier.Color(Utils.GreenHighlight)}", Color.White);
			return;
		}
		int id = 0;
		//int time = 60;
		var timeLimit = (int.MaxValue / 60) - 1;
		var foundplr = TSPlayer.FindByNameOrID(player);
		if (foundplr.Count == 0)
		{
			user.SendErrorMessage($"Unable to find any player named \"{player}\"");
			return;
		}
		else if (foundplr.Count > 1)
		{
			user.SendMultipleMatchError(foundplr.Select(p => p.Name));
			return;
		}
		else
		{
			if (!int.TryParse(buffname, out id))
			{
				var found = TShock.Utils.GetBuffByName(buffname);
				if (found.Count == 0)
				{
					user.SendErrorMessage($"Unable to find any buff named \"{buffname}\"");
					return;
				}
				else if (found.Count > 1)
				{
					user.SendMultipleMatchError(found.Select(b => Lang.GetBuffName(b)));
					return;
				}
				id = found[0];
			}
			//if (args.Parameters.Count == 3)
			//	int.TryParse(args.Parameters[2], out time);
			if (id > 0 && id < Main.maxBuffTypes)
			{
				var target = foundplr[0];
				if (time < 0 || time > timeLimit)
					time = timeLimit;
				target.SetBuff(id, time * 60);
				user.SendSuccessMessage($"You have buffed {(target == user ? "yourself" : target.Name)} with {TShock.Utils.GetBuffName(id)} ({TShock.Utils.GetBuffDescription(id)}) for {time} seconds!");
				if (/*!args.Silent &&*/ target != user)
					target.SendSuccessMessage($"{user.Name} has buffed you with {TShock.Utils.GetBuffName(id)} ({TShock.Utils.GetBuffDescription(id)}) for {time} seconds!");
			}
			else
				user.SendErrorMessage("Invalid buff ID!");
		}
	}

	[Command("godmode", "god")]
	[CommandPermissions(Permissions.godmode)]
	[HelpText("Toggles godmode on a player.")]
	public void ToggleGodMode([AllowSpaces] string? target = null)
	{
		TSPlayer playerToGod;
		if (!string.IsNullOrWhiteSpace(target))
		{
			if (!Sender.HasPermission(Permissions.godmodeother))
			{
				Sender.SendErrorMessage("You do not have permission to god mode another player.");
				return;
			}
			//string plStr = String.Join(" ", args.Parameters);
			var players = TSPlayer.FindByNameOrID(target);
			if (players.Count == 0)
			{
				Sender.SendErrorMessage("Invalid player!");
				return;
			}
			else if (players.Count > 1)
			{
				Sender.SendMultipleMatchError(players.Select(p => p.Name));
				return;
			}
			else
			{
				playerToGod = players[0];
			}
		}
		else if (!Sender.RealPlayer)
		{
			Sender.SendErrorMessage("You can't god mode a non player!");
			return;
		}
		else
		{
			playerToGod = Sender;
		}

		playerToGod.GodMode = !playerToGod.GodMode;

		var godPower = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();

		godPower.SetEnabledState(playerToGod.Index, playerToGod.GodMode);

		if (playerToGod != Sender)
		{
			Sender.SendSuccessMessage(string.Format("{0} is {1} in god mode.", playerToGod.Name, playerToGod.GodMode ? "now" : "no longer"));
		}

		//if (!args.Silent || (playerToGod == Sender))
		{
			playerToGod.SendSuccessMessage(string.Format("You are {0} in god mode.", playerToGod.GodMode ? "now" : "no longer"));
		}
	}

	[Command("heal")]
	[CommandPermissions(Permissions.heal)]
	[HelpText("Heals a player in HP and MP.")]
	public void Heal(TSPlayer player, int amount = 0)
	{
		// heal <player> [amount]
		// To-Do: break up heal self and heal other into two separate permissions
		var user = Sender;
		if (player is null)
		{
			user.SendMessage("Heal Syntax and Example", Color.White);
			user.SendMessage($"{"heal".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}> [{"amount".Color(Utils.GreenHighlight)}]", Color.White);
			user.SendMessage($"Example usage: {"heal".Color(Utils.BoldHighlight)} {user.Name.Color(Utils.RedHighlight)} {"100".Color(Utils.GreenHighlight)}", Color.White);
			user.SendMessage($"If no amount is specified, it will default to healing the target player by their max HP.", Color.White);
			//user.SendMessage($"To execute this command silently, use {SilentSpecifier.Color(Utils.GreenHighlight)} instead of {Specifier.Color(Utils.RedHighlight)}", Color.White);
			return;
		}
		//if (args.Parameters[0].Length == 0)
		//{
		//	user.SendErrorMessage($"You didn't put a player name.");
		//	return;
		//}

		//string targetName = args.Parameters[0];
		//var players = TSPlayer.FindByNameOrID(targetName);
		//if (players.Count == 0)
		//	user.SendErrorMessage($"Unable to find any players named \"{targetName}\"");
		//else if (players.Count > 1)
		//	user.SendMultipleMatchError(players.Select(p => p.Name));
		//else
		{
			//var target = players[0];
			var target = player;
			//int amount = target.TPlayer.statLifeMax2;
			if (amount <= 0)
				amount = target.TPlayer.statLifeMax2;

			if (target.Dead)
			{
				user.SendErrorMessage("You can't heal a dead player!");
				return;
			}

			//if (args.Parameters.Count == 2)
			//{
			//	int.TryParse(args.Parameters[1], out amount);
			//}
			target.Heal(amount);

			//if (args.Silent)
			//	user.SendSuccessMessage($"You healed {(target == user ? "yourself" : target.Name)} for {amount} HP.");
			//else
			TSPlayer.All.SendInfoMessage($"{user.Name} healed {(target == user ? (target.TPlayer.Male ? "himself" : "herself") : target.Name)} for {amount} HP.");
		}
	}

	[Command("healkill", "slay")]
	[CommandPermissions(Permissions.kill)]
	[HelpText("Kills another player.")]
	public void Kill(TSPlayer target)
	{
		// To-Do: separate kill self and kill other player into two permissions
		var user = Sender;
		if (target is null)
		{
			user.SendMessage("Kill syntax and example", Color.White);
			user.SendMessage($"{"kill".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}>", Color.White);
			user.SendMessage($"Example usage: {"kill".Color(Utils.BoldHighlight)} {user.Name.Color(Utils.RedHighlight)}", Color.White);
			//user.SendMessage($"You can use {SilentSpecifier.Color(Utils.GreenHighlight)} instead of {Specifier.Color(Utils.RedHighlight)} to execute this command silently.", Color.White);
			return;
		}

		//string targetName = String.Join(" ", args.Parameters);
		//var players = TSPlayer.FindByNameOrID(targetName);

		//if (players.Count == 0)
		//	user.SendErrorMessage($"Could not find any player named \"{targetName}\".");
		//else if (players.Count > 1)
		//	user.SendMultipleMatchError(players.Select(p => p.Name));
		//else
		{
			//var target = players[0];

			if (target.Dead)
			{
				user.SendErrorMessage($"{(target == user ? "You" : target.Name)} {(target == user ? "are" : "is")} already dead!");
				return;
			}
			target.KillPlayer();
			user.SendSuccessMessage($"You just killed {(target == user ? "yourself" : target.Name)}!");
			if (/*!args.Silent &&*/ target != user)
				target.SendErrorMessage($"{user.Name} just killed you!");
		}
	}

	[Command("me")]
	[CommandPermissions(Permissions.cantalkinthird)]
	[HelpText("Sends an action message to everyone.")]
	public void ThirdPerson([AllowSpaces] string message)
	{
		if (string.IsNullOrWhiteSpace(message))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}me <text>", TextOptions.CommandPrefix);
			return;
		}
		if (Sender.mute)
			Sender.SendErrorMessage("You are muted.");
		else
			TSPlayer.All.SendMessage(string.Format("*{0} {1}", Sender.Name, message), 205, 133, 63);
	}

	[Command("party", "p")]
	[CommandPermissions(Permissions.canpartychat)]
	[HelpText("Sends a message to everyone on your team.")]
	[AllowServer(false)]
	public void PartyChat([AllowSpaces] string message)
	{
		if (string.IsNullOrWhiteSpace(message))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}p <team chat text>", TextOptions.CommandPrefix);
			return;
		}
		int playerTeam = Sender.Team;

		if (Sender.mute)
			Sender.SendErrorMessage("You are muted.");
		else if (playerTeam != 0)
		{
			string msg = string.Format("<{0}> {1}", Sender.Name, message);
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.Active && player.Team == playerTeam)
					player.SendMessage(msg, Main.teamColor[playerTeam].R, Main.teamColor[playerTeam].G, Main.teamColor[playerTeam].B);
			}
		}
		else
			Sender.SendErrorMessage("You are not in a party!");
	}

	[Command("reply", "r")]
	[CommandPermissions(Permissions.whisper)]
	[HelpText("Replies to a PM sent to you.")]
	public void Reply([AllowSpaces] string message)
	{
		if (Sender.mute)
		{
			Sender.SendErrorMessage("You are muted.");
		}
		else if (Sender.LastWhisper != null && Sender.LastWhisper.Active)
		{
			if (!Sender.LastWhisper.AcceptingWhispers)
			{
				Sender.SendErrorMessage($"{Sender.LastWhisper.Name} is not accepting whispers.");
				return;
			}
			//var msg = string.Join(" ", args.Parameters);
			Sender.LastWhisper.SendMessage($"<From {Sender.Name}> {message}", Color.MediumPurple);
			Sender.SendMessage($"<To {Sender.LastWhisper.Name}> {message}", Color.MediumPurple);
		}
		else if (Sender.LastWhisper != null)
		{
			Sender.SendErrorMessage($"{Sender.LastWhisper.Name} is offline and cannot receive your reply.");
		}
		else
		{
			Sender.SendErrorMessage("You haven't previously received any whispers.");
			Sender.SendMessage($"You can use {TextOptions.CommandPrefix.Color(Utils.GreenHighlight)}{"w".Color(Utils.GreenHighlight)} to whisper to other players.", Color.White);
		}
	}

	[Command("rest")]
	[CommandPermissions(Rests.RestPermissions.restmanage)]
	[HelpText("Manages the REST API.")]
	public void ManageRest(string? command = null, [PageNumber] int pageNumber = 1)
	{
		string subCommand = "help";
		if (!String.IsNullOrWhiteSpace(command))
			subCommand = command;
		//if (args.Parameters.Count > 0)
		//	subCommand = args.Parameters[0];

		switch (subCommand.ToLower())
		{
			case "listusers":
				{
					//int pageNumber;
					//if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, Sender, out pageNumber))
					//	return;

					Dictionary<string, int> restUsersTokens = new Dictionary<string, int>();
					foreach (Rests.SecureRest.TokenData tokenData in TShock.RestApi.Tokens.Values)
					{
						if (restUsersTokens.ContainsKey(tokenData.Username))
							restUsersTokens[tokenData.Username]++;
						else
							restUsersTokens.Add(tokenData.Username, 1);
					}

					List<string> restUsers = new List<string>(
						restUsersTokens.Select(ut => string.Format("{0} ({1} tokens)", ut.Key, ut.Value)));

					PaginationTools.SendPage(
						Sender, pageNumber, PaginationTools.BuildLinesFromTerms(restUsers), new PaginationTools.Settings
						{
							NothingToDisplayString = "There are currently no active REST users.",
							HeaderFormat = "Active REST Users ({0}/{1}):",
							FooterFormat = "Type {0}rest listusers {{0}} for more.".SFormat(TextOptions.CommandPrefix)
						}
					);

					break;
				}
			case "destroytokens":
				{
					TShock.RestApi.Tokens.Clear();
					Sender.SendSuccessMessage("All REST tokens have been destroyed.");
					break;
				}
			default:
				{
					Sender.SendInfoMessage("Available REST Sub-Commands:");
					Sender.SendMessage("listusers - Lists all REST users and their current active tokens.", Color.White);
					Sender.SendMessage("destroytokens - Destroys all current REST tokens.", Color.White);
					break;
				}
		}
	}

	[Command("slap")]
	[CommandPermissions(Permissions.slap)]
	[HelpText("Slaps a player, dealing damage.")]
	public void Slap(TSPlayer player, int damage = 5)
	{
		if (player is null)
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}slap <player> [damage]", TextOptions.CommandPrefix);
			return;
		}
		//if (args.Parameters[0].Length == 0)
		//{
		//	Sender.SendErrorMessage("Invalid player!");
		//	return;
		//}

		//string plStr = args.Parameters[0];
		//var players = TSPlayer.FindByNameOrID(plStr);
		//if (players.Count == 0)
		//{
		//	Sender.SendErrorMessage("Invalid player!");
		//}
		//else if (players.Count > 1)
		//{
		//	Sender.SendMultipleMatchError(players.Select(p => p.Name));
		//}
		//else
		{
			var plr = player; // players[0];
							  //int damage = 5;
							  //if (args.Parameters.Count == 2)
							  //{
							  //	int.TryParse(args.Parameters[1], out damage);
							  //}
			if (!Sender.HasPermission(Permissions.kill))
			{
				damage = TShock.Utils.Clamp(damage, 15, 0);
			}
			plr.DamagePlayer(damage);
			TSPlayer.All.SendInfoMessage("{0} slapped {1} for {2} damage.", Sender.Name, plr.Name, damage);
			TShock.Log.Info("{0} slapped {1} for {2} damage.", Sender.Name, plr.Name, damage);
		}
	}

	[Command("whisper", "w", "tell", "pm", "dm")]
	[CommandPermissions(Permissions.whisper)]
	[HelpText("Sends a PM to a player.")]
	public void Whisper(TSPlayer player, [AllowSpaces] string message)
	{
		if (player is null || String.IsNullOrWhiteSpace(message))
		{
			Sender.SendMessage("Whisper Syntax", Color.White);
			Sender.SendMessage($"{"whisper".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}> <{"message".Color(Utils.PinkHighlight)}>", Color.White);
			Sender.SendMessage($"Example usage: {"w".Color(Utils.BoldHighlight)} {Sender.Name.Color(Utils.RedHighlight)} {"We're no strangers to love, you know the rules, and so do I.".Color(Utils.PinkHighlight)}", Color.White);
			return;
		}
		//var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
		//if (players.Count == 0)
		//{
		//	Sender.SendErrorMessage($"Could not find any player named \"{args.Parameters[0]}\"");
		//}
		//else if (players.Count > 1)
		//{
		//	Sender.SendMultipleMatchError(players.Select(p => p.Name));
		//}
		//else if (Sender.mute)
		//{
		//	Sender.SendErrorMessage("You are muted.");
		//}
		//else
		{
			var plr = player;// players[0];
			if (plr == Sender)
			{
				Sender.SendErrorMessage("You cannot whisper to yourself.");
				return;
			}
			if (!plr.AcceptingWhispers)
			{
				Sender.SendErrorMessage($"{plr.Name} is not accepting whispers.");
				return;
			}
			//var msg = string.Join(" ", args.Parameters.ToArray(), 1, args.Parameters.Count - 1);
			plr.SendMessage($"<From {Sender.Name}> {message}", Color.MediumPurple);
			Sender.SendMessage($"<To {plr.Name}> {message}", Color.MediumPurple);
			plr.LastWhisper = Sender;
			Sender.LastWhisper = plr;
		}
	}

	[Command("wallow", "wa")]
	[CommandPermissions(Permissions.slap)]
	[HelpText("Toggles to either ignore or recieve whispers from other players.")]
	[AllowServer(false)]
	public void Wallow()
	{
		Sender.AcceptingWhispers = !Sender.AcceptingWhispers;
		Sender.SendSuccessMessage($"You {(Sender.AcceptingWhispers ? "may now" : "will no longer")} receive whispers from other players.");
		Sender.SendMessage($"You can use {TextOptions.CommandPrefix.Color(Utils.GreenHighlight)}{"wa".Color(Utils.GreenHighlight)} to toggle this setting.", Color.White);
	}

	//[Command("dump-reference-data")]
	//[CommandPermissions(Permissions.createdumps)]
	//[HelpText("Creates a reference tables for Terraria data types and the TShock permission system in the server folder.")]
	//public void CreateDumps(ICommandService commandService)
	//{
	//	TShock.Utils.DumpPermissionMatrix("PermissionMatrix.txt");
	//	TShock.Utils.Dump(commandService, false);
	//	Sender.SendSuccessMessage("Your reference dumps have been created in the server folder.");
	//	return;
	//}

	[Command("sync")]
	[CommandPermissions(Permissions.synclocalarea)]
	[HelpText("Sends all tiles from the server to the player to resync the client with the actual world state.")]
	public void SyncLocalArea()
	{
		Sender.SendTileSquareCentered(Sender.TileX, Sender.TileY, 32);
		Sender.SendWarningMessage("Sync'd!");
		return;
	}

	[Command("respawn")]
	[CommandPermissions(Permissions.respawn)]
	[HelpText("Respawn yourself or another player.")]
	public void Respawn([AllowSpaces] string? target = null)
	{
		if (!Sender.RealPlayer && String.IsNullOrWhiteSpace(target))
		{
			Sender.SendErrorMessage("You can't respawn the server console!");
			return;
		}
		TSPlayer playerToRespawn;
		if (!String.IsNullOrWhiteSpace(target))
		{
			if (!Sender.HasPermission(Permissions.respawnother))
			{
				Sender.SendErrorMessage("You do not have permission to respawn another player.");
				return;
			}
			//string plStr = String.Join(" ", args.Parameters);
			var players = TSPlayer.FindByNameOrID(target);
			if (players.Count == 0)
			{
				Sender.SendErrorMessage($"Could not find any player named \"{target}\"");
				return;
			}
			if (players.Count > 1)
			{
				Sender.SendMultipleMatchError(players.Select(p => p.Name));
				return;
			}
			playerToRespawn = players[0];
		}
		else
			playerToRespawn = Sender;

		if (!playerToRespawn.Dead)
		{
			Sender.SendErrorMessage($"{(playerToRespawn == Sender ? "You" : playerToRespawn.Name)} {(playerToRespawn == Sender ? "are" : "is")} not dead.");
			return;
		}
		playerToRespawn.Spawn(PlayerSpawnContext.ReviveFromDeath);

		if (playerToRespawn != Sender)
		{
			Sender.SendSuccessMessage($"You have respawned {playerToRespawn.Name}");
			//if (!args.Silent)
			playerToRespawn.SendSuccessMessage($"{Sender.Name} has respawned you.");
		}
		else
			playerToRespawn.SendSuccessMessage("You have respawned yourself.");
	}
}
