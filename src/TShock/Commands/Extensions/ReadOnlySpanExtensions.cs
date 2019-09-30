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
using System.Diagnostics;

namespace TShock.Commands.Extensions {
    internal static class ReadOnlySpanExtensions {
        public static int ScanFor(this ReadOnlySpan<char> input, Func<char, bool> predicate, int start = 0) {
            Debug.Assert(predicate != null, "predicate != null");

            while (start < input.Length) {
                if (predicate(input[start])) break;

                ++start;
            }

            return start;
        }
    }
}
