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
using TShock.Properties;

namespace TShock.Commands.Parsers {
    internal sealed class Int32Parser : IArgumentParser<int> {
        public int Parse(ReadOnlySpan<char> input, out ReadOnlySpan<char> nextInput) {
            // Scan until we find some non-whitespace character.
            var start = 0;
            while (start < input.Length) {
                if (!char.IsWhiteSpace(input[start])) break;

                ++start;
            }

            // Now scan until we find some whitespace character.
            var end = start;
            while (end < input.Length) {
                if (char.IsWhiteSpace(input[end])) break;

                ++end;
            }
            
            var parse = input[start..end];
            nextInput = input[end..];

            // Calling Parse here instead of TryParse allows us to give better error messages.
            try {
                return int.Parse(parse);
            } catch (FormatException ex) {
                throw new ParseException(string.Format(Resources.Int32Parser_InvalidInteger, parse.ToString()), ex);
            } catch (OverflowException ex) {
                throw new ParseException(string.Format(Resources.Int32Parser_IntegerOutOfRange, parse.ToString()), ex);
            }
        }
    }
}
