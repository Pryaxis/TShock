namespace TShockAPI.Configuration;

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
