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
using System.Linq;
using System.Collections.Generic;
using TShockAPI.PermissionSystem;

namespace TShockAPI
{
	public class Group
	{
		// NOTE: Using a const still suffers from needing to recompile to change the default
		// ideally we would use a static but this means it can't be used for the default parameter :(
        /// <summary>
        /// Default chat color.
        /// </summary>
		public const string defaultChatColor = "255,255,255";

		public IPermissionManager permissionManager;

        /// <summary>
        /// The group's name.
        /// </summary>
		public string Name { get; set; }

        /// <summary>
        /// The group that this group inherits permissions from.
        /// </summary>
		public Group Parent { get; set; }

        /// <summary>
        /// The chat prefix for this group.
        /// </summary>
		public string Prefix { get; set; }

        /// <summary>
        /// The chat suffix for this group.
        /// </summary>
		public string Suffix { get; set; }

        /// <summary>
        /// The name of the parent, not particularly sure why this is here.
        /// We can use group.Parent.Name and not have this second reference. 
        /// This was added for rest, so a discussion with Shank is necessary.
        /// </summary>
		public string ParentName { get { return (null == Parent) ? "" : Parent.Name; } }

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
							return;
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
	        get { return permissionManager.ToString(); }
			set { permissionManager.Parse(value); }
		}

        /// <summary>
        /// The permissions of this group and all that it inherits from.
        /// </summary>
		public virtual List<string> TotalPermissions
		{
			get
			{
				var cur = this;
				var traversed = new List<Group>();
				HashSet<string> all = new HashSet<string>();
				while (cur != null)
				{
					cur.permissionManager.TotalPermissions().GetPermissions().All(p => all.Add(p));

					if (traversed.Contains(cur))
					{
						throw new Exception("Infinite group parenting ({0})".SFormat(cur.Name));
					}
					traversed.Add(cur);
					cur = cur.Parent;
				}
				return all.ToList();
			}
		}

		public byte R = 255;
		public byte G = 255;
		public byte B = 255;

	    public static Group DefaultGroup = null;

		public Group(string groupname, Group parentgroup = null, string chatcolor = "255,255,255", string permissions = null)
		{
			Name = groupname;
			Parent = parentgroup;
			ChatColor = chatcolor;
			permissionManager = new PermissionManager(permissions);
		}

        /// <summary>
        /// Checks to see if a group has a specified permission.
        /// </summary>
        /// <param name="permission">The permission to check.</param>
        /// <returns>Returns true if the user has that permission.</returns>
		public virtual bool HasPermission(string permission)
        {
	        return permissionManager.HasPermission(permission);
        }

        /// <summary>
        /// Adds a permission to the list of permissions.
        /// </summary>
        /// <param name="permission">The permission to add.</param>
		public void AddPermission(string permission)
		{
			permissionManager.AddPermission(permission);
		}

        /// <summary>
        /// Clears the permission list and sets it to the list provided, 
        /// will parse "!permssion" and add it to the negated permissions.
        /// </summary>
        /// <param name="permission"></param>
		public void SetPermission(List<string> permission)
        {
			permissionManager.Parse(permission);
        }

        /// <summary>
        /// Will remove a permission from the respective list,
        /// where "!permission" will remove a negated permission.
        /// </summary>
        /// <param name="permission"></param>
		public void RemovePermission(string permission)
		{
			permissionManager.RemovePermission(permission);
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
			permissionManager.Clone(otherGroup.permissionManager);
		}

		public override string ToString() {
			return this.Name;
		}
	}

    /// <summary>
    /// This class is the SuperAdminGroup, which has access to everything.
    /// </summary>
	public class SuperAdminGroup : Group
	{
		public override List<string> TotalPermissions
		{
			get { return new List<string> { "*" }; }
		}
		public SuperAdminGroup()
			: base("superadmin")
		{
			R = (byte) TShock.Config.SuperAdminChatRGB[0];
			G = (byte) TShock.Config.SuperAdminChatRGB[1];
			B = (byte) TShock.Config.SuperAdminChatRGB[2];
			Prefix = TShock.Config.SuperAdminChatPrefix;
			Suffix = TShock.Config.SuperAdminChatSuffix;
		}

        /// <summary>
        /// Override to allow access to everything.
        /// </summary>
        /// <param name="permission">The permission</param>
        /// <returns>True</returns>
		public override bool HasPermission(string permission)
		{
			return true;
		}
	}
}
