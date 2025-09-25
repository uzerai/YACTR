using FastEndpoints;

namespace YACTR.Endpoints.Organizations;

public class OrganizationTeamsEndpointGroup : SubGroup<OrganizationsEndpointGroup>
{
    public OrganizationTeamsEndpointGroup()
    {
        Configure("/{OrganizationId}/teams", ep => { });
    }
}