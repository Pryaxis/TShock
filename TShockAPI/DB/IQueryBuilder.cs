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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TShockAPI.Extensions;

namespace TShockAPI.DB
{
	/// <summary>
	/// Interface for various SQL related utilities.
	/// </summary>
	public interface IQueryBuilder
	{
		/// <summary>
		/// Creates a table from a SqlTable object.
		/// </summary>
		/// <param name="table">The SqlTable to create the table from</param>
		/// <returns>The sql query for the table creation.</returns>
		string CreateTable(SqlTable table);

		/// <summary>
		/// Alter a table from source to destination
		/// </summary>
		/// <param name="from">Must have name and column names. Column types are not required</param>
		/// <param name="to">Must have column names and column types.</param>
		/// <returns>The SQL Query</returns>
		string AlterTable(SqlTable from, SqlTable to);

		/// <summary>
		/// Converts the MySqlDbType enum to it's string representation.
		/// </summary>
		/// <param name="type">The MySqlDbType type</param>
		/// <param name="length">The length of the datatype</param>
		/// <returns>The string representation</returns>
		string DbTypeToString(MySqlDbType type, int? length);

		/// <summary>
		/// A UPDATE Query
		/// </summary>
		/// <param name="table">The table to update</param>
		/// <param name="values">The values to change</param>
		/// <param name="wheres"></param>
		/// <returns>The SQL query</returns>
		string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres);

		/// <summary>
		/// A INSERT query
		/// </summary>
		/// <param name="table">The table to insert to</param>
		/// <param name="values"></param>
		/// <returns>The SQL Query</returns>
		string InsertValues(string table, List<SqlValue> values);

		/// <summary>
		/// A SELECT query to get all columns
		/// </summary>
		/// <param name="table">The table to select from</param>
		/// <param name="wheres"></param>
		/// <returns>The SQL query</returns>
		string ReadColumn(string table, List<SqlValue> wheres);

		/// <summary>
		/// Deletes row(s).
		/// </summary>
		/// <param name="table">The table to delete the row from</param>
		/// <param name="wheres"></param>
		/// <returns>The SQL query</returns>
		string DeleteRow(string table, List<SqlValue> wheres);

