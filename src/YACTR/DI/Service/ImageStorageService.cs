using YACTR.DI.Repository.Interface;
using YACTR.Model;
using YACTR.Model.Authentication;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using FileSignatures;

namespace YACTR.DI.Service;

public class ImageStorageService : IImageStorageService
{
    private static readonly string BUCKET_NAME = "images";
    private readonly IMinioClient _minioClient;
    private readonly IFileFormatInspector _fileFormatInspector;
    private readonly IEntityRepository<Image> _imageRepository;
    private readonly ILogger<ImageStorageService> _logger;

    private FileSignatures.FileFormat? _uploadedFileFormat;

    public ImageStorageService(
        IMinioClient minioClient,
        IEntityRepository<Image> imageRepository,
        IFileFormatInspector fileFormatInspector,
        ILogger<ImageStorageService> logger)
    {
        _minioClient = minioClient;
        _imageRepository = imageRepository;
        _fileFormatInspector = fileFormatInspector;
        _logger = logger;
    }

    private bool IsImageFile(Stream image)
    {
      _uploadedFileFormat = _fileFormatInspector.DetermineFileFormat(image);

      return _uploadedFileFormat is FileSignatures.Formats.Image;
    }

    public async Task<Image> UploadImage(Stream image, User user, Guid? relatedEntityId)
    {
        if (!IsImageFile(image))
        {
          throw new Exception("File is not an image");
        }

        try
        {
            var objectName = Guid.NewGuid().ToString();
            var args = new PutObjectArgs()
                .WithBucket(BUCKET_NAME)
                .WithObject(objectName)
                .WithContentType(_uploadedFileFormat!.MediaType) // We guarantee it's an image and thus has a media type.
                .WithStreamData(image)
                .WithObjectSize(image.Length);

            var uploadedObject = await _minioClient.PutObjectAsync(args);

            Image imageEntity = await _imageRepository.CreateAsync(new()
            {
                Key = objectName,
                Bucket = BUCKET_NAME,
                UploaderId = user.Id,
                RelatedEntityId = relatedEntityId,
            });

            return imageEntity;
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex, "Error occurred while uploading image to MinIO");
            throw;
        }
    }

    public async Task<Image> RemoveImage(Guid imageId)
    {
        var image = await _imageRepository.GetByIdAsync(imageId)
          ?? throw new Exception($"Image with ID {imageId} not found");

        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(image.Bucket)
            .WithObject(image.Key));

        return image; 
    }
}