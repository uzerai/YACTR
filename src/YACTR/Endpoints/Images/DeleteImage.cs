using FastEndpoints;
using YACTR.Data.Model;
using YACTR.DI.Service;
using YACTR.DI.Authorization.UserContext;
using YACTR.DI.Authorization.Permissions;
using YACTR.Data.Model.Authorization.Permissions;
using Minio.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace YACTR.Endpoints;

public class ImageDeleteRequest
{
    [FromRoute]
    public Guid ImageId { get; set; }
}

public class DeleteImage : Endpoint<ImageDeleteRequest, Image>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IUserContext _userContext;

    public DeleteImage(IImageStorageService imageStorageService, IUserContext userContext)
    {
        _imageStorageService = imageStorageService;
        _userContext = userContext;
    }

    public override void Configure()
    {
        Delete("/{ImageId}");
        Group<ImagesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.ImagesWrite)));
    }

    public override async Task HandleAsync(ImageDeleteRequest req, CancellationToken ct)
    {
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