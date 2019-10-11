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
using System.Diagnostics;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using TShock.Logging.Themes;

namespace TShock.Logging.Formatting {
    internal sealed class TextFormatter : ITextFormatter {
        private readonly PlayerLogTheme _theme;
        private readonly string _text;

        public TextFormatter(PlayerLogTheme theme, string text) {
            Debug.Assert(theme != null, "theme should not be null");
            Debug.Assert(text != null, "text should not be null");

            _theme = theme;
            _text = text;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="logEvent"/> or <paramref name="output"/> are <see langword="null"/>.
        /// </exception>
        public void Format(LogEvent logEvent, TextWriter output) {
            Debug.Assert(logEvent != null, "log event should not be null");
            Debug.Assert(output != null, "output should not be null");

            output.Write(_theme.Stylize(_text, PlayerLogThemeStyle.Text));
        }
    }
}
