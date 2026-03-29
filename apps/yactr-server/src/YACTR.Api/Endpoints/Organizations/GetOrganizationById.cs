using YACTR.Domain.Model.Organizations;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Organizations;

public record GetOrganizationByIdRequest(Guid OrganizationId);

public class GetOrganizationById(IEntityRepository<Organization> organizationRepository) : AuthenticatedEndpoint<GetOrganizationByIdRequest, OrganizationResponse>
{
    public override void Configure()
    {
        Get("/{organization_id}");
        Group<OrganizationsEndpointGroup>();
    }

    public override async Task HandleAsync(GetOrganizationByIdRequest req, CancellationToken ct)
    {
        var organization = await organizationRepository.GetByIdAsync(req.OrganizationId, ct);

        if (organization is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(new OrganizationResponse(organization.Id, organization.Name), cancellation: ct);
    }
}