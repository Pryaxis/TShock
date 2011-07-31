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
        private readonly List<string> negatedpermissions = new List<string>();

        public string Name { get; protected set; }
        public Group Parent { get; protected set; }
        public int Order { get; set; }

        public byte R = 255;
        public byte G = 255;
        public byte B = 255;

        public Group(string groupname, Group parentgroup = null, string ChatColor = "255,255,255")
        {
            Name = groupname;
            Parent = parentgroup;
            byte.TryParse(ChatColor.Split(',')[0], out R);
            byte.TryParse(ChatColor.Split(',')[1], out G);
            byte.TryParse(ChatColor.Split(',')[2], out B);
        }

        public virtual bool HasPermission(string permission)
        {
            if (string.IsNullOrEmpty(permission))
                return true;
            if (negatedpermissions.Contains(permission))
                return false;
            if (permissions.Contains(permission))
                return true;
            if (Parent != null)
                return Parent.HasPermission(permission);
            bool ret = false;
            foreach( string g in permissions )
            {
                if (TShock.Groups.GroupExists(g))
                {
                    ret = checkGroupForPermission(g, permission);
                    if (ret)
                        return true;
                }
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

        private bool checkGroupForPermission(String g, String permission )
        {
            bool ret = false;
            if( !TShock.Groups.GroupExists( g  ) )
            {
                if( g.Equals(permission) )
                    return true;
            }
            else
            {
                Group group = Tools.GetGroup(g);
                foreach (String perm in group.permissions)
                {
                    if( perm.Equals( permission ) )
                    {
                        return true;
                    }
                    ret = checkGroupForPermission( perm, permission );

                    if( ret )
                        return ret;
                }
            }
            return ret;
        }
    }

    public class SuperAdminGroup : Group
    {
        public SuperAdminGroup()
            : base("superadmin")
        {
            R = (byte)TShock.Config.SuperAdminChatRGB[0];
            G = (byte)TShock.Config.SuperAdminChatRGB[1];
            B = (byte)TShock.Config.SuperAdminChatRGB[2];
        }

        public override bool HasPermission(string permission)
        {
            return true;
        }
    }
}