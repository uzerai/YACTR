using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace YACTR.Data.Model.Location;

[PrimaryKey(nameof(RouteId), nameof(PitchId))]
public class RoutePitch
{
    [ForeignKey("Route")]
    public required Guid RouteId { get; set; }
    [ForeignKey("Pitch")]
    public required Guid PitchId { get; set; }
    public required int PitchNumber { get; set; } = 0;

    public virtual Route Route { get; set; } = null!;
    public virtual Pitch Pitch { get; set; } = null!;
}