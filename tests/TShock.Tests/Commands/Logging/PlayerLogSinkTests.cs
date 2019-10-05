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
using Microsoft.Xna.Framework;
using Moq;
using Orion.Players;
using Serilog;
using Serilog.Core;
using Xunit;

namespace TShock.Commands.Logging {
    public class PlayerLogSinkTests {
        private readonly Mock<IPlayer> _mockPlayer;
        private readonly ILogger _logger;

        public PlayerLogSinkTests() {
            _mockPlayer = new Mock<IPlayer>();
            ILogEventSink sink = new PlayerLogSink(_mockPlayer.Object);
            _logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.Sink(sink).CreateLogger();
        }

        private void VerifyMessage(string regex) {
            _mockPlayer.Verify(p => p.SendMessage(It.IsRegex(regex), It.IsAny<Color>()));
        }

        [Fact]
        public void Emit_Verbose() {
            _logger.Verbose("test");

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:VRB\]\] test");
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Emit_Debug() {
            _logger.Debug("test");

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:DBG\]\] test");
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Emit_Information() {
            _logger.Information("test");

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:INF\]\] test");
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Emit_Warning() {
            _logger.Warning("test");

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:WRN\]\] test");
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Emit_Error() {
            _logger.Error("test");

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:ERR\]\] test");
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Emit_Fatal() {
            _logger.Fatal("test");

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:FTL\]\] test");
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Emit_WithProperties() {
            _logger.Verbose("{Bool} {Int}", true, 42);

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:VRB\]\] \[c/[a-fA-F0-9]{6}:True\] \[c/[a-fA-F0-9]{6}:42\]");
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Emit_PropertyMissing() {
            _logger.Verbose("{Bool}");

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:VRB\]\] \[c/[a-fA-F0-9]{6}:{Bool}\]");
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Emit_WithException() {
            static void Exception1() => Exception2();
            static void Exception2() => Exception3();
            static void Exception3() => throw new NotImplementedException();

            Exception exception = null;
            try {
                Exception1();
            } catch (NotImplementedException ex) {
                exception = ex;
            }

            _logger.Error(exception, "Exception");

            VerifyMessage(@"\[.+\] \[\[c/[a-fA-F0-9]{6}:ERR\]\] Exception");
            VerifyMessage(@"System\.NotImplementedException: The method or operation is not implemented.");
            VerifyMessage(@"  at TShock\.Commands\.Logging\.PlayerLogSinkTests.+");
            VerifyMessage(@"  at TShock\.Commands\.Logging\.PlayerLogSinkTests.+");
            VerifyMessage(@"  at TShock\.Commands\.Logging\.PlayerLogSinkTests.+");
            VerifyMessage(@"  at TShock\.Commands\.Logging\.PlayerLogSinkTests.+");
            _mockPlayer.VerifyNoOtherCalls();
        }
    }
}
