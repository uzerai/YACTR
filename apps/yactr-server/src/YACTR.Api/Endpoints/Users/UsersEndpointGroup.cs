using FastEndpoints;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Users;

public record CurrentUserResponse(
    Guid Id,
    string Username,
    ICollection<Permission> PlatformPermissions,
    ICollection<Permission> AdminPermissions
);

public record UserResponse(
    Guid Id,
    string Username
);

public class UsersEndpointGroup : Group
{
    public UsersEndpointGroup()
    {
        Configure("users", ep => { });
    }
}