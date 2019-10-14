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

using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Orion.Players;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace TShock.Logging {
    internal sealed class PlayerLogSink : ILogEventSink {
        private readonly IPlayer _player;
        private readonly ITextFormatter _formatter;

        public PlayerLogSink(IPlayer player, ITextFormatter formatter) {
            Debug.Assert(player != null, "player should not be null");
            Debug.Assert(formatter != null, "formatter should not be null");

            _player = player;
            _formatter = formatter;
        }

        public void Emit(LogEvent logEvent) {
            Debug.Assert(logEvent != null, "log event should not be null");

            var output = new StringWriter();
            _formatter.Format(logEvent, output);

            // When sending the text, we don't want any newlines if possible. So we strip the ending newline.
            var text = output.ToString();
            if (text.EndsWith('\n')) {
                text = text[0..^1];
            }

            _player.SendMessage(text, Color.White);
        }
    }
}
