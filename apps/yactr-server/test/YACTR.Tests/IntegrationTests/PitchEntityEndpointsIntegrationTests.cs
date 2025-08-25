using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Climbing;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using YACTR.Endpoints;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class PitchEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
    private async Task<(Area area, Sector sector)> CreateAreaAndSectorAsync(HttpClient client, string areaName = "Test Area", string sectorName = "Test Sector")
    {
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var areaLocation = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var areaBoundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });
        
        var areaRequest = new AreaRequestData(areaName, "Test area description", areaLocation, areaBoundary);
        var (areaResponse, area) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(areaRequest);
        areaResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));
        var sectorRequest = new SectorRequestData(sectorName, sectorArea, entryPoint, null, null, area.Id);
        var (sectorResponse, sector) = await client.POSTAsync<CreateSector, SectorRequestData, Sector>(sectorRequest);
        sectorResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        return (area, sector);
    }
    
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
        // Act
        var (response, result) = await client.GETAsync<GetAllPitches, EmptyRequest, List<Pitch>>(new());
                
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedPitch()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
        // Arrange - First create an area and sector
        var (area, sector) = await CreateAreaAndSectorAsync(client, "Test Area for Pitch", "Test Sector for Pitch");
        
        // Create pitch request
        var createRequest = new PitchRequestData(
            sector.Id,
            "Test Pitch",
            ClimbingType.Sport,
            "A challenging sport pitch",
            "5.10a"
        );
            
        // Act
        var (response, result) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test Pitch");
        result.Type.ShouldBe(ClimbingType.Sport);
        result.Description.ShouldBe("A challenging sport pitch");
        result.SectorId.ShouldBe(sector.Id);
    }
    
    [Fact]
    public async Task Create_WithDifferentPitchTypes_ReturnsCreatedPitch()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
        // Arrange - First create an area and sector
        var (area, sector) = await CreateAreaAndSectorAsync(client, "Test Area for Pitch Types", "Test Sector for Pitch Types");
        
        // Test different pitch types
        var pitchTypes = new[] { ClimbingType.Traditional, ClimbingType.Mixed, ClimbingType.Aid };
        
        foreach (var pitchType in pitchTypes)
        {
            var createRequest = new PitchRequestData(
                sector.Id,
                $"Test {pitchType} Pitch",
                pitchType,
                $"A {pitchType.ToString().ToLower()} pitch",
                "5.8"
            );
                
            // Act
            var (response, result) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);
            
            // Assert
            response.IsSuccessStatusCode.ShouldBeTrue();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            result.ShouldNotBeNull();
            result.Type.ShouldBe(pitchType);
        }
    }
    
    [Fact]
    public async Task GetById_WithValidId_ReturnsPitch()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
        // Arrange - First create an area, sector, and pitch
        var (area, sector) = await CreateAreaAndSectorAsync(client, "Test Area for GetById", "Test Sector for GetById");
        
        var createRequest = new PitchRequestData(
            sector.Id,
            "Test Pitch for GetById",
            ClimbingType.Sport,
            "Test description",
            "5.9"
        );
        
        var (createResponse, createdPitch) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        // Act
        var getRequest = new GetPitchByIdRequest(createdPitch.Id);
        var (response, result) = await client.GETAsync<GetPitchById, GetPitchByIdRequest, Pitch>(getRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdPitch.Id);
        result.Name.ShouldBe("Test Pitch for GetById");
    }
    
    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();
        
        // Act
        var getRequest = new GetPitchByIdRequest(invalidId);
        var (response, _) = await client.GETAsync<GetPitchById, GetPitchByIdRequest, Pitch>(getRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
        // Arrange - First create an area, sector, and pitch
        var (area, sector) = await CreateAreaAndSectorAsync(client, "Test Area for Update", "Test Sector for Update");
        
        var createRequest = new PitchRequestData(
            sector.Id,
            "Test Pitch for Update",
            ClimbingType.Sport,
            "Original description",
            "5.8"
        );
        
        var (createResponse, createdPitch) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        // Act
        var updateRequest = new UpdatePitchRequest(createdPitch.Id);
        var (response, _) = await client.PUTAsync<UpdatePitch, UpdatePitchRequest, EmptyResponse>(updateRequest);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();
            
        // Act
        var updateRequest = new UpdatePitchRequest(invalidId);
        var (response, _) = await client.PUTAsync<UpdatePitch, UpdatePitchRequest, EmptyResponse>(updateRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
        // Arrange - First create an area, sector, and pitch
        var (area, sector) = await CreateAreaAndSectorAsync(client, "Test Area for Delete", "Test Sector for Delete");
        
        var createRequest = new PitchRequestData(
            sector.Id,
            "Test Pitch for Delete",
            ClimbingType.Sport,
            "Test description",
            "5.7"
        );
        
        var (createResponse, createdPitch) = await client.POSTAsync<CreatePitch, PitchRequestData, Pitch>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        // Act
        var deleteRequest = new DeletePitchRequest(createdPitch.Id);
        var (response, _) = await client.DELETEAsync<DeletePitch, DeletePitchRequest, EmptyResponse>(deleteRequest);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        // Verify the pitch is actually deleted
        var getRequest = new GetPitchByIdRequest(createdPitch.Id);
        var (getResponse, _) = await client.GETAsync<GetPitchById, GetPitchByIdRequest, Pitch>(getRequest);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();
        
        // Act
        var deleteRequest = new DeletePitchRequest(invalidId);
        var (response, _) = await client.DELETEAsync<DeletePitch, DeletePitchRequest, EmptyResponse>(deleteRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
} 