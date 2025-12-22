using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Endpoints.Users;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class UserEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
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
        var (res, result) = await client.GETAsync<GetCurrentUser, EmptyRequest, User>(new());
        res.IsSuccessStatusCode.ShouldBeTrue();

        result.Auth0UserId.ShouldBe(expectedUser.Auth0UserId);
        result.Username.ShouldBe(expectedUser.Username);
    }

    [Fact]
    public async Task GetMe_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        using var client = fixture.CreateClient();

        // Act
        var (response, _) = await client.GETAsync<GetCurrentUser, EmptyRequest, User>(new());

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllUsers_WithAdminPermission_ReturnsUsers()
    {
        // Arrange - Create a user with admin permissions
        var adminUser = new User()
        {
            Auth0UserId = $"auth0|{Guid.NewGuid()}",
            Username = "admin_user",
            Email = "admin@test.dev",
            AdminPermissions = [Permission.UsersRead]
        };

        await fixture.GetEntityRepository<User>().CreateAsync(adminUser, TestContext.Current.CancellationToken);
        using var client = fixture.CreateAuthenticatedClient(adminUser);

        // Act
        var (response, result) = await client.GETAsync<GetAllUsers, EmptyRequest, IEnumerable<User>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAllUsers_WithoutAdminPermission_ReturnsForbidden()
    {
        // Arrange - Create a user without admin permissions
        var regularUser = new User()
        {
            Auth0UserId = $"auth0|{Guid.NewGuid()}",
            Username = "regular_user",
            Email = "regular@test.dev",
            AdminPermissions = [] // No admin permissions
        };

        using var client = fixture.CreateAuthenticatedClient(regularUser);

        // Act
        var (response, _) = await client.GETAsync<GetAllUsers, EmptyRequest, IEnumerable<User>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllUsers_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        using var client = fixture.CreateClient();

        // Act
        var (response, _) = await client.GETAsync<GetAllUsers, EmptyRequest, IEnumerable<User>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}