using System;
using System.Collections.Generic;

namespace TShockDBEditor
{
    public class Group
    {
        public readonly List<string> permissions = new List<string>();
        private readonly List<string> negatedpermissions = new List<string>();

        public int ID { get; protected set; }
        public string Name { get; set; }
        public Group Parent { get; set; }
        public int Order { get; set; }

        public Group(int id, string groupname, int order, Group parentgroup = null)
        {
            Order = order;
            ID = id;
            Name = groupname;
            Parent = parentgroup;
        }
    }
}