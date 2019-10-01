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
        public void Ctor_InParam_ThrowsNotSupportedException() {
            var testClass = new TestClass();
            Func<ICommand> func = () => GetCommand(testClass, nameof(TestClass.TestCommand_NoIn));

            func.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void Ctor_OutParam_ThrowsNotSupportedException() {
            var testClass = new TestClass();
            Func<ICommand> func = () => GetCommand(testClass, nameof(TestClass.TestCommand_NoOut));

            func.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void Ctor_RefParam_ThrowsNotSupportedException() {
            var testClass = new TestClass();
            Func<ICommand> func = () => GetCommand(testClass, nameof(TestClass.TestCommand_NoRef));

            func.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void Ctor_PointerParam_ThrowsNotSupportedException() {
            var testClass = new TestClass();
            Func<ICommand> func = () => GetCommand(testClass, nameof(TestClass.TestCommand_NoPointer));

            func.Should().Throw<NotSupportedException>();
        }

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
        [InlineData("    -x", true, false)]
        [InlineData("--xxx   ", true, false)]
        [InlineData("-y     ", false, true)]
        [InlineData("  --yyy", false, true)]
        [InlineData("  -xy  ", true, true)]
        [InlineData("   -x    --yyy", true, true)]
        [InlineData("--xxx --yyy", true, true)]
        public void Invoke_Flags_IsCorrect(string input, bool expectedX, bool expectedY) {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_Flags));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, input);

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.X.Should().Be(expectedX);
            testClass.Y.Should().Be(expectedY);
        }

        [Theory]
        [InlineData("1", 1, 1234, 5678)]
        [InlineData("  1  ", 1, 1234, 5678)]
        [InlineData("--val=9001 1", 1, 9001, 5678)]
        [InlineData(" --val=9001     1", 1, 9001, 5678)]
        public void Invoke_Optionals_IsCorrect(string input, int expectedRequired, int expectedVal, int expectedVal2) {
            _mockCommandService.Setup(cs => cs.RegisteredParsers).Returns(new Dictionary<Type, IArgumentParser> {
                [typeof(int)] = new Int32Parser()
            });
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_Optionals));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, input);

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.Required.Should().Be(expectedRequired);
            testClass.Val.Should().Be(expectedVal);
            testClass.Val2.Should().Be(expectedVal2);
        }

        [Theory]
        [InlineData("", false, false, 10)]
        [InlineData("-f", true, false, 10)]
        [InlineData("--force", true, false, 10)]
        [InlineData("-r", false, true, 10)]
        [InlineData("--recursive", false, true, 10)]
        [InlineData("-fr", true, true, 10)]
        [InlineData("-rf", true, true, 10)]
        [InlineData("-f --recursive", true, true, 10)]
        [InlineData("-r --force", true, true, 10)]
        [InlineData("--recursive --force", true, true, 10)]
        [InlineData("--depth=1 --recursive --force", true, true, 1)]
        [InlineData("-r --force --depth=100 ", true, true, 100)]
        public void Invoke_FlagsAndOptionals_IsCorrect(string input, bool expectedForce, bool expectedRecursive,
                                                       int expectedDepth) {
            _mockCommandService.Setup(cs => cs.RegisteredParsers).Returns(new Dictionary<Type, IArgumentParser> {
                [typeof(int)] = new Int32Parser()
            });
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_FlagsAndOptionals));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, input);

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.Force.Should().Be(expectedForce);
            testClass.Recursive.Should().Be(expectedRecursive);
            testClass.Depth.Should().Be(expectedDepth);
        }

        [Fact]
        public void Invoke_OptionalGetsRenamed() {
            _mockCommandService.Setup(cs => cs.RegisteredParsers).Returns(new Dictionary<Type, IArgumentParser> {
                [typeof(int)] = new Int32Parser()
            });
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_OptionalRename));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, "--hyphenated-optional-is-long=60");

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.HyphenatedOptionalIsLong.Should().Be(60);
        }

        [Theory]
        [InlineData("-xyz")]
        [InlineData("-z")]
        public void Invoke_UnexpectedShortFlag_ThrowsParseException(string input) {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_Flags));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Theory]
        [InlineData("--this-is-not-ok")]
        [InlineData("--neither-is-this")]
        public void Invoke_UnexpectedLongFlag_ThrowsParseException(string input) {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_Flags));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Theory]
        [InlineData("--required=123")]
        [InlineData("--not-ok=test")]
        public void Invoke_UnexpectedOptional_ThrowsParseException(string input) {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand_Optionals));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("bcd")]
        public void Invoke_TooManyArguments_ThrowsParseException(string input) {
            var testClass = new TestClass();
            var command = GetCommand(testClass, nameof(TestClass.TestCommand));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
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
            public int Required { get; private set; }
            public int Val { get; private set; }
            public int Val2 { get; private set; }
            public bool Force { get; private set; }
            public bool Recursive { get; private set; }
            public int Depth { get; private set; }
            public int HyphenatedOptionalIsLong { get; private set; }

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

            [CommandHandler("tshock_tests:test_no_ptr")]
            public unsafe void TestCommand_NoPointer(ICommandSender sender, int* x) { }
            
            [CommandHandler("tshock_tests:test_flags")]
            public void TestCommand_Flags(ICommandSender sender, [Flag('x', "xxx")] bool x, [Flag('y', "yyy")] bool y) {
                Sender = sender;
                X = x;
                Y = y;
            }
            
            [CommandHandler("tshock_tests:test_optionals")]
            public void TestCommand_Optionals(ICommandSender sender, int required, int val = 1234, int val2 = 5678) {
                Sender = sender;
                Required = required;
                Val = val;
                Val2 = val2;
            }

            [CommandHandler("tshock_tests:test_flags_and_optionals")]
            public void TestCommand_FlagsAndOptionals(ICommandSender sender, [Flag('f', "force")] bool force,
                                                      [Flag('r', "recursive")] bool recursive, int depth = 10) {
                Sender = sender;
                Force = force;
                Recursive = recursive;
                Depth = depth;
            }

            [CommandHandler("tshock_tests:test_optional_rename")]
            public void TestCommand_OptionalRename(ICommandSender sender, int hyphenated_optional_is_long = 100) {
                Sender = sender;
                HyphenatedOptionalIsLong = hyphenated_optional_is_long;
            }
        }
    }
}
