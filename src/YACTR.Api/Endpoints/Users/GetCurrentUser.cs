using FastEndpoints;
using NodaTime;
using YACTR.Domain.Model.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Users;

public record GetCurrentUserUserResponse(
    Guid Id,
    string Username,
    Instant CreatedAt
);

public record GetCurrentUserResponse(
    ICollection<Permission> PlatformPermissions,
    ICollection<Permission> AdminPermissions,
    Guid Id,
    string Username,
    Instant CreatedAt
) : GetCurrentUserUserResponse(Id, Username, CreatedAt);

public class GetCurrentUser : AuthenticatedEndpoint<EmptyRequest, GetCurrentUserResponse>
{
    public override void Configure()
    {
        Get("/me");
        Group<UsersEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var user = CurrentUser();

        await Send.OkAsync(new GetCurrentUserResponse(
            PlatformPermissions: user.PlatformPermissions,
            AdminPermissions: user.AdminPermissions,
            Id: CurrentUserId,
            Username: user.Username,
            CreatedAt: user.CreatedAt
        ), ct);
    }
}