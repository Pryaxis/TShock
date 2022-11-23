using System.Linq;
using Microsoft.EntityFrameworkCore;
using TShockAPI.DB;
using TShockAPI.Models;
using TShockAPI.Services.Scoping;

namespace TShockAPI.Services;

/// <summary>
/// Service providing group-related functionality.
/// </summary>
public sealed class UserGroupService : DatabaseService<UserGroup>
{
	/// <summary>
	/// TODO
	/// </summary>
	/// <param name="contextFactory"></param>
	public UserGroupService(IDbContextFactory<EntityContext<UserGroup>> contextFactory)
		: base(contextFactory)
	{
	}

	/// <summary>
	/// Saves the given group to persistent storage
	/// </summary>
	/// <param name="userGroup">Group to save</param>
	public void CreateGroup(UserGroup userGroup)
	{
		using var context = GetContext(saveOnDispose: true);
		context.Add(userGroup);
	}

	/// <summary>
	/// Retrieves a user group by name
	/// </summary>
	/// <param name="groupName">Name of the group to retrieve</param>
	/// <returns>A user group that's name matches the search term, or null</returns>
	public UserGroup? GetGroup(string groupName)
	{
		using EntityContext<UserGroup> context = GetContext(saveOnDispose: true);
		return context.Entities.FirstOrDefault(group => group.Name == groupName);
	}
}
