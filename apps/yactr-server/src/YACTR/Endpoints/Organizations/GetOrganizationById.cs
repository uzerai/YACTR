using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Organizations;

public record GetOrganizationByIdRequest(Guid Id);

public class GetOrganizationById : Endpoint<GetOrganizationByIdRequest, Organization>
{

  private readonly IEntityRepository<Organization> _organizationRepository;

  public GetOrganizationById(IEntityRepository<Organization> organizationRepository)
  {
    _organizationRepository = organizationRepository;
  }

  public override void Configure()
  {
    Get("/{Id}");
    Group<OrganizationsEndpointGroup>();
    Options(b => b.WithMetadata(new OrganizationPermissionRequiredAttribute(Permission.OrganizationsRead)));
  }

  public override async Task HandleAsync(GetOrganizationByIdRequest req, CancellationToken ct)
  {
    var organization = await _organizationRepository.GetByIdAsync(req.Id);

    if (organization is not null)
    {
      await SendAsync(organization, cancellation: ct);
      return;
    }

    await SendNotFoundAsync();
  }
}