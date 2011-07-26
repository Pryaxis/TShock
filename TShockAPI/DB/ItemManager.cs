using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;
using TShockAPI.DB;
using System.IO;

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
                        "CREATE TABLE IF NOT EXISTS ItemBans (ItemName VARCHAR(255), PRIMARY KEY (`ItemName`));";
                com.ExecuteNonQuery();

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
                                if (TShock.Config.StorageType.ToLower() == "sqlite")
                                    com.CommandText = "INSERT OR IGNORE INTO 'ItemBans' (ItemName) VALUES (@name);";
                                else if (TShock.Config.StorageType.ToLower() == "mysql")
                                    com.CommandText = "INSERT IGNORE INTO ItemBans SET ItemName=@name;";

                                int id = 0;
                                int.TryParse(line, out id);
                                com.AddParameter("@name", Tools.GetItemById(id).name);
                                com.ExecuteNonQuery();
                                com.Parameters.Clear();
                            }
                        }
                    }

                    String path = Path.Combine(TShock.SavePath, "old_configs");
                    String file2 = Path.Combine(path, "itembans.txt");
                    if (!Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    if (File.Exists(file2))
                        File.Delete(file2);
                    File.Move(file, file2);
                }
            }

            UpdateItemBans();
        }

        public void UpdateItemBans()
        {
            ItemBans.Clear();
            using (var com = database.CreateCommand())
            {
                com.CommandText = "SELECT * FROM ItemBans";

                using (var reader = com.ExecuteReader())
                {
                    while (reader!=null&&reader.Read())
                        ItemBans.Add(reader.Get<string>("ItemName"));
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
                    if( !ItemIsBanned( itemname ) )
                        ItemBans.Add(itemname);
                }
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
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "Delete FROM 'ItemBans' WHERE ItemName=@itemname;";
                    com.AddParameter("@itemname", Tools.GetItemByName(itemname)[0].name);
                    com.ExecuteNonQuery();
                    ItemBans.Remove(itemname);
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
