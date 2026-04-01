using FastEndpoints.Testing;
using Microsoft.AspNetCore.Http;
using YACTR.Api.Tests.TestData;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Tests.EndpointTests.Images;

public abstract class ImageEndpointTestsBase(ApiTestClassFixture fixture) : TestBase<ApiTestClassFixture>
{
    protected ApiTestClassFixture Fixture => fixture;

    protected User TestUserWithImagePermissions = new()
    {
        Username = "test_user_with_image_permissions",
        Email = "test_user@test.dev",
        Auth0UserId = $"test|{Guid.NewGuid()}",
        PlatformPermissions = Enum.GetValues<Permission>()
    };

    protected readonly IFormFile TestFile = new FormFile(
        baseStream: new MemoryStream(TestDataConstants.MINIMAL_JPEG),
        baseStreamOffset: 0,
        length: TestDataConstants.MINIMAL_JPEG.Length,
        name: "image",
        fileName: "test.jpeg"
    )
    {
        Headers = new HeaderDictionary(),
        ContentType = "image/jpeg"
    };

    protected readonly IFormFile TestSvgFile = new FormFile(
        baseStream: new MemoryStream(TestDataConstants.MINIMAL_SVG),
        baseStreamOffset: 0,
        length: TestDataConstants.MINIMAL_SVG.Length,
        name: "image",
        fileName: "test.svg"
    )
    {
        Headers = new HeaderDictionary(),
        ContentType = "image/svg+xml"
    };

    protected override async ValueTask SetupAsync()
    {
        await base.SetupAsync();

        await fixture.GetEntityRepository<User>().CreateAsync(TestUserWithImagePermissions, TestContext.Current.CancellationToken);
    }
}
