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
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using TShockAPI;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

[Command("worldevent")]
[CommandPermissions(Permissions.manageevents)]
[HelpText("Enables starting and stopping various world events.")]
class EventCommands : CommandCallbacks<TSPlayer>
{
	static readonly List<string> _validEvents = new List<string>()
	{
		"meteor",
		"fullmoon",
		"bloodmoon",
		"eclipse",
		"invasion",
		"sandstorm",
		"rain",
		"lanternsnight"
	};
	static readonly List<string> _validInvasions = new List<string>()
	{
		"goblins",
		"snowmen",
		"pirates",
		"pumpkinmoon",
		"frostmoon",
		"martians"
	};

	[SubCommand(SubCommandType.Default)]
	public void ManageWorldEvent()
	{
		Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}worldevent <event type>", TextOptions.CommandPrefix);
		Sender.SendErrorMessage("Valid event types: {0}", String.Join(", ", _validEvents));
		Sender.SendErrorMessage("Valid invasion types if spawning an invasion: {0}", String.Join(", ", _validInvasions));
	}

	[SubCommand("meteor")]
	[CommandPermissions(Permissions.dropmeteor, Permissions.managemeteorevent)]
	public void Meteor()
	{
		WorldGen.spawnMeteor = false;
		WorldGen.dropMeteor();
		//if (args.Silent)
		//{
		//	Sender.SendInfoMessage("A meteor has been triggered.");
		//}
		//else
		{
			TSPlayer.All.SendInfoMessage("{0} triggered a meteor.", Sender.Name);
		}
	}

	[SubCommand("fullmoon", "full moon")]
	[CommandPermissions(Permissions.fullmoon, Permissions.managefullmoonevent)]
	public void Fullmoon()
	{
		TSPlayer.Server.SetFullMoon();
		//if (args.Silent)
		//{
		//	Sender.SendInfoMessage("Started a full moon.");
		//}
		//else
		{
			TSPlayer.All.SendInfoMessage("{0} started a full moon.", Sender.Name);
		}
	}

	[SubCommand("bloodmoon", "blood moon")]
	[CommandPermissions(Permissions.bloodmoon, Permissions.managebloodmoonevent)]
	public void Bloodmoon()
	{
		TSPlayer.Server.SetBloodMoon(!Main.bloodMoon);
		//if (args.Silent)
		//{
		//	Sender.SendInfoMessage("{0}ed a blood moon.", Main.bloodMoon ? "start" : "stopp");
		//}
		//else
		{
			TSPlayer.All.SendInfoMessage("{0} {1}ed a blood moon.", Sender.Name, Main.bloodMoon ? "start" : "stopp");
		}
	}

	[SubCommand("eclipse")]
	[CommandPermissions(Permissions.eclipse, Permissions.manageeclipseevent)]
	public void Eclipse()
	{
		TSPlayer.Server.SetEclipse(!Main.eclipse);
		//if (args.Silent)
		//{
		//	Sender.SendInfoMessage("{0}ed an eclipse.", Main.eclipse ? "start" : "stopp");
		//}
		//else
		{
			TSPlayer.All.SendInfoMessage("{0} {1}ed an eclipse.", Sender.Name, Main.eclipse ? "start" : "stopp");
		}
	}

	[SubCommand("invade", "invasion")]
	[CommandPermissions(Permissions.invade, Permissions.manageinvasionevent)]
	public void Invade(string? type = null, int wave = 1)
	{
		if (Main.invasionSize <= 0)
		{
			if (String.IsNullOrWhiteSpace(type))
			{
				Sender.SendErrorMessage("Invalid syntax! Proper syntax:  {0}worldevent invasion [invasion type] [invasion wave]", TextOptions.CommandPrefix);
				Sender.SendErrorMessage("Valid invasion types: {0}", String.Join(", ", _validInvasions));
				return;
			}

			//int wave = 1;
			switch (type.ToLowerInvariant())
			{
				case "goblin":
				case "goblins":
					TSPlayer.All.SendInfoMessage("{0} has started a goblin army invasion.", Sender.Name);
					TShock.Utils.StartInvasion(1);
					break;

				case "snowman":
				case "snowmen":
					TSPlayer.All.SendInfoMessage("{0} has started a snow legion invasion.", Sender.Name);
					TShock.Utils.StartInvasion(2);
					break;

				case "pirate":
				case "pirates":
					TSPlayer.All.SendInfoMessage("{0} has started a pirate invasion.", Sender.Name);
					TShock.Utils.StartInvasion(3);
					break;

				case "pumpkin":
				case "pumpkinmoon":
					//if (args.Parameters.Count > 2)
					{
						if (/*!int.TryParse(args.Parameters[2], out wave) ||*/ wave <= 0)
						{
							Sender.SendErrorMessage("Invalid wave!");
							break;
						}
					}

					TSPlayer.Server.SetPumpkinMoon(true);
					Main.bloodMoon = false;
					NPC.waveKills = 0f;
					NPC.waveNumber = wave;
					TSPlayer.All.SendInfoMessage("{0} started the pumpkin moon at wave {1}!", Sender.Name, wave);
					break;

				case "frost":
				case "frostmoon":
					//if (args.Parameters.Count > 2)
					{
						if (/*!int.TryParse(args.Parameters[2], out wave) ||*/ wave <= 0)
						{
							Sender.SendErrorMessage("Invalid wave!");
							return;
						}
					}

					TSPlayer.Server.SetFrostMoon(true);
					Main.bloodMoon = false;
					NPC.waveKills = 0f;
					NPC.waveNumber = wave;
					TSPlayer.All.SendInfoMessage("{0} started the frost moon at wave {1}!", Sender.Name, wave);
					break;

				case "martian":
				case "martians":
					TSPlayer.All.SendInfoMessage("{0} has started a martian invasion.", Sender.Name);
					TShock.Utils.StartInvasion(4);
					break;

				default:
					Sender.SendErrorMessage("Invalid invasion type! Valid invasion types: {0}", String.Join(", ", _validInvasions));
					break;
			}
		}
		else if (DD2Event.Ongoing)
		{
			DD2Event.StopInvasion();
			TSPlayer.All.SendInfoMessage("{0} has ended the Old One's Army event.", Sender.Name);
		}
		else
		{
			TSPlayer.All.SendInfoMessage("{0} has ended the invasion.", Sender.Name);
			Main.invasionSize = 0;
		}
	}

	[SubCommand("sandstorm")]
	[CommandPermissions(Permissions.sandstorm, Permissions.managesandstormevent)]
	public void Sandstorm()
	{
		if (Terraria.GameContent.Events.Sandstorm.Happening)
		{
			Terraria.GameContent.Events.Sandstorm.StopSandstorm();
			TSPlayer.All.SendInfoMessage("{0} stopped the sandstorm.", Sender.Name);
		}
		else
		{
			Terraria.GameContent.Events.Sandstorm.StartSandstorm();
			TSPlayer.All.SendInfoMessage("{0} started a sandstorm.", Sender.Name);
		}
	}

	[SubCommand("rain")]
	[CommandPermissions(Permissions.rain, Permissions.managerainevent)]
	public void Rain(string? type = null)
	{
		bool slime = false;
		if (type?.ToLowerInvariant() == "slime")
		{
			slime = true;
		}

		if (!slime)
		{
			Sender.SendInfoMessage("Use \"{0}worldevent rain slime\" to start slime rain!", TextOptions.CommandPrefix);
		}

		if (slime && Main.raining) //Slime rain cannot be activated during normal rain
		{
			Sender.SendErrorMessage("You should stop the current downpour before beginning a slimier one!");
			return;
		}

		if (slime && Main.slimeRain) //Toggle slime rain off
		{
			Main.StopSlimeRain(false);
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
			TSPlayer.All.SendInfoMessage("{0} ended the slimey downpour.", Sender.Name);
			return;
		}

		if (slime && !Main.slimeRain) //Toggle slime rain on
		{
			Main.StartSlimeRain(false);
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
			TSPlayer.All.SendInfoMessage("{0} caused it to rain slime.", Sender.Name);
		}

		if (Main.raining && !slime) //Toggle rain off
		{
			Main.StopRain();
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
			TSPlayer.All.SendInfoMessage("{0} ended the downpour.", Sender.Name);
			return;
		}

		if (!Main.raining && !slime) //Toggle rain on
		{
			Main.StartRain();
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
			TSPlayer.All.SendInfoMessage("{0} caused it to rain.", Sender.Name);
			return;
		}
	}

	[SubCommand("lanternsnight", "lanterns")]
	[CommandPermissions(Permissions.managelanternsnightevent)]
	public void LanternsNight()
	{
		LanternNight.ToggleManualLanterns();
		string msg = $" st{(LanternNight.LanternsUp ? "art" : "opp")}ed a lantern night.";
		//if (args.Silent)
		//{
		//	Sender.SendInfoMessage("You" + msg);
		//}
		//else
		{
			TSPlayer.All.SendInfoMessage(Sender.Name + msg);
		}
	}
}
