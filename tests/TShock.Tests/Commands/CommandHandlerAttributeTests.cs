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
        public void Ctor_NullQualifiedName_ThrowsArgumentNullException() {
            Func<CommandHandlerAttribute> func = () => new CommandHandlerAttribute(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void HelpText_GetWithResourceType() {
            var attribute = new CommandHandlerAttribute("tshock_test:test") {
                HelpText = nameof(TestClass.HelpText),
                ResourceType = typeof(TestClass)
            };

            attribute.HelpText.Should().Be(TestClass.HelpText);
        }

        [Fact]
        public void QualifiedName_Get() {
            var attribute = new CommandHandlerAttribute("tshock_test:test");

            attribute.QualifiedName.Should().Be("tshock_test:test");
        }

        [Fact]
        public void QualifiedName_GetWithResourceType() {
            var attribute = new CommandHandlerAttribute(nameof(TestClass.QualifiedName)) {
                ResourceType = typeof(TestClass)
            };

            attribute.QualifiedName.Should().Be(TestClass.QualifiedName);
        }

        [Fact]
        public void HelpText_SetNullValue_ThrowsArgumentNullException() {
            var attribute = new CommandHandlerAttribute("tshock_test:test");
            Action action = () => attribute.HelpText = null;

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UsageText_GetWithResourceType() {
            var attribute = new CommandHandlerAttribute("tshock_test:test") {
                UsageText = nameof(TestClass.UsageText),
                ResourceType = typeof(TestClass)
            };

            attribute.UsageText.Should().Be(TestClass.UsageText);
        }

        [Fact]
        public void UsageText_SetNullValue_ThrowsArgumentNullException() {
            var attribute = new CommandHandlerAttribute("tshock_test:test");
            Action action = () => attribute.UsageText = null;

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ResourceType_SetNullValue_ThrowsArgumentNullException() {
            var attribute = new CommandHandlerAttribute("tshock_test:test");
            Action action = () => attribute.ResourceType = null;

            action.Should().Throw<ArgumentNullException>();
        }

        private class TestClass {
            public static string QualifiedName => "tshock:qualified_name_test";
            public static string HelpText => "HelpText test";
            public static string UsageText => "UsageText test";
        }
    }
}
