/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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