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
using Xunit;

namespace TShock.Commands {
    public class CommandHandlerAttributeTests {
        [Fact]
        public void Ctor_NullCommandName_ThrowsArgumentNullException() {
            Func<CommandHandlerAttribute> func = () => new CommandHandlerAttribute(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullCommandSubNames_ThrowsArgumentNullException() {
            Func<CommandHandlerAttribute> func = () => new CommandHandlerAttribute("", null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CommandName_Get_IsCorrect() {
            var attribute = new CommandHandlerAttribute("test");

            attribute.CommandName.Should().Be("test");
        }

        [Fact]
        public void CommandSubNames_Get_IsCorrect() {
            var attribute = new CommandHandlerAttribute("", "test1", "test2");

            attribute.CommandSubNames.Should().BeEquivalentTo("test1", "test2");
        }
    }
}
