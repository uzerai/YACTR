using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace YACTR.Data.Model.Climbing;

/// <summary>
/// Represents a pitch of a climbing route.
/// 
/// A pitch in the context of the application, can be a rope-climbed pitch, or a boulder problem.
/// </summary>
public class Pitch : BaseEntity
{
    public string? Name { get; set; }
    public required ClimbingType Type { get; set; } = ClimbingType.Sport;
    public string? Description { get; set; }
    public int? Height { get; set; }
    public int PitchOrder { get; set; } = 0;

    public Guid? RouteSvgOverlayId { get; set; }
    [ForeignKey("RouteSvgOverlayId")]
    public Image? RouteSvgOverlay { get; set; }

    public Guid SectorId { get; set; }
    [JsonIgnore]
    [ForeignKey("SectorId")]
    public virtual Sector Sector { get; set; } = null!;

    public Guid RouteId { get; set; }
    [JsonIgnore]
    [ForeignKey("RouteId")]
    public virtual Route Route { get; set; } = null!;
}
