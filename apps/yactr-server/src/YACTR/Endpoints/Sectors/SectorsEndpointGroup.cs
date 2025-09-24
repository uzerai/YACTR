using FastEndpoints;
using NetTopologySuite.Geometries;

namespace YACTR.Endpoints.Sectors;

/// <summary>
/// Request data for creating and updating the <see cref="Sector"/> entity.
/// </summary>
/// <param name="Name">The name of the sector.</param>
/// <param name="SectorArea">The area of the sector.</param>
/// <param name="EntryPoint">The entry point of the sector.</param>
public record SectorRequestData(
    string Name,
    Polygon SectorArea,
    Point EntryPoint,
    Point? RecommendedParkingLocation,
    LineString? ApproachPath,
    Guid AreaId);

public class SectorsEndpointGroup : Group
{
    public SectorsEndpointGroup()
    {
        Configure("sectors", ep => {});
    }
} 