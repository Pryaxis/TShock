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

namespace TShock.Commands.Parsers.Attributes {
    public class FlagAttributeTests {
        [Fact]
        public void Ctor_NullFlag_ThrowsArgumentNullException() {
            Func<FlagAttribute> func = () => new FlagAttribute(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_NullAlternateFlags_ThrowsArgumentNullException() {
            Func<FlagAttribute> func = () => new FlagAttribute("", null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Ctor_AlternateFlagsNullElement_ThrowsArgumentException() {
            Func<FlagAttribute> func = () => new FlagAttribute("", "test", null);

            func.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Flags_Get() {
            var attribute = new FlagAttribute("test1", "test2", "test3");

            attribute.Flags.Should().BeEquivalentTo("test1", "test2", "test3");
        }
    }
}
