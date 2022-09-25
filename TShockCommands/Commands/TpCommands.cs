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

class TpCommands : CommandCallbacks<TSPlayer>
{
	//			#region TP Commands
	//			add(new Command(Permissions.home, Home, "home")
	//			{
	//				AllowServer = false,
	//				HelpText = "Sends you to your spawn point."
	//			});
	//			add(new Command(Permissions.spawn, Spawn, "spawn")
	//			{
	//				AllowServer = false,
	//				HelpText = "Sends you to the world's spawn point."
	//			});
	//			add(new Command(Permissions.tp, TP, "tp")
	//			{
	//				AllowServer = false,
	//				HelpText = "Teleports a player to another player."
	//			});
	//			add(new Command(Permissions.tpothers, TPHere, "tphere")
	//			{
	//				AllowServer = false,
	//				HelpText = "Teleports a player to yourself."
	//			});
	//			add(new Command(Permissions.tpnpc, TPNpc, "tpnpc")
	//			{
	//				AllowServer = false,
	//				HelpText = "Teleports you to an npc."
	//			});
	//			add(new Command(Permissions.tppos, TPPos, "tppos")
	//			{
	//				AllowServer = false,
	//				HelpText = "Teleports you to tile coordinates."
	//			});
	//			add(new Command(Permissions.getpos, GetPos, "pos")
	//			{
	//				AllowServer = false,
	//				HelpText = "Returns the user's or specified user's current position."
	//			});
	//			add(new Command(Permissions.tpallow, TPAllow, "tpallow")
	//			{
	//				AllowServer = false,
	//				HelpText = "Toggles whether other people can teleport you."
	//			});
	//			#endregion
}
