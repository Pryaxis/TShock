namespace TShockAPI.Configuration;

/// <summary>
/// Settings specific to TShock's auto-kick and ban rules.
/// </summary>
public sealed class PunishmentSettings
{
	/// <summary>
	/// Kick players who connect via a proxy, when detected by the GeoIP library.
	/// <para/>
	/// Default: <see langword="false"/>.
	/// </summary>
	public bool KickProxyUsers { get; set; } = false;

	/// <summary>
	/// Kick players who do not present a UUID when connecting to the server.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickEmptyUuid { get; set; } = false;

	/// <summary>
	/// Kick players who exceed the tile place threshold.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnTilePlaceThresholdExceeded { get; set; } = false;

	/// <summary>
	/// Kick players who exceed the tile kill threshold.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnTileKillThresholdExceeded { get; set; } = false;

	/// <summary>
	/// Kick players who exceed the paint threshold.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnPaintThresholdExceeded { get; set; } = false;

	/// <summary>
	/// Kick players who exceed the liquid threshold.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnLiquidThresholdExceeded { get; set; } = false;

	/// <summary>
	/// Kick players who exceed the projectile threshold.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnProjectileThresholdExceeded { get; set; } = false;

	/// <summary>
	/// Kick players who exceed the heal other threshold.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnHealOtherThresholdExceeded { get; set; } = false;

	/// <summary>
	/// Kick players who exceed the damage threshold.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnDamageThresholdExceeded { get; set; } = false;

	/// <summary>
	/// Kick players who die while using a medium-core character.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnMediumcoreDeath { get; set; } = false;

	/// <summary>
	/// Kick players who die while using a hard-core character.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool KickOnHardcoreDeath { get; set; } = false;

	/// <summary>
	/// Ban players who die while using a medium-core character.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool BanOnMediumcoreDeath { get; set; } = false;

	/// <summary>
	/// Ban players who die while using a hard-core character.
	/// <para/>
	/// Default: <see langword="false"/>
	/// </summary>
	public bool BanOnHardcoreDeath { get; set; } = false;
}
