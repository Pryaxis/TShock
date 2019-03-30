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
using System.Text;

namespace TShockAPI.Extensions
{
	public static class RandomExt
	{
		public static string NextString(this Random rand, int length)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				switch (rand.Next(0, 3))
				{
					case 0:
						sb.Append((char) rand.Next('a', 'z' + 1));
						break;
					case 1:
						sb.Append((char) rand.Next('A', 'Z' + 1));
						break;
					case 2:
						sb.Append((char) rand.Next('0', '9' + 1));
						break;
				}
			}
			return sb.ToString();
		}
	}
}