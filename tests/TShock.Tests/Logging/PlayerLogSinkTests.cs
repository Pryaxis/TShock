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
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Moq;
using Orion.Packets.World;
using Orion.Players;
using Serilog.Events;
using Serilog.Formatting;
using Xunit;

namespace TShock.Logging {
    public class PlayerLogSinkTests {
        [Fact]
        public void Emit() {
            var mockPlayer = new Mock<IPlayer>();
            var mockFormatter = new Mock<ITextFormatter>();
            mockFormatter
                .Setup(f => f.Format(It.IsAny<LogEvent>(), It.IsAny<TextWriter>()))
                .Callback((LogEvent logEvent, TextWriter output) => output.Write("TEST"));
            var sink = new PlayerLogSink(mockPlayer.Object, mockFormatter.Object);

            sink.Emit(new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Debug, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>()));

            mockPlayer.Verify(p => p.SendPacket(
                It.Is<ChatPacket>(cp => cp.Color == Color.White && cp.Text == "TEST")));
        }

        [Fact]
        public void Emit_EndsInNewLine() {
            var mockPlayer = new Mock<IPlayer>();
            var mockFormatter = new Mock<ITextFormatter>();
            mockFormatter
                .Setup(f => f.Format(It.IsAny<LogEvent>(), It.IsAny<TextWriter>()))
                .Callback((LogEvent logEvent, TextWriter output) => output.Write("TEST\n"));
            var sink = new PlayerLogSink(mockPlayer.Object, mockFormatter.Object);

            sink.Emit(new LogEvent(
                DateTimeOffset.Now, LogEventLevel.Debug, null,
                MessageTemplate.Empty, Enumerable.Empty<LogEventProperty>()));

            mockPlayer.Verify(p => p.SendPacket(
                It.Is<ChatPacket>(cp => cp.Color == Color.White && cp.Text == "TEST")));
        }
    }
}
