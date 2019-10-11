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
using System.Globalization;
using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Serilog.Events;
using TShock.Logging.Themes;
using Xunit;

namespace TShock.Logging {
    public class PlayerLogValueVisitorTests {
        private readonly PlayerLogTheme _theme =
            new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color> {
                [PlayerLogThemeStyle.Null] = new Color(0x00, 0x00, 0x00),
                [PlayerLogThemeStyle.String] = new Color(0x00, 0x80, 0x00),
                [PlayerLogThemeStyle.Boolean] = new Color(0x00, 0x00, 0x80),
                [PlayerLogThemeStyle.Character] = new Color(0x00, 0x00, 0xff),
                [PlayerLogThemeStyle.Number] = new Color(0xff, 0xff, 0x00),
                [PlayerLogThemeStyle.Scalar] = new Color(0x00, 0x00, 0x01),
                [PlayerLogThemeStyle.Separator] = new Color(0xc0, 0xc0, 0xc0),
                [PlayerLogThemeStyle.Identifier] = new Color(0xff, 0x00, 0x00),
                [PlayerLogThemeStyle.Type] = new Color(0xff, 0x00, 0xff)
            });

        [Fact]
        public void Format_Null() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new ScalarValue(null), writer);
            
            writer.ToString().Should().Be("[c/000000:null]");
        }

        [Fact]
        public void Format_String() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new ScalarValue("test"), writer);
            
            writer.ToString().Should().Be("[c/008000:test]");
        }

        [Fact]
        public void Format_Bool() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new ScalarValue(true), writer);
            
            writer.ToString().Should().Be("[c/000080:True]");
        }

        [Fact]
        public void Format_Char() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new ScalarValue('t'), writer);
            
            writer.ToString().Should().Be("[c/0000ff:t]");
        }

        [Fact]
        public void Format_Number() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new ScalarValue(-12345), writer);
            
            writer.ToString().Should().Be("[c/ffff00:-12345]");
        }

        [Fact]
        public void Format_Scalar() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new ScalarValue(new StringBuilder("test")), writer);
            
            writer.ToString().Should().Be("[c/000001:test]");
        }

        [Fact]
        public void Format_Sequence() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new SequenceValue(new[] { new ScalarValue(1), new ScalarValue(2) }), writer);
            
            writer.ToString().Should().Be("[[c/ffff00:1][c/c0c0c0:, ][c/ffff00:2]]");
        }

        [Fact]
        public void Format_Sequence_NoSeparator() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new SequenceValue(new[] { new ScalarValue(1) }), writer);
            
            writer.ToString().Should().Be("[[c/ffff00:1]]");
        }

        [Fact]
        public void Format_Structure() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new StructureValue(new[] { new LogEventProperty("Test", new ScalarValue(1)) }), writer);
            
            writer.ToString().Should().Be("{[c/ff0000:Test][c/c0c0c0:=][c/ffff00:1]}");
        }

        [Fact]
        public void Format_Structure_WithSeparator() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new StructureValue(new[] {
                new LogEventProperty("Test", new ScalarValue(1)),
                new LogEventProperty("Test2", new ScalarValue(2))
            }), writer);
            
            writer.ToString().Should().Be(
                "{[c/ff0000:Test][c/c0c0c0:=][c/ffff00:1][c/c0c0c0:, ][c/ff0000:Test2][c/c0c0c0:=][c/ffff00:2]}");
        }

        [Fact]
        public void Format_Structure_WithTypeTag() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(
                new StructureValue(new[] { new LogEventProperty("Test", new ScalarValue(1)) }, "Type"),
                writer);
            
            writer.ToString().Should().Be("[c/ff00ff:Type]{[c/ff0000:Test][c/c0c0c0:=][c/ffff00:1]}");
        }

        [Fact]
        public void Format_Dictionary() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new DictionaryValue(new[] {
                new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue(1), new ScalarValue("test"))
            }), writer);
            
            writer.ToString().Should().Be("{[[c/ffff00:1]][c/c0c0c0:=][c/008000:test]}");
        }

        [Fact]
        public void Format_Dictionary_WithSeparator() {
            var visitor = new PlayerLogValueVisitor(_theme, CultureInfo.InvariantCulture);
            var writer = new StringWriter();

            visitor.Format(new DictionaryValue(new[] {
                new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue(1), new ScalarValue("test")),
                new KeyValuePair<ScalarValue, LogEventPropertyValue>(new ScalarValue(2), new ScalarValue("test2"))
            }), writer);
            
            writer.ToString().Should().Be(
                "{[[c/ffff00:1]][c/c0c0c0:=][c/008000:test][c/c0c0c0:, ][[c/ffff00:2]][c/c0c0c0:=][c/008000:test2]}");
        }
    }
}
