
namespace TShockAPI;

/// <summary>
/// The pvp mode forced by the server.
/// </summary>
public enum ServerPvpMode
{
	/// <summary>
	/// Vanilla, pvp can be toggled manually by players.
	/// </summary>
	Vanilla,
	/// <summary>
	/// Pvp is force-toggled on for all players.
	/// </summary>
	ForceEnabled,
	/// <summary>
	/// Pvp is force-toggled off for all players.
	/// </summary>
	ForceDisabled
}
