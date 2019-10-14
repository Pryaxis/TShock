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
using Microsoft.Xna.Framework;

namespace TShock.Logging.Themes {
    /// <summary>
    /// Provides <see cref="PlayerLogTheme"/> instances.
    /// </summary>
    public static class PlayerLogThemes {
        /// <summary>
        /// Gets a theme that uses Visual Studio's colors wherever possible.
        /// </summary>
        public static PlayerLogTheme VisualStudio { get; } =
            new PlayerLogTheme(new Dictionary<PlayerLogThemeStyle, Color> {
                [PlayerLogThemeStyle.Text] = new Color(0xdc, 0xdc, 0xdc),
                [PlayerLogThemeStyle.Separator] = new Color(0x9b, 0x9b, 0x9b),
                [PlayerLogThemeStyle.Null] = new Color(0x56, 0x9c, 0xd6),
                [PlayerLogThemeStyle.Boolean] = new Color(0x56, 0x9c, 0xd6),
                [PlayerLogThemeStyle.String] = new Color(0xd6, 0x9d, 0x85),
                [PlayerLogThemeStyle.Character] = new Color(0xd6, 0x9d, 0x85),
                [PlayerLogThemeStyle.Number] = new Color(0xb5, 0xce, 0xa8),
                [PlayerLogThemeStyle.Scalar] = new Color(0x86, 0xc6, 0x91),
                [PlayerLogThemeStyle.Identifier] = new Color(0x9c, 0xdc, 0xfe),
                [PlayerLogThemeStyle.Type] = new Color(0x4e, 0xc9, 0xb0),

                // The below don't really have any analogs, so I made some colors up.
                [PlayerLogThemeStyle.Timestamp] = new Color(0x86, 0xc6, 0x91),
                [PlayerLogThemeStyle.Exception] = new Color(0xcc, 0x44, 0x44),
                [PlayerLogThemeStyle.VerboseLevel] = new Color(0xcc, 0xcc, 0xcc),
                [PlayerLogThemeStyle.DebugLevel] = new Color(0xff, 0xff, 0x44),
                [PlayerLogThemeStyle.WarningLevel] = new Color(0xff, 0x88, 0x44),
                [PlayerLogThemeStyle.ErrorLevel] = new Color(0xcc, 0x44, 0x44),
                [PlayerLogThemeStyle.FatalLevel] = new Color(0xff, 0x00, 0x00),
                [PlayerLogThemeStyle.Invalid] = new Color(0xcc, 0x44, 0x44),
            });
    }
}
