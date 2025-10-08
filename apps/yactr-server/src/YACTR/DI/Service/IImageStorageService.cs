using YACTR.Data.Model;

namespace YACTR.DI.Service;

// TODO: Split IImageStorageService into MinioService and ImageService to
// handle the minio-specific logic and the database image entity logic separately later.
public interface IImageStorageService
{
    Task<Image> UploadImageAsync(Stream image, Guid userId, CancellationToken ct = default);
    Task<Image> RemoveImage(Guid imageId, CancellationToken ct = default);
    Task<string> GetImageUrl(Guid imageId, CancellationToken ct = default);
}
