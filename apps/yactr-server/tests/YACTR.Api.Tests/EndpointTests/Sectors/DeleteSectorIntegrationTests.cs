using System.Net;

using FastEndpoints.Testing;

using Shouldly;

using YACTR.Api.Endpoints.Sectors;

namespace YACTR.Api.Tests.EndpointTests.Sectors;

[Collection("IntegrationTests")]
public class DeleteSectorIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, sector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var (response, _) = await client.DELETEAsync<DeleteSector, DeleteSectorRequest, EmptyResponse>(new(sector.Id));
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var (getResponse, _) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, SectorResponse>(new(sector.Id));
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (response, _) = await client.DELETEAsync<DeleteSector, DeleteSectorRequest, EmptyResponse>(new(Guid.NewGuid()));
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
