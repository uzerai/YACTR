using System.ComponentModel.DataAnnotations.Schema;
using YACTR.Domain.Model.Authentication;

namespace YACTR.Domain.Model.Climbing.Rating;

/// <summary>
/// Represents a user's rating of a climbing route.
/// </summary>
public class RouteRating : BaseEntity
{
    public required Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public required Guid RouteId { get; set; }
    [ForeignKey("RouteId")]
    public virtual Route Route { get; set; } = null!;

    public required int Rating { get; set; }
}