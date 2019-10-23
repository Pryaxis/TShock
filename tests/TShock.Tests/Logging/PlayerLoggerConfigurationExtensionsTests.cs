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
using FluentAssertions;
using Microsoft.Xna.Framework;
using Moq;
using Orion.Packets.World;
using Orion.Players;
using Serilog;
using TShock.Logging.Themes;
using Xunit;

namespace TShock.Logging {
    public class PlayerLoggerConfigurationExtensionsTests {
        [Fact]
        public void Player() {
            var theme = new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color> {
                [PlayerLogThemeStyle.Text] = new Color(0x12, 0x34, 0x56)
            });
            var mockPlayer = new Mock<IPlayer>();
            var logger = new LoggerConfiguration()
                .WriteTo.Player(mockPlayer.Object, theme: theme)
                .CreateLogger();

            logger.Error("FAIL");

            mockPlayer.Verify(p => p.SendPacket(It.Is<ChatPacket>(cp => cp.Text.Contains("[c/123456:FAIL]"))));
        }

        [Fact]
        public void Player_NullTheme() {
            var mockPlayer = new Mock<IPlayer>();
            var logger = new LoggerConfiguration()
                .WriteTo.Player(mockPlayer.Object)
                .CreateLogger();

            logger.Error("FAIL");

            mockPlayer.Verify(p => p.SendPacket(It.Is<ChatPacket>(cp => cp.Text.Contains("[c/dcdcdc:FAIL]"))));
        }

        [Fact]
        public void Player_NullConfiguration_ThrowsArgumentNullException() {
            var player = new Mock<IPlayer>().Object;
            Func<LoggerConfiguration> func = () => PlayerLoggerConfigurationExtensions.Player(null, player);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Player_NullPlayer_ThrowsArgumentNullException() {
            var configuration = new LoggerConfiguration();
            Func<LoggerConfiguration> func = () => configuration.WriteTo.Player(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Player_NullOutputTemplate_ThrowsArgumentNullException() {
            var configuration = new LoggerConfiguration();
            var player = new Mock<IPlayer>().Object;
            Func<LoggerConfiguration> func = () => configuration.WriteTo.Player(player, outputTemplate: null);

            func.Should().Throw<ArgumentNullException>();
        }
    }
}
