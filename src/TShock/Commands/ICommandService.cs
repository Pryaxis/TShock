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
using Orion;
using Orion.Events;
using TShock.Commands.Parsers;
using TShock.Events.Commands;

namespace TShock.Commands {
    /// <summary>
    /// Represents a service that manages commands. Provides command-related hooks and methods.
    /// </summary>
    public interface ICommandService : IService {
        /// <summary>
        /// Gets a read-only mapping from qualified command names to commands.
        /// </summary>
        IReadOnlyDictionary<string, ICommand> Commands { get; }

        /// <summary>
        /// Gets a read-only mapping from types to parsers.
        /// </summary>
        IReadOnlyDictionary<Type, IArgumentParser> Parsers { get; }

        /// <summary>
        /// Gets or sets the event handlers that occur when registering a command. This event can be canceled.
        /// </summary>
        EventHandlerCollection<CommandRegisterEventArgs>? CommandRegister { get; set; }

        /// <summary>
        /// Gets or sets the event handlers that occur when executing a command. This event can be canceled.
        /// </summary>
        EventHandlerCollection<CommandExecuteEventArgs>? CommandExecute { get; set; }

        /// <summary>
        /// Gets or sets the event handlers that occur when unregistering a command. This event can be canceled.
        /// </summary>
        EventHandlerCollection<CommandUnregisterEventArgs>? CommandUnregister { get; set; }

        /// <summary>
        /// Registers and returns the commands defined with <paramref name="handlerObject"/>'s command handlers. Command
        /// handlers are specified using the <see cref="CommandHandlerAttribute"/> attribute.
        /// </summary>
        /// <param name="handlerObject">The object.</param>
        /// <returns>The resulting commands.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="handlerObject"/> is ().
        /// </exception>
        IReadOnlyCollection<ICommand> RegisterCommands(object handlerObject);

        /// <summary>
        /// Registers <paramref name="parser"/> as the parser for <typeparamref name="TParse"/>. This will allow
        /// <typeparamref name="TParse"/> to be parsed in command handlers.
        /// </summary>
        /// <typeparam name="TParse">The parse type.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parser"/> is ().</exception>
        void RegisterParser<TParse>(IArgumentParser<TParse> parser);

        /// <summary>
        /// Finds and returns a command with <paramref name="input"/>. A command name (possibly qualified) will be
        /// extracted and tested from <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>The command.</returns>
        /// <exception cref="CommandParseException">The command does not exist or is ambiguous.</exception>
        ICommand FindCommand(ref ReadOnlySpan<char> input);

        /// <summary>
        /// Unregisters <paramref name="command"/> and returns a value indicating success.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="command"/> was unregistered; otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is ().</exception>
        bool UnregisterCommand(ICommand command);
    }
}
