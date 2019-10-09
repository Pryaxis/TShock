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
using System.Reflection;

namespace TShock.Commands {
    /// <summary>
    /// Represents a command. Commands can be executed by command senders, and provide bits of functionality.
    /// </summary>
    public interface ICommand {
        /// <summary>
        /// Gets the command's qualified name. This includes the namespace: e.g., "tshock:kick".
        /// </summary>
        string QualifiedName { get; }

        /// <summary>
        /// Gets the command's help text. If <see langword="null"/>, then no help text exists. This will show up in the
        /// /help command.
        /// </summary>
        string? HelpText { get; }

        /// <summary>
        /// Gets the command's usage text. If <see langword="null"/>, then no usage text exists. This will show up in the
        /// /help command and when invalid syntax is used.
        /// </summary>
        string? UsageText { get; }

        /// <summary>
        /// Gets the object associated with the command's handler.
        /// </summary>
        object HandlerObject { get; }

        /// <summary>
        /// Gets the command's handler.
        /// </summary>
        MethodBase Handler { get; }

        /// <summary>
        /// Invokes the command as a <paramref name="sender"/> with the <paramref name="input"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="input">The input. This does not include the command's name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="sender"/> is <see langword="null"/>.</exception>
        /// <exception cref="CommandExecuteException">The command could not be executed.</exception>
        /// <exception cref="CommandParseException">The command input could not be parsed.</exception>
        void Invoke(ICommandSender sender, ReadOnlySpan<char> input);
    }
}
