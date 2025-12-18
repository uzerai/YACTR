using FastEndpoints;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Organizations;

public class GetAllOrganizations : Endpoint<EmptyRequest, List<Organization>>
{
    private readonly IEntityRepository<Organization> _organizationRepository;

    public override void Configure()
    {
        Get("/");
        Group<OrganizationsEndpointGroup>();
    }

    public GetAllOrganizations(IEntityRepository<Organization> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    public override async Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        await Send.OkAsync([.. await _organizationRepository.GetAllAsync(ct)], cancellation: ct);
    }
}