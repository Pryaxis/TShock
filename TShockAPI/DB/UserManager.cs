/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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
using System.CodeDom.Compiler;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using BCrypt.Net;
using System.Security.Cryptography;

namespace TShockAPI.DB
{
	public class UserManager
	{
		private IDbConnection database;

		public UserManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Users",
			                         new SqlColumn("ID", MySqlDbType.Int32) {Primary = true, AutoIncrement = true},
			                         new SqlColumn("Username", MySqlDbType.VarChar, 32) {Unique = true},
			                         new SqlColumn("Password", MySqlDbType.VarChar, 128),
                                     new SqlColumn("UUID", MySqlDbType.VarChar, 128),
			                         new SqlColumn("Usergroup", MySqlDbType.Text),
									 new SqlColumn("Registered", MySqlDbType.Text),
                                     new SqlColumn("LastAccessed", MySqlDbType.Text),
                                     new SqlColumn("KnownIPs", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
		}

		/// <summary>
		/// Adds a given username to the database
		/// </summary>
		/// <param name="user">User user</param>
		public void AddUser(User user)
		{
			if (!TShock.Groups.GroupExists(user.Group))
				throw new GroupNotExistsException(user.Group);

			int ret;
			try
			{
				ret = database.Query("INSERT INTO Users (Username, Password, UUID, UserGroup, Registered) VALUES (@0, @1, @2, @3, @4);", user.Name,
								   user.Password, user.UUID, user.Group, DateTime.UtcNow.ToString("s"));
			}
			catch (Exception ex)
			{
				// Detect duplicate user using a regexp as Sqlite doesn't have well structured exceptions
				if (Regex.IsMatch(ex.Message, "Username.*not unique"))
					throw new UserExistsException(user.Name);
				throw new UserManagerException("AddUser SQL returned an error (" + ex.Message + ")", ex);
			}

			if (1 > ret)
				throw new UserExistsException(user.Name);

			Hooks.AccountHooks.OnAccountCreate(user);
		}

