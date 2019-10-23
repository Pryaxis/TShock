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

namespace TShock.Utils.Extensions {
    /// <summary>
    /// Provides extensions for the <see cref="ReadOnlySpan{T}"/> structure.
    /// </summary>
    public static class ReadOnlySpanExtensions {
        /// <summary>
        /// Searches for <paramref name="value"/> and returns the first index of its occurrence, or the length of the
        /// span if it is not found.
        /// </summary>
        /// <typeparam name="T">The type of span.</typeparam>
        /// <param name="span">The span.</param>
        /// <param name="value">The value.</param>
        /// <returns>The index of the first occurrence, or the length of the span if it is not found.</returns>
        public static int IndexOfOrEnd<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T> {
            var index = span.IndexOf(value);
            return index >= 0 ? index : span.Length;
        }
    }
}
