using NetTopologySuite;
using NetTopologySuite.Geometries;
using YACTR.Data;
using YACTR.Data.Model.Climbing;

using Area = YACTR.Data.Model.Climbing.Area;

namespace YACTR.Tests.TestData
{
    public class TestDataSeeder
    {
        private readonly DatabaseContext _context;
        private readonly GeometryFactory _geometryFactory;

        public TestDataSeeder(DatabaseContext context)
        {
            _context = context;
            _geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

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

        public async Task<(Area area, Sector sector, ICollection<Route> routes)> SeedAreaWithSectorAndRouteAsync()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            Area area = new()
            {
                Name = "Test Area",
                Description = "Test area description",
                Location = NewPoint(),
                Boundary = NewMultiPolygon()
            };

            await _context.AddAsync(area);

            Sector sector = new()
            {
                Name = "Test Sector",
                AreaId = area.Id,
                SectorArea = NewPolygon(),
                EntryPoint = NewPoint(),
                RecommendedParkingLocation = NewPoint(),
                ApproachPath = NewLineString()
            };

            await _context.AddAsync(sector);

            ICollection<Route> routes = Enum.GetValues<ClimbingType>()
                .Select<ClimbingType, Route>(type => new()
                {
                    Name = "Test Route",
                    Description = "Test route for ascents",
                    Type = type,
                    Grade = "5.10a",
                    SectorId = sector.Id
                }).ToList();

            await _context.AddRangeAsync(routes);
            await _context.SaveChangesAsync();

            return (area, sector, routes);
        }

    }
}