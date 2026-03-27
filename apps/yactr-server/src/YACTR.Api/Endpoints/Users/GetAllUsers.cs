using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Domain.Model.Authentication;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;
using Permission = YACTR.Domain.Model.Authorization.Permissions.Permission;

namespace YACTR.Api.Endpoints.Users;

public class GetAllUsersRequest : PaginationRequest {}

public class GetAllUsers(IEntityRepository<User> userRepository) : AuthenticatedEndpoint<GetAllUsersRequest, PaginatedResponse<UserResponse>>
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
            .ToPaginatedResponse(e => new UserResponse(e.Id, e.Username), req);

        await Send.OkAsync(allUsers, ct);
    }
}