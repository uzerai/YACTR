using Microsoft.EntityFrameworkCore;
using YACTR.Data.Model.Authorization.Permissions;
using YACTR.Data.Model.Organizations;
using YACTR.Data.Repository.Interface;
using YACTR.DI.Authorization.Permissions;

namespace YACTR.Endpoints.Organizations;

public record GetAllOrganizationTeamsRequest(Guid OrganizationId);

public class GetAllOrganizationTeams : AuthenticatedEndpoint<GetAllOrganizationTeamsRequest, List<OrganizationTeam>>
{
    private readonly IEntityRepository<OrganizationTeam> _organizationTeamRepository;

    public GetAllOrganizationTeams(IEntityRepository<OrganizationTeam> organizationTeamRepository)
    {
        _organizationTeamRepository = organizationTeamRepository;
    }

    public override void Configure()
    {
        Get("/");
        Group<OrganizationTeamsEndpointGroup>();
        Options(b => b.WithMetadata(new OrganizationPermissionRequiredAttribute(Permission.TeamsRead)));
    }

    public override async Task HandleAsync(GetAllOrganizationTeamsRequest req, CancellationToken ct)
    {
        var teams = await _organizationTeamRepository.BuildReadonlyQuery()
            .Where(e => e.OrganizationId == req.OrganizationId)
            .ToListAsync(ct);

        await Send.OkAsync(teams, cancellation: ct);
    }
}
