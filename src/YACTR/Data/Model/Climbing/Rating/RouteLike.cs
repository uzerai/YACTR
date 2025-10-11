using System.ComponentModel.DataAnnotations.Schema;
using YACTR.Data.Model.Authentication;

namespace YACTR.Data.Model.Climbing.Rating;

/// <summary>
/// Represents a user's like of a climbing route.
/// </summary>
public class RouteLike : BaseEntity
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public required Guid RouteId { get; set; }
    [ForeignKey("RouteId")]
    public virtual Route Route { get; set; } = null!;
}