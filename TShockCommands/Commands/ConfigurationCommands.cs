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

class ConfigurationCommands : CommandCallbacks<TSPlayer>
{
	//			#region Configuration Commands
	//			add(new Command(Permissions.maintenance, CheckUpdates, "checkupdates")
	//			{
	//				HelpText = "Checks for TShock updates."
	//			});
	//			add(new Command(Permissions.maintenance, Off, "off", "exit", "stop")
	//			{
	//				HelpText = "Shuts down the server while saving."
	//			});
	//			add(new Command(Permissions.maintenance, OffNoSave, "off-nosave", "exit-nosave", "stop-nosave")
	//			{
	//				HelpText = "Shuts down the server without saving."
	//			});
	//			add(new Command(Permissions.cfgreload, Reload, "reload")
	//			{
	//				HelpText = "Reloads the server configuration file."
	//			});
	//			add(new Command(Permissions.cfgpassword, ServerPassword, "serverpassword")
	//			{
	//				HelpText = "Changes the server password."
	//			});
	//			add(new Command(Permissions.maintenance, GetVersion, "version")
	//			{
	//				HelpText = "Shows the TShock version."
	//			});
	//			add(new Command(Permissions.whitelist, Whitelist, "whitelist")
	//			{
	//				HelpText = "Manages the server whitelist."
	//			});
	//			#endregion
}
