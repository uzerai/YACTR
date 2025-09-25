using FastEndpoints;

namespace YACTR.Endpoints.Images;

public class ImageUploadRequest
{
    public required IFormFile? Image { get; init; }
    public Guid? RelatedEntityId { get; init; }
};

public class ImagesEndpointGroup : Group
{
    public ImagesEndpointGroup()
    {
        Configure("images", ep => { });
    }
}