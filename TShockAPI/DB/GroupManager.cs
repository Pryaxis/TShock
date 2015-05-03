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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using MySql.Data.MySqlClient;
using TShockAPI.PermissionSystem;

namespace TShockAPI.DB
{
	public class GroupManager : IEnumerable<Group>
	{
		private IDbConnection database;

		/// <summary>
		/// A read-only list of groups that mirrors the database contents.
		/// </summary>
		public readonly List<Group> groups = new List<Group>();

		/// <summary>
		/// Creates a new GroupManager instance and connects to a database.
		/// </summary>
		/// <param name="db">The database connection to use.</param>
		public GroupManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("GroupList",
				new SqlColumn("GroupName", MySqlDbType.VarChar, 32) { Primary = true },
				new SqlColumn("Parent", MySqlDbType.VarChar, 32),
				new SqlColumn("Commands", MySqlDbType.Text),
				new SqlColumn("ChatColor", MySqlDbType.Text),
				new SqlColumn("Prefix", MySqlDbType.Text),
				new SqlColumn("Suffix", MySqlDbType.Text)
				);

			var creator = new SqlTableCreator(db,
				db.GetSqlType() == SqlType.Sqlite
				? (IQueryBuilder)new SqliteQueryCreator()
				: new MysqlQueryCreator()
				);

			if (creator.EnsureTableStructure(table))
			{
				// Add default groups if they don't exist
				AddDefaultGroup("guest", "",
					String.Join(",", Permissions.canbuild, Permissions.canregister, Permissions.canlogin, Permissions.canpartychat,
						Permissions.cantalkinthird, Permissions.canchat));

				AddDefaultGroup("default", "guest",
					String.Join(",", Permissions.warp, Permissions.canchangepassword));

				AddDefaultGroup("newadmin", "default",
					String.Join(",", Permissions.kick, Permissions.editspawn, Permissions.reservedslot));

				AddDefaultGroup("admin", "newadmin",
					String.Join(",", Permissions.ban, Permissions.whitelist, "tshock.world.time.*", Permissions.spawnboss,
						Permissions.spawnmob, Permissions.managewarp, Permissions.time, Permissions.tp, Permissions.slap,
						Permissions.kill, Permissions.logs,
						Permissions.immunetokick, Permissions.tpothers));

				AddDefaultGroup("trustedadmin", "admin",
					String.Join(",", Permissions.maintenance, "tshock.cfg.*", "tshock.world.*", Permissions.butcher, Permissions.item,
						Permissions.heal, Permissions.immunetoban, Permissions.usebanneditem));

				AddDefaultGroup("vip", "default", Permissions.reservedslot);
			}

			// Load Permissions from the DB
			LoadPermisions();

			Group.DefaultGroup = GetGroupByName(TShock.Config.DefaultGuestGroupName);
		}

		/// <summary>
		/// Adds a new default group to the database.
		/// </summary>
		/// <param name="name">The name of the group to add.</param>
		/// <param name="parent">The name of the group's parent.</param>
		/// <param name="permissions">A CSV string containing the group's list of permissions.</param>
		/// <param name="chatcolor">The group's chat color in the RRR,GGG,BBB format.</param>
		private void AddDefaultGroup(string name, string parent, string permissions, string chatcolor = Group.defaultChatColor)
		{
			if (!GroupExists(name))
				AddGroup(name, parent, permissions, chatcolor);
		}

