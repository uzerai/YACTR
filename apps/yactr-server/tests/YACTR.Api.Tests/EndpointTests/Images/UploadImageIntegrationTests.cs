using System.Net;
using Shouldly;
using YACTR.Api.Endpoints.Images;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Tests.EndpointTests.Images;

[Collection("IntegrationTests")]
public class UploadImageIntegrationTests(ApiTestClassFixture fixture) : ImageEndpointTestsBase(fixture)
{
    [Fact]
    public async Task UploadImage_WithValidImageData_ReturnsCreatedImage()
    {
        // Arrange
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        var request = new UploadImageRequest()
        {
            Image = TestFile
        };

        // Act
        var (response, result) = await client.POSTAsync<UploadImage, UploadImageRequest, UploadImageResponse>(request, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.ImageId.ShouldNotBe(Guid.Empty);
        result.ImageUrl.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task UploadImage_WithValidSvgData_ReturnsCreatedImage()
    {
        // Arrange
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        var request = new UploadImageRequest()
        {
            Image = TestSvgFile
        };

        // Act
        var (response, result) = await client.POSTAsync<UploadImage, UploadImageRequest, UploadImageResponse>(request, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.ImageId.ShouldNotBe(Guid.Empty);
        result.ImageUrl.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task UploadImage_WithoutPermissions_ReturnsForbidden()
    {
        // Arrange
        User userWithoutPermissions = new()
        {
            Auth0UserId = $"user_without_image_perms|{Guid.NewGuid()}",
            Username = "user_without_image_perms",
            Email = "user@test.example",
            PlatformPermissions = [.. DefaultUserPermissions.PlatformPermissions]
        };

        using var client = Fixture.CreateAuthenticatedClient(userWithoutPermissions);
        var request = new UploadImageRequest()
        {
            Image = TestFile
        };

        // Act
        var (response, _) = await client.POSTAsync<UploadImage, UploadImageRequest, UploadImageResponse>(request, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UploadImage_WithValidImageData_ReturnsImageResponseWithUrl()
    {
        // Arrange
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        var request = new UploadImageRequest()
        {
            Image = TestFile
        };

        // Act
        var (response, result) = await client.POSTAsync<UploadImage, UploadImageRequest, UploadImageResponse>(request, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        result.ImageId.ShouldNotBe(Guid.Empty);
        result.ImageUrl.ShouldNotBeNullOrEmpty();
        result.ImageUrl.ShouldStartWith("http"); // Should be a valid URL
    }

    [Fact]
    public async Task UploadImage_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var request = new UploadImageRequest()
        {
            Image = TestFile
        };

        // Act
        var (response, _) = await Fixture.CreateClient().POSTAsync<UploadImage, UploadImageRequest, UploadImageResponse>(request, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
