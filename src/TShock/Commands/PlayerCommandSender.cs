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
using Destructurama;
using Microsoft.Xna.Framework;
using Orion.Players;
using Serilog;
using Serilog.Events;
using TShock.Logging;

namespace TShock.Commands {
    /// <summary>
    /// Represents a player-based command sender.
    /// </summary>
    public sealed class PlayerCommandSender : ICommandSender {
#if DEBUG
        private const LogEventLevel LogLevel = LogEventLevel.Verbose;
#else
        private const LogEventLevel LogLevel = LogEventLevel.Error;
#endif

        /// <inheritdoc/>
        public string Name => Player.Name;

        /// <inheritdoc/>
        public ILogger Log { get; }

        /// <inheritdoc/>
        public IPlayer Player { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerCommandSender"/> class with the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <exception cref="ArgumentNullException"><paramref name="player"/> is <see langword="null"/>.</exception>
        public PlayerCommandSender(IPlayer player) {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Log = new LoggerConfiguration()
                .Destructure.UsingAttributes()
                .MinimumLevel.Is(LogLevel)
                .WriteTo.Player(player)
                .CreateLogger();
        }

        /// <inheritdoc/>
        public void SendMessage(string message) => SendMessage(message, Color.White);

        /// <inheritdoc/>
        public void SendMessage(string message, Color color) => Player.SendMessage(message, color);
    }
}
