// Copyright (c) 2019 Pryaxis & TShock Contributors
// 
// This file is part of TShock.
// 
// TShock is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// TShock is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with TShock.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Globalization;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Orion.Players;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace TShock.Commands {
    /// <summary>
    /// Represents a console-based command sender.
    /// </summary>
    public sealed class ConsoleCommandSender : ICommandSender {
#if DEBUG
        private const LogEventLevel LogLevel = LogEventLevel.Verbose;
#else
        private const LogEventLevel LogLevel = LogEventLevel.Error;
#endif
        private const string ResetColorString = "\x1b[0m";
        private const string ColorTag = "c/";

        /// <summary>
        /// Gets the console-based command sender.
        /// </summary>
        public static ConsoleCommandSender Instance { get; } = new ConsoleCommandSender();

        /// <inheritdoc/>
        public string Name => "Console";

        /// <inheritdoc/>
        public ILogger Log { get; }

        /// <inheritdoc/>
        public IPlayer? Player => null;

        [Pure]
        private static string GetColorString(Color color) =>
            FormattableString.Invariant($"\x1b[38;2;{color.R};{color.G};{color.B}m");

        private ConsoleCommandSender() {
            Log = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Properties} {Message:lj}{NewLine}{Exception}",
                    theme: AnsiConsoleTheme.Code)
                .MinimumLevel.Is(LogLevel)
                .CreateLogger();
        }

        /// <inheritdoc/>
        public void SendMessage(string message) => SendMessageImpl(message, string.Empty);

        /// <inheritdoc/>
        public void SendMessage(string message, Color color) => SendMessageImpl(message, GetColorString(color));

        private static void SendMessageImpl(ReadOnlySpan<char> message, string colorString) {
            var output = new StringBuilder(message.Length);
            while (true) {
                output.Append(colorString);
                var leftBracket = message.IndexOf('[');
                var rightBracket = leftBracket + 1 + message[(leftBracket + 1)..].IndexOf(']');
                if (leftBracket < 0 || rightBracket < 0) {
                    break;
                }

                output.Append(message[..leftBracket]);
                var inside = message[(leftBracket + 1)..rightBracket];
                message = message[(rightBracket + 1)..];
                var colon = inside.IndexOf(':');
                var isValidColorTag =
                    inside.StartsWith(ColorTag, StringComparison.OrdinalIgnoreCase) && colon > ColorTag.Length;
                if (!isValidColorTag) {
                    output.Append('[').Append(inside).Append(']');
                    continue;
                }

                if (int.TryParse(inside[ColorTag.Length..colon], NumberStyles.AllowHexSpecifier,
                        CultureInfo.InvariantCulture, out var numberColor)) {
                    var tagColor = new Color((numberColor >> 16) & 255, (numberColor >> 8) & 255, numberColor & 255);
                    output.Append(GetColorString(tagColor));
                }

                output.Append(inside[(colon + 1)..]);
            }

            output.Append(message).Append(ResetColorString);
            Console.WriteLine(output);
        }
    }
}
