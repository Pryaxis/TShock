using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
    public class GroupManager
    {
        private IDbConnection database;

        public List<Group> groups = new List<Group>();

        public GroupManager(IDbConnection db)
        {
            database = db;

            var table = new SqlTable("GroupList",
                new SqlColumn("GroupName", MySqlDbType.VarChar, 32) { Primary = true },
                new SqlColumn("Parent", MySqlDbType.VarChar, 32),
                new SqlColumn("Commands", MySqlDbType.Text),
                new SqlColumn("ChatColor", MySqlDbType.Text)
            );
            var creator = new SqlTableCreator(db, db.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            creator.EnsureExists(table);

            //Add default groups
            AddGroup("default", "canwater,canlava,warp,canbuild");
            AddGroup("newadmin", "default", "kick,editspawn,reservedslot");
            AddGroup("admin", "newadmin", "ban,unban,whitelist,causeevents,spawnboss,spawnmob,managewarp,time,tp,pvpfun,kill,logs,immunetokick,tphere");
            AddGroup("trustedadmin", "admin", "maintenance,cfg,butcher,item,heal,immunetoban,ignorecheatdetection,ignoregriefdetection,usebanneditem,manageusers");
            AddGroup("vip", "default", "canwater,canlava,warp,canbuild,reservedslot");

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
                            for (int i = 1; i < size; i++)
                            {
                                if (!comms.Equals(""))
                                    comms = comms + ",";
                                comms = comms + info[i].Trim();
                            }

                            string query = "";
                            if (TShock.Config.StorageType.ToLower() == "sqlite")
                                query = "INSERT OR IGNORE INTO GroupList (GroupName, Commands) VALUES (@0, @1);";
                            else if (TShock.Config.StorageType.ToLower() == "mysql")
                                query = "INSERT IGNORE INTO GroupList SET GroupName=@0, Commands=@1;";

                            db.Query(query, info[0].Trim(), comms);

                        }
                    }
                }
                String path = Path.Combine(TShock.SavePath, "old_configs");
                String file2 = Path.Combine(path, "groups.txt");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (File.Exists(file2))
                    File.Delete(file2);
                File.Move(file, file2);
            }

        }


        public bool GroupExists(string group)
        {
            if (group == "superadmin")
                return true;


            return groups.Any(g => g.Name.Equals(group));
        }

        /// <summary>
        /// Adds group with name and permissions if it does not exist.
        /// </summary>
        /// <param name="name">name of group</param>
        /// <param name="parentname">parent of group</param>
        /// <param name="permissions">permissions</param>
        public String AddGroup(String name, string parentname, String permissions, String chatcolor)
        {
            String message = "";
            if (GroupExists(name))
                return "Error: Group already exists.  Use /modGroup to change permissions.";

            var group = new Group(name, null, chatcolor);
            group.permissions.Add(permissions);
            if (!string.IsNullOrWhiteSpace(parentname))
            {
                var parent = groups.FirstOrDefault(gp => gp.Name == parentname);
                if (parent == null)
                {
                    var error = "Invalid parent {0} for group {1}".SFormat(group.Name, parentname);
                    Log.ConsoleError(error);
                    return error;
                }
                group.Parent = parent;
            }

            string query = (TShock.Config.StorageType.ToLower() == "sqlite") ?
                "INSERT OR IGNORE INTO GroupList (GroupName, Parent, Commands, ChatColor) VALUES (@0, @1, @2, @3);" :
                "INSERT IGNORE INTO GroupList SET GroupName=@0, Parent=@1, Commands=@1, ChatColor=@1";
            if (database.Query(query, name, parentname, permissions, chatcolor) == 1)
                message = "Group " + name + " has been created successfully.";

            groups.Add(group);

            return message;
        }
        public String AddGroup(String name, String permissions)
        {
            return AddGroup(name, "", permissions, "255,255,255");
        }
        public String AddGroup(String name, string parent, String permissions)
        {
            return AddGroup(name, parent, permissions, "255,255,255");
        }

        public String DeleteGroup(String name)
        {
            String message = "";
            if (!GroupExists(name))
                return "Error: Group doesn't exists.";

            if (database.Query("DELETE FROM GroupList WHERE GroupName=@0", name) == 1)
                message = "Group " + name + " has been deleted successfully.";
            groups.Remove(Tools.GetGroup(name));

            return message;
        }

        public String AddPermissions(String name, List<String> permissions)
        {
            String message = "";
            if (!GroupExists(name))
                return "Error: Group doesn't exists.";

            var group = Tools.GetGroup(name);
            //Add existing permissions (without duplicating)
            permissions.AddRange(group.permissions.Where(s => !permissions.Contains(s)));

            if (database.Query("UPDATE GroupList SET Commands=@0 WHERE GroupName=@1", String.Join(",", permissions), name) != 0)
                message = "Group " + name + " has been modified successfully.";

            return message;
        }

        public String DeletePermissions(String name, List<String> permissions)
        {
            String message = "";
            if (!GroupExists(name))
                return "Error: Group doesn't exists.";

            var group = Tools.GetGroup(name);

            //Only get permissions that exist in the group.
            var newperms = permissions.Where(s => group.permissions.Contains(s));

            if (database.Query("UPDATE GroupList SET Commands=@0 WHERE GroupName=@1", String.Join(",", newperms), name) != 0)
                message = "Group " + name + " has been modified successfully.";

            return message;
        }

        public void LoadPermisions()
        {
            //Create a temporary list so if there is an error it doesn't override the currently loaded groups with broken groups.
            var tempgroups = new List<Group>();
            tempgroups.Add(new SuperAdminGroup());

            if (groups == null || groups.Count < 2)
                groups = tempgroups;

            try
            {
                var groupsparents = new List<Tuple<Group, string>>();
                using (var reader = database.QueryReader("SELECT * FROM GroupList"))
                {
                    while (reader.Read())
                    {
                        string groupname = reader.Get<String>("GroupName");
                        var group = new Group(groupname);

                        //Inherit Given commands
                        String[] commands = reader.Get<String>("Commands").Split(',');
                        foreach (var t in commands)
                        {
                            var str = t.Trim();
                            if (str.StartsWith("!"))
                            {
                                group.NegatePermission(str.Substring(1));
                            }
                            else
                            {
                                group.AddPermission(str);
                            }
                        }
                        String[] chatcolour = (reader.Get<String>("ChatColor") ?? "").Split(',');
                        if (chatcolour.Length == 3)
                        {
                            byte.TryParse(chatcolour[0], out group.R);
                            byte.TryParse(chatcolour[1], out group.G);
                            byte.TryParse(chatcolour[2], out group.B);
                        }

                        groupsparents.Add(Tuple.Create(group, reader.Get<string>("Parent")));
                    }
                }

                foreach (var t in groupsparents)
                {
                    var group = t.Item1;
                    var parentname = t.Item2;
                    if (!string.IsNullOrWhiteSpace(parentname))
                    {
                        var parent = groupsparents.FirstOrDefault(gp => gp.Item1.Name == parentname);
                        if (parent == null)
                        {
                            Log.ConsoleError("Invalid parent {0} for group {1}".SFormat(group.Name, parentname));
                            return;
                        }
                        group.Parent = parent.Item1;
                    }
                    tempgroups.Add(group);
                }


                groups = tempgroups;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
