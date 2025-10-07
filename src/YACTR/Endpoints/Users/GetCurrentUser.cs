using FastEndpoints;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Users;

public class GetCurrentUser : AuthenticatedEndpoint<EmptyRequest, User>
{
    public required IEntityRepository<User> UserRepository { get; init; }

    public override void Configure()
    {
        Get("/me");
        Group<UsersEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        await SendOkAsync((await UserRepository.GetByIdAsync(CurrentUserId, ct))!, ct);
    }
}