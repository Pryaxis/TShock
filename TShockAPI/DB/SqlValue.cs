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

using System.Collections.Generic;
using System.Data;

namespace TShockAPI.DB
{
	/// <summary>
	/// Represents an SQL name-value pair.
	/// </summary>
	public class SqlValue
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public object Value { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlValue"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		public SqlValue(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}

	/// <summary>
	/// Sql table editor.
	/// </summary>
	public class SqlTableEditor
	{
		/// <summary>
		/// A database connection.
		/// </summary>
		private IDbConnection _database;

		/// <summary>
		/// A query builder.
		/// </summary>
		private IQueryBuilder _creator;

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlTableEditor"/> class.
		/// </summary>
		/// <param name="db">Database connection.</param>
		/// <param name="provider">Query builder.</param>
		public SqlTableEditor(IDbConnection db, IQueryBuilder provider)
		{
			_database = db;
			_creator = provider;
		}

		/// <summary>
		/// Updates values.
		/// </summary>
		/// <param name="table">Table to update.</param>
		/// <param name="values">List of name-value pairs.</param>
		/// <param name="wheres">A list of where conditions.</param>
		public void UpdateValues(string table, List<SqlValue> values, List<SqlValue> wheres)
		{
			_database.Query(_creator.UpdateValue(table, values, wheres));
		}

		/// <summary>
		/// Inserts values.
		/// </summary>
		/// <param name="table">Table to insert into.</param>
		/// <param name="values">List of name-value pairs.</param>
		public void InsertValues(string table, List<SqlValue> values)
		{
			_database.Query(_creator.InsertValues(table, values));
		}

		/// <summary>
		/// Retrieve the specified column rows.
		/// </summary>
		/// <returns>A list of results.</returns>
		/// <param name="table">Table to query.</param>
		/// <param name="column">Name of column.</param>
		/// <param name="wheres">A list of where conditions.</param>
		public List<object> ReadColumn(string table, string column, List<SqlValue> wheres)
		{
			List<object> values = new List<object>();

			using (var reader = _database.QueryReader(_creator.ReadColumn(table, wheres)))
			{
				while (reader.Read())
					values.Add(reader.Reader.Get<object>(column));
			}

			return values;
		}
	}
}