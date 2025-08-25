using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Http;
using NetTopologySuite.Geometries;
using Shouldly;
using YACTR.Data.Model;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Climbing;
using YACTR.Endpoints;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class ImageEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{

    public User TestUserWithImagePermissions = new()
    {
        Username = "test_user_with_image_permissions",
        Email = "test_user@test.dev",
        Auth0UserId = $"test|{Guid.NewGuid()}",
        PlatformPermissions = Enum.GetValues<Permission>()
    };
    // Minimum jpeg valid magic bytes.
    public static readonly byte[] MINIMAL_JPEG = { 0xFF,0xD8,0xFF,0xE0,0x00,0x10,0x4A,0x46,0x49,0x46,0x00,0x01,0x01,0x01,0x00,0x48,0x00,0x48,0x00,0x00,
        0xFF,0xDB,0x00,0x43,0x00,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,
        0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xFF,0xC2,0x00,0x0B,0x08,0x00,0x01,0x00,0x01,0x01,0x01,
        0x11,0x00,0xFF,0xC4,0x00,0x14,0x10,0x01,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,
        0x00,0x00,0x00,0x00,0xFF,0xDA,0x00,0x08,0x01,0x01,0x00,0x01,0x3F,0x10 };
    
    private readonly IFormFile TEST_FILE = new FormFile(
        baseStream: new MemoryStream(MINIMAL_JPEG),
        baseStreamOffset: 0,
        length: MINIMAL_JPEG.Length,
        name: "image",
        fileName: "test.jpeg"
    )
    {
        Headers = new HeaderDictionary(),
        ContentType = "image/jpeg"
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
        var request = new ImageUploadRequest(){
          Image = TEST_FILE
        };
        
        // Act
        var (response, result) = await client.POSTAsync<UploadImage, ImageUploadRequest, Image>(request, true);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task UploadImage_WithoutPermissions_ReturnsForbidden()
    {

        // Arrange
        using var client = fixture.CreateAuthenticatedClient();
        var request = new ImageUploadRequest()
        {
            Image = TEST_FILE
        };
        
        // Act
        var (response, _) = await client.POSTAsync<UploadImage, ImageUploadRequest, Image>(request, true);
        
        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UploadImage_WithRelatedEntityId_ReturnsCreatedImage()
    {
        // Arrange
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);

        var area = new Area()
        {
            Name = "Test Area",
            Description = "Test Area Description",
            Location = new Point(0, 0),
            Boundary = new MultiPolygon([new(new LinearRing([new(0, 0), new(1, 0), new(1, 1), new(0, 1), new(0, 0)]))])
        };

        area = await fixture.GetEntityRepository<Area>().CreateAsync(area, TestContext.Current.CancellationToken);

        var request = new ImageUploadRequest()
        {
            Image = TEST_FILE,
            RelatedEntityId = area.Id
        };

        // Act
        var (response, result) = await client.POSTAsync<UploadImage, ImageUploadRequest, Image>(request, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        result.ShouldNotBeNull();
        var imageFromDatabase = await fixture.GetEntityRepository<Image>()
            .GetByIdAsync(result.Id, TestContext.Current.CancellationToken);

        imageFromDatabase!.RelatedEntityId.ShouldBeEquivalentTo(area.Id);
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
        var (response, _) = await fixture.AnonymousClient.POSTAsync<UploadImage, ImageUploadRequest, Image>(request, true);
        
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
        var (uploadResponse, uploadedImage) = await client.POSTAsync<UploadImage, ImageUploadRequest, Image>(uploadRequest, true);
        uploadResponse.IsSuccessStatusCode.ShouldBeTrue();
        uploadedImage.ShouldNotBeNull();

        var deleteRequest = new ImageDeleteRequest()
        {
            ImageId = uploadedImage.Id
        };

        // Act
        var (response, result) = await client.DELETEAsync<DeleteImage, ImageDeleteRequest, Image>(deleteRequest, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(uploadedImage.Id);
    }

    [Fact]
    public async Task DeleteImage_WithoutPermissions_ReturnsForbidden()
    {
        // Arrange
        using var clientWithPermissions = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        using var clientWithoutPermissions = fixture.CreateAuthenticatedClient();
        
        // First create an image with a user that has permissions
        var uploadRequest = new ImageUploadRequest()
        {
            Image = TEST_FILE
        };
        var (uploadResponse, uploadedImage) = await clientWithPermissions.POSTAsync<UploadImage, ImageUploadRequest, Image>(uploadRequest, true);
        uploadResponse.IsSuccessStatusCode.ShouldBeTrue();

        var deleteRequest = new ImageDeleteRequest()
        {
            ImageId = uploadedImage.Id
        };

        // Act - try to delete with user without permissions
        var (response, _) = await clientWithoutPermissions.DELETEAsync<DeleteImage, ImageDeleteRequest, Image>(deleteRequest, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteImage_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var deleteRequest = new ImageDeleteRequest()
        {
            ImageId = Guid.NewGuid()
        };

        // Act
        var (response, _) = await fixture.AnonymousClient.DELETEAsync<DeleteImage, ImageDeleteRequest, Image>(deleteRequest, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteImage_WithNonExistentImageId_ReturnsNotFound()
    {
        // Arrange
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);
        var deleteRequest = new ImageDeleteRequest()
        {
            ImageId = Guid.NewGuid() // Non-existent image ID
        };

        // Act
        var (response, _) = await client.DELETEAsync<DeleteImage, ImageDeleteRequest, Image>(deleteRequest, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeFalse();
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteImage_WithRelatedEntityId_ReturnsDeletedImage()
    {
        // Arrange
        using var client = fixture.CreateAuthenticatedClient(TestUserWithImagePermissions);

        // Create an area to associate with the image
        var area = new Area()
        {
            Name = "Test Area for Delete",
            Description = "Test Area Description",
            Location = new Point(0, 0),
            Boundary = new MultiPolygon([new(new LinearRing([new(0, 0), new(1, 0), new(1, 1), new(0, 1), new(0, 0)]))])
        };
        area = await fixture.GetEntityRepository<Area>().CreateAsync(area, TestContext.Current.CancellationToken);

        // Create an image with related entity
        var uploadRequest = new ImageUploadRequest()
        {
            Image = TEST_FILE,
            RelatedEntityId = area.Id
        };
        var (uploadResponse, uploadedImage) = await client.POSTAsync<UploadImage, ImageUploadRequest, Image>(uploadRequest, true);
        uploadResponse.IsSuccessStatusCode.ShouldBeTrue();

        var deleteRequest = new ImageDeleteRequest()
        {
            ImageId = uploadedImage.Id
        };

        // Act
        var (response, result) = await client.DELETEAsync<DeleteImage, ImageDeleteRequest, Image>(deleteRequest, true);

        // Assert
        response.IsSuccessStatusCode.ShouldBeTrue();
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.ShouldNotBeNull();
        result.Id.ShouldBe(uploadedImage.Id);
    }
} 