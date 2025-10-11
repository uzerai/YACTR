using FastEndpoints;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;

namespace YACTR.Endpoints.Organizations.Teams;

public record CreateOrganizationTeamRequest(Guid OrganizationId, string Name);

public class CreateOrganizationTeam : Endpoint<CreateOrganizationTeamRequest, OrganizationTeam>
{
    private readonly IEntityRepository<OrganizationTeam> _organizationTeamRepository;

    public CreateOrganizationTeam(IEntityRepository<OrganizationTeam> organizationTeamRepository)
    {
        _organizationTeamRepository = organizationTeamRepository;
    }

    public override void Configure()
    {
        Post("/");
        Group<OrganizationTeamsEndpointGroup>();
        // Options(b => b.WithMetadata(new OrganizationPermissionRequiredAttribute(Permission.TeamsWrite)));
    }

    public override async Task HandleAsync(CreateOrganizationTeamRequest req, CancellationToken ct)
    {
        var team = new OrganizationTeam
        {
            Name = req.Name,
            OrganizationId = req.OrganizationId,
        };

        var createdTeam = await _organizationTeamRepository.CreateAsync(team, ct);

        await SendAsync(createdTeam, cancellation: ct);
    }
}
