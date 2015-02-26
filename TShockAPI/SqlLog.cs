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
using System.Globalization;
using System.Linq;
using TShockAPI.DB;

namespace TShockAPI
{
	struct LogInfo
	{
		public string timestamp;
		public string message;
		public string caller;
		public LogLevel logLevel;

		public override string ToString()
		{
			return String.Format("Message: {0}: {1}: {2}",
				caller, logLevel.ToString().ToUpper(), message);
		}
	}

	/// <summary>
	/// Class inheriting ILog for writing logs to TShock's SQL database
	/// </summary>
	public class SqlLog : ILog, IDisposable
	{
		private readonly LogLevel _logLevel;
		private readonly IDbConnection _database;
		private readonly TextLog _backupLog;
		private readonly List<LogInfo> _failures = new List<LogInfo>(TShock.Config.RevertToTextLogsOnSqlFailures);
		private bool _useTextLog;

		public string Name
		{
			get { return "SQL Log Writer"; }
		}

		public SqlLog(LogLevel logLevel, IDbConnection db, string textlogFilepath, bool clearTextLog)
		{
			_logLevel = logLevel;
			_database = db;
			_backupLog = new TextLog(textlogFilepath, logLevel, clearTextLog);
		}

		public bool MayWriteType(LogLevel type)
		{
			return ((_logLevel & type) == type);
		}

		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Data(String message)
		{
			Write(message, LogLevel.Data);
		}

		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Data(string format, params object[] args)
		{
			Data(String.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Error(String message)
		{
			Write(message, LogLevel.Error);
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Error(string format, params object[] args)
		{
			Error(String.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void ConsoleError(String message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Write(message, LogLevel.Error);
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void ConsoleError(string format, params object[] args)
		{
			ConsoleError(String.Format(format, args));
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Warn(String message)
		{
			Write(message, LogLevel.Warning);
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Warn(string format, params object[] args)
		{
			Warn(String.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Info(String message)
		{
			Write(message, LogLevel.Info);
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Info(string format, params object[] args)
		{
			Info(String.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file. Also outputs to the console.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void ConsoleInfo(String message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Write(message, LogLevel.Info);
		}

		/// <summary>
		/// Writes an informative string to the log file. Also outputs to the console.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void ConsoleInfo(string format, params object[] args)
		{
			ConsoleInfo(String.Format(format, args));
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Debug(String message)
		{
			Write(message, LogLevel.Debug);
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Debug(string format, params object[] args)
		{
			Debug(String.Format(format, args));
		}

		public void Write(string message, LogLevel level)
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

				_database.Query("INSERT INTO Logs (LogLevel, TimeStamp, Caller, Message) VALUES (@0, @1, @2, @3)",
					level, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
					caller, message);

				if (_failures.Count > 0)
				{
					var info = _failures.Last();
					var success = true;

					try
					{
						_database.Query("INSERT INTO Logs (LogLevel, TimeStamp, Caller, Message) VALUES (@0, @1, @2, @3)",
							info.logLevel, info.timestamp, info.caller, info.message);
					}
					catch (Exception ex)
					{
						success = false;
						_failures.Add(new LogInfo
						{
							caller = "TShock",
							logLevel = LogLevel.Error,
							message = String.Format("SQL Log insert query failed: {0}", ex),
							timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
						});
					}

					if (success)
						_failures.RemoveAt(_failures.Count - 1);
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
					timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
				});
			}

			if (_failures.Count >= TShock.Config.RevertToTextLogsOnSqlFailures)
			{
				_useTextLog = true;
				_backupLog.ConsoleError("SQL Logging disabled due to errors. Reverting to text logging.");

				for (var i = _failures.Count - 1; i >= 0; --i)
				{
					_backupLog.Write(String.Format("SQL log failed at: {0}. {1}", _failures[i].timestamp, _failures[i]),
						LogLevel.Error);
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