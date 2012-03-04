/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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

namespace TShockAPI.DB
{
	public interface IQueryBuilder
	{
		string CreateTable(SqlTable table);
		string AlterTable(SqlTable from, SqlTable to);
		string DbTypeToString(MySqlDbType type, int? length);
		string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres);
		string InsertValues(string table, List<SqlValue> values);
		string ReadColumn(string table, List<SqlValue> wheres);
		string DeleteRow(string table, List<SqlValue> wheres);
		string RenameTable(string from, string to);
	}

	public class SqliteQueryCreator : GenericQueryCreator, IQueryBuilder
	{
		public override string CreateTable(SqlTable table)
		{
			var columns =
				table.Columns.Select(
					c =>
					"'{0}' {1} {2} {3} {4} {5}".SFormat(c.Name, DbTypeToString(c.Type, c.Length), c.Primary ? "PRIMARY KEY" : "",
					                                c.AutoIncrement ? "AUTOINCREMENT" : "", c.NotNull ? "NOT NULL" : "",
					                                c.Unique ? "UNIQUE" : ""));
			return "CREATE TABLE {0} ({1})".SFormat(EscapeTableName(table.Name), string.Join(", ", columns));
		}

		public override string RenameTable(string from, string to)
		{
			return "ALTER TABLE {0} RENAME TO {1}".SFormat(from, to);
		}

		private static readonly Dictionary<MySqlDbType, string> TypesAsStrings = new Dictionary<MySqlDbType, string>
		{
		    {MySqlDbType.VarChar, "TEXT"},
		    {MySqlDbType.String, "TEXT"},
		    {MySqlDbType.Text, "TEXT"},
		    {MySqlDbType.TinyText, "TEXT"},
		    {MySqlDbType.MediumText, "TEXT"},
		    {MySqlDbType.LongText, "TEXT"},
		    {MySqlDbType.Int32, "INTEGER"},
			{MySqlDbType.Blob, "BLOB"},
		};

		public string DbTypeToString(MySqlDbType type, int? length)
		{
			string ret;
			if (TypesAsStrings.TryGetValue(type, out ret))
				return ret;
			throw new NotImplementedException(Enum.GetName(typeof (MySqlDbType), type));
		}

		protected override string EscapeTableName(string table)
		{
			return table.SFormat("'{0}'", table);
		}
	}

	public class MysqlQueryCreator : GenericQueryCreator, IQueryBuilder
	{
		public override string CreateTable(SqlTable table)
		{
			var columns =
				table.Columns.Select(
					c =>
					"{0} {1} {2} {3} {4}".SFormat(c.Name, DbTypeToString(c.Type, c.Length), c.Primary ? "PRIMARY KEY" : "",
					                          c.AutoIncrement ? "AUTO_INCREMENT" : "", c.NotNull ? "NOT NULL" : ""));
			var uniques = table.Columns.Where(c => c.Unique).Select(c => c.Name);
			return "CREATE TABLE {0} ({1} {2})".SFormat(EscapeTableName(table.Name), string.Join(", ", columns),
			                                            uniques.Count() > 0
			                                            	? ", UNIQUE({0})".SFormat(string.Join(", ", uniques))
			                                            	: "");
		}

		public override string RenameTable(string from, string to)
		{
			return "RENAME TABLE {0} TO {1}".SFormat(from, to);
		}

		private static readonly Dictionary<MySqlDbType, string> TypesAsStrings = new Dictionary<MySqlDbType, string>
		{
		    {MySqlDbType.VarChar, "VARCHAR"},
		    {MySqlDbType.String, "CHAR"},
		    {MySqlDbType.Text, "TEXT"},
		    {MySqlDbType.TinyText, "TINYTEXT"},
		    {MySqlDbType.MediumText, "MEDIUMTEXT"},
		    {MySqlDbType.LongText, "LONGTEXT"},
		    {MySqlDbType.Int32, "INT"},
		};

		public string DbTypeToString(MySqlDbType type, int? length)
		{
			string ret;
			if (TypesAsStrings.TryGetValue(type, out ret))
				return ret + (length != null ? "({0})".SFormat((int) length) : "");
			throw new NotImplementedException(Enum.GetName(typeof (MySqlDbType), type));
		}

		protected override string EscapeTableName(string table)
		{
			return table.SFormat("`{0}`", table);
		}
	}

	public abstract class GenericQueryCreator
	{
		protected static Random rand = new Random();
		protected abstract string EscapeTableName(string table);
		public abstract string CreateTable(SqlTable table);
		public abstract string RenameTable(string from, string to);

		/// <summary>
		/// Alter a table from source to destination
		/// </summary>
		/// <param name="from">Must have name and column names. Column types are not required</param>
		/// <param name="to">Must have column names and column types.</param>
		/// <returns></returns>
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

		public string DeleteRow(string table, List<SqlValue> wheres)
		{
			return "DELETE FROM {0}{1}".SFormat(EscapeTableName(table), BuildWhere(wheres));
		}

		public string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres)
		{
			if (0 == values.Count)
				throw new ArgumentException("No values supplied");

			return "UPDATE {0} SET {1}{2}".SFormat(EscapeTableName(table), string.Join(", ", values.Select(v => v.Name + " = " + v.Value)), BuildWhere(wheres));
		}

		public string ReadColumn(string table, List<SqlValue> wheres)
		{
			return "SELECT * FROM {0}{1}".SFormat(EscapeTableName(table), BuildWhere(wheres));
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

		protected static string BuildWhere(List<SqlValue> wheres)
		{
			if (0 == wheres.Count)
				return string.Empty;

			return "WHERE {0}".SFormat(string.Join(", ", wheres.Select(v => v.Name + " = " + v.Value)));
		}
	}
}
