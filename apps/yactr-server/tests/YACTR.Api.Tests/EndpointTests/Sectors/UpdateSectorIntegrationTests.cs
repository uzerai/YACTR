using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Sectors;

namespace YACTR.Api.Tests.EndpointTests.Sectors;

[Collection("IntegrationTests")]
public class UpdateSectorIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var updateRequest = new UpdateSectorRequest
        {
            SectorId = sector.Id,
            Data = new SectorRequestData(
                "Updated Sector",
                fixture.TestDataFactory.NewPolygon(),
                fixture.TestDataFactory.NewPoint(),
                area.Id,
                fixture.TestDataFactory.NewPoint(),
                fixture.TestDataFactory.NewLineString(),
                null,
                null)
        };

        var (response, _) = await client.PUTAsync<UpdateSector, UpdateSectorRequest, EmptyResponse>(updateRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var updateRequest = new UpdateSectorRequest
        {
            SectorId = Guid.NewGuid(),
            Data = new SectorRequestData(
                "",
                fixture.TestDataFactory.NewPolygon(),
                fixture.TestDataFactory.NewPoint(),
                Guid.NewGuid(),
                fixture.TestDataFactory.NewPoint(),
                fixture.TestDataFactory.NewLineString(),
                null,
                null)
        };

        var (response, _) = await client.PUTAsync<UpdateSector, UpdateSectorRequest, EmptyResponse>(updateRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateSector_WithSectorImages_UpdatesOrdersWithoutDuplicatePivotRows()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var image1 = await fixture.TestDataSeeder.CreateImageAsync();
        var image2 = await fixture.TestDataSeeder.CreateImageAsync();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var createRequest = new SectorRequestData(
            "Test Sector With Images For Update",
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            [new SectorImageRequestData(image1.Id, 1), new SectorImageRequestData(image2.Id, 2)],
            image1.Id
        );

        var (createResponse, createdSector) = await client.POSTAsync<CreateSector, SectorRequestData, CreatedSectorResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        var updateRequest = new UpdateSectorRequest
        {
            SectorId = createdSector.Id,
            Data = new SectorRequestData(
                "Updated Sector With Images For Update",
                fixture.TestDataFactory.NewPolygon(),
                fixture.TestDataFactory.NewPoint(),
                area.Id,
                fixture.TestDataFactory.NewPoint(),
                fixture.TestDataFactory.NewLineString(),
                [new SectorImageRequestData(image1.Id, 2), new SectorImageRequestData(image2.Id, 1)],
                image2.Id)
        };

        var (updateResponse, _) = await client.PUTAsync<UpdateSector, UpdateSectorRequest, EmptyResponse>(updateRequest);
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var (getResponse, updatedSector) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, SectorResponse>(new(createdSector.Id));
        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedSector.SectorImages.Count().ShouldBe(2);
        updatedSector.SectorImages.Single(si => si.ImageId == image1.Id).Order.ShouldBe(2);
        updatedSector.SectorImages.Single(si => si.ImageId == image2.Id).Order.ShouldBe(1);
        updatedSector.PrimarySectorImageId.ShouldBe(image2.Id);
    }
}
