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
using Microsoft.Xna.Framework;
using Xunit;

namespace TShock.Logging.Themes {
    public class PlayerLogThemeTests {
        [Fact]
        public void Stylize() {
            var theme = new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color> {
                [PlayerLogThemeStyle.Invalid] = new Color(0x65, 0x43, 0x21)
            });

            theme.Stylize("test", PlayerLogThemeStyle.Invalid).Should().Be("[c/654321:test]");
        }

        [Fact]
        public void Stylize_MissingStyle() {
            var theme = new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color>());

            theme.Stylize("test", PlayerLogThemeStyle.Text).Should().Be("test");
        }
    }
}
