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

namespace TShock.Utils {
    public class ResourceHelperTests {
        [Fact]
        public void LoadResource_PublicProperty() =>
            ResourceHelper.LoadResource<int>(typeof(TestClass), nameof(TestClass.PublicProperty))
                .Should().Be(TestClass.PublicProperty);

        [Fact]
        public void LoadResource_InternalProperty() =>
            ResourceHelper.LoadResource<int>(typeof(TestClass), nameof(TestClass.InternalProperty))
                .Should().Be(TestClass.InternalProperty);

        [Fact]
        public void LoadResource_NullResourceType_ThrowsArgumentNullException() {
            Func<int> func = () => ResourceHelper.LoadResource<int>(null, "");

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LoadResource_NullName_ThrowsArgumentNullException() {
            Func<int> func = () => ResourceHelper.LoadResource<int>(typeof(TestClass), null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void LoadResource_InvalidProperty_ThrowsArgumentException() {
            Func<int> func = () => ResourceHelper.LoadResource<int>(typeof(TestClass), "DoesNotExist");

            func.Should().Throw<ArgumentException>();
        }

        private class TestClass {
            public static int PublicProperty => 123;
            internal static int InternalProperty => 456;
        }
    }
}
