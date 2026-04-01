using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes.RouteRatings;

namespace YACTR.Api.Tests.EndpointTests.Routes.RouteRatings;

[Collection("IntegrationTests")]
public class CreateOrUpdateRouteRatingIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task CreateRouteRating_WithUnauthenticatedClient_ReturnsUnauthorized()
    {
        using var client = fixture.CreateClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var (response, _) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(new()
        {
            RouteId = routes.First().Id,
            RatingData = new(3)
        });

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateRouteRating_WithValidData_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var (response, created) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(new()
        {
            RouteId = routes.First().Id,
            RatingData = new(3)
        });
        response.IsSuccessStatusCode.ShouldBeTrue();
        created.Rating.ShouldBe(3);
    }

    [Fact]
    public async Task CreateRouteRating_WithMissingRoute_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (response, _) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(new()
        {
            RouteId = Guid.CreateVersion7(),
            RatingData = new(3)
        });
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateRouteRating_WithExistingRating_ReturnsSuccessStatusCode_AndUpdatedRating()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var routeId = routes.First().Id;

        var (_, firstCreated) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(new()
        {
            RouteId = routeId,
            RatingData = new(1)
        });

        var (secondResponse, secondCreated) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(new()
        {
            RouteId = routeId,
            RatingData = new(3)
        });

        secondResponse.IsSuccessStatusCode.ShouldBeTrue();
        secondCreated.ShouldNotBeNull();
        secondCreated.Id.ShouldBe(firstCreated.Id);
        secondCreated.Rating.ShouldBe(3);
    }
}
