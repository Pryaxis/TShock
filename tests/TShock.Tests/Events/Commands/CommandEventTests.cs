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
    public class CommandEventTests {
        [Fact]
        public void Ctor_NullCommand_ThrowsArgumentNullException() {
            Func<CommandEvent> func = () => new TestCommandEvent(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Command_Get() {
            var command = new Mock<ICommand>().Object;
            var e = new TestCommandEvent(command);

            e.Command.Should().BeSameAs(command);
        }

        [Fact]
        public void Command_SetNullValue_ThrowsArgumentNullException() {
            var command = new Mock<ICommand>().Object;
            var e = new TestCommandEvent(command);
            Action action = () => e.Command = null;

            action.Should().Throw<ArgumentNullException>();
        }

        private class TestCommandEvent : CommandEvent {
            public TestCommandEvent(ICommand command) : base(command) { }
        }
    }
}
