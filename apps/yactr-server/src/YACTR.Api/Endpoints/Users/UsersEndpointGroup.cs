using FastEndpoints;

using NodaTime;

using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Users;

public record CurrentUserResponse(
    ICollection<Permission> PlatformPermissions,
    ICollection<Permission> AdminPermissions,
    Guid Id,
    string Username,
    Instant CreatedAt
) : UserResponse(Id, Username, CreatedAt);

public record UserResponse(
    Guid Id,
    string Username,
    Instant CreatedAt
);

public class UsersEndpointGroup : Group
{
    public UsersEndpointGroup()
    {
        Configure("users", ep => { });
    }
}