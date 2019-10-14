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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Serilog.Events;
using Serilog.Parsing;
using TShock.Logging.Themes;
using Xunit;

namespace TShock.Logging.Formatting {
    public class PropertiesFormatterTests {
        private readonly PlayerLogTheme _theme = new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color>());

        [Fact]
        public void Format() {
            var formatter = new PropertiesFormatter(_theme, MessageTemplate.Empty, CultureInfo.InvariantCulture);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Debug, null,
                MessageTemplate.Empty,
                new[] {
                    new LogEventProperty("Test", new ScalarValue(1)),
                    new LogEventProperty("Test2", new ScalarValue(2)),
                });

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("{Test=1, Test2=2}");
        }

        [Fact]
        public void Format_ExistsInOutputTemplate() {
            var formatter = new PropertiesFormatter(_theme,
                new MessageTemplate(new[] { new PropertyToken("Test2", "{Test2}") }), CultureInfo.InvariantCulture);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Debug, null,
                MessageTemplate.Empty,
                new[] {
                    new LogEventProperty("Test", new ScalarValue(1)),
                    new LogEventProperty("Test2", new ScalarValue(2)),
                });

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("{Test=1}");
        }

        [Fact]
        public void Format_ExistsInTemplate() {
            var formatter = new PropertiesFormatter(_theme, MessageTemplate.Empty, CultureInfo.InvariantCulture);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Debug, null,
                new MessageTemplate(new[] { new PropertyToken("Test", "{Test}") }),
                new[] {
                    new LogEventProperty("Test", new ScalarValue(1)),
                    new LogEventProperty("Test2", new ScalarValue(2)),
                });

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("{Test2=2}");
        }
    }
}
