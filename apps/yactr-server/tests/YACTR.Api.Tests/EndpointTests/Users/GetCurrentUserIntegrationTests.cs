using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Users;
using YACTR.Domain.Model.Authentication;

namespace YACTR.Api.Tests.EndpointTests.Users;

[Collection("IntegrationTests")]
public class GetCurrentUserIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    [Theory]
    [InlineData("user1")]
    [InlineData("user2")]
    [InlineData("user3")]
    public async Task GetMe_WithValidAuthentication_ReturnsCurrentUser(string userName)
    {
        var expectedUser = new User()
        {
            Auth0UserId = $"auth0|{Guid.NewGuid()}",
            Username = userName,
            Email = $"{userName}@test.dev"
        };

        var client = fixture.CreateAuthenticatedClient(expectedUser);
        // Act
        var (res, result) = await client.GETAsync<GetCurrentUser, EmptyRequest, GetCurrentUserResponse>(EmptyRequest.Instance);
        res.IsSuccessStatusCode.ShouldBeTrue();

        result.Username.ShouldBe(expectedUser.Username);
    }

    [Fact]
    public async Task GetMe_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        using var client = fixture.CreateClient();

        // Act
        var (response, _) = await client.GETAsync<GetCurrentUser, EmptyRequest, GetCurrentUserResponse>(EmptyRequest.Instance);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
