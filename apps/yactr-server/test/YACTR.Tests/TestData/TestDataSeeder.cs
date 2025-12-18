using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using YACTR.Data;
using YACTR.Data.Model;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Model.Climbing;
using YACTR.DI.Service;
using Area = YACTR.Data.Model.Climbing.Area;

namespace YACTR.Tests.TestData;

public class TestDataSeeder
{
    private readonly DatabaseContext _context;
    private readonly GeometryFactory _geometryFactory;
    private readonly IImageStorageService _imageStorageService;
    public TestDataSeeder(DatabaseContext context, IServiceProvider services)
    {
        _context = context;
        _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        _imageStorageService = services.GetRequiredService<IImageStorageService>();
    }

    public Point NewPoint()
    {
        return _geometryFactory.CreatePoint(
            new Coordinate(-122.4194, 37.7749)
        );
    }

    public Polygon NewPolygon()
    {
        return _geometryFactory.CreatePolygon([
            new Coordinate(-122.42, 37.77),
            new Coordinate(-122.42, 37.78),
            new Coordinate(-122.41, 37.78),
            new Coordinate(-122.41, 37.77),
            new Coordinate(-122.42, 37.77)
        ]);
    }

    public MultiPolygon NewMultiPolygon()
    {
        return _geometryFactory.CreateMultiPolygon([
            NewPolygon()
        ]);
    }

    public LineString NewLineString()
    {
        return _geometryFactory.CreateLineString([
            new Coordinate(-122.42, 37.77),
            new Coordinate(-122.42, 37.78),
        ]);
    }

    public async Task<User> CreateUserAsync()
    {

        User? user = await _context.Users.Where(x => x.Auth0UserId == "seeded_user:cantbematchedforauth").FirstOrDefaultAsync();

        if (user == null)
        {
            user = _context.Users.Add(new()
            {
                Auth0UserId = "seeded_user:cantbematchedforauth",
                Email = "test@test.com",
                Username = "test",
            }).Entity;

            await _context.SaveChangesAsync();
        }

        return user;
    }


    public async Task<(Area area, Sector sector, ICollection<Route> routes)> SeedAreaWithSectorAndRouteAsync()
    {
        User user = await CreateUserAsync();

        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        Area area = new()
        {
            Name = "Test Area",
            Description = "Test area description",
            Location = NewPoint(),
            Boundary = NewMultiPolygon()
        };

        await _context.AddAsync(area);
        Image image = await CreateImageAsync();

        Sector sector = new()
        {
            Name = "Test Sector",
            AreaId = area.Id,
            SectorArea = NewPolygon(),
            EntryPoint = NewPoint(),
            RecommendedParkingLocation = NewPoint(),
            ApproachPath = NewLineString(),
            PrimarySectorImageId = image.Id,
        };

        await _context.AddAsync(sector);

        ICollection<Route> routes = Enum.GetValues<ClimbingType>()
            .Select<ClimbingType, Route>(type => new()
            {
                Name = "Test Route",
                Description = "Test route for ascents",
                Type = type,
                Grade = 500,
                SectorId = sector.Id
            }).ToList();

        await _context.AddRangeAsync(routes);
        await _context.SaveChangesAsync();

        return (area, sector, routes);
    }

    public async Task<Image> CreateImageAsync()
    {
        return await CreateImageAsync(TestDataConstants.MINIMAL_JPEG);
    }

    public async Task<Image> CreateImageAsync(byte[] image)
    {
        return await _imageStorageService.UploadImageAsync(new MemoryStream(image), _context.Users.First().Id, CancellationToken.None);
    }
}