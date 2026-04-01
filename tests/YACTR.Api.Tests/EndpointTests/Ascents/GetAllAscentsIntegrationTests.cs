using FastEndpoints.Testing;
using NodaTime;
using Shouldly;
using YACTR.Api.Endpoints.Ascents;
using YACTR.Domain.Model.Achievement;

namespace YACTR.Api.Tests.EndpointTests.Ascents;

[Collection("IntegrationTests")]
public class GetAllAscentsIntegrationTests(ApiTestClassFixture fixture) : AscentEndpointTestsBase(fixture)
{
    [Fact]
    public async Task GetAllAscents_ReturnsSuccessStatusCode()
    {
        using var client = Fixture.CreateClient();

        // Act
        var (response, result) = await client.GETAsync<GetAllAscents, EmptyRequest, List<AscentResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAllAscents_WithMultipleAscents_ReturnsAllAscentsOrderedByCompletedAt()
    {
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithAscentPermissions);

        // Arrange - Create test data and multiple ascents
        var (_, _, routes) = await Fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();
        var route = routes.First();

        var now = SystemClock.Instance.GetCurrentInstant();
        var yesterday = now.Minus(Duration.FromDays(1));
        var twoDaysAgo = now.Minus(Duration.FromDays(2));

        // Create ascent (oldest)
        var ascentRequest1 = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Redpoint,
            CompletedAt: twoDaysAgo
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(ascentRequest1);

        // Create another ascent (middle)
        var ascentRequest2 = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Flash,
            CompletedAt: yesterday
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(ascentRequest2);

        // Create another ascent (newest)
        var ascentRequest3 = new CreateAscentRequest(
            RouteId: route.Id,
            Type: AscentType.Onsight,
            CompletedAt: now
        );
        await client.POSTAsync<CreateAscent, CreateAscentRequest, AscentResponse>(ascentRequest3);

        // Act
        var (response, result) = await client.GETAsync<GetAllAscents, EmptyRequest, List<AscentResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(3);

        // Find our test ascents and verify they're ordered by completion date (newest first)
        var ourAscents = result.Where(a => a.UserId == TestUserWithAscentPermissions.Id).ToList();
        ourAscents.Count.ShouldBe(3);

        // Should be ordered by CompletedAt descending (newest first)
        ourAscents[0].Type.ShouldBe(AscentType.Onsight);
        ourAscents[1].Type.ShouldBe(AscentType.Flash);
        ourAscents[2].Type.ShouldBe(AscentType.Redpoint);
    }
}
