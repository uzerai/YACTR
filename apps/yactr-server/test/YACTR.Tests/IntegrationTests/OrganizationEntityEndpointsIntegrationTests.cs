// using YACTR.Data.Model.Organizations;
// using System.Net;
// using YACTR.Endpoints.Organizations;

// namespace YACTR.Tests.Controllers;

// [Collection("IntegrationTests")]
// public class OrganizationEntityEndpointsIntegrationTests : IntegrationTestClassFixture
// {
//     public OrganizationEntityEndpointsIntegrationTests(TestWebApplicationFactory factory) : base(factory)
//     {
//     }
    
//     [Fact]
//     public async Task GetAll_ReturnsSuccessStatusCode()
//     {
//         using var client = CreateAuthenticatedClient();
//         // Act
//         var response = await client.GetAsync("/organizations");
                
//         // Assert
//         response.EnsureSuccessStatusCode();
//     }
    
//     [Fact]
//     public async Task GetAll_WithoutAuthentication_ReturnsUnauthorized()
//     {
//         // Arrange
//         using var client = CreateAnonymousClient();
        
//         // Act
//         var response = await client.GetAsync("/organizations");
        
//         // Assert
//         Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//     }
    
//     [Fact]
//     public async Task Create_WithValidData_ReturnsCreatedOrganization()
//     {
//         // Arrange
//         using var client = CreateAuthenticatedClient();
//         var createRequest = new CreateOrganizationRequestData("Integration Test Org");
        
//         var content = SerializeJsonFromRequestData(createRequest);
            
//         // Act
//         var response = await client.PostAsync("/organizations", content);
        
//         // Assert
//         response.EnsureSuccessStatusCode();
        
//         var organization = await DeserializeEntityFromResponse<Organization>(response);
//         Assert.NotNull(organization);
//         Assert.Equal("Integration Test Org", organization.Name);
//     }
    
//     [Fact]
//     public async Task Create_WithEmptyName_IsAllowed()
//     {
//         // Arrange
//         using var client = CreateAuthenticatedClient();
//         var createRequest = new CreateOrganizationRequestData("");
        
//         var content = SerializeJsonFromRequestData(createRequest);
            
//         // Act
//         var response = await client.PostAsync("/organizations", content);
        
//         // Assert
//         Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//     }
    
//     [Fact]
//     public async Task Create_WithoutAuthentication_ReturnsUnauthorized()
//     {
//         // Arrange
//         using var client = CreateAnonymousClient();
        
//         var createRequest = new CreateOrganizationRequestData("Test Org");
        
//         var content = SerializeJsonFromRequestData(createRequest);
            
//         // Act
//         var response = await client.PostAsync("/organizations", content);
        
//         // Assert
//         Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//     }
    
//     [Fact]
//     public async Task GetById_WithValidId_ReturnsOrganization()
//     {
//         // Arrange - First create an organization
//         using var client = CreateAuthenticatedClient();
//         var createRequest = new CreateOrganizationRequestData("Test Organization for Get");
        
//         var createContent = SerializeJsonFromRequestData(createRequest);
            
//         var createResponse = await client.PostAsync("/organizations", createContent);
//         createResponse.EnsureSuccessStatusCode();
        
//         var createdOrg = await DeserializeEntityFromResponse<Organization>(createResponse);
        
//         // Act
//         var response = await client.GetAsync($"/organizations/{createdOrg.Id}");
        
//         // Assert
//         response.EnsureSuccessStatusCode();
        
//         var organization = await DeserializeEntityFromResponse<Organization>(response);
//         Assert.NotNull(organization);
//         Assert.Equal(createdOrg.Id, organization.Id);
//         Assert.Equal("Test Organization for Get", organization.Name);
//     }
    
//     [Fact]
//     public async Task Get_WithInvalidId_ReturnsForbidden()
//     {
//         // Arrange
//         var invalidId = Guid.NewGuid();
//         using var client = CreateAuthenticatedClient();
        
//         // Act
//         var response = await client.GetAsync($"/organizations/{invalidId}");
        
//         // Assert
//         Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
//     }
    
//     [Fact]
//     public async Task Get_WithoutAuthentication_ReturnsUnauthorized()
//     {
//         // Arrange
//         using var client = CreateAnonymousClient();
        
//         var invalidId = Guid.NewGuid();
        
//         // Act
//         var response = await client.GetAsync($"/organizations/{invalidId}");
        
//         // Assert
//         Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//     }
// }
