using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Areas;

namespace YACTR.Api.Tests.EndpointTests.Areas;

[Collection("IntegrationTests")]
public class UpdateAreaIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var updateRequest = new UpdateAreaRequest
        {
            AreaId = area.Id,
            Data = new UpdateAreaBody("Test Area for Update", "Updated description", fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.NewMultiPolygon())
        };

        var (response, _) = await client.PUTAsync<UpdateArea, UpdateAreaRequest, EmptyResponse>(updateRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var updateRequest = new UpdateAreaRequest
        {
            AreaId = Guid.NewGuid(),
            Data = new UpdateAreaBody("Non-existent Area", "Doesn't matter", fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.NewMultiPolygon())
        };

        var (response, _) = await client.PUTAsync<UpdateArea, UpdateAreaRequest, EmptyResponse>(updateRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_WithEmptyName_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var updateRequest = new UpdateAreaRequest
        {
            AreaId = area.Id,
            Data = new UpdateAreaBody("", "Updated description", fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.NewMultiPolygon())
        };

        var (response, _) = await client.PUTAsync<UpdateArea, UpdateAreaRequest, EmptyResponse>(updateRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_WithEmptyBoundary_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var updateRequest = new UpdateAreaRequest
        {
            AreaId = area.Id,
            Data = new UpdateAreaBody("Valid Area Name", "Updated description", fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.EmptyMultiPolygon())
        };

        var (response, _) = await client.PUTAsync<UpdateArea, UpdateAreaRequest, EmptyResponse>(updateRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
