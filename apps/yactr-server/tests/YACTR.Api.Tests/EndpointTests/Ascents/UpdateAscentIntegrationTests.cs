using System.Net;
using FastEndpoints.Testing;
using NodaTime;
using Shouldly;
using YACTR.Api.Endpoints.Ascents;
using YACTR.Domain.Model.Achievement;

namespace YACTR.Api.Tests.EndpointTests.Ascents;

[Collection("IntegrationTests")]
public class UpdateAscentIntegrationTests(ApiTestClassFixture fixture) : AscentEndpointTestsBase(fixture)
{
    [Fact]
    public async Task UpdateAscent_WithValidData_ReturnsNoContent()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - First create an ascent
        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Project,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - Update the ascent
        var newCompletedAt = SystemClock.Instance.GetCurrentInstant();
        var updateRequest = new UpdateAscentRequest(
            AscentId: createdAscent.Id,
            Type: AscentType.Redpoint,
            CompletedAt: newCompletedAt
        );

        var (response, _) = await client.PUTAsync<UpdateAscent, UpdateAscentRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the update
        var getRequest = new GetAscentByIdRequest(createdAscent.Id);
        var (getResponse, updatedAscent) = await client.GETAsync<GetAscentById, GetAscentByIdRequest, AscentResponse>(getRequest);
        getResponse.IsSuccessStatusCode.ShouldBeTrue();
        updatedAscent.Type.ShouldBe(AscentType.Redpoint);
    }

    [Fact]
    public async Task UpdateAscent_WithNonExistentId_ReturnsNotFound()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Act
        var updateRequest = new UpdateAscentRequest(
            AscentId: Guid.NewGuid(),
            Type: AscentType.Tick,
            CompletedAt: SystemClock.Instance.GetCurrentInstant()
        );

        var (response, _) = await client.PUTAsync<UpdateAscent, UpdateAscentRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateAscent_WithDifferentUser_ReturnsForbidden()
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

        var (createResponse, createdAscent) = await clientWithPermissions.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - Try to update with a different user
        var updateRequest = new UpdateAscentRequest(
            AscentId: createdAscent.Id,
            Type: AscentType.Redpoint,
            CompletedAt: SystemClock.Instance.GetCurrentInstant()
        );

        var (response, _) = await clientWithoutPermissions.PUTAsync<UpdateAscent, UpdateAscentRequest, EmptyResponse>(updateRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
