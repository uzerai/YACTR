using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using YACTR.Model.Location;
using NodaTime;

namespace YACTR.DI.Repository;

public class PitchRepository : EntityRepository<Pitch>, IPitchRepository
{
    public PitchRepository(DatabaseContext context, IClock clock)
        : base(context, clock)
    {
    }
} 