using System.Net;

using FastEndpoints.Testing;

using Shouldly;

using YACTR.Api.Endpoints.Routes.RouteLikes;

namespace YACTR.Api.Tests.EndpointTests.Routes.RouteLikes;

[Collection("IntegrationTests")]
public class CreateRouteLikeIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task CreateRouteLike_WithUnauthenticatedClient_ReturnsUnauthorized()
    {
        using var client = fixture.CreateClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var (response, _) = await client.POSTAsync<CreateRouteLike, CreateRouteLikeRequest, CreateRouteLikeResponse>(new(routes.First().Id));
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateRouteLike_WithValidData_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var (response, created) = await client.POSTAsync<CreateRouteLike, CreateRouteLikeRequest, CreateRouteLikeResponse>(new(routes.First().Id));
        response.IsSuccessStatusCode.ShouldBeTrue();
        created.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateRouteLike_WithMissingRoute_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (response, _) = await client.POSTAsync<CreateRouteLike, CreateRouteLikeRequest, CreateRouteLikeResponse>(new(Guid.CreateVersion7()));
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateRouteLike_WithExistingLike_ReturnsSuccessStatusCode_AndExistingLike()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var request = new CreateRouteLikeRequest(routes.First().Id);

        var (_, firstCreated) = await client.POSTAsync<CreateRouteLike, CreateRouteLikeRequest, CreateRouteLikeResponse>(request);
        var (secondResponse, secondCreated) = await client.POSTAsync<CreateRouteLike, CreateRouteLikeRequest, CreateRouteLikeResponse>(request);

        secondResponse.IsSuccessStatusCode.ShouldBeTrue();
        secondCreated.ShouldNotBeNull();
        secondCreated.Id.ShouldBe(firstCreated.Id);
    }
}
