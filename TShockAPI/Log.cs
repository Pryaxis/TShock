/*   
TShock, a server mod for Terraria
Copyright (C) <year>  <name of author>

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
        /// Internal method which writes a message directly to the log file.
        /// </summary>
        private static void Write(String message, LogLevel level)
        {
            StackTrace trace = new StackTrace();
            StackFrame frame = null;

            frame = trace.GetFrame(2);

            string caller = "TShock: ";

            /*if (frame != null && frame.GetMethod().DeclaringType != null)
            {
                caller = frame.GetMethod().DeclaringType.Name + ": ";
            }*/

            switch (level)
            {
                case LogLevel.Debug:
                    message = "DEBUG: " + message;
                    break;
                case LogLevel.Info:
                    message = "INFO: " + message;
                    break;
                case LogLevel.Warning:
                    message = "WARNING: " + message;
                    break;
                case LogLevel.Error:
                    message = "ERROR: " + message;
                    break;
            }

            /*try
            {
                _logWriter = new StreamWriter(_filename, true);
            }
            catch (IOException)
            {
                _logWriter = new StreamWriter(_filename + "." + Process.GetCurrentProcess().Id.ToString(), true);
            }*/

            String text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " - " + caller +
                          message;

            Console.WriteLine(text);

            if (!MayWriteType(level))
            {
                return;
            }

            _logWriter.WriteLine(text);
            _logWriter.Flush();
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
        /// Writes an error to the log file.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void Error(String message)
        {
            Write(message, LogLevel.Error);
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
        /// Writes an informative string to the log file.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void Info(String message)
        {
            Write(message, LogLevel.Info);
        }

        /// <summary>
        /// Writes a debug string to the log file.
        /// </summary>
        /// <param name="message">The message to be written.</param>
        public static void Debug(String message)
        {
            if (!MayWriteType(LogLevel.Debug))
            {
                return;
            }

            Write(message, LogLevel.Debug);
        }
    }
}