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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TerrariaApi.Configurator;
using TShockAPI.Modules;

namespace TShockCommands;

public class CommandConfigurator : ServiceConfigurator
{
	/// <inheritdoc/>
	public override void Configure(HostBuilderContext hostContext, IServiceCollection services, string[] args)
	{
		services
			.AddSingleton<ICommandService, EasyCommandService>()
			.AddSingleton<EasyCommandService>()
			.AddScoped<CommandArgs>();

		services.Configure<CommandOptions>(hostContext.Configuration.GetSection(nameof(CommandOptions)));
	}
}
