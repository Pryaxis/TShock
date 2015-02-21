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
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace TShockAPI
{
	[Flags]
	public enum LogLevel
	{
		None = 0,
		Debug = 1,
		Info = 2,
		Warning = 4,
		Error = 8,
		Data = 16,
		All = 31
	}

	public static class Log
	{
		private static string _filename;
		private static LogLevel _logLevel;
		private static StreamWriter _logWriter;

		/// <summary>
		/// Creates the log file stream and sets the initial log level.
		/// </summary>
		/// <param name="filename">The output filename. This file will be overwritten if 'clear' is set.</param>
		/// <param name="logLevel">The <see cref="LogLevel" /> value which sets the type of messages to output.</param>
		/// <param name="clear">Whether or not to clear the log file on initialization.</param>
		public static void Initialize(string filename, LogLevel logLevel, bool clear)
		{
			_filename = filename;
			_logLevel = logLevel;

			_logWriter = new StreamWriter(filename, !clear);
		}

		/// <summary>
		/// Checks whether the log level contains the specified flag.
		/// </summary>
		/// <param name="type">The <see cref="LogLevel" /> value to check.</param>
		private static bool MayWriteType(LogLevel type)
		{
			return ((_logLevel & type) == type);
		}

		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void Data(String message)
		{
			Write(message, LogLevel.Data);
		}

		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public static void Data(string format, params object[] args)
		{
			Data(String.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void Error(String message)
		{
			Write(message, LogLevel.Error);
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public static void Error(string format, params object[] args)
		{
			Error(String.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void ConsoleError(String message)
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
		public static void ConsoleError(string format, params object[] args)
		{
			ConsoleError(String.Format(format, args));
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void Warn(String message)
		{
			Write(message, LogLevel.Warning);
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public static void Warn(string format, params object[] args)
		{
			Warn(String.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void Info(String message)
		{
			Write(message, LogLevel.Info);
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public static void Info(string format, params object[] args)
		{
			Info(String.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file. Also outputs to the console.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void ConsoleInfo(String message)
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
		public static void ConsoleInfo(string format, params object[] args)
		{
			ConsoleInfo(String.Format(format, args));
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public static void Debug(String message)
		{
			Write(message, LogLevel.Debug);
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public static void Debug(string format, params object[] args)
		{
			Debug(String.Format(format, args));
		}

		/// <summary>
		/// Disposes objects that are being used.
		/// </summary>
		public static void Dispose()
		{
			_logWriter.Dispose();
		}

		/// <summary>
		/// Internal method which writes a message directly to the log file.
		/// </summary>
		private static void Write(String message, LogLevel level)
		{
			if (!MayWriteType(level))
			{
				return;
			}

			string caller = "TShock";

			StackFrame frame = new StackTrace().GetFrame(2);
			if (frame != null)
			{
				var meth = frame.GetMethod();
				if (meth != null)
					caller = meth.DeclaringType.Name;
			}

		    try
		    {
                _logWriter.WriteLine(string.Format("{0} - {1}: {2}: {3}",
                                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                   caller, level.ToString().ToUpper(), message));
                _logWriter.Flush();
		    }
		    catch (ObjectDisposedException)
		    {
                Console.WriteLine("Unable to write to log as log has been disposed.");
                Console.WriteLine("{0} - {1}: {2}: {3}",
                                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                                   caller, level.ToString().ToUpper(), message);
		    }
		}
	}
}