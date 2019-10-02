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
using Serilog;
using Xunit;

namespace TShock.Commands {
    public class ConsoleCommandSenderTests {
        [Fact]
        public void Name_Get_IsCorrect() {
            var sender = ICommandSender.Console;

            sender.Name.Should().Be("Console");
        }

        [Fact]
        public void Log_Get_IsCorrect() {
            var sender = ICommandSender.Console;

            sender.Log.Should().BeSameAs(Log.Logger);
        }

        [Fact]
        public void Player_Get_IsCorrect() {
            var sender = ICommandSender.Console;

            sender.Player.Should().BeNull();
        }
    }
}
