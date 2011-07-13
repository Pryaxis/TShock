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
        public List<string> ItemBans = new List<string>();

        public ItemManager(IDbConnection db)
        {
            database = db;

            using (var com = database.CreateCommand())
            {
                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS 'ItemBans' ('ItemName' TEXT PRIMARY KEY);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS ItemBans (ItemName VARCHAR(255) PRIMARY);";
                com.ExecuteNonQuery();

                com.CommandText = "SELECT *FROM ItemBans";

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                        ItemBans.Add(reader.Get<string>("ItemName"));

                    reader.Close();
                }
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
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
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
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public bool ItemIsBanned(string name)
        {
            if (ItemBans.Contains(name))
                return true;

            return false;
        }
    }
}
