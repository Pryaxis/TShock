namespace TShockAPI.Repositories;

/// <summary>
/// Repository providing persistence for <see cref="Group"/>s.
/// </summary>
public sealed class GroupRepository : IRepository<Group, int>
{
	/// <summary>
	/// Persists the given group.
	/// </summary>
	/// <param name="group">Group to persist</param>
	public void Create(Group group)
	{

	}

	/// <summary>
	/// Attempts to retrieve a group with the given ID.
	/// </summary>
	/// <param name="id">ID of the group to retrieve</param>
	/// <returns>A matching group, or null if none was found</returns>
	public Group? Read(int id)
	{
		return default;
	}

	/// <summary>
	/// Persists an update for the given group.
	/// </summary>
	/// <param name="group"></param>
	public void Update(Group group)
	{

	}

	/// <summary>
	/// Deletes the given group.
	/// </summary>
	/// <param name="group"></param>
	public void Delete(Group group)
	{

	}
}
