using System.Net;

using NodaTime;

using Shouldly;

using YACTR.Api.Endpoints.Ascents;
using YACTR.Domain.Model.Achievement;

namespace YACTR.Api.Tests.EndpointTests.Ascents;

[Collection("IntegrationTests")]
public class DeleteAscentIntegrationTests(ApiTestClassFixture fixture) : AscentEndpointTestsBase(fixture)
{
    [Fact]
    public async Task DeleteAscent_WithValidId_ReturnsDeletedAscent()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - First create an ascent
        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var deleteRequest = new DeleteAscentRequest(createdAscent.Id);
        var (response, result) = await client.DELETEAsync<DeleteAscent, DeleteAscentRequest, DeleteAscentResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdAscent.Id);

        // Verify the ascent is deleted
        var getRequest = new GetAscentByIdRequest(createdAscent.Id);
        var (getResponse, _) = await client.GETAsync<GetAscentById, GetAscentByIdRequest, GetAscentByIdResponse>(getRequest);
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAscent_WithNonExistentId_ReturnsNotFound()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Act
        var deleteRequest = new DeleteAscentRequest(Guid.NewGuid());
        var (response, _) = await client.DELETEAsync<DeleteAscent, DeleteAscentRequest, DeleteAscentResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteAscent_WithDifferentUser_ReturnsForbidden()
    {
        using var clientWithPermissions = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        using var clientWithoutPermissions = Fixture.CreateAuthenticatedClient();

        // Arrange - Create an ascent with the first user
        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await clientWithPermissions.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - Try to delete with a different user
        var deleteRequest = new DeleteAscentRequest(createdAscent.Id);
        var (response, _) = await clientWithoutPermissions.DELETEAsync<DeleteAscent, DeleteAscentRequest, DeleteAscentResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteAscent_WithoutAuthentication_ReturnsUnauthorized()
    {
        // This test covers the branch where the user is not authenticated
        // Arrange - Create an ascent with a valid user first
        using var validClient = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);
        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Tick,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await validClient.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - Try to delete without authentication (this tests the missing branch where ClaimValue returns null)
        var deleteRequest = new DeleteAscentRequest(createdAscent.Id);
        var (response, _) = await Fixture.CreateClient().DELETEAsync<DeleteAscent, DeleteAscentRequest, DeleteAscentResponse>(deleteRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
