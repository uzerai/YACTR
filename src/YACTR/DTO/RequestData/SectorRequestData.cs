using NetTopologySuite.Geometries;

namespace YACTR.DTO.RequestData;

public record SectorRequestData(
    string Name,
    Polygon SectorArea,
    Point EntryPoint,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    Guid AreaId);