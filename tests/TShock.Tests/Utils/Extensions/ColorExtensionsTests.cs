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

using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Xunit;

namespace TShock.Utils.Extensions {
    public class ColorExtensionsTests {
        public static readonly IEnumerable<object[]> ToHexStringData = new List<object[]> {
            new object[] { new Color(0xf3, 0x20, 0x00), "f32000" },
            new object[] { new Color(0x01, 0x02, 0x03), "010203" }
        };

        [Theory]
        [MemberData(nameof(ToHexStringData))]
        public void ToHexString(Color color, string expected) => color.ToHexString().Should().Be(expected);
    }
}
