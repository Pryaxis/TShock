using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;
using TShockAPI.DB;

namespace TShockAPI.DB
{
    public class ItemManager
    {
        private IDbConnection database;

        public ItemManager(IDbConnection db)
        {
            database = db;

            using (var com = database.CreateCommand())
            {
                com.CommandText =
                    "CREATE TABLE IF NOT EXISTS 'ItemBans' ('ItemName' TEXT UNIQUE);";
                com.ExecuteNonQuery();
            }
        }

        public void AddNewBan(string itemname = "")
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "INSERT INTO ItemBans (ItemName) VALUES (@itemname);";
                    com.AddParameter("@itemname", Tools.GetItemByName(itemname)[0].name);
                    com.ExecuteNonQuery();
                }
            }
            catch (SqliteExecutionException ex)
            {
            }
        }

        public void RemoveBan(string itemname)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "DELETE FROM ItemBans WHERE ItemName=@itemname;";
                    com.AddParameter("@itemname", Tools.GetItemByName(itemname)[0].name);
                    com.ExecuteNonQuery();
                }
            }
            catch (SqliteExecutionException ex)
            {
            }
        }

        public bool ItemIsBanned(string name)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT *FROM ItemBans WHERE ItemName=@name";
                    com.AddParameter("@name", name);
                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                            if (reader.Get<string>("ItemName") == name)
                                return true;
                    }
                }
            }
            catch (SqliteExecutionException ex)
            {
            }
            return false;
        }
    }
}
