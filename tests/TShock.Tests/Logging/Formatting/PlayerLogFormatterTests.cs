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
    public class PlayerLogFormatterTests {
        [Fact]
        public void Format() {
            var theme = new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color>());
            var formatter = new PlayerLogFormatter(theme,
                "[{Timestamp:HH:mm:ss} {Level}] {Property1}{Message}{NewLine}{Exception}",
                CultureInfo.InvariantCulture);
            var logEvent = new LogEvent(
                new DateTimeOffset(new DateTime(2019, 10, 10, 9, 59, 59)), LogEventLevel.Debug,
                new Exception("TEST"),
                new MessageTemplate(new[] { new TextToken("my message") }),
                new[] {
                    new LogEventProperty("Property1", new ScalarValue(1))
                });
            var writer = new StringWriter();

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[09:59:59 DBG] 1my message\nSystem.Exception: TEST");
        }
    }
}
