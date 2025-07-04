using System.Net;
using System.Text.Json;
using YACTR.Data.Model.Authentication;

namespace YACTR.IntegrationTests.Controllers;

public class UsersControllerIntegrationTests : IntegrationTestClassFixture
{
    public UsersControllerIntegrationTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

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

        var client = CreateAuthenticatedClient(expectedUser);
        // Act
        var response = await client.GetAsync("/users/me");

        // Assert
        response.EnsureSuccessStatusCode();

        var responseString = await response.Content.ReadAsStringAsync();

        var responseUser = JsonSerializer.Deserialize<User>(responseString, _jsonSerializerOptions);
        Assert.NotNull(responseUser);
        Assert.Equal(responseUser.Auth0UserId, expectedUser.Auth0UserId);
        Assert.Equal(responseUser.Username, expectedUser.Username);
        Assert.Equal(responseUser.Email, expectedUser.Email);
    }
    
    [Fact]
    public async Task GetMe_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        using var client = CreateAnonymousClient();
        
        // Act
        var response = await client.GetAsync("/users/me");
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 