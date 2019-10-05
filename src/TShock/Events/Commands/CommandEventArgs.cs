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
using Orion.Events;
using TShock.Commands;

namespace TShock.Events.Commands {
    /// <summary>
    /// Provides data for command-related events.
    /// </summary>
    public abstract class CommandEventArgs : EventArgs, ICancelable {
        private ICommand _command;

        /// <inheritdoc />
        public string? CancellationReason { get; set; }

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        public ICommand Command {
            get => _command;
            set => _command = value ?? throw new ArgumentNullException(nameof(value));
        }

        private protected CommandEventArgs(ICommand command) {
            _command = command ?? throw new ArgumentNullException(nameof(command));
        }
    }
}
