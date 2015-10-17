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
using System.Globalization;
using System.Linq;
using System.Text;

using Mono.Data.Sqlite;
using MySql.Data.MySqlClient;
using TShockAPI.Extensions;

namespace TShockAPI.DB
{
	/// <summary>
	/// Query builder interface.
	/// </summary>
	public interface IQueryBuilder
	{
		/// <summary>
		/// Generates SQL for creating a table.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="table">The table to generate.</param>
		string CreateTable(SqlTable table);

		/// <summary>
		/// Generates SQL for creating a table index.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="index">The index to generate.</param>
		string CreateIndex(SqlIndex index);

		/// <summary>
		/// Alter a table from source to destination
		/// </summary>
		/// <param name="from">Must have name and column names. Column types are not required</param>
		/// <param name="to">Must have column names and column types.</param>
		/// <returns>The SQL query.</returns>
		string AlterTable(SqlTable from, SqlTable to);

		/// <summary>
		/// Get the string value of a column data type.
		/// </summary>
		/// <returns>The type as a string.</returns>
		/// <param name="type">Column data type.</param>
		/// <param name="length">Length of column.</param>
		string DbTypeToString(MySqlDbType type, int? length);
		
		/// <summary>
		/// Updates values for the specified columns.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="table">Name of table.</param>
		/// <param name="values">List of <see cref="TShockAPI.DB.SqlValue"/>.</param>
		/// <param name="wheres">A list of WHERE conditions.</param>
		string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres);
		
		/// <summary>
		/// Inserts a row with the specified column and value pairs.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="table">Name of table.</param>
		/// <param name="values">List of <see cref="TShockAPI.DB.SqlValue"/>.</param>
		string InsertValues(string table, List<SqlValue> values);
		
		string ReadColumn(string table, List<SqlValue> wheres);
		
		/// <summary>
		/// Delete row(s).
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="table">The source table.</param>
		/// <param name="wheres">A list of WHERE conditions.</param>
		string DeleteRow(string table, List<SqlValue> wheres);
		
		/// <summary>
		/// Rename a table.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="from">Source table name</param>
		/// <param name="to">Destination table name</param>
		string RenameTable(string from, string to);

		/// <summary>
		/// Quotes the identifier.
		/// </summary>
		/// <returns>The quoted identifier.</returns>
		/// <param name="identifier">An identifier.</param>
		/// <param name="alias">An optional alias.</param>
		string QuoteIdentifier(string identifier, string alias = "");

