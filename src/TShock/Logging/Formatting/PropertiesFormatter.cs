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
using System.Linq;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Parsing;
using TShock.Logging.Themes;

namespace TShock.Logging.Formatting {
    internal sealed class PropertiesFormatter : ITextFormatter {
        private readonly MessageTemplate _outputTemplate;
        private readonly PlayerLogValueVisitor _visitor;

        public PropertiesFormatter(PlayerLogTheme theme, MessageTemplate outputTemplate,
                IFormatProvider? formatProvider) {
            Debug.Assert(theme != null, "theme should not be null");
            Debug.Assert(outputTemplate != null, "output template should not be null");

            _outputTemplate = outputTemplate;
            _visitor = new PlayerLogValueVisitor(theme, formatProvider);
        }

        public void Format(LogEvent logEvent, TextWriter output) {
            Debug.Assert(logEvent != null, "log event should not be null");
            Debug.Assert(output != null, "output should not be null");

            bool IsExcluded(string propertyName) {
                return _outputTemplate.Tokens
                    .Concat(logEvent.MessageTemplate.Tokens)
                    .All(t => !(t is PropertyToken p) || p.PropertyName != propertyName);
            }

            var properties = logEvent.Properties
                .Where(kvp => IsExcluded(kvp.Key))
                .Select(kvp => new LogEventProperty(kvp.Key, kvp.Value));
            _visitor.Format(new StructureValue(properties), output);
        }
    }
}
