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
	/// <summary>UserManager - Methods for dealing with database users and other user functionality within TShock.</summary>
	public class UserManager
	{
		/// <summary>database - The database object to use for connections.</summary>
		private IDbConnection database;

		/// <summary>UserManager - Creates a UserManager object. During instantiation, this method will verify the table structure against the format below.</summary>
		/// <param name="db">db - The database to connect to.</param>
		/// <returns>A UserManager object.</returns>
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

		/// <summary>UpdateLogin - Updates the last accessed time for a database user to the current time.</summary>
		/// <param name="user">user - The user object to modify.</param>
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

		/// <summary>GetUserID - Gets the database ID of a given user object from the database.</summary>
		/// <param name="username">username - The username of the user to query for.</param>
		/// <returns>int - The user's ID</returns>
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

		/// <summary>GetUserByName - Gets a user object by name.</summary>
		/// <param name="name">name - The user's name.</param>
		/// <returns>User - The user object returned from the search.</returns>
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

		/// <summary>GetUserByID - Gets a user object by their user ID.</summary>
		/// <param name="id">id - The user's ID.</param>
		/// <returns>User - The user object returned from the search.</returns>
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

		/// <summary>GetUser - Gets a user object by a user object.</summary>
		/// <param name="user">user - The user object to search by.</param>
		/// <returns>User - The user object that is returned from the search.</returns>
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

		/// <summary>GetUsers - Gets all users from the database.</summary>
		/// <returns>List - The users from the database.</returns>
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

		/// <summary>
		/// GetUsersByName - Gets all users from the database with a username that starts with or contains <see cref="username"/>
		/// </summary>
		/// <param name="username">String - Rough username search. "n" will match "n", "na", "nam", "name", etc</param>
		/// <param name="notAtStart">Boolean - If <see cref="username"/> is not the first part of the username. If true then "name" would match "name", "username", "wordsnamewords", etc</param>
		/// <returns>List or null - Matching users or null if exception is thrown</returns>
		public List<User> GetUsersByName(string username, bool notAtStart = false)
		{
			try
			{
				List<User> users = new List<User>();
				string search = notAtStart ? string.Format("%{0}%", username) : string.Format("{0}%", username);
				using (var reader = database.QueryReader("SELECT * FROM Users WHERE Username LIKE @0",
					search))
				{
					while (reader.Read())
					{
						users.Add(LoadUserFromResult(new User(), reader));
					}
				}
				return users;
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return null;
		}

		/// <summary>LoadUserFromResult - Fills out the fields of a User object with the results from a QueryResult object.</summary>
		/// <param name="user">user - The user to add data to.</param>
		/// <param name="result">result - The QueryResult object to add data from.</param>
		/// <returns>User - The 'filled out' user object.</returns>
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

	/// <summary>User - A database user.</summary>
	public class User
	{
		/// <summary>ID - The database ID of the user.</summary>
		public int ID { get; set; }

		/// <summary>Name - The user's name.</summary>
		public string Name { get; set; }

		/// <summary>Password - The hashed password for the user.</summary>
		public string Password { get; internal set; }

		/// <summary>UUID - The user's saved Univerally Unique Identifier token.</summary>
		public string UUID { get; set; }
		
		/// <summary>Group - The group object that the user is a part of.</summary>
		public string Group { get; set; }

		/// <summary>Registered - The unix epoch corresponding to the registration date of the user.</summary>
		public string Registered { get; set; }

		/// <summary>LastAccessed - The unix epoch corresponding to the last access date of the user.</summary>
		public string LastAccessed { get; set; }

		/// <summary>KnownIps - A JSON serialized list of known IP addresses for a user.</summary>
		public string KnownIps { get; set; }

		/// <summary>User - Constructor for the user object, assuming you define everything yourself.</summary>
		/// <param name="name">name - The user's name.</param>
		/// <param name="pass">pass - The user's password hash.</param>
		/// <param name="uuid">uuid - The user's UUID.</param>
		/// <param name="group">group - The user's group name.</param>
		/// <param name="registered">registered - The unix epoch for the registration date.</param>
		/// <param name="last">last - The unix epoch for the last access date.</param>
		/// <param name="known">known - The known IPs for the user, serialized as a JSON object</param>
		/// <returns>A completed user object.</returns>
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

		/// <summary>User - Default constructor for a user object; holds no data.</summary>
		/// <returns>A user object.</returns>
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
					UpgradePasswordWorkFactor(password);
					return true;
				}
			} 
			catch (SaltParseException)
			{
				if (HashPassword(password).ToUpper() == this.Password.ToUpper())
				{
					// The password is not stored using BCrypt; upgrade it to BCrypt immediately
					UpgradePasswordToBCrypt(password);
					return true;
				}
				return false;
			}
			return false;
		}

		/// <summary>Upgrades a password to BCrypt, from an insecure hashing algorithm.</summary>
		/// <param name="password">string password - the raw user password (unhashed) to upgrade</param>
		protected void UpgradePasswordToBCrypt(string password)
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
		protected void UpgradePasswordWorkFactor(string password)
		{
			// If the destination work factor is not greater, we won't upgrade it or re-hash it
			int currentWorkFactor = 0;
			try
			{
				currentWorkFactor = Int32.Parse((this.Password.Split('$')[2]));
			}
			catch (FormatException)
			{
				TShock.Log.ConsoleError("Warning: Not upgrading work factor because bcrypt hash in an invalid format.");
				return;
			}

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
		/// A dictionary of hashing algorithms and an implementation object.
		/// </summary>
		protected readonly Dictionary<string, Func<HashAlgorithm>> HashTypes = new Dictionary<string, Func<HashAlgorithm>>
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
		protected string HashPassword(byte[] bytes)
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
		protected string HashPassword(string password)
		{
			if (string.IsNullOrEmpty(password) || password == "non-existant password")
				return null;
			return HashPassword(Encoding.UTF8.GetBytes(password));
		}

	}

	/// <summary>UserManagerException - An exception generated by the user manager.</summary>
	[Serializable]
	public class UserManagerException : Exception
	{
		/// <summary>UserManagerException - Creates a new UserManagerException object.</summary>
		/// <param name="message">message - The message for the object.</param>
		/// <returns>public - a new UserManagerException object.</returns>
		public UserManagerException(string message)
			: base(message)
		{
		}

		/// <summary>UserManagerException - Creates a new UserManagerObject with an internal exception.</summary>
		/// <param name="message">message - The message for the object.</param>
		/// <param name="inner">inner - The inner exception for the object.</param>
		/// <returns>public - a nwe UserManagerException with a defined inner exception.</returns>
		public UserManagerException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	/// <summary>UserExistsException - A UserExistsException object, used when a user already exists when attempting to create a new one.</summary>
	[Serializable]
	public class UserExistsException : UserManagerException
	{
		/// <summary>UserExistsException - Creates a new UserExistsException object.</summary>
		/// <param name="name">name - The name of the user that already exists.</param>
		/// <returns>public - a UserExistsException object with the user's name passed in the message.</returns>
		public UserExistsException(string name)
			: base("User '" + name + "' already exists")
		{
		}
	}

	/// <summary>UserNotExistException - A UserNotExistException, used when a user does not exist and a query failed as a result of it.</summary>
	[Serializable]
	public class UserNotExistException : UserManagerException
	{
		/// <summary>UserNotExistException - Creates a new UserNotExistException object, with the user's name in the message.</summary>
		/// <param name="name">name - The user's name to be pasesd in the message.</param>
		/// <returns>public - a new UserNotExistException object with a message containing the user's name that does not exist.</returns>
		public UserNotExistException(string name)
			: base("User '" + name + "' does not exist")
		{
		}
	}

	/// <summary>GroupNotExistsException - A GroupNotExistsException, used when a group does not exist.</summary>
	[Serializable]
	public class GroupNotExistsException : UserManagerException
	{
		/// <summary>GroupNotExistsException - Creates a new GroupNotExistsException object with the group's name in the message.</summary>
		/// <param name="group">group - The group name.</param>
		/// <returns>public - a new GroupNotExistsException with the group that does not exist's name in the message.</returns>
		public GroupNotExistsException(string group)
			: base("Group '" + group + "' does not exist")
		{
		}
	}
}