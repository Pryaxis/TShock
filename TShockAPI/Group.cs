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
        private string name;
        private Group parent;
        private List<string> permissions = new List<string>();
        private List<string> negatedPermissions = new List<string>();

        public Group(string groupName, Group parentGroup = null)
        {
            name = groupName;
            parent = parentGroup;
        }

        public Group()
        {
            throw new Exception("Called Group constructor with no parameters");
        }

        public string GetName()
        {
            return name;
        }

        public Group GetParent()
        {
            return parent;
        }

        public virtual bool HasPermission(string permission)
        {
            if (permission.Equals(""))
            {
                return true;
            }
            if (negatedPermissions.Contains(permission))
            {
                return false;
            }
            else if (permissions.Contains(permission))
            {
                return true;
            }
            else if (parent != null)
            {
                //inception
                return parent.HasPermission(permission);
            }
            return false;
        }

        public void NegatePermission(string permission)
        {
            negatedPermissions.Add(permission);
        }

        public void AddPermission(string permission)
        {
            permissions.Add(permission);
        }
    }

    public class SuperAdminGroup : Group
    {
        public SuperAdminGroup(string groupName, Group parentGroup = null) : base(groupName, parentGroup)
        {
        }

        public SuperAdminGroup()
        {
        }

        public override bool HasPermission(string permission)
        {
            return true;
        }
    }
}