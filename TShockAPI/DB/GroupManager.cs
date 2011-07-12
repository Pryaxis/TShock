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

        public List<Group> groups = new List<Group>();

        public GroupManager(IDbConnection db)
        {
            database = db;

            using (var com = database.CreateCommand())
            {
                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS 'GroupList' ('GroupName' TEXT UNIQUE, 'Commands' TEXT, 'OrderBy' TEXT);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS GroupList (GroupName VARCHAR(255) UNIQUE, Commands VARCHAR(255), OrderBy VARCHAR(255));";

                com.ExecuteNonQuery();

                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText = "INSERT OR IGNORE INTO GroupList (GroupName, Commands, OrderBy) VALUES (@groupname, @commands, @order);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText = "INSERT IGNORE INTO GroupList SET GroupName=@groupname, Commands=@commands, OrderBy=@order;";
                com.AddParameter("@groupname", "trustedadmin");
                com.AddParameter("@commands", "maintenance,cfg,butcher,cheat,immunetoban,ignorecheatdetection,ignoregriefdetection,usebanneditem");
                com.AddParameter("@order", "1");
                com.ExecuteNonQuery();
                com.Parameters.Clear();

                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText = "INSERT OR IGNORE INTO GroupList (GroupName, Commands, OrderBy) VALUES (@groupname, @commands, @order);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText = "INSERT IGNORE INTO GroupList SET GroupName=@groupname, Commands=@commands, OrderBy=@order;";
                com.AddParameter("@groupname", "admin");
                com.AddParameter("@commands", "ban,unban,whitelist,causeevents,spawnboss,spawnmob,managewarp,time,tp,pvpfun,kill,logs,immunetokick,tphere");
                com.AddParameter("@order", "2");
                com.ExecuteNonQuery();
                com.Parameters.Clear();

                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText = "INSERT OR IGNORE INTO GroupList (GroupName, Commands, OrderBy) VALUES (@groupname, @commands, @order);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText = "INSERT IGNORE INTO GroupList SET GroupName=@groupname, Commands=@commands, OrderBy=@order;";
                com.AddParameter("@groupname", "newadmin");
                com.AddParameter("@commands", "kick,editspawn,reservedslot");
                com.AddParameter("@order", "3");
                com.ExecuteNonQuery();
                com.Parameters.Clear();

                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText = "INSERT OR IGNORE INTO GroupList (GroupName, Commands, OrderBy) VALUES (@groupname, @commands, @order);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText = "INSERT IGNORE INTO GroupList SET GroupName=@groupname, Commands=@commands, OrderBy=@order;";
                com.AddParameter("@groupname", "default");
                com.AddParameter("@commands", "canwater,canlava,warp,manageusers");
                com.AddParameter("@order", "4");
                com.ExecuteNonQuery();
                com.Parameters.Clear();

                LoadPermisions();
            }
        }

        public bool GroupExists(string group)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Grouplist WHERE GroupName=@groupname";
                    com.AddParameter("@groupname", group);
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                            if (reader.Get<string>("GroupName") == group)
                                return true;
                    }
                }
            }
            catch (SqliteExecutionException ex)
            {
                Log.Error(ex.ToString());
            }
            return false;
        }

        public void LoadPermisions()
        {
            groups = new List<Group>();
            groups.Add(new SuperAdminGroup());

            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Grouplist;";
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Group group = null;
                            string groupname = reader.Get<string>("GroupName");
                            int order = Int32.Parse(reader.Get<string>("OrderBy"));
                            group = new Group(groupname);
                            group.Order = order;

                            //Inherit Given commands
                            foreach (string perm in reader.Get<string>("Commands").Split(','))
                            {
                                group.AddPermission(perm);
                            }

                            groups.Add(group);
                        }
                    }

                    //Inherit all commands from group below in order, unless Order is 0 (unique groups anyone)
                    foreach (Group group in groups)
                    {
                        if (group.Order != 0 && group.Order < groups.Count)
                        {
                            for (int i = group.Order + 1; i < groups.Count; i++)
                            {
                                for (int j = 0; j < groups[i].permissions.Count; j++)
                                {
                                    group.AddPermission(groups[i].permissions[j]);
                                }
                            }
                        }
                    }
                }
            }
            catch (SqliteExecutionException ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
