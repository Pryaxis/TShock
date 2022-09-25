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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Terraria;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockCommands.Annotations;
using Utils = TShockAPI.Utils;

namespace TShockCommands.Commands;

class AccountCommands : CommandCallbacks<TSPlayer>
{
	[Command("login")]
	[HelpText("Logs you into an account.")]
	[CommandPermissions(Permissions.canlogin)]
	[DoLog(false), AllowServer(false)]
	[SyntaxOverride("[username] <password>")] // legacy format
	public void AttemptLogin(string? first = null, string? second = null)
	{
		var bHasFirst = !String.IsNullOrWhiteSpace(first);
		var bHasSecond = !String.IsNullOrWhiteSpace(second);

		string? name = null, password = null;
		if (bHasFirst && bHasSecond)
		{
			name = first;
			password = second;
		}
		else if (bHasFirst)
		{
			password = first;
		}

		if (Sender.LoginAttempts > TShock.Config.Settings.MaximumLoginAttempts && (TShock.Config.Settings.MaximumLoginAttempts != -1))
		{
			TShock.Log.Warn($"{Sender.IP} ({Sender.Name}) had {TShock.Config.Settings.MaximumLoginAttempts} or more invalid login attempts and was kicked automatically.");
			Sender.Kick("Too many invalid login attempts.");
			return;
		}

		if (Sender.IsLoggedIn)
		{
			Sender.SendErrorMessage("You are already logged in, and cannot login again.");
			return;
		}

		UserAccount? account = TShock.UserAccounts.GetUserAccountByName(Sender.Name);

		bool usingUUID = false;

		if (!String.IsNullOrWhiteSpace(password))
		{
			if (TShock.Config.Settings.AllowLoginAnyUsername
				&& !String.IsNullOrWhiteSpace(name))
			{
				if (PlayerHooks.OnPlayerPreLogin(Sender, name, password))
					return;

				account = TShock.UserAccounts.GetUserAccountByName(name);
			}
			else
			{
				if (PlayerHooks.OnPlayerPreLogin(Sender, Sender.Name, password))
					return;
			}
		}
		else if (!TShock.Config.Settings.DisableUUIDLogin)
		{
			password = "";
			if (PlayerHooks.OnPlayerPreLogin(Sender, Sender.Name, password))
				return;
			usingUUID = true;
		}
		else
		{
			if (!TShock.Config.Settings.DisableUUIDLogin)
				Sender.SendMessage($"{TextOptions.CommandPrefix}login - Logs in using your UUID and character name.", Color.White);

			if (TShock.Config.Settings.AllowLoginAnyUsername)
				Sender.SendMessage($"{TextOptions.CommandPrefix}login {"username".Color(Utils.GreenHighlight)} {"password".Color(Utils.BoldHighlight)} - Logs in using your username and password.", Color.White);
			else
				Sender.SendMessage($"{TextOptions.CommandPrefix}login {"password".Color(Utils.BoldHighlight)} - Logs in using your password and character name.", Color.White);

			Sender.SendWarningMessage("If you forgot your password, there is no way to recover it.");
			return;
		}

		try
		{
			if (account is null)
			{
				Sender.SendErrorMessage("A user account by that name does not exist.");
			}
			else if (account.VerifyPassword(password) ||
					(usingUUID && account.UUID == Sender.UUID && !TShock.Config.Settings.DisableUUIDLogin &&
					!String.IsNullOrWhiteSpace(Sender.UUID)))
			{
				var group = TShock.Groups.GetGroupByName(account.Group);

				if (!TShock.Groups.AssertGroupValid(Sender, group, false))
				{
					Sender.SendErrorMessage("Login attempt failed - see the message above.");
					return;
				}

				Sender.PlayerData = TShock.CharacterDB.GetPlayerData(Sender, account.ID);

				Sender.Group = group;
				Sender.tempGroup = null;
				Sender.Account = account;
				Sender.IsLoggedIn = true;
				Sender.IsDisabledForSSC = false;

				if (Main.ServerSideCharacter)
				{
					if (Sender.HasPermission(Permissions.bypassssc))
					{
						Sender.PlayerData.CopyCharacter(Sender);
						TShock.CharacterDB.InsertPlayerData(Sender);
					}
					Sender.PlayerData.RestoreCharacter(Sender);
				}
				Sender.LoginFailsBySsi = false;

				if (Sender.HasPermission(Permissions.ignorestackhackdetection))
					Sender.IsDisabledForStackDetection = false;

				if (Sender.HasPermission(Permissions.usebanneditem))
					Sender.IsDisabledForBannedWearable = false;

				Sender.SendSuccessMessage("Authenticated as " + account.Name + " successfully.");

				TShock.Log.ConsoleInfo(Sender.Name + " authenticated successfully as user: " + account.Name + ".");
				if ((Sender.LoginHarassed) && (TShock.Config.Settings.RememberLeavePos))
				{
					if (TShock.RememberedPos.GetLeavePos(Sender.Name, Sender.IP) != Vector2.Zero)
					{
						Vector2 pos = TShock.RememberedPos.GetLeavePos(Sender.Name, Sender.IP);
						Sender.Teleport((int)pos.X * 16, (int)pos.Y * 16);
					}
					Sender.LoginHarassed = false;
				}
				TShock.UserAccounts.SetUserAccountUUID(account, Sender.UUID);

				PlayerHooks.OnPlayerPostLogin(Sender);
			}
			else
			{
				if (usingUUID && !TShock.Config.Settings.DisableUUIDLogin)
				{
					Sender.SendErrorMessage("UUID does not match this character!");
				}
				else
				{
					Sender.SendErrorMessage("Invalid password!");
				}
				TShock.Log.Warn(Sender.IP + " failed to authenticate as user: " + account.Name + ".");
				Sender.LoginAttempts++;
			}
		}
		catch (Exception ex)
		{
			Sender.SendErrorMessage("There was an error processing your request.");
			TShock.Log.Error(ex.ToString());
		}
	}

