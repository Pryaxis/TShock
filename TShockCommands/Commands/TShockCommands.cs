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

class TShockCommands : CommandCallbacks<TSPlayer>
{
	//			add(new Command(SetupToken, "setup")
	//			{
	//				AllowServer = false,
	//				HelpText = "Used to authenticate as superadmin when first setting up TShock."
	//			});
	//			add(new Command(Permissions.user, ManageUsers, "user")
	//			{
	//				DoLog = false,
	//				HelpText = "Manages user accounts."
	//			});


	//			add(new Command(Aliases, "aliases")
	//			{
	//				HelpText = "Shows a command's aliases."
	//			});
	//			add(new Command(Help, "help")
	//			{
	//				HelpText = "Lists commands or gives help on them."
	//			});
	//			add(new Command(Motd, "motd")
	//			{
	//				HelpText = "Shows the message of the day."
	//			});
	//			add(new Command(ListConnectedPlayers, "playing", "online", "who")
	//			{
	//				HelpText = "Shows the currently connected players."
	//			});
	//			add(new Command(Rules, "rules")
	//			{
	//				HelpText = "Shows the server's rules."
	//			});
}
