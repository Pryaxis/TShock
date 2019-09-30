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
using TShock.Commands;

namespace TShock.Events.Commands {
    /// <summary>
    /// Provides data for the <see cref="ICommandService.CommandExecute"/> event.
    /// </summary>
    public sealed class CommandExecuteEventArgs : CommandEventArgs {
        private string _input;

        /// <summary>
        /// Gets the command sender.
        /// </summary>
        public ICommandSender Sender { get; }

        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        public string Input {
            get => _input;
            set => _input = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecuteEventArgs"/> class with the specified command,
        /// command sender, and input.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="sender">The command sender.</param>
        /// <param name="input">The input. This does not include the command's name or sub-names.</param>
        public CommandExecuteEventArgs(ICommand command, ICommandSender sender, string input) : base(command) {
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }
    }
}
