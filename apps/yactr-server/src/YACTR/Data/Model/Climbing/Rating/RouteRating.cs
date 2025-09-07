using YACTR.Data.Model.Authentication;

namespace YACTR.Data.Model.Climbing.Rating;

/// <summary>
/// Represents a user's rating of a climbing route.
/// </summary>
public class RouteRating : BaseEntity
{
    public required Guid UserId { get; set; }
    public virtual User User { get; set; } = null!;

    public required Guid RouteId { get; set; }
    public virtual Route Route { get; set; } = null!;

    public required int Rating { get; set; }
}