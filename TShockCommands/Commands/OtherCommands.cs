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

class OtherCommands : CommandCallbacks<TSPlayer>
{
	//			#region Other Commands
	//			add(new Command(Permissions.buff, Buff, "buff")
	//			{
	//				AllowServer = false,
	//				HelpText = "Gives yourself a buff or debuff for an amount of time. Putting -1 for time will set it to 415 days."
	//			});
	//			add(new Command(Permissions.clear, Clear, "clear")
	//			{
	//				HelpText = "Clears item drops or projectiles."
	//			});
	//			add(new Command(Permissions.buffplayer, GBuff, "gbuff", "buffplayer")
	//			{
	//				HelpText = "Gives another player a buff or debuff for an amount of time. Putting -1 for time will set it to 415 days."
	//			});
	//			add(new Command(Permissions.godmode, ToggleGodMode, "godmode", "god")
	//			{
	//				HelpText = "Toggles godmode on a player."
	//			});
	//			add(new Command(Permissions.heal, Heal, "heal")
	//			{
	//				HelpText = "Heals a player in HP and MP."
	//			});
	//			add(new Command(Permissions.kill, Kill, "kill", "slay")
	//			{
	//				HelpText = "Kills another player."
	//			});
	//			add(new Command(Permissions.cantalkinthird, ThirdPerson, "me")
	//			{
	//				HelpText = "Sends an action message to everyone."
	//			});
	//			add(new Command(Permissions.canpartychat, PartyChat, "party", "p")
	//			{
	//				AllowServer = false,
	//				HelpText = "Sends a message to everyone on your team."
	//			});
	//			add(new Command(Permissions.whisper, Reply, "reply", "r")
	//			{
	//				HelpText = "Replies to a PM sent to you."
	//			});
	//			add(new Command(Rests.RestPermissions.restmanage, ManageRest, "rest")
	//			{
	//				HelpText = "Manages the REST API."
	//			});
	//			add(new Command(Permissions.slap, Slap, "slap")
	//			{
	//				HelpText = "Slaps a player, dealing damage."
	//			});
	//			add(new Command(Permissions.warp, Warp, "warp")
	//			{
	//				HelpText = "Teleports you to a warp point or manages warps."
	//			});
	//			add(new Command(Permissions.whisper, Whisper, "whisper", "w", "tell", "pm", "dm")
	//			{
	//				HelpText = "Sends a PM to a player."
	//			});
	//			add(new Command(Permissions.whisper, Wallow, "wallow", "wa")
	//			{
	//				AllowServer = false,
	//				HelpText = "Toggles to either ignore or recieve whispers from other players."
	//			});
	//			add(new Command(Permissions.createdumps, CreateDumps, "dump-reference-data")
	//			{
	//				HelpText = "Creates a reference tables for Terraria data types and the TShock permission system in the server folder."
	//			});
	//			add(new Command(Permissions.synclocalarea, SyncLocalArea, "sync")
	//			{
	//				HelpText = "Sends all tiles from the server to the player to resync the client with the actual world state."
	//			});
	//			add(new Command(Permissions.respawn, Respawn, "respawn")
	//			{
	//				HelpText = "Respawn yourself or another player."
	//			});
	//			#endregion
}
