using YACTR.Domain.Model;

namespace YACTR.Infrastructure.Service;

// TODO: Split IImageStorageService into MinioService and ImageService to
// handle the minio-specific logic and the database image entity logic separately later.
public interface IImageStorageService
{
    Task<Image> UploadImageAsync(Stream image, Guid userId, CancellationToken ct = default);
    Task<Image> RemoveImageAsync(Guid imageId, CancellationToken ct = default);
    Task<string> GetImageUrlAsync(Guid imageId, CancellationToken ct = default);
}
