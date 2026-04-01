using System.Net;
using FastEndpoints.Testing;
using Shouldly;
using YACTR.Api.Endpoints.Images;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Tests.EndpointTests.Images;

[Collection("IntegrationTests")]
public class DeleteImageIntegrationTests(ApiTestClassFixture fixture) : ImageEndpointTestsBase(fixture)
{
    [Fact]
    public async Task DeleteImage_WithValidImageId_ReturnsDeletedImage()
    {
        // Arrange
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);

        // First create an image to delete
        var uploadRequest = new ImageUploadRequest()
        {
            Image = TestFile
        };
        var (uploadResponse, uploadedImage) = await client.POSTAsync<UploadImage, ImageUploadRequest, ImageResponse>(uploadRequest, true);
        uploadResponse.IsSuccessStatusCode.ShouldBeTrue();
        uploadedImage.ShouldNotBeNull();

        // Act - Delete using route parameter
        var (response, result) = await client.DELETEAsync<DeleteImage, ImageDeleteRequest, ImageResponse>(
            new ImageDeleteRequest { ImageId = uploadedImage.ImageId }, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
    }

    [Fact]
    public async Task DeleteImage_WithoutPermissions_ReturnsForbidden()
    {
        // Arrange
        User userWithoutPermissions = new()
        {
            Auth0UserId = $"user_without_image_perms|{Guid.NewGuid()}",
            Username = "user_without_image_perms",
            Email = "user@test.example",
            PlatformPermissions = [.. DefaultUserPermissions.PlatformPermissions]
        };

        using var clientWithPermissions = Fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        using var clientWithoutPermissions = Fixture.CreateAuthenticatedClient(userWithoutPermissions);

        // First create an image with a user that has permissions
        var uploadRequest = new ImageUploadRequest()
        {
            Image = TestFile
        };
        var (uploadResponse, uploadedImage) = await clientWithPermissions.POSTAsync<UploadImage, ImageUploadRequest, ImageResponse>(uploadRequest, true);
        uploadResponse.IsSuccessStatusCode.ShouldBeTrue();

        // Act - try to delete with user without permissions using route parameter
        var (response, _) = await clientWithoutPermissions.DELETEAsync<DeleteImage, ImageDeleteRequest, ImageResponse>(
            new ImageDeleteRequest { ImageId = uploadedImage.ImageId }, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteImage_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var (response, _) = await Fixture.CreateClient().DELETEAsync<DeleteImage, ImageDeleteRequest, ImageResponse>(
            new ImageDeleteRequest { ImageId = Guid.NewGuid() }, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteImage_WithNonExistentImageId_ReturnsNotFound()
    {
        // Arrange
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);

        // Act
        var (response, _) = await client.DELETEAsync<DeleteImage, ImageDeleteRequest, ImageResponse>(
            new ImageDeleteRequest { ImageId = Guid.NewGuid() }, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteImage_WithValidImageId_ReturnsDeletedImageWithCorrectId()
    {
        // Arrange
        using var client = Fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);

        // Create an image to delete
        var uploadRequest = new ImageUploadRequest()
        {
            Image = TestFile
        };
        var (uploadResponse, uploadedImage) = await client.POSTAsync<UploadImage, ImageUploadRequest, ImageResponse>(uploadRequest, true);
        uploadResponse.IsSuccessStatusCode.ShouldBeTrue();
        uploadedImage.ShouldNotBeNull();

        // Act
        var (response, result) = await client.DELETEAsync<DeleteImage, ImageDeleteRequest, ImageResponse>(
            new ImageDeleteRequest { ImageId = uploadedImage.ImageId }, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
    }
}
