using FastEndpoints;
using Minio.Exceptions;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Endpoints.Images;

public class DeleteImageRequest
{
    [BindFrom("image_id")]
    public required Guid ImageId { get; set; }
}

public record DeleteImageResponse(Guid ImageId, string ImageUrl);

public class DeleteImage : AuthenticatedEndpoint<DeleteImageRequest, DeleteImageResponse>
{
    public required IImageStorageService ImageStorageService { get; init; }

    public override void Configure()
    {
        Delete("/{image_id}");
        Group<ImagesEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.ImagesWrite)));
    }

    public override async Task HandleAsync(DeleteImageRequest req, CancellationToken ct)
    {
        try
        {
            var image = await ImageStorageService.RemoveImageAsync(req.ImageId, ct);
            var imageUrl = await ImageStorageService.GetImageUrlAsync(image.Id, ct);
            await Send.OkAsync(new DeleteImageResponse(image.Id, imageUrl), cancellation: ct);
        }
        catch (ObjectNotFoundException)
        {
            await Send.ErrorsAsync(404, ct);
        }
        catch
        {
            await Send.ErrorsAsync(500, ct);
        }
    }
}