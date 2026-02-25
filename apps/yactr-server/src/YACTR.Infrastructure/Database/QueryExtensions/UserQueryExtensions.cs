using YACTR.Domain.Model.Authentication;

namespace YACTR.Infrastructure.Database.QueryExtensions;

public static class UserQueryExtensions
{
    public static IQueryable<User> WhereAuth0UserId(this IQueryable<User> query, string auth0UserId) => query.Where(x => x.Auth0UserId == auth0UserId);
}
