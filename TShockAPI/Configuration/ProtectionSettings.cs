namespace TShockAPI.Configuration;

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
