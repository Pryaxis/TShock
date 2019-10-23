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
using TShock.Commands;
using Xunit;

namespace TShock.Events.Commands {
    public class CommandExecuteEventTests {
        [Fact]
        public void Ctor_NullCommand_ThrowsArgumentNullException() {
            var sender = new Mock<ICommandSender>().Object;
            Func<CommandEvent> func = () => new CommandExecuteEvent(null, sender, "");

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullSender_ThrowsArgumentNullException() {
            var command = new Mock<ICommand>().Object;
            Func<CommandExecuteEvent> func = () => new CommandExecuteEvent(command, null, "");

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullInput_ThrowsArgumentNullException() {
            var command = new Mock<ICommand>().Object;
            var sender = new Mock<ICommandSender>().Object;
            Func<CommandExecuteEvent> func = () => new CommandExecuteEvent(command, sender, null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Sender_Get() {
            var command = new Mock<ICommand>().Object;
            var sender = new Mock<ICommandSender>().Object;
            var e = new CommandExecuteEvent(command, sender, "");

            e.Sender.Should().BeSameAs(sender);
        }

        [Fact]
        public void Input_Get() {
            var command = new Mock<ICommand>().Object;
            var sender = new Mock<ICommandSender>().Object;
            var e = new CommandExecuteEvent(command, sender, "test");

            e.Input.Should().Be("test");
        }

        [Fact]
        public void Input_SetNullValue_ThrowsArgumentNullException() {
            var command = new Mock<ICommand>().Object;
            var sender = new Mock<ICommandSender>().Object;
            var e = new CommandExecuteEvent(command, sender, "");
            Action action = () => e.Input = null;

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
