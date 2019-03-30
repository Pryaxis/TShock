/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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
using MySql.Data.MySqlClient;
using TShockAPI.Hooks;

namespace TShockAPI.DB
{
	public class ItemManager
	{
		private IDbConnection database;
		public List<ItemBan> ItemBans = new List<ItemBan>();

		public ItemManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("ItemBans",
			                         new SqlColumn("ItemName", MySqlDbType.VarChar, 50) {Primary = true},
			                         new SqlColumn("AllowedGroups", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
			                                  db.GetSqlType() == SqlType.Sqlite
			                                  	? (IQueryBuilder) new SqliteQueryCreator()
			                                  	: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
			UpdateItemBans();
		}

		public void UpdateItemBans()
		{
			ItemBans.Clear();

			using (var reader = database.QueryReader("SELECT * FROM ItemBans"))
			{
				while (reader != null && reader.Read())
				{
					ItemBan ban = new ItemBan(reader.Get<string>("ItemName"));
					ban.SetAllowedGroups(reader.Get<string>("AllowedGroups"));
					ItemBans.Add(ban);
				}
			}
		}

		public void AddNewBan(string itemname = "")
		{
			try
			{
				database.Query("INSERT INTO ItemBans (ItemName, AllowedGroups) VALUES (@0, @1);",
				               itemname, "");
				if (!ItemIsBanned(itemname, null))
					ItemBans.Add(new ItemBan(itemname));
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
		}

		public void RemoveBan(string itemname)
		{
			if (!ItemIsBanned(itemname, null))
				return;
			try
			{
				database.Query("DELETE FROM ItemBans WHERE ItemName=@0;", itemname);
				ItemBans.Remove(new ItemBan(itemname));
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
		}

		public bool ItemIsBanned(string name)
		{
			if (ItemBans.Contains(new ItemBan(name)))
			{
				return true;
			}
			return false;
		}

		public bool ItemIsBanned(string name, TSPlayer ply)
		{
			ItemBan b = GetItemBanByName(name);
			return b != null &&!b.HasPermissionToUseItem(ply);
		}

		public bool AllowGroup(string item, string name)
		{
			string groupsNew = "";
			ItemBan b = GetItemBanByName(item);
			if (b != null)
			{
				try
				{
					groupsNew = String.Join(",", b.AllowedGroups);
					if (groupsNew.Length > 0)
						groupsNew += ",";
					groupsNew += name;
					b.SetAllowedGroups(groupsNew);

					int q = database.Query("UPDATE ItemBans SET AllowedGroups=@0 WHERE ItemName=@1", groupsNew,
					                       item);
					
					return q > 0;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}

			return false;
		}

		public bool RemoveGroup(string item, string group)
		{
			ItemBan b = GetItemBanByName(item);
			if (b != null)
			{
				try
				{				
					b.RemoveGroup(group);
					string groups = string.Join(",", b.AllowedGroups);
					int q = database.Query("UPDATE ItemBans SET AllowedGroups=@0 WHERE ItemName=@1", groups,
					                       item);
					
					if (q > 0)
						return true;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}
			return false;
		}

		public ItemBan GetItemBanByName(String name)
		{
			for (int i = 0; i < ItemBans.Count; i++)
			{
				if (ItemBans[i].Name == name)
				{
					return ItemBans[i];
				}
			}
			return null;
		}
	}

	public class ItemBan : IEquatable<ItemBan>
	{
		public string Name { get; set; }
		public List<string> AllowedGroups { get; set; }

		public ItemBan(string name)
			: this()
		{
			Name = name;
			AllowedGroups = new List<string>();
		}

		public ItemBan()
		{
			Name = "";
			AllowedGroups = new List<string>();
		}

		public bool Equals(ItemBan other)
		{
			return Name == other.Name;
		}

		public bool HasPermissionToUseItem(TSPlayer ply)
		{
			if (ply == null)
				return false;

			if (ply.HasPermission(Permissions.usebanneditem))
				return true;

			PermissionHookResult hookResult = PlayerHooks.OnPlayerItembanPermission(ply, this);
			if (hookResult != PermissionHookResult.Unhandled)
				return hookResult == PermissionHookResult.Granted;

			var cur = ply.Group;
			var traversed = new List<Group>();
			while (cur != null)
			{
				if (AllowedGroups.Contains(cur.Name))
				{
					return true;
				}
				if (traversed.Contains(cur))
				{
					throw new InvalidOperationException("Infinite group parenting ({0})".SFormat(cur.Name));
				}
				traversed.Add(cur);
				cur = cur.Parent;
			}
			return false;
			// could add in the other permissions in this class instead of a giant if switch.
		}

		public void SetAllowedGroups(String groups)
		{
			// prevent null pointer exceptions
			if (!string.IsNullOrEmpty(groups))
			{
				List<String> groupArr = groups.Split(',').ToList();

				for (int i = 0; i < groupArr.Count; i++)
				{
					groupArr[i] = groupArr[i].Trim();
					//Console.WriteLine(groupArr[i]);
				}
				AllowedGroups = groupArr;
			}
		}

		public bool RemoveGroup(string groupName)
		{
			return AllowedGroups.Remove(groupName);
		}
		
		public override string ToString()
		{
			return Name + (AllowedGroups.Count > 0 ? " (" + String.Join(",", AllowedGroups) + ")" : "");
		}
	}
}