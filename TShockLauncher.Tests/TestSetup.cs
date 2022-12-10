using NUnit.Framework;
using Terraria;
using Terraria.Initializers;
using Terraria.Localization;
using TShockAPI;

namespace TShockLauncher.Tests;

[SetUpFixture]
public class TestSetup
{
	/// <summary>
	/// This will be called automatically by NUnit before the first test.
	/// It serves to initialise the bare minimum variables needed for TShock to be testable without booting up an actual server.
	/// </summary>
	[OneTimeSetUp]
	public static void SetupTShock()
	{
		ChatInitializer.Load();

		Program.SavePath = ""; // 1.4.4.2 staticness introduced this where by default it is null, and any touch to Terraria.Main will use it and cause a crash.
		LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture); // TShockAPI.Localization will fail without ActiveCulture set
		Lang.InitializeLegacyLocalization(); // TShockAPI.Localization will fail without preparing NPC names etc

		var ts = new TShock(null); // prepares configs etc
		ts.Initialize(); // used to prepare tshocks own static variables, such as the TShock.DB instance
	}
}
