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
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

class ServerCommands : CommandCallbacks<TSPlayer>
{
	//			add(new Command(Permissions.serverinfo, ServerInfo, "serverinfo")
	//			{
	//				HelpText = "Shows the server information."
	//			});
	//			add(new Command(Permissions.worldinfo, WorldInfo, "worldinfo")
	//			{
	//				HelpText = "Shows information about the current world."
	//			});

	//		#region Stupid commands

	//		private static void ServerInfo(CommandArgs args)
	//		{
	//			args.Player.SendInfoMessage("Memory usage: " + Process.GetCurrentProcess().WorkingSet64);
	//			args.Player.SendInfoMessage("Allocated memory: " + Process.GetCurrentProcess().VirtualMemorySize64);
	//			args.Player.SendInfoMessage("Total processor time: " + Process.GetCurrentProcess().TotalProcessorTime);
	//			args.Player.SendInfoMessage("Operating system: " + Environment.OSVersion);
	//			args.Player.SendInfoMessage("Proc count: " + Environment.ProcessorCount);
	//			args.Player.SendInfoMessage("Machine name: " + Environment.MachineName);
	//		}

	//		private static void WorldInfo(CommandArgs args)
	//		{
	//			args.Player.SendInfoMessage("Information about the currently running world");
	//			args.Player.SendInfoMessage("Name: " + (TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName));
	//			args.Player.SendInfoMessage("Size: {0}x{1}", Main.maxTilesX, Main.maxTilesY);
	//			args.Player.SendInfoMessage("ID: " + Main.worldID);
	//			args.Player.SendInfoMessage("Seed: " + WorldGen.currentWorldSeed);
	//			args.Player.SendInfoMessage("Mode: " + Main.GameMode);
	//			args.Player.SendInfoMessage("Path: " + Main.worldPathName);
	//		}

	//		#endregion
}
