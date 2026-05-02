using Microsoft.EntityFrameworkCore;

using NodaTime;

using YACTR.Domain.Model;
using YACTR.Domain.Model.Authentication;
using YACTR.Domain.Model.Climbing;
using YACTR.Infrastructure.Database;
using YACTR.Infrastructure.Service;

namespace YACTR.Api.Tests.TestData;

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
    MutableTestClock testClock,
    IImageStorageService imageStorageService)
{
    private int _seedSequence = 0;

    public sealed record SeedAreaWithSectorAndRouteOptions(
        string NamePrefix,
        Instant? AreaCreatedAt = null,
        Instant? SectorCreatedAt = null,
        Instant? FirstRouteCreatedAt = null,
        Duration? RouteCreatedAtStep = null
    );

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

    public async Task<(Area area, Sector sector, ICollection<Route> routes)> SeedAreaWithSectorAndRouteAsync(
        SeedAreaWithSectorAndRouteOptions? options = null)
    {
        var sequence = ++_seedSequence;
        options ??= new SeedAreaWithSectorAndRouteOptions($"Seed{sequence}");

        User user = await SeedUserAsync();
        var routeCreatedAtStep = options.RouteCreatedAtStep ?? Duration.FromMinutes(1);
        var defaultStart = testClock.GetCurrentInstant();
        var areaCreatedAt = options.AreaCreatedAt ?? defaultStart;
        var sectorCreatedAt = options.SectorCreatedAt ?? areaCreatedAt.Plus(Duration.FromMinutes(1));
        var firstRouteCreatedAt = options.FirstRouteCreatedAt ?? sectorCreatedAt.Plus(Duration.FromMinutes(1));

        CountryData country = new()
        {
            CountryName = $"{options.NamePrefix} Country",
            AdminName = $"{options.NamePrefix} Admin",
            Code = $"T{sequence:000}",
            Continent = $"{options.NamePrefix} Continent",
            Region = $"{options.NamePrefix} Region",
            Subregion = $"{options.NamePrefix} Subregion",
            WorldBlock = $"{options.NamePrefix} WorldBlock",
            Geometry = testDataFactory.NewMultiPolygon()
        };

        await context.AddAsync(country);
        await context.SaveChangesAsync();

        testClock.SetCurrentInstant(areaCreatedAt);
        Area area = new()
        {
            Name = $"{options.NamePrefix} Area",
            Description = $"{options.NamePrefix} area description",
            Location = testDataFactory.NewPoint(),
            Boundary = testDataFactory.NewMultiPolygon(),
            CountryId = country.Id,
            Country = country
        };

        await context.AddAsync(area);
        await context.SaveChangesAsync();

        testClock.SetCurrentInstant(sectorCreatedAt);
        Image image = await CreateImageAsync();

        Sector sector = new()
        {
            Name = $"{options.NamePrefix} Sector",
            AreaId = area.Id,
            SectorArea = testDataFactory.NewPolygon(),
            EntryPoint = testDataFactory.NewPoint(),
            RecommendedParkingLocation = testDataFactory.NewPoint(),
            ApproachPath = testDataFactory.NewLineString(),
            PrimarySectorImageId = image.Id,
        };

        await context.AddAsync(sector);
        await context.SaveChangesAsync();

        List<Route> routes = [];
        var routeCreatedAt = firstRouteCreatedAt;
        foreach (var type in Enum.GetValues<ClimbingType>())
        {
            testClock.SetCurrentInstant(routeCreatedAt);
            var route = new Route
            {
                Name = $"{options.NamePrefix} Route {type}",
                Description = $"{options.NamePrefix} route for ascents",
                Type = type,
                Grade = 500,
                SectorId = sector.Id
            };

            await context.AddAsync(route);
            await context.SaveChangesAsync();
            routes.Add(route);
            routeCreatedAt = routeCreatedAt.Plus(routeCreatedAtStep);
        }

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