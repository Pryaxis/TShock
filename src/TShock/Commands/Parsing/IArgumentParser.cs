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

namespace TShock.Commands.Parsing {
    /// <summary>
    /// Provides parsing support for a type.
    /// </summary>
    /// <typeparam name="TParse">The parse type.</typeparam>
    public interface IArgumentParser<out TParse> {
        /// <summary>
        /// Parses the given input and returns a corresponding instance of the parse type along with what the next
        /// input should be.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="nextInput">The next input.</param>
        /// <returns>A corresponding instance of the parse type.</returns>
        /// <exception cref="ParseException">The input could not be parsed properly.</exception>
        TParse Parse(ReadOnlySpan<char> input, out ReadOnlySpan<char> nextInput);
    }
}
