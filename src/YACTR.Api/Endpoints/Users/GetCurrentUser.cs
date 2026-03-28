using FastEndpoints;
using YACTR.Domain.Model.Authentication;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Users;

public class GetCurrentUser : AuthenticatedEndpoint<EmptyRequest, CurrentUserResponse>
{
    public required IEntityRepository<User> UserRepository { get; init; }

    public override void Configure()
    {
        Get("/me");
        Group<UsersEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var user = await UserRepository.GetByIdAsync(CurrentUserId, ct);

        if (user == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new CurrentUserResponse(
            PlatformPermissions: user.PlatformPermissions,
            AdminPermissions: user.AdminPermissions,
            Id: CurrentUserId,
            Username: user.Username,
            CreatedAt: user.CreatedAt
        ), ct);
    }
}