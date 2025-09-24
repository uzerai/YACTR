using FastEndpoints;
using YACTR.Data.Model.Authentication;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.Endpoints.Users;

public class GetCurrentUser : Endpoint<EmptyRequest, User>
{
    private readonly IUserContext _userContext;

    public GetCurrentUser(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override void Configure()
    {
        Get("/me");
        Group<UsersEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest req, CancellationToken ct)
    {
        await SendAsync(_userContext.CurrentUser!, cancellation: ct);
    }
} 