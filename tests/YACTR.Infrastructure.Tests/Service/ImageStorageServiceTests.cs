using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using FileSignatures;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;
using NSubstitute;
using Shouldly;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model;
using YACTR.Infrastructure.FileFormatExtensions;
using YACTR.Infrastructure.Service;

namespace YACTR.Infrastructure.Tests.Service;

public class ImageStorageServiceTests
{
    [Fact]
    public async Task UploadImageAsync_ShouldThrow_WhenFileIsNotImage()
    {
        var minioClient = Substitute.For<IMinioClient>();
        var imageRepository = Substitute.For<IEntityRepository<Image>>();
        var fileFormatInspector = Substitute.For<IFileFormatInspector>();
        var logger = Substitute.For<ILogger<ImageStorageService>>();
        var service = new ImageStorageService(minioClient, imageRepository, fileFormatInspector, logger);
        using var stream = new MemoryStream([1, 2, 3, 4]);

        fileFormatInspector.DetermineFileFormat(Arg.Any<Stream>()).Returns((FileFormat?)null);

        var ex = await Should.ThrowAsync<Exception>(service.UploadImageAsync(stream, Guid.NewGuid(), TestContext.Current.CancellationToken));

        ex.Message.ShouldBe("File is not an image");
        await minioClient.DidNotReceiveWithAnyArgs().PutObjectAsync(default!);
    }

    [Fact]
    public async Task UploadImageAsync_ShouldCreateBucket_WhenBucketDoesNotExist()
    {
        var userId = Guid.NewGuid();
        var minioClient = Substitute.For<IMinioClient>();
        var imageRepository = Substitute.For<IEntityRepository<Image>>();
        var fileFormatInspector = Substitute.For<IFileFormatInspector>();
        var logger = Substitute.For<ILogger<ImageStorageService>>();
        var service = new ImageStorageService(minioClient, imageRepository, fileFormatInspector, logger);
        using var stream = new MemoryStream([1, 2, 3, 4]);

        fileFormatInspector.DetermineFileFormat(Arg.Any<Stream>()).Returns(new Svg());
        minioClient.BucketExistsAsync(Arg.Any<BucketExistsArgs>(), Arg.Any<CancellationToken>())
            .Returns(false);
        minioClient.PutObjectAsync(Arg.Any<PutObjectArgs>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(CreatePutObjectResponse(HttpStatusCode.OK));
        imageRepository.CreateAsync(Arg.Any<Image>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Image>());

        var result = await service.UploadImageAsync(stream, userId, TestContext.Current.CancellationToken);

        await minioClient.Received(1).MakeBucketAsync(Arg.Any<MakeBucketArgs>());
        await imageRepository.Received(1).CreateAsync(Arg.Any<Image>(), Arg.Any<CancellationToken>());
        result.UploaderId.ShouldBe(userId);
    }

    [Fact]
    public async Task UploadImageAsync_ShouldThrowMinioException_WhenResponseIsForbidden()
    {
        var minioClient = Substitute.For<IMinioClient>();
        var imageRepository = Substitute.For<IEntityRepository<Image>>();
        var fileFormatInspector = Substitute.For<IFileFormatInspector>();
        var logger = Substitute.For<ILogger<ImageStorageService>>();
        var service = new ImageStorageService(minioClient, imageRepository, fileFormatInspector, logger);
        using var stream = new MemoryStream([1, 2, 3, 4]);

        fileFormatInspector.DetermineFileFormat(Arg.Any<Stream>()).Returns(new Svg());
        minioClient.BucketExistsAsync(Arg.Any<BucketExistsArgs>(), Arg.Any<CancellationToken>())
            .Returns(true);
        minioClient.PutObjectAsync(Arg.Any<PutObjectArgs>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(CreatePutObjectResponse(HttpStatusCode.Forbidden));

        var ex = await Should.ThrowAsync<MinioException>(service.UploadImageAsync(stream, Guid.NewGuid(), TestContext.Current.CancellationToken));

        ex.Message.ShouldContain("Minio upload forbidden");
    }

    [Fact]
    public async Task UploadImageAsync_ShouldThrowMinioException_WhenResponseIsNotOk()
    {
        var minioClient = Substitute.For<IMinioClient>();
        var imageRepository = Substitute.For<IEntityRepository<Image>>();
        var fileFormatInspector = Substitute.For<IFileFormatInspector>();
        var logger = Substitute.For<ILogger<ImageStorageService>>();
        var service = new ImageStorageService(minioClient, imageRepository, fileFormatInspector, logger);
        using var stream = new MemoryStream([1, 2, 3, 4]);

        fileFormatInspector.DetermineFileFormat(Arg.Any<Stream>()).Returns(new Svg());
        minioClient.BucketExistsAsync(Arg.Any<BucketExistsArgs>(), Arg.Any<CancellationToken>())
            .Returns(true);
        minioClient.PutObjectAsync(Arg.Any<PutObjectArgs>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns(CreatePutObjectResponse(HttpStatusCode.BadRequest));

        var ex = await Should.ThrowAsync<MinioException>(service.UploadImageAsync(stream, Guid.NewGuid(), TestContext.Current.CancellationToken));

        ex.Message.ShouldContain("Unclassified error when uploading to minio");
    }

    [Fact]
    public async Task UploadImageAsync_ShouldRethrow_WhenMinioThrowsException()
    {
        var minioClient = Substitute.For<IMinioClient>();
        var imageRepository = Substitute.For<IEntityRepository<Image>>();
        var fileFormatInspector = Substitute.For<IFileFormatInspector>();
        var logger = Substitute.For<ILogger<ImageStorageService>>();
        var service = new ImageStorageService(minioClient, imageRepository, fileFormatInspector, logger);
        using var stream = new MemoryStream([1, 2, 3, 4]);

        fileFormatInspector.DetermineFileFormat(Arg.Any<Stream>()).Returns(new Svg());
        minioClient.BucketExistsAsync(Arg.Any<BucketExistsArgs>(), Arg.Any<CancellationToken>())
            .Returns(true);
        minioClient.PutObjectAsync(Arg.Any<PutObjectArgs>(), cancellationToken: Arg.Any<CancellationToken>())
            .Returns<Task<PutObjectResponse>>(_ => throw new MinioException("upstream failure"));

        var ex = await Should.ThrowAsync<MinioException>(service.UploadImageAsync(stream, Guid.NewGuid(), TestContext.Current.CancellationToken));

        ex.Message.ShouldContain("upstream failure");
    }

    private static PutObjectResponse CreatePutObjectResponse(HttpStatusCode statusCode)
    {
        var response = (PutObjectResponse)RuntimeHelpers.GetUninitializedObject(typeof(PutObjectResponse));
        SetFieldValueInHierarchy(typeof(PutObjectResponse), response, "<ResponseStatusCode>k__BackingField", statusCode);
        SetFieldValueInHierarchy(typeof(PutObjectResponse), response, "<StatusCode>k__BackingField", statusCode);

        return response;
    }

    private static void SetFieldValueInHierarchy(Type type, object instance, string fieldName, object value)
    {
        var current = type;
        while (current != null)
        {
            var field = current.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(instance, value);
                return;
            }

            current = current.BaseType!;
        }
    }
}
