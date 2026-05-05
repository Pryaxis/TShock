/*
TShock, a server mod for Terraria
Copyright (C) 2011-2025 Pryaxis & TShock Contributors

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
using System.Linq;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB.Queries;

/// <summary>
/// Query Creator for Sqlite
/// </summary>
public class SqliteQueryBuilder : GenericQueryBuilder, IQueryBuilder
{
	/// <summary>
	/// Creates a table from a SqlTable object.
	/// </summary>
	/// <param name="table">The SqlTable to create the table from</param>
	/// <returns>The sql query for the table creation.</returns>
	public override string CreateTable(SqlTable table)
	{
		ValidateSqlColumnType(table.Columns);
		var columns =
			table.Columns.Select(
				c =>
					"'{0}' {1} {2} {3} {4} {5}".SFormat(c.Name,
						DbTypeToString(c.Type, c.Length),
						c.Primary ? "PRIMARY KEY" : "",
						c.AutoIncrement ? "AUTOINCREMENT" : "",
						c.NotNull ? "NOT NULL" : "",
						c.DefaultCurrentTimestamp ? "DEFAULT CURRENT_TIMESTAMP" : ""));
		var uniques = table.Columns.Where(c => c.Unique).Select(c => c.Name);
		return "CREATE TABLE {0} ({1} {2})".SFormat(EscapeTableName(table.Name),
			string.Join(", ", columns),
			uniques.Count() > 0 ? ", UNIQUE({0})".SFormat(string.Join(", ", uniques)) : "");
	}

	/// <summary>
	/// Renames the given table.
	/// </summary>
	/// <param name="from">Old name of the table</param>
	/// <param name="to">New name of the table</param>
	/// <returns>The sql query for renaming the table.</returns>
	public override string RenameTable(string from, string to)
	{
		return "ALTER TABLE {0} RENAME TO {1}".SFormat(from, to);
	}

	private static readonly Dictionary<MySqlDbType, string> TypesAsStrings = new Dictionary<MySqlDbType, string>
	{
		{ MySqlDbType.VarChar, "TEXT" },
		{ MySqlDbType.String, "TEXT" },
		{ MySqlDbType.Text, "TEXT" },
		{ MySqlDbType.TinyText, "TEXT" },
		{ MySqlDbType.MediumText, "TEXT" },
		{ MySqlDbType.LongText, "TEXT" },
		{ MySqlDbType.Float, "REAL" },
		{ MySqlDbType.Double, "REAL" },
		{ MySqlDbType.Int32, "INTEGER" },
		{ MySqlDbType.Blob, "BLOB" },
		{ MySqlDbType.Int64, "BIGINT"},
		{ MySqlDbType.DateTime, "DATETIME"},
	};

	/// <summary>
	/// Converts the MySqlDbType enum to it's string representation.
	/// </summary>
	/// <param name="type">The MySqlDbType type</param>
	/// <param name="length">The length of the datatype</param>
	/// <returns>The string representation</returns>
	public override string DbTypeToString(MySqlDbType type, int? length)
	{
		if (TypesAsStrings.TryGetValue(type, out string ret))
		{
			return ret;
		}

		throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
	}

	/// <inheritdoc />
	protected override string EscapeColumnName(string column) => $"\'{column}\'";

	/// <summary>
	/// Escapes the table name
	/// </summary>
	/// <param name="table">The name of the table to be escaped</param>
	/// <returns></returns>
	protected override string EscapeTableName(string table) => $"\'{table}\'";
}
