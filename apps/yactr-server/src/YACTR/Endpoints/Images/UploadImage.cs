using FastEndpoints;
using YACTR.Data.Model;
using YACTR.DI.Service;
using YACTR.DI.Authorization.UserContext;
using YACTR.DI.Authorization.Permissions;
using YACTR.Data.Model.Authorization.Permissions;

namespace YACTR.Endpoints;

public class UploadImage : Endpoint<EmptyRequest, Image>
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
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.ImagesWrite)));
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        using var memoryStream = new MemoryStream();
        await HttpContext.Request.Body.CopyToAsync(memoryStream, ct);
        memoryStream.Position = 0;

        try
        {
            var uploadedImage = await _imageStorageService.UploadImage(
            memoryStream,
            _userContext.CurrentUser!,
            Guid.Empty);

            await SendCreatedAtAsync<UploadImage>(uploadedImage.Id, uploadedImage, cancellation: ct);
        }
        catch
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
    }
} 