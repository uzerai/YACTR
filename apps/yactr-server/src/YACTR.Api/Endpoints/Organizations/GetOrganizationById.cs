using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Organizations;

namespace YACTR.Api.Endpoints.Organizations;

public record GetOrganizationByIdRequest(Guid OrganizationId);
public record GetOrganizationByIdResponse(Guid Id, string Name);

public class GetOrganizationById(IEntityRepository<Organization> organizationRepository) : AuthenticatedEndpoint<GetOrganizationByIdRequest, GetOrganizationByIdResponse>
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

        await Send.OkAsync(new GetOrganizationByIdResponse(organization.Id, organization.Name), cancellation: ct);
    }
}