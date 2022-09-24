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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TerrariaApi.Configurator;
using TShockAPI.Modules;

namespace TShockAPI.Configurators;

/// <summary>
/// Configures <see cref="Module"/> instances from the across the TShockAPI assembly
/// </summary>
public class ModuleConfigurator : ServiceConfigurator
{
	/// <summary>
	/// Registers all non abstract classes that are assignable to <see cref="Module"/>.
	/// </summary>
	/// <inheritdoc/>
	public override void Configure(HostBuilderContext hostContext, IServiceCollection services, string[] args)
	{
		var modules = typeof(TShock).Assembly.GetExportedTypes()
			.Where(x => typeof(Module).IsAssignableFrom(x)
				&& !x.IsAbstract
			)
		;

		foreach (var module in modules)
			services
				// allow each type to be injected directly
				.AddSingleton(module)
				// or queried as a collection, with the same unique instance.
				.AddSingleton(typeof(Module), implFactory => implFactory.GetRequiredService(module));
	}
}