		/// <summary>
		/// Renames the given table.
		/// </summary>
		/// <param name="from">Old name of the table</param>
		/// <param name="to">New name of the table</param>
		/// <returns>The sql query for renaming the table.</returns>
		string RenameTable(string from, string to);
	}

	/// <summary>
	/// Query Creator for Sqlite
	/// </summary>
	public class SqliteQueryCreator : GenericQueryCreator, IQueryBuilder
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
		public string DbTypeToString(MySqlDbType type, int? length)
		{
			string ret;
			if (TypesAsStrings.TryGetValue(type, out ret))
				return ret;
			throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
		}

		/// <summary>
		/// Escapes the table name
		/// </summary>
		/// <param name="table">The name of the table to be escaped</param>
		/// <returns></returns>
		protected override string EscapeTableName(string table)
		{
			return table.SFormat("'{0}'", table);
		}
	}

	/// <summary>
	/// Query Creator for MySQL
	/// </summary>
	public class MysqlQueryCreator : GenericQueryCreator, IQueryBuilder
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
					"{0} {1} {2} {3} {4} {5}".SFormat(c.Name, DbTypeToString(c.Type, c.Length),
												c.Primary ? "PRIMARY KEY" : "",
												c.AutoIncrement ? "AUTO_INCREMENT" : "",
												c.NotNull ? "NOT NULL" : "",
												c.DefaultCurrentTimestamp ? "DEFAULT CURRENT_TIMESTAMP" : ""));
			var uniques = table.Columns.Where(c => c.Unique).Select(c => c.Name);
			return "CREATE TABLE {0} ({1} {2})".SFormat(EscapeTableName(table.Name), string.Join(", ", columns),
														uniques.Count() > 0
															? ", UNIQUE({0})".SFormat(string.Join(", ", uniques))
															: "");
		}

		/// <summary>
		/// Renames the given table.
		/// </summary>
		/// <param name="from">Old name of the table</param>
		/// <param name="to">New name of the table</param>
		/// <returns>The sql query for renaming the table.</returns>
		public override string RenameTable(string from, string to)
		{
			return "RENAME TABLE {0} TO {1}".SFormat(from, to);
		}

		private static readonly Dictionary<MySqlDbType, string> TypesAsStrings = new Dictionary<MySqlDbType, string>
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

		/// <summary>
		/// Converts the MySqlDbType enum to it's string representation.
		/// </summary>
		/// <param name="type">The MySqlDbType type</param>
		/// <param name="length">The length of the datatype</param>
		/// <returns>The string representation</returns>
		public string DbTypeToString(MySqlDbType type, int? length)
		{
			string ret;
			if (TypesAsStrings.TryGetValue(type, out ret))
				return ret + (length != null ? "({0})".SFormat((int)length) : "");
			throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
		}

		/// <summary>
		/// Escapes the table name
		/// </summary>
		/// <param name="table">The name of the table to be escaped</param>
		/// <returns></returns>
		protected override string EscapeTableName(string table)
		{
			return table.SFormat("`{0}`", table);
		}
	}

	/// <summary>
	/// A Generic Query Creator (abstract)
	/// </summary>
	public abstract class GenericQueryCreator
	{
		protected static Random rand = new Random();

		/// <summary>
		/// Escapes the table name
		/// </summary>
		/// <param name="table">The name of the table to be escaped</param>
		/// <returns></returns>
		protected abstract string EscapeTableName(string table);

		/// <summary>
		/// Creates a table from a SqlTable object.
		/// </summary>
		/// <param name="table">The SqlTable to create the table from</param>
		/// <returns>The sql query for the table creation.</returns>
		public abstract string CreateTable(SqlTable table);

		/// <summary>
		/// Renames the given table.
		/// </summary>
		/// <param name="from">Old name of the table</param>
		/// <param name="to">New name of the table</param>
		/// <returns>The sql query for renaming the table.</returns>
		public abstract string RenameTable(string from, string to);

		/// <summary>
		/// Alter a table from source to destination
		/// </summary>
		/// <param name="from">Must have name and column names. Column types are not required</param>
		/// <param name="to">Must have column names and column types.</param>
		/// <returns>The SQL Query</returns>
		public string AlterTable(SqlTable from, SqlTable to)
		{
			var rstr = rand.NextString(20);
			var escapedTable = EscapeTableName(from.Name);
			var tmpTable = EscapeTableName("{0}_{1}".SFormat(rstr, from.Name));
			var alter = RenameTable(escapedTable, tmpTable);
			var create = CreateTable(to);
			// combine all columns in the 'from' variable excluding ones that aren't in the 'to' variable.
			// exclude the ones that aren't in 'to' variable because if the column is deleted, why try to import the data?
			var columns = string.Join(", ", from.Columns.Where(c => to.Columns.Any(c2 => c2.Name == c.Name)).Select(c => c.Name));
			var insert = "INSERT INTO {0} ({1}) SELECT {1} FROM {2}".SFormat(escapedTable, columns, tmpTable);
			var drop = "DROP TABLE {0}".SFormat(tmpTable);
			return "{0}; {1}; {2}; {3};".SFormat(alter, create, insert, drop);
		}

		/// <summary>
		/// Check for errors in the columns.
		/// </summary>
		/// <param name="columns"></param>
		/// <exception cref="SqlColumnException"></exception>
		public void ValidateSqlColumnType(List<SqlColumn> columns)
		{
			columns.ForEach(x =>
			{
				if (x.DefaultCurrentTimestamp && x.Type != MySqlDbType.DateTime)
				{
					throw new SqlColumnException("Can't set to true SqlColumn.DefaultCurrentTimestamp " +
						"when the MySqlDbType is not DateTime");
				}
			});
		}

		/// <summary>
		/// Deletes row(s).
		/// </summary>
		/// <param name="table">The table to delete the row from</param>
		/// <param name="wheres"></param>
		/// <returns>The SQL query</returns>
		public string DeleteRow(string table, List<SqlValue> wheres)
		{
			return "DELETE FROM {0} {1}".SFormat(EscapeTableName(table), BuildWhere(wheres));
		}

		/// <summary>
		/// A UPDATE Query
		/// </summary>
		/// <param name="table">The table to update</param>
		/// <param name="values">The values to change</param>
		/// <param name="wheres"></param>
		/// <returns>The SQL query</returns>
		public string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres)
		{
			if (0 == values.Count)
				throw new ArgumentException("No values supplied");

			return "UPDATE {0} SET {1} {2}".SFormat(EscapeTableName(table), string.Join(", ", values.Select(v => v.Name + " = " + v.Value)), BuildWhere(wheres));
		}

		/// <summary>
		/// A SELECT query to get all columns
		/// </summary>
		/// <param name="table">The table to select from</param>
		/// <param name="wheres"></param>
		/// <returns>The SQL query</returns>
		public string ReadColumn(string table, List<SqlValue> wheres)
		{
			return "SELECT * FROM {0} {1}".SFormat(EscapeTableName(table), BuildWhere(wheres));
		}

		/// <summary>
		/// A INSERT query
		/// </summary>
		/// <param name="table">The table to insert to</param>
		/// <param name="values"></param>
		/// <returns>The SQL Query</returns>
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
		protected static string BuildWhere(List<SqlValue> wheres)
		{
			if (0 == wheres.Count)
				return string.Empty;

			return "WHERE {0}".SFormat(string.Join(", ", wheres.Select(v => v.Name + " = " + v.Value)));
		}
	}
}
