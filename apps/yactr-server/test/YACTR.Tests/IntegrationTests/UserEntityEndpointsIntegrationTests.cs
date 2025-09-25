// using System.Net;
// using FastEndpoints;
// using FastEndpoints.Testing;
// using Shouldly;
// using YACTR.Data.Model.Authentication;
// using YACTR.Endpoints.Users;

// namespace YACTR.Tests.Endpoints;

// [Collection("IntegrationTests")]
// public class UserEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
// {

//     [Theory]
//     [InlineData("user1")]
//     [InlineData("user2")]
//     [InlineData("user3")]
//     public async Task GetMe_WithValidAuthentication_ReturnsCurrentUser(string userName)
//     {
//         var expectedUser = new User()
//         {
//             Auth0UserId = $"auth0|{Guid.NewGuid()}",
//             Username = userName,
//             Email = $"{userName}@test.dev"
//         };

//         var client = fixture.CreateAuthenticatedClient(expectedUser);
//         // Act
//         var (res, result) = await client.GETAsync<GetCurrentUser, EmptyRequest, User>(new());
//         res.IsSuccessStatusCode.ShouldBeTrue();

//         result.Auth0UserId.ShouldBe(expectedUser.Auth0UserId);
//         result.Username.ShouldBe(expectedUser.Username);
//     }

//     [Fact]
//     public async Task GetMe_WithoutAuthentication_ReturnsUnauthorized()
//     {
//         // Arrange
//         using var client = fixture.AnonymousClient;

//         // Act
//         var response = await client.GetAsync("/users/me", TestContext.Current.CancellationToken);

//         // Assert
//         Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//     }
// }