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
using TShock.Commands;

namespace TShock.Events.Commands {
    /// <summary>
    /// An event that occurs when a command is registered. This event can be canceled.
    /// </summary>
    [Event("command-register")]
    public sealed class CommandRegisterEvent : CommandEvent, ICancelable {
        /// <inheritdoc/>
        [NotLogged]
        public string? CancellationReason { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandRegisterEvent"/> class with the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <exception cref="ArgumentNullException"><paramref name="command"/> is <see langword="null"/>.</exception>
        public CommandRegisterEvent(ICommand command) : base(command) { }
    }
}
