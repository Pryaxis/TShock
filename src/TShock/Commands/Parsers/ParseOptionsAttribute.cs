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
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TShock.Commands.Parsers {
    /// <summary>
    /// Specifies the options with which to parse a parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ParseOptionsAttribute : Attribute {
        /// <summary>
        /// Gets the options.
        /// </summary>
        public ISet<string> Options { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseOptionsAttribute"/> class with the specified options.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="otherOptions">The other options.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="otherOptions"/> contains a <see langword="null"/> element.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="option"/> or <paramref name="otherOptions"/> are <see langword="null"/>.
        /// </exception>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "strings are not user-facing")]
        public ParseOptionsAttribute(string option, params string[] otherOptions) {
            if (option is null) {
                throw new ArgumentNullException(nameof(option));
            }

            if (otherOptions is null) {
                throw new ArgumentNullException(nameof(otherOptions));
            }

            if (otherOptions.Any(o => o == null)) {
                throw new ArgumentException("Array contains null element.", nameof(otherOptions));
            }

            var optionsSet = new HashSet<string> { option };
            optionsSet.UnionWith(otherOptions);
            Options = optionsSet;
        }
    }
}
