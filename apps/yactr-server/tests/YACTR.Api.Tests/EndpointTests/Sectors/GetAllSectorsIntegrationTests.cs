using FastEndpoints.Testing;

using NodaTime;

using Shouldly;

using YACTR.Api.Endpoints.Sectors;
using YACTR.Api.Pagination;

namespace YACTR.Api.Tests.EndpointTests.Sectors;

[Collection("IntegrationTests")]
public class GetAllSectorsIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"GetAll-{Guid.NewGuid():N}";
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { Name = tag });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPageAndTotalCount()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Pagination-{Guid.NewGuid():N}";
        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { Name = tag, Page = 1, PageSize = 1 });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        for (var i = 0; i < 3; i++)
        {
            await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"{tag}-{i}"));
        }

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { Name = tag, Page = 2, PageSize = 2 });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount + 3);
        result.Items.Count.ShouldBe(Math.Clamp(result.TotalCount - 2, 0, 2));
    }

    [Fact]
    public async Task GetAll_WithPageSizeBelowMinimum_ClampsToMinimum()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Clamp-{Guid.NewGuid():N}";
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { Name = tag, Page = 1, PageSize = 1 });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { Name = tag, Page = 1, PageSize = 0 });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount);
        result.Items.Count.ShouldBe(1);
        result.Items.Single().Id.ShouldBe(baselineResult.Items.Single().Id);
    }

    [Fact]
    public async Task GetAll_WithDifferentPages_ReturnsDifferentItems()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"PageDiff-{Guid.NewGuid():N}";
        for (var i = 0; i < 2; i++)
        {
            await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"{tag}-{i}"));
        }

        var (pageOneResponse, pageOneResult) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { Name = tag, Page = 1, PageSize = 1 });
        var (pageTwoResponse, pageTwoResult) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { Name = tag, Page = 2, PageSize = 1 });

        pageOneResponse.IsSuccessStatusCode.ShouldBeTrue();
        pageTwoResponse.IsSuccessStatusCode.ShouldBeTrue();
        pageOneResult.ShouldNotBeNull();
        pageTwoResult.ShouldNotBeNull();
        pageOneResult.TotalCount.ShouldBe(pageTwoResult.TotalCount);
        pageOneResult.Items.Count.ShouldBe(1);
        pageTwoResult.Items.Count.ShouldBe(1);
        pageOneResult.Items.Single().Id.ShouldNotBe(pageTwoResult.Items.Single().Id);
    }

    [Fact]
    public async Task GetAll_WithNameFilter_ReturnsMatchingSectors()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Granite-{Guid.NewGuid():N}";
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Limestone-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { Name = tag });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.All(e => e.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithAreaNameFilter_ReturnsMatchingSectors()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Granite-{Guid.NewGuid():N}";
        var (_, graniteSector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Limestone-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { AreaName = $"{tag} Area" });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == graniteSector.Id);
    }

    [Fact]
    public async Task GetAll_WithAreaIdFilter_ReturnsMatchingSectors()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (targetArea, targetSector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Target-{Guid.NewGuid():N}"));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Other-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { AreaId = targetArea.Id });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == targetSector.Id);
    }

    [Fact]
    public async Task GetAll_WithCreatedBeforeFilter_ReturnsOlderSectors()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var older = Instant.FromUtc(2025, 4, 1, 0, 0);
        var newer = Instant.FromUtc(2025, 4, 5, 0, 0);

        var (_, olderSector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Older-{Guid.NewGuid():N}", SectorCreatedAt: older));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Newer-{Guid.NewGuid():N}", SectorCreatedAt: newer));

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { CreatedBefore = Instant.FromUtc(2025, 4, 3, 0, 0) });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == olderSector.Id);
    }

    [Fact]
    public async Task GetAll_WithCreatedAfterFilter_ReturnsNewerSectors()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var older = Instant.FromUtc(2025, 5, 1, 0, 0);
        var newer = Instant.FromUtc(2025, 5, 5, 0, 0);

        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Older-{Guid.NewGuid():N}", SectorCreatedAt: older));
        var (_, newerSector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Newer-{Guid.NewGuid():N}", SectorCreatedAt: newer));

        var (response, result) = await client.GETAsync<GetAllSectors, GetAllSectorsRequest, PaginatedResponse<GetAllSectorsResponseItem>>(new() { CreatedAfter = Instant.FromUtc(2025, 5, 3, 0, 0) });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == newerSector.Id);
    }
}
