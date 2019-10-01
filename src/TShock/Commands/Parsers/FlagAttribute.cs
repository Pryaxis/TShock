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

namespace TShock.Commands.Parsers {
    /// <summary>
    /// An attribute that can be applied to a <c>bool</c> parameter to specify flag parsing for that specific parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FlagAttribute : Attribute {
        /// <summary>
        /// Gets the short form of the flag.
        /// </summary>
        public char ShortFlag { get; }

        /// <summary>
        /// Gets the long form of the flag. If <c>null</c>, then there is no long flag.
        /// </summary>
        public string? LongFlag { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlagAttribute"/> class with the specified short and long flags.
        /// </summary>
        /// <param name="shortFlag">The short flag.</param>
        /// <param name="longFlag">The long flag.</param>
        public FlagAttribute(char shortFlag, string? longFlag = null) {
            ShortFlag = shortFlag;
            LongFlag = longFlag;
        }
    }
}
