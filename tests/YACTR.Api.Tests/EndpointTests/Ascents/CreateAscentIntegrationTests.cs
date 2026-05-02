using System.Net;
using NodaTime;
using Shouldly;
using YACTR.Api.Endpoints.Ascents;
using YACTR.Domain.Model.Achievement;

namespace YACTR.Api.Tests.EndpointTests.Ascents;

[Collection("IntegrationTests")]
public class CreateAscentIntegrationTests(ApiTestClassFixture fixture) : AscentEndpointTestsBase(fixture)
{
    [Fact]
    public async Task CreateAscent_WithValidAscentData_ReturnsCreatedAscent()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange
        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Redpoint,
            CompletedAt: completedAt
        );

        // Act
        var (response, result) = await client.POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.Type.ShouldBe(AscentType.Redpoint);
        result.UserId.ShouldBe(TestUserWithAscentPermissions.Id);
        result.Route.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateAscent_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var createRequest = new CreateAscentRequest(
            RouteId: Guid.NewGuid(),
            Type: AscentType.Tick,
            CompletedAt: SystemClock.Instance.GetCurrentInstant()
        );

        // Act
        var (response, _) = await Fixture.CreateClient().POSTAsync<CreateAscent, CreateAscentRequest, CreateAscentResponse>(createRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
