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
		: base(contextFactory)
	{
	}

	/// <summary>
	/// Retrieves a region by its ID, optionally allowing context re-use.
	/// </summary>
	/// <param name="id">ID of the region to find</param>
	/// <param name="context">Optional context, to perform multiple actions in a single unit of work</param>
	/// <returns>A <see cref="Region"/> if one was found with a matching ID</returns>
	public Region? GetRegion(int id, EntityContext<Region>? context = null)
	{
		if (context == null)
		{
			using EntityContext<Region> singleUseContext = GetContext(saveOnDispose: true);
			return singleUseContext.Find<Region>(id);
		}

		return context.Find<Region>(id);
	}
}
