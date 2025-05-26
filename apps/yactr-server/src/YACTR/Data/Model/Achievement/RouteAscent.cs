using System.ComponentModel.DataAnnotations.Schema;
using Route = YACTR.Data.Model.Location.Route;

namespace YACTR.Data.Model.Achievement;

public class RouteAscent : BaseAscent
{
  [ForeignKey("Route")]
  public required Guid RouteId { get; set; }
  public virtual Route Route { get; set; } = null!;
}

