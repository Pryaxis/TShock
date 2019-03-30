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
using System.Diagnostics;

namespace TShockAPI
{
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
		/// <param name="type">The <see cref="TraceLevel" /> value to check.</param>
		bool MayWriteType(TraceLevel type);

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
		void Write(string message, TraceLevel level);

		/// <summary>
		/// Writes a debug string to the log file. Only works if the DEBUG preprocessor conditional is set.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		void Debug(string message);

		/// <summary>
		/// Writes a debug string to the log file. Only works if the DEBUG preprocessor conditional is set.
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