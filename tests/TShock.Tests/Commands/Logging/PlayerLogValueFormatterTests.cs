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

using System.Collections.Generic;
using FluentAssertions;
using Serilog.Events;
using Xunit;

namespace TShock.Commands.Logging {
    public class PlayerLogValueFormatterTests {
        [Fact]
        public void Format_Null() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new ScalarValue(null)).Should().MatchRegex(@"\[c/[a-fA-F0-9]{6}:null\]");
        }

        [Fact]
        public void Format_String() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new ScalarValue("test")).Should().MatchRegex(@"\[c/[a-fA-F0-9]{6}:""test""\]");
        }

        [Fact]
        public void Format_Bool() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new ScalarValue(true)).Should().MatchRegex(@"\[c/[a-fA-F0-9]{6}:True\]");
        }

        [Fact]
        public void Format_Char() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new ScalarValue('t')).Should().MatchRegex(@"\[c/[a-fA-F0-9]{6}:'t'\]");
        }

        [Fact]
        public void Format_Number() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new ScalarValue(-12345)).Should().MatchRegex(@"\[c/[a-fA-F0-9]{6}:-12345\]");
        }

        [Fact]
        public void Format_Sequence() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new SequenceValue(new[] { new ScalarValue(1), new ScalarValue(2) }))
                .Should().MatchRegex(@"\[\[c/[a-fA-F0-9]{6}:1\], \[c/[a-fA-F0-9]{6}:2\]\]");
        }

        [Fact]
        public void Format_Structure() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new StructureValue(new[] { new LogEventProperty("Test", new ScalarValue(1)) }))
                .Should().MatchRegex(@"{Test=\[c/[a-fA-F0-9]{6}:1\]}");
        }

        [Fact]
        public void Format_Structure_WithTypeTag() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new StructureValue(new[] { new LogEventProperty("Test", new ScalarValue(1)) }, "Type"))
                .Should().MatchRegex(@"\[c/[a-fA-F0-9]{6}:Type\] {Test=\[c/[a-fA-F0-9]{6}:1\]}");
        }

        [Fact]
        public void Format_Dictionary() {
            var formatter = new PlayerLogValueFormatter();

            formatter.Format(new DictionaryValue(new[] {
                new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue(1), new ScalarValue("test"))
            })).Should().MatchRegex(@"{\[\[c/[a-fA-F0-9]{6}:1\]\]=\[c/[a-fA-F0-9]{6}:""test""\]}");
        }
    }
}
