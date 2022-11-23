namespace TShockAPI.Settings;

/// <summary>
/// Settings specific to TShock's anti-cheat functions.
/// </summary>
public sealed class AntiCheatSettings
{
	/// <summary>
	/// Enables or disables the use of the Zenith projectile with different objects instead of weapons.
	/// </summary>
	public bool EnableModifiedZenith { get; set; } = true;

	/// <summary>
	/// Enables or disables the ability for clients to send their own death messages.
	/// Clients may send death messages with any content at any time, if enabled.
	/// <para/>
	/// Default: <see langword="false"/>.
	/// </summary>
	public bool EnableCustomDeathMessages { get; set; } = false;
}
