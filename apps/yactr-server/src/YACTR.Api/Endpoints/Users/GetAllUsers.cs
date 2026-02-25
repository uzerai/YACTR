using FastEndpoints;
using YACTR.Domain.Model.Authentication;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;
using Permission = YACTR.Domain.Model.Authorization.Permissions.Permission;

namespace YACTR.Api.Endpoints.Users;

public class GetAllUsers(IEntityRepository<User> userRepository) : AuthenticatedEndpoint<EmptyRequest, IEnumerable<User>>
{
    public override void Configure()
    {
        Get("/");
        Group<UsersEndpointGroup>();
        Options(b => b.WithMetadata(new AdminPermissionRequiredAttribute(Permission.UsersRead)));
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        var allUsers = await userRepository.GetAllAsync(ct);

        await Send.OkAsync(allUsers, ct);
    }
}