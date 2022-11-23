using Microsoft.EntityFrameworkCore;

namespace TShockAPI.DB;

/// <summary>
/// Represents a session with the database, providing a set of entities that can be queried and modified.
/// <para/>
/// This object should always be disposed of after usage. See the EF Core Documentation on
/// <see href="https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/">DbContext</see> for more information.
/// </summary>
/// <typeparam name="TEntity">The type of object being operated on by the context</typeparam>
public class EntityContext<TEntity> : DbContext where TEntity : class
{
	/// <summary>
	/// Whether this context should save any changes when disposed
	/// </summary>
	public bool SaveOnDispose { get; set; }

	/// <summary>
	/// The set of entities present on the context
	/// </summary>
	public DbSet<TEntity> Entities { get; set; } = null!;

	/// <summary>
	/// Constructs a new EntityContext with the given options
	/// </summary>
	/// <param name="options"></param>
	public EntityContext(DbContextOptions<EntityContext<TEntity>> options)
		: base(options) { }

	/// <summary>
	/// Disposes the context, saving changes if <see cref="SaveOnDispose"/> is true
	/// </summary>
	public override void Dispose()
	{
		if (SaveOnDispose && ChangeTracker.HasChanges())
		{
			SaveChanges();
		}

		base.Dispose();
	}
}
