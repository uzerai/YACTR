using System.Net;
using FastEndpoints;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Organizations;

public record CreateOrganizationTeamUserRequest(Guid OrganizationId, Guid TeamId, Guid UserId, List<Permission> Permissions);

public class CreateOrganizationTeamUser : Endpoint<CreateOrganizationTeamUserRequest, OrganizationTeamUser>
{
    private readonly IRepository<OrganizationTeamUser> _organizationTeamUserRepository;
    private readonly IEntityRepository<OrganizationTeam> _organizationTeamRepository;

    public CreateOrganizationTeamUser(
        IRepository<OrganizationTeamUser> organizationTeamUserRepository,
        IEntityRepository<OrganizationTeam> organizationTeamRepository)
    {
        _organizationTeamUserRepository = organizationTeamUserRepository;
        _organizationTeamRepository = organizationTeamRepository;
    }

    public override void Configure()
    {
        Post("/{TeamId}/users");
        Group<OrganizationTeamsEndpointGroup>();
        // Options(b => b.WithMetadata(new OrganizationPermissionRequiredAttribute(Permission.TeamsWrite)));
    }

    public override async Task HandleAsync(CreateOrganizationTeamUserRequest req, CancellationToken ct)
    {
        var organizationTeamUser = new OrganizationTeamUser
        {
            OrganizationId = req.OrganizationId,
            OrganizationTeamId = req.TeamId,
            UserId = req.UserId,
            Permissions = req.Permissions,
        };

        if (await _organizationTeamRepository.GetByIdAsync(req.TeamId, ct) is null)
        {
            AddError(r => r.TeamId, "Team does not exist");
            await Send.ErrorsAsync((int)HttpStatusCode.FailedDependency, ct);
            return;
        }

        var createdTeamUser = await _organizationTeamUserRepository.CreateAsync(organizationTeamUser, ct);

        await Send.OkAsync(createdTeamUser, cancellation: ct);
    }
}