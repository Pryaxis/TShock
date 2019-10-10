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
using System.Reflection;
using Orion;
using Orion.Events;
using TShock.Commands.Parsers;
using TShock.Events.Commands;

namespace TShock.Commands {
    internal sealed class TShockCommandService : OrionService, ICommandService {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        private readonly Dictionary<Type, IArgumentParser> _parsers = new Dictionary<Type, IArgumentParser>();

        public IReadOnlyDictionary<string, ICommand> Commands => _commands;
        public IReadOnlyDictionary<Type, IArgumentParser> Parsers => _parsers;

        public EventHandlerCollection<CommandRegisterEventArgs> CommandRegister { get; }
            = new EventHandlerCollection<CommandRegisterEventArgs>();

        public EventHandlerCollection<CommandExecuteEventArgs> CommandExecute { get; }
            = new EventHandlerCollection<CommandExecuteEventArgs>();

        public EventHandlerCollection<CommandUnregisterEventArgs> CommandUnregister { get; }
            = new EventHandlerCollection<CommandUnregisterEventArgs>();

        public IReadOnlyCollection<ICommand> RegisterCommands(object handlerObject) {
            if (handlerObject is null) {
                throw new ArgumentNullException(nameof(handlerObject));
            }

            ICommand? RegisterCommand(ICommand command) {
                var args = new CommandRegisterEventArgs(command);
                CommandRegister.Invoke(this, args);
                if (args.IsCanceled()) {
                    return null;
                }

                _commands.Add(command.QualifiedName, command);
                return command;
            }

            return handlerObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany(m => m.GetCustomAttributes<CommandHandlerAttribute>(),
                    (m, a) => RegisterCommand(new TShockCommand(this, handlerObject, m, a)))
                .Where(c => c != null).ToList()!;
        }

        public void RegisterParser<TParse>(IArgumentParser<TParse> parser) =>
            _parsers[typeof(TParse)] = parser ?? throw new ArgumentNullException(nameof(parser));

        public bool UnregisterCommand(ICommand command) {
            if (command is null) {
                throw new ArgumentNullException(nameof(command));
            }

            var args = new CommandUnregisterEventArgs(command);
            CommandUnregister.Invoke(this, args);
            return !args.IsCanceled() && _commands.Remove(command.QualifiedName);
        }
    }
}
