using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace YACTR.Data.Model.Climbing;

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

    [ForeignKey("Area")]
    public required Guid AreaId { get; set; }
    public virtual Area Area { get; set; } = null!;

    [ForeignKey("SectorImage")]
    public Guid? SectorImageId { get; set; }
    public virtual Image? SectorImage { get; set; }

    public virtual ICollection<Route> Routes { get; set; } = [];
}
