/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using TShockAPI.PermissionSystem;

namespace TShockAPI
{
	public class User
	{
		/// <summary>
		/// An unique identifier across the same database.
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// The username entered when creating a new user.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The user's password.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// The UUID of the last player to log in as this user.
		/// </summary>
		public string UUID { get; set; }

		/// <summary>
		/// The user's group.
		/// </summary>
		public Group Group { get; set; }

		/// <summary>
		/// The date of registration.
		/// </summary>
		public DateTime Registered { get; set; }

		/// <summary>
		/// The date of the last time this user has logged in.
		/// </summary>
		public DateTime LastAccessed { get; set; }

		/// <summary>
		/// A list of all IPv4 addresses this user has connected from.
		/// </summary>
		public List<string> KnownIps { get; set; }

		/// <summary>
		/// The permissions of the user in string form.
		/// </summary>
		public string Permissions
		{
			get { return PermissionManager.ToString(); }
			set { PermissionManager.Parse(value); }
		}

		/// <summary>
		/// The manager of this user's permissions
		/// </summary>
		public IPermissionManager PermissionManager { get; private set; }

		/// <summary>
		/// The default constructor.
		/// </summary>
		public User()
		{
			List<string> KnownIps = new List<string>();
			PermissionManager = new NegatedPermissionManager();
		}

		/// <summary>
		/// The complete constructor, parses values from strings.
		/// </summary>
		/// <param name="name">The user's name.</param>
		/// <param name="pass">The user's password.</param>
		/// <param name="uuid">The UUID of the last player who's logged in as this user.</param>
		/// <param name="group">The user group's name.</param>
		/// <param name="registered">The date of registration in sortable ("s") format.</param>
		/// <param name="last">The date of last access in sortable ("s") format.</param>
		/// <param name="known">A JSON-encoded list of strings for all IPv4 Addresses this user has connected from.</param>
		/// <param name="permissions">A CSV string containing a list of permissions.</param>
		public User(string name, string pass, string uuid, string group, string registered, string last, string known, string permissions)
			: this()
		{
			Name = name;
			Password = pass;
			UUID = uuid;
			Group = TShock.Groups.GetGroupByName(group) ?? Group.DefaultGroup;
			Registered = DateTime.Parse(registered);
			LastAccessed = DateTime.Parse(last);
			KnownIps = JsonConvert.DeserializeObject<List<string>>(known);
			PermissionManager = new NegatedPermissionManager(permissions);
		}

		/// <summary>
		/// Adds a permission to the list of permissions.
		/// </summary>
		/// <param name="permission">The permission to add.</param>
		public void AddPermission(string permission)
		{
			PermissionManager.AddPermission(permission);
		}

		/// <summary>
		/// Determines whether an user has a specified permission.
		/// </summary>
		/// <param name="permission">The permission to check.</param>
		/// <returns>Returns true if the user has that permission.</returns>
		public bool HasPermission(string permission)
		{
			return PermissionManager.HasPermission(new PermissionNode(permission));
		}

		/// <summary>
		/// Removes a permission from the list of permissions.
		/// </summary>
		/// <param name="permission">The permission to remove.</param>
		public void RemovePermission(string permission)
		{
			PermissionManager.RemovePermission(permission);
		}

		/// <summary>
		/// Clears the permission list and sets it to the list provided.
		/// </summary>
		/// <param name="permissions">The permission list to set.</param>
		public void SetPermissions(List<string> permissions)
		{
			PermissionManager.Parse(permissions);
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
