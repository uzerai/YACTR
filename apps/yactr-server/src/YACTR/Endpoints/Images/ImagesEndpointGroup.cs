using FastEndpoints;

namespace YACTR.Endpoints;

public class ImageUploadRequest
{
    public required IFormFile? Image { get; init; }
};

public class ImagesEndpointGroup : Group
{
    public ImagesEndpointGroup()
    {
        Configure("images", ep => { });
    }
} 