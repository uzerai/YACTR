using System.Net;

using FastEndpoints.Testing;

using Shouldly;

using YACTR.Api.Endpoints.Sectors;

namespace YACTR.Api.Tests.EndpointTests.Sectors;

[Collection("IntegrationTests")]
public class GetSectorByIdIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task GetById_WithValidId_ReturnsSector()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (_, sector, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var (response, result) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, GetSectorByIdResponse>(new(sector.Id));
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(sector.Id);
        result.Name.ShouldBe(sector.Name);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (response, _) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, GetSectorByIdResponse>(new(Guid.NewGuid()));
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetSectorById_WithSectorImages_ReturnsSectorWithImageUrls()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var image1 = await fixture.TestDataSeeder.CreateImageAsync();
        var image2 = await fixture.TestDataSeeder.CreateImageAsync();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var createRequest = new CreateSectorRequest(
            "Test Sector With Images For Get",
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            [new CreateSectorImageRequest(image1.Id, 1), new CreateSectorImageRequest(image2.Id, 2)],
            image1.Id
        );

        var (createResponse, createdSector) = await client.POSTAsync<CreateSector, CreateSectorRequest, CreatedSectorResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        var (response, result) = await client.GETAsync<GetSectorById, GetSectorByIdRequest, GetSectorByIdResponse>(new(createdSector.Id));
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.SectorImages.ShouldNotBeNull();
        result.SectorImages.Count().ShouldBe(2);
        result.SectorImages.All(img => !string.IsNullOrEmpty(img.ImageUrl)).ShouldBeTrue();
    }
}
