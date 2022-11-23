using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Terraria;
using TerrariaApi.Server.Services;
using TShockAPI.Settings;

using Timer = System.Threading.Timer;

namespace TShockAPI.Services;

/// <summary>
/// Service providing world backup related functionality.
/// </summary>
public sealed class BackupService : PluginService
{
	private readonly IOptionsMonitor<SaveSettings> _saveSettingsMonitor;
	private readonly WorldSaveService _saveService;
	private readonly ILogger<BackupService> _logger;
	private readonly Timer _backupTimer;
	private readonly Timer _deleteTimer;

	/// <summary>
	/// Constructs a new BackupService with its required dependencies.
	/// </summary>
	/// <param name="saveSettingsMonitor">Settings monitor providing up-to-date save settings</param>
	/// <param name="saveService">Service providing world saving</param>
	/// <param name="logger">Logger instance providing logging for this class</param>
	public BackupService(IOptionsMonitor<SaveSettings> saveSettingsMonitor,
						 WorldSaveService saveService,
						 ILogger<BackupService> logger)
	{
		_saveSettingsMonitor = saveSettingsMonitor;
		_saveService = saveService;
		_logger = logger;
		_backupTimer = new Timer(DoBackup, null, Timeout.Infinite, Timeout.Infinite);
		_deleteTimer = new Timer(DoDelete, null, Timeout.Infinite, Timeout.Infinite);
	}

	/// <summary>
	/// Starts the backup service
	/// </summary>
	public override void Start()
	{
		// Load the current settings snapshot
		SaveSettings settings = _saveSettingsMonitor.CurrentValue;

		// Start the timers
		_backupTimer.Change(settings.BackupInterval * 60 * 1000, Timeout.Infinite);
		_deleteTimer.Change(1000 * 60, 1000 * 60); // Delete timer will poll every minute
	}

	private void DoBackup(object? state)
	{
		try
		{
			// Load the current settings snapshot
			SaveSettings settings = _saveSettingsMonitor.CurrentValue;

			string worldName = Main.worldPathName;
			string name = Path.GetFileName(worldName);

			string backupRoot = Path.Combine(AppContext.BaseDirectory, settings.BackupRoot);
			Main.ActiveWorldFileData._path =
				Path.Combine(backupRoot, $"{name}.{DateTime.UtcNow:yyyy-MM-ddTHH.mm.ssZ}.bak");

			string? worldPath = Path.GetDirectoryName(Main.worldPathName);
			if (worldPath != null && !Directory.Exists(worldPath))
			{
				Directory.CreateDirectory(worldPath);
			}

			TSPlayer.All.SendInfoMessage(settings.SaveMessage);
			_logger.LogDebug("Backing up world...");
			_saveService.SaveWorld();
			_logger.LogDebug("World backed up");
			_logger.LogInformation("World backed up ({WorldName})", Main.worldPathName);

			Main.ActiveWorldFileData._path = worldName;

			// restart the timer
			_backupTimer.Change(settings.BackupInterval * 60 * 1000, Timeout.Infinite);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Backup failed!");
		}
	}

	void DoDelete(object? state)
	{
		// Load the current settings snapshot
		SaveSettings settings = _saveSettingsMonitor.CurrentValue;
		string backupRoot = Path.Combine(AppContext.BaseDirectory, settings.BackupRoot);

		foreach (FileInfo file in new DirectoryInfo(backupRoot).GetFiles("*.bak"))
		{
			if ((DateTime.UtcNow - file.LastWriteTimeUtc).TotalMinutes >= settings.BackupExpireInterval)
			{
				file.Delete();
			}
		}
	}
}
