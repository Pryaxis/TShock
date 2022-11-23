using Microsoft.EntityFrameworkCore;
using TShockAPI.DB;

namespace TShockAPI.Services.Scoping;

/// <summary>
/// Service providing scoped <see cref="EntityContext{TEntity}"/> instances
/// </summary>
/// <typeparam name="TEntity">Type of entity being operated on by the <see cref="EntityContext{TEntity}"/></typeparam>
public class DatabaseService<TEntity> where TEntity : class
{
	private readonly IDbContextFactory<EntityContext<TEntity>> _contextFactory;

	/// <summary>
	/// Constructs a new DatabaseService with the given <see cref="IDbContextFactory{TContext}"/>
	/// </summary>
	/// <param name="contextFactory">Context factory that will be used to create database contexts</param>
	public DatabaseService(IDbContextFactory<EntityContext<TEntity>> contextFactory)
	{
		_contextFactory = contextFactory;
	}

	/// <summary>
	///	Creates a new database context, optionally allowing the context to save changes when it is disposed.
	/// <para/>
	/// Contexts should always be disposed after use. Where possible, consider using contexts in a
	/// <c>using</c> block
	/// </summary>
	/// <returns>An <see cref="EntityContext{TEntity}"/></returns>
	public EntityContext<TEntity> GetContext(bool saveOnDispose = false)
	{
		EntityContext<TEntity> context = _contextFactory.CreateDbContext();
		context.SaveOnDispose = saveOnDispose;
		return context;
	}
}
