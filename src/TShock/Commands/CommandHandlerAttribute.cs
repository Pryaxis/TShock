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
using JetBrains.Annotations;

namespace TShock.Commands {
    /// <summary>
    /// Specifies that a method is a command handler. This can be applied multiple times to a method to provide
    /// aliasing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [MeansImplicitUse]
    public sealed class CommandHandlerAttribute : Attribute {
        /// <summary>
        /// Gets the command's qualified name. This includes the command's namespace: e.g., "tshock:kick".
        /// </summary>
        public string QualifiedCommandName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerAttribute"/> class with the specified qualified
        /// command name.
        /// </summary>
        /// <param name="qualifiedCommandName">
        /// The qualified command name. This includes the namespace: e.g., "tshock:kick".
        /// </param>
        /// <exception cref="ArgumentException">
        /// <paramref name="qualifiedCommandName"/> is missing the namespace or name, or contains a space.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="qualifiedCommandName"/> is ().
        /// </exception>
        public CommandHandlerAttribute(string qualifiedCommandName) {
            if (qualifiedCommandName is null) {
                throw new ArgumentNullException(nameof(qualifiedCommandName));
            }

            var colon = qualifiedCommandName.IndexOf(':');
            if (colon <= 0) {
                throw new ArgumentException("Qualified command name is missing the namespace.",
                    nameof(qualifiedCommandName));
            }

            if (colon >= qualifiedCommandName.Length - 1) {
                throw new ArgumentException("Qualified command name is missing the name.",
                    nameof(qualifiedCommandName));
            }

            if (qualifiedCommandName.IndexOf(' ') >= 0) {
                throw new ArgumentException("Qualified command name contains a space.",
                    nameof(qualifiedCommandName));
            }

            QualifiedCommandName = qualifiedCommandName;
        }
    }
}
