using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes;
using YACTR.Api.Endpoints.Routes.RouteRatings;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Authentication;

namespace YACTR.Api.Tests.EndpointTests.Routes.RouteRatings;

[Collection("IntegrationTests")]
public class GetRouteRatingsForRouteIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetRouteRatingsForRoute_WithExistingRoute_ReturnsPaginatedRatings()
    {
        var secondUser = new User
        {
            Auth0UserId = $"auth0|route-rating-second-{Guid.NewGuid()}",
            Email = "route-rating-second@test.dev",
            Username = "route-rating-second-user"
        };

        using var clientOne = fixture.CreateAuthenticatedClient();
        using var clientTwo = fixture.CreateAuthenticatedClient(secondUser);
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var routeId = routes.First().Id;

        var (createOneResponse, _) = await clientOne.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, CreateOrUpdateRouteRatingResponse>(new()
        {
            RouteId = routeId,
            RatingData = new CreateOrUpdateRouteRatingData(2)
        });
        var (createTwoResponse, _) = await clientTwo.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, CreateOrUpdateRouteRatingResponse>(new()
        {
            RouteId = routeId,
            RatingData = new CreateOrUpdateRouteRatingData(4)
        });
        createOneResponse.IsSuccessStatusCode.ShouldBeTrue();
        createTwoResponse.IsSuccessStatusCode.ShouldBeTrue();

        using var anonymousClient = fixture.CreateClient();
        var (response, result) = await anonymousClient.GETAsync<GetRouteRatingsForRoute, GetRouteRatingsForRouteRequest, PaginatedResponse<GetRouteRatingsForRouteResponseItem>>(new()
        {
            RouteId = routeId
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(2);
        result.Items.Count.ShouldBe(2);
        result.Items.All(e => e.RouteId == routeId).ShouldBeTrue();
    }

    [Fact]
    public async Task GetRouteRatingsForRoute_WithMissingRoute_ReturnsNotFound()
    {
        using var client = fixture.CreateClient();
        var (response, _) = await client.GETAsync<GetRouteRatingsForRoute, GetRouteRatingsForRouteRequest, PaginatedResponse<GetRouteRatingsForRouteResponseItem>>(new()
        {
            RouteId = Guid.CreateVersion7()
        });

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
