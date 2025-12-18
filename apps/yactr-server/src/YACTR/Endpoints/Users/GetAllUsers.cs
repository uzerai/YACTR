using FastEndpoints;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;
using Permission = YACTR.Data.Model.Authorization.Permissions.Permission;

namespace YACTR.Endpoints.Users;

[AdminPermissionRequired(Permission.UsersRead)]
public class GetAllUsers : Endpoint<EmptyRequest, IEnumerable<User>>
{
    private readonly IEntityRepository<User> _userRepository;

    public GetAllUsers(IEntityRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public override void Configure()
    {
        Get("/");
        Group<UsersEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        await Send.OkAsync((await _userRepository.GetAllAsync(ct))!, ct);
    }
}