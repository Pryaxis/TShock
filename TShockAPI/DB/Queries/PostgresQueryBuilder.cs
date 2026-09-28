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
/// Query Creator for PostgreSQL
/// </summary>
public class PostgresQueryBuilder : GenericQueryBuilder
{
	/// <inheritdoc />
	public override string DbTypeToString(MySqlDbType type, int? length) => type switch
	{
		MySqlDbType.VarChar when length is not null => "VARCHAR({0})".SFormat(length),
		MySqlDbType.String when length is not null => "CHAR({0})".SFormat(length),
		MySqlDbType.Text => "TEXT",
		MySqlDbType.TinyText => "TEXT",
		MySqlDbType.MediumText => "TEXT",
		MySqlDbType.LongText => "TEXT",
		MySqlDbType.Float => "REAL",
		MySqlDbType.Double => "DOUBLE PRECISION",
		MySqlDbType.Int32 => "INT",
		MySqlDbType.Int64 => "BIGINT",
		MySqlDbType.DateTime => "TIMESTAMP",

		_ => throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type))
	};

	/// <inheritdoc />
	protected override string EscapeColumnName(string column) => $"\"{column.ToLowerInvariant().Replace("\"", "\"\"")}\"";

	/// <inheritdoc />
	protected override string EscapeTableName(string table) => table.SFormat("\"{0}\"", table);

	/// <inheritdoc />
	public override string CreateTable(SqlTable table)
	{
		ValidateSqlColumnType(table.Columns);

		IEnumerable<string> columns = table.Columns.Select(c =>
		{
			// Handle PostgreSQL-specific auto-increment using SERIAL/BIGSERIAL
			string dataType;

			if (c.AutoIncrement)
			{
				dataType = c.Type is MySqlDbType.Int32 ? "SERIAL" : "BIGSERIAL";
			}
			else
			{
				dataType = DbTypeToString(c.Type, c.Length);
			}

			return "{0} {1} {2} {3} {4}".SFormat(EscapeColumnName(c.Name),
				dataType,
				c.Primary ? "PRIMARY KEY" : "",
				c.NotNull && !c.AutoIncrement ? "NOT NULL" : "", // SERIAL implies NOT NULL
				c.DefaultCurrentTimestamp ? "DEFAULT CURRENT_TIMESTAMP" : "");
		});

		string[] uniques = table.Columns
			.Where(c => c.Unique).Select(c => EscapeColumnName(c.Name))
			.ToArray(); // No re-enumeration

		return $"CREATE TABLE {EscapeTableName(table.Name)} ({string.Join(", ", columns)} {(uniques.Any() ? ", UNIQUE({0})".SFormat(string.Join(", ", uniques)) : "")})";
	}

	/// <inheritdoc />
	public override string RenameTable(string from, string to) => "ALTER TABLE {0} RENAME TO {1}".SFormat(from, to);
}
