using NodaTime;
using YACTR.Domain.Model.Climbing.Rating;
using YACTR.Domain.Model.Climbing.Topo;

namespace YACTR.Domain.Model.Climbing;

/// <summary>
/// Represents a climbing route; from top to bottom.
/// 
/// A route must contain at least one pitch.
/// </summary>
public class Route : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int InSectorOrder { get; set; } = 0;
    public int? Grade { get; set; }
    public Instant? FirstAscentDate { get; set; }
    public string? FirstAscentClimberName { get; set; }
    public string? BolterName { get; set; }
    public required ClimbingType Type { get; set; }

    public required Guid SectorId { get; set; }
    public virtual Sector Sector { get; set; } = null!;
    public Guid? SectorTopoImageId { get; set; }
    public virtual Image? SectorTopoImage { get; set; }

    public virtual List<TopoLinePoint>? SectorTopoLinePoints { get; set; }
    public Guid? SectorTopoImageOverlaySvgId { get; set; }
    public virtual Image? SectorTopoImageOverlaySvg { get; set; }

    public Guid? TopoImageId { get; set; }
    public virtual Image? TopoImage { get; set; }

    public virtual List<TopoLinePoint>? TopoLinePoints { get; set; }
    public Guid? TopoImageOverlaySvgId { get; set; }
    public Image? TopoImageOverlaySvg { get; set; }

    public virtual ICollection<Pitch> Pitches { get; set; } = [];
    public virtual ICollection<RouteRating> RouteRatings { get; set; } = [];
    public virtual ICollection<RouteLike> RouteLikes { get; set; } = [];
}