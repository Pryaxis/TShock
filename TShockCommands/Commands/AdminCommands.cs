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

class AdminCommands : CommandCallbacks<TSPlayer>
{
	//			#region Admin Commands
	//			add(new Command(Permissions.ban, Ban, "ban")
	//			{
	//				HelpText = "Manages player bans."
	//			});
	//			add(new Command(Permissions.broadcast, Broadcast, "broadcast", "bc", "say")
	//			{
	//				HelpText = "Broadcasts a message to everyone on the server."
	//			});
	//			add(new Command(Permissions.logs, DisplayLogs, "displaylogs")
	//			{
	//				HelpText = "Toggles whether you receive server logs."
	//			});
	//			add(new Command(Permissions.managegroup, Group, "group")
	//			{
	//				HelpText = "Manages groups."
	//			});
	//			add(new Command(Permissions.manageitem, ItemBan, "itemban")
	//			{
	//				HelpText = "Manages item bans."
	//			});
	//			add(new Command(Permissions.manageprojectile, ProjectileBan, "projban")
	//			{
	//				HelpText = "Manages projectile bans."
	//			});
	//			add(new Command(Permissions.managetile, TileBan, "tileban")
	//			{
	//				HelpText = "Manages tile bans."
	//			});
	//			add(new Command(Permissions.manageregion, Region, "region")
	//			{
	//				HelpText = "Manages regions."
	//			});
	//			add(new Command(Permissions.kick, Kick, "kick")
	//			{
	//				HelpText = "Removes a player from the server."
	//			});
	//			add(new Command(Permissions.mute, Mute, "mute", "unmute")
	//			{
	//				HelpText = "Prevents a player from talking."
	//			});
	//			add(new Command(Permissions.savessc, OverrideSSC, "overridessc", "ossc")
	//			{
	//				HelpText = "Overrides serverside characters for a player, temporarily."
	//			});
	//			add(new Command(Permissions.savessc, SaveSSC, "savessc")
	//			{
	//				HelpText = "Saves all serverside characters."
	//			});
	//			add(new Command(Permissions.uploaddata, UploadJoinData, "uploadssc")
	//			{
	//				HelpText = "Upload the account information when you joined the server as your Server Side Character data."
	//			});
	//			add(new Command(Permissions.settempgroup, TempGroup, "tempgroup")
	//			{
	//				HelpText = "Temporarily sets another player's group."
	//			});
	//			add(new Command(Permissions.su, SubstituteUser, "su")
	//			{
	//				HelpText = "Temporarily elevates you to Super Admin."
	//			});
	//			add(new Command(Permissions.su, SubstituteUserDo, "sudo")
	//			{
	//				HelpText = "Executes a command as the super admin."
	//			});
	//			add(new Command(Permissions.userinfo, GrabUserUserInfo, "userinfo", "ui")
	//			{
	//				HelpText = "Shows information about a player."
	//			});
	//			#endregion



}
