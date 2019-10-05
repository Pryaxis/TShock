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

namespace TShock.Utils.Extensions {
    public class ReadOnlySpanExtensionsTests {
        [Fact]
        public void IndexOfOrEnd() => "abcde".AsSpan().IndexOfOrEnd('b').Should().Be(1);

        [Fact]
        public void IndexOfOrEnd_AtEnd() => "abcde".AsSpan().IndexOfOrEnd('f').Should().Be(5);
    }
}
