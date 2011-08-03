using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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
        IQuery query;
        public SqlTableCreator(IDbConnection db, IQuery provider)
        {
            database = db;
            query = provider;
        }

        public void EnsureExists(SqlTable table)
        {
            var columns = GetColumns(table);
            if (columns.Count > 0)
            {
                if (table.Columns.All(c => columns.Contains(c.Name)))
                {
                    
                }
            }
            else
            {
                database.Query(query.CreateTable(table));
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
                throw new NotImplementedException();
            }
            else
            {
                throw new NotSupportedException();
            }

            return ret;
        }
    }
}
