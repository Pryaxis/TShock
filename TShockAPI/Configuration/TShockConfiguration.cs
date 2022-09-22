

namespace TShockAPI.Configuration;

/// <summary>
/// Contains configuration settings for TShock
/// </summary>
public sealed class TShockConfiguration
{
	public ServerSettings Server { get; set; }
	public SaveSettings Save { get; set; }
	public GameSettings Game { get; set; }
	public ProtectionSettings Protection { get; set; }
	public AuthenticationSettings UserAccount { get; set; }
	public PunishmentSettings Punishments { get; set; }
	public AntiCheatSettings AntiCheat { get; set; }

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
		/// Note that this requires <see cref="LoginBeforeJoin"/> to be enabled.
		/// Note that this uses players slots defined in <see cref="MaxPlayerSlots"/>.
		/// <para/>
		/// Default: 0.
		/// </summary>
		public byte ReservedSlots { get; set; } = 0;

		/// <summary>
		/// Overrides the world name while the server is running, if set.
		/// </summary>
		public string ServerName { get; set; }

		/// <summary>
		/// Server password. Users will need to provide the password to enter the server, if set.
		/// <para/>
		/// Overridden by <see cref="LoginBeforeJoin"/> if enabled.
		/// </summary>
		public string Password { get; set; }
	}

	/// <summary>
	/// Settings specific to saving and backups.
	/// </summary>
	public sealed class SaveSettings
	{
		/// <summary>
		/// Enable or disable Terraria's built in world auto-save.
		/// <para/>
		/// Default: true.
		/// </summary>
		public bool AutoSave { get; set; } = true;

		/// <summary>
		/// Enable or disable saving the world if the server crashes unexpectedly.
		/// <para/>
		/// Default: true.
		/// </summary>
		public bool AutoSaveOnCrash { get; set; } = true;

		/// <summary>
		/// Enable or disable saving the world when the last player disconnects.
		/// <para/>
		/// Default: true.
		/// </summary>
		public bool AutoSaveOnLastPlayerExit { get; set; } = true;

		/// <summary>
		/// The message that will be broadcast to the server when the world saves, if set.
		/// <para/>
		/// Default: <see langword="null"/>
		/// </summary>
		public string SaveMessage { get; set; }

		/// <summary>
		/// How frequently to save world file backups, in minutes.
		/// <para/>
		/// Default: 10.
		/// </summary>
		public int BackupInterval { get; set; } = 10;

		/// <summary>
		/// How long backups should be kept before being deleted, in minutes.
		/// <para/>
		/// Default: 240.
		/// </summary>
		public int BackupExpireInterval { get; set; } = 240;
	}

	/// <summary>
	/// Settings specific to how the game operates.
	/// </summary>
	public sealed class GameSettings
	{
		/// <summary>
		/// The default maximum number of mobs that can be spawned during a wave.
		/// A higher number means more mobs will spawn.
		/// <para/>
		/// Default: 5.
		/// </summary>
		public int MaximumMobSpawns { get; set; } = 5;

		/// <summary>
		/// The interval between mob spawn waves. A lower value means waves will occur more frequently.
		/// <para/>
		/// Default: 600.
		/// </summary>
		public int SpawnRate { get; set; } = 600;

		/// <summary>
		/// The number of NPCs a statue can spawn within 200 pixels of itself, before it stops spawning.
		/// <para/>
		/// Default: 3.
		/// </summary>
		public int StatueSpawn200 { get; set; } = 3;

		/// <summary>
		/// The number of NPCs a statue can spawn within 600 pixels of itself, before it stops spawning.
		/// <para/>
		/// Default: 6.
		/// </summary>
		public int StatueSpawn600 { get; set; } = 6;

		/// <summary>
		/// The number of NPCs a statue can spawn in total, before it stops spawning.
		/// <para/>
		/// Default: 10.
		/// </summary>
		public int StatueSpawnWorld { get; set; } = 10;

		/// <summary>
		/// How large to make invasions. Invasion size is calculated as (100 + (multiplier * onlinePlayerCount)).
		/// For never-ending invasions, set this to a very large number.
		/// <para/>
		/// Default: 1.
		/// </summary>
		public int InvasionMultiplier { get; set; } = 1;

		/// <summary>
		/// Server PVP mode. Defines the PVP state enforced by the server.
		/// <para/>
		/// Default: <see cref="ServerPvpMode.Vanilla"/>.
		/// </summary>
		public ServerPvpMode PvpMode { get; set; } = ServerPvpMode.Vanilla;

		/// <summary>
		/// Server character mode. Defines which types of characters may join the server.
		/// <para/>
		/// Default: <see cref="ServerCharacterMode.Vanilla"/>.
		/// </summary>
		public ServerCharacterMode CharacterMode { get; set; } = ServerCharacterMode.Vanilla;

		/// <summary>
		/// Server time mode. Defines how time works in the server.
		/// <para/>
		/// Default: <see cref="ServerTimeMode.Vanilla"/>.
		/// </summary>
		public ServerTimeMode TimeMode { get; set; } = ServerTimeMode.Vanilla;

		/// <summary>
		/// Enable or disable crimson spreading.
		/// <para/>
		/// Default: <see langword="true"/>.
		/// </summary>
		public bool EnableCrimsonSpread { get; set; } = true;

		/// <summary>
		/// Enable or disable hallow spreading.
		/// <para/>
		/// Default: <see langword="true"/>.
		/// </summary>
		public bool EnableHallowSpread { get; set; } = true;

		/// <summary>
		/// Enable or disable corruption spreading.
		/// <para/>
		/// Default: <see langword="true"/>.
		/// </summary>
		public bool EnableCorruptionSpread { get; set; } = true;

		/// <summary>
		/// Enable to allow Christmas events year-round, disable to allow Christmas events only during Christmas.
		/// <para/>
		/// Default: <see langword="false"/>.
		/// </summary>
		public bool ForceChristmas { get; set; } = false;

		/// <summary>
		/// Enable to allow Halloween events year-round, disable to allow Halloween events only during Halloween.
		/// <para/>
		/// Default: <see langword="false"/>.
		/// </summary>
		public bool ForceHalloween { get; set; } = false;

		/// <summary>
		/// Enable or disable clown bomb projectiles from spawning.
		/// <para/>
		/// Default: <see langword="false"/>.
		/// </summary>
		public bool EnableClownBombs { get; set; } = false;

		/// <summary>
		/// Enable or disable snowball projectiles from spawning.
		/// <para/>
		/// Default: <see langword="false"/>.
		/// </summary>
		public bool EnableSnowballs { get; set; } = false;

		/// <summary>
		/// Enable or disable tombstones dropping when players die.
		/// <para/>
		/// Default: <see langword="false"/>.
		/// </summary>
		public bool EnableTombstones { get; set; } = false;

		/// <summary>
		/// Enable or disable Skeletron Prime's bomb projectiles.
		/// <para/>
		/// Default: <see langword="true"/>.
		/// </summary>
		public bool EnablePrimeBombs { get; set; } = true;

		/// <summary>
		/// Enable or disable world-wide announcements when a boss or invasion is spawned.
		/// <para/>
		/// Default: <see langword="false"/>.
		/// </summary>
		public bool EnableBossSpawnAnnouncements { get; set; } = false;

		/// <summary>
		/// Enable or disable the dungeon guardian being spawned when a player delves too deep into the dungeon.
		/// Teleports the player back to their spawnpoint instead, if disabled.
		/// <para/>
		/// Default: <see langword="true"/>.
		/// </summary>
		public bool EnableDungeonGuardian { get; set; } = true;

		/// <summary>
		/// Enable or disable hardmode activating when the Wall of Flesh is killed.
		/// <para/>
		/// Default: <see langword="true"/>.
		/// </summary>
		public bool EnableHardmode { get; set; } = true;
	}

	/// <summary>
	/// Settings specific to the tile and world protection systems in TShock.
	/// </summary>
	public sealed class ProtectionSettings
	{
		/// <summary>
		/// The number of tiles around the world spawn point that will be automatically protected, if set.
		/// <para/>
		/// Default: 10.
		/// </summary>
		public int? SpawnProtectionRadius { get; set; } = 10;

		/// <summary>
		/// Enable or disable region protection being applied to chests within regions.
		/// <para/>
		/// Default: <see langword="true"/>.
		/// </summary>
		public bool EnableChestProtectionInRegions { get; set; } = true;

		/// <summary>
		/// Enable or disable region protection being applied to gem locks within regions.
		/// <para/>
		/// Default: <see langword="true"/>.
		/// </summary>
		public bool EnableGemLockProtectionInRegions { get; set; } = true;
	}

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

		public bool KickOnTilePlaceThresholdExceeded { get; set; } = false;

		public bool KickOnTileKillThresholdExceeded { get; set; } = false;

		public bool KickOnPaintThresholdExceeded { get; set; } = false;

		public bool KickOnLiquidThresholdExceeded { get; set; } = false;

		public bool KickOnProjectileThresholdExceeded { get; set; } = false;

		public bool KickOnHealOtherThresholdExceeded { get; set; } = false;

		public bool KickOnDamageThresholdExceeded { get; set; } = false;

		public bool KickOnMediumcoreDeath { get; set; } = false;

		public bool KickOnHardcoreDeath { get; set; } = false;

		public bool BanOnMediumcoreDeath { get; set; } = false;

		public bool BanOnHardcoreDeath { get; set; } = false;
	}

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
