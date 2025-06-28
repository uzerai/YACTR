using System.Net.Http.Headers;
using System.Text.Json;
using YACTR.Data.Model;
using System.Net;

namespace YACTR.IntegrationTests.Controllers;

public class ImageControllerIntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public ImageControllerIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        // Setup authentication for protected endpoints
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyTestToken==");
    }
    
    [Fact]
    public async Task UploadImage_WithValidImageData_ReturnsCreatedImage()
    {
        // Arrange
        var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 }; // Minimal JPEG header
        var content = new ByteArrayContent(imageBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        
        // Act
        var response = await _client.PostAsync("/images", content);
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var responseString = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        
        var image = JsonSerializer.Deserialize<Image>(responseString, options);
        Assert.NotNull(image);
        Assert.NotEqual(Guid.Empty, image.Id);
    }
    
    [Fact]
    public async Task UploadImage_WithEmptyData_ReturnsBadRequest()
    {
        // Arrange
        var emptyBytes = new byte[0];
        var content = new ByteArrayContent(emptyBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        
        // Act
        var response = await _client.PostAsync("/images", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task UploadImage_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var client = _client;
        client.DefaultRequestHeaders.Authorization = null; // Remove authentication
        
        var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01 };
        var content = new ByteArrayContent(imageBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
        
        // Act
        var response = await client.PostAsync("/images", content);
        
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 