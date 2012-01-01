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