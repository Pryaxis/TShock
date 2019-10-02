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
using FluentAssertions;
using Microsoft.Xna.Framework;
using Moq;
using Orion.Packets.World;
using Orion.Players;
using Xunit;

namespace TShock.Commands {
    public class PlayerCommandSenderTests {
        [Fact]
        public void Name_Get_IsCorrect() {
            var mockPlayer = new Mock<IPlayer>();
            mockPlayer.SetupGet(p => p.Name).Returns("test");
            ICommandSender sender = new PlayerCommandSender(mockPlayer.Object);

            sender.Name.Should().Be("test");

            mockPlayer.VerifyGet(p => p.Name);
            mockPlayer.VerifyNoOtherCalls();
        }

        [Fact]
        public void Player_Get_IsCorrect() {
            var player = new Mock<IPlayer>().Object;
            ICommandSender sender = new PlayerCommandSender(player);

            sender.Player.Should().NotBeNull();
            sender.Player.Should().Be(player);
        }

        [Fact]
        public void SendMessage_IsCorrect() {
            var mockPlayer = new Mock<IPlayer>();
            ICommandSender sender = new PlayerCommandSender(mockPlayer.Object);

            sender.SendMessage("test", Color.White);

            mockPlayer.Verify(
                p => p.SendPacket(It.Is<ChatPacket>(cp => cp.ChatText == "test" && cp.ChatColor == Color.White)));
            mockPlayer.VerifyNoOtherCalls();
        }
    }
}
