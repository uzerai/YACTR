using FastEndpoints;
using YACTR.Data.Model;
using YACTR.DI.Service;
using YACTR.DI.Authorization.Permissions;
using YACTR.Data.Model.Authorization.Permissions;
using Minio.Exceptions;
using Microsoft.AspNetCore.Mvc;
using FastEndpoints.Security;
using System.Security.Claims;

namespace YACTR.Endpoints.Images;

public class ImageDeleteRequest
{
    [FromRoute]
    public Guid ImageId { get; set; }
}

[PlatformPermissionRequired(Permission.ImagesWrite)]
public class DeleteImage : Endpoint<ImageDeleteRequest, Image>
{
    private readonly IImageStorageService _imageStorageService;

    public DeleteImage(IImageStorageService imageStorageService)
    {
        _imageStorageService = imageStorageService;
    }

    public override void Configure()
    {
        Delete("/{ImageId}");
        Group<ImagesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.ImagesWrite)));
    }

    public override async Task HandleAsync(ImageDeleteRequest req, CancellationToken ct)
    {
        if (!Guid.TryParse(HttpContext.User.ClaimValue(ClaimTypes.Sid), out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        try
        {
            var image = await _imageStorageService.RemoveImage(req.ImageId, ct);
            await SendOkAsync(image, cancellation: ct);
        }
        catch (ObjectNotFoundException)
        {
            await SendErrorsAsync(404, ct);
        }
        catch
        {
            await SendErrorsAsync(500, ct);
        }
    }
}