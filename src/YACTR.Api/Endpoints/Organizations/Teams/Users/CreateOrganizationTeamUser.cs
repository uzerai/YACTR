using System.Net;
using FastEndpoints;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Organizations;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;
using Void = FastEndpoints.Void;

namespace YACTR.Api.Endpoints.Organizations;

public record CreateOrganizationTeamUserRequest(Guid OrganizationId, Guid TeamId, Guid UserId, List<Permission> Permissions);
public record CreateOrganizationTeamUserResponse(Guid OrganizationId, Guid TeamId, Guid UserId);

public class CreateOrganizationTeamUser(
    IRepository<OrganizationUser> _organizationUserRepository,
    IRepository<OrganizationTeamUser> _organizationTeamUserRepository, 
    IEntityRepository<OrganizationTeam> _organizationTeamRepository) : AuthenticatedEndpoint<CreateOrganizationTeamUserRequest, CreateOrganizationTeamUserResponse>
{
    public override void Configure()
    {
        Post("/{TeamId}/users");
        Group<OrganizationTeamsEndpointGroup>();
        Options(b => b.WithMetadata(new OrganizationPermissionRequiredAttribute(Permission.TeamsWrite)));
    }

    public override async Task<Void> HandleAsync(CreateOrganizationTeamUserRequest req, CancellationToken ct)
    {
        var organizationTeamUser = new OrganizationTeamUser
        {
            OrganizationId = req.OrganizationId,
            OrganizationTeamId = req.TeamId,
            UserId = req.UserId,
            Permissions = req.Permissions,
        };

        var organizationTeam = await _organizationTeamRepository.GetByIdAsync(req.TeamId, ct);
        if (organizationTeam is null)
        {
            AddError(r => r.TeamId, "Team does not exist");
            return await Send.ErrorsAsync((int)HttpStatusCode.FailedDependency, ct);
        }
        var organizationUser = await _organizationUserRepository.CreateAsync(new()
        {
            OrganizationId = organizationTeam.OrganizationId,
            UserId = req.UserId
        }, ct);

        var createdTeamUser = await _organizationTeamUserRepository.CreateAsync(organizationTeamUser, ct);

        return await Send.OkAsync(new (organizationTeam.OrganizationId, organizationTeam.Id, createdTeamUser.UserId), cancellation: ct);
    }
}