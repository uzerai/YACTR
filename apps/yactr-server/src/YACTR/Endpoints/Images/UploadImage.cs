using YACTR.DI.Service;
using YACTR.DI.Authorization.Permissions;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.Endpoints.Images;

public class UploadImage : AuthenticatedEndpoint<ImageUploadRequest, ImageResponse>
{
    public required IImageStorageService ImageStorageService { get; init; }

    public override void Configure()
    {
        Post("/");
        Group<ImagesEndpointGroup>();
        AllowFileUploads();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.ImagesWrite)));
    }

    public override async Task HandleAsync(ImageUploadRequest req, CancellationToken ct)
    {
        if (req.Image is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        try
        {
            var uploadedImage = await ImageStorageService.UploadImageAsync(
                req.Image.OpenReadStream(),
                CurrentUserId,
                ct);

            var uploadedImageUrl = await ImageStorageService.GetImageUrlAsync(uploadedImage.Key, uploadedImage.Bucket, ct);

            await SendCreatedAtAsync<UploadImage>(uploadedImage.Id, new(uploadedImage.Id, uploadedImageUrl), cancellation: ct);
        }
        catch
        {
            await SendErrorsAsync(422, cancellation: ct);
            return;
        }

    }
}