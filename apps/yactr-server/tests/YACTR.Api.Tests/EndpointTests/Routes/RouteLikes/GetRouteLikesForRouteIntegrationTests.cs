using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes;
using YACTR.Api.Endpoints.Routes.RouteLikes;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Authentication;

namespace YACTR.Api.Tests.EndpointTests.Routes.RouteLikes;

[Collection("IntegrationTests")]
public class GetRouteLikesForRouteIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetRouteLikesForRoute_WithExistingRoute_ReturnsPaginatedLikes()
    {
        var secondUser = new User
        {
            Auth0UserId = $"auth0|route-like-second-{Guid.NewGuid()}",
            Email = "route-like-second@test.dev",
            Username = "route-like-second-user"
        };

        using var clientOne = fixture.CreateAuthenticatedClient();
        using var clientTwo = fixture.CreateAuthenticatedClient(secondUser);
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var routeId = routes.First().Id;

        var (createOneResponse, _) = await clientOne.POSTAsync<CreateRouteLike, CreateRouteLikeRequest, CreateRouteLikeResponse>(new(routeId));
        var (createTwoResponse, _) = await clientTwo.POSTAsync<CreateRouteLike, CreateRouteLikeRequest, CreateRouteLikeResponse>(new(routeId));
        createOneResponse.IsSuccessStatusCode.ShouldBeTrue();
        createTwoResponse.IsSuccessStatusCode.ShouldBeTrue();

        using var anonymousClient = fixture.CreateClient();
        var (response, result) = await anonymousClient.GETAsync<GetRouteLikesForRoute, GetRouteLikesForRouteRequest, PaginatedResponse<GetRouteLikesForRouteResponseItem>>(new()
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
    public async Task GetRouteLikesForRoute_WithMissingRoute_ReturnsNotFound()
    {
        using var client = fixture.CreateClient();
        var (response, _) = await client.GETAsync<GetRouteLikesForRoute, GetRouteLikesForRouteRequest, PaginatedResponse<GetRouteLikesForRouteResponseItem>>(new()
        {
            RouteId = Guid.CreateVersion7()
        });

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
