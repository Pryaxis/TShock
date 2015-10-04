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
using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace TShockAPI.DB
{
	/// <summary>
	/// Base class for constraints.
	/// </summary>
	public abstract class SqlConstraint
	{
		/// <summary>
		/// Gets or sets the constraint name.
		/// </summary>
		/// <value>The constraint name.</value>
		public string Name { get; set; }
	}

	/// <summary>
	/// Sql foreign key.
	/// </summary>
	public class SqlForeignKey : SqlConstraint
	{
		/// <summary>
		/// Gets or sets the table columns.
		/// </summary>
		/// <value>The table columns.</value>
		public IEnumerable<string> Columns { get; set; }

		/// <summary>
		/// Gets or sets the name of the parent table.
		/// </summary>
		/// <value>The name of the parent table.</value>
		public string ParentTable { get; set; }

		/// <summary>
		/// Gets or sets the parent columns.
		/// </summary>
		/// <value>The parent columns.</value>
		public IEnumerable<string> ParentColumns { get; set; }

		/// <summary>
		/// The on delete event.
		/// </summary>
		public OnDelete OnDeleteEvent = new OnDelete(Action.Restrict);

		/// <summary>
		/// The on update event.
		/// </summary>
		public OnUpdate OnUpdateEvent = new OnUpdate(Action.Restrict);

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlForeignKey"/> class.
		/// </summary>
		/// <param name="columns">Table columns.</param>
		/// <param name="parentTable">Parent table name.</param>
		/// <param name="parentColumns">Parent columns.</param>
		public SqlForeignKey(IEnumerable<string> columns, string parentTable, IEnumerable<string> parentColumns)
			: this(columns, parentTable, parentColumns, Action.Restrict, Action.Restrict)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlForeignKey"/> class.
		/// </summary>
		/// <param name="column">Table column.</param>
		/// <param name="parentTable">Parent table name.</param>
		/// <param name="parentColumn">Parent column.</param>
		public SqlForeignKey(string column, string parentTable, string parentColumn)
			: this(column, parentTable, parentColumn, Action.Restrict, Action.Restrict)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlForeignKey"/> class.
		/// </summary>
		/// <param name="column">Table column.</param>
		/// <param name="parentTable">Parent table name.</param>
		/// <param name="parentColumn">Parent column.</param>
		/// <param name="OnDeleteAction">On delete action.</param>
		/// <param name="OnUpdateAction">On update action.</param>
		public SqlForeignKey(string column, string parentTable, string parentColumn, Action OnDeleteAction, Action OnUpdateAction)
			: this(new string[] { column }, parentTable, new string[] { parentColumn }, OnDeleteAction, OnUpdateAction)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlForeignKey"/> class.
		/// </summary>
		/// <param name="parentTable">Parent table name.</param>
		/// <param name="parentColumns">Parent columns.</param>
		/// <param name="columns">Table columns.</param>
		/// <param name="OnDeleteAction">On delete action.</param>
		/// <param name="OnUpdateAction">On update action.</param>
		public SqlForeignKey(IEnumerable<string> columns, string parentTable, IEnumerable<string> parentColumns,
							 Action OnDeleteAction, Action OnUpdateAction)
		{
			if (columns == null || columns.Count() < 1)
				throw new ArgumentException("Cannot create foreign key with no columns.");

			if (String.IsNullOrEmpty(parentTable))
				throw new ArgumentException("Cannot create foreign key without a reference table.");

			if (parentColumns == null || parentColumns.Count() < 1)
				throw new ArgumentException("Cannot create foreign key without columns from the reference table.");

			Columns = columns;
			ParentTable = parentTable;
			ParentColumns = parentColumns;
			OnDeleteEvent.Action = OnDeleteAction;
			OnUpdateEvent.Action = OnUpdateAction;
		}

		/// <summary>
		/// Action types for foreign key events.
		/// </summary>
		public enum Action
		{
			/// <summary>
			/// No action.
			/// </summary>
			NoAction,
			/// <summary>
			/// Restrict.
			/// </summary>
			Restrict,
			/// <summary>
			/// Set NULL.
			/// </summary>
			SetNull,
			/// <summary>
			/// Set default.
			/// </summary>
			SetDefault,
			/// <summary>
			/// Cascade.
			/// </summary>
			Cascade,
		}

		/// <summary>
		/// Base class for foreign key events.
		/// </summary>
		abstract public class CascadeEvent
		{
			/// <summary>
			/// Gets or sets the action.
			/// </summary>
			/// <value>The action.</value>
			public Action Action { get; set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlForeignKey.CascadeEvent"/> class.
			/// </summary>
			public CascadeEvent()
			{
				this.Action = Action.Restrict;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlForeignKey.CascadeEvent"/> class.
			/// </summary>
			/// <param name="action">Action.</param>
			public CascadeEvent(Action action)
			{
				this.Action = action;
			}
		}

		/// <summary>
		/// On delete foreign key event.
		/// </summary>
		public class OnDelete : CascadeEvent
		{
			public OnDelete() : base()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlForeignKey.OnDelete"/> class.
			/// </summary>
			/// <param name="action">Action.</param>
			public OnDelete(Action action) : base(action)
			{
			}

			/// <summary>
			/// Returns a <see cref="System.String"/> that represents the current <see cref="TShockAPI.DB.SqlForeignKey.OnDelete"/>.
			/// </summary>
			/// <returns>A <see cref="System.String"/> that represents the current <see cref="TShockAPI.DB.SqlForeignKey.OnDelete"/>.</returns>
			public override string ToString()
			{
				return "ON DELETE " + ActionToString(Action);
			}
		}

		/// <summary>
		/// On update foreign key event.
		/// </summary>
		public class OnUpdate : CascadeEvent
		{
			public OnUpdate() : base()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlForeignKey.OnUpdate"/> class.
			/// </summary>
			/// <param name="action">Action.</param>
			public OnUpdate(Action action) : base(action)
			{
			}

			/// <summary>
			/// Returns a <see cref="System.String"/> that represents the current <see cref="TShockAPI.DB.SqlForeignKey.OnUpdate"/>.
			/// </summary>
			/// <returns>A <see cref="System.String"/> that represents the current <see cref="TShockAPI.DB.SqlForeignKey.OnUpdate"/>.</returns>
			public override string ToString()
			{
				return "ON UPDATE " + ActionToString(Action);
			}
		}

		/// <summary>
		/// Maps enum types to a string representation.
		/// </summary>
		private static readonly Dictionary<Action, string> ActionStringMap = new Dictionary<Action, string>
		{
			{ Action.NoAction, "NO ACTION" },
			{ Action.Restrict, "RESTRICT" },
			{ Action.SetNull, "SET NULL" },
			{ Action.SetDefault, "SET DEFAULT" },
			{ Action.Cascade, "CASCADE" },
		};

		/// <summary>
		/// Action to string.
		/// </summary>
		/// <returns>A string representation of <see cref="TShockAPI.DB.SqlForeignKey.Action"/>.</returns>
		/// <param name="type">Type of action.</param>
		public static string ActionToString(Action type)
		{
			string ret;
			if (ActionStringMap.TryGetValue(type, out ret))
				return ret;
			throw new NotImplementedException(Enum.GetName(typeof(Action), type));
		}
	}
}

