using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Terraria;
using TerrariaApi.Server;
using TShockAPI.Configuration;

using Timer = System.Threading.Timer;

namespace TShockAPI.Services;

/// <summary>
/// Service providing world backup related functionality.
/// </summary>
public sealed class BackupService : PluginService
{
	private IOptionsMonitor<SaveSettings> _saveSettingsMonitor;
	private WorldSaveService _saveService;
	private ILogger<BackupService> _logger;
	private Timer _backupTimer;
	private Timer _deleteTimer;

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

	void DoBackup(object? state)
	{
		try
		{
			// Load the current settings snapshot
			SaveSettings settings = _saveSettingsMonitor.CurrentValue;

			string worldname = Main.worldPathName;
			string name = Path.GetFileName(worldname);

			string backupRoot = Path.Combine(AppContext.BaseDirectory, settings.BackupRoot);
			Main.ActiveWorldFileData._path = Path.Combine(backupRoot, string.Format("{0}.{1:yyyy-MM-ddTHH.mm.ssZ}.bak", name, DateTime.UtcNow));

			string? worldpath = Path.GetDirectoryName(Main.worldPathName);
			if (worldpath != null && !Directory.Exists(worldpath))
			{
				Directory.CreateDirectory(worldpath);
			}

			TSPlayer.All.SendInfoMessage(settings.SaveMessage);
			_logger.LogDebug("Backing up world...");
			_saveService.SaveWorld();
			_logger.LogDebug("World backed up.");
			_logger.LogInformation("World backed up ({worldName}).", Main.worldPathName);

			Main.ActiveWorldFileData._path = worldname;

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
