using System.Net;

using NodaTime;

using Shouldly;

using YACTR.Api.Endpoints.Ascents;
using YACTR.Domain.Model.Achievement;

namespace YACTR.Api.Tests.EndpointTests.Ascents;

[Collection("IntegrationTests")]
public class GetAscentByIdIntegrationTests(ApiTestClassFixture fixture) : AscentEndpointTestsBase(fixture)
{
    [Fact]
    public async Task GetAscentById_WithValidAscentId_ReturnsAscent()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - First create an ascent
        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();
        var completedAt = SystemClock.Instance.GetCurrentInstant().Minus(Duration.FromDays(1));

        var createRequest = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Onsight,
            CompletedAt: completedAt
        );

        var (createResponse, createdAscent) = await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(createRequest);
        createResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act
        var getRequest = new GetAscentByIdRequest(createdAscent.Id);
        var (response, result) = await client.GETAsync<GetAscentById, GetAscentByIdRequest, AscentResponse>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Id.ShouldBe(createdAscent.Id);
        result.Type.ShouldBe(AscentType.Onsight);
        result.Route.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAscentById_WithNonExistentId_ReturnsNotFound()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Act
        var getRequest = new GetAscentByIdRequest(Guid.NewGuid());
        var (response, _) = await client.GETAsync<GetAscentById, GetAscentByIdRequest, AscentResponse>(getRequest);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