		/// <summary>
		/// Removes a given username from the database
		/// </summary>
		/// <param name="user">User user</param>
		public void RemoveUser(User user)
		{
			try
			{
				var tempuser = GetUser(user);
				int affected = database.Query("DELETE FROM Users WHERE Username=@0", user.Name);

				if (affected < 1)
					throw new UserNotExistException(user.Name);

				Hooks.AccountHooks.OnAccountDelete(tempuser);
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
		/// <param name="password">string password</param>
		public void SetUserPassword(User user, string password)
		{
			try
			{
				if (
					database.Query("UPDATE Users SET Password = @0 WHERE Username = @1;", user.Password,
					               user.Name) == 0)
					throw new UserNotExistException(user.Name);
			}
			catch (Exception ex)
			{
				throw new UserManagerException("SetUserPassword SQL returned an error", ex);
			}
		}

		/// <summary>
		/// Sets the UUID for a given username
		/// </summary>
		/// <param name="user">User user</param>
		/// <param name="uuid">string uuid</param>
		public void SetUserUUID(User user, string uuid)
		{
			try
			{
				if (
					database.Query("UPDATE Users SET UUID = @0 WHERE Username = @1;", uuid,
								   user.Name) == 0)
					throw new UserNotExistException(user.Name);
			}
			catch (Exception ex)
			{
				throw new UserManagerException("SetUserUUID SQL returned an error", ex);
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
				Group grp = TShock.Groups.GetGroupByName(group);
				if (null == grp)
					throw new GroupNotExistsException(group);

				if (database.Query("UPDATE Users SET UserGroup = @0 WHERE Username = @1;", group, user.Name) == 0)
					throw new UserNotExistException(user.Name);
				
				// Update player group reference for any logged in player
				foreach (var player in TShock.Players.Where(p => null != p && p.UserAccountName == user.Name))
				{
					player.Group = grp;
				}
			}
			catch (Exception ex)
			{
				throw new UserManagerException("SetUserGroup SQL returned an error", ex);
			}
		}

	    public void UpdateLogin(User user)
	    {
            try
            {
                if (database.Query("UPDATE Users SET LastAccessed = @0, KnownIps = @1 WHERE Username = @2;", DateTime.UtcNow.ToString("s"), user.KnownIps, user.Name) == 0)
                    throw new UserNotExistException(user.Name);
            }
            catch (Exception ex)
            {
                throw new UserManagerException("UpdateLogin SQL returned an error", ex);
            }
	    }

		public int GetUserID(string username)
		{
			try
			{
				using (var reader = database.QueryReader("SELECT * FROM Users WHERE Username=@0", username))
				{
					if (reader.Read())
					{
						return reader.Get<int>("ID");
					}
				}
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError("FetchHashedPasswordAndGroup SQL returned an error: " + ex);
			}
			return -1;
		}

		public User GetUserByName(string name)
		{
			try
			{
				return GetUser(new User {Name = name});
			}
			catch (UserManagerException)
			{
				return null;
			}
		}

		public User GetUserByID(int id)
		{
			try
			{
				return GetUser(new User {ID = id});
			}
			catch (UserManagerException)
			{
				return null;
			}
		}

		public User GetUser(User user)
		{
			bool multiple = false;
			string query;
			string type;
			object arg;
			if (0 != user.ID)
			{
				query = "SELECT * FROM Users WHERE ID=@0";
				arg = user.ID;
				type = "id";
			}
			else
			{
				query = "SELECT * FROM Users WHERE Username=@0";
				arg = user.Name;
				type = "name";
			}

			try
			{
				using (var result = database.QueryReader(query, arg))
				{
					if (result.Read())
					{
						user = LoadUserFromResult(user, result);
						// Check for multiple matches
						if (!result.Read())
							return user;
						multiple = true;
					}
				}
			}
			catch (Exception ex)
			{
				throw new UserManagerException("GetUser SQL returned an error (" + ex.Message + ")", ex);
			}
			if (multiple)
				throw new UserManagerException(String.Format("Multiple users found for {0} '{1}'", type, arg));

			throw new UserNotExistException(user.Name);
		}

		public List<User> GetUsers()
		{
			try
			{
				List<User> users = new List<User>();
				using (var reader = database.QueryReader("SELECT * FROM Users"))
				{
					while (reader.Read())
					{
						users.Add(LoadUserFromResult(new User(), reader));
					}
					return users;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}

		private User LoadUserFromResult(User user, QueryResult result)
		{
			user.ID = result.Get<int>("ID");
			user.Group = result.Get<string>("Usergroup");
			user.Password = result.Get<string>("Password");
            user.UUID = result.Get<string>("UUID");
			user.Name = result.Get<string>("Username");
			user.Registered = result.Get<string>("Registered");
            user.LastAccessed = result.Get<string>("LastAccessed");
            user.KnownIps = result.Get<string>("KnownIps");
			return user;
		}
	}

	public class User
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Password { get; internal set; }
		public string UUID { get; set; }
		public string Group { get; set; }
		public string Registered { get; set; }
		public string LastAccessed { get; set; }
		public string KnownIps { get; set; }

		public User(string name, string pass, string uuid, string group, string registered, string last, string known)
		{
			Name = name;
			Password = pass;
			UUID = uuid;
			Group = group;
			Registered = registered;
			LastAccessed = last;
			KnownIps = known;
		}

		public User()
		{
			Name = "";
			Password = "";
			UUID = "";
			Group = "";
			Registered = "";
			LastAccessed = "";
			KnownIps = "";
		}

		/// <summary>
		/// Verifies if a password matches the one stored in the database.
		/// If the password is stored in an unsafe hashing algorithm, it will be converted to BCrypt.
		/// If the password is stored using BCrypt, it will be re-saved if the work factor in the config
		/// is greater than the existing work factor with the new work factor.
		/// </summary>
		/// <param name="password">string password - The password to check against the user object.</param>
		/// <returns>bool true, if the password matched, or false, if it didn't.</returns>
		public bool VerifyPassword(string password)
		{
			try
			{
				if (BCrypt.Net.BCrypt.Verify(password, this.Password)) 
				{
					// If necessary, perform an upgrade to the highest work factor.
					upgradePasswordWorkFactor(password);
					return true;
				}
			} 
			catch (SaltParseException)
			{
				if (hashPassword(password).ToUpper() == this.Password.ToUpper())
				{
					// The password is not stored using BCrypt; upgrade it to BCrypt immediately
					upgradePasswordToBCrypt(password);
					return true;
				}
				return false;
			}
			return false;
		}

		/// <summary>Upgrades a password to BCrypt, from an insecure hashing algorithm.</summary>
		/// <param name="password">string password - the raw user password (unhashed) to upgrade</param>
		protected internal void upgradePasswordToBCrypt(string password)
		{
			// Save the old password, in the event that we have to revert changes.
			string oldpassword = this.Password;
			
			// Convert the password to BCrypt, and save it.
			try 
			{
				this.Password = BCrypt.Net.BCrypt.HashPassword(password, TShock.Config.BCryptWorkFactor);
			}
			catch (ArgumentOutOfRangeException)
			{
				TShock.Log.ConsoleError("Invalid BCrypt work factor in config file! Upgrading user password to BCrypt using default work factor.");
				this.Password = BCrypt.Net.BCrypt.HashPassword(password);
			}

			try
			{
				TShock.Users.SetUserPassword(this, this.Password);
			}
			catch (UserManagerException e)
			{
				TShock.Log.ConsoleError(e.ToString());
				this.Password = oldpassword; // Revert changes
			}
		}

		/// <summary>Upgrades a password to the highest work factor available in the config.</summary>
		/// <param name="password">string password - the raw user password (unhashed) to upgrade</param>
		protected internal void upgradePasswordWorkFactor(string password)
		{
			// If the destination work factor is not greater, we won't upgrade it or re-hash it
			int currentWorkFactor = Convert.ToInt32((this.Password.Split('$')[2]));

			if (currentWorkFactor < TShock.Config.BCryptWorkFactor)
			{
				try
				{
					this.Password = BCrypt.Net.BCrypt.HashPassword(password, TShock.Config.BCryptWorkFactor);
				}
				catch (ArgumentOutOfRangeException)
				{
					TShock.Log.ConsoleError("Invalid BCrypt work factor in config file! Refusing to change work-factor on exsting password.");
				}

				try
				{
					TShock.Users.SetUserPassword(this, this.Password);
				}
				catch (UserManagerException e)
				{
					TShock.Log.ConsoleError(e.ToString());
				}
			}
		}

		/// <summary>Creates a BCrypt hash for a user and stores it in this object.</summary>
		/// <param name="password">string password - the plain text password to hash</param>
		public void CreateBCryptHash(string password)
		{
			if (password.Trim().Length < Math.Max(4, TShock.Config.MinimumPasswordLength))
			{
				throw new ArgumentOutOfRangeException("password", "Password must be > " + TShock.Config.MinimumPasswordLength + " characters.");
			}
			try
			{
				this.Password = BCrypt.Net.BCrypt.HashPassword(password.Trim(), TShock.Config.BCryptWorkFactor);
			}
			catch (ArgumentOutOfRangeException)
			{
				TShock.Log.ConsoleError("Invalid BCrypt work factor in config file! Creating new hash using default work factor.");
				this.Password = BCrypt.Net.BCrypt.HashPassword(password.Trim());
			}
		}

		/// <summary>Creates a BCrypt hash for a user and stores it in this object.</summary>
		/// <param name="password">string password - the plain text password to hash</param>
		/// <param name="workFactor">int workFactor - the work factor to use in generating the password hash</param>
		public void CreateBCryptHash(string password, int workFactor)
		{
			if (password.Trim().Length < Math.Max(4, TShock.Config.MinimumPasswordLength))
			{
				throw new ArgumentOutOfRangeException("password", "Password must be > " + TShock.Config.MinimumPasswordLength + " characters.");
			}
			this.Password = BCrypt.Net.BCrypt.HashPassword(password.Trim(), workFactor);
		}

		/// <summary>
		/// A dictionary of hashing algortihms and an implementation object.
		/// </summary>
		protected internal readonly Dictionary<string, Func<HashAlgorithm>> HashTypes = new Dictionary<string, Func<HashAlgorithm>>
																																					{
																																						{"sha512", () => new SHA512Managed()},
																																						{"sha256", () => new SHA256Managed()},
																																						{"md5", () => new MD5Cng()},
																																						{"sha512-xp", () => SHA512.Create()},
																																						{"sha256-xp", () => SHA256.Create()},
																																						{"md5-xp", () => MD5.Create()},
																																					};

		/// <summary>
		/// Returns a hashed string for a given string based on the config file's hash algo
		/// </summary>
		/// <param name="bytes">bytes to hash</param>
		/// <returns>string hash</returns>
		protected internal string hashPassword(byte[] bytes)
		{
			if (bytes == null)
				throw new NullReferenceException("bytes");
			Func<HashAlgorithm> func;
			if (!HashTypes.TryGetValue(TShock.Config.HashAlgorithm.ToLower(), out func))
				throw new NotSupportedException("Hashing algorithm {0} is not supported".SFormat(TShock.Config.HashAlgorithm.ToLower()));

			using (var hash = func())
			{
				var ret = hash.ComputeHash(bytes);
				return ret.Aggregate("", (s, b) => s + b.ToString("X2"));
			}
		}

		/// <summary>
		/// Returns a hashed string for a given string based on the config file's hash algo
		/// </summary>
		/// <param name="password">string to hash</param>
		/// <returns>string hash</returns>
		protected internal string hashPassword(string password)
		{
			if (string.IsNullOrEmpty(password) || password == "non-existant password")
				return null;
			return hashPassword(Encoding.UTF8.GetBytes(password));
		}

	}

	[Serializable]
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

	[Serializable]
	public class UserExistsException : UserManagerException
	{
		public UserExistsException(string name)
			: base("User '" + name + "' already exists")
		{
		}
	}

	[Serializable]
	public class UserNotExistException : UserManagerException
	{
		public UserNotExistException(string name)
			: base("User '" + name + "' does not exist")
		{
		}
	}

	[Serializable]
	public class GroupNotExistsException : UserManagerException
	{
		public GroupNotExistsException(string group)
			: base("Group '" + group + "' does not exist")
		{
		}
	}
}