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

using System.Collections.Generic;
using System.ComponentModel;
using TShockAPI.DB;

namespace TShockAPI.Hooks
{
	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerPostLogin"/> event.
	/// </summary>
	public class PlayerPostLoginEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// Initializes a new instance of the PlayerPostLoginEventArgs class.
		/// </summary>
		/// <param name="ply">The player who fired the event.</param>
		public PlayerPostLoginEventArgs(TSPlayer ply)
		{
			Player = ply;
		}
	}

	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerPreLogin"/> event.
	/// </summary>
	public class PlayerPreLoginEventArgs : HandledEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// The player's login name.
		/// </summary>
		public string LoginName { get; set; }

		/// <summary>
		/// The player's raw password.
		/// </summary>
		public string Password { get; set; }
	}

	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerLogout"/> event.
	/// </summary>
	public class PlayerLogoutEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// Initializes a new instance of the PlayerLogoutEventArgs class.
		/// </summary>
		/// <param name="player">The player who fired the event.</param>
		public PlayerLogoutEventArgs(TSPlayer player)
		{
			Player = player;
		}
	}

	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerCommand"/> event.
	/// </summary>
	public class PlayerCommandEventArgs : HandledEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// The command's name that follows the <see cref="Commands.Specifier"/>.
		/// </summary>
		public string CommandName { get; set; }

		/// <summary>
		/// The command's full text.
		/// </summary>
		public string CommandText { get; set; }

		/// <summary>
		/// The command's parameters extracted from <see cref="CommandText"/>.
		/// </summary>
		public List<string> Parameters { get; set; }

		/// <summary>
		/// The full list of server commands.
		/// </summary>
		public IEnumerable<Command> CommandList { get; set; }

		/// <summary>
		/// The prefix used to send the command (either <see cref="Commands.Specifier"/> or <see cref="Commands.SilentSpecifier"/>).
		/// </summary>
		public string CommandPrefix { get; set; }
	}

	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerChat"/> event.
	/// </summary>
	public class PlayerChatEventArgs : HandledEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// The raw chat text as received by the server.
		/// </summary>
		public string RawText { get; set; }

		/// <summary>
		/// The <see cref="RawText"/> string after being formatted by TShock as specified in the config file.
		/// </summary>
		public string TShockFormattedText { get; set; }
	}

	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerPermission"/> event.
	/// </summary>
	public class PlayerPermissionEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// The permission being checked.
		/// </summary>
		public string Permission { get; set; }

		/// <summary>
		/// <see cref="PermissionHookResult"/> of the hook.
		/// </summary>
		public PermissionHookResult Result { get; set; }

		/// <summary>
		/// Initializes a new instance of the PlayerPermissionEventArgs class.
		/// </summary>
		/// <param name="player">The player who fired the event.</param>
		/// <param name="permission">The permission being checked.</param>
		public PlayerPermissionEventArgs(TSPlayer player, string permission)
		{
			Player = player;
			Permission = permission;
			Result = PermissionHookResult.Unhandled;
		}
	}

	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerItembanPermission"/> event.
	/// </summary>
	public class PlayerItembanPermissionEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// The banned item being checked.
		/// </summary>
		public ItemBan BannedItem { get; set; }

		/// <summary>
		/// <see cref="PermissionHookResult"/> of the hook.
		/// </summary>
		public PermissionHookResult Result { get; set; }

		/// <summary>
		/// Initializes a new instance of the PlayerItembanPermissionEventArgs class.
		/// </summary>
		/// <param name="player">The player who fired the event.</param>
		/// <param name="bannedItem">The banned item being checked.</param>
		public PlayerItembanPermissionEventArgs(TSPlayer player, ItemBan bannedItem)
		{
			Player = player;
			BannedItem = bannedItem;
			Result = PermissionHookResult.Unhandled;
		}
	}

	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerProjbanPermission"/> event.
	/// </summary>
	public class PlayerProjbanPermissionEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// The banned projectile being checked.
		/// </summary>
		public ProjectileBan BannedProjectile { get; set; }

		/// <summary>
		/// <see cref="PermissionHookResult"/> of the hook.
		/// </summary>
		public PermissionHookResult Result { get; set; }

		/// <summary>
		/// Initializes a new instance of the PlayerProjbanPermissionEventArgs class.
		/// </summary>
		/// <param name="player">The player who fired the event.</param>
		/// <param name="checkedProjectile">The banned projectile being checked.</param>
		public PlayerProjbanPermissionEventArgs(TSPlayer player, ProjectileBan checkedProjectile)
		{
			Player = player;
			BannedProjectile = checkedProjectile;
			Result = PermissionHookResult.Unhandled;
		}
	}

	/// <summary>
	/// EventArgs used for the <see cref="PlayerHooks.PlayerTilebanPermission"/> event.
	/// </summary>
	public class PlayerTilebanPermissionEventArgs
	{
		/// <summary>
		/// The player who fired the event.
		/// </summary>
		public TSPlayer Player { get; set; }

		/// <summary>
		/// The banned tile being checked.
		/// </summary>
		public TileBan BannedTile { get; set; }

		/// <summary>
		/// <see cref="PermissionHookResult"/> of the hook.
		/// </summary>
		public PermissionHookResult Result { get; set; }

		/// <summary>
		/// Initializes a new instance of the PlayerTilebanPermissionEventArgs class.
		/// </summary>
		/// <param name="player">The player who fired the event.</param>
		/// <param name="checkedTile">The banned tile being checked.</param>
		public PlayerTilebanPermissionEventArgs(TSPlayer player, TileBan checkedTile)
		{
			Player = player;
			BannedTile = checkedTile;
			Result = PermissionHookResult.Unhandled;
		}
	}

	/// <summary>
	/// A collection of events fired by players that can be hooked to.
	/// </summary>
	public static class PlayerHooks
	{
		/// <summary>
		/// The delegate of the <see cref="PlayerPostLogin"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerPostLoginD(PlayerPostLoginEventArgs e);
		/// <summary>
		/// Fired by players after they've successfully logged in to a user account.
		/// </summary>
		public static event PlayerPostLoginD PlayerPostLogin;

		/// <summary>
		/// The delegate of the <see cref="PlayerPreLogin"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerPreLoginD(PlayerPreLoginEventArgs e);
		/// <summary>
		/// Fired by players when sending login credentials to the server.
		/// </summary>
		public static event PlayerPreLoginD PlayerPreLogin;

		/// <summary>
		/// The delegate of the <see cref="PlayerLogout"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerLogoutD(PlayerLogoutEventArgs e);
		/// <summary>
		/// Fired by players upon logging out from a user account.
		/// </summary>
		public static event PlayerLogoutD PlayerLogout;

		/// <summary>
		/// The delegate of the <see cref="PlayerCommand"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerCommandD(PlayerCommandEventArgs e);
		/// <summary>
		/// Fired by players when using a command.
		/// </summary>
		public static event PlayerCommandD PlayerCommand;

		/// <summary>
		/// The delegate of the <see cref="PlayerChat"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerChatD(PlayerChatEventArgs e);
		/// <summary>
		/// Fired by players when they send a chat message packet to the server
		/// and before it is transmitted to the rest of the players.
		/// </summary>
		public static event PlayerChatD PlayerChat;

		/// <summary>
		/// The delegate of the <see cref="PlayerPermission"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerPermissionD(PlayerPermissionEventArgs e);
		/// <summary>
		/// Fired by players every time a permission check involving them occurs.
		/// </summary>
		public static event PlayerPermissionD PlayerPermission;

		/// <summary>
		/// The delegate of the <see cref="PlayerItembanPermission"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerItembanPermissionD(PlayerItembanPermissionEventArgs e);
		/// <summary>
		/// Fired by players every time a permission check on banned items involving them occurs.
		/// </summary>
		public static event PlayerItembanPermissionD PlayerItembanPermission;

		/// <summary>
		/// The delegate of the <see cref="PlayerProjbanPermission"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerProjbanPermissionD(PlayerProjbanPermissionEventArgs e);
		/// <summary>
		/// Fired by players every time a permission check on banned projectiles involving them occurs.
		/// </summary>
		public static event PlayerProjbanPermissionD PlayerProjbanPermission;

		/// <summary>
		/// The delegate of the <see cref="PlayerTilebanPermission"/> event.
		/// </summary>
		/// <param name="e">The EventArgs for this event.</param>
		public delegate void PlayerTilebanPermissionD(PlayerTilebanPermissionEventArgs e);
		/// <summary>
		/// Fired by players every time a permission check on banned tiles involving them occurs.
		/// </summary>
		public static event PlayerTilebanPermissionD PlayerTilebanPermission;


		/// <summary>
		/// Fires the <see cref="PlayerPostLogin"/> event.
		/// </summary>
		/// <param name="ply">The player firing the event.</param>
		public static void OnPlayerPostLogin(TSPlayer ply)
		{
			if (PlayerPostLogin == null)
			{
					return;
			}

			PlayerPostLoginEventArgs args = new PlayerPostLoginEventArgs(ply);
			PlayerPostLogin(args);
		}

		/// <summary>
		/// Fires the <see cref="PlayerCommand"/> event.
		/// </summary>
		/// <param name="player">The player firing the event.</param>
		/// <param name="cmdName">The command name.</param>
		/// <param name="cmdText">The raw command text.</param>
		/// <param name="args">The command args extracted from the command text.</param>
		/// <param name="commands">The list of commands.</param>
		/// <param name="cmdPrefix">The command specifier used.</param>
		/// <returns>True if the event has been handled.</returns>
		public static bool OnPlayerCommand(TSPlayer player, string cmdName, string cmdText, List<string> args, ref IEnumerable<Command> commands, string cmdPrefix)
		{
			if (PlayerCommand == null)
			{
				return false;
			}
			PlayerCommandEventArgs playerCommandEventArgs = new PlayerCommandEventArgs()
			{
				Player = player,
				CommandName = cmdName,
				CommandText = cmdText,
				Parameters = args,
				CommandList = commands,
				CommandPrefix = cmdPrefix,
			};
			PlayerCommand(playerCommandEventArgs);
			return playerCommandEventArgs.Handled;
		}

		/// <summary>
		/// Fires the <see cref="PlayerPreLogin"/> event.
		/// </summary>
		/// <param name="ply">The player firing the event.</param>
		/// <param name="name">The user name.</param>
		/// <param name="pass">The password.</param>
		/// <returns>True if the event has been handled.</returns>
		public static bool OnPlayerPreLogin(TSPlayer ply, string name, string pass)
		{
			if (PlayerPreLogin == null)
				return false;

			var args = new PlayerPreLoginEventArgs {Player = ply, LoginName = name, Password = pass};
			PlayerPreLogin(args);
			return args.Handled;
		}

		/// <summary>
		/// Fires the <see cref="PlayerLogout"/> event.
		/// </summary>
		/// <param name="ply">The player firing the event.</param>
		public static void OnPlayerLogout(TSPlayer ply)
		{
			if (PlayerLogout == null)
				return;

			var args = new PlayerLogoutEventArgs(ply);
			PlayerLogout(args);
		}

		/// <summary>
		/// Fires the <see cref="PlayerChat"/> event.
		/// </summary>
		/// <param name="ply">The player firing the event.</param>
		/// <param name="rawtext">The raw chat text sent by the player.</param>
		/// <param name="tshockText">The chat text after being formatted.</param>
		public static void OnPlayerChat(TSPlayer ply, string rawtext, ref string tshockText)
		{
			if (PlayerChat == null)
				return;

			var args = new PlayerChatEventArgs {Player = ply, RawText = rawtext, TShockFormattedText = tshockText};
			PlayerChat(args);
			tshockText = args.TShockFormattedText;
		}

		/// <summary>
		/// Fires the <see cref="PlayerPermission"/> event.
		/// </summary>
		/// <param name="player">The player firing the event.</param>
		/// <returns>Event result if the event has been handled, otherwise <see cref="PermissionHookResult.Unhandled"/>.</returns>
		public static PermissionHookResult OnPlayerPermission(TSPlayer player, string permission)
		{
			if (PlayerPermission == null)
				return PermissionHookResult.Unhandled;

			var args = new PlayerPermissionEventArgs(player, permission);
			PlayerPermission(args);

			return args.Result;
		}

		/// <summary>
		/// Fires the <see cref="PlayerItembanPermission"/> event.
		/// </summary>
		/// <param name="player">The player firing the event.</param>
		/// <returns>Event result if the event has been handled, otherwise <see cref="PermissionHookResult.Unhandled"/>.</returns>
		public static PermissionHookResult OnPlayerItembanPermission(TSPlayer player, ItemBan bannedItem)
		{
			if (PlayerItembanPermission == null)
				return PermissionHookResult.Unhandled;

			var args = new PlayerItembanPermissionEventArgs(player, bannedItem);
			PlayerItembanPermission(args);

			return args.Result;
		}

		/// <summary>
		/// Fires the <see cref="PlayerProjbanPermission"/> event.
		/// </summary>
		/// <param name="player">The player firing the event.</param>
		/// <returns>Event result if the event has been handled, otherwise <see cref="PermissionHookResult.Unhandled"/>.</returns>
		public static PermissionHookResult OnPlayerProjbanPermission(TSPlayer player, ProjectileBan bannedProj)
		{
			if (PlayerProjbanPermission == null)
				return PermissionHookResult.Unhandled;

			var args = new PlayerProjbanPermissionEventArgs(player, bannedProj);
			PlayerProjbanPermission(args);

			return args.Result;
		}

		/// <summary>
		/// Fires the <see cref="PlayerTilebanPermission"/> event.
		/// </summary>
		/// <param name="player">The player firing the event.</param>
		/// <returns>Event result if the event has been handled, otherwise <see cref="PermissionHookResult.Unhandled"/>.</returns>
		public static PermissionHookResult OnPlayerTilebanPermission(TSPlayer player, TileBan bannedTile)
		{
			if (PlayerTilebanPermission == null)
				return PermissionHookResult.Unhandled;

			var args = new PlayerTilebanPermissionEventArgs(player, bannedTile);
			PlayerTilebanPermission(args);

			return args.Result;
		}

	}

	/// <summary>
	/// Defines the possible outcomes of <see cref="PlayerHooks.PlayerPermission"/> handlers.
	/// </summary>
	public enum PermissionHookResult
	{
		/// <summary>Hook doesn't return a result on the permission check.</summary>
		Unhandled,
		/// <summary>Permission is explicitly denied by a hook.</summary>
		Denied,
		/// <summary>Permission is explicitly granted by a hook.</summary>
		Granted
	}

}
