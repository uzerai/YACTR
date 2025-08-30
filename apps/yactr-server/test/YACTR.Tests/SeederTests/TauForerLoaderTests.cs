using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Climbing;
using YACTR.Endpoints;
using YACTR.Tests.TestData;

namespace YACTR.Tests.SeederTests;

[Collection("IntegrationTests")]
public class TauForerLoaderTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    [Fact]
    public async Task LoadAndSaveToDatabaseAsync_ShouldSucceed()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
        // Arrange - Load test data directly using TauForerDataLoader
        var (areas, sectors, routes) = await TauForerDataLoader.LoadAndSaveToDatabaseAsync(fixture.DatabaseContext);
        
        // Act & Assert - Verify areas exist
        var (areasResponse, areasFromApi) = await client.GETAsync<GetAllAreas, EmptyRequest, List<Area>>(new());
        areasResponse.IsSuccessStatusCode.ShouldBeTrue();
        areasFromApi.ShouldNotBeNull();
        areasFromApi.Count.ShouldBeGreaterThan(0);
        areasFromApi.Count.ShouldBe(areas.Count());
        
        // Verify sectors exist
        var (sectorsResponse, sectorsFromApi) = await client.GETAsync<GetAllSectors, EmptyRequest, List<Sector>>(new());
        sectorsResponse.IsSuccessStatusCode.ShouldBeTrue();
        sectorsFromApi.ShouldNotBeNull();
        sectorsFromApi.Count.ShouldBeGreaterThan(0);
        sectorsFromApi.Count.ShouldBe(sectors.Count());
        
        // Verify routes exist
        var (routesResponse, routesFromApi) = await client.GETAsync<GetAllRoutes, EmptyRequest, List<Route>>(new());
        routesResponse.IsSuccessStatusCode.ShouldBeTrue();
        routesFromApi.ShouldNotBeNull();
        routesFromApi.Count.ShouldBeGreaterThan(0);
        routesFromApi.Count.ShouldBe(routes.Count());
        
        // Additional verification - check that the data makes sense
        areasFromApi.ShouldAllBe(a => !string.IsNullOrEmpty(a.Name));
        sectorsFromApi.ShouldAllBe(s => !string.IsNullOrEmpty(s.Name));
        routesFromApi.ShouldAllBe(r => !string.IsNullOrEmpty(r.Name));
        
        // Verify relationships exist
        sectorsFromApi.ShouldAllBe(s => areasFromApi.Any(a => a.Id == s.AreaId));
        routesFromApi.ShouldAllBe(r => sectorsFromApi.Any(s => s.Id == r.SectorId));
        
        // Log some statistics for verification
        Console.WriteLine($"Direct loader: Loaded {areas.Count()} areas, {sectors.Count()} sectors, and {routes.Count()} routes from test data");
    }
}
