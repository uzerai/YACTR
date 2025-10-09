using FastEndpoints;

namespace YACTR.Endpoints.Images;

public class ImageUploadRequest
{
    public required IFormFile? Image { get; init; }
};

public record ImageResponse(Guid ImageId, string ImageUrl);

public class ImagesEndpointGroup : Group
{
    public ImagesEndpointGroup()
    {
        Configure("images", ep => { });
    }
}