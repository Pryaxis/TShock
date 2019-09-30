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
    public class FlagAttributeTests {
        [Fact]
        public void Ctor_NullLongFlag_ThrowsArgumentNullException() {
            Func<FlagAttribute> func = () => new FlagAttribute('f', null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ShortFlag_Get_IsCorrect() {
            var attribute = new FlagAttribute('f', "force");

            attribute.ShortFlag.Should().Be('f');
        }

        [Fact]
        public void LongFlag_Get_IsCorrect() {
            var attribute = new FlagAttribute('f', "force");

            attribute.LongFlag.Should().Be("force");
        }
    }
}
