using Microsoft.AspNetCore.Mvc;
using Minio.Exceptions;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.DI.Authorization.Permissions;
using YACTR.DI.Service;

namespace YACTR.Endpoints.Images;

public class ImageDeleteRequest
{
    [FromRoute]
    public Guid ImageId { get; set; }
}

public class DeleteImage : AuthenticatedEndpoint<ImageDeleteRequest, ImageResponse, ImageDataMapper>
{
    public required IImageStorageService ImageStorageService { get; init; }

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
            var image = await ImageStorageService.RemoveImageAsync(req.ImageId, ct);
            await SendOkAsync(await Map.FromEntityAsync(image, ct), cancellation: ct);
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