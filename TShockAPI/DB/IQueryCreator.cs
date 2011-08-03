using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.DB
{
    public interface IQuery
    {
        string CreateTable(SqlTable table);
    }

    public class SqliteQuery : IQuery
    {
        public string CreateTable(SqlTable table)
        {
            var columns = table.Columns.Select(c => "'{0}' {1} {2} {3} {4}".SFormat(c.Name, c.Type, c.Primary ? "PRIMARY KEY" : "", c.AutoIncrement ? "AUTOINCREMENT" : "", c.NotNull ? "NOT NULL" : "", c.Unique ? "UNIQUE" : ""));
            return "CREATE TABLE '{0}' ({1})".SFormat(table.Name, string.Join(", ", columns));
        }
    }
    public class MysqlQuery : IQuery
    {
        public string CreateTable(SqlTable table)
        {
            throw new NotImplementedException();
        }
    }
}
