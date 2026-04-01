using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Routes;

namespace YACTR.Api.Tests.EndpointTests.Routes;

[Collection("IntegrationTests")]
public class DeleteRouteIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, _, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var created = routes.First();

        var (response, _) = await client.DELETEAsync<DeleteRoute, DeleteRouteRequest, EmptyResponse>(new(created.Id));
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var (getResp, _) = await client.GETAsync<GetRouteById, GetRouteByIdRequest, RouteResponse>(new(created.Id));
        getResp.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (response, _) = await client.DELETEAsync<DeleteRoute, DeleteRouteRequest, EmptyResponse>(new(Guid.NewGuid()));
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
