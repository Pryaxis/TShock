using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
    public class SqlTable
    {
        public List<SqlColumn> Columns { get; protected set; }
        public string Name { get; protected set; }
        public SqlTable(string name, params SqlColumn[] columns)
            : this(name, new List<SqlColumn>(columns))
        {
        }
        public SqlTable(string name, List<SqlColumn> columns)
        {
            Name = name;
            Columns = columns;
        }
    }

    public class SqlTableCreator
    {
        IDbConnection database;
        IQueryBuilder creator;
        public SqlTableCreator(IDbConnection db, IQueryBuilder provider)
        {
            database = db;
            creator = provider;
        }

        public void EnsureExists(SqlTable table)
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
                database.Query(creator.CreateTable(table));
            }
        }

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
                using (var reader = database.QueryReader("SELECT COLUMN_NAME FROM information_schema.COLUMNS WHERE TABLE_NAME=@0 AND TABLE_SCHEMA=@1", table.Name, database.Database))
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

        public void DeleteRow(string table, List<SqlValue> wheres)
        {
            database.Query(creator.DeleteRow(table, wheres));
        }
    }
}
