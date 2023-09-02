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

using System.ComponentModel;
using TShockAPI.DB;
namespace TShockAPI.Hooks
{
	public class AccountDeleteEventArgs
	{
		public UserAccount Account { get; private set; }

		public AccountDeleteEventArgs(UserAccount account)
		{
			this.Account = account;
		}
	}

	public class AccountCreateEventArgs
	{
		public UserAccount Account { get; private set; }

		public AccountCreateEventArgs(UserAccount account)
		{
			this.Account = account;
		}
	}

	public class AccountGroupUpdateEventArgs : HandledEventArgs
	{
		public string AccountName { get; private set; }
		public Group Group { get; set; }

		public AccountGroupUpdateEventArgs(string accountName, Group group)
		{
			this.AccountName = accountName;
			this.Group = group;
		}
	}

	public class AccountGroupUpdateByPlayerEventArgs : AccountGroupUpdateEventArgs
	{
		/// <summary>
		/// The player who updated the user's group
		/// </summary>
		public TSPlayer Player { get; private set; }

		public AccountGroupUpdateByPlayerEventArgs(TSPlayer player, string accountName, Group group) : base(accountName, group)
		{
			this.Player = player;
		}
	}

	public class AccountHooks
	{
		public delegate void AccountCreateD(AccountCreateEventArgs e);
		public static event AccountCreateD AccountCreate;

		public static void OnAccountCreate(UserAccount u)
		{
			if (AccountCreate == null)
				return;

			AccountCreate(new AccountCreateEventArgs(u));
		}

		public delegate void AccountDeleteD(AccountDeleteEventArgs e);
		public static event AccountDeleteD AccountDelete;

		public static void OnAccountDelete(UserAccount u)
		{
			if (AccountDelete == null)
				return;

			AccountDelete(new AccountDeleteEventArgs(u));
		}

		public delegate void AccountGroupUpdateD(AccountGroupUpdateEventArgs e);
		public static event AccountGroupUpdateD AccountGroupUpdate;

		public static bool OnAccountGroupUpdate(UserAccount account, TSPlayer author, ref Group group)
		{
			AccountGroupUpdateEventArgs args = new AccountGroupUpdateByPlayerEventArgs(author, account.Name, group);
			AccountGroupUpdate?.Invoke(args);
			group = args.Group;

			return args.Handled;
		}
		public static bool OnAccountGroupUpdate(UserAccount account, ref Group group)
		{
			AccountGroupUpdateEventArgs args = new AccountGroupUpdateEventArgs(account.Name, group);
			AccountGroupUpdate?.Invoke(args);
			group = args.Group;

			return args.Handled;
		}
	}
}
