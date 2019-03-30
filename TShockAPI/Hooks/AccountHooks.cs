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
	}
}
