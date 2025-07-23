// using YACTR.Data.Model.Location;
// using NetTopologySuite.Geometries;
// using NetTopologySuite;
// using System.Net;
// using YACTR.Endpoints;

// namespace YACTR.Tests.Controllers;

// [Collection("IntegrationTests")]
// public class AreaEntityEndpointsIntegrationTests : IntegrationTestClassFixture
// {    
//     public AreaEntityEndpointsIntegrationTests(TestWebApplicationFactory factory) : base(factory)
//     {
//     }
    
//     [Fact]
//     public async Task GetAll_ReturnsSuccessStatusCode()
//     {
//         using var client = CreateAuthenticatedClient();
//         // Act
//         var response = await client.GetAsync("/areas");
                
//         // Assert
//         response.EnsureSuccessStatusCode();
//     }
    
//     [Fact]
//     public async Task Create_WithValidData_ReturnsCreatedArea()
//     {
//         using var client = CreateAuthenticatedClient();
//         // Arrange
//         var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
//         var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
//         var boundary = geometryFactory.CreateMultiPolygon(new[] {
//             geometryFactory.CreatePolygon(new[] {
//                 new Coordinate(-122.42, 37.77),
//                 new Coordinate(-122.42, 37.78),
//                 new Coordinate(-122.41, 37.78),
//                 new Coordinate(-122.41, 37.77),
//                 new Coordinate(-122.42, 37.77)
//             })
//         });
        
//         var createRequest = new AreaRequestData(
//             "Test Climbing Area",
//             "A beautiful climbing area for testing",
//             location,
//             boundary
//         );

//         var content = SerializeJsonFromRequestData(createRequest);

//         // Act
//         var response = await client.PostAsync("/areas", content);
        
//         // Assert
//         response.EnsureSuccessStatusCode();
//         Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
//         var area = await DeserializeEntityFromResponse<Area>(response);
//         Assert.NotNull(area);
//         Assert.Equal("Test Climbing Area", area.Name);
//         Assert.Equal("A beautiful climbing area for testing", area.Description);
//     }
    
//     [Fact]
//     public async Task GetById_WithValidId_ReturnsArea()
//     {
//         using var client = CreateAuthenticatedClient();
//         // Arrange - First create an area
//         var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
//         var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
//         var boundary = geometryFactory.CreateMultiPolygon(new[] {
//             geometryFactory.CreatePolygon(new[] {
//                 new Coordinate(-122.42, 37.77),
//                 new Coordinate(-122.42, 37.78),
//                 new Coordinate(-122.41, 37.78),
//                 new Coordinate(-122.41, 37.77),
//                 new Coordinate(-122.42, 37.77)
//             })
//         });
        
//         var createRequest = new AreaRequestData(
//             "Test Area for GetById",
//             "Test description",
//             location,
//             boundary
//         );
        
//         var createContent = SerializeJsonFromRequestData(createRequest);
            
//         var createResponse = await client.PostAsync("/areas", createContent);
//         createResponse.EnsureSuccessStatusCode();
        
//         var createdArea = await DeserializeEntityFromResponse<Area>(createResponse);
        
//         // Act
//         var response = await client.GetAsync($"/areas/{createdArea!.Id}");
        
//         // Assert
//         response.EnsureSuccessStatusCode();
        
//         var area = await DeserializeEntityFromResponse<Area>(response);
//         Assert.NotNull(area);
//         Assert.Equal(createdArea.Id, area.Id);
//         Assert.Equal("Test Area for GetById", area.Name);
//     }
    
//     [Fact]
//     public async Task GetById_WithInvalidId_ReturnsNotFound()
//     {
//         // Arrange
//         var invalidId = Guid.NewGuid();
//         using var client = CreateAuthenticatedClient();
//         // Act
//         var response = await client.GetAsync($"/areas/{invalidId}");
        
//         // Assert
//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }
    
