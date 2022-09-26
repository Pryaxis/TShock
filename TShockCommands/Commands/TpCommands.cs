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
using System;
using System.Collections.Generic;
using System.Linq;
using EasyCommands;
using EasyCommands.Commands;
using Terraria;
using TShockAPI;
using TShockAPI.Localization;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

class TpCommands : CommandCallbacks<TSPlayer>
{
	[Command("home")]
	[CommandPermissions(Permissions.home)]
	[HelpText("Sends you to your spawn point.")]
	[AllowServer(false)]
	public void Home()
	{
		if (Sender.Dead)
		{
			Sender.SendErrorMessage("You are dead.");
			return;
		}
		Sender.Spawn(PlayerSpawnContext.RecallFromItem);
		Sender.SendSuccessMessage("Teleported to your spawnpoint.");
	}

	[Command("spawn")]
	[CommandPermissions(Permissions.spawn)]
	[HelpText("Sends you to the world's spawn point.")]
	[AllowServer(false)]
	public void Spawn()
	{
		if (Sender.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48))
			Sender.SendSuccessMessage("Teleported to the map's spawnpoint.");
	}

	[Command("tp")]
	[CommandPermissions(Permissions.tp)]
	[HelpText("Teleports a player to another player.")]
	[AllowServer(false)]
	public void TP(string player, string? player2 = null)
	{
		if (String.IsNullOrWhiteSpace(player))
		{
			if (Sender.HasPermission(Permissions.tpothers))
				Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tp <player> [player 2]", TextOptions.CommandPrefix);
			else
				Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tp <player>", TextOptions.CommandPrefix);
			return;
		}

		if (String.IsNullOrWhiteSpace(player2))
		{
			var players = TSPlayer.FindByNameOrID(player);
			if (players.Count == 0)
				Sender.SendErrorMessage("Invalid player!");
			else if (players.Count > 1)
				Sender.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				var target = players[0];
				if (!target.TPAllow && !Sender.HasPermission(Permissions.tpoverride))
				{
					Sender.SendErrorMessage("{0} has disabled players from teleporting.", target.Name);
					return;
				}
				if (Sender.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
				{
					Sender.SendSuccessMessage("Teleported to {0}.", target.Name);
					if (!Sender.HasPermission(Permissions.tpsilent))
						target.SendInfoMessage("{0} teleported to you.", Sender.Name);
				}
			}
		}
		else
		{
			if (!Sender.HasPermission(Permissions.tpothers))
			{
				Sender.SendErrorMessage("You do not have access to this command.");
				return;
			}

			var players1 = TSPlayer.FindByNameOrID(player);
			var players2 = TSPlayer.FindByNameOrID(player2);

			if (players2.Count == 0)
				Sender.SendErrorMessage("Invalid player!");
			else if (players2.Count > 1)
				Sender.SendMultipleMatchError(players2.Select(p => p.Name));
			else if (players1.Count == 0)
			{
				if (player == "*")
				{
					if (!Sender.HasPermission(Permissions.tpallothers))
					{
						Sender.SendErrorMessage("You do not have access to this command.");
						return;
					}

					var target = players2[0];
					foreach (var source in TShock.Players.Where(p => p != null && p != Sender))
					{
						if (!target.TPAllow && !Sender.HasPermission(Permissions.tpoverride))
							continue;
						if (source.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
						{
							if (Sender != source)
							{
								if (Sender.HasPermission(Permissions.tpsilent))
									source.SendSuccessMessage("You were teleported to {0}.", target.Name);
								else
									source.SendSuccessMessage("{0} teleported you to {1}.", Sender.Name, target.Name);
							}
							if (Sender != target)
							{
								if (Sender.HasPermission(Permissions.tpsilent))
									target.SendInfoMessage("{0} was teleported to you.", source.Name);
								if (!Sender.HasPermission(Permissions.tpsilent))
									target.SendInfoMessage("{0} teleported {1} to you.", Sender.Name, source.Name);
							}
						}
					}
					Sender.SendSuccessMessage("Teleported everyone to {0}.", target.Name);
				}
				else
					Sender.SendErrorMessage("Invalid player!");
			}
			else if (players1.Count > 1)
				Sender.SendMultipleMatchError(players1.Select(p => p.Name));
			else
			{
				var source = players1[0];
				if (!source.TPAllow && !Sender.HasPermission(Permissions.tpoverride))
				{
					Sender.SendErrorMessage("{0} has disabled players from teleporting.", source.Name);
					return;
				}
				var target = players2[0];
				if (!target.TPAllow && !Sender.HasPermission(Permissions.tpoverride))
				{
					Sender.SendErrorMessage("{0} has disabled players from teleporting.", target.Name);
					return;
				}
				Sender.SendSuccessMessage("Teleported {0} to {1}.", source.Name, target.Name);
				if (source.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
				{
					if (Sender != source)
					{
						if (Sender.HasPermission(Permissions.tpsilent))
							source.SendSuccessMessage("You were teleported to {0}.", target.Name);
						else
							source.SendSuccessMessage("{0} teleported you to {1}.", Sender.Name, target.Name);
					}
					if (Sender != target)
					{
						if (Sender.HasPermission(Permissions.tpsilent))
							target.SendInfoMessage("{0} was teleported to you.", source.Name);
						if (!Sender.HasPermission(Permissions.tpsilent))
							target.SendInfoMessage("{0} teleported {1} to you.", Sender.Name, source.Name);
					}
				}
			}
		}
	}

	[Command("tphere")]
	[CommandPermissions(Permissions.tpothers)]
	[HelpText("Teleports a player to yourself.")]
	[AllowServer(false)]
	public void TPHere([AllowSpaces] string? playerName = null)
	{
		if (String.IsNullOrWhiteSpace(playerName))
		{
			if (Sender.HasPermission(Permissions.tpallothers))
				Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tphere <player|*>", TextOptions.CommandPrefix);
			else
				Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tphere <player>", TextOptions.CommandPrefix);
			return;
		}

		//string playerName = String.Join(" ", args.Parameters);
		var players = TSPlayer.FindByNameOrID(playerName);
		if (players.Count == 0)
		{
			if (playerName == "*")
			{
				if (!Sender.HasPermission(Permissions.tpallothers))
				{
					Sender.SendErrorMessage("You do not have permission to use this command.");
					return;
				}
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					if (Main.player[i].active && (Main.player[i] != Sender.TPlayer))
					{
						if (TShock.Players[i].Teleport(Sender.TPlayer.position.X, Sender.TPlayer.position.Y))
							TShock.Players[i].SendSuccessMessage(String.Format("You were teleported to {0}.", Sender.Name));
					}
				}
				Sender.SendSuccessMessage("Teleported everyone to yourself.");
			}
			else
				Sender.SendErrorMessage("Invalid player!");
		}
		else if (players.Count > 1)
			Sender.SendMultipleMatchError(players.Select(p => p.Name));
		else
		{
			var plr = players[0];
			if (plr.Teleport(Sender.TPlayer.position.X, Sender.TPlayer.position.Y))
			{
				plr.SendInfoMessage("You were teleported to {0}.", Sender.Name);
				Sender.SendSuccessMessage("Teleported {0} to yourself.", plr.Name);
			}
		}
	}

	[Command("tpnpc")]
	[CommandPermissions(Permissions.tpnpc)]
	[HelpText("Teleports you to an npc.")]
	[AllowServer(false)]
	public void TPNpc([AllowSpaces] string npcName)
	{
		if (String.IsNullOrWhiteSpace(npcName))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tpnpc <NPC>", TextOptions.CommandPrefix);
			return;
		}

		var npcStr = npcName; // string.Join(" ", args.Parameters);
		var matches = new List<NPC>();
		foreach (var npc in Main.npc.Where(npc => npc.active))
		{
			var englishName = EnglishLanguage.GetNpcNameById(npc.netID);

			if (string.Equals(npc.FullName, npcStr, StringComparison.InvariantCultureIgnoreCase) ||
				string.Equals(englishName, npcStr, StringComparison.InvariantCultureIgnoreCase))
			{
				matches = new List<NPC> { npc };
				break;
			}
			if (npc.FullName.ToLowerInvariant().StartsWith(npcStr.ToLowerInvariant()) ||
				englishName?.StartsWith(npcStr, StringComparison.InvariantCultureIgnoreCase) == true)
				matches.Add(npc);
		}

		if (matches.Count > 1)
		{
			Sender.SendMultipleMatchError(matches.Select(n => $"{n.FullName}({n.whoAmI})"));
			return;
		}
		if (matches.Count == 0)
		{
			Sender.SendErrorMessage("Invalid NPC!");
			return;
		}

		var target = matches[0];
		Sender.Teleport(target.position.X, target.position.Y);
		Sender.SendSuccessMessage("Teleported to the '{0}'.", target.FullName);
	}

	[Command("tppos")]
	[CommandPermissions(Permissions.tppos)]
	[HelpText("Teleports you to tile coordinates.")]
	[AllowServer(false)]
	public void TPPos(int x, int y)
	{
		//if (args.Parameters.Count != 2)
		//{
		//	Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}tppos <tile x> <tile y>", Specifier);
		//	return;
		//}

		//int x, y;
		//if (!int.TryParse(args.Parameters[0], out x) || !int.TryParse(args.Parameters[1], out y))
		//{
		//	Sender.SendErrorMessage("Invalid tile positions!");
		//	return;
		//}
		x = Math.Max(0, x);
		y = Math.Max(0, y);
		x = Math.Min(x, Main.maxTilesX - 1);
		y = Math.Min(y, Main.maxTilesY - 1);

		Sender.Teleport(16 * x, 16 * y);
		Sender.SendSuccessMessage("Teleported to {0}, {1}!", x, y);
	}

	[Command("pos")]
	[CommandPermissions(Permissions.getpos)]
	[HelpText("Returns the user's or specified user's current position.")]
	[AllowServer(false)]
	public void GetPos([AllowSpaces] string? playerName = null)
	{
		var player = String.IsNullOrWhiteSpace(playerName) ? Sender.Name : playerName;

		var players = TSPlayer.FindByNameOrID(player);
		if (players.Count == 0)
		{
			Sender.SendErrorMessage("Invalid player!");
		}
		else if (players.Count > 1)
		{
			Sender.SendMultipleMatchError(players.Select(p => p.Name));
		}
		else
		{
			Sender.SendSuccessMessage("Location of {0} is ({1}, {2}).", players[0].Name, players[0].TileX, players[0].TileY);
		}
	}

	[Command("tpallow")]
	[CommandPermissions(Permissions.tpallow)]
	[HelpText("Toggles whether other people can teleport you.")]
	[AllowServer(false)]
	public void TPAllow()
	{
		if (!Sender.TPAllow)
			Sender.SendSuccessMessage("You have removed your teleportation protection.");
		if (Sender.TPAllow)
			Sender.SendSuccessMessage("You have enabled teleportation protection.");
		Sender.TPAllow = !Sender.TPAllow;
	}
}
