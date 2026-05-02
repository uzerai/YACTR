using FastEndpoints.Testing;
using NodaTime;
using Shouldly;
using YACTR.Api.Endpoints.Areas;
using YACTR.Api.Pagination;

namespace YACTR.Api.Tests.EndpointTests.Areas;

[Collection("IntegrationTests")]
public class GetAllAreasIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateClient();
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var (response, result) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new());

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPageAndTotalCount()
    {
        using var client = fixture.CreateClient();

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        for (var i = 0; i < 3; i++)
        {
            await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        }

        var (response, result) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            Page = 2,
            PageSize = 2
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount + 3);
        result.Items.Count.ShouldBe(Math.Clamp(result.TotalCount - 2, 0, 2));
    }

    [Fact]
    public async Task GetAll_WithPageSizeBelowMinimum_ClampsToMinimum()
    {
        using var client = fixture.CreateClient();

        for (var i = 0; i < 3; i++)
        {
            await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        }

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        var (response, result) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            Page = 1,
            PageSize = 0
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount);
        result.Items.Count.ShouldBe(1);
        result.Items.Single().Id.ShouldBe(baselineResult.Items.Single().Id);
    }

    [Fact]
    public async Task GetAll_WithDifferentPages_ReturnsDifferentItems()
    {
        using var client = fixture.CreateClient();

        for (var i = 0; i < 2; i++)
        {
            await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        }

        var (pageOneResponse, pageOneResult) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            Page = 1,
            PageSize = 1
        });

        var (pageTwoResponse, pageTwoResult) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            Page = 2,
            PageSize = 1
        });

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
    public async Task GetAll_WithNameFilter_ReturnsMatchingAreas()
    {
        using var client = fixture.CreateClient();
        var tag = $"Alpine-{Guid.NewGuid():N}";
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Desert-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            Name = tag
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.All(e => e.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithCountryNameFilter_ReturnsMatchingAreas()
    {
        using var client = fixture.CreateClient();
        var tag = $"Alpine-{Guid.NewGuid():N}";
        var (alpineArea, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Desert-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            CountryName = $"{tag} Country"
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == alpineArea.Id);
    }

    [Fact]
    public async Task GetAll_WithCountryIdFilter_ReturnsMatchingAreas()
    {
        using var client = fixture.CreateClient();
        var (alpineArea, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Alpine-{Guid.NewGuid():N}"));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Desert-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            CountryId = alpineArea.CountryId
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == alpineArea.Id);
    }

    [Fact]
    public async Task GetAll_WithCreatedBeforeFilter_ReturnsOlderAreas()
    {
        using var client = fixture.CreateClient();
        var early = Instant.FromUtc(2025, 2, 1, 0, 0);
        var late = Instant.FromUtc(2025, 2, 10, 0, 0);

        var (olderArea, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Older-{Guid.NewGuid():N}", AreaCreatedAt: early));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Newer-{Guid.NewGuid():N}", AreaCreatedAt: late));

        var (response, result) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            CreatedBefore = Instant.FromUtc(2025, 2, 5, 0, 0)
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == olderArea.Id);
    }

    [Fact]
    public async Task GetAll_WithCreatedAfterFilter_ReturnsNewerAreas()
    {
        using var client = fixture.CreateClient();
        var early = Instant.FromUtc(2025, 3, 1, 0, 0);
        var late = Instant.FromUtc(2025, 3, 10, 0, 0);

        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Older-{Guid.NewGuid():N}", AreaCreatedAt: early));
        var (newerArea, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Newer-{Guid.NewGuid():N}", AreaCreatedAt: late));

        var (response, result) = await client.GETAsync<GetAllAreas, GetAllAreasRequest, PaginatedResponse<GetAllAreasResponseItem>>(new()
        {
            CreatedAfter = Instant.FromUtc(2025, 3, 5, 0, 0)
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == newerArea.Id);
    }
}
