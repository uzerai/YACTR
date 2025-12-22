using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Endpoints.Routes.RouteRatings;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class RouteRatingEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{

    [Fact]
    public async Task CreateRouteRating_WithUnauthenticatedClient_ReturnsUnauthorized()
    {
        using var client = fixture.CreateClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var requestData = new CreateOrUpdateRouteRatingRequest()
        {
            RouteId = routes.First().Id,
            RatingData = new (3)
        };

        var (response, created) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(requestData);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateRouteRating_WithValidData_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var requestData = new CreateOrUpdateRouteRatingRequest()
        {
            RouteId = routes.First().Id,
            RatingData = new (3)
        };

        var (response, created) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(requestData);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        created.Rating.ShouldBe(3);
    }

    [Fact]
    public async Task CreateRouteRating_WithMissingRoute_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();

        // Act
        var requestData = new CreateOrUpdateRouteRatingRequest()
        {
            RouteId = Guid.CreateVersion7(),
            RatingData = new (3)
        };
        var (response, created) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(requestData);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateRouteRating_WithExistingRating_ReturnsSuccessStatusCode_AndUpdatedRating()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var originalRatingData = new CreateOrUpdateRouteRatingRequest()
        {
            RouteId = routes.First().Id,
            RatingData = new (1)
        };
        var updatedRatingData = new CreateOrUpdateRouteRatingRequest()
        {
            RouteId = routes.First().Id,
            RatingData = new (3)
        };

        var (firstResponse, firstCreated) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(originalRatingData);
        var (secondResponse, secondCreated) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(updatedRatingData);

        secondResponse.IsSuccessStatusCode.ShouldBeTrue();
        secondCreated.ShouldNotBeNull();
        secondCreated.Id.ShouldBe(firstCreated.Id);
        secondCreated.Rating.ShouldBe(updatedRatingData.RatingData.Rating);

    }

    [Fact]
    public async Task DeleteRouteRating_WithExistingLike_ReturnsSuccessStatusCode()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var createRequestData = new CreateOrUpdateRouteRatingRequest()
        {
            RouteId = routes.First().Id,
            RatingData = new (3)
        };
        var deleteRequestData = new DeleteRouteRatingRequest()
        {
            RouteId = routes.First().Id
        };

        var (createResponse, created) = await client.POSTAsync<CreateOrUpdateRouteRating, CreateOrUpdateRouteRatingRequest, RouteRatingResponse>(createRequestData);
        var (deleteResponse, deleted) = await client.DELETEAsync<DeleteRouteRating, DeleteRouteRatingRequest, RouteRatingResponse>(deleteRequestData);
        // Assert
        deleteResponse.IsSuccessStatusCode.ShouldBeTrue();
        deleted.Id.ShouldBe(created.Id);
    }

    [Fact]
    public async Task DeleteRouteLike_WithoutExistingLike_ReturnsNotFound()
    {
        using var client = fixture.CreateAuthenticatedClient();
        var (area, sector, routes) = await fixture.TestDataSeeder.SeedAreaWithSectorAndRouteAsync();

        // Act
        var requestData = new DeleteRouteRatingRequest()
        {
            RouteId = routes.First().Id
        };
        var (deleteResponse, deleted) = await client.DELETEAsync<DeleteRouteRating, DeleteRouteRatingRequest, RouteRatingResponse>(requestData);

        // Assert
        deleteResponse.IsSuccessStatusCode.ShouldBeFalse();
        deleteResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NotFound);
    }
}