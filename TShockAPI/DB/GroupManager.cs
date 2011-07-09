using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;

namespace TShockAPI.DB
{
    public class GroupManager
    {
        private IDbConnection database;

        public GroupManager(IDbConnection db)
        {
            database = db;
        }

        public bool GroupExists(string group)
        {
            return true;
        }
    }
}
