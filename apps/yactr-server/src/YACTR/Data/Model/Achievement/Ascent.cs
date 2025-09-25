using System.ComponentModel.DataAnnotations.Schema;
using YACTR.Data.Model.Authentication;
using NodaTime;
using Route = YACTR.Data.Model.Climbing.Route;

namespace YACTR.Data.Model.Achievement;

/// <summary>
/// Represents a user's ascent of a climbing route.
/// </summary>
public class Ascent : BaseEntity
{
    public required AscentType Type { get; set; } = AscentType.Tick;
    public required Instant CompletedAt { get; set; }
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    [ForeignKey("Route")]
    public required Guid RouteId { get; set; }
    public virtual Route Route { get; set; } = null!;
}