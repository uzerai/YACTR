
using YACTR.DI.Repository.Interface;
using NodaTime;

using YACTR.DI.Data;
using YACTR.Model.Authentication;

namespace YACTR.DI.Repository;

public class UserRepository : EntityRepository<User>, IUserRepository
{
    public UserRepository(DatabaseContext context, IClock clock) : base(context, clock)
    {
    }
}
