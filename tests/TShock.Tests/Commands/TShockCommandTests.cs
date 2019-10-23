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
using Orion;
using Orion.Events;
using Serilog.Core;
using TShock.Commands.Exceptions;
using TShock.Commands.Parsers;
using TShock.Commands.Parsers.Attributes;
using TShock.Events.Commands;
using TShock.Properties;
using Xunit;

namespace TShock.Commands {
    public class TShockCommandTests {
        [Fact]
        public void QualifiedName_Get() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var attribute = new CommandHandlerAttribute("test");
            var command = new TShockCommand(
                commandService, "",
                typeof(TShockCommandTests).GetMethod(nameof(QualifiedName_Get)), attribute);

            command.QualifiedName.Should().Be("test");
        }

        [Fact]
        public void HelpText_Get() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var attribute = new CommandHandlerAttribute("test") { HelpText = "HelpTest" };
            var command = new TShockCommand(
                commandService, "",
                typeof(TShockCommandTests).GetMethod(nameof(HelpText_Get)), attribute);

            command.HelpText.Should().Be("HelpTest");
        }

        [Fact]
        public void HelpText_GetMissing() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var attribute = new CommandHandlerAttribute("test");
            var command = new TShockCommand(
                commandService, "",
                typeof(TShockCommandTests).GetMethod(nameof(HelpText_GetMissing)), attribute);

            command.HelpText.Should().Be(Resources.Command_MissingHelpText);
        }

        [Fact]
        public void UsageText_Get() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var attribute = new CommandHandlerAttribute("test") { UsageText = "UsageTest" };
            var command = new TShockCommand(
                commandService, "",
                typeof(TShockCommandTests).GetMethod(nameof(UsageText_Get)), attribute);

            command.UsageText.Should().Be("UsageTest");
        }

        [Fact]
        public void UsageText_GetMissing() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var attribute = new CommandHandlerAttribute("test");
            var command = new TShockCommand(
                commandService, "",
                typeof(TShockCommandTests).GetMethod(nameof(UsageText_GetMissing)), attribute);

            command.UsageText.Should().Be(Resources.Command_MissingUsageText);
        }

        [Fact]
        public void ShouldBeLogged_Get() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var attribute = new CommandHandlerAttribute("test") { ShouldBeLogged = false };
            var command = new TShockCommand(
                commandService, "",
                typeof(TShockCommandTests).GetMethod(nameof(ShouldBeLogged_Get)), attribute);

            command.ShouldBeLogged.Should().Be(false);
        }

        [Fact]
        public void Invoke_Sender() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, "");

            testClass.Sender.Should().BeSameAs(commandSender);
        }

        [Theory]
        [InlineData("1 test", 1, "test")]
        [InlineData(@"-56872 ""test abc\"" def""", -56872, "test abc\" def")]
        public void Invoke_SenderIntString(string input, int expectedInt, string expectedString) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            commandService.RegisterParser(new Int32Parser());
            commandService.RegisterParser(new StringParser());

            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Int_String));
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
        public void Invoke_Flags(string input, bool expectedX, bool expectedY) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            commandService.RegisterParser(new Int32Parser());
            commandService.RegisterParser(new StringParser());

            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Flags));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, input);

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.X.Should().Be(expectedX);
            testClass.Y.Should().Be(expectedY);
        }

        [Theory]
        [InlineData("1", 1, 1234, 5678)]
        [InlineData("  1  ", 1, 1234, 5678)]
        [InlineData("1 2", 1, 2, 5678)]
        [InlineData("1 2 3", 1, 2, 3)]
        [InlineData("--val=9001 1", 1, 9001, 5678)]
        [InlineData(" --val=9001     1", 1, 9001, 5678)]
        [InlineData("--val2=5678 1", 1, 1234, 5678)]
        [InlineData(" --val2=5678     1", 1, 1234, 5678)]
        public void Invoke_Optionals(string input, int expectedRequired, int expectedVal, int expectedVal2) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            commandService.RegisterParser(new Int32Parser());
            commandService.RegisterParser(new StringParser());

            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Optionals));
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
        [InlineData("--depth=1 --recursive -f", true, true, 1)]
        [InlineData("--force -r --depth=100 ", true, true, 100)]
        [InlineData("--force -r --depth=   100 ", true, true, 100)]
        public void Invoke_FlagsAndOptionals(
                string input, bool expectedForce, bool expectedRecursive, int expectedDepth) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            commandService.RegisterParser(new Int32Parser());
            commandService.RegisterParser(new StringParser());

            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_FlagsAndOptionals));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, input);

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.Force.Should().Be(expectedForce);
            testClass.Recursive.Should().Be(expectedRecursive);
            testClass.Depth.Should().Be(expectedDepth);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1", 1)]
        [InlineData("1  2", 1, 2)]
        [InlineData("    -1  2   -5", -1, 2, -5)]
        public void Invoke_Params(string input, params int[] expectedInts) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            commandService.RegisterParser(new Int32Parser());
            commandService.RegisterParser(new StringParser());

            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Params));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, input);

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.Ints.Should().BeEquivalentTo(expectedInts);
        }

        [Fact]
        public void Invoke_OptionalGetsRenamed() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            commandService.RegisterParser(new Int32Parser());
            commandService.RegisterParser(new StringParser());

            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_OptionalRename));
            var commandSender = new Mock<ICommandSender>().Object;

            command.Invoke(commandSender, "--hyphenated-optional-is-long=60");

            testClass.Sender.Should().BeSameAs(commandSender);
            testClass.HyphenatedOptionalIsLong.Should().Be(60);
        }

        [Fact]
        public void Invoke_TriggersCommandExecute() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand));
            var commandSender = new Mock<ICommandSender>().Object;

            var isRun = false;
            kernel.RegisterHandler<CommandExecuteEvent>(e => {
                isRun = true;
                e.Command.Should().Be(command);
                e.Input.Should().BeEmpty();
            }, Logger.None);

            command.Invoke(commandSender, "");

            testClass.Sender.Should().BeSameAs(commandSender);
            isRun.Should().BeTrue();
        }

        [Fact]
        public void Invoke_CommandExecuteCanceled_IsCanceled() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand));
            var commandSender = new Mock<ICommandSender>().Object;
            kernel.RegisterHandler<CommandExecuteEvent>(e => e.Cancel(), Logger.None);

            command.Invoke(commandSender, "failing input");
        }

        [Theory]
        [InlineData("1 ")]
        [InlineData("-7345734    ")]
        public void Invoke_MissingArg_ThrowsCommandParseException(string input) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Int_String));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Theory]
        [InlineData("-xyz")]
        [InlineData("-z")]
        public void Invoke_UnexpectedShortFlag_ThrowsCommandParseException(string input) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Flags));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Theory]
        [InlineData("--this-is-not-ok")]
        [InlineData("--neither-is-this")]
        public void Invoke_UnexpectedLongFlag_ThrowsCommandParseException(string input) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Flags));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Theory]
        [InlineData("--required=123")]
        [InlineData("--not-ok=test")]
        public void Invoke_UnexpectedOptional_ThrowsCommandParseException(string input) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Optionals));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Theory]
        [InlineData("-")]
        [InlineData("- ")]
        [InlineData("--")]
        [InlineData("-- ")]
        [InlineData("--= ")]
        public void Invoke_InvalidHyphenatedArgs_ThrowsCommandParseException(string input) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_FlagsAndOptionals));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Fact]
        public void Invoke_UnexpectedArgType_ThrowsCommandParseException() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_NoTestClass));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, "");

            action.Should().Throw<CommandParseException>();
        }

        [Theory]
        [InlineData("a")]
        [InlineData("bcd")]
        public void Invoke_TooManyArguments_ThrowsCommandParseException(string input) {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, input);

            action.Should().Throw<CommandParseException>();
        }

        [Fact]
        public void Invoke_ThrowsException_ThrowsCommandException() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand_Exception));
            var commandSender = new Mock<ICommandSender>().Object;
            Action action = () => command.Invoke(commandSender, "");

            action.Should().Throw<CommandExecuteException>().WithInnerException<NotImplementedException>();
        }

        [Fact]
        public void Invoke_NullSender_ThrowsArgumentNullException() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var command = GetCommand(commandService, testClass, nameof(TestClass.TestCommand));
            Action action = () => command.Invoke(null, "");

            action.Should().Throw<ArgumentNullException>();
        }

        private ICommand GetCommand(TShockCommandService commandService, TestClass testClass, string methodName) {
            var handler = typeof(TestClass).GetMethod(methodName);
            var attribute = handler.GetCustomAttribute<CommandHandlerAttribute>();
            return new TShockCommand(commandService, testClass, handler, attribute);
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
            public int[] Ints { get; private set; }

            [CommandHandler("tshock_tests:test")]
            public void TestCommand(ICommandSender sender) => Sender = sender;

            [CommandHandler("tshock_tests:test_int_string")]
            public void TestCommand_Int_String(ICommandSender sender, int @int, string @string) {
                Sender = sender;
                Int = @int;
                String = @string;
            }

            [CommandHandler("tshock_tests:test_flags")]
            public void TestCommand_Flags(ICommandSender sender, [Flag("x", "xxx")] bool x, [Flag("y", "yyy")] bool y) {
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
            public void TestCommand_FlagsAndOptionals(
                    ICommandSender sender, [Flag("f", "force")] bool force, [Flag("r", "recursive")] bool recursive,
                    int depth = 10) {
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

            [CommandHandler("tshock_tests:test_params")]
            public void TestCommand_Params(ICommandSender sender, params int[] ints) {
                Sender = sender;
                Ints = ints;
            }

            [CommandHandler("tshock_tests:exception")]
            public void TestCommand_Exception(ICommandSender sender) => throw new NotImplementedException();

            [CommandHandler("tshock_tests:test_no_testclass")]
            public void TestCommand_NoTestClass(ICommandSender sender, TestClass testClass) { }
        }
    }
}
