using System;
using System.Collections.Generic;

namespace TShockAPI.DB
{
	/// <summary>
	/// Sql index.
	/// </summary>
	public class SqlIndex
	{
		/// <summary>
		/// Gets or sets the index name.
		/// </summary>
		/// <value>The index name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the name of the table for the index.
		/// </summary>
		/// <value>The name of the table.</value>
		public string Table { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="TShockAPI.DB.SqlIndex"/> requires unique index values.
		/// </summary>
		/// <value><c>true</c> if unique; otherwise, <c>false</c>.</value>
		public bool Unique { get; set; }

		/// <summary>
		/// Gets or sets the table columns.
		/// </summary>
		/// <value>The table columns.</value>
		/// <remarks>Column order matters, hence the List</remarks>
		public List<SqlIndexColumn> Columns { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlIndex"/> class.
		/// </summary>
		/// <param name="name">Name of index.</param>
		/// <param name="table">Table to index.</param>
		/// <param name="columns">Columns to index.</param>
		public SqlIndex(string name, string table, List<SqlIndexColumn> columns)
		{
			Name = name;
			Table = table;
			Columns = columns;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlIndex"/> class.
		/// </summary>
		/// <param name="name">Name of index.</param>
		/// <param name="table">Table to index.</param>
		/// <param name="columns">Columns to index.</param>
		public SqlIndex(string name, string table, params SqlIndexColumn[] columns)
			: this(name, table, new List<SqlIndexColumn>(columns))
		{
		}
	}

	/// <summary>
	/// An index column.
	/// </summary>
	public class SqlIndexColumn
	{
		/// <summary>
		/// Gets or sets the column name.
		/// </summary>
		/// <value>The column name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Length of the data type, null = default
		/// </summary>
		public int? Length { get; set; }

		/// <summary>
		/// Gets or sets the order when used by an index.
		/// </summary>
		/// <value>The order.</value>
		public SortOrder Order { get; set; }

		/// <summary>
		/// Index sort order.
		/// </summary>
		public enum SortOrder
		{
			/// <summary>
			/// Ascending column sort order.
			/// </summary>
			Ascending,
			/// <summary>
			/// Descending column sort order.
			/// </summary>
			Descending
		}

		/// <summary>
		/// Maps enum types to a string representation.
		/// </summary>
		private static readonly Dictionary<SortOrder, string> SortOrderStringMap = new Dictionary<SortOrder, string>
		{
			{ SortOrder.Ascending, "ASC" },
			{ SortOrder.Descending, "DESC" },
		};

		/// <summary>
		/// Action to string.
		/// </summary>
		/// <returns>A string representation of <see cref="TShockAPI.DB.SqlForeignKey.Action"/>.</returns>
		/// <param name="type">Type of action.</param>
		public static string SortOrderToString(SortOrder type)
		{
			string ret;
			if (SortOrderStringMap.TryGetValue(type, out ret))
				return ret;
			throw new NotImplementedException(Enum.GetName(typeof(SortOrder), type));
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlIndexColumn"/> class.
		/// </summary>
		/// <param name="name">Name of column to index.</param>
		/// <param name="length">Length to index.</param>
		public SqlIndexColumn(string name, int? length = null)
			: this(name, SortOrder.Ascending, length)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TShockAPI.DB.SqlIndexColumn"/> class.
		/// </summary>
		/// <param name="name">Name of column to index.</param>
		/// <param name="order">The column sort order.</param>
		/// <param name="length">Length to index.</param>
		public SqlIndexColumn(string name, SortOrder order, int? length = null)
		{
			if (String.IsNullOrEmpty(name))
				throw new ArgumentException("Cannot create an index column that has no name.");

			Name = name;
			Order = order;
			Length = length;
		}
	}
}

