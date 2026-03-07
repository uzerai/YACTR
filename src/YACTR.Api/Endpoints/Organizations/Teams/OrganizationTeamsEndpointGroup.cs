using FastEndpoints;

namespace YACTR.Api.Endpoints.Organizations;

public record OrganizationTeamResponse(
    Guid Id,
    string Name
);

public class OrganizationTeamsEndpointGroup : SubGroup<OrganizationsEndpointGroup>
{
    public OrganizationTeamsEndpointGroup()
    {
        Configure("/{OrganizationId}/teams", ep => { });
    }
}