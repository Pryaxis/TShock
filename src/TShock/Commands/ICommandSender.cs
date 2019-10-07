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

using Microsoft.Xna.Framework;
using Orion.Players;
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
        /// Gets the sender's player. If <see langword="null"/>, then there is no associated player.
        /// </summary>
        IPlayer? Player { get; }

        /// <summary>
        /// Sends a <paramref name="message"/> to the sender.
        /// </summary>
        /// <param name="message">The message.</param>
        void SendMessage(string message);

        /// <summary>
        /// Sends a <paramref name="message"/> to the sender with the given <paramref name="color"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="color">The color.</param>
        void SendMessage(string message, Color color);

        /// <summary>
        /// Sends an error <paramref name="message"/> to the sender.
        /// </summary>
        /// <param name="message">The error message.</param>
        void SendErrorMessage(string message) => SendMessage(message, new Color(0xcc, 0x44, 0x44));

        /// <summary>
        /// Sends an informational <paramref name="message"/> to the sender.
        /// </summary>
        /// <param name="message">The informational message.</param>
        void SendInfoMessage(string message) => SendMessage(message, new Color(0xff, 0xff, 0x44));
    }
}
