/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace TShockAPI
{
	public static class LinqExt
	{
		public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (action == null) throw new ArgumentNullException("action");

			foreach (T item in source)
				action(item);
		}

		/// <summary>
		/// Attempts to retrieve the value at the given index from the enumerable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="enumerable"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool TryGetValue<T>(this IEnumerable<T> enumerable, int index, out T value)
		{
			if (index < enumerable.Count())
			{
				value = enumerable.ElementAt(index);
				return true;
			}

			value = default;
			return false;
		}
	}
}
