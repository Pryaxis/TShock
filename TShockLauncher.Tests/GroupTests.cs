using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Terraria;
using Terraria.Localization;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Modules;
using TShockCommands;

namespace TShockLauncher.Tests;

public class GroupTests
{
	[SetUp]
	public static void SetupTShock()
	{
		LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture);
		Lang.InitializeLegacyLocalization();

		IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
			.ConfigureServices(svcs => svcs
				.AddSingleton<ServiceLoader>()
				.AddSingleton<HookService>()
				.AddSingleton<TShock>()
				.AddSingleton<ICommandService, EasyCommandService>()
			);

		IHost host = hostBuilder.Build();

		var ts = host.Services.GetRequiredService<TShock>();
		ts.Start();
	}

	/// <summary>
	/// This tests to ensure the group commands work.
	/// </summary>
	/// <remarks>Due to the switch to Microsoft.Data.Sqlite, nulls have to be replaced with DBNull for the query to complete</remarks>
	[TestCase]
	public void TestPermissions()
	{
		var groups = TShock.Groups = new GroupManager(TShock.DB);

		if (!groups.GroupExists("test"))
			groups.AddGroup("test", null, "test", Group.defaultChatColor);

		groups.AddPermissions("test", new() { "abc" });

		var hasperm = groups.GetGroupByName("test").Permissions.Contains("abc");
		Assert.IsTrue(hasperm);

		groups.DeletePermissions("test", new() { "abc" });

		hasperm = groups.GetGroupByName("test").Permissions.Contains("abc");
		Assert.IsFalse(hasperm);

		groups.DeleteGroup("test");

		var g = groups.GetGroupByName("test");
		Assert.IsNull(g);
	}
}

