using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI.Extensions;

namespace TShockAPI.DB
{
    public interface IQueryBuilder
    {
        string CreateTable(SqlTable table);
        string AlterTable(SqlTable from, SqlTable to);
    }

    public class SqliteQueryCreator : IQueryBuilder
    {
        public string CreateTable(SqlTable table)
        {
            var columns = table.Columns.Select(c => "'{0}' {1} {2} {3} {4}".SFormat(c.Name, c.Type, c.Primary ? "PRIMARY KEY" : "", c.AutoIncrement ? "AUTOINCREMENT" : "", c.NotNull ? "NOT NULL" : "", c.Unique ? "UNIQUE" : ""));
            return "CREATE TABLE '{0}' ({1})".SFormat(table.Name, string.Join(", ", columns));
        }
        static Random rand = new Random();
        public string AlterTable(SqlTable from, SqlTable to)
        {
            return "";
            /*
                ALTER TABLE "main"."Bans" RENAME TO "oXHFcGcd04oXHFcGcd04_Bans"
                CREATE TABLE "main"."Bans" ("IP" TEXT PRIMARY KEY ,"Name" TEXT)
                INSERT INTO "main"."Bans" SELECT "IP","Name" FROM "main"."oXHFcGcd04oXHFcGcd04_Bans"
                DROP TABLE "main"."oXHFcGcd04oXHFcGcd04_Bans"
             */
        }
    }
    public class MysqlQueryCreator : IQueryBuilder
    {
        public string CreateTable(SqlTable table)
        {
            throw new NotImplementedException();
        }


        public string AlterTable(SqlTable from, SqlTable to)
        {
            throw new NotImplementedException();
        }
    }
}
