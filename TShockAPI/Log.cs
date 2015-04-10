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

namespace TShockAPI
{
	public static class Log
	{
		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Data")]
		public static void Data(string message)
		{
			Write(message, TraceLevel.Verbose);
		}

		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Data")]
		public static void Data(string format, params object[] args)
		{
			Data(string.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Error")]
		public static void Error(string message)
		{
			Write(message, TraceLevel.Error);
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Error")]
		public static void Error(string format, params object[] args)
		{
			Error(string.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.ConsoleError")]
		public static void ConsoleError(string message)
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
		[Obsolete("Please use TShock.Log.ConsoleError")]
		public static void ConsoleError(string format, params object[] args)
		{
			ConsoleError(string.Format(format, args));
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Warn")]
		public static void Warn(string message)
		{
			Write(message, TraceLevel.Warning);
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Warn")]
		public static void Warn(string format, params object[] args)
		{
			Warn(string.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Info")]
		public static void Info(string message)
		{
			Write(message, TraceLevel.Info);
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Info")]
		public static void Info(string format, params object[] args)
		{
			Info(string.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file. Also outputs to the console.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.ConsoleInfo")]
		public static void ConsoleInfo(string message)
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
		[Obsolete("Please use TShock.Log.ConsoleInfo")]
		public static void ConsoleInfo(string format, params object[] args)
		{
			ConsoleInfo(string.Format(format, args));
		}
		
#if DEBUG
		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Debug")]
		public static void Debug(string message)
		{
			Write(message, TraceLevel.Verbose);
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Debug")]
		public static void Debug(string format, params object[] args)
		{
			Debug(string.Format(format, args));
		}
#endif

		/// <summary>
		/// Internal method which writes a message directly to the log file.
		/// </summary>
		private static void Write(string message, TraceLevel level)
		{
			TShock.Log.Write(message, level);
		}
	}
}