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
using Xunit;

namespace TShock.Utils.Extensions {
    public class StringExtensionsTests {
        [Fact]
        public void WithColor() => "test".WithColor(new Color(0x12, 0x34, 0x56)).Should().Be("[c/123456:test]");

        [Fact]
        public void WithColor_NullStr_ThrowsArgumentNullException() {
            Func<string> func = () => StringExtensions.WithColor(null, Color.White);

            func.Should().Throw<ArgumentNullException>();
        }
    }
}
