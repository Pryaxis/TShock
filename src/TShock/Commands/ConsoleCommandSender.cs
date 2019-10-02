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

namespace TShock.Commands {
    internal sealed class ConsoleCommandSender : ICommandSender {
        private const string ResetColorString = "\x1b[0m";
        private const string ColorTagPrefix = "c/";

        public string Name => "Console";
        public ILogger Log => Serilog.Log.Logger;
        public IPlayer? Player => null;

        [Pure]
        private static string GetColorString(Color color) => $"\x1b[38;2;{color.R};{color.G};{color.B}m";

        public void SendMessage(ReadOnlySpan<char> message) => SendMessage(message, string.Empty);
        public void SendMessage(ReadOnlySpan<char> message, Color color) => SendMessage(message, GetColorString(color));

        private static void SendMessage(ReadOnlySpan<char> message, string colorString) {
            var output = new StringBuilder(colorString);
            
            while (true) {
                var leftBracket = message.IndexOf('[');
                var rightBracket = leftBracket + 1 + message[(leftBracket + 1)..].IndexOf(']');
                if (leftBracket < 0 || rightBracket < 0) break;

                output.Append(message[..leftBracket]);
                var inside = message[(leftBracket + 1)..rightBracket];
                message = message[(rightBracket + 1)..];
                var colon = inside.IndexOf(':');
                var isValidColorTag = inside.StartsWith(ColorTagPrefix, StringComparison.OrdinalIgnoreCase) &&
                                      colon > ColorTagPrefix.Length;
                if (!isValidColorTag) {
                    output.Append('[').Append(inside).Append(']');
                    continue;
                }

                if (int.TryParse(inside[ColorTagPrefix.Length..colon], NumberStyles.AllowHexSpecifier,
                                 CultureInfo.InvariantCulture, out var numberColor)) {
                    var tagColor = new Color((numberColor >> 16) & 255, (numberColor >> 8) & 255, numberColor & 255);
                    output.Append(GetColorString(tagColor));
                }

                output.Append(inside[(colon + 1)..]);
                output.Append(colorString);
            }

            output.Append(message).Append(ResetColorString);
            Console.WriteLine(output);
        }
    }
}
