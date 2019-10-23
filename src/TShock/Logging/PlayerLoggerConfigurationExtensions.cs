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
using Orion.Players;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using TShock.Logging.Formatting;
using TShock.Logging.Themes;

namespace TShock.Logging {
    /// <summary>
    /// Extends <see cref="LoggerConfiguration"/> with a WriteTo.Player() method.
    /// </summary>
    public static class PlayerLoggerConfigurationExtensions {
        private const string DefaultOutputTemplate = "[{Timestamp:HH:mm:ss} {Level}] {Message}{NewLine}{Exception}";

        /// <summary>
        /// Writes log events to the given <paramref name="player"/> with optional settings.
        /// </summary>
        /// <param name="configuration">The logger sink configuration.</param>
        /// <param name="player">The player.</param>
        /// <param name="restrictedToMinimumLevel">The minimum level for events to be logged.</param>
        /// <param name="outputTemplate">The output template.</param>
        /// <param name="formatProvider">The format provider, or <see langword="null"/> for a default one.</param>
        /// <param name="levelSwitch">The level switch, or <see langword="null"/> for none.</param>
        /// <param name="theme">The theme, or <see langword="null"/> for a default one.</param>
        /// <returns>A logger configuration.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="configuration"/>, <paramref name="player"/>, or <paramref name="outputTemplate"/> are
        /// <see langword="null"/>.
        /// </exception>
        public static LoggerConfiguration Player(
                this LoggerSinkConfiguration configuration, IPlayer player,
                LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
                string outputTemplate = DefaultOutputTemplate,
                IFormatProvider? formatProvider = null,
                LoggingLevelSwitch? levelSwitch = null,
                PlayerLogTheme? theme = null) {
            if (configuration is null) {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (player is null) {
                throw new ArgumentNullException(nameof(player));
            }

            if (outputTemplate is null) {
                throw new ArgumentNullException(nameof(outputTemplate));
            }

            var formatter = new PlayerLogFormatter(
                theme ?? PlayerLogThemes.VisualStudio, outputTemplate, formatProvider);
            var sink = new PlayerLogSink(player, formatter);
            return configuration.Sink(sink, restrictedToMinimumLevel, levelSwitch);
        }
    }
}
