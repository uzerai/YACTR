using Microsoft.EntityFrameworkCore;
using NodaTime;

using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authentication;
using YACTR.Infrastructure.Authorization.Permissions;

using Permission = YACTR.Domain.Model.Authorization.Permissions.Permission;

namespace YACTR.Api.Endpoints.Users;

public class GetAllUsersRequest : PaginationRequest { }
public record GetAllUsersResponseItem(Guid Id, string Username, Instant CreatedAt);

public class GetAllUsers(IEntityRepository<User> userRepository) : AuthenticatedEndpoint<GetAllUsersRequest, PaginatedResponse<GetAllUsersResponseItem>>
{
    public override void Configure()
    {
        Get("/");
        Group<UsersEndpointGroup>();
        Options(b => b.WithMetadata(new AdminPermissionRequiredAttribute(Permission.UsersRead)));
    }

    public override async Task HandleAsync(GetAllUsersRequest req, CancellationToken ct)
    {
        var allUsers = userRepository.All()
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToPaginatedResponse(e => new GetAllUsersResponseItem(e.Id, e.Username, e.CreatedAt), req);

        await Send.OkAsync(allUsers, ct);
    }
}