	[Command("logout")]
	[HelpText("Logs you out of your current account.")]
	[CommandPermissions(Permissions.canlogout)]
	[DoLog(false), AllowServer(false)]
	public void Logout()
	{
		if (!Sender.IsLoggedIn)
		{
			Sender.SendErrorMessage("You are not logged in.");
			return;
		}

		if (Sender.TPlayer.talkNPC != -1)
		{
			Sender.SendErrorMessage("Please close NPC windows before logging out.");
			return;
		}

		Sender.Logout();
		Sender.SendSuccessMessage("You have been successfully logged out of your account.");
		if (Main.ServerSideCharacter)
		{
			Sender.SendWarningMessage("Server side characters are enabled. You need to be logged in to play.");
		}
	}

	[Command("password")]
	[HelpText("Changes your account's password.")]
	[CommandPermissions(Permissions.canchangepassword)]
	[DoLog(false), AllowServer(false)]
	public void PasswordUser(string? oldpassword, string? newpassword)
	{
		try
		{
			if (Sender.IsLoggedIn
				&& !String.IsNullOrWhiteSpace(oldpassword)
				&& !String.IsNullOrWhiteSpace(newpassword)
			)
			{
				if (Sender.Account.VerifyPassword(oldpassword))
				{
					try
					{
						Sender.SendSuccessMessage("You changed your password!");
						TShock.UserAccounts.SetUserAccountPassword(Sender.Account, newpassword); // SetUserPassword will hash it for you.
						TShock.Log.ConsoleInfo(Sender.IP + " named " + Sender.Name + " changed the password of account " +
											   Sender.Account.Name + ".");
					}
					catch (ArgumentOutOfRangeException)
					{
						Sender.SendErrorMessage("Password must be greater than or equal to " + TShock.Config.Settings.MinimumPasswordLength + " characters.");
					}
				}
				else
				{
					Sender.SendErrorMessage("You failed to change your password!");
					TShock.Log.ConsoleError(Sender.IP + " named " + Sender.Name + " failed to change password for account: " +
											Sender.Account.Name + ".");
				}
			}
			else
			{
				Sender.SendErrorMessage("Not logged in or invalid syntax! Proper syntax: {0}password <oldpassword> <newpassword>", TextOptions.CommandPrefix);
			}
		}
		catch (UserAccountManagerException ex)
		{
			Sender.SendErrorMessage("Sorry, an error occurred: " + ex.Message + ".");
			TShock.Log.ConsoleError("PasswordUser returned an error: " + ex);
		}
	}

