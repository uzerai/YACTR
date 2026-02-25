using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Areas;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Climbing;

namespace YACTR.Api.Tests.EndpointTests;

[Collection("IntegrationTests")]
public class AreaEntityEndpointsIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    protected override async ValueTask SetupAsync()
    {
        await base.SetupAsync();
    }

    [Fact]
    public async Task GetAll_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateClient();
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
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
        var location = fixture.TestDataSeeder.NewPoint();
        var boundary = fixture.TestDataSeeder.NewMultiPolygon();

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
        result.Name.ShouldBe(createRequest.Name);
        result.Description.ShouldBe(createRequest.Description);
    }

    [Fact]
    public async Task Create_WithoutPermissions_ReturnsUnauthorized()
    {
        User user = new()
        {
            Auth0UserId = "test|test|",
            Email = "test@userwithoutperms.com",
            Username = "user_without_area_permissions",
            PlatformPermissions = []
        };
        using var client = fixture.CreateAuthenticatedClient(user);

        // Arrange
        var location = fixture.TestDataSeeder.NewPoint();
        var boundary = fixture.TestDataSeeder.NewMultiPolygon();

        var createRequest = new AreaRequestData(
            "Test Climbing Area",
            "A beautiful climbing area for testing",
            location,
            boundary
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateArea, AreaRequestData, Area>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsArea()
    {
        // Arrange
        using var client = fixture.CreateAuthenticatedClient();
        var (area, _, _) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var getRequest = new GetAreaByIdRequest(area.Id);
        var (response, result) = await client.GETAsync<GetAreaById, GetAreaByIdRequest, Area>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(area.Id);
        result.Name.ShouldBe(area.Name);
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
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        var updateRequest = new UpdateAreaRequest
        {
            AreaId = area.Id,
            Data = new AreaRequestData(
                "Test Area for Update",
                "Updated description",
                fixture.TestDataSeeder.NewPoint(),
                fixture.TestDataSeeder.NewMultiPolygon()
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
        var location = fixture.TestDataSeeder.NewPoint();
        var boundary = fixture.TestDataSeeder.NewMultiPolygon();

        var updateRequest = new UpdateAreaRequest
        {
            AreaId = invalidId,
            Data = new AreaRequestData(
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
        var (area, sector, route) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var deleteRequest = new DeleteAreaRequest(area.Id);
        var (response, _) = await client.DELETEAsync<DeleteArea, DeleteAreaRequest, EmptyResponse>(deleteRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the area is actually deleted
        var getRequest = new GetAreaByIdRequest(area.Id);
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