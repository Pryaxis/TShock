using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace TShockAPI.DB
{
    public class GroupManager
    {
        private IDbConnection database;

        public List<Group> groups = new List<Group>();

        public GroupManager(IDbConnection db)
        {
            database = db;

            string query;
            if (TShock.Config.StorageType.ToLower() == "sqlite")
            {
                db.Query("CREATE TABLE IF NOT EXISTS 'GroupList' (GroupName TEXT PRIMARY KEY, Commands TEXT, 'OrderBy' TEXT);");
                query = "CREATE TEMPORARY TABLE GroupList_backup('GroupName' TEXT PRIMARY KEY, 'Commands' TEXT); INSERT INTO GroupList_backup SELECT 'GroupName','Commands' FROM GroupList; DROP TABLE GroupList; CREATE TABLE IF NOT EXISTS GroupList('GroupName' TEXT PRIMARY KEY, 'Commands' TEXT); INSERT INTO 'GroupList' SELECT 'GroupName','Commands' FROM GroupList_backup; DROP TABLE GroupList_backup;";
            }
            else
            {
                query =
                    "CREATE TABLE IF NOT EXISTS GroupList (GroupName VARCHAR(255) PRIMARY, Commands VARCHAR(255));";
                db.Query("ALTER TABLE GroupList DROP COLUMN OrderBy");
            }

            db.Query(query);

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
                            for (int i = 1; i < size; i++)
                            {
                                if (!comms.Equals(""))
                                    comms = comms + ",";
                                comms = comms + info[i].Trim();
                            }

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


            foreach (Group g in groups)
            {
                if (g.Name.Equals(group))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Adds group with name and permissions if it does not exist.
        /// </summary>
        /// <param name="name">name of group</param>
        /// <param name="permissions">permissions</param>
        public String AddGroup(String name, String permissions)
        {
            String message = "";
            if (GroupExists(name))
                return "Error: Group already exists.  Use /modGroup to change permissions.";

            string query = (TShock.Config.StorageType.ToLower() == "sqlite") ?
                "INSERT OR IGNORE INTO GroupList (GroupName, Commands) VALUES (@0, @1);" :
                "INSERT IGNORE INTO GroupList SET GroupName=@0, Commands=@1";
            if (database.Query(query, name, permissions) == 1)
                message = "Group " + name + " has been created successfully.";
            Group g = new Group(name);
            g.permissions.Add(permissions);
            groups.Add(g);

            return message;
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
            groups = new List<Group>();
            groups.Add(new SuperAdminGroup());

            try
            {

                using (var reader = database.QueryReader("SELECT * FROM Grouplist;"))
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
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
