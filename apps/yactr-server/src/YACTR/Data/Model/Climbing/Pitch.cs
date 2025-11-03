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
    /// <summary>
    /// The name of the pitch, if any.
    /// <br/>
    /// Ideally identical to the name of the route if the route is a single-pitch one.
    /// </summary>
    public string? Name { get; set; }
    public required ClimbingType Type { get; set; } = ClimbingType.Sport;
    public string? Description { get; set; }
    /// <summary>
    /// An attempted measure of the difficulty of the climb.
    /// </summary>
    public int Grade { get; set; }
    /// <summary>
    /// The height of the pitch in meters.
    /// </summary>
    public int? Height { get; set; }
    /// <summary>
    /// The number of pieces of gear (quickdraws or otherwise) required for the Pitch.
    /// </summary>
    public int? GearCount { get; set; }
    /// <summary>
    /// Which pitch number within a route this pitch is.
    /// </summary>
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
