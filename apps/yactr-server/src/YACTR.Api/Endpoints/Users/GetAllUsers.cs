using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authentication;
using YACTR.Infrastructure.Authorization.Permissions;
using Permission = YACTR.Domain.Model.Authorization.Permissions.Permission;

namespace YACTR.Api.Endpoints.Users;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GetAllUsersSortBy
{
    [EnumMember(Value = "username")]
    [JsonStringEnumMemberName("username")]
    Username,

    [EnumMember(Value = "created_at")]
    [JsonStringEnumMemberName("created_at")]
    CreatedAt,
}

public class GetAllUsersRequest : SortedPaginationRequest<GetAllUsersSortBy> { }
public record GetAllUsersResponseItem(Guid Id, string Username, Instant CreatedAt);

public class GetAllUsers(IEntityRepository<User> userRepository) : AuthenticatedEndpoint<GetAllUsersRequest, PaginatedResponse<GetAllUsersResponseItem>>
{
    private static readonly IReadOnlyDictionary<GetAllUsersSortBy, Expression<Func<User, object>>> SortKeySelectors =
        new Dictionary<GetAllUsersSortBy, Expression<Func<User, object>>>
        {
            [GetAllUsersSortBy.Username] = e => e.Username,
            [GetAllUsersSortBy.CreatedAt] = e => e.CreatedAt,
        };

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
            .ApplySort(req, SortKeySelectors, GetAllUsersSortBy.CreatedAt, SortDirection.Desc, e => e.Id)
            .ToPaginatedResponse(e => new GetAllUsersResponseItem(e.Id, e.Username, e.CreatedAt), req);

        await Send.OkAsync(allUsers, ct);
    }
}