using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
    public class ItemManager
    {
        private IDbConnection database;
        public List<string> ItemBans = new List<string>();

        public ItemManager(IDbConnection db)
        {
            database = db;

            var table = new SqlTable("ItemBans",
                new SqlColumn("ItemName", MySqlDbType.VarChar, 50) { Primary = true }
            );
            var creator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            creator.EnsureExists(table);

            String file = Path.Combine(TShock.SavePath, "itembans.txt");
            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Equals("") && !line.Substring(0, 1).Equals("#"))
                        {

                            string query = (TShock.Config.StorageType.ToLower() == "sqlite") ?
                                "INSERT OR IGNORE INTO 'ItemBans' (ItemName) VALUES (@0);" :
                                "INSERT IGNORE INTO ItemBans SET ItemName=@0;";

                            int id = 0;
                            int.TryParse(line, out id);

                            database.Query(query, TShock.Utils.GetItemById(id).name);
                        }
                    }
                }

                String path = Path.Combine(TShock.SavePath, "old_configs");
                String file2 = Path.Combine(path, "itembans.txt");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (File.Exists(file2))
                    File.Delete(file2);
                File.Move(file, file2);
            }

            UpdateItemBans();
        }

        public void UpdateItemBans()
        {
            ItemBans.Clear();

            using (var reader = database.QueryReader("SELECT * FROM ItemBans"))
            {
                while (reader != null && reader.Read())
                    ItemBans.Add(reader.Get<string>("ItemName"));
            }
        }
        public void AddNewBan(string itemname = "")
        {
            try
            {
                database.Query("INSERT INTO ItemBans (ItemName) VALUES (@0);", TShock.Utils.GetItemByName(itemname)[0].name);
                if (!ItemIsBanned(itemname))
                    ItemBans.Add(itemname);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void RemoveBan(string itemname)
        {
            if (!ItemIsBanned(itemname))
                return;
            try
            {
                database.Query("Delete FROM 'ItemBans' WHERE ItemName=@0;", TShock.Utils.GetItemByName(itemname)[0].name);
                ItemBans.Remove(itemname);
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
