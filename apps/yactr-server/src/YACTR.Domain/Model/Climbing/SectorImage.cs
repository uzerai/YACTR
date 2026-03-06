namespace YACTR.Domain.Model.Climbing;

public class SectorImage
{
    public int Order { get; set; }
    public Guid SectorId { get; set; }
    public virtual Sector Sector { get; set; } = null!;

    public Guid ImageId { get; set; }
    public virtual Image Image { get; set; } = null!;
}
