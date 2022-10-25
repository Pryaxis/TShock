namespace TShockAPI.Configuration;

/// <summary>
/// Settings specific to the setup of the server, such as password and port.
/// </summary>
public sealed class ServerSettings
{
	/// <summary>
	/// The port that the server will open under. This port cannot be already in use.
	/// </summary>
	public int Port { get; set; } = 7777;

	/// <summary>
	/// The maximum number of players who can join the server.
	/// <para/>
	/// Max: 255. Default: 8.
	/// </summary>
	public byte MaxPlayerSlots { get; set; } = 8;

	/// <summary>
	/// Slots that are reserved for players with the <see cref="Permissions.reservedslot"/> permission.
	/// <para/>
	/// Note that this requires <see cref="AuthenticationSettings.LoginBeforeJoin"/> to be enabled.
	/// Note that this uses players slots defined in <see cref="MaxPlayerSlots"/>.
	/// <para/>
	/// Default: 0.
	/// </summary>
	public byte ReservedSlots { get; set; } = 0;

	/// <summary>
	/// Overrides the world name while the server is running, if set.
	/// </summary>
	public string? ServerName { get; set; }

	/// <summary>
	/// Server password. Users will need to provide the password to enter the server, if set.
	/// <para/>
	/// Overridden by <see cref="AuthenticationSettings.LoginBeforeJoin"/> if enabled.
	/// </summary>
	public string? Password { get; set; }
}
