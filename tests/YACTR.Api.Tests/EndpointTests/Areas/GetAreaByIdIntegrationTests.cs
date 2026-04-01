using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Areas;

namespace YACTR.Api.Tests.EndpointTests.Areas;

[Collection("IntegrationTests")]
public class GetAreaByIdIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetById_WithValidId_ReturnsArea()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var (response, result) = await client.GETAsync<GetAreaById, GetAreaByIdRequest, AreaResponse>(new(area.Id));

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(area.Id);
        result.Name.ShouldBe(area.Name);
        result.Location.ShouldBe(area.Location);
        result.Boundary.ShouldNotBeNull();
        result.Boundary.IsEmpty.ShouldBeFalse();
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (response, _) = await client.GETAsync<GetAreaById, GetAreaByIdRequest, AreaResponse>(new(Guid.NewGuid()));
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
