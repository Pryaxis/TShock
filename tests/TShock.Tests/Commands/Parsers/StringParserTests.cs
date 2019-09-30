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
using FluentAssertions;
using Xunit;

namespace TShock.Commands.Parsers {
    public class StringParserTests {
        [Theory]
        [InlineData("", "", "")]
        [InlineData("test", "test", "")]
        [InlineData("test abc", "test", " abc")]
        [InlineData("       test", "test", "")]
        [InlineData(@"""test""", "test", "")]
        [InlineData(@"""test abc   def""", "test abc   def", "")]
        [InlineData(@"""test abc"" def ghi", "test abc", " def ghi")]
        [InlineData(@"test\ abc", "test abc", "")]
        [InlineData(@"test\ abc def", "test abc", " def")]
        [InlineData(@"\\", @"\", "")]
        [InlineData(@"\""", @"""", "")]
        [InlineData(@"\t", "\t", "")]
        [InlineData(@"\n", "\n", "")]
        public void Parse_IsCorrect(string inputString, string expected, string expectedNextInput) {
            var parser = new StringParser();
            var input = inputString.AsSpan();

            parser.Parse(ref input).Should().Be(expected);

            input.ToString().Should().Be(expectedNextInput);
        }

        [Theory]
        [InlineData(@"\")]
        [InlineData(@"\a")]
        public void Parse_EscapeReachesEnd_ThrowsParseException(string inputString) {
            var parser = new StringParser();
            Func<string> func = () => {
                var input = inputString.AsSpan();
                return parser.Parse(ref input);
            };

            func.Should().Throw<ParseException>();
        }

        [Fact]
        public void Parse_ToEndOfInput_IsCorrect() {
            var parser = new StringParser();
            var input = @"blah blah ""test"" blah blah".AsSpan();

            parser.Parse(ref input, new HashSet<string> {ParseOptions.ToEndOfInput})
                  .Should().Be(@"blah blah ""test"" blah blah");

            input.ToString().Should().BeEmpty();
        }
    }
}
