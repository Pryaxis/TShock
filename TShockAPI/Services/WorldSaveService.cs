using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Threading;
using System.Threading.Tasks;

using Terraria;
using Terraria.IO;

using TerrariaApi.Server;

using TShockAPI.Configuration;

namespace TShockAPI.Services;

/// <summary>
/// Service providing world saving functionality.
/// </summary>
public sealed class WorldSaveService : PluginService
{
	private IOptionsMonitor<SaveSettings> _saveSettings;
	private CancellationToken _cancellationToken;
	private ILogger<WorldSaveService> _logger;
	private EventWaitHandle _waitHandle;

	/// <summary>
	/// Constructs a new world save service
	/// </summary>
	/// <param name="saveSettings">Monitored save settings</param>
	/// <param name="applicationLifetime">ApplicationLifetime providing access to lifetime events</param>
	/// <param name="logger">Logger instance for this service</param>
	public WorldSaveService(IOptionsMonitor<SaveSettings> saveSettings,
							IHostApplicationLifetime applicationLifetime,
							ILogger<WorldSaveService> logger)
	{
		_saveSettings = saveSettings;
		_cancellationToken = applicationLifetime.ApplicationStopping;
		_logger = logger;
		_waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset);
	}

	/// <summary>
	/// Saves the world.
	/// </summary>
	/// <param name="resetTime">Whether the world's time should be reset to daytime</param>
	public void SaveWorld(bool resetTime = false)
	{
		QueueWorldSave(resetTime: resetTime);
	}

	/// <summary>
	/// Queues a world save.
	/// </summary>
	/// <param name="resetTime">Whether the world's time should be reset to daytime</param>
	/// <returns>An awaitable task that will complete when the save has completed.</returns>
	public Task QueueWorldSave(bool resetTime = false)
	{
		return Task.Run(() =>
		{
			if (WaitHandle.WaitAny(new[] { _waitHandle, _cancellationToken.WaitHandle }) == 0)
			{
				try
				{
					SaveSettings settings = _saveSettings.CurrentValue;
					WorldFile.SaveWorld(useCloudSaving: false, resetTime: resetTime);

					TSPlayer.All.SendInfoMessage(settings.SaveMessage);
					_logger.LogInformation("World saved at {worldPath}.", Main.worldPathName);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "World save failed.");
				}

				_waitHandle.Set(); // Set the wait handle so the next queued save can proceed
			}
		});
	}
}
