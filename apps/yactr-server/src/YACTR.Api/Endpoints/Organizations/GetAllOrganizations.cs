using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Organizations;

namespace YACTR.Api.Endpoints.Organizations;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GetAllOrganizationsSortBy
{
    [EnumMember(Value = "name")]
    [JsonStringEnumMemberName("name")]
    Name,

    [EnumMember(Value = "created_at")]
    [JsonStringEnumMemberName("created_at")]
    CreatedAt,
}

public class GetAllOrganizationsRequest : SortedPaginationRequest<GetAllOrganizationsSortBy> { }
public record GetAllOrganizationsResponseItem(Guid Id, string Name);

public class GetAllOrganizations(IEntityRepository<Organization> organizationRepository) : AuthenticatedEndpoint<GetAllOrganizationsRequest, PaginatedResponse<GetAllOrganizationsResponseItem>>
{
    private static readonly IReadOnlyDictionary<GetAllOrganizationsSortBy, Expression<Func<Organization, object>>> SortKeySelectors =
        new Dictionary<GetAllOrganizationsSortBy, Expression<Func<Organization, object>>>
        {
            [GetAllOrganizationsSortBy.Name] = e => e.Name,
            [GetAllOrganizationsSortBy.CreatedAt] = e => e.CreatedAt,
        };

    public override void Configure()
    {
        Get("/");
        Group<OrganizationsEndpointGroup>();
    }

    public override async Task HandleAsync(GetAllOrganizationsRequest request, CancellationToken ct)
    {
        var organizations = organizationRepository.All()
            .AsNoTracking()
            .ApplySort(request, SortKeySelectors, GetAllOrganizationsSortBy.Name, SortDirection.Asc, e => e.Id)
            .ToPaginatedResponse(e => new GetAllOrganizationsResponseItem(e.Id, e.Name), request);

        await Send.OkAsync(organizations, ct);
    }
}