		/// <summary>
		/// Determines whether a group exists.
		/// </summary>
		/// <param name="group">The group's name.</param>
		/// <returns>True if the group exists.</returns>
		public bool GroupExists(string group)
		{
			if (group == "superadmin")
				return true;

			return groups.Any(g => g.Name.Equals(group));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Returns an enumerator that itinerates through the list of groups.
		/// </summary>
		/// <returns>The enumerator.</returns>
		public IEnumerator<Group> GetEnumerator()
		{
			return groups.GetEnumerator();
		}

		/// <summary>
		/// Gets a group with the given name.
		/// </summary>
		/// <param name="name">The name of the group.</param>
		/// <returns>A matching group, or null if none or more than one are found.</returns>
		public Group GetGroupByName(string name)
		{
			List<Group> ret = groups.FindAll(g => g.Name == name);
			return ret.Count == 1 ? ret[0] : null;
		}

		/// <summary>
		/// Adds a group with name and permissions if it does not exist.
		/// </summary>
		/// <param name="name">The name of the group.</param>
		/// <param name="parentname">The parent group's name.</param>
		/// <param name="permissions">A CSV string containing a list of permissions.</param>
		/// <param name="chatcolor">The chat color in the RRR,GGG,BBB format.</param>
		/// <returns>Whether the group was successfully added.</returns>
		[Obsolete("Use SaveGroup() instead.")]
		public bool AddGroup(string name, string parentname, string permissions, string chatcolor)
		{
			if (GroupExists(name))
			{
				throw new GroupExistsException(name);
			}

			var group = new Group(name, null, chatcolor, permissions);
			if (!String.IsNullOrWhiteSpace(parentname))
			{
				Group parent = groups.FirstOrDefault(gp => gp.Name == parentname);
				if (parent == null || name == parentname)
				{
					var error = "Invalid parent '{0}' for group '{1}'.".SFormat(parentname, group.Name);
					TShock.Log.ConsoleError(error);
					throw new GroupManagerException(error);
				}
				group.Parent = parent;
			}

			string query = (TShock.Config.StorageType.ToLower() == "sqlite")
							? "INSERT OR IGNORE INTO GroupList (GroupName, Parent, Commands, ChatColor) VALUES (@0, @1, @2, @3);"
							: "INSERT IGNORE INTO GroupList SET GroupName=@0, Parent=@1, Commands=@2, ChatColor=@3";
			try
			{
				if (database.Query(query, name, parentname, permissions, chatcolor) == 1)
				{
					groups.Add(group);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new GroupManagerException("Failed to add group '" + name + "'.", ex);
			}
		}

		/// <summary>
		/// Adds group with name and permissions if it does not exist.
		/// </summary>
		/// <param name="name">name of group</param>
		/// <param name="parentname">parent of group</param>
		/// <param name="permissions">permissions</param>
		/// <param name="chatcolor">chatcolor</param>
		/// <param name="exceptions">exceptions true indicates use exceptions for errors false otherwise</param>
		[Obsolete("Use SaveGroup() instead.")]
		public String AddGroup(String name, string parentname, String permissions, String chatcolor = Group.defaultChatColor, bool exceptions = false)
		{
			if (GroupExists(name))
			{
				if (exceptions)
					throw new GroupExistsException(name);
				return "Error: Group already exists; unable to add group.";
			}

			var group = new Group(name, null, chatcolor);
			group.Permissions = permissions;
			if (!string.IsNullOrWhiteSpace(parentname))
			{
				var parent = groups.FirstOrDefault(gp => gp.Name == parentname);
				if (parent == null || name == parentname)
				{
					var error = "Invalid parent {0} for group {1}".SFormat(parentname, group.Name);
					if (exceptions)
						throw new GroupManagerException(error);
					TShock.Log.ConsoleError(error);
					return error;
				}
				group.Parent = parent;
			}

			string query = (TShock.Config.StorageType.ToLower() == "sqlite")
							? "INSERT OR IGNORE INTO GroupList (GroupName, Parent, Commands, ChatColor) VALUES (@0, @1, @2, @3);"
							: "INSERT IGNORE INTO GroupList SET GroupName=@0, Parent=@1, Commands=@2, ChatColor=@3";
			if (database.Query(query, name, parentname, permissions, chatcolor) == 1)
			{
				groups.Add(group);
				return "Group " + name + " has been created successfully.";
			}
			else if (exceptions)
				throw new GroupManagerException("Failed to add group '" + name + ".'");

			return "";
		}

		[Obsolete("Use SaveGroup() instead.")]
		public String AddGroup(String name, String permissions)
		{
			return AddGroup(name, null, permissions, Group.defaultChatColor, false);
		}

		[Obsolete("Use RemoveGroup() instead.")]
		public String DeleteGroup(String name, bool exceptions = false)
		{
			if (!GroupExists(name))
			{
				if (exceptions)
					throw new InvalidGroupException(name);
				return "Error: Group doesn't exist.";
			}

			if (database.Query("DELETE FROM GroupList WHERE GroupName=@0", name) == 1)
			{
				groups.Remove(TShock.Utils.GetGroup(name));
				return "Group " + name + " has been deleted successfully.";
			}
			else if (exceptions)
				throw new GroupManagerException("Failed to delete group '" + name + ".'");

			return "";
		}

		/// <summary>
		/// Removes a group.
		/// </summary>
		/// <param name="name">The name of the group to remove.</param>
		/// <returns>Whether the group was successfully removed.</returns>
		public bool RemoveGroup(string name)
		{
			Group group = GetGroupByName(name);
			if (group == null)
				throw new InvalidGroupException(name);

			string query = "DELETE FROM GroupList WHERE GroupName=@0";
			try
			{
				if (database.Query(query, name) == 1)
				{
					groups.Remove(group);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new GroupManagerException("Failed to remove group '" + name + "'.", ex);
			}
		}

		/// <summary>
		/// Saves a group to the database.
		/// Using this for an existing group will update the data.
		/// </summary>
		/// <exception cref="InvalidParentException">Thrown if the parent group isn't valid or doesn't exist.</exception>
		/// <exception cref="GroupManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <param name="group">The group to save.</param>
		/// <returns>True if the group was saved, or false if the query affected an unexpected amount of rows.</returns>
		public bool SaveGroup(Group group)
		{
			// Parent checks
			if (group.Parent != null)
			{
				Group parent = GetGroupByName(group.ParentName);
				if (parent == group)
					throw new InvalidParentException(group.Name, parent.Name);

				// Check if the new parent would cause loops
				List<Group> groupChain = new List<Group> { group, parent };
				Group checkingGroup = parent.Parent;
				while (checkingGroup != null)
				{
					if (groupChain.Contains(checkingGroup))
						throw new InvalidParentException(group.Name, parent.Name);

					groupChain.Add(checkingGroup);
					checkingGroup = checkingGroup.Parent;
				}
			}

			string query;
			if (GroupExists(group.Name))
				query = "UPDATE Users SET Parent=@1, Commands=@2, ChatColor=@3, Prefix=@4, Suffix=@5 WHERE GroupName=@0";
			else
			{
				query = (TShock.Config.StorageType.ToLower() == "sqlite")
					? "INSERT OR IGNORE INTO GroupList (GroupName, Parent, Commands, ChatColor, Prefix, Suffix) VALUES (@0, @1, @2, @3, @4, @5)"
					: "INSERT IGNORE INTO GroupList SET GroupName=@0, Parent=@1, Commands=@2, ChatColor=@3, Prefix=@4, Suffix=@5";
			}
			try
			{
				if (database.Query(query, group.Name, group.ParentName, group.Permissions, group.ChatColor, group.Prefix, group.Suffix) == 1)
				{
					groups.RemoveAll(g => g.Name == group.Name);
					groups.Add(group);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new GroupManagerException("Failed to save group '{0}'.".SFormat(group.Name), ex);
			}
		}

		/// <summary>
		/// Updates a group including permissions.
		/// </summary>
		/// <param name="name">The name of the group to update.</param>
		/// <param name="parentname">The group's parent group.</param>
		/// <param name="permissions">The group's permissions as a CSV string.</param>
		/// <param name="chatcolor">The chat color in the RRR,GGG,BBB format.</param>
		/// <param name="suffix">The group's suffix.</param>
		/// <param name="prefix">The group's prefix.</param>
		/// <returns>Whether the group was successfully updated.</returns>
		[Obsolete("Use SaveGroup() instead.")]
		public bool UpdateGroup(string name, string parentname, string permissions, string chatcolor, string suffix, string prefix)
		{
			Group group = GetGroupByName(name);
			if (group == null)
				throw new InvalidGroupException(name);

			Group parent = null;
			if (!String.IsNullOrWhiteSpace(parentname))
			{
				parent = GetGroupByName(parentname);
				if (parent == null || parent == group)
					throw new GroupManagerException("Invalid parent '{0}' for group '{1}'.".SFormat(parentname, name));

				// Check if the new parent would cause loops.
				List<Group> groupChain = new List<Group> { group, parent };
				Group checkingGroup = parent.Parent;
				while (checkingGroup != null)
				{
					if (groupChain.Contains(checkingGroup))
						throw new GroupManagerException(
							String.Format("Invalid parent '{0}' for group '{1}' would cause loops in the parent chain.", parentname, name));

					groupChain.Add(checkingGroup);
					checkingGroup = checkingGroup.Parent;
				}
			}

			// Ensure any group validation is also persisted to the DB.
			var newGroup = new Group(name, parent, chatcolor, permissions);
			newGroup.Prefix = prefix;
			newGroup.Suffix = suffix;
			string query = "UPDATE GroupList SET Parent=@0, Commands=@1, ChatColor=@2, Suffix=@3, Prefix=@4 WHERE GroupName=@5";
			try
			{
				if (database.Query(query, parentname, newGroup.Permissions, newGroup.ChatColor, suffix, prefix, name) == 1)
				{
					group.ChatColor = chatcolor;
					group.Permissions = permissions;
					group.Parent = parent;
					group.Prefix = prefix;
					group.Suffix = suffix;
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new GroupManagerException(String.Format("Failed to update group '{0}'.", name), ex);
			}
		}

		/// <summary>
		/// Update the permission list of a group.
		/// </summary>
		/// <exception cref="InvalidGroupException">Thrown if no group by the given name exists in the database.</exception>
		/// <exception cref="GroupManagerException">Thrown when the query operation fails. Contains the inner exception.</exception>
		/// <param name="name">The group's name.</param>
		/// <param name="permissions">The new list of permissions.</param>
		/// <returns>True of the permissions were updated, or false if the query affected an unexpected number of rows.</returns>
		public bool UpdatePermissions(string name, List<string> permissions)
		{
			Group group = GetGroupByName(name);
			if (group == null)
				throw new InvalidGroupException(name);

			string query = "UPDATE GroupList SET Commands=@1 WHERE GroupName=@0";
			try
			{
				if (database.Query(query, name, permissions) == 1)
				{
					group.SetPermissions(permissions);
					return true;
				}
				else
					return false;
			}
			catch (Exception ex)
			{
				throw new GroupManagerException("Failed to update permissions for group '{0}'.".SFormat(name), ex);
			}
		}

		[Obsolete("Use UpdatePermissions() instead.")]
		public String AddPermissions(String name, List<String> permissions)
		{
			if (!GroupExists(name))
				return "Error: Group doesn't exist.";

			var group = TShock.Utils.GetGroup(name);
			var oldperms = group.Permissions; // Store old permissions in case of error
			permissions.ForEach(p => group.AddPermission(p));
 
			if (database.Query("UPDATE GroupList SET Commands=@0 WHERE GroupName=@1", group.Permissions, name) == 1)
				return "Group " + name + " has been modified successfully.";

			// Restore old permissions so DB and internal object are in a consistent state
			group.Permissions = oldperms;
			return "";
		}

		[Obsolete("Use UpdatePermissions() instead.")]
		public String DeletePermissions(String name, List<String> permissions)
		{
			if (!GroupExists(name))
				return "Error: Group doesn't exist.";

			var group = TShock.Utils.GetGroup(name);
			var oldperms = group.Permissions; // Store old permissions in case of error
			permissions.ForEach(p => group.RemovePermission(p));

			if (database.Query("UPDATE GroupList SET Commands=@0 WHERE GroupName=@1", group.Permissions, name) == 1)
				return "Group " + name + " has been modified successfully.";
	
			// Restore old permissions so DB and internal object are in a consistent state
			group.Permissions = oldperms;
			return "";
		}

		/// <summary>
		/// Reloads the group list by reading from the database.
		/// </summary>
		public void LoadPermisions()
		{
			try
			{
				List<Group> newGroups = new List<Group>(groups.Count);
				Dictionary<string,string> newGroupParents = new Dictionary<string, string>(groups.Count);
				using (var reader = database.QueryReader("SELECT * FROM GroupList"))
				{
					while (reader.Read())
					{
						string groupName = reader.Get<string>("GroupName");
						if (groupName == "superadmin")
						{
							TShock.Log.ConsoleInfo("WARNING: Group 'superadmin' is defined in the database even though it's a reserved group name.");
							continue;
						}

						newGroups.Add(new Group(groupName, null, reader.Get<string>("ChatColor"), reader.Get<string>("Commands")) {
							Prefix = reader.Get<string>("Prefix"),
							Suffix = reader.Get<string>("Suffix"),
						});

						try
						{
							newGroupParents.Add(groupName, reader.Get<string>("Parent"));
						}
						catch (ArgumentException)
						{
							// Just in case somebody messed with the unique primary key.
							TShock.Log.ConsoleError("ERROR: Group name '{0}' occurs more than once. Keeping current group settings.");
							return;
						}
					}
				}

				try
				{
					// Get rid of deleted groups.
					for (int i = 0; i < groups.Count; i++)
						if (newGroups.All(g => g.Name != groups[i].Name))
							groups.RemoveAt(i--);

					// Apply changed group settings while keeping the current instances and add new groups.
					foreach (Group newGroup in newGroups)
					{
						Group currentGroup = groups.FirstOrDefault(g => g.Name == newGroup.Name);
						if (currentGroup != null)
							newGroup.AssignTo(currentGroup);
						else
							groups.Add(newGroup);
					}

					// Resolve parent groups.
					Debug.Assert(newGroups.Count == newGroupParents.Count);
					for (int i = 0; i < groups.Count; i++)
					{
						Group group = groups[i];
						string parentGroupName;
						if (!newGroupParents.TryGetValue(group.Name, out parentGroupName) || String.IsNullOrEmpty(parentGroupName))
							continue;

						group.Parent = groups.FirstOrDefault(g => g.Name == parentGroupName);
						if (group.Parent == null)
						{
							TShock.Log.ConsoleError(
								"ERROR: Group '{0}' is referencing non existent parent group '{1}', parent reference was removed.", 
								group.Name, parentGroupName);
						}
						else
						{
							if (group.Parent == group)
								TShock.Log.ConsoleInfo(
									"WARNING: Group '{0}' is referencing itself as parent group, parent reference was removed.", group.Name);

							List<Group> groupChain = new List<Group> { group };
							Group checkingGroup = group;
							while (checkingGroup.Parent != null)
							{
								if (groupChain.Contains(checkingGroup.Parent))
								{
									TShock.Log.ConsoleError(
										"ERROR: Group '{0}' is referencing parent group '{1}' which is already part of the parent chain. Parent reference removed.",
										checkingGroup.Name, checkingGroup.Parent.Name);

									checkingGroup.Parent = null;
									break;
								}
								groupChain.Add(checkingGroup);
								checkingGroup = checkingGroup.Parent;
							}
						}
					}
				}
				finally
				{
					if (!groups.Any(g => g is SuperAdminGroup))
						groups.Add(new SuperAdminGroup());
				}
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleError("Error on reloading groups: " + ex);
			}
		}
	}

	/// <summary>
	/// An exception generated by the group manager.
	/// </summary>
	[Serializable]
	public class GroupManagerException : Exception
	{
		/// <summary>
		/// Creates a new GroupManagerException with the given message.
		/// </summary>
		/// <param name="message">The exception's message.</param>
		public GroupManagerException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Creates a new GroupManagerException with the given message and inner exception.
		/// </summary>
		/// <param name="message">The exception's message.</param>
		/// <param name="inner">The inner exception in the stack.</param>
		public GroupManagerException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	/// <summary>
	/// An exception for handling violations of the unique group naming constraint.
	/// </summary>
	[Serializable]
	public class GroupExistsException : GroupManagerException
	{
		/// <summary>
		/// Creates a new GroupExistsException for the given group name.
		/// </summary>
		/// <param name="name">The name of the duplicate group.</param>
		public GroupExistsException(string name)
			: base("Group '" + name + "' already exists.")
		{
		}
	}

	/// <summary>
	/// An exception for handling cases where an unexistant group is accessed.
	/// </summary>
	[Serializable]
	public class InvalidGroupException : GroupManagerException
	{
		/// <summary>
		/// Creates a new InvalidGroupException for the given group name.
		/// </summary>
		/// <param name="name">The name of the group that doesn't exist.</param>
		public InvalidGroupException(string name)
			: base("Group '" + name + "' does not exist.")
		{
		}
	}

	/// <summary>
	/// An exception for handling invalid parenting, such as when a group is its own parent.
	/// </summary>
	[Serializable]
	public class InvalidParentException : GroupManagerException
	{
		/// <summary>
		/// Creates a new InvalidParentException for the given group and possible parent.
		/// </summary>
		/// <param name="group">The name of the group whose parent is invalid.</param>
		/// <param name="parent">The name of the possible parent group.</param>
		public InvalidParentException(string group, string parent)
			: base("Invalid parent '{0}' for group '{1}'.".SFormat(parent, group))
		{
		}
	}
} 
