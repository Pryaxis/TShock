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
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace TShock.Utils.Extensions {
    public class DictionaryExtensionsTests {
        [Fact]
        public void GetValueOrDefault() {
            var dictionary = new Dictionary<string, int>();

            dictionary.GetValueOrDefault("test").Should().Be(0);
        }

        [Fact]
        public void GetValueOrDefault_KeyExists() {
            var dictionary = new Dictionary<string, int> {
                ["test"] = 10
            };

            dictionary.GetValueOrDefault("test", () => 50).Should().Be(10);
        }

        [Fact]
        public void GetValueOrDefault_KeyDoesntExist() {
            var dictionary = new Dictionary<string, int>();

            dictionary.GetValueOrDefault("test", () => 50).Should().Be(50);
        }

        [Fact]
        public void GetValueOrDefault_Create() {
            var dictionary = new Dictionary<string, int>();

            dictionary.GetValueOrDefault("test", () => 50, true).Should().Be(50);

            dictionary["test"].Should().Be(50);
        }

        [Fact]
        public void GetValueOrDefault_NullDictionary_ThrowsArgumentNullException() {
            Func<int> func = () => DictionaryExtensions.GetValueOrDefault(null, "", () => 0);

            func.Should().Throw<ArgumentNullException>();
        }
    }
}
