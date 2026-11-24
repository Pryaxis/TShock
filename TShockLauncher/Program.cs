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
using System.Runtime.InteropServices;
using TShockPluginManager;

// On occasion, users have been seen extracting TShock into their client installation directory -- this is of course incorrect, and is known
// to cause issues. Let's attempt to catch this before anything happens (specifically, before Terraria assemblies are resolved) and prevent
// TShock from launching.
if (File.Exists("TerrariaServer.exe"))
{
	Console.ForegroundColor = ConsoleColor.Red;
	Console.Error.WriteLine("A \"TerrariaServer.exe\" file has been found in the current working directory.");
	Console.Error.WriteLine(
		"This indicates either installation into a Terraria client directory, or installation into a legacy (TShock 4 or older) TShock directory.");
	Console.Error.WriteLine(
		"TShock is never to be installed inside a Terraria client directory. You should instead extract your TShock installation into it's own directory.");
	Console.Error.WriteLine(
		"If you are updating a legacy TShock installation, please follow the following documentation to update: https://ikebukuro.tshock.co/#/?id=upgrading-from-tshock-4");
	Console.Error.WriteLine("The launcher will now exit.");
	Console.ResetColor();
	return 1;
}

Dictionary<string, Assembly> _cache = new Dictionary<string, Assembly>();

System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += Default_Resolving;
System.Runtime.Loader.AssemblyLoadContext.Default.ResolvingUnmanagedDll += Default_ResolvingUnmanagedDll;

return await StartAsync();

/// <summary>
/// Resolves native libraries (such as GNS_Wrapper) from ./runtimes/&lt;rid&gt;/native/
/// </summary>
IntPtr Default_ResolvingUnmanagedDll(Assembly assembly, string unmanagedDllName)
{
	if (!unmanagedDllName.Equals("GNS_Wrapper", StringComparison.OrdinalIgnoreCase)
		&& !unmanagedDllName.Equals("GNS_Wrapper.dll", StringComparison.OrdinalIgnoreCase)
		&& !unmanagedDllName.Equals("libGNS_Wrapper.so", StringComparison.OrdinalIgnoreCase))
	{
		return IntPtr.Zero;
	}

	if (OperatingSystem.IsWindows())
	{
		var nativeDir = Path.Combine(AppContext.BaseDirectory, "runtimes", "win-x64", "native");
		var dllPath = Path.Combine(nativeDir, "GNS_Wrapper.dll");
		if (File.Exists(dllPath))
			return NativeLibrary.Load(dllPath);
	}
	else if (OperatingSystem.IsLinux())
	{
		var nativeDir = Path.Combine(AppContext.BaseDirectory, "runtimes", "linux-x64", "native");
		var protobufPath = Path.Combine(nativeDir, "libprotobuf.so.3.15.3.0");
		if (File.Exists(protobufPath))
			NativeLibrary.TryLoad(protobufPath, out _);

		var soPath = Path.Combine(nativeDir, "libGNS_Wrapper.so");
		if (File.Exists(soPath))
			return NativeLibrary.Load(soPath);
	}

	return IntPtr.Zero;
}

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
async Task<int> StartAsync()
{
	if (args.Length > 0 && args[0].ToLower() == "plugins")
	{
		var items = args.ToList();
		items.RemoveAt(0);
		await TShockPluginManager.NugetCLI.Main(items);
		return 0;
	}

	TerrariaApi.Server.Program.Main(args);
	return 0;
}
