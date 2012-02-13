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

		public byte R = 255;
		public byte G = 255;
		public byte B = 255;

		public Group(string groupname, Group parentgroup = null, string chatcolor = "255,255,255")
		{
			Name = groupname;
			Parent = parentgroup;
			byte.TryParse(chatcolor.Split(',')[0], out R);
			byte.TryParse(chatcolor.Split(',')[1], out G);
			byte.TryParse(chatcolor.Split(',')[2], out B);
		}

		public string ChatColor()
		{
			return string.Format("{0}{1}{2}", R.ToString("X2"), G.ToString("X2"), B.ToString("X2"));
		}

		public virtual bool HasPermission(string permission)
		{
			var cur = this;
			var traversed = new List<Group>();
			while (cur != null)
			{
				if (string.IsNullOrEmpty(permission))
					return true;
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
			negatedpermissions.Add(permission);
		}

		public void AddPermission(string permission)
		{
			permissions.Add(permission);
		}

		public void SetPermission(List<string> permission)
		{
			permissions.Clear();
			foreach (string s in permission)
			{
				permissions.Add(s);
			}
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