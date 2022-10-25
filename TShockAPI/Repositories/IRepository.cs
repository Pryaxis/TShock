namespace TShockAPI.Repositories;

/// <summary>
/// Describes a generic repository that contains entities with unique IDs.
/// </summary>
/// <typeparam name="TEntity">Type of entity that is contained</typeparam>
/// <typeparam name="TKey">Key used to uniquely identify entities</typeparam>
public interface IRepository<TEntity, TKey>
{
	/// <summary>
	/// Persists the given entity.
	/// </summary>
	/// <param name="entity">Entity to persist</param>
	void Create(TEntity entity);

	/// <summary>
	/// Reads an entity from the repository with the given ID.
	/// </summary>
	/// <param name="id">ID uniquely identifying the entity to retrieve</param>
	/// <returns>A matching entity, or <c>default(TEntity)</c> if none was found</returns>
	TEntity? Read(TKey id);

	/// <summary>
	/// Updates the given entity in the repository.
	/// </summary>
	/// <param name="entity">Entity to update</param>
	void Update(TEntity entity);

	/// <summary>
	/// Removes the given entity from the repository.
	/// </summary>
	/// <param name="entity">Entity to remove</param>
	void Delete(TEntity entity);
}
