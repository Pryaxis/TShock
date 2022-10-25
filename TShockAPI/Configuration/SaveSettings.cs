namespace TShockAPI.Configuration;

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
	public string? SaveMessage { get; set; }

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

	/// <summary>
	/// Directory into which backups will be saved. Must be a relative directory path.
	/// <para/>
	/// Default: "backups".
	/// </summary>
	public string BackupRoot { get; set; } = "backups";
}
