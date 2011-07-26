
/*   
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Community.CsharpSqlite.SQLiteClient;
namespace TShockAPI.DB
{
    public class UserManager
    {
        private IDbConnection database;

        public UserManager(IDbConnection db)
        {
            database = db;

            using (var com = database.CreateCommand())
            {
                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS 'Users' ('ID' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE, 'Username' VARCHAR(32) UNIQUE, 'Password' VARCHAR(64), 'Usergroup' TEXT, 'IP' VARCHAR(32));";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS Users (ID INT UNSIGNED NOT NULL AUTO_INCREMENT, Username VARCHAR(32) UNIQUE, Password VARCHAR(64), Usergroup VARCHAR(255), IP VARCHAR(15), PRIMARY KEY (`ID`));";

                com.ExecuteNonQuery();

                String file = Path.Combine(TShock.SavePath, "users.txt");
                if (File.Exists(file))
                {
                    using (StreamReader sr = new StreamReader(file))
                    {
                        String line;
                        while ((line = sr.ReadLine()) != null )
                        {
                            if (line.Equals("") || line.Substring(0, 1).Equals("#") )
                                continue;
                            String[] info = line.Split(' ');
                            if (TShock.Config.StorageType.ToLower() == "sqlite")
                                com.CommandText = "INSERT OR IGNORE INTO Users (Username, Password, Usergroup, IP) VALUES (@name, @pass, @group, @ip);";
                            else if (TShock.Config.StorageType.ToLower() == "mysql")
                                com.CommandText = "INSERT IGNORE INTO Users SET Username=@name, Password=@pass, Usergroup=@group, IP=@ip ;";

                            String username = "";
                            String sha = "";
                            String group = "";
                            String ip = "";

                            String[] nameSha = info[0].Split(':');

                            if (nameSha.Length < 2)
                            {
                                username = nameSha[0];
                                ip = nameSha[0];
                                group = info[1];
                            }
                            else
                            {
                                username = nameSha[0];
                                sha = nameSha[1];
                                group = info[1];
                            }
                            com.AddParameter("@name", username.Trim());
                            com.AddParameter("@pass", sha.Trim());
                            com.AddParameter("@group", group.Trim());
                            com.AddParameter("@ip", ip.Trim());
                            com.ExecuteNonQuery();
                            com.Parameters.Clear();
                        }
                    }
                    String path = Path.Combine(TShock.SavePath, "old_configs");
                    String file2 = Path.Combine(path, "users.txt");
                    if (!Directory.Exists(path))
                        System.IO.Directory.CreateDirectory(path);
                    if (File.Exists(file2))
                        File.Delete(file2);
                    File.Move(file, file2);
                }
            }
        }

        /// <summary>
        /// Adds a given username to the database
        /// </summary>
        /// <param name="user">User user</param>
        public void AddUser(User user)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "INSERT INTO Users (Username, Password, UserGroup, IP) VALUES (@name, @password, @group, @ip);";
                    com.AddParameter("@name", user.Name);
                    com.AddParameter("@password", Tools.HashPassword(user.Password));

                    if (!TShock.Groups.GroupExists(user.Group))
                        throw new GroupNotExistsException(user.Group);

                    com.AddParameter("@group", user.Group);
                    com.AddParameter("@ip", user.Address);

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.RecordsAffected < 1)
                            throw new UserExistsException(user.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserManagerException("AddUser SQL returned an error", ex);
            }
        }

        /// <summary>
        /// Removes a given username from the database
        /// </summary>
        /// <param name="user">User user</param>
        public void RemoveUser(User user)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    if (!string.IsNullOrEmpty(user.Address))
                    {
                        com.CommandText = "DELETE FROM Users WHERE IP=@ip";
                        com.AddParameter("@ip", user.Address);
                    }
                    else
                    {
                        com.CommandText = "DELETE FROM Users WHERE Username=@name";
                        com.AddParameter("@name", user.Name);
                    }

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.RecordsAffected < 1)
                            throw new UserNotExistException(string.IsNullOrEmpty(user.Address) ? user.Name : user.Address);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserManagerException("RemoveUser SQL returned an error", ex);
            }
        }


        /// <summary>
        /// Sets the Hashed Password for a given username
        /// </summary>
        /// <param name="user">User user</param>
        /// <param name="group">string password</param>
        public void SetUserPassword(User user, string password)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "UPDATE Users SET Password = @password WHERE Username = @name;";
                    com.AddParameter("@name", user.Name);
                    com.AddParameter("@password", Tools.HashPassword(password));

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.RecordsAffected < 1)
                            throw new UserExistsException(user.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserManagerException("SetUserPassword SQL returned an error", ex);
            }
        }

        /// <summary>
        /// Sets the group for a given username
        /// </summary>
        /// <param name="user">User user</param>
        /// <param name="group">string group</param>
        public void SetUserGroup(User user, string group)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "UPDATE Users SET UserGroup = @group WHERE Username = @name;";
                    com.AddParameter("@name", user.Name);

                    if (!TShock.Groups.GroupExists(group))
                        throw new GroupNotExistsException(group);

                    com.AddParameter("@group", group);

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.RecordsAffected < 1)
                            throw new UserExistsException(user.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserManagerException("SetUserGroup SQL returned an error", ex);
            }
        }


        /// <summary>
        /// Fetches the hashed password and group for a given username
        /// </summary>
        /// <param name="username">string username</param>
        /// <returns>string[] {password, group}</returns>
        public string[] FetchHashedPasswordAndGroup(string username)
        {
            string[] returndata = new string[2];
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Users WHERE Username=@name";
                    com.AddParameter("@name", username.ToLower());

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            returndata[0] = reader.Get<string>("Password");
                            returndata[1] = reader.Get<string>("UserGroup");
                            return returndata;
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError("FetchHashedPasswordAndGroup SQL returned an error: " + ex.ToString());
            }
            return returndata;
        }

        public int GetUserID(string username)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Users WHERE Username=@name";
                    com.AddParameter("@name", username.ToLower());

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return reader.Get<int>("ID");
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError("FetchHashedPasswordAndGroup SQL returned an error: " + ex.ToString());
            }
            return -1;
        }

        /// <summary>
        /// Returns a Group for a ip from the database
        /// </summary>
        /// <param name="ply">string ip</param>
        public Group GetGroupForIP(string ip)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Users WHERE IP=@ip";
                    com.AddParameter("@ip", ip);

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string group = reader.Get<string>("UserGroup");
                            return Tools.GetGroup(group);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError("GetGroupForIP SQL returned an error: " + ex.ToString());
            }
            return Tools.GetGroup("default");
        }

        public Group GetGroupForIPExpensive(string ip)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "SELECT * FROM Users";

                    using (var reader = com.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            if (Tools.GetIPv4Address(reader.Get<string>("IP")) == ip)
                            {
                                string group = reader.Get<string>("UserGroup");
                                return Tools.GetGroup(group);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError("GetGroupForIP SQL returned an error: " + ex.ToString());
            }
            return Tools.GetGroup("default");
        }


        public User GetUserByName(string name)
        {
            try
            {
                return GetUser(new User { Name = name });
            }
            catch (UserManagerException)
            {
                return null;
            }
        }
        public User GetUserByIP(string ip)
        {
            try
            {
                return GetUser(new User { Address = ip });
            }
            catch (UserManagerException)
            {
                return null;
            }
        }
        public User GetUser(User user)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    if (string.IsNullOrEmpty(user.Address))
                    {
                        com.CommandText = "SELECT * FROM Users WHERE Username=@name";
                        com.AddParameter("@name", user.Name);
                    }
                    else
                    {
                        com.CommandText = "SELECT * FROM Users WHERE IP=@ip";
                        com.AddParameter("@ip", user.Address);
                    }

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user.ID = reader.Get<int>("ID");
                            user.Group = reader.Get<string>("Usergroup");
                            return user;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserManagerException("GetUserID SQL returned an error", ex);
            }
            throw new UserNotExistException(string.IsNullOrEmpty(user.Address) ? user.Name : user.Address);
        }
    }

    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Group { get; set; }
        public string Address { get; set; }

        public User(string ip, string name, string pass, string group)
        {
            Address = ip;
            Name = name;
            Password = pass;
            Group = group;
        }
        public User()
        {
            Address = "";
            Name = "";
            Password = "";
            Group = "";
        }
    }

    public class UserManagerException : Exception
    {
        public UserManagerException(string message)
            : base(message)
        {

        }
        public UserManagerException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
    public class UserExistsException : UserManagerException
    {
        public UserExistsException(string name)
            : base("User '" + name + "' already exists")
        {
        }
    }
    public class UserNotExistException : UserManagerException
    {
        public UserNotExistException(string name)
            : base("User '" + name + "' does not exist")
        {
        }
    }

    public class GroupNotExistsException : UserManagerException
    {
        public GroupNotExistsException(string group)
            : base("Group '" + group + "' does not exist")
        {
        }
    }
}
