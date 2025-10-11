using FileSignatures;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using YACTR.Data.Repository.Interface;
using Image = YACTR.Data.Model.Image;

namespace YACTR.DI.Service;

public class ImageStorageService(
    IMinioClient minioClient,
    IEntityRepository<Image> imageRepository,
    IFileFormatInspector fileFormatInspector,
    ILogger<ImageStorageService> logger) : IImageStorageService
{
    private static readonly string BUCKET_NAME = "images";
    private readonly IMinioClient _minioClient = minioClient;
    private readonly IFileFormatInspector _fileFormatInspector = fileFormatInspector;
    private readonly IEntityRepository<Image> _imageRepository = imageRepository;
    private readonly ILogger<ImageStorageService> _logger = logger;

    private FileFormat? _uploadedFileFormat;

    private bool IsImageFile(Stream image)
    {
        _uploadedFileFormat = _fileFormatInspector.DetermineFileFormat(image);

        if (_uploadedFileFormat is FileSignatures.Formats.Image)
        {
            return true;
        }

        return false;
    }

    public async Task<Image> UploadImageAsync(Stream image, Guid userId, CancellationToken ct = default)
    {
        if (!IsImageFile(image))
        {
            throw new Exception("File is not an image");
        }

        try
        {
            if (!await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BUCKET_NAME), ct))
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(BUCKET_NAME));
            }

            var imageId = Guid.CreateVersion7();
            var objectName = imageId.ToString();
            var args = new PutObjectArgs()
                .WithBucket(BUCKET_NAME)
                .WithObject(objectName)
                .WithContentType(_uploadedFileFormat!.MediaType) // We guarantee it's an image above and thus has a media type.
                .WithStreamData(image)
                .WithObjectSize(image.Length);

            var uploadedObject = await _minioClient.PutObjectAsync(args, cancellationToken: ct);

            if (uploadedObject.ResponseStatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                throw new MinioException("Minio upload forbidden");
            }
            else if (uploadedObject.ResponseStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new MinioException("Unclassified error when uploading to minio");
            }

            Image imageEntity = await _imageRepository.CreateAsync(new()
            {
                Id = imageId,
                UploaderId = userId,
            }, ct);

            return imageEntity;
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex, "Error occurred while uploading image to MinIO");
            throw;
        }
    }

    public async Task<string> GetImageUrlAsync(Guid imageId, CancellationToken ct = default)
    {
        return await _minioClient.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(BUCKET_NAME)
            .WithObject(imageId.ToString())
            .WithExpiry(double.ConvertToInteger<int>(TimeSpan.FromDays(1).TotalSeconds)));
    }

    public async Task<Image> RemoveImageAsync(Guid imageId, CancellationToken ct)
    {
        var image = await _imageRepository.GetByIdAsync(imageId, ct)
          ?? throw new ObjectNotFoundException($"Image with ID {imageId} not found");

        await _minioClient.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(BUCKET_NAME)
            .WithObject(image.Id.ToString()), ct);

        return image;
    }

}