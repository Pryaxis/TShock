using System;
using System.Linq;
using System.Reflection;
using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using OTAPI;
using OTAPI.Shims.TShock;
using TerrariaApi.Server;

namespace TShock.Launcher
{
	class Program
	{
		static void Main(string[] args)
		{
			AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
			{
				if (eventArgs.Name.StartsWith("TerrariaServer,"))
					return typeof(ServerApi).Assembly;
				return null;
			};

			SetupLogManager();

			ServerApi.PluginLoader = new PluginLoader(args.Any(a => a == "-ignoreversion"));

			Launch.Start(args);
		}

		static void SetupLogManager()
		{
			//TODO: Make logger configurable
			var config = new LoggingConfiguration();

			// Targets where to log to: File and Console
			var logfile = new FileTarget("logfile") { FileName = "ServerLog.txt" };
			logfile.ArchiveEvery = FileArchivePeriod.Day;
			logfile.ArchiveNumbering = ArchiveNumberingMode.Date;

			var consoleTarget = new ColoredConsoleTarget();
			consoleTarget.UseDefaultRowHighlightingRules = true;
			consoleTarget.Layout = Layout.FromMethod(logEvent =>
			{
				return $"[{logEvent.LoggerName}]{logEvent.FormattedMessage}";
			});
			// Rules for mapping loggers to targets
			config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
			config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

			// Apply config
			LogManager.Configuration = config;
		}
	}
}
