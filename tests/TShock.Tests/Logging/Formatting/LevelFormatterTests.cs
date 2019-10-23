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
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Serilog.Events;
using TShock.Logging.Themes;
using Xunit;

namespace TShock.Logging.Formatting {
    public class LevelFormatterTests {
        private readonly PlayerLogTheme _theme = new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color> {
            [PlayerLogThemeStyle.VerboseLevel] = new Color(0x00, 0x00, 0xff),
            [PlayerLogThemeStyle.DebugLevel] = new Color(0x00, 0xff, 0x00),
            [PlayerLogThemeStyle.InformationLevel] = new Color(0x00, 0xff, 0xff),
            [PlayerLogThemeStyle.WarningLevel] = new Color(0xff, 0x00, 0x00),
            [PlayerLogThemeStyle.ErrorLevel] = new Color(0xff, 0x00, 0xff),
            [PlayerLogThemeStyle.FatalLevel] = new Color(0xff, 0xff, 0x00),
            [PlayerLogThemeStyle.Invalid] = new Color(0x00, 0x00, 0x00)
        });

        [Fact]
        public void Format_Verbose() {
            var formatter = new LevelFormatter(_theme);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Verbose, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>());

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[c/0000ff:VRB]");
        }

        [Fact]
        public void Format_Debug() {
            var formatter = new LevelFormatter(_theme);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Debug, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>());

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[c/00ff00:DBG]");
        }

        [Fact]
        public void Format_Information() {
            var formatter = new LevelFormatter(_theme);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Information, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>());

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[c/00ffff:INF]");
        }

        [Fact]
        public void Format_Warning() {
            var formatter = new LevelFormatter(_theme);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Warning, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>());

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[c/ff0000:WRN]");
        }

        [Fact]
        public void Format_Error() {
            var formatter = new LevelFormatter(_theme);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Error, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>());

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[c/ff00ff:ERR]");
        }

        [Fact]
        public void Format_Fatal() {
            var formatter = new LevelFormatter(_theme);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Fatal, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>());

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[c/ffff00:FTL]");
        }

        [Fact]
        public void Format_Unknown() {
            var formatter = new LevelFormatter(_theme);
            var writer = new StringWriter();
            var logEvent = new LogEvent(
                DateTimeOffset.Now, (LogEventLevel)(-1), null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>());

            formatter.Format(logEvent, writer);

            writer.ToString().Should().Be("[c/000000:???]");
        }
    }
}
