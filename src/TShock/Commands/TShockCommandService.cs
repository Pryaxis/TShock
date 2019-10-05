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
using System.Globalization;
using System.Linq;
using System.Reflection;
using Orion;
using Orion.Events;
using Orion.Events.Extensions;
using TShock.Commands.Parsers;
using TShock.Events.Commands;
using TShock.Properties;
using TShock.Utils.Extensions;

namespace TShock.Commands {
    internal sealed class TShockCommandService : OrionService, ICommandService {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        private readonly Dictionary<Type, IArgumentParser> _parsers = new Dictionary<Type, IArgumentParser>();

        // Map from command name -> set of qualified command names.
        private readonly IDictionary<string, ISet<string>> _qualifiedNames = new Dictionary<string, ISet<string>>();

        public IReadOnlyDictionary<string, ICommand> Commands => _commands;
        public IReadOnlyDictionary<Type, IArgumentParser> Parsers => _parsers;
        public EventHandlerCollection<CommandRegisterEventArgs>? CommandRegister { get; set; }
        public EventHandlerCollection<CommandExecuteEventArgs>? CommandExecute { get; set; }
        public EventHandlerCollection<CommandUnregisterEventArgs>? CommandUnregister { get; set; }

        public TShockCommandService() {
            RegisterParser(new Int32Parser());
            RegisterParser(new StringParser());
        }

        public IReadOnlyCollection<ICommand> RegisterCommands(object handlerObject) {
            if (handlerObject is null) {
                throw new ArgumentNullException(nameof(handlerObject));
            }

            var registeredCommands = new List<ICommand>();

            void RegisterCommand(ICommand command) {
                var args = new CommandRegisterEventArgs(command);
                CommandRegister?.Invoke(this, args);
                if (args.IsCanceled()) {
                    return;
                }

                var qualifiedName = command.QualifiedName;
                var name = qualifiedName.Substring(qualifiedName.IndexOf(':', StringComparison.Ordinal) + 1);
                _qualifiedNames[name] = _qualifiedNames.GetValueOrDefault(name, () => new HashSet<string>());
                _qualifiedNames[name].Add(qualifiedName);

                _commands.Add(qualifiedName, command);
                registeredCommands.Add(command);
            }

            foreach (var command in handlerObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .SelectMany(m => m.GetCustomAttributes<CommandHandlerAttribute>(),
                        (handler, attribute) => (handler, attribute))
                    .Select(t => new TShockCommand(this, t.attribute.QualifiedCommandName, handlerObject, t.handler))) {
                RegisterCommand(command);
            }

            return registeredCommands;
        }

        public void RegisterParser<TParse>(IArgumentParser<TParse> parser) =>
            _parsers[typeof(TParse)] = parser ?? throw new ArgumentNullException(nameof(parser));

        public ICommand FindCommand(ref ReadOnlySpan<char> input) {
            string ProcessQualifiedName(ref ReadOnlySpan<char> input) {
                input = input.TrimStart();
                if (input.IsEmpty) {
                    throw new CommandParseException(Resources.CommandParse_MissingCommand);
                }

                var space = input.IndexOfOrEnd(' ');
                var maybeQualifiedName = input[..space].ToString();
                var isQualifiedName = maybeQualifiedName.IndexOf(':', StringComparison.Ordinal) >= 0;
                input = input[space..];
                if (isQualifiedName) {
                    return maybeQualifiedName;
                }

                var qualifiedNames = _qualifiedNames.GetValueOrDefault(maybeQualifiedName, () => new HashSet<string>());
                if (qualifiedNames.Count == 0) {
                    throw new CommandParseException(
                        string.Format(CultureInfo.InvariantCulture, Resources.CommandParse_UnrecognizedCommand,
                            maybeQualifiedName));
                }

                if (qualifiedNames.Count > 1) {
                    throw new CommandParseException(
                        string.Format(CultureInfo.InvariantCulture, Resources.CommandParse_AmbiguousName,
                            maybeQualifiedName, string.Join(", ", qualifiedNames.Select(n => $"\"/{n}\""))));
                }

                return qualifiedNames.Single();
            }

            var qualifiedName = ProcessQualifiedName(ref input);
            if (!_commands.TryGetValue(qualifiedName, out var command)) {
                throw new CommandParseException(
                    string.Format(CultureInfo.InvariantCulture, Resources.CommandParse_UnrecognizedCommand,
                        qualifiedName));
            }

            return command;
        }

        public bool UnregisterCommand(ICommand command) {
            if (command is null) {
                throw new ArgumentNullException(nameof(command));
            }

            var args = new CommandUnregisterEventArgs(command);
            CommandUnregister?.Invoke(this, args);
            if (args.IsCanceled()) {
                return false;
            }

            var qualifiedName = command.QualifiedName;
            var name = qualifiedName.Substring(qualifiedName.IndexOf(':', StringComparison.Ordinal) + 1);
            _qualifiedNames[name] = _qualifiedNames.GetValueOrDefault(name, () => new HashSet<string>());
            _qualifiedNames[name].Remove(qualifiedName);

            return _commands.Remove(qualifiedName);
        }
    }
}
