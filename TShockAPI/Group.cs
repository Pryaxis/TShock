/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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

namespace TShockAPI
{
	public class Group
	{
		public readonly List<string> permissions = new List<string>();
		public readonly List<string> negatedpermissions = new List<string>();

		public string Name { get; set; }
		public Group Parent { get; set; }
		public int Order { get; set; }
		public string Prefix { get; set; }
		public string Suffix { get; set; }
		public string ParentName { get { return (null == Parent) ? "" : Parent.Name; } }
		public string ChatColor
		{
			get { return string.Format("{0}{1}{2}", R.ToString("X2"), G.ToString("X2"), B.ToString("X2")); }
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

		public string Permissions
		{
			get
			{
				List<string> all = new List<string>(permissions);
				negatedpermissions.ForEach(p => all.Add("!" + p));
				return string.Join(",", all);
			}
			set
			{
				permissions.Clear();
				negatedpermissions.Clear();
				if (null != value)
					value.Split(',').ForEach(p => AddPermission(p.Trim()));
			}
		}

		public List<string> TotalPermissions
		{
			get
			{
				var cur = this;
				var traversed = new List<Group>();
				HashSet<string> all = new HashSet<string>();
				while (cur != null)
				{
					foreach (var perm in cur.permissions)
					{
						all.Add(perm);
					}

					foreach (var perm in cur.negatedpermissions)
					{
						all.Remove(perm);
					}

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

		public Group(string groupname, Group parentgroup = null, string chatcolor = "255,255,255", string permissions = null)
		{
			Name = groupname;
			Parent = parentgroup;
			ChatColor = chatcolor;
			Permissions = permissions;
		}

		public virtual bool HasPermission(string permission)
		{
			if (string.IsNullOrEmpty(permission))
				return true;

			var cur = this;
			var traversed = new List<Group>();
			while (cur != null)
			{
				if (cur.negatedpermissions.Contains(permission))
					return false;
				if (cur.permissions.Contains(permission))
					return true;
				if (traversed.Contains(cur))
				{
					throw new Exception("Infinite group parenting ({0})".SFormat(cur.Name));
				}
				traversed.Add(cur);
				cur = cur.Parent;
			}
			return false;
		}

		public void NegatePermission(string permission)
		{
			// Avoid duplicates
			if (!negatedpermissions.Contains(permission))
			{
				negatedpermissions.Add(permission);
				permissions.Remove(permission); // Ensure we don't have conflicting definitions for a permissions
			}
		}

		public void AddPermission(string permission)
		{
			if (permission.StartsWith("!"))
			{
				NegatePermission(permission.Substring(1));
				return;
			}
			// Avoid duplicates
			if (!permissions.Contains(permission))
			{
				permissions.Add(permission);
				negatedpermissions.Remove(permission); // Ensure we don't have conflicting definitions for a permissions
			}
		}

		public void SetPermission(List<string> permission)
		{
			permissions.Clear();
			negatedpermissions.Clear();
			permission.ForEach(p => AddPermission(p));
		}

		public void RemovePermission(string permission)
		{
			if (permission.StartsWith("!"))
			{
				negatedpermissions.Remove(permission.Substring(1));
				return;
			}
			permissions.Remove(permission);
		}
	}

	public class SuperAdminGroup : Group
	{
		public SuperAdminGroup()
			: base("superadmin")
		{
			R = (byte) TShock.Config.SuperAdminChatRGB[0];
			G = (byte) TShock.Config.SuperAdminChatRGB[1];
			B = (byte) TShock.Config.SuperAdminChatRGB[2];
			Prefix = TShock.Config.SuperAdminChatPrefix;
			Suffix = TShock.Config.SuperAdminChatSuffix;
		}

		public override bool HasPermission(string permission)
		{
			return true;
		}
	}
}