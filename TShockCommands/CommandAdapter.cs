using EasyCommands.Commands;
using System.Collections.Generic;
using System.Linq;
using TShockAPI;
using TShockAPI.Modules;

namespace TShockCommands;

/// <summary>
/// Translates the easy command delegate to TShock's command interface
/// </summary>
public struct CommandAdapter : ICommand
{
	/// <summary>
	/// The delegate to invoke
	/// </summary>
	public CommandDelegate<TSPlayer> CommandDelegate { get; set; }

	/// <summary>
	/// Translates a easy commands command to a TShock ICommand
	/// </summary>
	/// <param name="commandDelegate"></param>
	public CommandAdapter(CommandDelegate<TSPlayer> commandDelegate)
	{
		CommandDelegate = commandDelegate;
	}

	public IEnumerable<string> Names => new[] { CommandDelegate.Name }.Concat(CommandDelegate.Aliases);
}
