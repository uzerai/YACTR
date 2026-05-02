using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Sectors;

namespace YACTR.Api.Tests.EndpointTests.Sectors;

[Collection("IntegrationTests")]
public class CreateSectorIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task CreateSector_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var createRequest = new CreateSectorRequest(
            "Test Sector",
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            null,
            null
        );

        var (response, result) = await client.POSTAsync<CreateSector, CreateSectorRequest, CreatedSectorResponse>(createRequest);
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateSector_WithSectorImages_CreatesSectorWithImages()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var image1 = await fixture.TestDataSeeder.CreateImageAsync();
        var image2 = await fixture.TestDataSeeder.CreateImageAsync();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var createRequest = new CreateSectorRequest(
            "Test Sector With Images",
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            [new CreateSectorImageRequest(image1.Id, 1), new CreateSectorImageRequest(image2.Id, 2)],
            image1.Id
        );

        var (response, result) = await client.POSTAsync<CreateSector, CreateSectorRequest, CreatedSectorResponse>(createRequest);
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.SectorImageIds.Count().ShouldBe(2);
        result.SectorImageIds.ShouldContain(image1.Id);
        result.SectorImageIds.ShouldContain(image2.Id);
        result.PrimarySectorImageId.ShouldBe(image1.Id);
    }

    [Fact]
    public async Task CreateSector_WithSectorImagesButNoPrimary_UsesFirstImageAsPrimary()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var image1 = await fixture.TestDataSeeder.CreateImageAsync();
        var image2 = await fixture.TestDataSeeder.CreateImageAsync();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var createRequest = new CreateSectorRequest(
            "Test Sector With Images No Primary",
            fixture.TestDataFactory.NewPolygon(),
            fixture.TestDataFactory.NewPoint(),
            area.Id,
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewLineString(),
            [new CreateSectorImageRequest(image1.Id, 1), new CreateSectorImageRequest(image2.Id, 2)],
            null
        );

        var (response, result) = await client.POSTAsync<CreateSector, CreateSectorRequest, CreatedSectorResponse>(createRequest);
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.SectorImageIds.Count().ShouldBe(2);
        result.PrimarySectorImageId.ShouldBe(image1.Id);
    }
}
