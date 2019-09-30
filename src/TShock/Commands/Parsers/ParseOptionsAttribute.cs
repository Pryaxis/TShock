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
using JetBrains.Annotations;

namespace TShock.Commands.Parsers {
    /// <summary>
    /// An attribute that can be applied to a parameter to specify options for parsing that specific parameter.
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
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> is <c>null</c>.</exception>
        public ParseOptionsAttribute([ValueProvider("TShock.Commands.Parsers.ParseOptions")]
                                     params string[] options) {
            if (options is null) throw new ArgumentNullException(nameof(options));

            var optionsSet = new HashSet<string>();
            optionsSet.UnionWith(options);
            Options = optionsSet;
        }
    }
}
