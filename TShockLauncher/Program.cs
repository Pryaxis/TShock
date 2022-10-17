/*
TShock, a server mod for Terraria
Copyright (C) 2021-2022 Pryaxis & TShock Contributors

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

/*
 * The purpose of this project is to be the launcher of the TSAPI server.
 * We use this project:
 *	- to copy/move around TShockAPI.dll (the TShock plugin to TSAPI)
 *	- to publish TShock releases.
 *	- move dependencies to a ./bin folder
 * 
 * The assembly name of this launcher (TShock.exe) was decided on by a community poll.
 */

using System.Reflection;

Dictionary<string, Assembly> _cache = new Dictionary<string, Assembly>();

System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += Default_Resolving;

Start();

/// <summary>
/// Resolves a module from the ./bin folder, either with a .dll by preference or .exe
/// </summary>
Assembly? Default_Resolving(System.Runtime.Loader.AssemblyLoadContext arg1, AssemblyName arg2)
{
	if (arg2?.Name is null) return null;
	if (_cache.TryGetValue(arg2.Name, out Assembly? asm) && asm is not null) return asm;

	var loc = Path.Combine(AppContext.BaseDirectory, "bin", arg2.Name + ".dll");
	if (File.Exists(loc))
		asm = arg1.LoadFromAssemblyPath(loc);

	loc = Path.ChangeExtension(loc, ".exe");
	if (File.Exists(loc))
		asm = arg1.LoadFromAssemblyPath(loc);

	if(asm is not null)
		_cache[arg2.Name] = asm;

	return asm;
}

/// <summary>
/// Initiates the TSAPI server.
/// </summary>
/// <remarks>This method exists so that the resolver can attach before TSAPI needs its dependencies.</remarks>
void Start()
{
	TerrariaApi.Server.Program.Main(args);
}