		/// <summary>
		/// Quotes the identifiers.
		/// </summary>
		/// <returns>The quoted identifiers.</returns>
		/// <param name="identifiers">Identifiers.</param>
		IEnumerable<string> QuoteIdentifiers(IEnumerable<string> identifiers);
	}

	/// <summary>
	/// Sqlite query creator.
	/// </summary>
	public class SqliteQueryCreator : GenericQueryCreator
	{
		/// <summary>
		/// Gets the quote identifier symbol.
		/// </summary>
		/// <value>The quote identifier symbol.</value>
		public override char IdentifierQuoteChar { get; set; } = '"';

		/// <summary>
		/// Creates a column definition.
		/// </summary>
		/// <returns>The column definition.</returns>
		/// <param name="column">Column.</param>
		protected override string GetColumnDefinition(SqlColumn column)
		{
			List<string> columnDefinition = new List<string>();
			Action<string> add = definition => {
				if (!String.IsNullOrEmpty(definition))
					columnDefinition.Add(definition);
			};

			add(QuoteIdentifier(column.Name));
			add(DbTypeToString(column.Type, column.Length));

			if (column.AutoIncrement && column.Primary)
				add("PRIMARY KEY AUTOINCREMENT");

			if (column.NotNull)
				add("NOT NULL");

			return String.Join(" ", columnDefinition);
		}

		/// <summary>
		/// Generates the primary key definition.
		/// </summary>
		/// <returns>The primary key definition.</returns>
		/// <param name="columns">Columns.</param>
		/// <remarks>
		/// In SQLite an auto incrememnt column must be defined with an INTEGER PRIMARY
		/// KEY column definition. If column is auto increment we bail because a duplicate
		/// primary key definition would occur and a compound key results in an error.
		/// <see cref="TShockAPI.DB.SqliteQueryCreator.GetColumnDefinition"/>
		/// </remarks>
		protected override string GetPrimaryKeyDefinition(IEnumerable<SqlColumn> columns)
		{
			if (columns.Count() < 1)
				return null;

			// See remark.
			if (columns.Any(c => c.AutoIncrement))
				return null;

			var quotedColumns = columns.Select(c => QuoteIdentifier(c.Name));
			return String.Format(CultureInfo.InvariantCulture, "PRIMARY KEY ({0})", String.Join(", ", quotedColumns));
		}

		/// <summary>
		/// Generates the index column definition.
		/// </summary>
		/// <returns>The index column definition.</returns>
		/// <param name="column">The index column.</param>
		/// <remarks>SQLite doesn't support length, we ignore it here</remarks>
		protected override string GetIndexColumnDefinition(SqlIndexColumn column)
		{
			return String.Format(CultureInfo.InvariantCulture, "{0} {1}", 
				QuoteIdentifier(column.Name),
				SqlIndexColumn.SortOrderToString(column.Order));
		}

		/// <summary>
		/// Rename a table.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="from">Source table name</param>
		/// <param name="to">Destination table name</param>
		public override string RenameTable(string from, string to)
		{
			return String.Format(CultureInfo.InvariantCulture, "ALTER TABLE {0} RENAME TO {1}", from, to);
		}

		/// <summary>
		/// Maps enum types to a string representation.
		/// </summary>
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
		};

		/// <summary>
		/// Get the string value of a column data type.
		/// </summary>
		/// <returns>The type as a string.</returns>
		/// <param name="type">Column data type.</param>
		/// <param name="length">Length of column.</param>
		/// <remarks>SQLite as of 3.x ignores length</remarks>
		public override string DbTypeToString(MySqlDbType type, int? length)
		{
			string ret;
			if (TypesAsStrings.TryGetValue(type, out ret))
				return ret + (length != null ? "({0})".SFormat((int)length) : "");
			throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
		}
	}

	/// <summary>
	/// Mysql query creator.
	/// </summary>
	public class MysqlQueryCreator : GenericQueryCreator
	{
		/// <summary>
		/// Gets the quote identifier symbol.
		/// </summary>
		/// <value>The quote identifier symbol.</value>
		public override char IdentifierQuoteChar { get; set; } = '`';

		/// <summary>
		/// Creates a column definition.
		/// </summary>
		/// <returns>The column definition.</returns>
		/// <param name="column">Column.</param>
		protected override string GetColumnDefinition(SqlColumn column)
		{
			List<string> columnDefinition = new List<string>();
			Action<string> add = definition => {
				if (!String.IsNullOrEmpty(definition))
					columnDefinition.Add(definition);
			};

			add(QuoteIdentifier(column.Name));
			add(DbTypeToString(column.Type, column.Length));

			if (column.AutoIncrement)
				add("AUTO_INCREMENT");

			if (column.NotNull)
				add("NOT NULL");

			return String.Join(" ", columnDefinition);
		}

		/// <summary>
		/// Rename a table.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="from">Source table name</param>
		/// <param name="to">Destination table name</param>
		public override string RenameTable(string from, string to)
		{
			return "RENAME TABLE {0} TO {1}".SFormat(from, to);
		}

		/// <summary>
		/// Maps enum types to a string representation.
		/// </summary>
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
		};

		/// <summary>
		/// Get the string value of a column data type.
		/// </summary>
		/// <returns>The type as a string.</returns>
		/// <param name="type">Column data type.</param>
		/// <param name="length">Length of column.</param>
		public override string DbTypeToString(MySqlDbType type, int? length)
		{
			string ret;
			if (TypesAsStrings.TryGetValue(type, out ret))
				return ret + (length != null ? "({0})".SFormat((int)length) : "");
			throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
		}
	}

	/// <summary>
	/// Generic query creator.
	/// </summary>
	public abstract class GenericQueryCreator : IQueryBuilder
	{
		protected static Random rand = new Random();

		/// <summary>
		/// Gets the quote identifier symbol.
		/// </summary>
		/// <value>The quote identifier symbol.</value>
		public virtual char IdentifierQuoteChar { get; set; } = '"';

		/// <summary>
		/// Generates SQL for creating a table.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="table">The table to generate.</param>
		public virtual string CreateTable(SqlTable table)
		{
			List<string> createDefinition = new List<string>();
			Action<string> add = definition => {
				if (!String.IsNullOrEmpty(definition))
					createDefinition.Add(definition);
			};

			add(GetColumnDefinitions(table.Columns));
			add(GetUniqueDefinition(table.Columns.Where(c => c.Unique)));
			add(GetPrimaryKeyDefinition(table.Columns.Where(c => c.Primary)));
			add(GetForeignKeyDefinitions(table.ForeignKeys));

			return String.Format(CultureInfo.InvariantCulture, "CREATE TABLE {0} ({1})", QuoteIdentifier(table.Name), String.Join(", ", createDefinition));
		}

		/// <summary>
		/// Generates SQL for creating a table index.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="index">The index to generate.</param>
		public virtual string CreateIndex(SqlIndex index)
		{
			List<string> createDefinition = new List<string>();
			Action<string> add = definition => {
				if (!String.IsNullOrEmpty(definition))
					createDefinition.Add(definition);
			};

			add(GetIndexColumnDefinitions(index.Columns));

			return String.Format(CultureInfo.InvariantCulture, "CREATE {0} INDEX {1} ON {2} ({3})",
				index.Unique ? "UNIQUE" : "",
				QuoteIdentifier(index.Name),
				QuoteIdentifier(index.Table),
				String.Join(", ", createDefinition));
		}

		/// <summary>
		/// Builds an SQL query to rename a table.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="from">Source table name</param>
		/// <param name="to">Destination table name</param>
		public abstract string RenameTable(string from, string to);

		/// <summary>
		/// Alter a table from source to destination
		/// </summary>
		/// <param name="from">Must have name and column names. Column types are not required</param>
		/// <param name="to">Must have column names and column types.</param>
		/// <returns>The SQL query.</returns>
		public string AlterTable(SqlTable from, SqlTable to)
		{
			/*
			 * Any example outpuf from this looks like:-
				ALTER TABLE "main"."Bans" RENAME TO "oXHFcGcd04oXHFcGcd04_Bans"
				CREATE TABLE "main"."Bans" ("IP" TEXT PRIMARY KEY ,"Name" TEXT)
				INSERT INTO "main"."Bans" SELECT "IP","Name" FROM "main"."oXHFcGcd04oXHFcGcd04_Bans"
				DROP TABLE "main"."oXHFcGcd04oXHFcGcd04_Bans"
			 *
			 * Twitchy - Oh. I get it!
			 */
			var rstr = rand.NextString(20);
			var escapedTable = QuoteIdentifier(from.Name);
			var tmpTable = QuoteIdentifier(String.Format(CultureInfo.InvariantCulture, "{0}_{1}", rstr, from.Name));
			var alter = RenameTable(escapedTable, tmpTable);
			var create = CreateTable(to);
			// combine all columns in the 'from' variable excluding ones that aren't in the 'to' variable.
			// exclude the ones that aren't in 'to' variable because if the column is deleted, why try to import the data?
			var columns = String.Join(", ", from.Columns.Where(c => to.Columns.Any(c2 => c2.Name == c.Name)).Select(c => c.Name));
			var insert = String.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} ({1}) SELECT {1} FROM {2}", escapedTable, columns, tmpTable);
			var drop = String.Format(CultureInfo.InvariantCulture, "DROP TABLE {0}", tmpTable);
			return "{0}; {1}; {2}; {3};".SFormat(alter, create, insert, drop);
		}

		/// <summary>
		/// Delete row(s).
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="table">The source table.</param>
		/// <param name="wheres">Where conditions.</param>
		public string DeleteRow(string table, List<SqlValue> wheres)
		{
			return String.Format(CultureInfo.InvariantCulture, "DELETE FROM {0} {1}", QuoteIdentifier(table), BuildWhere(wheres));
		}

		/// <summary>
		/// Updates values for the specified columns.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="table">Name of table.</param>
		/// <param name="values">List of <see cref="TShockAPI.DB.SqlValue"/>.</param>
		/// <param name="wheres">A list of WHERE conditions.</param>
		public string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres)
		{
			if (0 == values.Count)
				throw new ArgumentException("No values supplied");

			return String.Format(CultureInfo.InvariantCulture, "UPDATE {0} SET {1} {2}", QuoteIdentifier(table), String.Join(", ", values.Select(v => v.Name + " = " + v.Value)), BuildWhere(wheres));
		}

		/// <summary>
		/// Reads a column.
		/// </summary>
		/// <returns>The column.</returns>
		/// <param name="table">The table.</param>
		/// <param name="wheres">A list of WHERE conditions.</param>
		public string ReadColumn(string table, List<SqlValue> wheres)
		{
			return String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} {1}", QuoteIdentifier(table), BuildWhere(wheres));
		}

		/// <summary>
		/// Inserts a row with the specified column and value pairs.
		/// </summary>
		/// <returns>The SQL query.</returns>
		/// <param name="table">Name of table.</param>
		/// <param name="values">List of <see cref="TShockAPI.DB.SqlValue"/>.</param>
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

			return String.Format(CultureInfo.InvariantCulture, "INSERT INTO {0} ({1}) VALUES ({2})", QuoteIdentifier(table), sbnames, sbvalues);
		}

		/// <summary>
		/// Quotes the identifier.
		/// </summary>
		/// <returns>The quoted identifier.</returns>
		/// <param name="identifier">An identifier.</param>
		/// <param name="alias">An optional alias.</param>
		public string QuoteIdentifier(string identifier, string alias = "")
		{
			const char separator = '.';
			var identifiers = identifier.Split(new char[] { separator });
			var quoted = identifiers.Select(
				// Also escape identifier quote chars in the identifier (ie: 'identifier' to '''identifier''')
				x => IdentifierQuoteChar 
						+ x.Replace(IdentifierQuoteChar.ToString(), new String(IdentifierQuoteChar, 2)) 
						+ IdentifierQuoteChar);
			var joined = String.Join(separator.ToString(), quoted);

			if (!String.IsNullOrEmpty(alias))
				joined += " AS " + QuoteIdentifier(alias);
			
			return joined;
		}

		/// <summary>
		/// Quotes the identifiers.
		/// </summary>
		/// <returns>The quoted identifiers.</returns>
		/// <param name="identifiers">Identifiers.</param>
		public IEnumerable<string> QuoteIdentifiers(IEnumerable<string> identifiers)
		{
			return identifiers.Select(x => QuoteIdentifier(x));
		}

		/// <summary>
		/// Builds a series of where conditions.
		/// </summary>
		/// <returns>A where clause.</returns>
		/// <param name="wheres">A list of name-value pairs for testing equality.</param>
		protected string BuildWhere(List<SqlValue> wheres)
		{
			if (0 == wheres.Count)
				return String.Empty;

			return String.Format(CultureInfo.InvariantCulture, "WHERE {0}", String.Join(", ", wheres.Select(v => QuoteIdentifier(v.Name) + " = " + v.Value)));
		}

		/// <summary>
		/// Maps enum types to a string representation.
		/// </summary>
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
		};

		/// <summary>
		/// Get the string value of a column data type.
		/// </summary>
		/// <returns>The type as a string.</returns>
		/// <param name="type">Column data type.</param>
		/// <param name="length">Length of column.</param>
		public virtual string DbTypeToString(MySqlDbType type, int? length)
		{
			string ret;
			if (TypesAsStrings.TryGetValue(type, out ret))
				return ret + (length != null ? "({0})".SFormat((int)length) : "");
			throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
		}

		/// <summary>
		/// Generates column definitions for an index.
		/// </summary>
		/// <returns>The column definitions.</returns>
		/// <param name="columns">Columns.</param>
		protected virtual string GetIndexColumnDefinitions(IEnumerable<SqlIndexColumn> columns)
		{
			if (columns.Count() < 1)
				return null;

			return String.Join(", ", columns.Select(column => GetIndexColumnDefinition(column)));
		}

		/// <summary>
		/// Generates the index column definition.
		/// </summary>
		/// <returns>The index column definition.</returns>
		/// <param name="column">The index column.</param>
		protected virtual string GetIndexColumnDefinition(SqlIndexColumn column)
		{
			return String.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", 
				QuoteIdentifier(column.Name),
				column.Length.HasValue ? $"({column.Length})" : "",
				SqlIndexColumn.SortOrderToString(column.Order));
		}

		/// <summary>
		/// Generates the foreign key definitions.
		/// </summary>
		/// <returns>The foreign key definitions.</returns>
		/// <param name="keys">Keys.</param>
		protected virtual string GetForeignKeyDefinitions(IEnumerable<SqlForeignKey> keys)
		{
			if (keys.Count() < 1)
				return null;
			return String.Join(", ", keys.Select(fk => GetForeignKeyDefinition(fk)));
		}

		/// <summary>
		/// Generates the foreign key definition.
		/// </summary>
		/// <returns>The foreign key definition.</returns>
		/// <param name="key">Key.</param>
		protected virtual string GetForeignKeyDefinition(SqlForeignKey key)
		{
			var sql = String.Format(CultureInfo.InvariantCulture, "FOREIGN KEY ({0}) REFERENCES {1}({2}) {3} {4}",
				String.Join(", ", QuoteIdentifiers(key.Columns)),
				QuoteIdentifier(key.ParentTable),
				String.Join(", ", QuoteIdentifiers(key.ParentColumns)),
				key.OnDeleteEvent,
				key.OnUpdateEvent);

			if (!String.IsNullOrEmpty(key.Name))
				sql = String.Format(CultureInfo.InvariantCulture, "CONSTRAINT {0} {1}", QuoteIdentifier(key.Name), sql);

			return sql;
		}

		/// <summary>
		/// Generates the primary key definition.
		/// </summary>
		/// <returns>The primary key definition.</returns>
		/// <param name="columns">Columns.</param>
		protected virtual string GetPrimaryKeyDefinition(IEnumerable<SqlColumn> columns)
		{
			if (columns.Count() < 1)
				return null;

			var quotedColumns = columns.Select(c => QuoteIdentifier(c.Name));
			return String.Format(CultureInfo.InvariantCulture, "PRIMARY KEY ({0})", String.Join(", ", quotedColumns));
		}

		/// <summary>
		/// Generates a unique constraint definition.
		/// </summary>
		/// <returns>The unique definition.</returns>
		/// <param name="columns">Columns.</param>
		protected virtual string GetUniqueDefinition(IEnumerable<SqlColumn> columns)
		{
			if (columns.Count() < 1)
				return null;
			var quotedColumns = columns.Select(c => QuoteIdentifier(c.Name));
			return String.Format(CultureInfo.InvariantCulture, "UNIQUE ({0})", String.Join(", ", quotedColumns));
		}

		/// <summary>
		/// Creates a column definition.
		/// </summary>
		/// <returns>The column definition.</returns>
		/// <param name="column">Column.</param>
		protected virtual string GetColumnDefinition(SqlColumn column)
		{
			return String.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}",
				QuoteIdentifier(column.Name),
				DbTypeToString(column.Type, column.Length),
				column.AutoIncrement ? "AUTO_INCREMENT" : "",
				column.NotNull ? "NOT NULL" : "");
		}

		/// <summary>
		/// Generates column definitions.
		/// </summary>
		/// <returns>The column definitions.</returns>
		/// <param name="columns">Columns.</param>
		protected virtual string GetColumnDefinitions(IEnumerable<SqlColumn> columns)
		{
			if (columns.Count() < 1)
				return null;
			return String.Join(", ", columns.Select(c => GetColumnDefinition(c)));
		}
	}
}
