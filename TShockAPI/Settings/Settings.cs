namespace TShockAPI.Settings;

/// <summary>
/// Contains configuration settings for TShock
/// </summary>
public sealed class Settings
{
	/// <summary>
	/// Settings specific to the setup of the server, such as password and port.
	/// </summary>
	public ServerSettings ServerSettings { get; set; } = new ServerSettings();
	/// <summary>
	/// Settings specific to saving and backups.
	/// </summary>
	public SaveSettings SaveSettings { get; set; } = new SaveSettings();
	/// <summary>
	/// Settings specific to how the game operates.
	/// </summary>
	public GameSettings GameSettings { get; set; } = new GameSettings();
	/// <summary>
	/// Settings specific to the tile and world protection systems in TShock.
	/// </summary>
	public ProtectionSettings ProtectionSettings { get; set; } = new ProtectionSettings();
	/// <summary>
	/// Settings specific to TShock user groups.
	/// </summary>
	public GroupSettings GroupSettings { get; set; } = new GroupSettings();
	/// <summary>
	/// Settings specific to TShock user account authentication.
	/// </summary>
	public AuthenticationSettings AuthenticationSettings { get; set; } = new AuthenticationSettings();
	/// <summary>
	/// Settings specific to TShock's auto-kick and ban rules.
	/// </summary>
	public PunishmentSettings PunishmentSettings { get; set; } = new PunishmentSettings();
	/// <summary>
	/// Settings specific to TShock's anti-cheat functions.
	/// </summary>
	public AntiCheatSettings AntiCheatSettings { get; set; } = new AntiCheatSettings();

	/*
    Notes:
    * LogPath, DebugLogs, any other log config needs to be dealt with in NLog config
    * Convert MaxHp & MaxMP config options to permissions -> player.hp.<value>, player.mp.<value>?
    * Convert damage config options to permissions -> player.damage.<value>, player.projdamage.<value>?
    * Convert BombExplosionRadius config option to permission -> player.bombradius.<value>?
    * Convert MaxRangeForDisabled config option to permission -> player.disableradius.<value>?
	* Convert thresholds to permission -> player.threshold.tilekill.<value>, etc.
    * Migrate RangeChecks config option to a permission.
    * Migrate DisableBuild config option to a permission.
    * Migrate DisableInvisPvP config option to a permission?
    * Migrate IgnoreProjUpdate to permission
    * Migrate IgnoreProjKill to permission
    * Migrate AllowIce to permission
    * Migrate AllowCutTilesAndBreakables to permission
    * Migrate PreventBannedItemSpawn to permission
    * Migrate PreventDeadModification to permission
    * Migrate PreventInvalidPlaceStyle to permission
    * Migrate AllowAllowedGroupsToSpawnBannedItems to permission?
    */
}
