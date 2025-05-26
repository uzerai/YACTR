using System.ComponentModel.DataAnnotations.Schema;
using YACTR.Model.Location;

namespace YACTR.Model.Achievement;

public class PitchAscent : BaseAscent
{
  [ForeignKey("Pitch")]
  public required Guid PitchId { get; set; }
  public virtual Pitch Pitch { get; set; } = null!;
}