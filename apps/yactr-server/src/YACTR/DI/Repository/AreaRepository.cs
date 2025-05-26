using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using YACTR.Model.Location;
using NodaTime;

namespace YACTR.DI.Repository;

public class AreaRepository : EntityRepository<Area>, IAreaRepository
{
    public AreaRepository(DatabaseContext context, IClock clock)
        : base(context, clock)
    {
    }
} 