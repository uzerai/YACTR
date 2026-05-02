using Microsoft.EntityFrameworkCore;
using YACTR.Api.Pagination;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Organizations;

namespace YACTR.Api.Endpoints.Organizations;

public class GetAllOrganizationsRequest : PaginationRequest { }

public class GetAllOrganizations(IEntityRepository<Organization> organizationRepository) : AuthenticatedEndpoint<GetAllOrganizationsRequest, PaginatedResponse<OrganizationResponse>>
{
    public override void Configure()
    {
        Get("/");
        Group<OrganizationsEndpointGroup>();
    }

    public override async Task HandleAsync(GetAllOrganizationsRequest request, CancellationToken ct)
    {
        var organizations = organizationRepository.All()
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToPaginatedResponse(e => new OrganizationResponse(e.Id, e.Name), request);

        await Send.OkAsync(organizations, ct);
    }
}