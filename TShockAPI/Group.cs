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

        public Group(string groupname, Group parentgroup = null)
        {
            Name = groupname;
            Parent = parentgroup;
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
            return false;
        }

        public void NegatePermission(string permission)
        {
            negatedpermissions.Add(permission);
        }

        public void AddPermission(string permission)
        {
            recursePermissions(permission, new List<String>());
        }

        public void recursePermissions( string permission, List<String> used){
            if (used.Contains(permission))
            {
                return;
            }
            else if( TShock.Groups.GroupExists( permission ) )
            {
                used.Add(permission);
                Group g = Tools.GetGroup(permission);
                foreach (string perm in g.permissions)
                {
                    recursePermissions(perm, used);
                }
            }
            else
            {
                permissions.Add(permission);
            }
        }
    }

    public class SuperAdminGroup : Group
    {
        public SuperAdminGroup()
            : base("superadmin")
        {
        }

        public override bool HasPermission(string permission)
        {
            return true;
        }
    }
}