using Microsoft.EntityFrameworkCore;
using YACTR.Domain.Model.Authorization.Permissions;
using YACTR.Domain.Model.Organizations;
using YACTR.Infrastructure.Authorization.Permissions;
using YACTR.Infrastructure.Database.Repository.Interface;

namespace YACTR.Api.Endpoints.Organizations;

public record GetAllOrganizationTeamsRequest(Guid OrganizationId);

public class GetAllOrganizationTeams(IEntityRepository<OrganizationTeam> organizationTeamRepository)
    : AuthenticatedEndpoint<GetAllOrganizationTeamsRequest, List<OrganizationTeam>>
{
    private readonly IEntityRepository<OrganizationTeam> _organizationTeamRepository = organizationTeamRepository;

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
