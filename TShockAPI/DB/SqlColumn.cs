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

using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
	/// <summary>
	/// Sql column.
	/// </summary>
	public class SqlColumn
	{
		/// <summary>
		/// Gets or sets the column name.
		/// </summary>
		/// <value>The column name.</value>
		public string Name { get; set; }
		
		/// <summary>
		/// Gets or sets the column data type.
		/// </summary>
		/// <value>The column data type.</value>
		public MySqlDbType Type { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TShockAPI.DB.SqlColumn"/> is unique.
		/// </summary>
		/// <value><c>true</c> if unique; otherwise, <c>false</c>.</value>
		public bool Unique { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TShockAPI.DB.SqlColumn"/> is a primary key.
		/// </summary>
		/// <value><c>true</c> if primary; otherwise, <c>false</c>.</value>
		public bool Primary { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TShockAPI.DB.SqlColumn"/> will auto increment.
		/// </summary>
		/// <value><c>true</c> if auto increment; otherwise, <c>false</c>.</value>
		public bool AutoIncrement { get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TShockAPI.DB.SqlColumn"/> prohibits null value.
		/// </summary>
		/// <value><c>true</c> if not null; otherwise, <c>false</c>.</value>
		public bool NotNull { get; set; }
		
		/// <summary>
		/// Gets or sets the default value.
		/// </summary>
		/// <value>The default value.</value>
		public string DefaultValue { get; set; }

		/// <summary>
		/// Length of the data type, null = default
		/// </summary>
		public int? Length { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlColumn"/> class.
		/// </summary>
		/// <param name="name">Name of column.</param>
		/// <param name="type">Data type of column.</param>
		public SqlColumn(string name, MySqlDbType type)
			: this(name, type, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlColumn"/> class.
		/// </summary>
		/// <param name="name">Name of column.</param>
		/// <param name="type">Data type of column.</param>
		/// <param name="length">Length of data type.</param>
		public SqlColumn(string name, MySqlDbType type, int? length = null)
		{
			Name = name;
			Type = type;
			Length = length;
		}
	}
}