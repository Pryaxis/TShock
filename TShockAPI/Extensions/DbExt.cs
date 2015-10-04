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
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Mono.Data.Sqlite;

namespace TShockAPI.DB
{
	public static class DbExt
	{
		/// <summary>
		/// Enforces the foreign key support for SQLite,
		/// </summary>
		/// <param name="db">The database instance.</param>
		internal static void EnforceForeignKeys(this SqliteConnection db)
		{
			if (db.GetSqlType() == SqlType.Sqlite && TShock.Config.SqliteEnforceForeignKeys)
			{
				((SqliteConnection)db).StateChange += delegate(object sender, StateChangeEventArgs e)
				{
					if (e.CurrentState != ConnectionState.Open)
						return;

					// Important: use a new connection, do not Clone() or CloneEx()
					using (var conn = new SqliteConnection(db.ConnectionString))
					{
						conn.Open();
						using (var cmd = conn.CreateCommand())
						{
							cmd.CommandText = "PRAGMA foreign_keys = ON";
							cmd.ExecuteNonQuery();

							cmd.CommandText = "PRAGMA foreign_keys";
							bool? enabled = SqliteConvert.ToBoolean(cmd.ExecuteScalar());
							if (!enabled.HasValue || enabled == false)
								TerrariaApi.Server.ServerApi.LogWriter.PluginWriteLine(TShock.instance,
										"SQLite isn't enforcing foreign keys, data integrity can't be guaranteed!", TraceLevel.Warning);
						}
					}
				};
			}
		}

