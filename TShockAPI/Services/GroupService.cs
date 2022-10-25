using TerrariaApi.Server;
using TShockAPI.Repositories;

namespace TShockAPI.Services;

/// <summary>
/// Service providing group-related functionality.
/// </summary>
public sealed class GroupService : PluginService
{
	private GroupRepository _groupRepository;

	/// <summary>
	/// Constructs a new GroupService with its required dependencies
	/// </summary>
	/// <param name="groupRepository">Repository for persisting groups</param>
	public GroupService(GroupRepository groupRepository)
	{
		_groupRepository = groupRepository;
	}
}
