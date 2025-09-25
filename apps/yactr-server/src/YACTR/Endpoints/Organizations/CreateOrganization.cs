using System.Security.Claims;
using FastEndpoints;
using FastEndpoints.Security;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Organizations;

public record CreateOrganizationRequestData(string Name);

public class CreateOrganization : Endpoint<CreateOrganizationRequestData, Organization>
{

    private readonly IEntityRepository<Organization> _organizationRepository;
    private readonly IRepository<OrganizationUser> _organizationUserRepository;

    public CreateOrganization(IEntityRepository<Organization> organizationRepository, IRepository<OrganizationUser> organizationUserRepository)
    {
        _organizationRepository = organizationRepository;
        _organizationUserRepository = organizationUserRepository;
    }

    public override void Configure()
    {
        Post("/");
        Group<OrganizationsEndpointGroup>();
        Options(b => b.WithMetadata(new PlatformPermissionRequiredAttribute(Permission.OrganizationsWrite)));
    }

    public override async Task HandleAsync(CreateOrganizationRequestData req, CancellationToken ct)
    {
        if (!Guid.TryParse(HttpContext.User.ClaimValue(ClaimTypes.Sid), out Guid userId))
        {
            await SendUnauthorizedAsync(ct);
            return;
        }

        var newOrganization = new Organization
        {
            Name = req.Name
        };

        var createdOrganization = await _organizationRepository.CreateAsync(newOrganization, ct);

        Logger.LogInformation("Creating organizationUser for {UserId} in organization {OrganizationId}", userId, createdOrganization.Id);
        var organizationUser = new OrganizationUser
        {
            OrganizationId = createdOrganization.Id,
            UserId = userId,
            Permissions = Enum.GetValues<Permission>()
        };

        await _organizationUserRepository.CreateAsync(organizationUser, ct);

        await SendCreatedAtAsync<GetOrganizationById>(createdOrganization.Id, createdOrganization, cancellation: ct);
    }
}