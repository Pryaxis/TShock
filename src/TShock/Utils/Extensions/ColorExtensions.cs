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

using Microsoft.Xna.Framework;

namespace TShock.Utils.Extensions {
    /// <summary>
    /// Provides extensions for the <see cref="Color"/> structure.
    /// </summary>
    public static class ColorExtensions {
        /// <summary>
        /// Converts the <paramref name="color"/> to a hex string representation.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The hex string representation.</returns>
        public static string ToHexString(this Color color) => $"{color.R:x2}{color.G:x2}{color.B:x2}";
    }
}
