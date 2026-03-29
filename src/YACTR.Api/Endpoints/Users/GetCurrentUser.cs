using FastEndpoints;

namespace YACTR.Api.Endpoints.Users;

public class GetCurrentUser : AuthenticatedEndpoint<EmptyRequest, CurrentUserResponse>
{
    public override void Configure()
    {
        Get("/me");
        Group<UsersEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var user = CurrentUser();

        await Send.OkAsync(new CurrentUserResponse(
            PlatformPermissions: user.PlatformPermissions,
            AdminPermissions: user.AdminPermissions,
            Id: CurrentUserId,
            Username: user.Username,
            CreatedAt: user.CreatedAt
        ), ct);
    }
}