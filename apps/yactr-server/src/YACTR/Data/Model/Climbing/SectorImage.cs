using System.ComponentModel.DataAnnotations.Schema;
using YACTR.Data.Model.Climbing;

namespace YACTR.Data.Model;

public class SectorImage
{
    public int Order { get; set; }
    public Guid SectorId { get; set; }
    [ForeignKey("SectorId")]
    public virtual Sector Sector { get; set; } = null!;

    public Guid ImageId { get; set; }
    [ForeignKey("ImageId")]
    public virtual Image Image { get; set; } = null!;
}
