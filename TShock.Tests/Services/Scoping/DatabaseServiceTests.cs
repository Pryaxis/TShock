using Microsoft.EntityFrameworkCore;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using TShockAPI.DB;
using TShockAPI.Services.Scoping;

namespace TShock.Tests.Services.Scoping;

[TestFixture]
public class DatabaseServiceTests
{
	public class TestEntity { }

	private class TestDatabaseService : DatabaseService<TestEntity>
	{
		public TestDatabaseService(IDbContextFactory<EntityContext<TestEntity>> contextFactory)
			: base(contextFactory) { }

		public void DoContextSafeFunc(TestEntity entity, EntityContext<TestEntity>? context = null)
		{
			ContextSafeFunc(ctx => ctx.Find<TestEntity>(entity),
				context,
				saveOnDispose: false);
		}
	}

	private EntityContext<TestEntity> _context = null!;
	private TestDatabaseService _databaseService = null!;
	private IDbContextFactory<EntityContext<TestEntity>> _contextFactory = null!;

	[SetUp]
	public void SetUp()
	{
		_context = Substitute.For<EntityContext<TestEntity>>(
			new DbContextOptions<EntityContext<TestEntity>>());
		_contextFactory =
			Substitute.For<IDbContextFactory<EntityContext<TestEntity>>>();
		_contextFactory.CreateDbContext().Returns(_context);

		_databaseService = new TestDatabaseService(_contextFactory);
	}

	[TestCase]
	public void GetContext_CreatesContext()
	{
		EntityContext<TestEntity> context = _databaseService.GetContext(saveOnDispose: true);
		Assert.True(context.SaveOnDispose);
		_contextFactory.Received().CreateDbContext();
	}

	[TestCase]
	public void ContextSafeFunc_UsesContext_IfProvided()
	{
		TestEntity entity = new();
		EntityContext<TestEntity> context = _databaseService.GetContext(saveOnDispose: true);
		_databaseService.DoContextSafeFunc(entity, context);
		context.Received().Find<TestEntity>(Arg.Is(entity));
	}

	[TestCase]
	public void ContextSafeFunc_GeneratesContext_IfNoneProvided()
	{
		TestEntity entity = new();
		_databaseService.DoContextSafeFunc(entity, context: null);
		_contextFactory.Received().CreateDbContext();
		_context.Received().Find<TestEntity>(Arg.Is(entity));
	}
}
