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

namespace TShockCommands;

/// <summary>
/// Defines the command args for a given command.
/// </summary>
/// <remarks>These can be requested via DI using the scope created by the custom command repository</remarks>
[Obsolete("This API is not yet integrated in third party libraries")]
public class CommandArgs
{
	/// <summary>
	/// The raw command entered by the user
	/// </summary>
	public string? RawCommand { get; set; }

	/// <summary>
	/// Whether the user requested this command to be silent.
	/// </summary>
	public bool Silent { get; set; }
}
