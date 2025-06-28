using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using YACTR.DTO.RequestData;
using YACTR.Data.Model.Location;
using NetTopologySuite.Geometries;
using NetTopologySuite;
using System.Net;
using System.Text.Json.Serialization;
using NetTopologySuite.IO.Converters;

namespace YACTR.IntegrationTests.Controllers;

public class AreaControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;
    
    public AreaControllerIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        // Setup authentication for protected endpoints
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyTestToken==");
        
        // Configure JSON options for NetTopologySuite geometries
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters = { new GeoJsonConverterFactory() },
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 32
        };
    }
    
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/areas");
                
        // Assert
        response.EnsureSuccessStatusCode();
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedArea()
    {
        // Arrange
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var boundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });
        
        var createRequest = new AreaRequestData(
            "Test Climbing Area",
            "A beautiful climbing area for testing",
            location,
            boundary
        );
        
        var content = new StringContent(
            JsonSerializer.Serialize(createRequest, _jsonOptions),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PostAsync("/areas", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var area = JsonSerializer.Deserialize<Area>(responseString, _jsonOptions);
        Assert.NotNull(area);
        Assert.Equal("Test Climbing Area", area.Name);
        Assert.Equal("A beautiful climbing area for testing", area.Description);
    }
    
    [Fact]
    public async Task GetById_WithValidId_ReturnsArea()
    {
        // Arrange - First create an area
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var boundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });
        
        var createRequest = new AreaRequestData(
            "Test Area for GetById",
            "Test description",
            location,
            boundary
        );
        
        var createContent = new StringContent(
            JsonSerializer.Serialize(createRequest, _jsonOptions),
            Encoding.UTF8,
            "application/json");
            
        var createResponse = await _client.PostAsync("/areas", createContent);
        createResponse.EnsureSuccessStatusCode();
        
        var createdArea = JsonSerializer.Deserialize<Area>(
            await createResponse.Content.ReadAsStringAsync(),
            _jsonOptions
        );
        
        // Act
        var response = await _client.GetAsync($"/areas/{createdArea!.Id}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var area = JsonSerializer.Deserialize<Area>(
            await response.Content.ReadAsStringAsync(),
            _jsonOptions
        );
        Assert.NotNull(area);
        Assert.Equal(createdArea.Id, area.Id);
        Assert.Equal("Test Area for GetById", area.Name);
    }
    
    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await _client.GetAsync($"/areas/{invalidId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        // Arrange - First create an area
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var boundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });
        
        var createRequest = new AreaRequestData(
            "Test Area for Update",
            "Original description",
            location,
            boundary
        );
        
        var createContent = new StringContent(
            JsonSerializer.Serialize(createRequest, _jsonOptions),
            Encoding.UTF8,
            "application/json");
            
        var createResponse = await _client.PostAsync("/areas", createContent);
        createResponse.EnsureSuccessStatusCode();
        
        var createdArea = JsonSerializer.Deserialize<Area>(
            await createResponse.Content.ReadAsStringAsync(),
            _jsonOptions
        );
        
        // Create update request
        var updateRequest = new AreaRequestData(
            "Updated Area Name",
            "Updated description",
            location,
            boundary
        );
        
        var updateContent = new StringContent(
            JsonSerializer.Serialize(updateRequest, _jsonOptions),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PutAsync($"/areas/{createdArea!.Id}", updateContent);
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
    
    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var boundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });
        
        var updateRequest = new AreaRequestData(
            "Updated Name",
            "Updated description",
            location,
            boundary
        );
        
        var content = new StringContent(
            JsonSerializer.Serialize(updateRequest, _jsonOptions),
            Encoding.UTF8,
            "application/json");
            
        // Act
        var response = await _client.PutAsync($"/areas/{invalidId}", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
    
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        // Arrange - First create an area
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
        var boundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.42, 37.77),
                new Coordinate(-122.42, 37.78),
                new Coordinate(-122.41, 37.78),
                new Coordinate(-122.41, 37.77),
                new Coordinate(-122.42, 37.77)
            })
        });
        
        var createRequest = new AreaRequestData(
            "Test Area for Delete",
            "Test description",
            location,
            boundary
        );
        
        var createContent = new StringContent(
            JsonSerializer.Serialize(createRequest, _jsonOptions),
            Encoding.UTF8,
            "application/json");
            
        var createResponse = await _client.PostAsync("/areas", createContent);
        createResponse.EnsureSuccessStatusCode();
        
        var createdArea = JsonSerializer.Deserialize<Area>(
            await createResponse.Content.ReadAsStringAsync(),
            _jsonOptions
        );
        
        // Act
        var response = await _client.DeleteAsync($"/areas/{createdArea!.Id}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
        // Verify the area is actually deleted
        var getResponse = await _client.GetAsync($"/areas/{createdArea.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        
        // Act
        var response = await _client.DeleteAsync($"/areas/{invalidId}");
        
        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}