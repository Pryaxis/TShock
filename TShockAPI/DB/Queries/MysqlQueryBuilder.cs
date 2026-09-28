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
/// Query Creator for MySQL
/// </summary>
public class MysqlQueryBuilder : GenericQueryBuilder, IQueryBuilder
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
					"`{0}` {1} {2} {3} {4} {5}".SFormat(c.Name, DbTypeToString(c.Type, c.Length),
						c.Primary ? "PRIMARY KEY" : "",
						c.AutoIncrement ? "AUTO_INCREMENT" : "",
						c.NotNull ? "NOT NULL" : "",
						c.DefaultCurrentTimestamp ? "DEFAULT CURRENT_TIMESTAMP" : ""));

		var uniques = table.Columns.Where(c => c.Unique).Select(c => $"`{c.Name}`");
		return "CREATE TABLE {0} ({1} {2})".SFormat(EscapeTableName(table.Name), string.Join(", ", columns),
			uniques.Any()
				? ", UNIQUE({0})".SFormat(string.Join(", ", uniques))
				: "");
	}


	/// <inheritdoc />
	public override string RenameTable(string from, string to) => /*lang=mysql*/"RENAME TABLE {0} TO {1}".SFormat(from, to);

	private static readonly Dictionary<MySqlDbType, string> TypesAsStrings = new()
	{
		{ MySqlDbType.VarChar, "VARCHAR" },
		{ MySqlDbType.String, "CHAR" },
		{ MySqlDbType.Text, "TEXT" },
		{ MySqlDbType.TinyText, "TINYTEXT" },
		{ MySqlDbType.MediumText, "MEDIUMTEXT" },
		{ MySqlDbType.LongText, "LONGTEXT" },
		{ MySqlDbType.Float, "FLOAT" },
		{ MySqlDbType.Double, "DOUBLE" },
		{ MySqlDbType.Int32, "INT" },
		{ MySqlDbType.Int64, "BIGINT"},
		{ MySqlDbType.DateTime, "DATETIME"},
	};

	/// <inheritdoc />
	public override string DbTypeToString(MySqlDbType type, int? length)
	{
		if (TypesAsStrings.TryGetValue(type, out string ret))
		{
			return ret + (length is not null ? "({0})".SFormat((int)length) : "");
		}

		throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
	}

	/// <inheritdoc />
	protected override string EscapeColumnName(string column) => $"`{column}`";

	/// <inheritdoc />
	protected override string EscapeTableName(string table) => table.SFormat("`{0}`", table);
}
