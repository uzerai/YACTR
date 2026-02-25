using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes.RouteLikes;

namespace YACTR.Api.Tests.EndpointTests;

[Collection("IntegrationTests")]
public class RouteLikeEntityEndpointsIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{

    [Fact]
    public async Task CreateRouteLike_WithUnauthenticatedClient_ReturnsUnauthorized()
    {
        using var client = fixture.CreateClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var requestData = new RouteLikeRequest(routes.First().Id);

        var (response, created) = await client.POSTAsync<CreateRouteLike, RouteLikeRequest, RouteLikeResponse>(requestData);

        // Assert
        response.IsSuccessStatusCode.ShouldBe(false);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateRouteLike_WithValidData_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var requestData = new RouteLikeRequest(routes.First().Id);

        var (response, created) = await client.POSTAsync<CreateRouteLike, RouteLikeRequest, RouteLikeResponse>(requestData);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        created.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateRouteLike_WithMissingRoute_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Act
        var requestData = new RouteLikeRequest(Guid.CreateVersion7());
        var (response, created) = await client.POSTAsync<CreateRouteLike, RouteLikeRequest, RouteLikeResponse>(requestData);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateRouteLike_WithExistingLike_ReturnsSuccessStatusCode_AndExistingLike()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var requestData = new RouteLikeRequest(routes.First().Id);

        var (firstResponse, firstCreated) = await client.POSTAsync<CreateRouteLike, RouteLikeRequest, RouteLikeResponse>(requestData);
        var (secondResponse, secondCreated) = await client.POSTAsync<CreateRouteLike, RouteLikeRequest, RouteLikeResponse>(requestData);

        // Assert
        secondResponse.IsSuccessStatusCode.ShouldBeTrue();
        secondCreated.ShouldNotBeNull();
        secondCreated.Id.ShouldBe(firstCreated.Id);
    }

    [Fact]
    public async Task DeleteRouteLike_WithExistingLike_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var requestData = new RouteLikeRequest(routes.First().Id);

        var (createResponse, created) = await client.POSTAsync<CreateRouteLike, RouteLikeRequest, RouteLikeResponse>(requestData);
        var (deleteResponse, deleted) = await client.DELETEAsync<CreateRouteLike, RouteLikeRequest, RouteLikeResponse>(requestData);

        // Assert
        deleteResponse.IsSuccessStatusCode.ShouldBeTrue();
        deleted.ShouldNotBeNull();
        deleted.Id.ShouldBe(created.Id);
    }

    [Fact]
    public async Task DeleteRouteLike_WithoutExistingLike_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var requestData = new RouteLikeRequest(routes.First().Id);

        var (deleteResponse, deleted) = await client.DELETEAsync<CreateRouteLike, RouteLikeRequest, RouteLikeResponse>(requestData);

        // Assert
        deleteResponse.IsSuccessStatusCode.ShouldBeFalse();
        deleteResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }
}