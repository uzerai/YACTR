using System.Text.Json;
using NetTopologySuite.Geometries;
using NodaTime;
using YACTR.Data;
using YACTR.Data.Model.Climbing;

namespace YACTR.Tests.TestData;

public class TauForerDataLoader
{
    public class TauForerData
    {
        public int NumRegions { get; set; }
        public int NumAreas { get; set; }
        public int NumSectors { get; set; }
        public int NumProblems { get; set; }
        public List<TauForerRegion> Regions { get; set; } = [];
    }

    public class TauForerRegion
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TauForerArea> Areas { get; set; } = [];
    }

    public class TauForerArea
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public TauForerCoordinates Coordinates { get; set; } = new();
        public bool LockedAdmin { get; set; }
        public bool LockedSuperadmin { get; set; }
        public int SunFromHour { get; set; }
        public int SunToHour { get; set; }
        public List<TauForerSector> Sectors { get; set; } = [];
    }

    public class TauForerSector
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Sorting { get; set; }
        public TauForerCoordinates? Parking { get; set; }
        public List<TauForerCoordinates> Outline { get; set; } = [];
        public TauForerWallDirection? WallDirectionCalculated { get; set; }
        public bool LockedAdmin { get; set; }
        public bool LockedSuperadmin { get; set; }
        public int SunFromHour { get; set; }
        public int SunToHour { get; set; }
        public List<TauForerProblem> Problems { get; set; } = [];
    }

    public class TauForerProblem
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public bool LockedAdmin { get; set; }
        public bool LockedSuperadmin { get; set; }
        public int Nr { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TauForerCoordinates Coordinates { get; set; } = new();
        public string Grade { get; set; } = string.Empty;
        public string? Fa { get; set; }
        public int FaYear { get; set; }
        public int NumTicks { get; set; }
        public double Stars { get; set; }
        public bool Ticked { get; set; }
        public bool Todo { get; set; }
        public TauForerType T { get; set; } = new();
        public int NumPitches { get; set; }
    }

    public class TauForerCoordinates
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Elevation { get; set; }
        public string ElevationSource { get; set; } = string.Empty;
        public double Distance { get; set; }
    }

    public class TauForerWallDirection
    {
        public int Id { get; set; }
        public string Direction { get; set; } = string.Empty;
    }

    public class TauForerType
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string SubType { get; set; } = string.Empty;
    }

    public static TauForerData LoadFromFile(string filePath)
    {
        var jsonContent = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Deserialize<TauForerData>(jsonContent, options)
               ?? throw new InvalidOperationException("Failed to deserialize JSON data");
    }

    public static async Task<(IEnumerable<Area> areas, IEnumerable<Sector> sectors, IEnumerable<Route> routes)> LoadAndSaveToDatabaseAsync(DatabaseContext context)
    {
        // Get the directory where the test assembly is located
        var testAssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var testDirectory = Path.GetDirectoryName(testAssemblyLocation);
        var jsonFilePath = Path.Combine(testDirectory!, "TestData", "tau.forer.no.json");

        if (!File.Exists(jsonFilePath))
        {
            throw new FileNotFoundException($"Test data file not found: {jsonFilePath}");
        }

        // Load and parse the JSON data
        var tauForerData = LoadFromFile(jsonFilePath);

        // Create ID mappings
        var areaIdMapping = CreateAreaIdMapping(tauForerData);
        var sectorIdMapping = CreateSectorIdMapping(tauForerData);

        // Convert to our entity models
        var areas = ConvertToAreas(tauForerData, areaIdMapping);
        var sectors = ConvertToSectors(tauForerData, areaIdMapping, sectorIdMapping);
        var routes = ConvertToRoutes(tauForerData, sectorIdMapping);

        // Add to database
        await context.Areas.AddRangeAsync(areas);
        await context.Sectors.AddRangeAsync(sectors);
        await context.Routes.AddRangeAsync(routes);

        await context.SaveChangesAsync();

        return (areas, sectors, routes);
    }

    private static Dictionary<int, Guid> CreateAreaIdMapping(TauForerData data)
    {
        var mapping = new Dictionary<int, Guid>();

        foreach (var region in data.Regions)
        {
            foreach (var area in region.Areas)
            {
                // Generate a new GUID for each area
                mapping[area.Id] = Guid.NewGuid();
            }
        }

        return mapping;
    }

    private static Dictionary<int, Guid> CreateSectorIdMapping(TauForerData data)
    {
        var mapping = new Dictionary<int, Guid>();

        foreach (var region in data.Regions)
        {
            foreach (var area in region.Areas)
            {
                foreach (var sector in area.Sectors)
                {
                    // Generate a new GUID for each sector
                    mapping[sector.Id] = Guid.NewGuid();
                }
            }
        }

        return mapping;
    }

    public static List<Area> ConvertToAreas(TauForerData data, Dictionary<int, Guid> areaIdMapping)
    {
        var areas = new List<Area>();

        foreach (var region in data.Regions)
        {
            foreach (var tauForerArea in region.Areas)
            {
                var area = new Area
                {
                    Id = areaIdMapping[tauForerArea.Id],
                    Name = tauForerArea.Name,
                    Description = $"Imported from tau.forer.no - {region.Name}",
                    Location = CreatePoint(tauForerArea.Coordinates.Latitude, tauForerArea.Coordinates.Longitude),
                    Boundary = CreateBoundaryFromSectors(tauForerArea.Sectors),
                    MaintainerOrganizationId = null // No organization mapping in the data
                };

                areas.Add(area);
            }
        }

        return areas;
    }

    public static List<Sector> ConvertToSectors(TauForerData data, Dictionary<int, Guid> areaIdMapping, Dictionary<int, Guid> sectorIdMapping)
    {
        var sectors = new List<Sector>();

        foreach (var region in data.Regions)
        {
            foreach (var tauForerArea in region.Areas)
            {
                var areaId = areaIdMapping[tauForerArea.Id];

                foreach (var tauForerSector in tauForerArea.Sectors)
                {
                    var sector = new Sector
                    {
                        Id = sectorIdMapping[tauForerSector.Id],
                        Name = tauForerSector.Name,
                        SectorArea = CreatePolygonFromOutline(tauForerSector.Outline),
                        EntryPoint = CreatePoint(tauForerSector.Outline.FirstOrDefault()?.Latitude ?? 0,
                                               tauForerSector.Outline.FirstOrDefault()?.Longitude ?? 0),
                        RecommendedParkingLocation = tauForerSector.Parking != null
                            ? CreatePoint(tauForerSector.Parking.Latitude, tauForerSector.Parking.Longitude)
                            : null,
                        ApproachPath = null, // Not available in the data
                        AreaId = areaId
                    };

                    sectors.Add(sector);
                }
            }
        }

        return sectors;
    }

    public static List<Route> ConvertToRoutes(TauForerData data, Dictionary<int, Guid> sectorIdMapping)
    {
        var routes = new List<Route>();

        foreach (var region in data.Regions)
        {
            foreach (var tauForerArea in region.Areas)
            {
                foreach (var tauForerSector in tauForerArea.Sectors)
                {
                    var sectorId = sectorIdMapping[tauForerSector.Id];

                    foreach (var tauForerProblem in tauForerSector.Problems)
                    {
                        var route = new Route
                        {
                            Id = Guid.NewGuid(),
                            Name = tauForerProblem.Name,
                            Description = tauForerProblem.Description,
                            Grade = tauForerProblem.Grade,
                            FirstAscentDate = tauForerProblem.FaYear > 0
                                ? Instant.FromDateTimeUtc(new DateTime(tauForerProblem.FaYear, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                                : null,
                            FirstAscentClimberName = tauForerProblem.Fa,
                            BolterName = null, // Not available in the data
                            SectorId = sectorId,
                            TopoImageId = null,
                            Type = MapClimbingType(tauForerProblem.T.SubType)
                        };

                        routes.Add(route);
                    }
                }
            }
        }

        return routes;
    }

    private static Point CreatePoint(double latitude, double longitude)
    {
        return new Point(longitude, latitude) { SRID = 4326 };
    }

    private static Polygon CreatePolygonFromOutline(List<TauForerCoordinates> outline)
    {
        if (outline.Count < 3)
        {
            // Create a simple polygon if not enough points
            var centerLat = outline.FirstOrDefault()?.Latitude ?? 0;
            var centerLon = outline.FirstOrDefault()?.Longitude ?? 0;
            var offset = 0.001; // Small offset for a simple polygon

            var coordinates = new Coordinate[]
            {
                new(centerLon - offset, centerLat - offset),
                new(centerLon + offset, centerLat - offset),
                new(centerLon + offset, centerLat + offset),
                new(centerLon - offset, centerLat + offset),
                new(centerLon - offset, centerLat - offset) // Close the ring
            };

            return new Polygon(new LinearRing(coordinates)) { SRID = 4326 };
        }

        var outlineCoordinates = outline.Select(c => new Coordinate(c.Longitude, c.Latitude)).ToArray();
        // Close the ring if not already closed
        if (outlineCoordinates.First() != outlineCoordinates.Last())
        {
            var closedCoordinates = new Coordinate[outlineCoordinates.Length + 1];
            Array.Copy(outlineCoordinates, closedCoordinates, outlineCoordinates.Length);
            closedCoordinates[outlineCoordinates.Length] = outlineCoordinates[0];
            outlineCoordinates = closedCoordinates;
        }

        return new Polygon(new LinearRing(outlineCoordinates)) { SRID = 4326 };
    }

    private static MultiPolygon CreateBoundaryFromSectors(List<TauForerSector> sectors)
    {
        var polygons = sectors.Select(s => CreatePolygonFromOutline(s.Outline)).ToArray();
        return new MultiPolygon(polygons) { SRID = 4326 };
    }

    private static ClimbingType MapClimbingType(string subType)
    {
        return subType.ToLowerInvariant() switch
        {
            "bolt" => ClimbingType.Sport,
            "trad" => ClimbingType.Traditional,
            "boulder" => ClimbingType.Boulder,
            "mixed" => ClimbingType.Mixed,
            "aid" => ClimbingType.Aid,
            "top rope" => ClimbingType.Sport, // Map top rope to sport
            _ => ClimbingType.Sport // Default to sport
        };
    }
}
