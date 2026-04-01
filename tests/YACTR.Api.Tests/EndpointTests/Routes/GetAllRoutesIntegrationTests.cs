using FastEndpoints.Testing;
using NodaTime;
using Shouldly;
using YACTR.Api.Endpoints.Routes;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests.Routes;

[Collection("IntegrationTests")]
public class GetAllRoutesIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new());
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAll_WithPagination_ReturnsRequestedPageAndTotalCount()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { Page = 1, PageSize = 1 });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { Page = 2, PageSize = 2 });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount + routes.Count);
        result.Items.Count.ShouldBe(Math.Clamp(result.TotalCount - 2, 0, 2));
    }

    [Fact]
    public async Task GetAll_WithPageSizeBelowMinimum_ClampsToMinimum()
    {
        using var client = fixture.CreateAuthenticatedClient();
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { Page = 1, PageSize = 1 });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { Page = 1, PageSize = 0 });
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
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var (pageOneResponse, pageOneResult) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { Page = 1, PageSize = 1 });
        var (pageTwoResponse, pageTwoResult) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { Page = 2, PageSize = 1 });
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
    public async Task GetAll_WithNameFilter_ReturnsMatchingRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Granite-{Guid.NewGuid():N}";
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Limestone-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { Name = $"{tag} Route" });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.All(e => e.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithSectorNameFilter_ReturnsMatchingRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Granite-{Guid.NewGuid():N}";
        var (_, _, targetRoutes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Limestone-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { SectorName = $"{tag} Sector" });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == targetRoutes.First().Id);
        result.Items.All(e => e.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithSectorIdFilter_ReturnsMatchingRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Target-{Guid.NewGuid():N}";
        var (_, sector, targetRoutes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Other-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { SectorId = sector.Id });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == targetRoutes.First().Id);
        result.Items.All(e => e.SectorId == sector.Id).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithAreaNameFilter_ReturnsMatchingRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Target-{Guid.NewGuid():N}";
        var (_, _, targetRoutes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Other-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { AreaName = $"{tag} Area" });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == targetRoutes.First().Id);
        result.Items.All(e => e.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithAreaIdFilter_ReturnsMatchingRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Target-{Guid.NewGuid():N}";
        var (targetArea, _, targetRoutes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Other-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { AreaId = targetArea.Id });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == targetRoutes.First().Id);
        result.Items.All(e => e.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithCountryNameFilter_ReturnsMatchingRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Target-{Guid.NewGuid():N}";
        var (_, _, targetRoutes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Other-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { CountryName = $"{tag} Country" });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == targetRoutes.First().Id);
        result.Items.All(e => e.Name.Contains(tag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithCountryIdFilter_ReturnsMatchingRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var tag = $"Target-{Guid.NewGuid():N}";
        var (targetArea, _, targetRoutes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(tag));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Other-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { CountryId = targetArea.CountryId });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == targetRoutes.First().Id);
    }

    [Fact]
    public async Task GetAll_WithTypeFilter_ReturnsMatchingRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Target-{Guid.NewGuid():N}"));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Other-{Guid.NewGuid():N}"));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { Type = ClimbingType.Sport });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.Items.All(e => e.Type == ClimbingType.Sport).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithCreatedBeforeFilter_ReturnsOlderRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var older = Instant.FromUtc(2025, 6, 1, 0, 0);
        var newer = Instant.FromUtc(2025, 6, 5, 0, 0);

        var olderTag = $"Older-{Guid.NewGuid():N}";
        var (_, _, olderRoutes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(olderTag, FirstRouteCreatedAt: older));
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Newer-{Guid.NewGuid():N}", FirstRouteCreatedAt: newer));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { CreatedBefore = Instant.FromUtc(2025, 6, 3, 0, 0) });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == olderRoutes.First().Id);
        result.Items.All(e => e.Name.Contains(olderTag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }

    [Fact]
    public async Task GetAll_WithCreatedAfterFilter_ReturnsNewerRoutes()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var older = Instant.FromUtc(2025, 7, 1, 0, 0);
        var newer = Instant.FromUtc(2025, 7, 5, 0, 0);

        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"Older-{Guid.NewGuid():N}", FirstRouteCreatedAt: older));
        var newerTag = $"Newer-{Guid.NewGuid():N}";
        var (_, _, newerRoutes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new(newerTag, FirstRouteCreatedAt: newer));

        var (response, result) = await client.GETAsync<GetAllRoutes, GetAllRoutesRequest, PaginatedResponse<RouteResponse>>(new() { CreatedAfter = Instant.FromUtc(2025, 7, 3, 0, 0) });
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Items.ShouldContain(e => e.Id == newerRoutes.First().Id);
        result.Items.All(e => e.Name.Contains(newerTag, StringComparison.OrdinalIgnoreCase)).ShouldBeTrue();
    }
}
