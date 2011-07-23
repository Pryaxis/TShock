using System;
using System.IO;
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
                        "CREATE TABLE IF NOT EXISTS 'GroupList' ('GroupName' TEXT PRIMARY KEY, 'Commands' TEXT, 'OrderBy' TEXT);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS GroupList (GroupName VARCHAR(255) PRIMARY, Commands VARCHAR(255), OrderBy VARCHAR(255));";

                com.ExecuteNonQuery();
            }

            //Add default groups
            AddGroup("trustedadmin", "admin,maintenance,cfg,butcher,item,heal,immunetoban,ignorecheatdetection,ignoregriefdetection,usebanneditem,manageusers");
            AddGroup("admin", "newadmin,ban,unban,whitelist,causeevents,spawnboss,spawnmob,managewarp,time,tp,pvpfun,kill,logs,immunetokick,tphere");
            AddGroup("newadmin", "default,kick,editspawn,reservedslot");
            AddGroup("default", "canwater,canlava,warp,canbuild");
            AddGroup("vip", "default,canwater,canlava,warp,canbuild,reservedslot");


            String file = Path.Combine(TShock.SavePath, "groups.txt");
            if (File.Exists(file))
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (!line.Equals("") && !line.Substring(0, 1).Equals("#"))
                        {
                            String[] info = line.Split(' ');
                            String comms = "";
                            int size = info.Length;
                            int test = 0;
                            bool hasOrder = int.TryParse(info[info.Length - 1], out test);
                            if (hasOrder)
                                size = info.Length - 1;
                            for (int i = 1; i < size; i++)
                            {
                                if (!comms.Equals(""))
                                    comms = comms + ",";
                                comms = comms + info[i].Trim();
                            }
                            using (var com = database.CreateCommand())
                            {
                                if (TShock.Config.StorageType.ToLower() == "sqlite")
                                    com.CommandText = "INSERT OR IGNORE INTO GroupList (GroupName, Commands, OrderBy) VALUES (@groupname, @commands, @order);";
                                else if (TShock.Config.StorageType.ToLower() == "mysql")
                                    com.CommandText = "INSERT IGNORE INTO GroupList SET GroupName=@groupname, Commands=@commands, OrderBy=@order;";

                                com.AddParameter("@groupname", info[0].Trim());
                                com.AddParameter("@commands", comms);
                                com.AddParameter("@order", hasOrder ? info[info.Length - 1] : "0");
                                com.ExecuteNonQuery();
                            }
                        }
                    }
                }
                String path = Path.Combine(TShock.SavePath, "old_configs");
                String file2 = Path.Combine(path, "groups.txt");
                if (!Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
                if (File.Exists(file2))
                    File.Delete(file2);
                File.Move(file, file2);
            }

        }

        /// <summary>
        /// Adds group with name and permissions if it does not exist.
        /// </summary>
        /// <param name="name">name of group</param>
        /// <param name="commands">permissions</param>
        public void AddGroup(string name, string commands)
        {
            using (var com = database.CreateCommand())
            {
                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText =
                        "INSERT OR IGNORE INTO GroupList (GroupName, Commands, OrderBy) VALUES (@groupname, @commands, @order);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText =
                        "INSERT IGNORE INTO GroupList SET GroupName=@groupname, Commands=@commands, OrderBy=@order;";
                com.AddParameter("@groupname", name);
                com.AddParameter("@commands", commands);
                com.AddParameter("@order", "0");
                com.ExecuteNonQuery();
            }
        }

        public bool GroupExists(string group)
        {
            if (group == "superadmin")
                return true;


            foreach (Group g in groups)
            {
                if (g.Name.Equals(group))
                    return true;
            }

            return false;
        }

        public void LoadPermisions()
        {
            groups = new List<Group>();
            groups.Add(new SuperAdminGroup());

            if (TShock.Users == null)
            {
                TShock.Users = new UserManager(TShock.DB);
            }

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
                            string groupname = reader.Get<String>("GroupName");
                            group = new Group(groupname);

                            //Inherit Given commands
                            String[] commands = reader.Get<String>("Commands").Split(',');
                            for (int i = 0; i < commands.Length; i++)
                            {
                                group.AddPermission(commands[i].Trim());
                            }
                            groups.Add(group);
                        }
                    }
                    /** ORDER BY IS DUMB
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
                    }*/
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
