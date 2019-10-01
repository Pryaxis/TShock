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
using System.Reflection;
using TShock.Commands.Parsers;

namespace TShock.Commands {
    /// <summary>
    /// Represents a command.
    /// </summary>
    public interface ICommand {
        /// <summary>
        /// Gets the command's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the command's sub-names.
        /// </summary>
        IEnumerable<string> SubNames { get; }

        /// <summary>
        /// Gets the object associated with the command's handler.
        /// </summary>
        object HandlerObject { get; }

        /// <summary>
        /// Gets the command's handler.
        /// </summary>
        MethodBase Handler { get; }

        /// <summary>
        /// Invokes the command as the given sender with the specified input.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="input">The input. This does not include the command's name or sub-names.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> or <paramref name="input"/> are <c>null</c>.
        /// </exception>
        /// <exception cref="CommandException">The command could not be executed.</exception>
        /// <exception cref="CommandParseException">The command input could not be parsed.</exception>
        void Invoke(ICommandSender sender, string input);
    }
}
