namespace TShockAPI;

/// <summary>
/// The character mode allowed by the server.
/// </summary>
public enum ServerCharacterMode
{
	/// <summary>
	/// Vanilla, no restrictions on characters.
	/// </summary>
	Vanilla,
	/// <summary>
	/// Only softcore characters are allowed into the server.
	/// </summary>
	SoftcoreOnly,
	/// <summary>
	/// Only mediumcore characters are allowed into the server.
	/// </summary>
	MediumcoreOnly,
	/// <summary>
	/// Only hardcore characters are allowed into the server.
	/// </summary>
	HardcoreOnly
}
