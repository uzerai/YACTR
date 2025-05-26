using NodaTime;
using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using YACTR.Model.Organizations;

namespace YACTR.DI.Repository;

public class OrganizationRepository : EntityRepository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(DatabaseContext context, IClock clock)
        : base(context, clock)
    {
    }
}