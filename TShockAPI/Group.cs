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

using System.Collections.Generic;
using TShockAPI.PermissionSystem;

namespace TShockAPI
{
	/// <summary>
	/// Representation of a user group.
	/// </summary>
	public class Group
	{
		/// <summary>
		/// Manager of this groups permissions.
		/// </summary>
		public IPermissionManager PermissionManager { get; private set; }

		/// <summary>
		/// The group's name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The chat prefix for this group.
		/// </summary>
		public string Prefix { get; set; }

		/// <summary>
		/// The chat suffix for this group.
		/// </summary>
		public string Suffix { get; set; }

		// NOTE: Using a const still suffers from needing to recompile to change the default
		// ideally we would use a static but this means it can't be used for the default parameter :(
		/// <summary>
		/// Default chat color.
		/// </summary>
		public const string defaultChatColor = "255,255,255";

		/// <summary>
		/// The Red color portion for Chat Color
		/// </summary>
		public byte R = 255;

		/// <summary>
		/// The Green color portion for Chat Color
		/// </summary>
		public byte G = 255;

		/// <summary>
		/// The Blue color portion for Chat Color
		/// </summary>
		public byte B = 255;

		/// <summary>
		/// The chat color of the group.
		/// Returns "255,255,255", sets "255,255,255"
		/// </summary>
		public string ChatColor
		{
			get { return string.Format("{0},{1},{2}", R.ToString("D3"), G.ToString("D3"), B.ToString("D3")); }
			set
			{
				if (null != value)
				{
					string[] parts = value.Split(',');
					if (3 == parts.Length)
					{
						byte r, g, b;
						if (byte.TryParse(parts[0], out r) && byte.TryParse(parts[1], out g) && byte.TryParse(parts[2], out b))
						{
							R = r;
							G = g;
							B = b;
						}
					}
				}
			}
		}

		/// <summary>
		/// The permissions of the user in string form.
		/// </summary>
		public string Permissions
		{
			get { return PermissionManager.ToString(); }
			set { PermissionManager.Parse(value); }
		}

		/// <summary>
		/// The parent group for this parent.
		/// </summary>
		public Group Parent { get; set; } 

		/// <summary>
		/// The name of the parent group, or "" if there is none.
		/// </summary>
		public string ParentName {get { return Parent == null ? "" : Parent.Name; }}

		/// <summary>
		/// The default group a new group should be dumped into.
		/// </summary>
		public static Group DefaultGroup = null;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="groupname">The name of this group.</param>
		/// <param name="parentgroup">The parent group.</param>
		/// <param name="chatcolor">The color of this groups chat.</param>
		/// <param name="permissions">The permission list this group has.</param>
		public Group(string groupname, Group parentgroup = null, string chatcolor = "255,255,255", string permissions = null)
		{
			Name = groupname;

			Parent = parentgroup;

			ChatColor = chatcolor;
			PermissionManager = new NegatedPermissionManager(permissions);
		}

		/// <summary>
		/// Checks to see if a group has a specified permission.
		/// </summary>
		/// <param name="permission">The permission to check.</param>
		/// <returns>Returns true if the user has that permission.</returns>
		public virtual bool HasPermission(string permission)
		{
			return PermissionManager.HasPermission(new PermissionNode(permission));
		}

		/// <summary>
		/// Adds a permission to the list of permissions.
		/// </summary>
		/// <param name="permission">The permission to add.</param>
		public void AddPermission(string permission)
		{
			PermissionManager.AddPermission(new PermissionNode(permission, PermissionType.Negated));
		}

		/// <summary>
		/// Clears the permission list and sets it to the list provided, 
		/// will parse "!permssion" and add it to the negated permissions.
		/// </summary>
		/// <param name="permission"></param>
		public void SetPermission(List<string> permission)
		{
			PermissionManager.Parse(permission);
		}

		/// <summary>
		/// Will remove a permission from the respective list,
		/// where "!permission" will remove a negated permission.
		/// </summary>
		/// <param name="permission"></param>
		public void RemovePermission(string permission)
		{
			PermissionManager.RemovePermission(permission);
		}

		/// <summary>
		/// Assigns all fields of this instance to another.
		/// </summary>
		/// <param name="otherGroup">The other instance.</param>
		public void AssignTo(Group otherGroup)
		{
			otherGroup.Name = Name;
			otherGroup.Parent = Parent;
			otherGroup.Prefix = Prefix;
			otherGroup.Suffix = Suffix;
			otherGroup.R = R;
			otherGroup.G = G;
			otherGroup.B = B;
			PermissionManager.Clone(otherGroup.PermissionManager);
		}

		public override string ToString() {
			return Name;
		}
	}

	/// <summary>
	/// This class is the SuperAdminGroup, which has access to everything.
	/// </summary>
	public class SuperAdminGroup : Group
	{
		/// <summary>
		/// Superadmin group shim.
		/// </summary>
		public SuperAdminGroup()
			: base("superadmin")
		{
			R = (byte) TShock.Config.SuperAdminChatRGB[0];
			G = (byte) TShock.Config.SuperAdminChatRGB[1];
			B = (byte) TShock.Config.SuperAdminChatRGB[2];
			Prefix = TShock.Config.SuperAdminChatPrefix;
			Suffix = TShock.Config.SuperAdminChatSuffix;
			PermissionManager.AddPermission(new PermissionNode("*"));
		}
	}
}
