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

/**
 *  Note:
 *  To avoid reinventing the wheel, this file has been taken from the following link:
 *  https://github.com/ZakFahey/easy-commands-tshock/blob/master/EasyCommandsTShock/EasyCommandsTShock/Attributes.cs
 */

using EasyCommands;

namespace TShockCommands.Annotations;

/// <summary>
/// The documentation for a command
/// </summary>
public class HelpText : CustomAttribute
{
	/// <summary>
	/// Gets a value of the current documentation for this command
	/// </summary>
	public string Documentation { get; private set; }

	/// <summary>
	/// Allows documentation to be defined for a command
	/// </summary>
	/// <param name="documentation">The users documentation</param>
	public HelpText(string documentation)
	{
		Documentation = documentation;
	}
}
