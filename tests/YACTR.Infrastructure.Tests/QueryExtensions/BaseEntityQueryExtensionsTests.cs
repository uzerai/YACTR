using NodaTime;
using Shouldly;
using YACTR.Domain.Model;
using YACTR.Infrastructure.Database.QueryExtensions;

namespace YACTR.Infrastructure.Tests.QueryExtensions;

public class BaseEntityQueryExtensionsTests
{
    [Fact]
    public void WhereDeleted_ShouldReturnOnlyDeletedEntities()
    {
        var entities = BuildEntities().AsQueryable();

        var result = entities.WhereDeleted().ToList();

        result.Count.ShouldBe(1);
        result.Single().Id.ShouldBe(Guid.Parse("00000000-0000-0000-0000-000000000002"));
    }

    [Fact]
    public void WhereCreatedAtBefore_ShouldReturnEntitiesBeforeInstant()
    {
        var entities = BuildEntities().AsQueryable();
        var threshold = Instant.FromUnixTimeSeconds(20);

        var result = entities.WhereCreatedAtBefore(threshold).ToList();

        result.Select(x => x.Id).ShouldBe(
        [
            Guid.Parse("00000000-0000-0000-0000-000000000001")
        ]);
    }

    [Fact]
    public void WhereCreatedAtAfter_ShouldReturnEntitiesAfterInstant()
    {
        var entities = BuildEntities().AsQueryable();
        var threshold = Instant.FromUnixTimeSeconds(20);

        var result = entities.WhereCreatedAtAfter(threshold).ToList();

        result.Select(x => x.Id).ShouldBe(
        [
            Guid.Parse("00000000-0000-0000-0000-000000000003")
        ]);
    }

    [Fact]
    public void WhereCreatedAtBetween_ShouldReturnEntitiesWithinInclusiveRange()
    {
        var entities = BuildEntities().AsQueryable();
        var start = Instant.FromUnixTimeSeconds(20);
        var end = Instant.FromUnixTimeSeconds(30);

        var result = entities.WhereCreatedAtBetween(start, end).ToList();

        result.Select(x => x.Id).ShouldBe(
        [
            Guid.Parse("00000000-0000-0000-0000-000000000002"),
            Guid.Parse("00000000-0000-0000-0000-000000000003")
        ]);
    }

    private static List<TestEntity> BuildEntities() =>
    [
        new()
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            CreatedAt = Instant.FromUnixTimeSeconds(10),
            UpdatedAt = Instant.FromUnixTimeSeconds(10),
            DeletedAt = null
        },
        new()
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            CreatedAt = Instant.FromUnixTimeSeconds(20),
            UpdatedAt = Instant.FromUnixTimeSeconds(20),
            DeletedAt = Instant.FromUnixTimeSeconds(21)
        },
        new()
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
            CreatedAt = Instant.FromUnixTimeSeconds(30),
            UpdatedAt = Instant.FromUnixTimeSeconds(30),
            DeletedAt = null
        }
    ];

    private sealed class TestEntity : BaseEntity;
}
