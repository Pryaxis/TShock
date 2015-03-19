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
	/// <summary>
	/// Flags to define which types of message are logged
	/// </summary>
	[Flags]
	public enum LogLevel
	{
		/// <summary>
		/// No messages will be logged
		/// </summary>
		None = 0,

		/// <summary>
		/// Debug messages will be logged
		/// </summary>
		Debug = 1,

		/// <summary>
		/// Informative messages will be logged
		/// </summary>
		Info = 2,

		/// <summary>
		/// Warning message will be logged
		/// </summary>
		Warning = 4,

		/// <summary>
		/// Error messages will be logged
		/// </summary>
		Error = 8,

		/// <summary>
		/// Data messages will be logged
		/// </summary>
		Data = 16,

		/// <summary>
		/// All messages will be logged
		/// </summary>
		All = 31
	}

	/// <summary>
	/// Logging interface
	/// </summary>
	public interface ILog
	{
		/// <summary>
		/// Log file name
		/// </summary>
		string FileName { get; set; }

		/// <summary>
		/// Checks whether the log level contains the specified flag.
		/// </summary>
		/// <param name="type">The <see cref="LogLevel" /> value to check.</param>
		bool MayWriteType(LogLevel type);

		/// <summary>
		/// Writes an informative string to the log and to the console.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		void ConsoleInfo(string message);

		/// <summary>
		/// Writes an informative string to the log and to the console.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		void ConsoleInfo(string format, params object[] args);

		/// <summary>
		/// Writes an error message to the log and to the console.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		void ConsoleError(string message);

		/// <summary>
		/// Writes an error message to the log and to the console.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		void ConsoleError(string format, params object[] args);

		/// <summary>
		/// Writes a warning to the log.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		void Warn(string message);

		/// <summary>
		/// Writes a warning to the log.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		void Warn(string format, params object[] args);

		/// <summary>
		/// Writes an error to the log.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		void Error(string message);

		/// <summary>
		/// Writes an error to the log.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		void Error(string format, params object[] args);

		/// <summary>
		/// Writes an informative string to the log.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		void Info(string message);

		/// <summary>
		/// Writes an informative string to the log.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		void Info(string format, params object[] args);

		/// <summary>
		/// Writes data to the log.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		void Data(string message);

		/// <summary>
		/// Writes data to the log.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		void Data(string format, params object[] args);

		/// <summary>
		/// Writes a message to the log
		/// </summary>
		/// <param name="message">Message to write</param>
		/// <param name="level">LogLevel assosciated with the message</param>
		void Write(string message, LogLevel level);

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		void Debug(String message);

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		void Debug(string format, params object[] args);

		/// <summary>
		/// Dispose the Log
		/// </summary>
		void Dispose();
	}
}