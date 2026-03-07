using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Tests;

/// <summary>
/// Seeder for the integration tests.
/// This is used to seed the database with test data for the integration tests.
/// </summary>
/// <param name="context">The database context.</param>
/// <param name="testDataFactory">Test object factory</param>
/// <param name="imageStorageService">The image storage service.</param>
public class IntegrationTestDataSeeder(
    DatabaseContext context,
    TestDataFactory testDataFactory,
    IImageStorageService imageStorageService)
{
    public async Task<User> SeedUserAsync()
    {
        User? user = await context.Users
            .FirstOrDefaultAsync(x => x.Auth0UserId == "seeded_user:cantbematchedforauth");

        if (user == null)
        {
            user = context.Users.Add(new()
            {
                Auth0UserId = "seeded_user:cantbematchedforauth",
                Email = "test@test.com",
                Username = "test",
            }).Entity;

            await context.SaveChangesAsync();
        }

        return user;
    }

    public async Task<(Area area, Sector sector, ICollection<Route> routes)> SeedAreaWithSectorAndRouteAsync()
    {
        User user = await SeedUserAsync();

        Area area = new()
        {
            Name = "Test Area",
            Description = "Test area description",
            Location = testDataFactory.NewPoint(),
            Boundary = testDataFactory.NewMultiPolygon()
        };

        await context.AddAsync(area);
        Image image = await CreateImageAsync();

        Sector sector = new()
        {
            Name = "Test Sector",
            AreaId = area.Id,
            SectorArea = testDataFactory.NewPolygon(),
            EntryPoint = testDataFactory.NewPoint(),
            RecommendedParkingLocation = testDataFactory.NewPoint(),
            ApproachPath = testDataFactory.NewLineString(),
            PrimarySectorImageId = image.Id,
        };

        await context.AddAsync(sector);

        ICollection<Route> routes = Enum.GetValues<ClimbingType>()
            .Select(type => new Route
            {
                Name = "Test Route",
                Description = "Test route for ascents",
                Type = type,
                Grade = 500,
                SectorId = sector.Id
            }).ToList();

        await context.AddRangeAsync(routes);
        await context.SaveChangesAsync();

        return (area, sector, routes);
    }

    public async Task<Image> CreateImageAsync()
    {
        return await CreateImageAsync(TestDataConstants.MINIMAL_JPEG);
    }

    public async Task<Image> CreateImageAsync(byte[] image)
    {
        return await imageStorageService.UploadImageAsync(new MemoryStream(image), context.Users.First().Id, CancellationToken.None);
    }
}