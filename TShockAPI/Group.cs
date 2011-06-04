using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI
{
    public class Group
    {
        private string name;
        private Group parent = null;

        public Group(string groupName, Group parentGroup = null)
        {
            name = groupName;
            parent = parentGroup;
        }

        public string GetName()
        {
            return name;
        }

        public Group GetParent()
        {
            return parent;
        }

        public bool HasPermission(string permission)
        {
            //TODO: implement this
            return true;
        }
    }
}
