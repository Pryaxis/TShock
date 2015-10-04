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
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
	/// <summary>
	/// Sql table.
	/// </summary>
	public class SqlTable
	{
		/// <summary>
		/// Gets or sets the table columns.
		/// </summary>
		/// <value>The table columns.</value>
		public List<SqlColumn> Columns { get; protected set; }
		
		/// <summary>
		/// Gets or sets the table name.
		/// </summary>
		/// <value>The table name.</value>
		public string Name { get; protected set; }

		/// <summary>
		/// The foreign keys.
		/// </summary>
		public List<SqlForeignKey> ForeignKeys = new List<SqlForeignKey>();

		/// <summary>
		/// The indexes.
		/// </summary>
		public List<SqlIndex> Indexes = new List<SqlIndex>();

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlTable"/> class.
		/// </summary>
		/// <param name="name">Name of table.</param>
		/// <param name="columns">Table columns.</param>
		public SqlTable(string name, params SqlColumn[] columns)
			: this(name, new List<SqlColumn>(columns))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlTable"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="columns">Columns.</param>
		/// <param name="fkeys">Foreign keys.</param>
		/// <param name="indexes">Indexes.</param>
		public SqlTable(string name, SqlColumn[] columns, SqlForeignKey[] fkeys, SqlIndex[] indexes)
			: this(name, new List<SqlColumn>(columns), new List<SqlForeignKey>(fkeys), new List<SqlIndex>(indexes))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlTable"/> class.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="columns">Columns.</param>
		/// <param name="fkeys">Foreign keys.</param>
		/// <param name="indexes">Indexes.</param>
		public SqlTable(string name, List<SqlColumn> columns, List<SqlForeignKey> fkeys, List<SqlIndex> indexes)
		{
			Name = name;
			Columns = columns;
			ForeignKeys = fkeys;
			Indexes = indexes;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlTable"/> class.
		/// </summary>
		/// <param name="name">Name of table.</param>
		/// <param name="columns">Table columns.</param>
		public SqlTable(string name, List<SqlColumn> columns)
		{
			Name = name;
			Columns = columns;
		}

		/// <summary>
		/// Adds a foreign key.
		/// </summary>
		/// <returns>The instance of <see cref="TShockAPI.DB.SqlTable"/> to provide a fluent interface for method chaining.</returns>
		/// <param name="foreignKey">Foreign key.</param>
		public SqlTable AddForeignKey(SqlForeignKey foreignKey)
		{
			ForeignKeys.Add(foreignKey);
			return this;
		}

		/// <summary>
		/// Adds an index.
		/// </summary>
		/// <returns>The instance of <see cref="TShockAPI.DB.SqlTable"/> to provide a fluent interface for method chaining.</returns>
		/// <param name="index">An index.</param>
		public SqlTable AddIndex(SqlIndex index)
		{
			Indexes.Add(index);
			return this;
		}
	}

	/// <summary>
	/// Sql table creator.
	/// </summary>
	public class SqlTableCreator
	{
		private IDbConnection database;
		private IQueryBuilder creator;

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlTableCreator"/> class.
		/// </summary>
		/// <param name="db">A database connection handler.</param>
		/// <param name="provider">A query builder.</param>
		public SqlTableCreator(IDbConnection db, IQueryBuilder provider)
		{
			database = db;
			creator = provider;
		}

		/// <summary>
		/// Ensures the table structure exists, creates it if it doesn't.
		/// </summary>
		/// <returns><c>true</c>, if table was created, <c>false</c> otherwise.</returns>
		/// <param name="table">Table.</param>
		public bool EnsureTableStructure(SqlTable table)
		{
			var columns = GetColumns(table);
			if (columns.Count > 0)
			{
				if (!table.Columns.All(c => columns.Contains(c.Name)) || !columns.All(c => table.Columns.Any(c2 => c2.Name == c)))
				{
					var from = new SqlTable(table.Name, columns.Select(s => new SqlColumn(s, MySqlDbType.String)).ToList());
					database.Query(creator.AlterTable(from, table));
				}
			}
			else
			{
				/* 
				 * Use a transaction for creating the table. MySQL and Oracle allow defining INDEXes
				 * in CREATE TABLE, but SQLite and PostgreSQL do not. For this, we need a transaction.
				 */
				var queries = new List<string>();
				queries.Add(creator.CreateTable(table));
				queries.AddRange(table.Indexes.Select(index => creator.CreateIndex(index)));
				return database.AsTransaction(queries);
			}
			return false;
		}

		/// <summary>
		/// Ensures a table exists and that its structure is correct
		/// </summary>
		/// <param name="table">The table name</param>
		[Obsolete("This method will be replaced by EnsureTableStructure.")]
		public void EnsureExists(SqlTable table)
		{
			EnsureTableStructure(table);
		}

		/// <summary>
		/// Gets the table columns.
		/// </summary>
		/// <returns>List of column names.</returns>
		/// <param name="table">Table to retrieve column names from.</param>
		public List<string> GetColumns(SqlTable table)
		{
			var ret = new List<string>();
			var name = database.GetSqlType();
			if (name == SqlType.Sqlite)
			{
				using (var reader = database.QueryReader("PRAGMA table_info({0})".SFormat(table.Name)))
				{
					while (reader.Read())
						ret.Add(reader.Get<string>("name"));
				}
			}
			else if (name == SqlType.Mysql)
			{
				using (
					var reader =
						database.QueryReader(
							"SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_NAME=@0 AND TABLE_SCHEMA=@1", table.Name,
							database.Database))
				{
					while (reader.Read())
						ret.Add(reader.Get<string>("COLUMN_NAME"));
				}
			}
			else
			{
				throw new NotSupportedException();
			}

			return ret;
		}

		/// <summary>
		/// Deletes row(s) from the database.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="wheres">A list of where conditions.</param>
		public void DeleteRow(string table, List<SqlValue> wheres)
		{
			database.Query(creator.DeleteRow(table, wheres));
		}
	}
}