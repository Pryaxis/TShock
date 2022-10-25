namespace TShockAPI.Configuration;

/// <summary>
/// Settings specific to TShock user groups.
/// </summary>
public sealed class GroupSettings
{
	/// <summary>
	/// The default group newly registered users will be placed into.
	/// <para/>
	/// Default: "default".
	/// </summary>
	public string DefaultUserGroup { get; set; } = "default";

	/// <summary>
	/// The group unregistered users will be placed in.
	/// <para/>
	/// Default: "guest".
	/// </summary>
	public string GuestGroup { get; set; } = "guest";
}
