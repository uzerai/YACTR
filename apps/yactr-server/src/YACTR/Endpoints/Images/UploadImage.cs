using FastEndpoints;
using YACTR.Data.Model;
using YACTR.DI.Service;
using YACTR.DI.Authorization.UserContext;
using YACTR.DI.Authorization.Permissions;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.Endpoints;

public class UploadImage : Endpoint<ImageUploadRequest, Image>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IUserContext _userContext;

    public UploadImage(IImageStorageService imageStorageService, IUserContext userContext)
    {
        _imageStorageService = imageStorageService;
        _userContext = userContext;
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
        if (req.Image is null)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        try
        {
            var uploadedImage = await _imageStorageService.UploadImageAsync(
                req.Image.OpenReadStream(),
                _userContext.CurrentUser!,
                Guid.Empty, ct);

            await SendCreatedAtAsync<UploadImage>(uploadedImage.Id, uploadedImage, cancellation: ct);
        }
        catch
        {
            await SendErrorsAsync(422, cancellation: ct);
            return;
        }
        
    }
} 