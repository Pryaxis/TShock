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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using BCrypt.Net;
using MySql.Data.MySqlClient;
using TShockAPI.Hooks;
using TShockAPI.PermissionSystem;

namespace TShockAPI.DB
{
	/// <summary>
	/// Methods for dealing with database users and other user functionality within TShock.
	/// </summary>
	public class UserManager
	{
		/// <summary>
		/// The database object to use for connections.
		/// </summary>
		private IDbConnection database;

		/// <summary>
		/// Contains users which were added or have been retrieved from the database at least once
		/// during this session.
		/// </summary>
		private List<User> userCache = new List<User>();

		/// <summary>
		/// Creates a UserManager object.
		/// During instantiation, this method will verify the table structure against the format below.
		/// </summary>
		/// <param name="db">The database to connect to.</param>
		public UserManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("Users",
				new SqlColumn("ID", MySqlDbType.Int32) { Primary = true, AutoIncrement = true },
				new SqlColumn("Username", MySqlDbType.VarChar, 32) { Unique = true },
				new SqlColumn("Password", MySqlDbType.VarChar, 128),
				new SqlColumn("UUID", MySqlDbType.VarChar, 128),
				new SqlColumn("Usergroup", MySqlDbType.Text),
				new SqlColumn("Registered", MySqlDbType.Text),
				new SqlColumn("LastAccessed", MySqlDbType.Text),
				new SqlColumn("KnownIPs", MySqlDbType.Text),
				new SqlColumn("Permissions", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
				db.GetSqlType() == SqlType.Sqlite
				? (IQueryBuilder)new SqliteQueryCreator()
				: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
		}

		/// <summary>
		/// Adds a given username to the database
		/// </summary>
		/// <param name="user">User user</param>
		[Obsolete("Use SaveUser() instead.")]
		public void AddUser(User user)
		{
			if (!TShock.Groups.GroupExists(user.Group))
				throw new GroupNotFoundException(user.Group);

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
		/// Removes an user from the database.
		/// </summary>
		/// <exception cref="UserManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <exception cref="UserNotFoundException">Thrown if no user by the given name is found in the database.</exception>
		/// <param name="username">The name of the user to remove.</param>
		/// <returns>True if the user was removed, or false if the query affected an unexpected amount of rows.</returns>
		public bool RemoveUser(string username)
		{
			User user = GetUserByName(username);
			if (user == null)
				throw new UserNotFoundException(username);

			string query = "DELETE FROM Users WHERE Username=@0";
			try
			{
				if (database.Query(query, username) == 1)
				{
					userCache.RemoveAll(u => u.Name == username);
					AccountHooks.OnAccountDelete(user);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new UserManagerException("Failed to remove user '{0}'.".SFormat(username), ex);
			}
		}

		/// <summary>
		/// Removes a given username from the database
		/// </summary>
		/// <param name="user">User user</param>
		[Obsolete("Use bool RemoveUser() instead.")]
		public void RemoveUser(User user)
		{
			try
			{
				var tempuser = GetUser(user);
				int affected = database.Query("DELETE FROM Users WHERE Username=@0", user.Name);

				if (affected < 1)
					throw new UserNotFoundException(user.Name);

				Hooks.AccountHooks.OnAccountDelete(tempuser);
			}
			catch (Exception ex)
			{
				throw new UserManagerException("RemoveUser SQL returned an error", ex);
			}
		}

		/// <summary>
		/// Saves an user to the database.
		/// Using this for an existing user will update the data.
		/// </summary>
		/// <exception cref="GroupNotFoundException">Thrown when the user group doesn't exist.</exception>
		/// <exception cref="UserManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <param name="user">The user to save.</param>
		/// <returns>True if the user was saved, or false if the query affected an unexpected amount of rows.</returns>
		public bool SaveUser(User user)
		{
			if (!TShock.Groups.GroupExists(user.Group))
				throw new GroupNotExistException(user.Group);

			var querybuilder = new StringBuilder();
			bool updating = UserExists(user.Name);
			if (updating)
				querybuilder.Append("UPDATE Users SET Password=@1, UUID=@2, UserGroup=@3, Registered=@4, LastAccessed=@5, KnownIPs=@6, Permissions=@7 WHERE Username=@0");
			else
			{
				querybuilder.Append(TShock.Config.StorageType.ToLower() == "sqlite"
					? "INSERT OR IGNORE INTO Users (Username, Password, UUID, UserGroup, Registered, LastAccessed, KnownIPs, Permissions) VALUES (@0, @1, @2, @3, @4, @5, @6, @7);"
					: "INSERT IGNORE INTO Users SET Username=@0, Password=@1, UUID=@2, UserGroup=@3, Registered=@4, LastAccessed=@5, KnownIPs=@6, Permissions=@7;");
			}

			// Add query for obtaining the user id
			querybuilder.Append(TShock.Config.StorageType.ToLower() == "sqlite"
				? "SELECT ID FROM Users WHERE Username=@0;"
				: "SELECT LAST_INSERT_ID();");

			try
			{
				using (QueryResult result = database.QueryReader(querybuilder.ToString(), user.Name, user.Password, user.UUID, user.Group,
					user.Registered, user.LastAccessed, user.KnownIps, user.Permissions))
				{
					if (result.Read())
					{
						user.ID = result.Get<int>("ID");
						userCache.RemoveAll(u => u.Name == user.Name);
						userCache.Add(user);
						if (!updating)
							AccountHooks.OnAccountCreate(user);
						return true;
					}
					else
						return false;
				}
			}
			catch (Exception ex)
			{
				throw new UserManagerException("Failed to save user '{0}'.".SFormat(user.Name), ex);
			}
		}

		/// <summary>
		/// Updates the group of an user.
		/// </summary>
		/// <exception cref="GroupNotFoundException">Thrown when the given group doesn't exist.</exception>
		/// <exception cref="UserManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <exception cref="UserNotFoundException">Thrown if no user by the given name is found in the database.</exception>
		/// <param name="username">The name of the user to modify.</param>
		/// <param name="groupname">The name of the user group.</param>
		/// <returns></returns>
		public bool SetUserGroup(string username, string groupname)
		{
			if (!UserExists(username))
				throw new UserNotFoundException(username);
			if (!TShock.Groups.GroupExists(groupname))
				throw new GroupNotFoundException(groupname);

			string query = "UPDATE Users SET UserGroup=@1 WHERE Username=@0";
			try
			{
				if (database.Query(query, username, groupname) == 1)
				{
					// Using FindAll followed by ForEach removes the need of checking if userCache contains the user
					userCache.FindAll(u => u.Name == username).ForEach(u => u.Group = groupname);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new UserManagerException("Failed to set the group for '{0}'.".SFormat(groupname), ex);
			}
		}

		/// <summary>
		/// Sets the group for a given username
		/// </summary>
		/// <param name="user">User user</param>
		/// <param name="group">string group</param>
		[Obsolete("Use bool SetUserGroup() instead.")]
		public void SetUserGroup(User user, string group)
		{
			try
			{
				Group grp = TShock.Groups.GetGroupByName(group);
				if (null == grp)
					throw new GroupNotFoundException(group);

				if (database.Query("UPDATE Users SET UserGroup = @0 WHERE Username = @1;", group, user.Name) == 0)
					throw new UserNotFoundException(user.Name);

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

		/// <summary>
		/// Updates the hashed password of an user.
		/// </summary>
		/// <exception cref="UserManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <exception cref="UserNotFoundException">Thrown if no user by the given name is found in the database.</exception>
		/// <param name="username">The name of the user to modify.</param>
		/// <param name="password">The new hashed password.</param>
		/// <returns>True if the password was modified, or false if the query affected an unexpected amount of rows.</returns>
		public bool SetUserPassword(string username, string password)
		{
			if (!UserExists(username))
				throw new UserNotFoundException(username);

			string query = "UPDATE Users SET Password=@1 WHERE Username=@0";
			try
			{
				if (database.Query(query, username, password) == 1)
				{
					// Using FindAll followed by ForEach removes the need of checking if userCache contains the user
					userCache.FindAll(u => u.Name == username).ForEach(u => u.Password = password);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new UserManagerException("Failed to set the hashed password for '{0}'.".SFormat(username), ex);
			}
		}

		/// <summary>
		/// Sets the Hashed Password for a given username
		/// </summary>
		/// <param name="user">User user</param>
		/// <param name="password">string password</param>
		[Obsolete("Use bool SetUserPassword() instead.")]
		public void SetUserPassword(User user, string password)
		{
			try
			{
				if (
					database.Query("UPDATE Users SET Password = @0 WHERE Username = @1;", user.Password,
								   user.Name) == 0)
					throw new UserNotFoundException(user.Name);
			}
			catch (Exception ex)
			{
				throw new UserManagerException("SetUserPassword SQL returned an error", ex);
			}
		}

		/// <summary>
		/// Updates the UUID of an user.
		/// </summary>
		/// <exception cref="UserManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <exception cref="UserNotFoundException">Thrown if no user by the given name is found in the database.</exception>
		/// <param name="username">The name of the user to modify.</param>
		/// <param name="uuid">The Universally Unique Identifier string to set.</param>
		/// <returns>True if the UUID was modified, or false if the query affected an unexpected amount of rows.</returns>
		public bool SetUserUUID(string username, string uuid)
		{
			if (!UserExists(username))
				throw new UserNotFoundException(username);

			string query = "UPDATE Users SET UUID=@1 WHERE Username=@0";
			try
			{
				if (database.Query(query, username, uuid) == 1)
				{
					// Using FindAll followed by ForEach removes the need of checking if userCache contains the user
					userCache.FindAll(u => u.Name == username).ForEach(u => u.UUID = uuid);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new UserManagerException("Failed to set the UUID for '{0}'.".SFormat(username), ex);
			}
		}

		/// <summary>
		/// Sets the UUID for a given username
		/// </summary>
		/// <param name="user">User user</param>
		/// <param name="uuid">string uuid</param>
		[Obsolete("Use bool SetUserUUID() instead.")]
		public void SetUserUUID(User user, string uuid)
		{
			try
			{
				if (
					database.Query("UPDATE Users SET UUID = @0 WHERE Username = @1;", uuid,
								   user.Name) == 0)
					throw new UserNotFoundException(user.Name);
			}
			catch (Exception ex)
			{
				throw new UserManagerException("SetUserUUID SQL returned an error", ex);
			}
		}

		/// <summary>
		/// Updates an user's list of known IPs and sets the user's last accessed time to the current time.
		/// </summary>
		/// <exception cref="UserManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <exception cref="UserNotFoundException">Thrown if no user by the given name is found in the database.</exception>
		/// <param name="username">The name of the user to update.</param>
		/// <param name="knownIPs">A JSON serialized list of strings.</param>
		/// <returns>True if the user got updated, or false if the query affected an unexpected amount of rows.</returns>
		public bool UpdateLogin(string username, string knownIPs)
		{
			if (!UserExists(username))
				throw new UserNotFoundException(username);

			string query = "UPDATE Users SET LastAccessed=@1, KnownIps=@2 WHERE Username=@0";
			try
			{
				string date = DateTime.UtcNow.ToString("s");
				if (database.Query(query, username, date, knownIPs) == 1)
				{
					// Using FindAll followed by ForEach removes the need of checking if userCache contains the user
					userCache.FindAll(u => u.Name == username).ForEach(u =>
					{
						u.KnownIps = knownIPs;
						u.LastAccessed = date;
					});
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new UserManagerException("Failed to update login for '{0}'.".SFormat(username), ex);
			}
		}

		/// <summary>UpdateLogin - Updates the last accessed time for a database user to the current time.</summary>
		/// <param name="user">user - The user object to modify.</param>
		[Obsolete("Use bool UpdateLogin() instead.")]
		public void UpdateLogin(User user)
		{
			try
			{
				if (database.Query("UPDATE Users SET LastAccessed = @0, KnownIps = @1 WHERE Username = @2;", DateTime.UtcNow.ToString("s"), user.KnownIps, user.Name) == 0)
					throw new UserNotFoundException(user.Name);
			}
			catch (Exception ex)
			{
				throw new UserManagerException("UpdateLogin SQL returned an error", ex);
			}
		}

		/// <summary>GetUser - Gets a user object by a user object.</summary>
		/// <param name="user">user - The user object to search by.</param>
		/// <returns>User - The user object that is returned from the search.</returns>
		[Obsolete("Use GetUserByID() or GetUserByName() instead.")]
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

			throw new UserNotFoundException(user.Name);
		}

		/// <summary>
		/// Gets a user object by their user ID.
		/// </summary>
		/// <param name="id">The user's ID.</param>
		/// <returns>The user object returned from the search.</returns>
		public User GetUserByID(int id)
		{
			User user;

			// Check the user cache before resorting to database queries
			if ((user = userCache.Find(u => u.ID == id)) != null)
				return user;

			string query = "SELECT * FROM Users WHERE ID=@0";
			try
			{
				using (var reader = database.QueryReader(query, id))
				{
					if (reader.Read())
					{
						user = LoadUserFromResult(new User(), reader);
						userCache.Add(user);
					}
					return user;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return null;
			}
		}

		/// <summary>
		/// Gets a user object by name.
		/// </summary>
		/// <param name="name">The user's name.</param>
		/// <returns>The user object returned from the search.</returns>
		public User GetUserByName(string name)
		{
			User user;

			// Check the user cache before resorting to database queries
			if ((user = userCache.Find(u => u.Name == name)) != null)
				return user;

			string query = "SELECT * FROM Users WHERE Username=@0";
			try
			{
				using (var reader = database.QueryReader(query, name))
				{
					if (reader.Read())
					{
						user = LoadUserFromResult(new User(), reader);
						userCache.Add(user);
					}
					return user;
				}
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return null;
			}
		}

		/// <summary>
		/// Gets the database ID of a given user object from the database.</summary>
		/// <exception cref="UserManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <exception cref="UserNotFoundException">Thrown if no user by the given name is found in the database.</exception>
		/// <param name="username">The name of the user to look for.</param>
		/// <returns>The user's ID.</returns>
		public int GetUserID(string username)
		{
			if (!UserExists(username))
				throw new UserNotFoundException(username);

			string query = "SELECT ID FROM Users WHERE Username=@0";
			try
			{
				using (var reader = database.QueryReader(query, username))
				{
					if (reader.Read())
						return reader.Get<int>("ID");
					else
						// This really shouldn't happen
						return -1;
				}
			}
			catch (Exception ex)
			{
				throw new UserManagerException("Failed to get the user ID for '{0}'.".SFormat(username), ex);
			}
		}

		/// <summary>
		/// Gets all users from the database. Also loads up the user cache.
		/// </summary>
		/// <exception cref="UserManagerException">Thrown if the query operation fails. Contains the inner exception.</exception>
		/// <returns>The users from the database.</returns>
		public List<User> GetUsers()
		{
			try
			{
				userCache.Clear();
				using (var reader = database.QueryReader("SELECT * FROM Users"))
				{
					while (reader.Read())
					{
						userCache.Add(LoadUserFromResult(new User(), reader));
					}
					return userCache;
				}
			}
			catch (Exception ex)
			{
				throw new UserManagerException("Failed to get all users.", ex);
			}
		}

		/// <summary>
		/// Gets all users from the database with a username that starts with or contains <see cref="username"/>.
		/// </summary>
		/// <param name="username">Rough username search. "n" will match "n", "na", "nam", "name", etc.</param>
		/// <param name="notAtStart">
		/// If <see cref="username"/> is not the first part of the username.
		/// If true then "name" would match "name", "username", "wordsnamewords", etc.
		/// </param>
		/// <returns>The list of matching users or null if an exception is thrown.</returns>
		public List<User> GetUsersByName(string username, bool notAtStart = false)
		{
			try
			{
				List<User> users = new List<User>();
				string search = notAtStart ? string.Format("%{0}%", username) : string.Format("{0}%", username);
				using (var reader = database.QueryReader("SELECT * FROM Users WHERE Username LIKE @0", search))
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

		/// <summary>
		/// Returns whether an user exists or not.
		/// </summary>
		/// <param name="id">The user ID to look for.</param>
		/// <returns>True if the user exists, or false if it doesn't.</returns>
		public bool UserExists(int id)
		{
			return GetUserByID(id) != null;
		}

		/// <summary>
		/// Returns whether an user exists or not.
		/// </summary>
		/// <param name="user">The exact user name to look for.</param>
		/// <returns>True if the user exists, or false if it doesn't.</returns>
		public bool UserExists(string user)
		{
			return GetUserByName(user) != null;
		}

		/// <summary>
		/// Fills out the fields of a User object with the results from a QueryResult object.
		/// </summary>
		/// <param name="user">The user to add data to.</param>
		/// <param name="result">The QueryResult object to add data from.</param>
		/// <returns>The 'filled out' user object.</returns>
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
			user.Permissions = result.Get<string>("Permissions");
			return user;
		}
	}

	/// <summary>
	/// A database user.
	/// </summary>
	public class User
	{
		/// <summary>
		/// The database ID of the user.
		/// </summary>
		public int ID { get; set; }

		/// <summary>
		/// The user's name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The hashed password for the user.
		/// </summary>
		public string Password { get; internal set; }

		/// <summary>
		/// The user's saved Universally Unique Identifier token.
		/// </summary>
		public string UUID { get; set; }

		/// <summary>
		/// The name of the group the user is part of.
		/// </summary>
		public string Group { get; set; }

		/// <summary>
		/// The unix epoch corresponding to the registration date of the user.
		/// </summary>
		public string Registered { get; set; }

		/// <summary>
		/// The unix epoch corresponding to the last access date of the user.
		/// </summary>
		public string LastAccessed { get; set; }

		/// <summary>
		/// A JSON serialized list of known IP addresses for a user.
		/// </summary>
		public string KnownIps { get; set; }

		/// <summary>
		/// The permissions of the user in string form.
		/// </summary>
		public string Permissions
		{
			get { return PermissionManager.ToString(); }
			set { PermissionManager.Parse(value); }
		}

		/// <summary>
		/// Manages the user's permissions.
		/// </summary>
		public IPermissionManager PermissionManager { get; private set; }

		/// <summary>
		/// Constructor for the user object, assuming you define everything yourself.
		/// </summary>
		/// <param name="name">The user's name.</param>
		/// <param name="pass">The user's password hash.</param>
		/// <param name="uuid">The user's UUID.</param>
		/// <param name="group">The user's group name.</param>
		/// <param name="registered">The unix epoch for the registration date.</param>
		/// <param name="last">The unix epoch for the last access date.</param>
		/// <param name="known">The known IPs for the user, serialized as a JSON object.</param>
		/// <param name="permissions">The user's list of permissions as a CSV string.</param>
		public User(string name, string pass, string uuid, string group, string registered, string last, string known, string permissions)
		{
			Name = name;
			Password = pass;
			UUID = uuid;
			Group = group;
			Registered = registered;
			LastAccessed = last;
			KnownIps = known;
			PermissionManager = new NegatedPermissionManager(permissions);
		}

		/// <summary>
		/// Default constructor for a user object.
		/// Holds no data.
		/// </summary>
		public User()
		{
			Name = "";
			Password = "";
			UUID = "";
			Group = "";
			Registered = "";
			LastAccessed = "";
			KnownIps = "";
			PermissionManager = new NegatedPermissionManager();
		}

		/// <summary>
		/// Adds a permission to the list of permissions.
		/// </summary>
		/// <param name="permission">The permission to add.</param>
		public void AddPermission(string permission)
		{
			PermissionManager.AddPermission(permission);
		}

		/// <summary>
		/// Determines whether the user has a specified permission.
		/// </summary>
		/// <param name="permission">The permission to check.</param>
		/// <returns>True if the user has the specified permission.</returns>
		public bool HasPermission(string permission)
		{
			return PermissionManager.HasPermission(new PermissionNode(permission));
		}

		/// <summary>
		/// Removes a permission from the list of permissions.
		/// </summary>
		/// <param name="permission">The permission to remove.</param>
		public void RemovePermission(string permission)
		{
			PermissionManager.RemovePermission(permission);
		}

		/// <summary>
		/// Clears the permission list and sets it to the list provided.
		/// </summary>
		/// <param name="permissions">The permission list to set.</param>
		public void SetPermissions(List<string> permissions)
		{
			PermissionManager.Parse(permissions);
		}

		/// <summary>
		/// Verifies if a password matches the one stored in the database.
		/// If the password is stored in an unsafe hashing algorithm, it will be converted to BCrypt.
		/// If the password is stored using BCrypt, it will be re-saved if the work factor in the config
		/// is greater than the existing work factor with the new work factor.
		/// </summary>
		/// <param name="password">The password to check against the user object.</param>
		/// <returns>True if the password matched, or false if it didn't.</returns>
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

		/// <summary>
		/// Upgrades a password to BCrypt, from an insecure hashing algorithm.
		/// </summary>
		/// <param name="password">The raw user password (unhashed) to upgrade.</param>
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

		/// <summary>
		/// Upgrades a password to the highest work factor available in the config.
		/// </summary>
		/// <param name="password">The raw user password (unhashed) to upgrade.</param>
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

		/// <summary>
		/// Creates a BCrypt hash for a user and stores it in this object.
		/// </summary>
		/// <param name="password">The plain text password to hash.</param>
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

		/// <summary>
		/// Creates a BCrypt hash for a user and stores it in this object.
		/// </summary>
		/// <param name="password">The plain text password to hash.</param>
		/// <param name="workFactor">The work factor to use in generating the password hash.</param>
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
		/// Returns a hashed string for a given string based on the config file's hash algo.
		/// </summary>
		/// <param name="bytes">Bytes to hash.</param>
		/// <returns>A hashed string.</returns>
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
		/// Returns a hashed string for a given string based on the config file's hash algo.
		/// </summary>
		/// <param name="password">String to hash.</param>
		/// <returns>A hashed string.</returns>
		protected string HashPassword(string password)
		{
			if (string.IsNullOrEmpty(password) || password == "non-existant password")
				return null;
			return HashPassword(Encoding.UTF8.GetBytes(password));
		}

		/// <summary>
		/// Returns the user's name.
		/// </summary>
		/// <returns>The user's name.</returns>
		public override string ToString()
		{
			return Name;
		}
	}

	/// <summary>
	/// An exception generated by the user manager.
	/// </summary>
	[Serializable]
	public class UserManagerException : Exception
	{
		/// <summary>
		/// Creates a new UserManagerException object.
		/// </summary>
		/// <param name="message">The message for the object.</param>
		public UserManagerException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Creates a new UserManagerObject with an internal exception.
		/// </summary>
		/// <param name="message">The message for the object.</param>
		/// <param name="inner">The inner exception for the object.</param>
		public UserManagerException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	/// <summary>
	/// A UserExistsException object, used when a user already exists when attempting to create a new one.
	/// </summary>
	[Serializable]
	public class UserExistsException : UserManagerException
	{
		/// <summary>
		/// Creates a new UserExistsException object.
		/// </summary>
		/// <param name="name">The name of the user that already exists.</param>
		public UserExistsException(string name)
			: base("User '" + name + "' already exists.")
		{
		}
	}

	/// <summary>
	/// A UserNotFoundException, used when a user does not exist and a query failed as a result of it.
	/// </summary>
	[Serializable]
	public class UserNotFoundException : UserManagerException
	{
		/// <summary>
		/// Creates a new UserNotFoundException object, with the user's name in the message.
		/// </summary>
		/// <param name="name">The user's name to be passed in the message.</param>
		public UserNotFoundException(string name)
			: base("User '" + name + "' does not exist.")
		{
		}
	}

	/// <summary>
	/// A GroupNotFoundException, used when a group does not exist.
	/// </summary>
	[Serializable]
	public class GroupNotFoundException : UserManagerException
	{
		/// <summary>
		/// Creates a new GroupNotFoundException object with the group's name in the message.
		/// </summary>
		/// <param name="group">The group name.</param>
		public GroupNotFoundException(string group)
			: base("Group '" + group + "' does not exist.")
		{
		}
	}
}