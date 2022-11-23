using Microsoft.EntityFrameworkCore;
using TShockAPI.DB;
using TShockAPI.Services.Scoping;
using Region = TShockAPI.Models.Region;

namespace TShockAPI.Services;

/// <summary>
/// Service providing region-related functionality
/// </summary>
public sealed class RegionService : DatabaseService<Region>
{
	/// <summary>
	/// Constructs a new region service with the given database context factory
	/// </summary>
	/// <param name="contextFactory">Factory for creating database contexts</param>
	public RegionService(IDbContextFactory<EntityContext<Region>> contextFactory)
		: base(contextFactory) { }

	/// <summary>
	/// Creates a region, optionally allowing context re-use.
	/// </summary>
	/// <param name="region">The region to create</param>
	/// <param name="context">Optional context, to perform multiple actions as a single unit of work</param>
	public void CreateRegion(Region region, EntityContext<Region>? context = null)
	{
		ContextSafeFunc(ctx => ctx.Add(region), context);
	}

	/// <summary>
	/// Retrieves a region by its name and world ID, optionally allowing context re-use.
	/// </summary>
	/// <param name="name">Name of the region to find</param>
	/// <param name="worldId">ID of the world the region is present in</param>
	/// <param name="context">Optional context, to perform multiple actions as a single unit of work</param>
	/// <returns>A <see cref="Region"/> if one was found matching the given name and world ID</returns>
	public Region? GetRegion(string name, int worldId, EntityContext<Region>? context = null)
	{
		return ContextSafeFunc(ctx => ctx.Find<Region>(name, worldId), context);
	}

	/// <summary>
	/// Updates a region, optionally allowing context re-use.
	/// </summary>
	/// <param name="region">The region to update</param>
	/// <param name="context">Optional context, to perform multiple actions as a single unit of work</param>
	public void UpdateRegion(Region region, EntityContext<Region>? context = null)
	{
		ContextSafeFunc(ctx => ctx.Update(region), context);
	}

	/// <summary>
	/// Deletes a region, optionally allowing context re-use.
	/// </summary>
	/// <param name="region">The region to delete</param>
	/// <param name="context">Optional context, to perform multiple actions as a single unit of work</param>
	public void DeleteRegion(Region region, EntityContext<Region>? context = null)
	{
		ContextSafeFunc(ctx => ctx.Remove(region), context);
	}
}
