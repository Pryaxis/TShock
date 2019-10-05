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
using System.Text;
using Microsoft.Xna.Framework;
using Orion.Players;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace TShock.Commands.Logging {
    internal sealed class PlayerLogSink : ILogEventSink {
        private const string LevelVerbose = "[c/c0c0c0:VRB]";
        private const string LevelDebug = "[c/c0c0c0:DBG]";
        private const string LevelInformation = "[c/ffffff:INF]";
        private const string LevelWarning = "[c/ffffaf:WRN]";
        private const string LevelError = "[c/ff005f:ERR]";
        private const string LevelFatal = "[c/ff005f:FTL]";
        private const string LevelUnknown = "[c/ff0000:UNK]";

        private static readonly Color _textColor = new Color(0xda, 0xda, 0xda);
        private static readonly Color _exceptionColor = new Color(0xda, 0xda, 0xda);
        private static readonly PlayerLogValueFormatter _formatter = new PlayerLogValueFormatter();

        private readonly IPlayer _player;

        public PlayerLogSink(IPlayer player) {
            Debug.Assert(player != null, "player should not be null");

            _player = player;
        }

        public void Emit(LogEvent logEvent) {
            var logLevel = logEvent.Level switch
            {
                LogEventLevel.Verbose => LevelVerbose,
                LogEventLevel.Debug => LevelDebug,
                LogEventLevel.Information => LevelInformation,
                LogEventLevel.Warning => LevelWarning,
                LogEventLevel.Error => LevelError,
                LogEventLevel.Fatal => LevelFatal,
                _ => LevelUnknown
            };

            var output = new StringBuilder($"[c/6c6c6c:{logEvent.Timestamp:HH:mm:ss zz}] [{logLevel}] ");
            foreach (var token in logEvent.MessageTemplate.Tokens) {
                if (token is TextToken textToken) {
                    output.Append(textToken.Text);
                    continue;
                }

                var propertyToken = (PropertyToken)token;
                if (!logEvent.Properties.TryGetValue(propertyToken.PropertyName, out var propertyValue)) {
                    output.Append($"[c/ff0000:{propertyToken}]");
                    continue;
                }

                output.Append(_formatter.Format(propertyValue));
            }

            _player.SendMessage(output.ToString(), _textColor);

            if (logEvent.Exception != null) {
                using var reader = new StringReader(logEvent.Exception.ToString());
                string line;
                while ((line = reader.ReadLine()) != null) {
                    _player.SendMessage(line, _exceptionColor);
                }
            }
        }
    }
}
