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
using System.Linq;
using TShockAPI;
using TShockAPI.DB;
using TShockCommands.Annotations;

namespace TShockCommands.Commands;

[Command("user")]
[HelpText("Manages user accounts.")]
[CommandPermissions(Permissions.user)]
class UserCommands : CommandCallbacks<TSPlayer>
{
	[SubCommand]
	public void Add(string username, string password, string group)
	{
		var account = new UserAccount();

		account.Name = username;
		try
		{
			account.CreateBCryptHash(password);
		}
		catch (ArgumentOutOfRangeException)
		{
			Sender.SendErrorMessage("Password must be greater than or equal to " + TShock.Config.Settings.MinimumPasswordLength + " characters.");
			return;
		}
		account.Group = group;

		try
		{
			TShock.UserAccounts.AddUserAccount(account);
			Sender.SendSuccessMessage("Account " + account.Name + " has been added to group " + account.Group + "!");
			TShock.Log.ConsoleInfo(Sender.Name + " added Account " + account.Name + " to group " + account.Group);
		}
		catch (GroupNotExistsException)
		{
			Sender.SendErrorMessage("Group " + account.Group + " does not exist!");
		}
		catch (UserAccountExistsException)
		{
			Sender.SendErrorMessage("User " + account.Name + " already exists!");
		}
		catch (UserAccountManagerException e)
		{
			Sender.SendErrorMessage("User " + account.Name + " could not be added, check console for details.");
			TShock.Log.ConsoleError(e.ToString());
		}
	}

	[SubCommand("del")]
	public void Delete(string username)
	{
		var account = new UserAccount();
		account.Name = username;

		try
		{
			TShock.UserAccounts.RemoveUserAccount(account);
			Sender.SendSuccessMessage("Account removed successfully.");
			TShock.Log.ConsoleInfo(Sender.Name + " successfully deleted account: " + username + ".");
		}
		catch (UserAccountNotExistException)
		{
			Sender.SendErrorMessage("The user " + account.Name + " does not exist! Deleted nobody!");
		}
		catch (UserAccountManagerException ex)
		{
			Sender.SendErrorMessage(ex.Message);
			TShock.Log.ConsoleError(ex.ToString());
		}
	}

	[SubCommand]
	public void Password(string username, string newpassword)
	{
		var account = new UserAccount();
		account.Name = username;

		try
		{
			TShock.UserAccounts.SetUserAccountPassword(account, newpassword);
			TShock.Log.ConsoleInfo(Sender.Name + " changed the password of account " + account.Name);
			Sender.SendSuccessMessage("Password change succeeded for " + account.Name + ".");
		}
		catch (UserAccountNotExistException)
		{
			Sender.SendErrorMessage("User " + account.Name + " does not exist!");
		}
		catch (UserAccountManagerException e)
		{
			Sender.SendErrorMessage("Password change for " + account.Name + " failed! Check console!");
			TShock.Log.ConsoleError(e.ToString());
		}
		catch (ArgumentOutOfRangeException)
		{
			Sender.SendErrorMessage("Password must be greater than or equal to " + TShock.Config.Settings.MinimumPasswordLength + " characters.");
		}
	}

	[SubCommand]
	public void Group(string username, string newgroup/*, CommandArgs args*/)
	{
		var account = new UserAccount();
		account.Name = username;

		try
		{
			TShock.UserAccounts.SetUserGroup(account, newgroup);
			TShock.Log.ConsoleInfo(Sender.Name + " changed account " + account.Name + " to group " + newgroup + ".");
			Sender.SendSuccessMessage("Account " + account.Name + " has been changed to group " + newgroup + "!");

			//send message to player with matching account name
			var player = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == account.Name);
			if (player != null /*&& !args.Silent*/)
				player.SendSuccessMessage($"{Sender.Name} has changed your group to {newgroup}");
		}
		catch (GroupNotExistsException)
		{
			Sender.SendErrorMessage("That group does not exist!");
		}
		catch (UserAccountNotExistException)
		{
			Sender.SendErrorMessage("User " + account.Name + " does not exist!");
		}
		catch (UserAccountManagerException e)
		{
			Sender.SendErrorMessage("User " + account.Name + " could not be added. Check console for details.");
			TShock.Log.ConsoleError(e.ToString());
		}
	}

	[SubCommand]
	public void Help()
	{
		Sender.SendInfoMessage("Use command help:");
		Sender.SendInfoMessage("{0}user add username password group   -- Adds a specified user", TextOptions.CommandPrefix);
		Sender.SendInfoMessage("{0}user del username                  -- Removes a specified user", TextOptions.CommandPrefix);
		Sender.SendInfoMessage("{0}user password username newpassword -- Changes a user's password", TextOptions.CommandPrefix);
		Sender.SendInfoMessage("{0}user group username newgroup       -- Changes a user's group", TextOptions.CommandPrefix);
	}
}