//     [Fact]
//     public async Task Update_WithValidData_ReturnsNoContent()
//     {
//         using var client = CreateAuthenticatedClient();
//         // Arrange - First create an area
//         var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
//         var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
//         var boundary = geometryFactory.CreateMultiPolygon(new[] {
//             geometryFactory.CreatePolygon(new[] {
//                 new Coordinate(-122.42, 37.77),
//                 new Coordinate(-122.42, 37.78),
//                 new Coordinate(-122.41, 37.78),
//                 new Coordinate(-122.41, 37.77),
//                 new Coordinate(-122.42, 37.77)
//             })
//         });
        
//         var createRequest = new AreaRequestData(
//             "Test Area for Update",
//             "Original description",
//             location,
//             boundary
//         );
        
//         var createContent = SerializeJsonFromRequestData(createRequest);
            
//         var createResponse = await client.PostAsync("/areas", createContent);
//         createResponse.EnsureSuccessStatusCode();
        
//         var createdArea = await DeserializeEntityFromResponse<Area>(createResponse);
        
//         // Create update request
//         var updateRequest = new AreaRequestData(
//             "Updated Area Name",
//             "Updated description",
//             location,
//             boundary
//         );
        
//         var updateContent = SerializeJsonFromRequestData(updateRequest);
            
//         // Act
//         var response = await client.PutAsync($"/areas/{createdArea!.Id}", updateContent);
        
//         // Assert
//         Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//     }
    
//     [Fact]
//     public async Task Update_WithInvalidId_ReturnsNotFound()
//     {
//         using var client = CreateAuthenticatedClient();
//         // Arrange
//         var invalidId = Guid.NewGuid();
//         var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
//         var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
//         var boundary = geometryFactory.CreateMultiPolygon(new[] {
//             geometryFactory.CreatePolygon(new[] {
//                 new Coordinate(-122.42, 37.77),
//                 new Coordinate(-122.42, 37.78),
//                 new Coordinate(-122.41, 37.78),
//                 new Coordinate(-122.41, 37.77),
//                 new Coordinate(-122.42, 37.77)
//             })
//         });
        
//         var updateRequest = new AreaRequestData(
//             "Updated Name",
//             "Updated description",
//             location,
//             boundary
//         );
        
//         var content = SerializeJsonFromRequestData(updateRequest);
            
//         // Act
//         var response = await client.PutAsync($"/areas/{invalidId}", content);
        
//         // Assert
//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }
    
//     [Fact]
//     public async Task Delete_WithValidId_ReturnsNoContent()
//     {
//         using var client = CreateAuthenticatedClient();
//         // Arrange - First create an area
//         var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
//         var location = geometryFactory.CreatePoint(new Coordinate(-122.4194, 37.7749));
//         var boundary = geometryFactory.CreateMultiPolygon(new[] {
//             geometryFactory.CreatePolygon(new[] {
//                 new Coordinate(-122.42, 37.77),
//                 new Coordinate(-122.42, 37.78),
//                 new Coordinate(-122.41, 37.78),
//                 new Coordinate(-122.41, 37.77),
//                 new Coordinate(-122.42, 37.77)
//             })
//         });
        
//         var createRequest = new AreaRequestData(
//             "Test Area for Delete",
//             "Test description",
//             location,
//             boundary
//         );
        
//         var createContent = SerializeJsonFromRequestData(createRequest);
            
//         var createResponse = await client.PostAsync("/areas", createContent);
//         createResponse.EnsureSuccessStatusCode();
        
//         var createdArea = await DeserializeEntityFromResponse<Area>(createResponse);
        
//         // Act
//         var response = await client.DeleteAsync($"/areas/{createdArea!.Id}");
        
//         // Assert
//         Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        
//         // Verify the area is actually deleted
//         var getResponse = await client.GetAsync($"/areas/{createdArea.Id}");
//         Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
//     }
    
//     [Fact]
//     public async Task Delete_WithInvalidId_ReturnsNotFound()
//     {
//         // Arrange
//         var invalidId = Guid.NewGuid();
//         using var client = CreateAuthenticatedClient();
//         // Act
//         var response = await client.DeleteAsync($"/areas/{invalidId}");
        
//         // Assert
//         Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//     }
// }