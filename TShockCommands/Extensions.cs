using System.Linq;
using EasyCommands.Commands;
using TShockAPI;
using TShockCommands.Annotations;

namespace TShockCommands;

public static class CommandDelegateExtensions
{
	public static HelpText GetHelpText(this CommandDelegate<TSPlayer> command)
		=> command.GetCustomAttribute<HelpText>();

	public static CommandPermissions GetCommandPermissions(this CommandDelegate<TSPlayer> command)
		=> command.GetCustomAttribute<CommandPermissions>();

	/// <summary>
	/// Determines if the given player can run/see the given command.
	/// </summary>
	/// <param name="command">Command to run or see</param>
	/// <param name="player">The player</param>
	/// <returns></returns>
	public static bool CanRun(this CommandDelegate<TSPlayer> command, TSPlayer player)
	{
		var permissions = command.GetCommandPermissions();
		return permissions is null || permissions.Permissions.Any(node => player.Group.HasPermission(node));
	}
}