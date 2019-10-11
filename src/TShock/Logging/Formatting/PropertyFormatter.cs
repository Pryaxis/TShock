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
using Serilog.Parsing;
using TShock.Logging.Themes;

namespace TShock.Logging.Formatting {
    internal sealed class PropertyFormatter : ITextFormatter {
        private readonly PlayerLogTheme _theme;
        private readonly PropertyToken _propertyToken;
        private readonly PlayerLogValueVisitor _visitor;

        public PropertyFormatter(PlayerLogTheme theme, PropertyToken propertyToken, IFormatProvider? formatProvider) {
            Debug.Assert(theme != null, "theme should not be null");
            Debug.Assert(propertyToken != null, "property token should not be null");

            _theme = theme;
            _propertyToken = propertyToken;
            _visitor = new PlayerLogValueVisitor(theme, formatProvider);
        }

        public void Format(LogEvent logEvent, TextWriter output) {
            Debug.Assert(logEvent != null, "log event should not be null");
            Debug.Assert(output != null, "output should not be null");

            if (!logEvent.Properties.TryGetValue(_propertyToken.PropertyName, out var propertyValue)) {
                output.Write(_theme.Stylize(_propertyToken.ToString(), PlayerLogThemeStyle.Invalid));
            } else {
                _visitor.Format(propertyValue, output);
            }
        }
    }
}
