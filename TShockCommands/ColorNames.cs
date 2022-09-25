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
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;

/**
 *  Note:
 *  To avoid reinventing the wheel, this file has been taken from the following link:
 *  https://github.com/ZakFahey/easy-commands-tshock/blob/master/EasyCommandsTShock/EasyCommandsTShock/ColorNames.cs
 */

namespace TShockCommands;

/// <summary>
/// Utility class for the Color parsing rule
/// </summary>
static class ColorNames
{
	private static Dictionary<string, Color> colors = new Dictionary<string, Color>();

	static ColorNames()
	{
		foreach (PropertyInfo color in typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public))
		{
			if (color.DeclaringType != typeof(Color)) continue;
			colors[color.Name] = (Color)color.GetValue(null)!;
		}
	}

	public static bool GetColorFromName(string name, out Color color)
	{
		return colors.TryGetValue(name, out color);
	}
}
