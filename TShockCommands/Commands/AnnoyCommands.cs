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
using System.Threading;
using Terraria;
using Terraria.ID;
using TShockAPI;
using TShockCommands.Annotations;
using Utils = TShockAPI.Utils;

namespace TShockCommands.Commands;

class AnnoyCommands : CommandCallbacks<TSPlayer>
{
	[Command("annoy")]
	[HelpText("Annoys a player for an amount of time.")]
	[CommandPermissions(Permissions.annoy)]
	public void Annoy(TSPlayer player, int annoy = 5)
	{
		//if (args.Parameters.Count != 2)
		if (player is null || annoy <= 0)
		{
			Sender.SendMessage("Annoy Syntax", Color.White);
			Sender.SendMessage($"{"annoy".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}> <{"seconds".Color(Utils.PinkHighlight)}>", Color.White);
			Sender.SendMessage($"Example usage: {"annoy".Color(Utils.BoldHighlight)} <{Sender.Name.Color(Utils.RedHighlight)}> <{"10".Color(Utils.PinkHighlight)}>", Color.White);
			//Sender.SendMessage($"You can use {TextOptions.CommandPrefix SilentSpecifier.Color(Utils.GreenHighlight)} instead of {Specifier.Color(Utils.RedHighlight)} to annoy a player silently.", Color.White);
			return;
		}
		//int annoy = 5;
		//int.TryParse(args.Parameters[1], out annoy);

		//var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
		//if (players.Count == 0)
		//	Sender.SendErrorMessage($"Could not find any player named \"{args.Parameters[0]}\"");
		//else if (players.Count > 1)
		//	Sender.SendMultipleMatchError(players.Select(p => p.Name));
		//else
		{
			//var ply = players[0];
			Sender.SendSuccessMessage($"Annoying {player.Name} for {annoy} seconds.");
			//if (!args.Silent)
			player.SendMessage("You are now being annoyed.", Color.LightGoldenrodYellow);
			new Thread(player.Whoopie).Start(annoy);
		}
	}

	[Command("rocket")]
	[HelpText("Rockets a player upwards. Requires SSC.")]
	[CommandPermissions(Permissions.annoy)]
	public void Rocket(TSPlayer target)
	{
		if (target is null)
		{
			Sender.SendMessage("Rocket Syntax", Color.White);
			Sender.SendMessage($"{"rocket".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}>", Color.White);
			Sender.SendMessage($"Example usage: {"rocket".Color(Utils.BoldHighlight)} {Sender.Name.Color(Utils.RedHighlight)}", Color.White);
			//Sender.SendMessage($"You can use {SilentSpecifier.Color(Utils.GreenHighlight)} instead of {Specifier.Color(Utils.RedHighlight)} to rocket a player silently.", Color.White);
			return;
		}
		//var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
		//if (players.Count == 0)
		//	Sender.SendErrorMessage($"Could not find any player named \"{args.Parameters[0]}\"");
		//else if (players.Count > 1)
		//	Sender.SendMultipleMatchError(players.Select(p => p.Name));
		//else
		{
			//var target = players[0];

			if (target.IsLoggedIn && Terraria.Main.ServerSideCharacter)
			{
				target.TPlayer.velocity.Y = -50;
				TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", target.Index);

				//if (!args.Silent)
				{
					TSPlayer.All.SendInfoMessage($"{Sender.Name} has launched {(target == Sender ? (Sender.TPlayer.Male ? "himself" : "herself") : target.Name)} into space.");
					return;
				}

				if (target == Sender)
					Sender.SendSuccessMessage("You have launched yourself into space.");
				else
					Sender.SendSuccessMessage($"You have launched {target.Name} into space.");
			}
			else
			{
				if (!Terraria.Main.ServerSideCharacter)
					Sender.SendErrorMessage("SSC must be enabled to use this command.");
				else
					Sender.SendErrorMessage($"Unable to rocket {target.Name} because {(target.TPlayer.Male ? "he" : "she")} is not logged in.");
			}
		}
	}

	[Command("firework")]
	[HelpText("Spawns fireworks at a player.")]
	[CommandPermissions(Permissions.annoy)]
	public void FireWork(TSPlayer target, string projectile = "red")
	{
		var user = Sender;
		if (target is null)
		{
			// firework <player> [R|G|B|Y]
			user.SendMessage("Firework Syntax", Color.White);
			user.SendMessage($"{"firework".Color(Utils.CyanHighlight)} <{"player".Color(Utils.PinkHighlight)}> [{"R".Color(Utils.RedHighlight)}|{"G".Color(Utils.GreenHighlight)}|{"B".Color(Utils.BoldHighlight)}|{"Y".Color(Utils.YellowHighlight)}]", Color.White);
			user.SendMessage($"Example usage: {"firework".Color(Utils.CyanHighlight)} {user.Name.Color(Utils.PinkHighlight)} {"R".Color(Utils.RedHighlight)}", Color.White);
			//user.SendMessage($"You can use {SilentSpecifier.Color(Utils.GreenHighlight)} instead of {Specifier.Color(Utils.RedHighlight)} to launch a firework silently.", Color.White);
			return;
		}
		//var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
		//if (players.Count == 0)
		//	user.SendErrorMessage($"Could not find any player named \"{args.Parameters[0]}\"");
		//else if (players.Count > 1)
		//	user.SendMultipleMatchError(players.Select(p => p.Name));
		//else
		{
			int type = ProjectileID.RocketFireworkRed;
			if (projectile is not null)
			{
				switch (projectile.ToLower())
				{
					case "red":
					case "r":
						type = ProjectileID.RocketFireworkRed;
						break;
					case "green":
					case "g":
						type = ProjectileID.RocketFireworkGreen;
						break;
					case "blue":
					case "b":
						type = ProjectileID.RocketFireworkBlue;
						break;
					case "yellow":
					case "y":
						type = ProjectileID.RocketFireworkYellow;
						break;
					case "r2":
					case "star":
						type = ProjectileID.RocketFireworksBoxRed;
						break;
					case "g2":
					case "spiral":
						type = ProjectileID.RocketFireworksBoxGreen;
						break;
					case "b2":
					case "rings":
						type = ProjectileID.RocketFireworksBoxBlue;
						break;
					case "y2":
					case "flower":
						type = ProjectileID.RocketFireworksBoxYellow;
						break;
					default:
						type = ProjectileID.RocketFireworkRed;
						break;
				}
			}
			//var target = players[0];
			int p = Projectile.NewProjectile(Projectile.GetNoneSource(), target.TPlayer.position.X, target.TPlayer.position.Y - 64f, 0f, -8f, type, 0, 0);
			Main.projectile[p].Kill();
			Sender.SendSuccessMessage($"You launched fireworks on {(target == user ? "yourself" : target.Name)}.");
			if (/*!args.Silent &&*/ target != user)
				target.SendSuccessMessage($"{user.Name} launched fireworks on you.");
		}
	}
}