	[Command("register")]
	[HelpText("Registers you an account.")]
	[CommandPermissions(Permissions.canregister)]
	[DoLog(false), AllowServer(false)]
	[SyntaxOverride("[username] <password>")] // legacy format
	public void RegisterUser(string? first = null, string? second = null)
	{
		var bHasFirst = !String.IsNullOrWhiteSpace(first);
		var bHasSecond = !String.IsNullOrWhiteSpace(second);

		string? username = null, password = null;
		if (bHasFirst && bHasSecond)
		{
			username = first;
			password = second;
		}
		else if (bHasFirst)
		{
			password = first;
		}

		try
		{
			var account = new UserAccount();
			string echoPassword = "";
			if (String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password))
			{
				account.Name = Sender.Name;
				echoPassword = password;
				try
				{
					account.CreateBCryptHash(password);
				}
				catch (ArgumentOutOfRangeException)
				{
					Sender.SendErrorMessage("Password must be greater than or equal to " + TShock.Config.Settings.MinimumPasswordLength + " characters.");
					return;
				}
			}
			else if (!String.IsNullOrWhiteSpace(username) && !String.IsNullOrWhiteSpace(password) && TShock.Config.Settings.AllowRegisterAnyUsername)
			{
				account.Name = username;
				echoPassword = password;
				try
				{
					account.CreateBCryptHash(password);
				}
				catch (ArgumentOutOfRangeException)
				{
					Sender.SendErrorMessage("Password must be greater than or equal to " + TShock.Config.Settings.MinimumPasswordLength + " characters.");
					return;
				}
			}
			else
			{
				Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}register <password>", TextOptions.CommandPrefix);
				return;
			}

			account.Group = TShock.Config.Settings.DefaultRegistrationGroupName;
			account.UUID = Sender.UUID;

			if (TShock.UserAccounts.GetUserAccountByName(account.Name) is null && account.Name != TSServerPlayer.AccountName) // Cheap way of checking for existance of a user
			{
				Sender.SendSuccessMessage("Account \"{0}\" has been registered.", account.Name);
				Sender.SendSuccessMessage("Your password is {0}.", echoPassword);

				if (!TShock.Config.Settings.DisableUUIDLogin)
					Sender.SendMessage($"Type {TextOptions.CommandPrefix}login to sign in to your account using your UUID.", Color.White);

				if (TShock.Config.Settings.AllowLoginAnyUsername)
					Sender.SendMessage($"Type {TextOptions.CommandPrefix}login \"{account.Name.Color(Utils.GreenHighlight)}\" {echoPassword.Color(Utils.BoldHighlight)} to sign in to your account.", Color.White);
				else
					Sender.SendMessage($"Type {TextOptions.CommandPrefix}login {echoPassword.Color(Utils.BoldHighlight)} to sign in to your account.", Color.White);

				TShock.UserAccounts.AddUserAccount(account);
				TShock.Log.ConsoleInfo("{0} registered an account: \"{1}\".", Sender.Name, account.Name);
			}
			else
			{
				Sender.SendErrorMessage("Sorry, " + account.Name + " was already taken by another person.");
				Sender.SendErrorMessage("Please try a different username.");
				TShock.Log.ConsoleInfo(Sender.Name + " failed to register an existing account: " + account.Name);
			}
		}
		catch (UserAccountManagerException ex)
		{
			Sender.SendErrorMessage("Sorry, an error occurred: " + ex.Message + ".");
			TShock.Log.ConsoleError("RegisterUser returned an error: " + ex);
		}
	}

	[Command("accountinfo", "ai")]
	[HelpText("Shows information about a user.")]
	[CommandPermissions(Permissions.checkaccountinfo)]
	public void ViewAccountInfo([AllowSpaces] string? username = null)
	{
		if (String.IsNullOrWhiteSpace(username))
		{
			Sender.SendErrorMessage("Invalid syntax! Proper syntax: {0}accountinfo <username>", TextOptions.CommandPrefix);
			return;
		}

		var account = TShock.UserAccounts.GetUserAccountByName(username);
		if (account is not null)
		{
			DateTime LastSeen;

			string Timezone = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours.ToString("+#;-#");

			if (DateTime.TryParse(account.LastAccessed, out LastSeen))
			{
				LastSeen = DateTime.Parse(account.LastAccessed).ToLocalTime();
				Sender.SendSuccessMessage("{0}'s last login occurred {1} {2} UTC{3}.", account.Name, LastSeen.ToShortDateString(),
					LastSeen.ToShortTimeString(), Timezone);
			}

			if (Sender.Group.HasPermission(Permissions.advaccountinfo))
			{
				List<string> KnownIps = JsonConvert.DeserializeObject<List<string>>(account.KnownIps?.ToString() ?? string.Empty);
				string ip = KnownIps?[KnownIps.Count - 1] ?? "N/A";
				DateTime Registered = DateTime.Parse(account.Registered).ToLocalTime();

				Sender.SendSuccessMessage("{0}'s group is {1}.", account.Name, account.Group);
				Sender.SendSuccessMessage("{0}'s last known IP is {1}.", account.Name, ip);
				Sender.SendSuccessMessage("{0}'s register date is {1} {2} UTC{3}.", account.Name, Registered.ToShortDateString(), Registered.ToShortTimeString(), Timezone);
			}
		}
		else
			Sender.SendErrorMessage("User {0} does not exist.", username);
	}
}
