using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Http;
using Shouldly;
using YACTR.Api.Endpoints.Images;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Tests.EndpointTests;

[Collection("IntegrationTests")]
public class ImageEntityEndpointsIntegrationTests(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    public User TestUserWithImagePermissions = new()
    {
        Username = "test_user_with_image_permissions",
        Email = "test_user@test.dev",
        Auth0UserId = $"test|{Guid.NewGuid()}",
        PlatformPermissions = Enum.GetValues<Permission>()
    };

    private readonly IFormFile TEST_FILE = new FormFile(
        baseStream: new MemoryStream(TestDataConstants.MINIMAL_JPEG),
        baseStreamOffset: 0,
        length: TestDataConstants.MINIMAL_JPEG.Length,
        name: "image",
        fileName: "test.jpeg"
    )
    {
        Headers = new HeaderDictionary(),
        ContentType = "image/jpeg"
    };

    private readonly IFormFile TEST_SVG_FILE = new FormFile(
        baseStream: new MemoryStream(TestDataConstants.MINIMAL_SVG),
        baseStreamOffset: 0,
        length: TestDataConstants.MINIMAL_SVG.Length,
        name: "image",
        fileName: "test.svg"
    )
    {
        Headers = new HeaderDictionary(),
        ContentType = "image/svg+xml"
    };

    // Let's just provide the test user with image permissions as a basis for all the tests.
    protected override async ValueTask SetupAsync()
    {
        await base.SetupAsync();

        await fixture.GetEntityRepository<User>().CreateAsync(TestUserWithImagePermissions, TestContext.Current.CancellationToken);
    }

    [Fact]
    public async Task UploadImage_WithValidImageData_ReturnsCreatedImage()
    {
        // Arrange
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        var request = new ImageUploadRequest()
        {
            Image = TEST_FILE
        };

        // Act
        var (response, result) = await client.POSTAsync<UploadImage, ImageUploadRequest, ImageResponse>(request, true);

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
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        var request = new ImageUploadRequest()
        {
            Image = TEST_SVG_FILE
        };

        // Act
        var (response, result) = await client.POSTAsync<UploadImage, ImageUploadRequest, ImageResponse>(request, true);

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

        using var client = fixture.CreateAuthenticatedClient(userWithoutPermissions);
        var request = new ImageUploadRequest()
        {
            Image = TEST_FILE
        };

        // Act
        var (response, _) = await client.POSTAsync<UploadImage, ImageUploadRequest, ImageResponse>(request, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UploadImage_WithValidImageData_ReturnsImageResponseWithUrl()
    {
        // Arrange
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        var request = new ImageUploadRequest()
        {
            Image = TEST_FILE
        };

        // Act
        var (response, result) = await client.POSTAsync<UploadImage, ImageUploadRequest, ImageResponse>(request, true);

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
        var request = new ImageUploadRequest()
        {
            Image = TEST_FILE
        };

        // Act
        var (response, _) = await fixture.CreateClient().POSTAsync<UploadImage, ImageUploadRequest, ImageResponse>(request, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteImage_WithValidImageId_ReturnsDeletedImage()
    {
        // Arrange
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);

        // First create an image to delete
        var uploadRequest = new ImageUploadRequest()
        {
            Image = TEST_FILE
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

        using var clientWithPermissions = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        using var clientWithoutPermissions = fixture.CreateAuthenticatedClient(userWithoutPermissions);

        // First create an image with a user that has permissions
        var uploadRequest = new ImageUploadRequest()
        {
            Image = TEST_FILE
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
        var (response, _) = await fixture.CreateClient().DELETEAsync<DeleteImage, ImageDeleteRequest, ImageResponse>(
            new ImageDeleteRequest { ImageId = Guid.NewGuid() }, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteImage_WithNonExistentImageId_ReturnsNotFound()
    {
        // Arrange
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);

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
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);

        // Create an image to delete
        var uploadRequest = new ImageUploadRequest()
        {
            Image = TEST_FILE
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