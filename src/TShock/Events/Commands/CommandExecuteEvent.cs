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
using Destructurama.Attributed;
using Orion.Events;
using Orion.Utils;
using TShock.Commands;

namespace TShock.Events.Commands {
    /// <summary>
    /// An event that occurs when a command executes. This event can be canceled and modified.
    /// </summary>
    [Event("command-execute")]
    public sealed class CommandExecuteEvent : CommandEvent, ICancelable, IDirtiable {
        private string _input;

        /// <inheritdoc/>
        [NotLogged]
        public string? CancellationReason { get; set; }

        /// <inheritdoc/>
        [NotLogged]
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Gets the command sender.
        /// </summary>
        public ICommandSender Sender { get; }

        /// <summary>
        /// Gets or sets the input. This does not include the command's name.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public string Input {
            get => _input;
            set => _input = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandExecuteEvent"/> class with the specified command,
        /// command sender, and input.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="sender">The command sender.</param>
        /// <param name="input">The input. This does not include the command's name.</param>
        public CommandExecuteEvent(ICommand command, ICommandSender sender, string input) : base(command) {
            Sender = sender ?? throw new ArgumentNullException(nameof(sender));
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        /// <inheritdoc/>
        public void Clean() => IsDirty = false;
    }
}
