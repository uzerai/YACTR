using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using YACTR.Model.Organizations;

namespace YACTR.DI.Repository;

public class OrganizationTeamRepository : EntityRepository<OrganizationTeam>, IOrganizationTeamRepository
{
    public OrganizationTeamRepository(DatabaseContext context, IClock clock)
        : base(context, clock)
    {
    }

    public async Task<IEnumerable<OrganizationTeam>> GetAllForOrganizationAsync(Guid organizationId)
    {
        return await BuildReadonlyQuery()
            .Where(e => e.OrganizationId == organizationId)
            .ToListAsync();
    }
}