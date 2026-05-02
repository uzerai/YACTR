using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Areas;

namespace YACTR.Api.Tests.EndpointTests.Areas;

[Collection("IntegrationTests")]
public class DeleteAreaIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var (response, _) = await client.DELETEAsync<DeleteArea, DeleteAreaRequest, EmptyResponse>(new(area.Id));
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var (getResponse, _) = await client.GETAsync<GetAreaById, GetAreaByIdRequest, GetAreaByIdResponse>(new(area.Id));
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (response, _) = await client.DELETEAsync<DeleteArea, DeleteAreaRequest, EmptyResponse>(new(Guid.NewGuid()));
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
