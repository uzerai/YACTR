using YACTR.DI.Data;
using YACTR.DI.Repository.Interface;
using YACTR.Model.Organizations;

namespace YACTR.DI.Repository;

public class OrganizationUserRepository : BaseRepository<OrganizationUser>, IOrganizationUserRepository
{
    public OrganizationUserRepository(DatabaseContext context)
        : base(context)
    {
    }
}