using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.DB
{
    public class SqlColumn
    {
        //Required
        public string Name { get; set; }
        public string Type { get; set; }

        //Optional
        public bool Unique { get; set; }
        public bool Primary { get; set; }
        public bool AutoIncrement { get; set; }
        public bool NotNull { get; set; }
        public string DefaultValue { get; set; }

        public SqlColumn(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}
