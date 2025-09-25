using YACTR.Data.Model;
using YACTR.Data.Model.Authentication;

namespace YACTR.DI.Service;

// TODO: Split IImageStorageService into MinioService and ImageService to
// handle the minio-specific logic and the database image entity logic separately later.
public interface IImageStorageService
{
    Task<Image> UploadImageAsync(Stream image, Guid userId, Guid? relatedEntityId, CancellationToken ct);
    Task<Image> RemoveImage(Guid imageId, CancellationToken ct);
}
