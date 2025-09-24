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
public class AreaEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{    
    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
        // Act
        var (response, result) = await client.GETAsync<GetAllAreas, EmptyRequest, List<Area>>(new());
                
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedArea()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
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

        // Act
        var (response, result) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(createRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Test Climbing Area");
        result.Description.ShouldBe("A beautiful climbing area for testing");
    }
    
    [Fact]
    public async Task GetById_WithValidId_ReturnsArea()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
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
        
        var (createResponse, createdArea) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        // Act
        var getRequest = new GetAreaByIdRequest(createdArea.Id);
        var (response, result) = await client.GETAsync<GetAreaById, GetAreaByIdRequest, Area>(getRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdArea.Id);
        result.Name.ShouldBe("Test Area for GetById");
    }
    
    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();
        
        // Act
        var getRequest = new GetAreaByIdRequest(invalidId);
        var (response, _) = await client.GETAsync<GetAreaById, GetAreaByIdRequest, Area>(getRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Update_WithValidData_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
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
        
        var (createResponse, createdArea) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        // Act
        var updatedLocation = geometryFactory.CreatePoint(new Coordinate(-122.4195, 37.775));
        var updatedBoundary = geometryFactory.CreateMultiPolygon(new[] {
            geometryFactory.CreatePolygon(new[] {
                new Coordinate(-122.421, 37.771),
                new Coordinate(-122.421, 37.781),
                new Coordinate(-122.411, 37.781),
                new Coordinate(-122.411, 37.771),
                new Coordinate(-122.421, 37.771)
            })
        });

        var updateRequest = new UpdateAreaRequest
        {
            AreaId = createdArea.Id,
            Area = new AreaRequestData(
                "Test Area for Update",
                "Updated description",
                updatedLocation,
                updatedBoundary
            )
        };

        var (response, _) = await client.PUTAsync<UpdateArea, UpdateAreaRequest, EmptyResponse>(updateRequest);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    
    [Fact]
    public async Task Update_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();
            
        // Act
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

        var updateRequest = new UpdateAreaRequest
        {
            AreaId = invalidId,
            Area = new AreaRequestData(
                "Non-existent Area",
                "Doesn't matter",
                location,
                boundary
            )
        };

        var (response, _) = await client.PUTAsync<UpdateArea, UpdateAreaRequest, EmptyResponse>(updateRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Delete_WithValidId_ReturnsNoContent()
    {
        using var client = fixture.CreateAuthenticatedClient();
        
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
        
        var (createResponse, createdArea) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();
        
        // Act
        var deleteRequest = new DeleteAreaRequest(createdArea.Id);
        var (response, _) = await client.DELETEAsync<DeleteArea, DeleteAreaRequest, EmptyResponse>(deleteRequest);
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        
        // Verify the area is actually deleted
        var getRequest = new GetAreaByIdRequest(createdArea.Id);
        var (getResponse, _) = await client.GETAsync<GetAreaById, GetAreaByIdRequest, Area>(getRequest);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task Delete_WithInvalidId_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var invalidId = Guid.NewGuid();
        
        // Act
        var deleteRequest = new DeleteAreaRequest(invalidId);
        var (response, _) = await client.DELETEAsync<DeleteArea, DeleteAreaRequest, EmptyResponse>(deleteRequest);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}