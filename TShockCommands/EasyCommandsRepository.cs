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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TShockAPI;
using TShockCommands.Annotations;

namespace TShockCommands;

public class EasyCommandsRepository : CommandRepository<TSPlayer>
{
	Dictionary<string, CommandDelegate<TSPlayer>> _commands = new();
	private readonly IOptions<CommandOptions> _options;
	private readonly ILogger<EasyCommandsRepository> _logger;
	private readonly IServiceProvider _serviceProvider;

	public EasyCommandsRepository(
		Context<TSPlayer> context,
		IOptions<CommandOptions> options,
		ILogger<EasyCommandsRepository> logger,
		IServiceProvider serviceProvider
	) : base(context)
	{
		_options = options;
		_logger = logger;
		_serviceProvider = serviceProvider;
	}

	protected override void AddCommand(CommandDelegate<TSPlayer> command, string[] names)
	{
		foreach (var name in names)
		{
			if (!Regex.IsMatch(name, Context.TextOptions.CommandNameValidationRegex))
				throw new CommandRegistrationException(string.Format(Context.TextOptions.InvalidCommandNameErrorMessage, name));

			if (!_commands.TryAdd(name, command))
				throw new CommandRegistrationException($"Cannot register `{name}` as one is already defined.");
		}
	}

	//public override bool CanResolveType(Type type) => _serviceProvider.GetService(type) is not null;

	public override async Task Invoke(TSPlayer sender, string command)
	{
		if (String.IsNullOrWhiteSpace(command))
			throw new CommandParsingException(Context.TextOptions.EmptyCommand);

		bool silent = false;

		var text = command.Trim();
		if (text.StartsWith(_options.Value.CommandSpecifier))
			text = text.Substring(_options.Value.CommandSpecifier.Length);
		else if (text.StartsWith(_options.Value.CommandSilentSpecifier))
		{
			text = text.Substring(_options.Value.CommandSilentSpecifier.Length);
			silent = true;
		}

		string name, parameters = "";
		CommandDelegate<TSPlayer>? ezcmd = null;
		var first_space = text.IndexOf(' ');
		if (first_space > 0)
		{
			name = text.Substring(0, first_space);
			parameters = text.Substring(first_space + 1);
		}
		else name = text;

		if (!String.IsNullOrWhiteSpace(name))
			_commands.TryGetValue(name, out ezcmd);

		if (ezcmd is null)
			throw new CommandParsingException(string.Format(Context.TextOptions.CommandNotFound, name ?? "<not found>"));

		// Stop the server from running a command if he's not allowed to, it's allowed by default
		if (sender is TSServerPlayer && ezcmd.GetCustomAttribute<AllowServer>()?.Allow == false)
			throw new CommandExecutionException("The server doesn't have the permission to execute this command.");

		// Stop a user from running a command or subcommand if they don't have permission to use it
		var permissions = ezcmd.GetCustomAttribute<CommandPermissions>();
		if (permissions != null && permissions.Permissions.Any(p => !sender.HasPermission(p)))
		{
			_logger.LogError($"{sender.Name} tried to execute {ezcmd.Name}.");
			throw new CommandExecutionException("You don't have the necessary permission to do that.");
		}

		if (ezcmd.GetCustomAttribute<DoLog>()?.Log != false)
			_logger.LogInformation($"{sender.Name} executed: {command}.");

		using var scope = _serviceProvider.CreateScope();
		var args = scope.ServiceProvider.GetRequiredService<CommandArgs>();
		args.RawCommand = command;
		args.Silent = silent;

		// TODO: implement a few changes in EasyCommands to allow external type resolvers
		//ezcmd.OnResolveType += OnResolveType;
		//try
		//{
		await ezcmd.Invoke(sender, parameters);
		//}
		//finally
		//{
		//	ezcmd.OnResolveType -= OnResolveType;
		//}

		//object OnResolveType(Type type) => scope.ServiceProvider.GetService(type);
	}
}
