using FastEndpoints;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Organizations;

public record GetOrganizationByIdRequest(Guid OrganizationId);

public class GetOrganizationById : Endpoint<GetOrganizationByIdRequest, Organization>
{

    private readonly IEntityRepository<Organization> _organizationRepository;

    public GetOrganizationById(IEntityRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public override void Configure()
    {
        Get("/{OrganizationId}");
        Group<OrganizationsEndpointGroup>();
    }

    public override async Task HandleAsync(GetOrganizationByIdRequest req, CancellationToken ct)
    {
        var organization = await _organizationRepository.GetByIdAsync(req.OrganizationId);

        if (organization is not null)
        {
            await SendAsync(organization, cancellation: ct);
            return;
        }

        await SendNotFoundAsync();
    }
}