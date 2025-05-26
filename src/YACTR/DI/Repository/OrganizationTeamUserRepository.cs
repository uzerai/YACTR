using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using YACTR.Model.Organizations;

namespace YACTR.DI.Repository;

public class OrganizationTeamUserRepository : BaseRepository<OrganizationTeamUser>, IOrganizationTeamUserRepository
{
    public OrganizationTeamUserRepository(DatabaseContext context)
        : base(context)
    {
    }
}
