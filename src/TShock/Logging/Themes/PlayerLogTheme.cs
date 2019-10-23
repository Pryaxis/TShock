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
using System.Diagnostics;
using Microsoft.Xna.Framework;
using TShock.Utils.Extensions;

namespace TShock.Logging.Themes {
    /// <summary>
    /// Represents a player log theme.
    /// </summary>
    public sealed class PlayerLogTheme {
        private readonly IDictionary<PlayerLogThemeStyle, Color> _styleToColor;

        internal PlayerLogTheme(IDictionary<PlayerLogThemeStyle, Color> styleToColor) {
            Debug.Assert(styleToColor != null, "style to color map should not be null");

            _styleToColor = styleToColor;
        }

        internal string Stylize(string text, PlayerLogThemeStyle style) {
            Debug.Assert(text != null, "Text should not be null");

            return _styleToColor.TryGetValue(style, out var color) ? text.WithColor(color) : text;
        }
    }
}
