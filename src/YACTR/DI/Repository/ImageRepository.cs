using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using YACTR.Model;

namespace YACTR.DI.Repository;

public class ImageRepository : EntityRepository<Image>, IEntityRepository<Image>
{
    public ImageRepository(DatabaseContext context) : base(context)
    {
    }
}