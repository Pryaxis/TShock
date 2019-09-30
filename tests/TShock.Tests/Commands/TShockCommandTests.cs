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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Moq;
using TShock.Commands.Parsers;
using Xunit;

namespace TShock.Commands {
    public class TShockCommandTests {
        private readonly Mock<ICommandService> _mockCommandService = new Mock<ICommandService>();

        [Fact]
        public void Invoke_Sender_IsCorrect() {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, "");

            testClass.Sender.Should().BeSameAs(commandSender);
        }

        [Theory]
        [InlineData("1 test", 1, "test")]
        [InlineData(@"-56872 ""test abc\"" def""", -56872, "test abc\" def")]
        public void Invoke_SenderIntString_IsCorrect(string input, int expectedInt, string expectedString) {
            // This test isn't super isolated, but it should be fine. Mocking IArgumentParser is kind of sketchy because
            // of ReadOnlySpan<T>.
            _mockCommandService.Setup(cs => cs.RegisteredParsers).Returns(new Dictionary<Type, IArgumentParser> {
                [typeof(int)] = new Int32Parser(),
                [typeof(string)] = new StringParser()
            });

            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_Int_String));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, input);

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.Int.Should().Be(expectedInt);
            testClass.String.Should().Be(expectedString);
        }

        [Theory]
        [InlineData("", false, false)]
        [InlineData("-x", true, false)]
        [InlineData("-y", false, true)]
        [InlineData("-xy", true, true)]
        public void Invoke_Flags_IsCorrect(string input, bool expectedX, bool expectedY) {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_Flags));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, input);

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.X.Should().Be(expectedX);
            testClass.Y.Should().Be(expectedY);
        }

        [Fact]
        public void Invoke_InParam_ThrowsParseException() {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_NoIn));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, "");

            action.Should().Throw<ParseException>();
        }

        [Fact]
        public void Invoke_OutParam_ThrowsParseException() {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_NoOut));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, "");

            action.Should().Throw<ParseException>();
        }

        [Fact]
        public void Invoke_RefParam_ThrowsParseException() {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_NoRef));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, "");

            action.Should().Throw<ParseException>();
        }

        [Fact]
        public void Invoke_NullSender_ThrowsArgumentNullException() {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand));
            Action action = () => command.Invoke(null, "");

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Invoke_NullInput_ThrowsArgumentNullException() {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, null);

            action.Should().Throw<ArgumentNullException>();
        }

        private ICommand GetCommand(TestClass testClass, string methodName) {
            var handler = typeof(TestClass).GetMethod(methodName);
            var attribute = handler.GetCustomAttribute<CommandHandlerAttribute>();
            return new TShockCommand(_mockCommandService.Object, attribute, testClass, handler);
        }

        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Testing")]
        private class TestClass {
            public ICommandSender Sender { get; private set; }
            public int Int { get; private set; }
            public string String { get; private set; }
            public bool X { get; private set; }
            public bool Y { get; private set; }

            [CommandHandler("tshock_tests:test")]
            public void TestCommand(ICommandSender sender) {
                Sender = sender;
            }

            [CommandHandler("tshock_tests:test_int_string")]
            public void TestCommand_Int_String(ICommandSender sender, int @int, string @string) {
                Sender = sender;
                Int = @int;
                String = @string;
            }

            [CommandHandler("tshock_tests:test_no_in")]
            public void TestCommand_NoIn(ICommandSender sender, in int x) { }

            [CommandHandler("tshock_tests:test_no_out")]
            public void TestCommand_NoOut(ICommandSender sender, out int x) {
                x = 0;
            }

            [CommandHandler("tshock_tests:test_no_out")]
            public void TestCommand_NoRef(ICommandSender sender, ref int x) { }
            
            [CommandHandler("tshock_tests:test_flags")]
            public void TestCommand_Flags(ICommandSender sender, [Flag('x', "xxx")] bool x, [Flag('y', "yyy")] bool y) {
                Sender = sender;
                X = x;
                Y = y;
            }
        }
    }
}
