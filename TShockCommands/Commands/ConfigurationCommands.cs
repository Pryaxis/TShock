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
using System;
using System.IO;
using TShockAPI;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

class ConfigurationCommands : CommandCallbacks<TSPlayer>
{
	[Command("checkupdates")]
	[HelpText("Checks for TShock updates.")]
	[CommandPermissions(Permissions.maintenance)]
	public void CheckUpdates()
	{
		Sender.SendInfoMessage("An update check has been queued.");
		try
		{
			_ = TShock.UpdateManager.UpdateCheckAsync(null!);
		}
		catch (Exception)
		{
			//swallow the exception
			return;
		}
	}

	[Command("off", "exit", "stop")]
	[HelpText("Checks for TShock updates.")]
	[CommandPermissions(Permissions.maintenance)]
	public void Off(string? reason = null)
	{
		if (Terraria.Main.ServerSideCharacter)
		{
			foreach (TSPlayer player in TShock.Players)
			{
				if (player != null && player.IsLoggedIn && !player.IsDisabledPendingTrashRemoval)
				{
					player.SaveServerCharacter();
				}
			}
		}

		reason = (!String.IsNullOrWhiteSpace(reason) ? "Server shutting down: " + reason : "Server shutting down!");
		TShock.Utils.StopServer(true, reason);
	}

	[Command("off-nosave", "exit-nosave", "stop-nosave")]
	[HelpText("Shuts down the server without saving.")]
	[CommandPermissions(Permissions.maintenance)]
	public void OffNoSave(string? reason = null)
	{
		reason = (!String.IsNullOrWhiteSpace(reason) ? "Server shutting down: " + reason : "Server shutting down!");
		TShock.Utils.StopServer(false, reason);
	}

	[Command("reload")]
	[HelpText("Reloads the server configuration file.")]
	[CommandPermissions(Permissions.cfgreload)]
	public void Reload()
	{
		TShock.Utils.Reload();
		TShockAPI.Hooks.GeneralHooks.OnReloadEvent(Sender);

		Sender.SendSuccessMessage(
			"Configuration, permissions, and regions reload complete. Some changes may require a server restart.");
	}

	[Command("serverpassword")]
	[HelpText("Changes the server password.")]
	[CommandPermissions(Permissions.cfgpassword)]
	public void ServerPassword(string newpassword)
	{
		if (String.IsNullOrWhiteSpace(newpassword))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}serverpassword \"<new password>\"", TextOptions.CommandPrefix);
			return;
		}
		TShock.Config.Settings.ServerPassword = newpassword;
		Sender.SendSuccessMessage($"Server password has been changed to: {newpassword}.");
	}

	[Command("version")]
	[HelpText("Shows the TShock version.")]
	[CommandPermissions(Permissions.maintenance)]
	public void GetVersion()
	{
		Sender.SendMessage($"TShock: {TShock.VersionNum.Color(Utils.BoldHighlight)} {TShock.VersionCodename.Color(Utils.RedHighlight)}.", Color.White);
	}

	[Command("whitelist")]
	[HelpText("Manages the server whitelist.")]
	[CommandPermissions(Permissions.whitelist)]
	public void Whitelist(string target)
	{
		if (!String.IsNullOrWhiteSpace(target))
		{
			using (var tw = new StreamWriter(FileTools.WhitelistPath, true))
			{
				tw.WriteLine(target);
			}
			Sender.SendSuccessMessage("Added " + target + " to the whitelist.");
		}
	}
}
