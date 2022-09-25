/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using EasyCommands;
using EasyCommands.Commands;
using EasyCommands.Defaults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Modules;
using TShockCommands.Annotations;

namespace TShockCommands;

/// <summary>
/// Integrates EasyCommands with TShock
/// </summary>
public class EasyCommandService : CommandHandler<TSPlayer>, ICommandService
{
	/// <inheritdoc/>
	public string Specifier => _options.Value.CommandSpecifier;

	/// <inheritdoc/>
	public string SilentSpecifier => _options.Value.CommandSilentSpecifier;

	private readonly ILogger<EasyCommandService> _logger;
	private readonly IServiceProvider _serviceProvider;
	private readonly IOptions<CommandOptions> _options;

	/// <summary>
	/// Constructor for DI to create a new EazyCommandService, with the given services.
	/// </summary>
	/// <param name="options">Command options from configuration</param>
	/// <param name="logger">Logger for this instance</param>
	/// <param name="repoLogger">Logger for the repository</param>
	/// <param name="serviceProvider">The service provider for the repository to allow DI in easy command callbacks</param>
	/// <exception cref="Exception">Configuration exceptions</exception>
	public EasyCommandService(
		IOptions<CommandOptions> options,
		ILogger<EasyCommandService> logger,
		ILogger<EasyCommandsRepository> repoLogger,
		IServiceProvider serviceProvider
	)
	{
		_logger = logger;
		_serviceProvider = serviceProvider;
		_options = options;

		if (String.IsNullOrWhiteSpace(options.Value.CommandSpecifier))
			throw new Exception($"{nameof(options.Value.CommandSpecifier)} has an invalid value. Must be a single character, found: {options.Value.CommandSpecifier ?? "<null>"}");

		if (String.IsNullOrWhiteSpace(options.Value.CommandSilentSpecifier))
			throw new Exception($"{nameof(options.Value.CommandSilentSpecifier)} has an invalid value. Must be a single character, found: {options.Value.CommandSilentSpecifier ?? "<null>"}");

		Context.TextOptions.CommandPrefix = options.Value.CommandSpecifier;
		AddParsingRules(typeof(DefaultParsingRules<TSPlayer>));
		AddParsingRules(typeof(ParsingRules));

		Context.CommandRepository = new EasyCommandsRepository(Context, options, repoLogger, serviceProvider);
	}

	#region EasyCommand adapter

	/// <summary>
	/// Called by easy command to initialise commands
	/// </summary>
	/// <remarks>no need action anything here, since this is called in the CommandHandler constructor so DI'd variables won't be available yet.</remarks>
	protected override void Initialize() { }

	/// <summary>
	/// Called by easy commands to send a falure message to a player
	/// </summary>
	/// <param name="sender">The player</param>
	/// <param name="message">The message</param>
	protected override void SendFailMessage(TSPlayer sender, string message) => sender.SendErrorMessage(message);

	/// <summary>
	/// Logs command exceptions
	/// </summary>
	/// <param name="e">The exception</param>
	protected override void HandleCommandException(Exception e) => _logger.LogError("Failed to handle command", e);

	/// <summary>
	/// Determines if a player has access to a command
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="command"></param>
	/// <returns></returns>
	public override bool CanSeeCommand(TSPlayer sender, CommandDelegate<TSPlayer> command)
	{
		var permissions = command.GetCustomAttribute<CommandPermissions>();
		return permissions is null || permissions.Permissions.Any(node => sender.Group.HasPermission(node));
	}

	#endregion

	#region TShock adapter

	/// <inheritdoc/>
	public bool Handle(TSPlayer sender, string text)
	{
		return HandleAsync(sender, text).Result;
	}

	/// <summary>
	/// Dispatches a command to the easy command library.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="text"></param>
	/// <returns></returns>
	public async Task<bool> HandleAsync(TSPlayer sender, string text)
	{
		try
		{
			await Context.CommandRepository.Invoke(sender, text);
		}
		catch (CommandParsingException ex)
		{
			SendFailMessage(sender, ex.Message);
		}
		catch (CommandExecutionException ex2)
		{
			SendFailMessage(sender, ex2.Message);
		}
		catch (Exception e)
		{
			HandleCommandException(e);
			SendFailMessage(sender, Context.TextOptions.CommandThrewException);
		}

		return true;
	}

	/// <inheritdoc/>	
	public IEnumerable<ICommand> GetCommands(string? permission = null)
	{
		IEnumerable<CommandDelegate<TSPlayer>> commands = this.CommandList;

		if (!String.IsNullOrWhiteSpace(permission))
			commands = commands.Where(x =>
			{
				var perms = x.GetCustomAttribute<CommandPermissions>();
				return perms is not null && perms.Permissions.Any(node => node == permission);
			});

		return commands.Select(cd => new CommandAdapter(cd)).Cast<ICommand>();
	}

	/// <summary>
	/// Translates the easy command delegate to TShock's command interface
	/// </summary>
	struct CommandAdapter : ICommand
	{
		private CommandDelegate<TSPlayer> _commandDelegate;

		public CommandAdapter(CommandDelegate<TSPlayer> commandDelegate)
		{
			_commandDelegate = commandDelegate;
		}

		public IEnumerable<string> Names => new[] { _commandDelegate.Name }.Concat(_commandDelegate.Aliases);
	}

	static IEnumerable<Type> GetExportedTypes(Assembly assembly)
	{
		try
		{
			return assembly.GetTypes();
		}
		catch (ReflectionTypeLoadException e)
		{
			return e.Types.Where(t => t is not null).Cast<Type>();
		}
	}

	void RegisterFromAssembly(Assembly assembly)
	{
		var types = GetExportedTypes(assembly);

		foreach (Type item in types.Where((Type t) => t.BaseType == typeof(CommandCallbacks<TSPlayer>) && !t.IsNested))
			RegisterCommands(item);
	}

	/// <inheritdoc/>
	public void Start()
	{
		var plugins = _serviceProvider.GetRequiredService<IEnumerable<PluginService>>();

		RegisterFromAssembly(this.GetType().Assembly);

		foreach (var plugin in plugins)
			RegisterFromAssembly(plugin.GetType().Assembly);
	}
	#endregion
}
