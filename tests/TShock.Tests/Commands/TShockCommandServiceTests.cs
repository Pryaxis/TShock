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
using System.Linq;
using FluentAssertions;
using Moq;
using Orion;
using Orion.Events;
using Serilog.Core;
using TShock.Commands.Parsers;
using TShock.Events.Commands;
using Xunit;

namespace TShock.Commands {
    public class TShockCommandServiceTests {
        [Fact]
        public void Commands_Get() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();

            var commands = commandService.RegisterCommands(testClass).ToList();

            commandService.Commands.Keys.Should().Contain(
                new[] { "tshock_tests:test", "tshock_tests:test2", "tshock_tests2:test" });
            commandService.Commands.Values.Should().Contain(commands);
        }

        [Fact]
        public void Parsers_Get() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var parser = new Mock<IArgumentParser<object>>().Object;
            commandService.RegisterParser(parser);

            commandService.Parsers.Should().Contain(new KeyValuePair<Type, IArgumentParser>(typeof(object), parser));
        }

        [Fact]
        public void RegisterCommands() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();

            var commands = commandService.RegisterCommands(testClass).ToList();

            commands.Should().HaveCount(3);
            foreach (var command in commands) {
                command.QualifiedName.Should().BeOneOf("tshock_tests:test", "tshock_tests:test2", "tshock_tests2:test");
            }
        }

        [Fact]
        public void RegisterCommands_NullObj_ThrowsArgumentNullException() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            Func<IReadOnlyCollection<ICommand>> func = () => commandService.RegisterCommands(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RegisterParser_NullParser_ThrowsArgumentNullException() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            Action action = () => commandService.RegisterParser<object>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UnregisterCommand() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var commands = commandService.RegisterCommands(testClass).ToList();
            var command = commands[0];

            commandService.UnregisterCommand(command).Should().BeTrue();

            commandService.Commands.Keys.Should().NotContain(command.QualifiedName);
            commandService.Commands.Values.Should().NotContain(command);
        }

        [Fact]
        public void UnregisterCommand_CommandDoesntExist_ReturnsFalse() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var mockCommand = new Mock<ICommand>();
            mockCommand.SetupGet(c => c.QualifiedName).Returns("test");

            commandService.UnregisterCommand(mockCommand.Object).Should().BeFalse();
        }

        [Fact]
        public void UnregisterCommand_NullCommand_ThrowsArgumentNullException() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            Func<bool> func = () => commandService.UnregisterCommand(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CommandRegister_IsTriggered() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var isRun = false;
            var testClass = new TestClass();
            kernel.RegisterHandler<CommandRegisterEvent>(e => {
                isRun = true;
                e.Command.QualifiedName.Should().BeOneOf(
                    "tshock_tests:test", "tshock_tests:test2", "tshock_tests2:test");
            }, Logger.None);

            commandService.RegisterCommands(testClass);

            isRun.Should().BeTrue();
        }

        [Fact]
        public void CommandRegister_Canceled() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            kernel.RegisterHandler<CommandRegisterEvent>(e => e.Cancel(), Logger.None);

            commandService.RegisterCommands(testClass).Should().BeEmpty();
        }

        [Fact]
        public void CommandUnregister_IsTriggered() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var isRun = false;
            var testClass = new TestClass();
            var commands = commandService.RegisterCommands(testClass).ToList();
            var command = commands[0];
            kernel.RegisterHandler<CommandUnregisterEvent>(e => {
                isRun = true;
                e.Command.Should().BeSameAs(command);
            }, Logger.None);

            commandService.UnregisterCommand(command);

            isRun.Should().BeTrue();
        }

        [Fact]
        public void CommandUnregister_Canceled() {
            using var kernel = new OrionKernel(Logger.None);
            using var commandService = new TShockCommandService(kernel, Logger.None);
            var testClass = new TestClass();
            var commands = commandService.RegisterCommands(testClass).ToList();
            var command = commands[0];
            kernel.RegisterHandler<CommandUnregisterEvent>(e => e.Cancel(), Logger.None);

            commandService.UnregisterCommand(command).Should().BeFalse();

            commandService.Commands.Values.Should().Contain(command);
        }

        private class TestClass {
            [CommandHandler("tshock_tests:test")]
            public void TestCommand() { }

            [CommandHandler("tshock_tests:test2")]
            public void TestCommand2() { }

            [CommandHandler("tshock_tests2:test")]
            public void TestCommandTest() { }
        }
    }
}
