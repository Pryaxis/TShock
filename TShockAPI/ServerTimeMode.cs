namespace TShockAPI;

/// <summary>
/// The time mode forced by the server.
/// </summary>
public enum ServerTimeMode
{
	/// <summary>
	/// No changes to how time would regularly flow.
	/// </summary>
	Vanilla,
	/// <summary>
	/// Time is forced to day.
	/// </summary>
	Day,
	/// <summary>
	/// Time is forced to night.
	/// </summary>
	Night
}
