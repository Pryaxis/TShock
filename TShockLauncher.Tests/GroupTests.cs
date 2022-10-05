using NUnit.Framework;
using Terraria;
using Terraria.Localization;
using TShockAPI;
using TShockAPI.DB;

namespace TShockLauncher.Tests;

public class GroupTests
{
	/// <summary>
	/// This will be called automatically by nunit before other tests in this class.
	/// It serves to initialise the bare minimum variables needed for TShock to be testable without booting up an actual server.
	/// </summary>
	[SetUp]
	public static void SetupTShock()
	{
		Program.SavePath = ""; // 1.4.4.2 staticness introduced this where by default it is null, and any touch to Terraria.Main will use it and cause a crash.
		LanguageManager.Instance.SetLanguage(GameCulture.DefaultCulture); // TShockAPI.Localization will fail without ActiveCulture set
		Lang.InitializeLegacyLocalization(); // TShockAPI.Localization will fail without preparing NPC names etc

		var ts = new TShock(null); // prepares configs etc
		ts.Initialize(); // used to prepare tshocks own static variables, such as the TShock.DB instance
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

