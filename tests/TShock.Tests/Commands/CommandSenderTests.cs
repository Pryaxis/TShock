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
using Moq;
using Orion.Players;
using Xunit;

namespace TShock.Commands {
    public class CommandSenderTests {
        [Fact]
        public void Console_Get_IsCorrect() {
            ICommandSender.Console.Should().BeOfType<ConsoleCommandSender>();
        }

        [Fact]
        public void Console_GetMultipleTimes_ReturnsSameInstance() {
            var sender1 = ICommandSender.Console;
            var sender2 = ICommandSender.Console;

            sender1.Should().BeSameAs(sender2);
        }

        [Fact]
        public void FromPlayer_IsCorrect() {
            var player = new Mock<IPlayer>().Object;
            var sender = ICommandSender.FromPlayer(player);

            sender.Should().BeOfType<PlayerCommandSender>();
            sender.Player.Should().BeSameAs(player);
        }

        [Fact]
        public void FromPlayer_NullPlayer_ThrowsArgumentNullException() {
            Func<ICommandSender> func = () => ICommandSender.FromPlayer(null);

            func.Should().Throw<ArgumentNullException>();
        }
    }
}
