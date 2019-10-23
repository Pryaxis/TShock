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
using System.Globalization;
using TShock.Commands.Exceptions;
using TShock.Properties;
using TShock.Utils.Extensions;

namespace TShock.Commands.Parsers {
    /// <summary>
    /// Parses a Double.
    /// </summary>
    public sealed class DoubleParser : IArgumentParser<double> {
        /// <inheritdoc/>
        public double Parse(ref ReadOnlySpan<char> input, ISet<Attribute> attributes) {
            var end = input.IndexOfOrEnd(' ');
            var parse = input[..end];

            // Calling Parse here instead of TryParse allows us to give better error messages.
            try {
                var value = double.Parse(parse, NumberStyles.Float, CultureInfo.InvariantCulture);
                input = input[end..];
                return value;
            } catch (FormatException ex) {
                throw new CommandParseException(
                    string.Format(CultureInfo.InvariantCulture, Resources.DoubleParser_InvalidDouble,
                        parse.ToString()), ex);
            }
        }
    }
}
