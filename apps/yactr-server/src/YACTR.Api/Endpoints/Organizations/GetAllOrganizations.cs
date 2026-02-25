using FastEndpoints;
using YACTR.Domain.Model.Organizations;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Organizations;

public class GetAllOrganizations(IEntityRepository<Organization> organizationRepository) : AuthenticatedEndpoint<EmptyRequest, List<Organization>>
{
    public override void Configure()
    {
        Get("/");
        Group<OrganizationsEndpointGroup>();
    }

    public override async Task HandleAsync(EmptyRequest request, CancellationToken ct)
    {
        await Send.OkAsync([.. await organizationRepository.GetAllAsync(ct)], ct);
    }
}