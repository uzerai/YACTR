using FastEndpoints;
using YACTR.Domain.Model.Authentication;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Users;

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
        await Send.OkAsync((await UserRepository.GetByIdAsync(CurrentUserId, ct))!, ct);
    }
}