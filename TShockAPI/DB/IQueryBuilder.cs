using System;
using System.Collections.Generic;
using System.Data;
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
    }

    public class SqliteQueryCreator : IQueryBuilder
    {
        public string CreateTable(SqlTable table)
        {
            var columns = table.Columns.Select(c => "'{0}' {1} {2} {3} {4}".SFormat(c.Name, DbTypeToString(c.Type, c.Length), c.Primary ? "PRIMARY KEY" : "", c.AutoIncrement ? "AUTOINCREMENT" : "", c.NotNull ? "NOT NULL" : "", c.Unique ? "UNIQUE" : ""));
            return "CREATE TABLE '{0}' ({1})".SFormat(table.Name, string.Join(", ", columns));
        }
        static Random rand = new Random();
        /// <summary>
        /// Alter a table from source to destination
        /// </summary>
        /// <param name="from">Must have name and column names. Column types are not required</param>
        /// <param name="to">Must have column names and column types.</param>
        /// <returns></returns>
        public string AlterTable(SqlTable from, SqlTable to)
        {
            var rstr = rand.NextString(20);
            var alter = "ALTER TABLE '{0}' RENAME TO '{1}_{0}'".SFormat(from.Name, rstr);
            var create = CreateTable(to);
            //combine all columns in the 'from' variable excluding ones that aren't in the 'to' variable.
            //exclude the ones that aren't in 'to' variable because if the column is deleted, why try to import the data?
            var insert = "INSERT INTO '{0}' ({1}) SELECT {1} FROM {2}_{0}".SFormat(from.Name, string.Join(", ", from.Columns.Where(c => to.Columns.Any(c2 => c2.Name == c.Name)).Select(c => c.Name)), rstr);
            var drop = "DROP TABLE '{0}_{1}'".SFormat(rstr, from.Name);
            return "{0}; {1}; {2}; {3};".SFormat(alter, create, insert, drop);
            /*
                ALTER TABLE "main"."Bans" RENAME TO "oXHFcGcd04oXHFcGcd04_Bans"
                CREATE TABLE "main"."Bans" ("IP" TEXT PRIMARY KEY ,"Name" TEXT)
                INSERT INTO "main"."Bans" SELECT "IP","Name" FROM "main"."oXHFcGcd04oXHFcGcd04_Bans"
                DROP TABLE "main"."oXHFcGcd04oXHFcGcd04_Bans"
             * 
             * Twitchy - Oh. I get it!
             */
        }
        public string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres)
        {
            var sbvalues = new StringBuilder();
            var sbwheres = new StringBuilder();
            int count = 0;
            foreach (SqlValue value in values)
            {
                sbvalues.Append(value.Name + "=" + value.Value.ToString());
                if (count != values.Count - 1)
                    sbvalues.Append(",");
                count++;
            }
            count = 0;
            foreach (SqlValue where in wheres)
            {
                sbwheres.Append(where.Name + "=" + where.Value.ToString());
                if (count != wheres.Count - 1)
                    sbvalues.Append(" AND ");
                count++;
            }

            if (wheres.Count > 0)
                return "UPDATE '{0}' SET {1} WHERE {2}".SFormat(table, sbvalues.ToString(), sbwheres.ToString());
            else
                return "UPDATE '{0}' SET {1}".SFormat(table, sbvalues.ToString());
        }
        public string InsertValues(string table, List<SqlValue> values)
        {
            var sbnames = new StringBuilder();
            var sbvalues = new StringBuilder();
            int count = 0;

            foreach (SqlValue name in values)
            {
                sbnames.Append(name.Name);

                if (count != values.Count - 1)
                    sbnames.Append(", ");
                count++;
            }
            count = 0;
            foreach (SqlValue value in values)
            {
                sbvalues.Append(value.Value.ToString());
                if (count != values.Count - 1)
                    sbvalues.Append(", ");
                count++;
            }

            return "INSERT INTO '{0}' ({1}) VALUES ({2})".SFormat(table, sbnames.ToString(), sbvalues.ToString());
        }
        public string ReadColumn(string table, List<SqlValue> wheres)
        {
            var sbwheres = new StringBuilder();
            int count = 0;

            foreach (SqlValue where in wheres)
            {
                sbwheres.Append(where.Name + "=" + where.Value.ToString());
                if (count != wheres.Count - 1)
                    sbwheres.Append(" AND ");
                count++;
            }

            if(wheres.Count > 0)
                return "SELECT * FROM {0} WHERE {1}".SFormat(table, sbwheres.ToString());
            else
                return "SELECT * FROM {0}".SFormat(table);
        }

        static readonly Dictionary<MySqlDbType, string> TypesAsStrings = new Dictionary<MySqlDbType, string>
        {
            {MySqlDbType.VarChar, "TEXT"},
            {MySqlDbType.String, "TEXT"},
            {MySqlDbType.Text, "TEXT"},
            {MySqlDbType.TinyText, "TEXT"},
            {MySqlDbType.MediumText, "TEXT"},
            {MySqlDbType.LongText, "TEXT"},
            {MySqlDbType.Int32, "INTEGER"},
        };
        public string DbTypeToString(MySqlDbType type, int? length)
        {
            string ret;
            if (TypesAsStrings.TryGetValue(type, out ret))
                return ret;
            throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
        }
    }
    public class MysqlQueryCreator : IQueryBuilder
    {
        public string CreateTable(SqlTable table)
        {
            var columns = table.Columns.Select(c => "{0} {1} {2} {3}".SFormat(c.Name, DbTypeToString(c.Type, c.Length), c.Primary ? "PRIMARY KEY" : "", c.AutoIncrement ? "AUTO_INCREMENT" : "", c.NotNull ? "NOT NULL" : ""));
            var uniques = table.Columns.Where(c => c.Unique).Select(c => c.Name);
            return "CREATE TABLE {0} ({1} {2})".SFormat(table.Name, string.Join(", ", columns), uniques.Count() > 0 ? ", UNIQUE({0})".SFormat(string.Join(", ", uniques)) : "");
        }
        static Random rand = new Random();
        /// <summary>
        /// Alter a table from source to destination
        /// </summary>
        /// <param name="from">Must have name and column names. Column types are not required</param>
        /// <param name="to">Must have column names and column types.</param>
        /// <returns></returns>
        public string AlterTable(SqlTable from, SqlTable to)
        {
            var rstr = rand.NextString(20);
            var alter = "RENAME TABLE {0} TO {1}_{0}".SFormat(from.Name, rstr);
            var create = CreateTable(to);
            //combine all columns in the 'from' variable excluding ones that aren't in the 'to' variable.
            //exclude the ones that aren't in 'to' variable because if the column is deleted, why try to import the data?
            var insert = "INSERT INTO {0} ({1}) SELECT {1} FROM {2}_{0}".SFormat(from.Name, string.Join(", ", from.Columns.Where(c => to.Columns.Any(c2 => c2.Name == c.Name)).Select(c => c.Name)), rstr);
            var drop = "DROP TABLE {0}_{1}".SFormat(rstr, from.Name);
            return "{0}; {1}; {2}; {3};".SFormat(alter, create, insert, drop);
        }
        public string UpdateValue(string table, List<SqlValue> values, List<SqlValue> wheres)
        {
            var sbvalues = new StringBuilder();
            var sbwheres = new StringBuilder();
            int count = 0;
            foreach (SqlValue value in values)
            {
                sbvalues.Append(value.Name + "=" + value.Value.ToString());
                if (count != values.Count - 1)
                    sbvalues.Append("AND");
                count++;
            }
            count = 0;
            foreach (SqlValue where in wheres)
            {
                sbwheres.Append(where.Name + "=" + where.Value.ToString());
                if (count != wheres.Count - 1)
                    sbvalues.Append(" AND ");
                count++;
            }

            if (wheres.Count > 0)
                return "UPDATE {0} SET {1} WHERE {2}".SFormat(table, sbvalues.ToString(), sbwheres.ToString());
            else
                return "UPDATE {0} SET {1}".SFormat(table, sbvalues.ToString());
        }
        public string InsertValues(string table, List<SqlValue> values)
        {
            var sbnames = new StringBuilder();
            var sbvalues = new StringBuilder();
            int count = 0;

            foreach (SqlValue name in values)
            {
                sbnames.Append(name.Name);

                if (count != values.Count - 1)
                    sbnames.Append(", ");
                count++;
            }
            count = 0;
            foreach (SqlValue value in values)
            {
                sbvalues.Append(value.Value.ToString());
                if (count != values.Count - 1)
                    sbvalues.Append(", ");
                count++;
            }

            return "INSERT INTO {0} ({1}) VALUES ({2})".SFormat(table, sbnames.ToString(), sbvalues.ToString());
        }
        public string ReadColumn(string table, List<SqlValue> wheres)
        {
            var sbwheres = new StringBuilder();
            int count = 0;

            foreach (SqlValue where in wheres)
            {
                sbwheres.Append(where.Name + "=" + where.Value.ToString());
                if (count != wheres.Count - 1)
                    sbwheres.Append(" AND ");
                count++;
            }

            if (wheres.Count > 0)
                return "SELECT * FROM {0} WHERE {1}".SFormat(table, sbwheres.ToString());
            else
                return "SELECT * FROM {0}".SFormat(table);
        }

        static readonly Dictionary<MySqlDbType, string> TypesAsStrings = new Dictionary<MySqlDbType, string>
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
                return ret + (length != null ? "({0})".SFormat((int)length) : "");
            throw new NotImplementedException(Enum.GetName(typeof(MySqlDbType), type));
        }
    }
}
