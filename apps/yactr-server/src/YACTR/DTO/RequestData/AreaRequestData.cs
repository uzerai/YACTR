using NetTopologySuite.Geometries;

namespace YACTR.DTO.RequestData;

public record AreaRequestData(
    string Name,
    string Description,
    Point Location,
    MultiPolygon Boundary
);
