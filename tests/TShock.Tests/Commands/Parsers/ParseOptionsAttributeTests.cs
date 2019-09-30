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

namespace TShock.Commands.Parsers {
    public class ParseOptionsAttributeTests {
        [Fact]
        public void Ctor_NullOptions_ThrowsArgumentNullException() {
            Func<ParseOptionsAttribute> func = () => new ParseOptionsAttribute(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Options_Get_IsCorrect() {
            var attribute = new ParseOptionsAttribute("test", "test2");

            attribute.Options.Should().BeEquivalentTo("test", "test2");
        }
    }
}
