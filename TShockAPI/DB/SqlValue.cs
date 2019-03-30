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

using System.Collections.Generic;
using System.Data;

namespace TShockAPI.DB
{
	public class SqlValue
	{
		public string Name { get; set; }
		public object Value { get; set; }

		public SqlValue(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}

	public class SqlTableEditor
	{
		private IDbConnection database;
		private IQueryBuilder creator;

		public SqlTableEditor(IDbConnection db, IQueryBuilder provider)
		{
			database = db;
			creator = provider;
		}

		public void UpdateValues(string table, List<SqlValue> values, List<SqlValue> wheres)
		{
			database.Query(creator.UpdateValue(table, values, wheres));
		}

		public void InsertValues(string table, List<SqlValue> values)
		{
			database.Query(creator.InsertValues(table, values));
		}

		public List<object> ReadColumn(string table, string column, List<SqlValue> wheres)
		{
			List<object> values = new List<object>();

			using (var reader = database.QueryReader(creator.ReadColumn(table, wheres)))
			{
				while (reader.Read())
					values.Add(reader.Reader.Get<object>(column));
			}

			return values;
		}
	}
}