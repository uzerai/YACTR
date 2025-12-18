using YACTR.Data.Model.Authorization.Permissions;
using YACTR.DI.Authorization.Permissions;
using YACTR.DI.Service;

namespace YACTR.Endpoints.Images;

public class UploadImage : AuthenticatedEndpoint<ImageUploadRequest, ImageResponse, ImageDataMapper>
{
    public required IImageStorageService ImageStorageService { get; init; }
    public override void Configure()
    {
        Post("/");
        Group<ImagesEndpointGroup>();
        AllowFileUploads();
        Description(b => b.Accepts<ImageUploadRequest>("multipart/form-data"));
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.ImagesWrite)));
    }

    public override async Task HandleAsync(ImageUploadRequest req, CancellationToken ct)
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

            await Send.CreatedAtAsync<UploadImage>(uploadedImage.Id, await Map.FromEntityAsync(uploadedImage, ct), cancellation: ct);
        }
        catch
        {
            await Send.ErrorsAsync(422, cancellation: ct);
            return;
        }

    }
}