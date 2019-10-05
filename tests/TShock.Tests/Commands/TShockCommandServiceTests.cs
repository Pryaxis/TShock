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
using Orion.Events.Extensions;
using TShock.Commands.Parsers;
using Xunit;

namespace TShock.Commands {
    public class TShockCommandServiceTests : IDisposable {
        private readonly ICommandService _commandService;

        public TShockCommandServiceTests() {
            _commandService = new TShockCommandService();
        }

        public void Dispose() {
            _commandService.Dispose();
        }

        [Fact]
        public void RegisteredCommands_Get() {
            var testClass = new TestClass();

            var commands = _commandService.RegisterCommands(testClass).ToList();
            
            _commandService.Commands.Should().BeEquivalentTo(commands);
        }

        [Fact]
        public void RegisteredParsers_Get() {
            var parser = new Mock<IArgumentParser<object>>().Object;
            _commandService.RegisterParser(parser);

            _commandService.Parsers.Should().Contain(new KeyValuePair<Type, IArgumentParser>(typeof(object), parser));
        }

        [Fact]
        public void RegisterCommands() {
            var testClass = new TestClass();

            var commands = _commandService.RegisterCommands(testClass).ToList();
            
            commands.Should().HaveCount(2);
            foreach (var command in commands) {
                command.HandlerObject.Should().BeSameAs(testClass);
                command.QualifiedName.Should().BeOneOf("tshock_tests:test", "tshock_tests:test2");
            }
        }

        [Fact]
        public void RegisterCommands_NullObj_ThrowsArgumentNullException() {
            Func<IReadOnlyCollection<ICommand>> func = () => _commandService.RegisterCommands(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void RegisterParser_NullParser_ThrowsArgumentNullException() {
            Action action = () => _commandService.RegisterParser<object>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void UnregisterCommand() {
            var testClass = new TestClass();
            var commands = _commandService.RegisterCommands(testClass).ToList();
            var command = commands[0];

            _commandService.UnregisterCommand(command).Should().BeTrue();

            _commandService.Commands.Should().NotContain(command);
        }

        [Fact]
        public void UnregisterCommand_NonexistentCommand_ReturnsFalse() {
            var command = new Mock<ICommand>().Object;

            _commandService.UnregisterCommand(command).Should().BeFalse();
        }

        [Fact]
        public void UnregisterCommand_NullCommand_ThrowsArgumentNullException() {
            Func<bool> func = () => _commandService.UnregisterCommand(null);

            func.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CommandRegister_IsTriggered() {
            var isRun = false;
            var testClass = new TestClass();
            _commandService.CommandRegister += (sender, args) => {
                isRun = true;
                args.Command.HandlerObject.Should().BeSameAs(testClass);
                args.Command.QualifiedName.Should().BeOneOf("tshock_tests:test", "tshock_tests:test2");
            };

            _commandService.RegisterCommands(testClass);

            isRun.Should().BeTrue();
        }

        [Fact]
        public void CommandRegister_Canceled() {
            var testClass = new TestClass();
            _commandService.CommandRegister += (sender, args) => {
                args.Cancel();
            };

            _commandService.RegisterCommands(testClass).Should().BeEmpty();
        }

        [Fact]
        public void CommandUnregister_IsTriggered() {
            var isRun = false;
            var testClass = new TestClass();
            var commands = _commandService.RegisterCommands(testClass).ToList();
            var command = commands[0];
            _commandService.CommandUnregister += (sender, args) => {
                isRun = true;
                args.Command.Should().BeSameAs(command);
            };

            _commandService.UnregisterCommand(command);

            isRun.Should().BeTrue();
        }

        [Fact]
        public void CommandUnregister_Canceled() {
            var testClass = new TestClass();
            var commands = _commandService.RegisterCommands(testClass).ToList();
            var command = commands[0];
            _commandService.CommandUnregister += (sender, args) => {
                args.Cancel();
            };

            _commandService.UnregisterCommand(command).Should().BeFalse();

            _commandService.Commands.Should().Contain(command);
        }

        private class TestClass {
            [CommandHandler("tshock_tests:test")]
            public void TestCommand() { }

            [CommandHandler("tshock_tests:test2")]
            public void TestCommand2() { }
        }
    }
}
