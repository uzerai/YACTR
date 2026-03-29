using FastEndpoints;

namespace YACTR.Api.Endpoints.Organizations;

public record OrganizationTeamResponse(
    Guid Id,
    Guid OrganizationId,
    string Name
);

public class OrganizationTeamsEndpointGroup : SubGroup<OrganizationsEndpointGroup>
{
    public OrganizationTeamsEndpointGroup()
    {
        Configure("/{organization_id}/teams", ep => { });
    }
}