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
using FluentAssertions;
using Xunit;

namespace TShock.Commands.Parsing {
    public class Int32ParserTests {
        [Theory]
        [InlineData("1234", 1234, "")]
        [InlineData("+1234", 1234, "")]
        [InlineData("000", 0, "")]
        [InlineData("-1234", -1234, "")]
        [InlineData("123 test", 123, " test")]
        [InlineData("    123", 123, "")]
        public void Parse_IsCorrect(string input, int expected, string expectedNextInput) {
            var parser = new Int32Parser();

            parser.Parse(input, out var nextInput).Should().Be(expected);

            nextInput.ToString().Should().Be(expectedNextInput);
        }

        [Theory]
        [InlineData("2147483648")]
        [InlineData("-2147483649")]
        public void Parse_IntegerOutOfRange_ThrowsParseException(string input) {
            var parser = new Int32Parser();
            Func<int> func = () => parser.Parse(input, out _);

            func.Should().Throw<ParseException>().WithInnerException<OverflowException>();
        }
        
        [Theory]
        [InlineData("aaa")]
        [InlineData("123a")]
        [InlineData("1.0")]
        public void Parse_InvalidInteger_ThrowsParseException(string input) {
            var parser = new Int32Parser();
            Func<int> func = () => parser.Parse(input, out _);

            func.Should().Throw<ParseException>().WithInnerException<FormatException>();
        }
    }
}
