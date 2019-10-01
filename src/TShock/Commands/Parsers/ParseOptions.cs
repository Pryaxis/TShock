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

namespace TShock.Commands.Parsers {
    /// <summary>
    /// Provides parse options. These are strings so that parse options can easily be added by consumers.
    /// </summary>
    public static class ParseOptions {
        /// <summary>
        /// An option which forces a string to be parsed to the end of the input.
        /// </summary>
        public const string ToEndOfInput = nameof(ToEndOfInput);

        /// <summary>
        /// An option which allows a parser to parse empty input.
        /// </summary>
        public const string AllowEmpty = nameof(AllowEmpty);
    }
}
