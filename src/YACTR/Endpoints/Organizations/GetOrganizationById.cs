using FastEndpoints;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Organizations;

public record GetOrganizationByIdRequest(Guid OrganizationId);

public class GetOrganizationById(IEntityRepository<Organization> organizationRepository) : AuthenticatedEndpoint<GetOrganizationByIdRequest, Organization>
{
    public override void Configure()
    {
        Get("/{OrganizationId}");
        Group<OrganizationsEndpointGroup>();
    }

    public override async Task HandleAsync(GetOrganizationByIdRequest req, CancellationToken ct)
    {
        var organization = await organizationRepository.GetByIdAsync(req.OrganizationId, ct);

        if (organization is not null)
        {
            await Send.OkAsync(organization, cancellation: ct);
            return;
        }

        await Send.NotFoundAsync(ct);
    }
}