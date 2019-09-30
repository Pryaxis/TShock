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
        /// Gets the registered commands.
        /// </summary>
        IEnumerable<ICommand> RegisteredCommands { get; }

        /// <summary>
        /// Gets the registered parsers.
        /// </summary>
        IDictionary<Type, IArgumentParser> RegisteredParsers { get; }

        /// <summary>
        /// Gets or sets the event handlers that occur when registering a command.
        /// </summary>
        EventHandlerCollection<CommandRegisterEventArgs>? CommandRegister { get; set; }

        /// <summary>
        /// Gets or sets the event handlers that occur when executing a command.
        /// </summary>
        EventHandlerCollection<CommandExecuteEventArgs>? CommandExecute { get; set; }

        /// <summary>
        /// Gets or sets the event handlers that occur when unregistering a command.
        /// </summary>
        EventHandlerCollection<CommandUnregisterEventArgs>? CommandUnregister { get; set; }

        /// <summary>
        /// Registers and returns the commands defined with the given object's command handlers.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The resulting commands.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj"/> is <c>null</c>.</exception>
        IReadOnlyCollection<ICommand> RegisterCommands(object obj);

        /// <summary>
        /// Registers the given parser as the definitive parser for the parse type.
        /// </summary>
        /// <param name="parseType">The parse type.</param>
        /// <param name="parser">The parser.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="parseType"/> or <paramref name="parser"/> are <c>null</c>.
        /// </exception>
        void RegisterParser(Type parseType, IArgumentParser parser);

        /// <summary>
        /// Unregisters the given command and returns a value indicating success.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A value indicating whether the command was successfully unregistered.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <c>null</c>.</exception>
        bool UnregisterCommand(ICommand command);
    }
}
