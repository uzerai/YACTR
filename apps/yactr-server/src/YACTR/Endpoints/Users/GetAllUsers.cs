using FastEndpoints;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;
using Permission = YACTR.Data.Model.Authorization.Permissions.Permission;

namespace YACTR.Endpoints.Users;

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