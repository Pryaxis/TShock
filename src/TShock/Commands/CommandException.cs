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
using System.Diagnostics.CodeAnalysis;

namespace TShock.Commands {
    /// <summary>
    /// The exception thrown when a command could not be executed.
    /// </summary>
    [Serializable, ExcludeFromCodeCoverage]
    public class CommandException : Exception {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandException"/> class.
        /// </summary>
        public CommandException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandException"/> class with the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public CommandException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandException"/> class with the specified message
        /// and inner exception.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CommandException(string message, Exception innerException) : base(message, innerException) { }
    }
}
