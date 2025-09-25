using FastEndpoints;
using YACTR.Data.Model;
using YACTR.DI.Service;
using YACTR.DI.Authorization.Permissions;
using YACTR.Data.Model.Authorization.Permissions;
using FastEndpoints.Security;
using System.Security.Claims;

namespace YACTR.Endpoints.Images;

public class UploadImage : Endpoint<ImageUploadRequest, Image>
{
    private readonly IImageStorageService _imageStorageService;

    public UploadImage(IImageStorageService imageStorageService)
    {
        _imageStorageService = imageStorageService;
    }

    public override void Configure()
    {
        Post("/");
        Group<ImagesEndpointGroup>();
        AllowFileUploads();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.ImagesWrite)));
    }

    public override async Task HandleAsync(ImageUploadRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(HttpContext.User.ClaimValue(ClaimTypes.Sid), out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        if (req.Image is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        try
        {
            var uploadedImage = await _imageStorageService.UploadImageAsync(
                req.Image.OpenReadStream(),
                userId,
                req.RelatedEntityId, ct);

            await SendCreatedAtAsync<UploadImage>(uploadedImage.Id, uploadedImage, cancellation: ct);
        }
        catch
        {
            await SendErrorsAsync(422, cancellation: ct);
            return;
        }

    }
}