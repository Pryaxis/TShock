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

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Serilog.Events;
using Serilog.Formatting;
using TShock.Logging.Themes;
using TShock.Utils.Extensions;

namespace TShock.Logging.Formatting {
    internal sealed class LevelFormatter : ITextFormatter {
        private static readonly IDictionary<LogEventLevel, string> _levelToText =
            new Dictionary<LogEventLevel, string> {
                [LogEventLevel.Verbose] = "VRB",
                [LogEventLevel.Debug] = "DBG",
                [LogEventLevel.Information] = "INF",
                [LogEventLevel.Warning] = "WRN",
                [LogEventLevel.Error] = "ERR",
                [LogEventLevel.Fatal] = "FTL"
            };

        private static readonly IDictionary<LogEventLevel, PlayerLogThemeStyle> _levelToStyle =
            new Dictionary<LogEventLevel, PlayerLogThemeStyle> {
                [LogEventLevel.Verbose] = PlayerLogThemeStyle.VerboseLevel,
                [LogEventLevel.Debug] = PlayerLogThemeStyle.DebugLevel,
                [LogEventLevel.Information] = PlayerLogThemeStyle.InformationLevel,
                [LogEventLevel.Warning] = PlayerLogThemeStyle.WarningLevel,
                [LogEventLevel.Error] = PlayerLogThemeStyle.ErrorLevel,
                [LogEventLevel.Fatal] = PlayerLogThemeStyle.FatalLevel
            };

        private readonly PlayerLogTheme _theme;

        public LevelFormatter(PlayerLogTheme theme) {
            Debug.Assert(theme != null, "theme should not be null");

            _theme = theme;
        }

        public void Format(LogEvent logEvent, TextWriter output) {
            Debug.Assert(logEvent != null, "log event should not be null");
            Debug.Assert(output != null, "output should not be null");

            var level = logEvent.Level;
            var text = _levelToText.GetValueOrDefault(level, () => "???");
            var style = _levelToStyle.GetValueOrDefault(level, () => PlayerLogThemeStyle.Invalid);
            output.Write(_theme.Stylize(text, style));
        }
    }
}