		/// <summary>
		/// Executes a query on a database.
		/// </summary>
		/// <param name="olddb">Database to query</param>
		/// <param name="query">Query string with parameters as @0, @1, etc.</param>
		/// <param name="args">Parameters to be put in the query</param>
		/// <returns>Rows affected by query</returns>
		[SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public static int Query(this IDbConnection olddb, string query, params object[] args)
		{
			using (var db = olddb.CloneEx())
			{
				db.Open();
				using (var com = db.CreateCommand())
				{
					com.CommandText = query;
					for (int i = 0; i < args.Length; i++)
						com.AddParameter("@" + i, args[i]);

					return com.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Executes a query on a database.
		/// </summary>
		/// <param name="olddb">Database to query</param>
		/// <param name="query">Query string with parameters as @0, @1, etc.</param>
		/// <param name="args">Parameters to be put in the query</param>
		/// <returns>Query result as IDataReader</returns>
		[SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
		public static QueryResult QueryReader(this IDbConnection olddb, string query, params object[] args)
		{
			var db = olddb.CloneEx();
			db.Open();
			using (var com = db.CreateCommand())
			{
				com.CommandText = query;
				for (int i = 0; i < args.Length; i++)
					com.AddParameter("@" + i, args[i]);

				return new QueryResult(db, com.ExecuteReader());
			}
		}

		public static QueryResult QueryReaderDict(this IDbConnection olddb, string query, Dictionary<string, object> values)
		{
			var db = olddb.CloneEx();
			db.Open();
			using (var com = db.CreateCommand())
			{
				com.CommandText = query;
				foreach (var kv in values)
					com.AddParameter("@" + kv.Key, kv.Value);

				return new QueryResult(db, com.ExecuteReader());
			}
		}

		public static IDbDataParameter AddParameter(this IDbCommand command, string name, object data)
		{
			var parm = command.CreateParameter();
			parm.ParameterName = name;
			parm.Value = data;
			command.Parameters.Add(parm);
			return parm;
		}

		public static IDbConnection CloneEx(this IDbConnection conn)
		{
			var clone = (IDbConnection)Activator.CreateInstance(conn.GetType(), conn.ConnectionString);

			if (conn.GetSqlType() == SqlType.Sqlite)
				((SqliteConnection)clone).EnforceForeignKeys();

			return clone;
		}

		public static SqlType GetSqlType(this IDbConnection conn)
		{
			var name = conn.GetType().Name;
			if (name == "SqliteConnection")
				return SqlType.Sqlite;
			if (name == "MySqlConnection")
				return SqlType.Mysql;
			return SqlType.Unknown;
		}

		private static readonly Dictionary<Type, Func<IDataReader, int, object>> ReadFuncs = new Dictionary
			<Type, Func<IDataReader, int, object>>
		                                                                                     	{
		                                                                                     		{
		                                                                                     			typeof (bool),
		                                                                                     			(s, i) => s.GetBoolean(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (bool?),
		                                                                                     			(s, i) => s.IsDBNull(i) ? null : (object)s.GetBoolean(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (byte),
		                                                                                     			(s, i) => s.GetByte(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (byte?),
		                                                                                     			(s, i) => s.IsDBNull(i) ? null : (object)s.GetByte(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (Int16),
		                                                                                     			(s, i) => s.GetInt16(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (Int16?),
		                                                                                     			(s, i) => s.IsDBNull(i) ? null : (object)s.GetInt16(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (Int32),
		                                                                                     			(s, i) => s.GetInt32(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (Int32?),
		                                                                                     			(s, i) => s.IsDBNull(i) ? null : (object)s.GetInt32(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (Int64),
		                                                                                     			(s, i) => s.GetInt64(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (Int64?),
		                                                                                     			(s, i) => s.IsDBNull(i) ? null : (object)s.GetInt64(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (string),
		                                                                                     			(s, i) => s.GetString(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (decimal),
		                                                                                     			(s, i) => s.GetDecimal(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (decimal?),
		                                                                                     			(s, i) => s.IsDBNull(i) ? null : (object)s.GetDecimal(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (float),
		                                                                                     			(s, i) => s.GetFloat(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (float?),
		                                                                                     			(s, i) => s.IsDBNull(i) ? null : (object)s.GetFloat(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (double),
		                                                                                     			(s, i) => s.GetDouble(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (double?),
		                                                                                     			(s, i) => s.IsDBNull(i) ? null : (object)s.GetDouble(i)
		                                                                                     			},
		                                                                                     		{
		                                                                                     			typeof (object),
		                                                                                     			(s, i) => s.GetValue(i)
		                                                                                     			},
		                                                                                     	};

		public static T Get<T>(this IDataReader reader, string column)
		{
			return reader.Get<T>(reader.GetOrdinal(column));
		}

		public static T Get<T>(this IDataReader reader, int column)
		{
			if (reader.IsDBNull(column))
				return default(T);

			if (ReadFuncs.ContainsKey(typeof (T)))
				return (T) ReadFuncs[typeof (T)](reader, column);

			throw new NotImplementedException();
		}

		/// <summary>
		/// Performs the collection of queries within a transaction.
		/// </summary>
		/// <returns><c>true</c>, if transaction was successful, <c>false</c> otherwise.</returns>
		/// <param name="olddb">A database instance.</param>
		/// <param name="queries">A list of SQL queries.</param>
		public static bool AsTransaction(this IDbConnection olddb, IEnumerable<string> queries)
		{
			using (var db = olddb.CloneEx())
			{
				db.Open();
				using (var transaction = db.BeginTransaction())
				using (var com = db.CreateCommand())
				{
					try
					{
						queries.ForEach(query => {
							com.CommandText = query;
							com.ExecuteNonQuery();
						});

						transaction.Commit();
					}
					catch (Exception ex)
					{
						TShock.Log.Error(ex.ToString());
						transaction.Rollback();
						return false;
					}
					return true;
				}
			}
		}
	}

	public enum SqlType
	{
		Unknown,
		Sqlite,
		Mysql
	}

	public class QueryResult : IDisposable
	{
		public IDbConnection Connection { get; protected set; }
		public IDataReader Reader { get; protected set; }

		public QueryResult(IDbConnection conn, IDataReader reader)
		{
			Connection = conn;
			Reader = reader;
		}

		~QueryResult()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (Reader != null)
				{
					Reader.Dispose();
					Reader = null;
				}
				if (Connection != null)
				{
					Connection.Dispose();
					Connection = null;
				}
			}
		}

		public bool Read()
		{
			if (Reader == null)
				return false;
			return Reader.Read();
		}

		public T Get<T>(string column)
		{
			if (Reader == null)
				return default(T);
			return Reader.Get<T>(Reader.GetOrdinal(column));
		}
	}
}