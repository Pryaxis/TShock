using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
    public class ItemManager
    {
        private IDbConnection database;
		public List<ItemBan> ItemBans = new List<ItemBan>();

        public ItemManager(IDbConnection db)
        {
            database = db;

            var table = new SqlTable("ItemBans",
                new SqlColumn("ItemName", MySqlDbType.VarChar, 50) { Primary = true },
				new SqlColumn("AllowedGroups", MySqlDbType.Text )
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
                                "INSERT OR IGNORE INTO 'ItemBans' (ItemName, AllowedGroups) VALUES (@0, @1);" :
								"INSERT IGNORE INTO ItemBans SET ItemName=@0,AllowedGroups=@1 ;";

                            int id = 0;
                            int.TryParse(line, out id);

                            database.Query(query, TShock.Utils.GetItemById(id).name, "");
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
            	{
					ItemBan ban = new ItemBan(reader.Get<string>("ItemName"));
					ban.SetAllowedGroups( reader.Get<string>("AllowedGroups") );
					ItemBans.Add(ban);
            	}
            }
        }

        public void AddNewBan(string itemname = "")
        {
            try
            {
                database.Query("INSERT INTO ItemBans (ItemName, AllowedGroups) VALUES (@0, @1);", TShock.Utils.GetItemByName(itemname)[0].name, "");
                if (!ItemIsBanned(itemname, null))
                    ItemBans.Add( new ItemBan(itemname) );
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public void RemoveBan(string itemname)
        {
            if (!ItemIsBanned(itemname, null))
                return;
            try
            {
                database.Query("Delete FROM 'ItemBans' WHERE ItemName=@0;", TShock.Utils.GetItemByName(itemname)[0].name);
                ItemBans.Remove( new ItemBan(itemname) );
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        public bool ItemIsBanned(string name)
        {
            if (ItemBans.Contains(new ItemBan(name)))
            {
                return true;
            }
            return false;
        }

        public bool ItemIsBanned(string name, TSPlayer ply)
        {
			if (ItemBans.Contains( new ItemBan(name) ) )
			{
				ItemBan b = GetItemBanByName(name);
				return !b.HasPermissionToUseItem(ply);
			}
			return false;
        }

		public bool AllowGroup(string item, string name)
		{
			string groupsNew = "";
			ItemBan b = GetItemBanByName(item);
			if (b != null)
			{
				groupsNew = String.Join(",", b.AllowedGroups);
				if (groupsNew.Length > 0)
					groupsNew += ",";
				groupsNew += name;
				b.SetAllowedGroups(groupsNew);

				int q = database.Query("UPDATE ItemBans SET AllowedGroups=@0 WHERE ItemName=@1", groupsNew,
									   item);

				return q > 0;
			}

			return false;
		}

		public bool RemoveGroup(string item, string group)
		{
			ItemBan b = GetItemBanByName(item);
			if (b != null)
			{
				b.RemoveGroup(group);
				string groups = string.Join(",", b.AllowedGroups);
				int q = database.Query("UPDATE ItemBans SET AllowedGroups=@0 WHERE ItemName=@1", groups,
									   item);
				if (q > 0)
					return true;
			}
			return false;
		}

		public ItemBan GetItemBanByName(String name)
		{
			foreach (ItemBan b in ItemBans)
			{
				if (b.Name == name)
				{
					return b;
				}
			}
			return null;
		}
    }

	public class ItemBan : IEquatable<ItemBan>
    {
        public string Name { get; set; }
		public List<string> AllowedGroups { get; set; }

        public ItemBan(string name)
            : this()
        {
            Name = name;
			AllowedGroups = new List<string>();
        }

        public ItemBan()
        {
            Name = "";
			AllowedGroups = new List<string>();
        }

		public bool Equals(ItemBan other)
		{
			return Name == other.Name;
		}

        public bool HasPermissionToUseItem(TSPlayer ply)
        {
			if (ply == null)
				return false;
			Console.WriteLine(ply.Group.Name);
			return AllowedGroups.Contains(ply.Group.Name); // could add in the other permissions in this class instead of a giant if switch.
        }
		
		public void SetAllowedGroups( String groups )
		{
				// prevent null pointer exceptions
				if (!string.IsNullOrEmpty(groups))
				{
					List<String> groupArr = groups.Split(',').ToList();

					for (int i = 0; i < groupArr.Count; i++)
					{
						groupArr[i] = groupArr[i].Trim();
						Console.WriteLine(groupArr[i]);
					}
					AllowedGroups = groupArr;
				}
		}

		public bool  RemoveGroup(string groupName)
		{
			return AllowedGroups.Remove(groupName);
		}
    }

}
