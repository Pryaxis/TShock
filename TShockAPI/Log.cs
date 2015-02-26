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

namespace TShockAPI
{
	public static class Log
	{
		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Data")]
		public static void Data(String message)
		{
			Write(message, LogLevel.Data);
		}

		/// <summary>
		/// Writes data to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Data")]
		public static void Data(string format, params object[] args)
		{
			Data(String.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Error")]
		public static void Error(String message)
		{
			Write(message, LogLevel.Error);
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Error")]
		public static void Error(string format, params object[] args)
		{
			Error(String.Format(format, args));
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.ConsoleError")]
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
		[Obsolete("Please use TShock.Log.ConsoleError")]
		public static void ConsoleError(string format, params object[] args)
		{
			ConsoleError(String.Format(format, args));
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Warn")]
		public static void Warn(String message)
		{
			Write(message, LogLevel.Warning);
		}

		/// <summary>
		/// Writes a warning to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Warn")]
		public static void Warn(string format, params object[] args)
		{
			Warn(String.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Info")]
		public static void Info(String message)
		{
			Write(message, LogLevel.Info);
		}

		/// <summary>
		/// Writes an informative string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Info")]
		public static void Info(string format, params object[] args)
		{
			Info(String.Format(format, args));
		}

		/// <summary>
		/// Writes an informative string to the log file. Also outputs to the console.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.ConsoleInfo")]
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
		[Obsolete("Please use TShock.Log.ConsoleInfo")]
		public static void ConsoleInfo(string format, params object[] args)
		{
			ConsoleInfo(String.Format(format, args));
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		[Obsolete("Please use TShock.Log.Debug")]
		public static void Debug(String message)
		{
			Write(message, LogLevel.Debug);
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		[Obsolete("Please use TShock.Log.Debug")]
		public static void Debug(string format, params object[] args)
		{
			Debug(String.Format(format, args));
		}

		/// <summary>
		/// Internal method which writes a message directly to the log file.
		/// </summary>
		private static void Write(String message, LogLevel level)
		{
			TShock.Log.Write(message, level);
		}
	}
}