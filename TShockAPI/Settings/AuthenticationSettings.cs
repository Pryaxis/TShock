namespace TShockAPI.Settings;

/// <summary>
/// Settings specific to TShock user account authentication.
/// </summary>
public sealed class AuthenticationSettings
{
	/// <summary>
	/// Enables or disables the requirement that all players must be logged in to play.
	/// <para/>
	/// Default: <see langword="false"/>.
	/// </summary>
	public bool EnableRequireLogin { get; set; } = false;

	/// <summary>
	/// Enables or disables the ability for players to login with usernames that do not match their character name.
	/// <para/>
	/// Default: <see langword="true"/>.
	/// </summary>
	public bool EnableLoginWithAnyUsername { get; set; } = true;

	/// <summary>
	/// Enables or disables the ability for players to register accounts that do not match their character name.
	/// <para/>
	/// Default: <see langword="false"/>.
	/// </summary>
	public bool EnableRegisterWithAnyUsername { get; set; } = false;

	/// <summary>
	/// Enable or disable players authenticating with their UUID.
	/// <para/>
	/// Note: UUIDs can be changed by players at any time. This should not be considered a secure authentication mechanism.
	/// <para/>
	/// Default: <see langword="true"/>.
	/// </summary>
	public bool EnableUuidLogin { get; set; } = true;

	/// <summary>
	/// Allows users to login before they finish connecting to the server.
	/// <para/>
	/// If enabled, the user will need to use their own password when presented with the password screen.
	/// This setting overrides the server password *for users who already have an account*.
	/// <para/>
	/// Default: true.
	/// </summary>
	public bool LoginBeforeJoin { get; set; } = true;

	/// <summary>
	/// The minimum length a user account's password may be. Cannot be lower than 4.
	/// <para/>
	/// Default: 4.
	/// </summary>
	public int MinimumPasswordLength { get; set; } = 4;

	/// <summary>
	/// BCrypt work factor to use. Higher values take longer to compute. 
	/// Increases to this number will upgrade all passwords to use the new work factor as they are used.
	/// <para/>
	/// For more information, see https://en.wikipedia.org/wiki/Bcrypt.
	/// <para/>
	/// Default: 7.
	/// </summary>
	public int BCryptWorkFactor { get; set; } = 7;

	/// <summary>
	/// The maximum number of failed logins a player may attempt before they are kicked, if set.
	/// <para/>
	/// Default: 3.
	/// </summary>
	public int? MaximumLoginAttempts { get; set; } = 3;

	/// <summary>
	/// Enables or disables the IP address allow list.
	/// <para/>
	/// Default: <see langword="false"/>.
	/// </summary>
	public bool EnableAllowlist { get; set; } = false;
}
