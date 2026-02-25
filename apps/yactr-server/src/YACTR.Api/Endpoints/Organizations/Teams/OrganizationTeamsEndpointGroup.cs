using FastEndpoints;

namespace YACTR.Api.Endpoints.Organizations;

public class OrganizationTeamsEndpointGroup : SubGroup<OrganizationsEndpointGroup>
{
    public OrganizationTeamsEndpointGroup()
    {
        Configure("/{OrganizationId}/teams", ep => { });
    }
}