using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using YACTR.Data.Model.Climbing.Rating;

namespace YACTR.Data.Model.Climbing;

/// <summary>
/// Represents a climbing route; from top to bottom.
/// 
/// A route must contain at least one pitch.
/// </summary>
public class Route : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Grade { get; set; }
    public Instant? FirstAscentDate { get; set; }
    public string? FirstAscentClimberName { get; set; }
    public string? BolterName { get; set; }
    public ClimbingType Type { get; set; }

    public Guid SectorId { get; set; }
    [ForeignKey("SectorId")]
    public virtual Sector Sector { get; set; } = null!;

    public Guid? SectorSvgOverlayId { get; set; }
    [ForeignKey("SectorSvgOverlayId")]
    public virtual Image? SectorSvgOverlay { get; set; }

    public Guid? TopoImageId { get; set; }
    [ForeignKey("TopoImageId")]
    public virtual Image? TopoImage { get; set; }

    public Guid? TopoImageOverlaySvgId { get; set; }
    [ForeignKey("TopoImageOverlaySvgId")]
    public Image? TopoImageOverlaySvg { get; set; }

    public virtual ICollection<Pitch> Pitches { get; set; } = [];
    public virtual ICollection<RouteRating> RouteRatings { get; set; } = [];
    public virtual ICollection<RouteLike> RouteLikes { get; set; } = [];

}