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
using Microsoft.Xna.Framework;
using Orion.Players;
using Serilog;
using Serilog.Events;
using TShock.Commands.Logging;

namespace TShock.Commands {
    internal sealed class PlayerCommandSender : ICommandSender {
#if DEBUG
        private const LogEventLevel LogLevel = LogEventLevel.Verbose;
#else
        private const LogEventLevel LogLevel = LogEventLevel.Error;
#endif

        public string Name => Player.Name;
        public ILogger Log { get; }
        public IPlayer Player { get; }

        public PlayerCommandSender(IPlayer player, string input) {
            Debug.Assert(player != null, "player should not be null");

            Player = player;
            Log = new LoggerConfiguration()
                .MinimumLevel.Is(LogLevel)
                .WriteTo.Sink(new PlayerLogSink(player))
                .Enrich.WithProperty("Player", player.Name)
                .Enrich.WithProperty("Cmd", input)
                .WriteTo.Logger(Serilog.Log.Logger)
                .CreateLogger();
        }

        public void SendMessage(ReadOnlySpan<char> message) => SendMessage(message, Color.White);

        public void SendMessage(ReadOnlySpan<char> message, Color color) =>
            Player.SendMessage(message.ToString(), color);
    }
}
