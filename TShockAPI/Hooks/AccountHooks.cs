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

using TShockAPI.DB;
using System.Linq;
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

	public class AccountGroupChangeEventArgs
	{
		public bool Handled = false;

		public UserAccount Author { get; private set; }
		public UserAccount Account { get; private set; }

		public Group OldGroup { get; private set; }
		public Group NewGroup { get; set; }

		public AccountGroupChangeEventArgs(UserAccount account, UserAccount author, Group oldGroup, Group newGroup)
		{
			this.Account = account;
			this.Author = author;
			this.OldGroup = oldGroup;
			this.NewGroup = newGroup;
		}

		public TSPlayer FindAuthor()
		{
			for (int i = 0; i < TShock.Players.Length; i++)
			{
				var plr = TShock.Players[i];
				if (plr.IsLoggedIn && plr.Account.ID == Author.ID)
					return plr;
			}
			return null;
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

		public delegate void AccountGroupChangeD(AccountGroupChangeEventArgs e);
		public static event AccountGroupChangeD AccountGroupChange;

		public static bool OnAccountGroupChange(UserAccount account, UserAccount author, Group oldGroup, ref Group newGroup)
		{
			AccountGroupChangeEventArgs args = new AccountGroupChangeEventArgs(account, author, oldGroup, newGroup);
			AccountGroupChange(args);
			newGroup = args.NewGroup;

			return args.Handled;
		}
	}
}
