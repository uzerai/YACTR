using YACTR.Model;
using YACTR.Model.Authentication;

namespace YACTR.DI.Service;

// TODO: Split IImageStorageService into MinioService and ImageService to
// handle the minio-specific logic and the database image entity logic separately later.
public interface IImageStorageService
{
    Task<Image> UploadImage(Stream image, User user, Guid? relatedEntityId);
    Task<Image> RemoveImage(Guid imageId);
}
