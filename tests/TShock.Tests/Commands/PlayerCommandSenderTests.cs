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

using FluentAssertions;
using Microsoft.Xna.Framework;
using Moq;
using Orion.Players;
using Xunit;

namespace TShock.Commands {
    public class PlayerCommandSenderTests {
        private readonly Mock<IPlayer> _mockPlayer = new Mock<IPlayer>();
        private readonly ICommandSender _sender;

        public PlayerCommandSenderTests() {
            _sender = new PlayerCommandSender(_mockPlayer.Object, "test");
            _mockPlayer.VerifyGet(p => p.Name);
        }

        [Fact]
        public void Name_Get() {
            _mockPlayer.SetupGet(p => p.Name).Returns("test");

            _sender.Name.Should().Be("test");

            _mockPlayer.VerifyGet(p => p.Name);
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Player_Get() {
            _sender.Player.Should().NotBeNull();
            _sender.Player.Should().Be(_mockPlayer.Object);
        }

        [Fact]
        public void SendMessage() {
            _sender.SendMessage("test");

            _mockPlayer.Verify(p => p.SendMessage("test", Color.White));
            _mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void SendMessage_WithColor() {
            _sender.SendMessage("test", Color.Orange);

            _mockPlayer.Verify(p => p.SendMessage("test", Color.Orange));
            _mockPlayer.VerifyNoOtherCalls();
        }
    }
}
