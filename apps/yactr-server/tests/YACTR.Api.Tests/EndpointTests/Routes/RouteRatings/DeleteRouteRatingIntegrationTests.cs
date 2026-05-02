using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes.RouteRatings;

namespace YACTR.Api.Tests.EndpointTests.Routes.RouteRatings;

[Collection("IntegrationTests")]
public class DeleteRouteRatingIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task DeleteRouteRating_WithExistingLike_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var routeId = routes.First().Id;

        var (_, created) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, CreateOrUpdateRouteRatingResponse>(new()
        {
            RouteId = routeId,
            RatingData = new CreateOrUpdateRouteRatingData(3)
        });

        var (deleteResponse, deleted) = await client.DELETEAsync<DeleteRouteRating, DeleteRouteRatingRequest, DeleteRouteRatingResponse>(new()
        {
            RouteId = routeId
        });

        deleteResponse.IsSuccessStatusCode.ShouldBeTrue();
        deleted.Id.ShouldBe(created.Id);
    }

    [Fact]
    public async Task DeleteRouteLike_WithoutExistingLike_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var (deleteResponse, _) = await client.DELETEAsync<DeleteRouteRating, DeleteRouteRatingRequest, DeleteRouteRatingResponse>(new()
        {
            RouteId = routes.First().Id
        });

        deleteResponse.IsSuccessStatusCode.ShouldBeFalse();
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
