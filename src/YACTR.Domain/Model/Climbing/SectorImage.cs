using System.ComponentModel.DataAnnotations.Schema;

namespace YACTR.Domain.Model.Climbing;

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
