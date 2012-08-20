/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

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
using System.IO;
using System.Linq;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
	public class GroupManager : IEnumerable<Group>
	{
		private IDbConnection database;
		public readonly List<Group> groups = new List<Group>();

		public GroupManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("GroupList",
			                         new SqlColumn("GroupName", MySqlDbType.VarChar, 32) {Primary = true},
			                         new SqlColumn("Parent", MySqlDbType.VarChar, 32),
			                         new SqlColumn("Commands", MySqlDbType.Text),
			                         new SqlColumn("ChatColor", MySqlDbType.Text),
			                         new SqlColumn("Prefix", MySqlDbType.Text),
			                         new SqlColumn("Suffix", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureExists(table);

			// Load Permissions from the DB
			LoadPermisions();

			// Add default groups if they don't exist
			AddDefaultGroup("guest", "", "canbuild,canregister,canlogin,canpartychat,cantalkinthird");
			AddDefaultGroup("default", "guest", "warp,canchangepassword");
			AddDefaultGroup("newadmin", "default", "kick,editspawn,reservedslot");
			AddDefaultGroup("admin", "newadmin",
			         "ban,unban,whitelist,causeevents,spawnboss,spawnmob,managewarp,time,tp,pvpfun,kill,logs,immunetokick,tphere");
			AddDefaultGroup("trustedadmin", "admin", "maintenance,cfg,butcher,item,heal,immunetoban,usebanneditem,manageusers");
			AddDefaultGroup("vip", "default", "reservedslot");
		}

		private void AddDefaultGroup(string name, string parent, string permissions)
		{
			if (!GroupExists(name))
				AddGroup(name, parent, permissions);
		}


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

		public IEnumerator<Group> GetEnumerator()
		{
			return groups.GetEnumerator();
		}

		public Group GetGroupByName(string name)
		{
			var ret = groups.Where(g => g.Name == name);
			return 1 == ret.Count() ? ret.ElementAt(0) : null;
		}

		/// <summary>
		/// Adds group with name and permissions if it does not exist.
		/// </summary>
		/// <param name="name">name of group</param>
		/// <param name="parentname">parent of group</param>
		/// <param name="permissions">permissions</param>
		/// <param name="chatcolor">chatcolor</param>
		/// <param name="exceptions">exceptions true indicates use exceptions for errors false otherwise</param>
		public String AddGroup(String name, string parentname, String permissions, String chatcolor = Group.defaultChatColor, bool exceptions = false)
		{
			if (GroupExists(name))
			{
				if (exceptions)
					throw new GroupExistsException(name);
				return "Error: Group already exists. Use /modgroup to change permissions.";
			}

			var group = new Group(name, null, chatcolor);
			group.Permissions = permissions;
			if (!string.IsNullOrWhiteSpace(parentname))
			{
				var parent = groups.FirstOrDefault(gp => gp.Name == parentname);
				if (parent == null)
				{
					var error = "Invalid parent {0} for group {1}".SFormat(parentname, group.Name);
					if (exceptions)
						throw new GroupManagerException(error);
					Log.ConsoleError(error);
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

		public String AddGroup(String name, String permissions)
		{
			return AddGroup(name, null, permissions, Group.defaultChatColor, false);
		}

#if COMPAT_SIGS
		[Obsolete("This method is for signature compatibility for external code only")]
		public String AddGroup(String name, string parentname, String permissions)
		{
			return AddGroup(name, parentname, permissions, Group.defaultChatColor, false);
		}

		[Obsolete("This method is for signature compatibility for external code only")]
		public String AddGroup(String name, string parentname, String permissions, String chatcolor)
		{
			return AddGroup(name, parentname, permissions, chatcolor, false);
		}
#endif
		/// <summary>
		/// Updates a group including permissions
		/// </summary>
		/// <param name="name">name of the group to update</param>
		/// <param name="parentname">parent of group</param>
		/// <param name="permissions">permissions</param>
		/// <param name="chatcolor">chatcolor</param>
		public void UpdateGroup(string name, string parentname, string permissions, string chatcolor)
		{
			if (!GroupExists(name))
				throw new GroupNotExistException(name);

			Group parent = null;
			if (!string.IsNullOrWhiteSpace(parentname))
			{
				parent = groups.FirstOrDefault(gp => gp.Name == parentname);
				if (null == parent)
					throw new GroupManagerException("Invalid parent {0} for group {1}".SFormat(parentname, name));
			}

			// NOTE: we use newgroup.XYZ to ensure any validation is also persisted to the DB
			var newgroup = new Group(name, parent, chatcolor, permissions);
			string query = "UPDATE GroupList SET Parent=@0, Commands=@1, ChatColor=@2 WHERE GroupName=@3";
			if (database.Query(query, parentname, newgroup.Permissions, string.Format("{0},{1},{2}", newgroup.R, newgroup.G, newgroup.B), name) != 1)
				throw new GroupManagerException("Failed to update group '" + name + "'");

			Group group = TShock.Utils.GetGroup(name);
			group.ChatColor = chatcolor;
			group.Permissions = permissions;
			group.Parent = TShock.Utils.GetGroup(parentname);
		}

#if COMPAT_SIGS
		[Obsolete("This method is for signature compatibility for external code only")]
		public String DeleteGroup(String name)
		{
			return DeleteGroup(name, false);
		}
#endif
		public String DeleteGroup(String name, bool exceptions = false)
		{
			if (!GroupExists(name))
			{
				if (exceptions)
					throw new GroupNotExistException(name);
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

		public void LoadPermisions()
		{
			// Create a temporary list so if there is an error it doesn't override the currently loaded groups with broken groups.
			var tempgroups = new List<Group>();
			tempgroups.Add(new SuperAdminGroup());

			if (groups == null || groups.Count < 2)
			{
				groups.Clear();
				groups.AddRange(tempgroups);
			}

			try
			{
				var groupsparents = new List<Tuple<Group, string>>();
				using (var reader = database.QueryReader("SELECT * FROM GroupList"))
				{
					while (reader.Read())
					{
						var group = new Group(reader.Get<String>("GroupName"), null, reader.Get<String>("ChatColor"), reader.Get<String>("Commands"));
						group.Prefix = reader.Get<String>("Prefix");
						group.Suffix = reader.Get<String>("Suffix");
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
							Log.ConsoleError("Invalid parent {0} for group {1}".SFormat(parentname, group.Name));
							return;
						}
						group.Parent = parent.Item1;
					}
					tempgroups.Add(group);
				}

				groups.Clear();
				groups.AddRange(tempgroups);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
		}
	}

	[Serializable]
	public class GroupManagerException : Exception
	{
		public GroupManagerException(string message)
			: base(message)
		{
		}

		public GroupManagerException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}

	[Serializable]
	public class GroupExistsException : GroupManagerException
	{
		public GroupExistsException(string name)
			: base("Group '" + name + "' already exists")
		{
		}
	}

	[Serializable]
	public class GroupNotExistException : GroupManagerException
	{
		public GroupNotExistException(string name)
			: base("Group '" + name + "' does not exist")
		{
		}
	}
} 
