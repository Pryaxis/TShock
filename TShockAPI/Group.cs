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