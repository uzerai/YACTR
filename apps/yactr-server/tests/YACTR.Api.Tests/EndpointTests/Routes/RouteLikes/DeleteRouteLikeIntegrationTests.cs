using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes.RouteLikes;

namespace YACTR.Api.Tests.EndpointTests.Routes.RouteLikes;

[Collection("IntegrationTests")]
public class DeleteRouteLikeIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task DeleteRouteLike_WithExistingLike_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var createRequest = new CreateRouteLikeRequest(routes.First().Id);
        var deleteRequest = new DeleteRouteLikeRequest
        {
            RouteId = routes.First().Id
        };

        var (_, created) = await client.POSTAsync<CreateRouteLike, CreateRouteLikeRequest, CreateRouteLikeResponse>(createRequest);
        var (deleteResponse, deleted) = await client.DELETEAsync<DeleteRouteLike, DeleteRouteLikeRequest, DeleteRouteLikeResponse>(deleteRequest);

        deleteResponse.IsSuccessStatusCode.ShouldBeTrue();
        deleted.ShouldNotBeNull();
        deleted.Id.ShouldBe(created.Id);
    }

    [Fact]
    public async Task DeleteRouteLike_WithoutExistingLike_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var (deleteResponse, _) = await client.DELETEAsync<DeleteRouteLike, DeleteRouteLikeRequest, DeleteRouteLikeResponse>(new()
        {
            RouteId = routes.First().Id
        });

        deleteResponse.IsSuccessStatusCode.ShouldBeFalse();
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
