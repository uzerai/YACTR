using NetTopologySuite.Geometries;

namespace YACTR.Domain.Model.Climbing;

/// <summary>
/// Represents a sector of a climbing area; most commonly a single face of a wall,
/// or an area suitable to belay one or more routes.
/// </summary>
public class Sector : BaseEntity
{
    public required string Name { get; set; }

    public required Polygon SectorArea { get; set; }
    public required Point EntryPoint { get; set; }
    public Point? RecommendedParkingLocation { get; set; }
    public LineString? ApproachPath { get; set; }

    public required Guid AreaId { get; set; }
    public virtual Area Area { get; set; } = null!;

    public Guid? PrimarySectorImageId { get; set; }
    public virtual Image? PrimarySectorImage { get; set; }

    public virtual ICollection<SectorImage> SectorImages { get; set; } = [];

    public virtual ICollection<Route> Routes { get; set; } = [];
}
