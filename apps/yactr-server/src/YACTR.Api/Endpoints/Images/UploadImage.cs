using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Endpoints.Images;

public class UploadImageRequest
{
    public required IFormFile? Image { get; init; }
}

public record UploadImageResponse(Guid ImageId, string ImageUrl);

public class UploadImage : AuthenticatedEndpoint<UploadImageRequest, UploadImageResponse>
{
    public required IImageStorageService ImageStorageService { get; init; }
    public override void Configure()
    {
        Post("/");
        Group<ImagesEndpointGroup>();
        AllowFileUploads();
        Description(b => b.Accepts<UploadImageRequest>("multipart/form-data"));
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.ImagesWrite)));
    }

    public override async Task HandleAsync(UploadImageRequest req, CancellationToken ct)
    {
        if (req.Image is null)
        {
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        try
        {
            var uploadedImage = await ImageStorageService.UploadImageAsync(
                req.Image.OpenReadStream(),
                CurrentUserId,
                ct);

            var imageUrl = await ImageStorageService.GetImageUrlAsync(uploadedImage.Id, ct);
            await Send.CreatedAtAsync<UploadImage>(uploadedImage.Id, new UploadImageResponse(uploadedImage.Id, imageUrl), cancellation: ct);
        }
        catch
        {
            await Send.ErrorsAsync(422, cancellation: ct);
            return;
        }

    }
}