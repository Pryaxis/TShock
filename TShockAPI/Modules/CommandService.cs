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

namespace TShockAPI.Modules;

/// <summary>
/// Defines the bare minimum TShock needs to know about a command
/// </summary>
public interface ICommand
{
	/// <summary>
	/// All names the command can be executed by
	/// </summary>
	IEnumerable<string> Names { get; }
}

/// <summary>
/// Defines a command service used by tshock
/// </summary>
public interface ICommandService : IDisposable
{
	/// <summary>
	/// Called when the service is ready to start
	/// </summary>
	void Start();

	/// <summary>
	/// The command specifier, defaults to "/"
	/// </summary>
	string Specifier { get; }

	/// <summary>
	/// The silent command specifier, defaults to "."
	/// </summary>
	string SilentSpecifier { get; }

	/// <summary>
	/// Handles players command
	/// </summary>
	/// <param name="player">The player/user</param>
	/// <param name="text">The command</param>
	/// <returns>True when the command was handled, false otherwise</returns>
	bool Handle(TSPlayer player, string text);

	/// <summary>
	/// Returns command created by the service
	/// </summary>
	/// <param name="permission">Optional permission filter</param>
	/// <returns>Read only list of commands</returns>
	IEnumerable<ICommand> GetCommands(string? permission = null);
}
