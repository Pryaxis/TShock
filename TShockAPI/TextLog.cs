﻿/*
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
using System.Globalization;
using System.IO;
using TerrariaApi.Server;

namespace TShockAPI
{
	/// <summary>
	/// Class inheriting ILog for writing logs to a text file
	/// </summary>
	public class TextLog : ILog, IDisposable
	{
		private readonly bool ClearFile;
		private StreamWriter _logWriter;

		/// <summary>
		/// File name of the Text log
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Creates the log file stream and sets the initial log level.
		/// </summary>
		/// <param name="filename">The output filename. This file will be overwritten if 'clear' is set.</param>
		/// <param name="clear">Whether or not to clear the log file on initialization.</param>
		public TextLog(string filename, bool clear)
		{
			FileName = filename;
			ClearFile = clear;
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
		/// <param name="message">The message to be written.</param>
		public void ConsoleWarn(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ForegroundColor = ConsoleColor.Gray;
			Write(message, TraceLevel.Warning);
		}

		/// <summary>
		/// Writes an error to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void ConsoleWarn(string format, params object[] args)
		{
			ConsoleWarn(string.Format(format, args));
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
		/// Writes a debug string to the log file. Also outputs to the console. Requires config TShock.DebugLogs to be true.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void ConsoleDebug(string message)
		{
			if (TShock.Config.Settings.DebugLogs)
			{
				Console.WriteLine("Debug: " + message);
				Write(message, TraceLevel.Verbose);
			}
		}

		/// <summary>
		/// Writes a debug string to the log file. Also outputs to the console. Requires config TShock.DebugLogs to be true.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void ConsoleDebug(string format, params object[] args)
		{
			ConsoleDebug(string.Format(format, args));
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="message">The message to be written.</param>
		public void Debug(string message)
		{
			if (TShock.Config.Settings.DebugLogs)
				Write(message, TraceLevel.Verbose);
		}

		/// <summary>
		/// Writes a debug string to the log file.
		/// </summary>
		/// <param name="format">The format of the message to be written.</param>
		/// <param name="args">The format arguments.</param>
		public void Debug(string format, params object[] args)
		{
			if (TShock.Config.Settings.DebugLogs)
				Debug(string.Format(format, args));
		}

		/// <summary>
		/// Writes a message to the log
		/// </summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		public void Write(string message, TraceLevel level)
		{
			if (!MayWriteType(level))
				return;
			if (_logWriter is null)
			{
				_logWriter = new StreamWriter(FileName, !ClearFile);
			}

			var caller = "TShock";

			var frame = new StackTrace().GetFrame(2);
			if (frame != null)
			{
				var meth = frame.GetMethod();
				if (meth != null && meth.DeclaringType != null)
					caller = meth.DeclaringType.Name;
			}

			var logEntry = string.Format("{0} - {1}: {2}: {3}",
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
					caller, level.ToString().ToUpper(), message);
			try
			{
				_logWriter.WriteLine(logEntry);
				_logWriter.Flush();
			}
			catch (ObjectDisposedException)
			{
				ServerApi.LogWriter.PluginWriteLine(TShock.instance, logEntry, TraceLevel.Error);
				Console.WriteLine("Unable to write to log as log has been disposed.");
				Console.WriteLine("{0} - {1}: {2}: {3}",
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
					caller, level.ToString().ToUpper(), message);
			}
		}

		public void Dispose()
		{
			if (_logWriter != null)
			{
				_logWriter.Dispose();
			}
		}
	}
}
