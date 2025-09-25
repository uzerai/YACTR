using FastEndpoints;
using YACTR.Data.Model.Authentication;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Users;

public class GetCurrentUser : Endpoint<EmptyRequest, User>
{

    private readonly IEntityRepository<User> _userRepository;

    public GetCurrentUser(IEntityRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public override void Configure()
    {
        Get("/me");
        Group<UsersEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {

        await SendOkAsync();
    }
}