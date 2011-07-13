
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
                        "CREATE TABLE IF NOT EXISTS 'Users' ('Username' TEXT PRIMARY KEY, 'Password' TEXT, 'UserGroup' TEXT, 'IP' TEXT);";
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    com.CommandText =
                        "CREATE TABLE IF NOT EXISTS Users (Username VARCHAR(255) PRIMARY, Password VARCHAR(255), UserGroup VARCHAR(255), IP VARCHAR(255));";
                
                com.ExecuteNonQuery();
            }
        }

        public int AddUser(string ip = "" , string name = "", string password = "", string group = "default")
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    com.CommandText = "INSERT INTO Users (Username, Password, UserGroup, IP) VALUES (@name, @password, @group, @ip);";
                    com.AddParameter("@name", name.ToLower());
                    com.AddParameter("@password", Tools.HashPassword(password));

                    if(TShock.Groups.GroupExists(group))
                        com.AddParameter("@group", group);
                    else
                        //Return code 2 (Group not exist)
                        return 2;

                    com.AddParameter("@ip", ip);

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.RecordsAffected > 0)
                        {
                            //Return code 1 (User added)
                            return 1;
                        }
                        else
                        {
                            //Return code 0 (Add failed)
                            return 0;
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                //Return code 0 (Add failed)
                Log.Error(ex.ToString());
                return 0;
            }
        }

        public int RemoveUser(string inputUser, bool ip)
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    if (ip)
                    {
                        com.CommandText = "DELETE FROM Users WHERE IP=`@ip`";
                        com.AddParameter("@ip", inputUser.ToLower());
                    } else
                    {
                        com.CommandText = "DELETE FROM Users WHERE Username=`@name`";
                        com.AddParameter("@name", inputUser.ToLower());
                    }

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.RecordsAffected > 0)
                        {
                            //Return code 1 (User removed)
                            reader.Close();
                            return 1;
                        }
                        else
                        {
                            //Return code 0 (Remove failed)
                            reader.Close();
                            return 0;
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                //Return code 0 (Remove failed)
                Log.Error(ex.ToString());
                return 0;
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
                Log.Error(ex.ToString());
            }
            return returndata;
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
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return Tools.GetGroup("default");
        }

        public string GetUserID(string username = "", string IP = "")
        {
            try
            {
                using (var com = database.CreateCommand())
                {
                    if (username != "" && username != null)
                    {
                        com.CommandText = "SELECT * FROM Users WHERE Username=@name";
                        com.AddParameter("@name", username);
                    }
                    else if (IP != "" && IP != null)
                    {
                        com.CommandText = "SELECT * FROM Users WHERE IP=@ip";
                        com.AddParameter("@ip", IP);
                    }
                    else
                        return "0";

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string ID = reader.Get<string>("ID");
                            return ID;
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            return "0";
        }
    }
}
