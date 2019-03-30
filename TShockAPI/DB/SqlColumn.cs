/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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
using System;

namespace TShockAPI.DB
{
	public class SqlColumn
	{
		//Required
		public string Name { get; set; }
		public MySqlDbType Type { get; set; }


		//Optional
		/// <summary>
		/// Sets/Gets if it's unique 
		/// </summary>
		public bool Unique { get; set; }
		/// <summary>
		/// Sets/Gets if it's primary key
		/// </summary>
		public bool Primary { get; set; }
		/// <summary>
		/// Sets/Gets if it autoincrements
		/// </summary>
		public bool AutoIncrement { get; set; }
		/// <summary>
		/// Sets/Gets if it can be or not null
		/// </summary>
		public bool NotNull { get; set; }
		/// <summary>
		/// Sets the default value
		/// </summary>
		public string DefaultValue { get; set; }
		/// <summary>
		/// Use on DateTime only, if true, sets the default value to the current date when creating the row.
		/// </summary>
		public bool DefaultCurrentTimestamp { get; set; }

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

	/// <summary>
	/// Used when a SqlColumn has validation errors.
	/// </summary>
	[Serializable]
	public class SqlColumnException : Exception
	{
		/// <summary>
		/// Creates a new SqlColumnException with the given message.
		/// </summary>
		/// <param name="message"></param>
		public SqlColumnException(string message) : base(message)
		{
		}
	}
}
