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
	public class TileManager
	{
		private IDbConnection database;
		public List<TileBan> TileBans = new List<TileBan>();

		public TileManager(IDbConnection db)
		{
			database = db;

			var table = new SqlTable("TileBans",
				new SqlColumn("TileId", MySqlDbType.Int32) { Primary = true },
				new SqlColumn("AllowedGroups", MySqlDbType.Text)
				);
			var creator = new SqlTableCreator(db,
				db.GetSqlType() == SqlType.Sqlite
					? (IQueryBuilder)new SqliteQueryCreator()
					: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
			UpdateBans();
		}

		public void UpdateBans()
		{
			TileBans.Clear();

			using (var reader = database.QueryReader("SELECT * FROM TileBans"))
			{
				while (reader != null && reader.Read())
				{
					TileBan ban = new TileBan((short)reader.Get<Int32>("TileId"));
					ban.SetAllowedGroups(reader.Get<string>("AllowedGroups"));
					TileBans.Add(ban);
				}
			}
		}

		public void AddNewBan(short id = 0)
		{
			try
			{
				database.Query("INSERT INTO TileBans (TileId, AllowedGroups) VALUES (@0, @1);",
					id, "");

				if (!TileIsBanned(id, null))
					TileBans.Add(new TileBan(id));
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
		}

		public void RemoveBan(short id)
		{
			if (!TileIsBanned(id, null))
				return;
			try
			{
				database.Query("DELETE FROM TileBans WHERE TileId=@0;", id);
				TileBans.Remove(new TileBan(id));
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
		}

		public bool TileIsBanned(short id)
		{
			if (TileBans.Contains(new TileBan(id)))
			{
				return true;
			}
			return false;
		}

		public bool TileIsBanned(short id, TSPlayer ply)
		{
			if (TileBans.Contains(new TileBan(id)))
			{
				TileBan b = GetBanById(id);
				return !b.HasPermissionToPlaceTile(ply);
			}
			return false;
		}

		public bool AllowGroup(short id, string name)
		{
			string groupsNew = "";
			TileBan b = GetBanById(id);
			if (b != null)
			{
				try
				{
					groupsNew = String.Join(",", b.AllowedGroups);
					if (groupsNew.Length > 0)
						groupsNew += ",";
					groupsNew += name;
					b.SetAllowedGroups(groupsNew);

					int q = database.Query("UPDATE TileBans SET AllowedGroups=@0 WHERE TileId=@1", groupsNew,
						id);

					return q > 0;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
				}
			}

			return false;
		}

		public bool RemoveGroup(short id, string group)
		{
			TileBan b = GetBanById(id);
			if (b != null)
			{
				try
				{
					b.RemoveGroup(group);
					string groups = string.Join(",", b.AllowedGroups);
					int q = database.Query("UPDATE TileBans SET AllowedGroups=@0 WHERE TileId=@1", groups,
						id);

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

		public TileBan GetBanById(short id)
		{
			foreach (TileBan b in TileBans)
			{
				if (b.ID == id)
				{
					return b;
				}
			}
			return null;
		}
	}

	public class TileBan : IEquatable<TileBan>
	{
		public short ID { get; set; }
		public List<string> AllowedGroups { get; set; }

		public TileBan(short id)
			: this()
		{
			ID = id;
			AllowedGroups = new List<string>();
		}

		public TileBan()
		{
			ID = 0;
			AllowedGroups = new List<string>();
		}

		public bool Equals(TileBan other)
		{
			return ID == other.ID;
		}

		public bool HasPermissionToPlaceTile(TSPlayer ply)
		{
			if (ply == null)
				return false;

			if (ply.HasPermission(Permissions.canusebannedtiles))
				return true;

			PermissionHookResult hookResult = PlayerHooks.OnPlayerTilebanPermission(ply, this);
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
			return ID + (AllowedGroups.Count > 0 ? " (" + String.Join(",", AllowedGroups) + ")" : "");
		}
	}
}