using FastEndpoints;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Organizations;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Organizations.Teams;

public record CreateOrganizationTeamRequest(Guid OrganizationId, string Name);

public class CreateOrganizationTeam(IEntityRepository<OrganizationTeam> organizationTeamRepository)
    : AuthenticatedEndpoint<CreateOrganizationTeamRequest, OrganizationTeam>
{
    public override void Configure()
    {
        Post("/");
        Group<OrganizationTeamsEndpointGroup>();
        Options(b => b.WithMetadata(new OrganizationPermissionRequiredAttribute(Permission.TeamsWrite)));
    }

    public override async Task HandleAsync(CreateOrganizationTeamRequest req, CancellationToken ct)
    {
        var team = new OrganizationTeam
        {
            Name = req.Name,
            OrganizationId = req.OrganizationId,
        };

        var createdTeam = await organizationTeamRepository.CreateAsync(team, ct);

        await Send.OkAsync(createdTeam, cancellation: ct);
    }
}
