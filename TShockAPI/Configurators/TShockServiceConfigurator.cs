using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TerrariaApi.Configurator;
using Microsoft.Extensions.Configuration;
using TShockAPI.Configuration;

namespace TShockAPI.Configurators;

/// <summary>
/// Configurator for TShock services.
/// </summary>
public sealed class TShockServiceConfigurator : ServiceConfigurator
{
	/// <inheritdoc/>
	public override void Configure(HostBuilderContext hostContext, IServiceCollection services, string[] args)
	{
		// Enable DI injection of config sections
		IConfiguration configuration = hostContext.Configuration;
		services.Configure<ServerSettings>(configuration.GetSection(nameof(ServerSettings)));
		services.Configure<SaveSettings>(configuration.GetSection(nameof(SaveSettings)));
		services.Configure<GameSettings>(configuration.GetSection(nameof(GameSettings)));
		services.Configure<ProtectionSettings>(configuration.GetSection(nameof(ProtectionSettings)));
		services.Configure<GroupSettings>(configuration.GetSection(nameof(GroupSettings)));
		services.Configure<AuthenticationSettings>(configuration.GetSection(nameof(AuthenticationSettings)));
		services.Configure<PunishmentSettings>(configuration.GetSection(nameof(PunishmentSettings)));
		services.Configure<AntiCheatSettings>(configuration.GetSection(nameof(AntiCheatSettings)));


	}
}
