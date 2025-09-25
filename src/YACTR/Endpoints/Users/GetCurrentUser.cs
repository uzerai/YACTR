using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
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
        if (!Guid.TryParse(HttpContext.User.ClaimValue(ClaimTypes.Sid), out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        await SendOkAsync((await _userRepository.GetByIdAsync(userId))!, ct);
    }
}