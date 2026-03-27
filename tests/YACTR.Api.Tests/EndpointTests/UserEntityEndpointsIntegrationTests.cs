using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Pagination;
using YACTR.Api.Endpoints.Users;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Tests.EndpointTests;

[Collection("IntegrationTests")]
public class UserEntityEndpointsIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
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
        var (res, result) = await client.GETAsync<GetCurrentUser, EmptyRequest, CurrentUserResponse>(new());
        res.IsSuccessStatusCode.ShouldBeTrue();

        result.Username.ShouldBe(expectedUser.Username);
    }

    [Fact]
    public async Task GetMe_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        using var client = fixture.CreateClient();

        // Act
        var (response, _) = await client.GETAsync<GetCurrentUser, EmptyRequest, CurrentUserResponse>(new());

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
        var (response, result) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAllUsers_WithPagination_ReturnsRequestedPageAndTotalCount()
    {
        var adminUser = new User()
        {
            Auth0UserId = $"auth0|{Guid.NewGuid()}",
            Username = "admin_user_pagination",
            Email = "admin-pagination@test.dev",
            AdminPermissions = [Permission.UsersRead]
        };
        await fixture.GetEntityRepository<User>().CreateAsync(adminUser, TestContext.Current.CancellationToken);
        using var client = fixture.CreateAuthenticatedClient(adminUser);

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        for (var i = 0; i < 3; i++)
        {
            await fixture.GetEntityRepository<User>().CreateAsync(new User
            {
                Auth0UserId = $"auth0|{Guid.NewGuid()}",
                Username = $"pagination_user_{i}_{Guid.NewGuid()}",
                Email = $"pagination-{i}-{Guid.NewGuid()}@test.dev"
            }, TestContext.Current.CancellationToken);
        }

        var (response, result) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new()
        {
            Page = 2,
            PageSize = 2
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount + 3);
        var expectedPageCount = Math.Clamp(result.TotalCount - 2, 0, 2);
        result.Items.Count.ShouldBe(expectedPageCount);
    }

    [Fact]
    public async Task GetAllUsers_WithPageSizeBelowMinimum_ClampsToMinimum()
    {
        var adminUser = new User()
        {
            Auth0UserId = $"auth0|{Guid.NewGuid()}",
            Username = "admin_user_clamp",
            Email = "admin-clamp@test.dev",
            AdminPermissions = [Permission.UsersRead]
        };
        await fixture.GetEntityRepository<User>().CreateAsync(adminUser, TestContext.Current.CancellationToken);
        using var client = fixture.CreateAuthenticatedClient(adminUser);

        var (baselineResponse, baselineResult) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });
        baselineResponse.IsSuccessStatusCode.ShouldBeTrue();
        baselineResult.ShouldNotBeNull();

        var (response, result) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new()
        {
            Page = 1,
            PageSize = 0
        });

        response.IsSuccessStatusCode.ShouldBeTrue();
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBe(baselineResult.TotalCount);
        result.Items.Count.ShouldBe(1);
        result.Items.Single().Id.ShouldBe(baselineResult.Items.Single().Id);
    }

    [Fact]
    public async Task GetAllUsers_WithDifferentPages_ReturnsDifferentItems()
    {
        var adminUser = new User()
        {
            Auth0UserId = $"auth0|{Guid.NewGuid()}",
            Username = "admin_user_diff_pages",
            Email = "admin-diff-pages@test.dev",
            AdminPermissions = [Permission.UsersRead]
        };
        await fixture.GetEntityRepository<User>().CreateAsync(adminUser, TestContext.Current.CancellationToken);
        using var client = fixture.CreateAuthenticatedClient(adminUser);

        for (var i = 0; i < 2; i++)
        {
            await fixture.GetEntityRepository<User>().CreateAsync(new User
            {
                Auth0UserId = $"auth0|{Guid.NewGuid()}",
                Username = $"page_diff_user_{i}_{Guid.NewGuid()}",
                Email = $"page-diff-{i}-{Guid.NewGuid()}@test.dev"
            }, TestContext.Current.CancellationToken);
        }

        var (pageOneResponse, pageOneResult) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new()
        {
            Page = 1,
            PageSize = 1
        });

        var (pageTwoResponse, pageTwoResult) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new()
        {
            Page = 2,
            PageSize = 1
        });

        pageOneResponse.IsSuccessStatusCode.ShouldBeTrue();
        pageTwoResponse.IsSuccessStatusCode.ShouldBeTrue();
        pageOneResult.ShouldNotBeNull();
        pageTwoResult.ShouldNotBeNull();
        pageOneResult.TotalCount.ShouldBe(pageTwoResult.TotalCount);
        pageOneResult.Items.Count.ShouldBe(1);
        pageTwoResult.Items.Count.ShouldBe(1);
        pageOneResult.Items.Single().Id.ShouldNotBe(pageTwoResult.Items.Single().Id);
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
        var (response, _) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new());

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
        var (response, _) = await client.GETAsync<GetAllUsers, GetAllUsersRequest, PaginatedResponse<UserResponse>>(new());

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}