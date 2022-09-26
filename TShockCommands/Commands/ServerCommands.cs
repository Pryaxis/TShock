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
using System.Diagnostics;
using EasyCommands;
using EasyCommands.Commands;
using Terraria;
using TShockAPI;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

class ServerCommands : CommandCallbacks<TSPlayer>
{
	[Command("serverinfo")]
	[CommandPermissions(Permissions.serverinfo)]
	[HelpText("Shows the server information.")]
	public void ServerInfo()
	{
		Sender.SendInfoMessage("Memory usage: " + Process.GetCurrentProcess().WorkingSet64);
		Sender.SendInfoMessage("Allocated memory: " + Process.GetCurrentProcess().VirtualMemorySize64);
		Sender.SendInfoMessage("Total processor time: " + Process.GetCurrentProcess().TotalProcessorTime);
		Sender.SendInfoMessage("Operating system: " + Environment.OSVersion);
		Sender.SendInfoMessage("Proc count: " + Environment.ProcessorCount);
		Sender.SendInfoMessage("Machine name: " + Environment.MachineName);
	}

	[Command("worldinfo")]
	[CommandPermissions(Permissions.worldinfo)]
	[HelpText("Shows information about the current world.")]
	public void WorldInfo()
	{
		Sender.SendInfoMessage("Information about the currently running world");
		Sender.SendInfoMessage("Name: " + (TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName));
		Sender.SendInfoMessage("Size: {0}x{1}", Main.maxTilesX, Main.maxTilesY);
		Sender.SendInfoMessage("ID: " + Main.worldID);
		Sender.SendInfoMessage("Seed: " + WorldGen.currentWorldSeed);
		Sender.SendInfoMessage("Mode: " + Main.GameMode);
		Sender.SendInfoMessage("Path: " + Main.worldPathName);
	}
}
