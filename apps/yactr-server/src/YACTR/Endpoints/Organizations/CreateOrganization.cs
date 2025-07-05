using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.UserContext;

namespace YACTR.Endpoints.Organizations;

public record CreateOrganizationRequestData(string Name);

public class CreateOrganization : Endpoint<CreateOrganizationRequestData, Organization>
{

  private readonly IEntityRepository<Organization> _organizationRepository;
  private readonly IRepository<OrganizationUser> _organizationUserRepository;
  private readonly IUserContext _userContext;

  public CreateOrganization(IEntityRepository<Organization> organizationRepository, IRepository<OrganizationUser> organizationUserRepository, IUserContext userContext)
  {
    _organizationRepository = organizationRepository;
    _organizationUserRepository = organizationUserRepository;
    _userContext = userContext;
  }

  public override void Configure()
  {
    Post("/");
    Group<OrganizationsEndpointGroup>();
    // Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.OrganizationsWrite)));
  }

  public override async Task HandleAsync(CreateOrganizationRequestData req, CancellationToken ct)
  {
    var newOrganization = new Organization
    {
      Name = req.Name
    };

    var createdOrganization = await _organizationRepository.CreateAsync(newOrganization, ct);
    var organizationUser = new OrganizationUser
    {
      OrganizationId = createdOrganization.Id,
      UserId = _userContext.CurrentUser!.Id,
      Permissions = Enum.GetValues<Permission>()
            .Select(permission => new OrganizationPermission
            {
              Permission = permission
            }).ToList()
    };

    await _organizationUserRepository.CreateAsync(organizationUser, ct);

    await SendCreatedAtAsync<GetOrganizationById>(createdOrganization.Id, createdOrganization, cancellation: ct);
  }
}