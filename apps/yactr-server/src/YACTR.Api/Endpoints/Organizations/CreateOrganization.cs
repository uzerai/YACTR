using System.Security.Claims;
using FastEndpoints.Security;
using YACTR.Domain.Interface.Repository;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Organizations;
using YACTR.Infrastructure.Authorization.Permissions;

namespace YACTR.Api.Endpoints.Organizations;

public record CreateOrganizationRequestData(string Name);
public record CreateOrganizationResponse(Guid Id, string Name);

public class CreateOrganization(
    IEntityRepository<Organization> organizationRepository,
    IRepository<OrganizationUser> organizationUserRepository)
    : AuthenticatedEndpoint<CreateOrganizationRequestData, CreateOrganizationResponse>
{
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
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var newOrganization = new Organization
        {
            Name = req.Name
        };

        var createdOrganization = await organizationRepository.CreateAsync(newOrganization, ct);

        Logger.LogInformation("Creating organizationUser for {UserId} in organization {OrganizationId}", userId, createdOrganization.Id);
        var organizationUser = new OrganizationUser
        {
            OrganizationId = createdOrganization.Id,
            UserId = userId,
            Permissions = Enum.GetValues<Permission>()
        };

        await organizationUserRepository.CreateAsync(organizationUser, ct);

        await Send.CreatedAtAsync<GetOrganizationById>(createdOrganization.Id, new CreateOrganizationResponse(createdOrganization.Id, createdOrganization.Name), cancellation: ct);
    }
}