using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NSubstitute;
using NUnit.Framework;
using TShockAPI.DB;
using TShockAPI.Models;
using TShockAPI.Services;

namespace TShock.Tests.Services;

[TestFixture]
public class RegionServiceTests
{
	private EntityContext<Region> _context = null!;
	private RegionService _regionService = null!;
	private IDbContextFactory<EntityContext<Region>> _contextFactory = null!;

	[SetUp]
	public void SetUp()
	{
		_context = Substitute.For<EntityContext<Region>>(
			new DbContextOptions<EntityContext<Region>>());
		_contextFactory = Substitute.For<IDbContextFactory<EntityContext<Region>>>();
		_contextFactory.CreateDbContext().Returns(_context);

		_regionService = new RegionService(_contextFactory);
	}

	[TestCase]
	public void GetRegion_GeneratesContext_IfNoneProvided()
	{
		const string REGION_NAME = "region";
		const int WORLD_ID = 1234;
		Region expected = new Region()
		{
			Name = REGION_NAME,
			WorldId = WORLD_ID
		};
		_context.Find<Region>(REGION_NAME, WORLD_ID).Returns(expected);

		Region? actual = _regionService.GetRegion(REGION_NAME, WORLD_ID);

		Assert.AreEqual(expected, actual);
		_contextFactory.Received().CreateDbContext();
	}

	[TestCase]
	public void GetRegion_UsesContext_IfProvided()
	{
		const string REGION_NAME = "region";
		const int WORLD_ID = 1234;
		Region expected = new Region()
		{
			Name = REGION_NAME,
			WorldId = WORLD_ID
		};
		EntityContext<Region> context = _regionService.GetContext();
		context.Find<Region>(REGION_NAME, WORLD_ID).Returns(expected);

		Region? actual = _regionService.GetRegion(REGION_NAME, WORLD_ID, context);

		Assert.AreEqual(expected, actual);
		context.Received().Find<Region>(Arg.Is(REGION_NAME), Arg.Is(WORLD_ID));
	}
}
