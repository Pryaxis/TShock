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

class WorldCommands : CommandCallbacks<TSPlayer>
{
	//			#region World Commands
	//			add(new Command(Permissions.toggleexpert, ChangeWorldMode, "worldmode", "gamemode")
	//			{
	//				HelpText = "Changes the world mode."
	//			});
	//			add(new Command(Permissions.antibuild, ToggleAntiBuild, "antibuild")
	//			{
	//				HelpText = "Toggles build protection."
	//			});
	//			add(new Command(Permissions.grow, Grow, "grow")
	//			{
	//				AllowServer = false,
	//				HelpText = "Grows plants at your location."
	//			});
	//			add(new Command(Permissions.halloween, ForceHalloween, "forcehalloween")
	//			{
	//				HelpText = "Toggles halloween mode (goodie bags, pumpkins, etc)."
	//			});
	//			add(new Command(Permissions.xmas, ForceXmas, "forcexmas")
	//			{
	//				HelpText = "Toggles christmas mode (present spawning, santa, etc)."
	//			});
	//			add(new Command(Permissions.manageevents, ManageWorldEvent, "worldevent")
	//			{
	//				HelpText = "Enables starting and stopping various world events."
	//			});
	//			add(new Command(Permissions.hardmode, Hardmode, "hardmode")
	//			{
	//				HelpText = "Toggles the world's hardmode status."
	//			});
	//			add(new Command(Permissions.editspawn, ProtectSpawn, "protectspawn")
	//			{
	//				HelpText = "Toggles spawn protection."
	//			});
	//			add(new Command(Permissions.worldsave, Save, "save")
	//			{
	//				HelpText = "Saves the world file."
	//			});
	//			add(new Command(Permissions.worldspawn, SetSpawn, "setspawn")
	//			{
	//				AllowServer = false,
	//				HelpText = "Sets the world's spawn point to your location."
	//			});
	//			add(new Command(Permissions.dungeonposition, SetDungeon, "setdungeon")
	//			{
	//				AllowServer = false,
	//				HelpText = "Sets the dungeon's position to your location."
	//			});
	//			add(new Command(Permissions.worldsettle, Settle, "settle")
	//			{
	//				HelpText = "Forces all liquids to update immediately."
	//			});
	//			add(new Command(Permissions.time, Time, "time")
	//			{
	//				HelpText = "Sets the world time."
	//			});
	//			add(new Command(Permissions.wind, Wind, "wind")
	//			{
	//				HelpText = "Changes the wind speed."
	//			});
	//			#endregion
}
