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
using Microsoft.Xna.Framework;

namespace TShock.Utils.Extensions {
    /// <summary>
    /// Provides extensions for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions {
        /// <summary>
        /// Returns a string with the given <paramref name="color"/>.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="color">The color.</param>
        /// <returns>A string with the given <paramref name="color"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="str"/> is <see langword="null"/>.</exception>
        public static string WithColor(this string str, Color color) {
            if (str is null) {
                throw new ArgumentNullException(nameof(str));
            }

            return $"[c/{color.ToHexString()}:{str}]";
        }
    }
}
