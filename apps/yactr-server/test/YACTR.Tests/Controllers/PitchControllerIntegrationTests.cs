using YACTR.Data.Model.Location;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System.Net;
using YACTR.Endpoints;

namespace YACTR.Tests.Controllers;

[Collection("IntegrationTests")]
public class PitchControllerIntegrationTests : IntegrationTestClassFixture
{
    public PitchControllerIntegrationTests(TestWebApplicationFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        var client = CreateAuthenticatedClient();
        // Act
        var response = await client.GetAsync("/pitches");
                
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedPitch()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an area and sector
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
        
        var areaRequest = new AreaRequestData(
            "Test Area for Pitch",
            "Test area description",
            areaLocation,
            areaBoundary
        );
        
        var areaContent = SerializeJsonFromRequestData(areaRequest);
            
        var areaResponse = await client.PostAsync("/areas", areaContent);
        areaResponse.EnsureSuccessStatusCode();
        
        var area = await DeserializeEntityFromResponse<Area>(areaResponse);
        
        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));
        
        var sectorRequest = new SectorRequestData(
            "Test Sector for Pitch",
            sectorArea,
            entryPoint,
            null,
            null,
            area!.Id
        );
        
        var sectorContent = SerializeJsonFromRequestData(sectorRequest);
            
        var sectorResponse = await client.PostAsync("/sectors", sectorContent);
        sectorResponse.EnsureSuccessStatusCode();
        
        var sector = await DeserializeEntityFromResponse<Sector>(sectorResponse);
        
        // Create pitch request
        var createRequest = new PitchRequestData(
            sector!.Id,
            "Test Pitch",
            PitchType.Sport,
            "A challenging sport pitch",
            "5.10a"
        );
        
        var content = SerializeJsonFromRequestData(createRequest);
            
        // Act
        var response = await client.PostAsync("/pitches", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var pitch = await DeserializeEntityFromResponse<Pitch>(response);
        Assert.NotNull(pitch);
        Assert.Equal("Test Pitch", pitch.Name);
        Assert.Equal(PitchType.Sport, pitch.Type);
        Assert.Equal("A challenging sport pitch", pitch.Description);
        Assert.Equal(sector.Id, pitch.SectorId);
    }
    
    [Fact]
    public async Task Create_WithDifferentPitchTypes_ReturnsCreatedPitch()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an area and sector
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
        
        var areaRequest = new AreaRequestData(
            "Test Area for Pitch Types",
            "Test area description",
            areaLocation,
            areaBoundary
        );
        
        var areaContent = SerializeJsonFromRequestData(areaRequest);
            
        var areaResponse = await client.PostAsync("/areas", areaContent);
        areaResponse.EnsureSuccessStatusCode();
        
        var area = await DeserializeEntityFromResponse<Area>(areaResponse);
        
        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));
        
        var sectorRequest = new SectorRequestData(
            "Test Sector for Pitch Types",
            sectorArea,
            entryPoint,
            null,
            null,
            area!.Id
        );
        
        var sectorContent = SerializeJsonFromRequestData(sectorRequest);
            
        var sectorResponse = await client.PostAsync("/sectors", sectorContent);
        sectorResponse.EnsureSuccessStatusCode();
        
        var sector = await DeserializeEntityFromResponse<Sector>(sectorResponse);
        
        // Test different pitch types
        var pitchTypes = new[] { PitchType.Traditional, PitchType.Mixed, PitchType.Aid };
        
        foreach (var pitchType in pitchTypes)
        {
            var createRequest = new PitchRequestData(
                sector!.Id,
                $"Test {pitchType} Pitch",
                pitchType,
                $"A {pitchType.ToString().ToLower()} pitch",
                "5.8"
            );
            
            var content = SerializeJsonFromRequestData(createRequest);
                
            // Act
            var response = await client.PostAsync("/pitches", content);
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var pitch = await DeserializeEntityFromResponse<Pitch>(response);
            Assert.NotNull(pitch);
            Assert.Equal(pitchType, pitch.Type);
        }
    }
    
    [Fact]
    public async Task GetById_WithValidId_ReturnsPitch()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an area, sector, and pitch
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
        
        var areaRequest = new AreaRequestData(
            "Test Area for GetById",
            "Test area description",
            areaLocation,
            areaBoundary
        );
        
        var areaContent = SerializeJsonFromRequestData(areaRequest);
            
        var areaResponse = await client.PostAsync("/areas", areaContent);
        areaResponse.EnsureSuccessStatusCode();
        
        var area = await DeserializeEntityFromResponse<Area>(areaResponse);
        
        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));
        
        var sectorRequest = new SectorRequestData(
            "Test Sector for GetById",
            sectorArea,
            entryPoint,
            null,
            null,
            area!.Id
        );
        
        var sectorContent = SerializeJsonFromRequestData(sectorRequest);
            
        var sectorResponse = await client.PostAsync("/sectors", sectorContent);
        sectorResponse.EnsureSuccessStatusCode();
        
        var sector = await DeserializeEntityFromResponse<Sector>(sectorResponse);
        
        var pitchRequest = new PitchRequestData(
            sector!.Id,
            "Test Pitch for GetById",
            PitchType.Sport,
            "Test pitch description",
            "5.9"
        );
        
        var pitchContent = SerializeJsonFromRequestData(pitchRequest);
            
        var pitchResponse = await client.PostAsync("/pitches", pitchContent);
        pitchResponse.EnsureSuccessStatusCode();
        
        var createdPitch = await DeserializeEntityFromResponse<Pitch>(pitchResponse);
        
        // Act
        var response = await client.GetAsync($"/pitches/{createdPitch!.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var pitch = await DeserializeEntityFromResponse<Pitch>(response);
        Assert.NotNull(pitch);
        Assert.Equal(createdPitch.Id, pitch.Id);
        Assert.Equal("Test Pitch for GetById", pitch.Name);
    }
    
    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await client.GetAsync($"/pitches/{invalidId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an area, sector, and pitch
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
        
        var areaRequest = new AreaRequestData(
            "Test Area for Update",
            "Test area description",
            areaLocation,
            areaBoundary
        );
        
        var areaContent = SerializeJsonFromRequestData(areaRequest);
            
        var areaResponse = await client.PostAsync("/areas", areaContent);
        areaResponse.EnsureSuccessStatusCode();
        
        var area = await DeserializeEntityFromResponse<Area>(areaResponse);
        
        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));
        
        var sectorRequest = new SectorRequestData(
            "Test Sector for Update",
            sectorArea,
            entryPoint,
            null,
            null,
            area!.Id
        );
        
        var sectorContent = SerializeJsonFromRequestData(sectorRequest);
            
        var sectorResponse = await client.PostAsync("/sectors", sectorContent);
        sectorResponse.EnsureSuccessStatusCode();
        
        var sector = await DeserializeEntityFromResponse<Sector>(sectorResponse);
        
        var pitchRequest = new PitchRequestData(
            sector!.Id,
            "Test Pitch for Update",
            PitchType.Sport,
            "Original description",
            "5.8"
        );
        
        var pitchContent = SerializeJsonFromRequestData(pitchRequest);
            
        var pitchResponse = await client.PostAsync("/pitches", pitchContent);
        pitchResponse.EnsureSuccessStatusCode();
        
        var createdPitch = await DeserializeEntityFromResponse<Pitch>(pitchResponse);
        
        // Create update request
        var updateRequest = new PitchRequestData(
            sector.Id,
            "Updated Pitch Name",
            PitchType.Traditional,
            "Updated description",
            "5.10a"
        );
        
        var updateContent = SerializeJsonFromRequestData(updateRequest);
            
        // Act
        var response = await client.PutAsync($"/pitches/{createdPitch!.Id}", updateContent);
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidId = Guid.NewGuid();
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));
        
        var sectorRequest = new SectorRequestData(
            "Test Sector",
            sectorArea,
            entryPoint,
            null,
            null,
            Guid.NewGuid()
        );
        
        var updateRequest = new PitchRequestData(
            Guid.NewGuid(),
            "Updated Name",
            PitchType.Sport,
            "Updated description",
            "5.10a"
        );
        
        var content = SerializeJsonFromRequestData(updateRequest);
            
        // Act
        var response = await client.PutAsync($"/pitches/{invalidId}", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        var client = CreateAuthenticatedClient();
        // Arrange - First create an area, sector, and pitch
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
        
        var areaRequest = new AreaRequestData(
            "Test Area for Delete",
            "Test area description",
            areaLocation,
            areaBoundary
        );
        
        var areaContent = SerializeJsonFromRequestData(areaRequest);
            
        var areaResponse = await client.PostAsync("/areas", areaContent);
        areaResponse.EnsureSuccessStatusCode();
        
        var area = await DeserializeEntityFromResponse<Area>(areaResponse);
        
        var sectorArea = geometryFactory.CreatePolygon(new[] {
            new Coordinate(-122.419, 37.774),
            new Coordinate(-122.419, 37.775),
            new Coordinate(-122.418, 37.775),
            new Coordinate(-122.418, 37.774),
            new Coordinate(-122.419, 37.774)
        });
        
        var entryPoint = geometryFactory.CreatePoint(new Coordinate(-122.4185, 37.7745));
        
        var sectorRequest = new SectorRequestData(
            "Test Sector for Delete",
            sectorArea,
            entryPoint,
            null,
            null,
            area!.Id
        );
        
        var sectorContent = SerializeJsonFromRequestData(sectorRequest);
            
        var sectorResponse = await client.PostAsync("/sectors", sectorContent);
        sectorResponse.EnsureSuccessStatusCode();
        
        var sector = await DeserializeEntityFromResponse<Sector>(sectorResponse);
        
        var pitchRequest = new PitchRequestData(
            sector!.Id,
            "Test Pitch for Delete",
            PitchType.Sport,
            "Test pitch description",
            "5.8"
        );
        
        var pitchContent = SerializeJsonFromRequestData(pitchRequest);
            
        var pitchResponse = await client.PostAsync("/pitches", pitchContent);
        pitchResponse.EnsureSuccessStatusCode();
        
        var createdPitch = await DeserializeEntityFromResponse<Pitch>(pitchResponse);
        
        // Act
        var response = await client.DeleteAsync($"/pitches/{createdPitch!.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the pitch is actually deleted
        var getResponse = await client.GetAsync($"/pitches/{createdPitch.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        var client = CreateAuthenticatedClient();
        // Arrange
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await client.DeleteAsync($"/pitches/{invalidId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
} 