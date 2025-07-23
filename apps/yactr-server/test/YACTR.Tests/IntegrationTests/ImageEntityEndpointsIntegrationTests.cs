using System.Net;
using FastEndpoints;
using FastEndpoints.Testing;
using Microsoft.AspNetCore.Http;
using Shouldly;
using YACTR.Data.Model;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Endpoints;

namespace YACTR.Tests.Endpoints;

[Collection("IntegrationTests")]
public class ImageEntityEndpointsIntegrationTests(IntegrationTestClassFixture fixture) : TestBase<IntegrationTestClassFixture>
{
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
    
    [Fact]
    public async Task UploadImage_WithValidImageData_ReturnsCreatedImage()
    {
        var user = new User()
        {
            Username = "test_user_with_image_permissions",
            Email = "test_user@test.dev",
            Auth0UserId = $"test|{Guid.NewGuid()}",
            PlatformPermissions = Enum.GetValues<Permission>()
        };

        await fixture.GetEntityRepository<User>().CreateAsync(user, TestContext.Current.CancellationToken);

        // Arrange
        using var client = fixture.CreateAuthenticatedClient(user);
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
        var user = new User()
        {
            Username = "test_user_without_permissions",
            Email = "test_user2@test.dev",
            Auth0UserId = $"test|{Guid.NewGuid()}",
            PlatformPermissions = []
        };

        await fixture.GetEntityRepository<User>().CreateAsync(user, TestContext.Current.CancellationToken);

        // Arrange
        using var client = fixture.CreateAuthenticatedClient(user);
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
} 