using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using NodaTime;
using Route = YACTR.Model.Location.Route;

namespace YACTR.DI.Repository;

public class RouteRepository : EntityRepository<Route>, IRouteRepository
{
    public RouteRepository(DatabaseContext context, IClock clock)
        : base(context, clock)
    {
    }
} 