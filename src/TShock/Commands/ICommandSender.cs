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
        /// <value>The sender's name.</value>
        string Name { get; }

        /// <summary>
        /// Gets the sender's log.
        /// </summary>
        /// <value>The sender's log.</value>
        /// <remarks>
        /// This property's value can be used to log information to the sender. For example, you can pass debugging or
        /// error messages and they will be formatted appropriately.
        /// </remarks>
        ILogger Log { get; }

        /// <summary>
        /// Gets the sender's player. If <see langword="null"/>, then there is no associated player.
        /// </summary>
        /// <value>The sender's player.</value>
        /// <remarks>
        /// This property's value can be used to modify the command sender's state. For example, you can heal or kill
        /// the player.
        /// </remarks>
        IPlayer? Player { get; }

        /// <summary>
        /// Sends a <paramref name="message"/> to the sender.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        void SendMessage(string message);

        /// <summary>
        /// Sends a <paramref name="message"/> to the sender with the given <paramref name="color"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="color">The color.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/>.</exception>
        void SendMessage(string message, Color color);
    }

    /// <summary>
    /// Provides extensions for the <see cref="ICommandSender"/> interface.
    /// </summary>
    public static class CommandSenderExtensions {
        /// <summary>
        /// Sends an error <paramref name="message"/> to the <paramref name="sender"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The error message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> or <paramref name="message"/> are <see langword="null"/>.
        /// </exception>
        public static void SendErrorMessage(this ICommandSender sender, string message) {
            if (sender is null) {
                throw new ArgumentNullException(nameof(sender));
            }

            if (message is null) {
                throw new ArgumentNullException(nameof(message));
            }

            sender.SendMessage(message, new Color(0xcc, 0x44, 0x44));
        }

        /// <summary>
        /// Sends an informational <paramref name="message"/> to the <paramref name="sender"/>.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The informational message.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sender"/> or <paramref name="message"/> are <see langword="null"/>.
        /// </exception>
        public static void SendInfoMessage(this ICommandSender sender, string message) {
            if (sender is null) {
                throw new ArgumentNullException(nameof(sender));
            }

            if (message is null) {
                throw new ArgumentNullException(nameof(message));
            }

            sender.SendMessage(message, new Color(0xff, 0xf0, 0x14));
        }
    }
}
