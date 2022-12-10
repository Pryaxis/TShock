using Microsoft.Xna.Framework;
using NUnit.Framework;
using Terraria.UI.Chat;

namespace TShockLauncher.Tests;

public class ChatTests
{
	/// <summary>
	/// Ensures that the <see cref="Terraria.GameContent.UI.Chat.AchievementTagHandler"/> does not cause exceptions when used on the server.
	/// </summary>
	///
	/// <remarks>The behaviour of TShock regarding the achievement tag handler changes depending on if TShock has
	/// a <see cref="Terraria.Main"/> instance or not. Therefore, we do not check the correctness of the parsed message, but only if it
	/// throws an exception.
	/// </remarks>
	[TestCase]
	public void TestChatAchievementTagHandler()
	{
		Assert.That(() =>
		{
			ChatManager.ParseMessage("No achievement tags", Color.White);
			ChatManager.ParseMessage("One achievement tag: [a:KILL_THE_SUN]", Color.White);
			ChatManager.ParseMessage("One achievement tag, using the longer variant: [achievement:KILL_THE_SUN]", Color.White);
			ChatManager.ParseMessage("Multiple achievement tags: [a:KILL_THE_SUN] and [a:TOPPED_OFF]", Color.White);
			ChatManager.ParseMessage("One achievement tag, referring to a non-existent achievement: [a:_THIS_WILL_NEVER_EXIST_]", Color.White);
			ChatManager.ParseMessage("Both valid and invalid achievement tags: [a:KILL_THE_SUN] and [a:_THIS_WILL_NEVER_EXIST_]", Color.White);
		}, Throws.Nothing);
	}
}
