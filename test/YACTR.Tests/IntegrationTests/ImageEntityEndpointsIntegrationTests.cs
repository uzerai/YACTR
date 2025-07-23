// using YACTR.Data.Model;
// using System.Net;
// using YACTR.Data.Model.Authentication;
// using YACTR.Data.Model.Authorization.Permissions;

// namespace YACTR.Tests.Controllers;

// [Collection("IntegrationTests")]
// public class ImageEntityEndpointsIntegrationTests : IntegrationTestClassFixture
// {
//     // Minimum jpeg valid magic bytes.
//     public readonly byte[] MINIMAL_JPEG = { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 };

//     public ImageEntityEndpointsIntegrationTests(TestWebApplicationFactory factory) : base(factory)
//     {
//     }
    
//     [Fact]
//     public async Task UploadImage_WithValidImageData_ReturnsCreatedImage()
//     {
//         var user = new User()
//         {
//             Username = "test_user_with_image_permissions",
//             Email = "test_user@test.dev",
//             Auth0UserId = $"test|{Guid.NewGuid()}",
//             PlatformPermissions = Enum.GetValues<Permission>()
//         };

//         _databaseContext.Add(user);
//         await _databaseContext.SaveChangesAsync();

//         // Arrange
//         var client = CreateAuthenticatedClient(user);
//         var imagesBytes =  MINIMAL_JPEG;
//         var content = new MultipartFormDataContent
//         {
//           { new ByteArrayContent(imagesBytes), "Image"}
//         };
        
//         // Act
//         var response = await client.PostAsync("/images", content);
        
//         // Assert
//         response.EnsureSuccessStatusCode();
//         Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
//         var image = await DeserializeEntityFromResponse<Image>(response);
//         Assert.NotNull(image);
//         Assert.NotEqual(Guid.Empty, image.Id);
//     }
    
//     [Fact]
//     public async Task UploadImage_WithEmptyData_ReturnsBadRequest()
//     {
//         // Arrange
//         var user = new User()
//         {
//             Username = "test_user_with_image_permissions",
//             Email = "test_user@test.dev",
//             Auth0UserId = $"test|{Guid.NewGuid()}",
//             PlatformPermissions = Enum.GetValues<Permission>()
//         };

//         _databaseContext.Add(user);
//         await _databaseContext.SaveChangesAsync();

//         var client = CreateAuthenticatedClient(user);
//         var emptyBytes = new byte[0];
//         var content = new ByteArrayContent(emptyBytes);
//         content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        
//         // Act
//         var response = await client.PostAsync("/images", content);
        
//         // Assert
//         Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//     }
    
//     [Fact]
//     public async Task UploadImage_WithoutAuthentication_ReturnsUnauthorized()
//     {
//         // Arrange
//         var client = CreateAnonymousClient();
        
//         var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 };
//         var content = new ByteArrayContent(imageBytes);
//         content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        
//         // Act
//         var response = await client.PostAsync("/images", content);
        
//         // Assert
//         Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
//     }
// } 