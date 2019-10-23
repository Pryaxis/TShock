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
using System.Linq;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Serilog.Events;
using TShock.Logging.Themes;
using Xunit;

namespace TShock.Logging.Formatting {
    public class TimestampFormatterTests {
        private readonly PlayerLogTheme _theme = new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color> {
            [PlayerLogThemeStyle.Timestamp] = new Color(0xfa, 0xdf, 0xad)
        });

        [Fact]
        public void Format() {
            var formatter = new TimestampFormatter(_theme, "yyyyMMdd", CultureInfo.InvariantCulture);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                new DateTimeOffset(new DateTime(2019, 10, 10)), LogEventLevel.Debug, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>());

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[c/fadfad:20191010]");
        }
    }
}
