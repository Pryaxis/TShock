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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using MySql.Data.MySqlClient;
using TShockAPI.DB;

namespace TShockAPI
{
	struct LogInfo
	{
		public string timestamp;
		public string message;
		public string caller;
		public TraceLevel logLevel;

		public override string ToString()
		{
			return string.Format("Message: {0}: {1}: {2}",
				caller, logLevel.ToString().ToUpper(), message);
		}
	}

	/// <summary>
	/// Class inheriting ILog for writing logs to TShock's SQL database
	/// </summary>
	public class SqlLog : ILog, IDisposable
	{
		private readonly IDbConnection _database;
		private readonly TextLog _backupLog;
		private readonly List<LogInfo> _failures = new List<LogInfo>(TShock.Config.RevertToTextLogsOnSqlFailures);
		private bool _useTextLog;

		public string FileName { get; set; }

		/// <summary>
		/// Sets the database connection and the initial log level.
		/// </summary>
		/// <param name="db">Database connection</param>
		/// <param name="textlogFilepath">File path to a backup text log in case the SQL log fails</param>
		/// <param name="clearTextLog"></param>
		public SqlLog(IDbConnection db, string textlogFilepath, bool clearTextLog)
		{
			FileName = string.Format("{0}://database", db.GetSqlType());
			_database = db;
			_backupLog = new TextLog(textlogFilepath, clearTextLog);

			var table = new SqlTable("Logs",
				new SqlColumn("ID", MySqlDbType.Int32) {AutoIncrement = true, Primary = true},
				new SqlColumn("TimeStamp", MySqlDbType.Text),
				new SqlColumn("LogLevel", MySqlDbType.Int32),
				new SqlColumn("Caller", MySqlDbType.Text),
				new SqlColumn("Message", MySqlDbType.Text)
				);

			var creator = new SqlTableCreator(db,
				db.GetSqlType() == SqlType.Sqlite
					? (IQueryBuilder) new SqliteQueryCreator()
					: new MysqlQueryCreator());
			creator.EnsureTableStructure(table);
		}

		public bool MayWriteType(TraceLevel type)
		{
			return type != TraceLevel.Off;
		}

		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Data(string message)
		{
			Write(message, TraceLevel.Verbose);
		}

		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Data(string format, params object[] args)
		{
			Data(string.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Error(string message)
		{
			Write(message, TraceLevel.Error);
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Error(string format, params object[] args)
		{
			Error(string.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void ConsoleError(string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Write(message, TraceLevel.Error);
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void ConsoleError(string format, params object[] args)
		{
			ConsoleError(string.Format(format, args));
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Warn(string message)
		{
			Write(message, TraceLevel.Warning);
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Warn(string format, params object[] args)
		{
			Warn(string.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Info(string message)
		{
			Write(message, TraceLevel.Info);
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Info(string format, params object[] args)
		{
			Info(string.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file. Also outputs to the console.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void ConsoleInfo(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Write(message, TraceLevel.Info);
		}

		/// <summary>
		/// Writes an informative string to the log file. Also outputs to the console.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void ConsoleInfo(string format, params object[] args)
		{
			ConsoleInfo(string.Format(format, args));
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Debug(string message)
		{
#if DEBUG
			Write(message, TraceLevel.Verbose);
#endif
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Debug(string format, params object[] args)
		{
#if DEBUG
			Debug(string.Format(format, args));
#endif
		}

		public void Write(string message, TraceLevel level)
		{
			if (!MayWriteType(level))
				return;

			var caller = "TShock";

			var frame = new StackTrace().GetFrame(2);
			if (frame != null)
			{
				var meth = frame.GetMethod();
				if (meth != null && meth.DeclaringType != null)
					caller = meth.DeclaringType.Name;
			}

			try
			{
				if (_useTextLog)
				{
					_backupLog.Write(message, level);
					return;
				}

				_database.Query("INSERT INTO Logs (TimeStamp, Caller, LogLevel, Message) VALUES (@0, @1, @2, @3)",
					DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), caller, (int)level, message);

				var success = true;
				while (_failures.Count > 0 && success)
				{
					var info = _failures.First();

					try
					{
						_database.Query("INSERT INTO Logs (TimeStamp, Caller, LogLevel, Message) VALUES (@0, @1, @2, @3)",
							info.timestamp, info.caller, (int)info.logLevel, info.message);
					}
					catch (Exception ex)
					{
						success = false;
						_failures.Add(new LogInfo
						{
							caller = "TShock",
							logLevel = TraceLevel.Error,
							message = string.Format("SQL Log insert query failed: {0}", ex),
							timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
						});
					}

					if (success)
						_failures.RemoveAt(0);
				}
			}
			catch (Exception ex)
			{
				_backupLog.ConsoleError("SQL Log insert query failed: {0}", ex);

				_failures.Add(new LogInfo
				{
					logLevel = level,
					message = message,
					caller = caller,
					timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
				});
			}

			if (_failures.Count >= TShock.Config.RevertToTextLogsOnSqlFailures)
			{
				_useTextLog = true;
				_backupLog.ConsoleError("SQL Logging disabled due to errors. Reverting to text logging.");

				foreach (var logInfo in _failures)
				{
					_backupLog.Write(string.Format("SQL log failed at: {0}. {1}", logInfo.timestamp, logInfo),
						TraceLevel.Error);
				}
				_failures.Clear();
			}
		}

		public void Dispose()
		{
			_backupLog.Dispose();
		}
	}
}