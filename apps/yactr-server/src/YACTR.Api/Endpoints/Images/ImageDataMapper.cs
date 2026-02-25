using FastEndpoints;
using YACTR.Domain.Model;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Endpoints.Images;

public class ImageUploadRequest
{
    public required IFormFile? Image { get; init; }
};

public record ImageResponse(Guid ImageId, string ImageUrl);

public class ImageDataMapper(IImageStorageService imageStorageService) : Mapper<ImageUploadRequest, ImageResponse, Image>
{
    private IImageStorageService ImageStorageService { get; set; } = imageStorageService;

    public override Image ToEntity(ImageUploadRequest r) => throw new NotImplementedException();
    public override async Task<ImageResponse> FromEntityAsync(Image e, CancellationToken ct) => new(e.Id, await ImageStorageService.GetImageUrlAsync(e.Id, ct));
}