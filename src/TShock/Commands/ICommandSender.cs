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
using Microsoft.Xna.Framework;
using Serilog;

namespace TShock.Commands {
    /// <summary>
    /// Represents a command sender. Provides the ability to communicate with the sender.
    /// </summary>
    public interface ICommandSender {
        /// <summary>
        /// Gets the sender's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the sender's log.
        /// </summary>
        ILogger Log { get; }

        /// <summary>
        /// Sends a message to the sender.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <c>null</c>.</exception>
        void SendMessage(string message);

        /// <summary>
        /// Sends a message to the sender with the given color.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="color">The color.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <c>null</c>.</exception>
        void SendMessage(string message, Color color);
    }
}
