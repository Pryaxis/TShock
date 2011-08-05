using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
    public class SqlColumn
    {
        //Required
        public string Name { get; set; }
        public MySqlDbType Type { get; set; }


        //Optional
        public bool Unique { get; set; }
        public bool Primary { get; set; }
        public bool AutoIncrement { get; set; }
        public bool NotNull { get; set; }
        public string DefaultValue { get; set; }

        /// <summary>
        /// Length of the data type, null = default
        /// </summary>
        public int? Length { get; set; }

        public SqlColumn(string name, MySqlDbType type)
            : this(name, type, null)
        {
        }
        public SqlColumn(string name, MySqlDbType type, int? length)
        {
            Name = name;
            Type = type;
            Length = length;
        }
    }
}
