using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Areas;
using YACTR.Domain.Model.Authentication;

namespace YACTR.Api.Tests.EndpointTests.Areas;

[Collection("IntegrationTests")]
public class CreateAreaIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedArea()
    {
        using var client = fixture.CreateAuthenticatedClient();
        await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync(new($"CountrySeed-{Guid.NewGuid():N}"));
        var createRequest = new CreateAreaRequest(
            "Test Climbing Area",
            "A beautiful climbing area for testing",
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewMultiPolygon()
        );

        var (response, result) = await client.POSTAsync<CreateArea, CreateAreaRequest, CreateAreaResponse>(createRequest);

        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Name.ShouldBe(createRequest.Name);
        result.Description.ShouldBe(createRequest.Description);
        result.Location.ShouldBe(createRequest.Location);
        result.Boundary.ShouldNotBeNull();
        result.Boundary.IsEmpty.ShouldBeFalse();
    }

    [Fact]
    public async Task Create_WithEmptyName_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var createRequest = new CreateAreaRequest("", "A description", fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.NewMultiPolygon());
        var (response, _) = await client.POSTAsync<CreateArea, CreateAreaRequest, CreateAreaResponse>(createRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithNameTooShort_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var createRequest = new CreateAreaRequest("A", "A description", fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.NewMultiPolygon());
        var (response, _) = await client.POSTAsync<CreateArea, CreateAreaRequest, CreateAreaResponse>(createRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithNameTooLong_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var createRequest = new CreateAreaRequest(new string('x', 256), "A description", fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.NewMultiPolygon());
        var (response, _) = await client.POSTAsync<CreateArea, CreateAreaRequest, CreateAreaResponse>(createRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithDescriptionTooLong_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var createRequest = new CreateAreaRequest("Valid Area Name", new string('x', 1001), fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.NewMultiPolygon());
        var (response, _) = await client.POSTAsync<CreateArea, CreateAreaRequest, CreateAreaResponse>(createRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithEmptyLocation_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var createRequest = new CreateAreaRequest("Valid Area Name", "A description", fixture.TestDataFactory.EmptyPoint(), fixture.TestDataFactory.NewMultiPolygon());
        var (response, _) = await client.POSTAsync<CreateArea, CreateAreaRequest, CreateAreaResponse>(createRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Create_WithEmptyBoundary_ReturnsBadRequest()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var createRequest = new CreateAreaRequest("Valid Area Name", "A description", fixture.TestDataFactory.NewPoint(), fixture.TestDataFactory.EmptyMultiPolygon());
        var (response, _) = await client.POSTAsync<CreateArea, CreateAreaRequest, CreateAreaResponse>(createRequest);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
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
        var createRequest = new CreateAreaRequest(
            "Test Climbing Area",
            "A beautiful climbing area for testing",
            fixture.TestDataFactory.NewPoint(),
            fixture.TestDataFactory.NewMultiPolygon()
        );

        var (response, _) = await client.POSTAsync<CreateArea, CreateAreaRequest, CreateAreaResponse>(createRequest);
        response.IsSuccessStatusCode.ShouldBeFalse();
    }
}
