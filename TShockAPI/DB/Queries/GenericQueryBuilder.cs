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
using System.Text;
using MySql.Data.MySqlClient;
using TShockAPI.Extensions;

namespace TShockAPI.DB.Queries;

/// <summary>
/// A Generic Query Creator (abstract)
/// </summary>
public abstract class GenericQueryBuilder : IQueryBuilder
{
	protected static Random rand = new Random();

	/// <summary>
	/// Escapes the table name
	/// </summary>
	/// <param name="table">The name of the table to be escaped</param>
	/// <returns></returns>
	protected abstract string EscapeTableName(string table);

	/// <summary>
	/// Escapes a column identifier for the current SQL dialect.
	/// </summary>
	/// <param name="column">The column name to escape.</param>
	/// <returns>The escaped column identifier.</returns>
	protected virtual string EscapeColumnName(string column) => column;

	/// <inheritdoc />
	public abstract string CreateTable(SqlTable table);

	/// <inheritdoc />
	public abstract string RenameTable(string from, string to);

	/// <inheritdoc />
	public string AlterTable(SqlTable from, SqlTable to)
	{
		var rstr = rand.NextString(20);
		var escapedTable = EscapeTableName(from.Name);
		var tmpTable = EscapeTableName("{0}_{1}".SFormat(rstr, from.Name));
		var alter = RenameTable(escapedTable, tmpTable);
		var create = CreateTable(to);
		// combine all columns in the 'from' variable excluding ones that aren't in the 'to' variable.
		// exclude the ones that aren't in 'to' variable because if the column is deleted, why try to import the data?
		var columns = string.Join(", ", from.Columns.Where(c => to.Columns.Any(c2 => c2.Name.Equals(c.Name, StringComparison.OrdinalIgnoreCase))).Select(c => EscapeColumnName(c.Name)));
		var insert = "INSERT INTO {0} ({1}) SELECT {1} FROM {2}".SFormat(escapedTable, columns, tmpTable);
		var drop = "DROP TABLE {0}".SFormat(tmpTable);
		return "{0}; {1}; {2}; {3};".SFormat(alter, create, insert, drop);
	}

	/// <inheritdoc />
	public abstract string DbTypeToString(MySqlDbType type, int? length);

	/// <summary>
	/// Check for errors in the columns.
	/// </summary>
	/// <param name="columns"></param>
	/// <exception cref="SqlColumnException"></exception>
	protected static void ValidateSqlColumnType(List<SqlColumn> columns)
	{
		columns.ForEach(x =>
		{
			if (x.DefaultCurrentTimestamp && x.Type != MySqlDbType.DateTime)
			{
				throw new SqlColumnException(GetString("Can't set to true SqlColumn.DefaultCurrentTimestamp when the MySqlDbType is not DateTime"));
			}
		});
	}


	/// <inheritdoc />
	public string DeleteRow(string table, List<SqlValue> wheres)
	{
		return "DELETE FROM {0} {1}".SFormat(EscapeTableName(table), BuildWhere(wheres));
	}

	/// <inheritdoc />
	public string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres)
	{
		if (0 == values.Count)
			throw new ArgumentException(GetString("No values supplied"));

		return "UPDATE {0} SET {1} {2}".SFormat(EscapeTableName(table), string.Join(", ", values.Select(v => v.Name + " = " + v.Value)), BuildWhere(wheres));
	}


	/// <inheritdoc />
	public string ReadColumn(string table, List<SqlValue> wheres)
	{
		return "SELECT * FROM {0} {1}".SFormat(EscapeTableName(table), BuildWhere(wheres));
	}


	public string InsertValues(string table, List<SqlValue> values)
	{
		var sbnames = new StringBuilder();
		var sbvalues = new StringBuilder();
		int count = 0;
		foreach (SqlValue value in values)
		{
			sbnames.Append(value.Name);
			sbvalues.Append(value.Value.ToString());

			if (count != values.Count - 1)
			{
				sbnames.Append(", ");
				sbvalues.Append(", ");
			}
			count++;
		}

		return "INSERT INTO {0} ({1}) VALUES ({2})".SFormat(EscapeTableName(table), sbnames, sbvalues);
	}

	/// <summary>
	/// Builds the SQL WHERE clause
	/// </summary>
	/// <param name="wheres"></param>
	/// <returns></returns>
	protected static string BuildWhere(List<SqlValue> wheres) => wheres.Count > 0
		? "WHERE {0}".SFormat(string.Join(" AND ", wheres.Select(v => $"{v.Name} = {v.Value}")))
		: string.Empty;
}
