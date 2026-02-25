using System.ComponentModel.DataAnnotations.Schema;
using NodaTime;
using YACTR.Domain.Model.Authentication;
using Route = YACTR.Domain.Model.Climbing.Route;

namespace YACTR.Domain.Model.Achievement;

/// <summary>
/// Represents a user's ascent of a climbing route.
/// </summary>
public class Ascent : BaseEntity
{
    public required AscentType Type { get; set; } = AscentType.Tick;
    public required Instant CompletedAt { get; set; }
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public required Guid RouteId { get; set; }
    [ForeignKey("RouteId")]
    public virtual Route Route { get; set; } = null!;
}